using System.Text;
using System.Text.Json;
using BE.League.Desktop.Connection;
using BE.League.Desktop.GameClientApi;
using BE.League.Desktop.LeagueClientApi;
using BE.League.Desktop.Models;

namespace BE.League.Desktop.WinFormsShowcase;

/// <summary>
/// WinForms showcase for BE.League.Desktop.
/// Demonstrates the League Client API (LCU) and the Game Client API (port 2999).
/// Official docs: https://developer.riotgames.com/docs/lol
/// </summary>
public sealed class MainForm : Form
{
    // ──────────────────────────── CLIENTS ─────────────────────────────────
    private LeagueClientApiReader? _leagueClient;
    private readonly GameClientApiReader _gameClient = new();

    // ──────────────────────────── TIMER ───────────────────────────────────
    private readonly System.Windows.Forms.Timer _refreshTimer = new() { Interval = 3000 };

    // ──────────────────────────── STATE TRACKING (transition-based nav) ───
    private bool _prevInGame;
    private bool _prevLobbyActive;
    private bool _prevRcActive;
    private bool _prevCsActive;

    // ──────────────────────────── TOOLSTRIP ───────────────────────────────
    private readonly ToolStrip _toolStrip = new();
    private readonly ToolStripButton _btnRefresh = new()
        { Text = "⟳  Refresh", ToolTipText = "Refresh all data now" };
    private readonly ToolStripButton _btnAutoToggle = new()
        { CheckOnClick = true, Checked = true, Text = "⏱  Auto: ON", ToolTipText = "Toggle auto-refresh (3 s)" };
    private readonly ToolStripButton _btnAccept = new()
        { Text = "✔  Accept Ready Check", ForeColor = Color.DarkGreen, Enabled = false,
          ToolTipText = "POST /lol-matchmaking/v1/ready-check/accept" };
    private readonly ToolStripButton _btnDecline = new()
        { Text = "✘  Decline Ready Check", ForeColor = Color.DarkRed, Enabled = false,
          ToolTipText = "POST /lol-matchmaking/v1/ready-check/decline" };

    // ──────────────────────────── STATUSSTRIP ─────────────────────────────
    private readonly StatusStrip _statusStrip = new();
    private readonly ToolStripStatusLabel _sslLeague = new()
        { Text = "League Client: —", BorderSides = ToolStripStatusLabelBorderSides.Right };
    private readonly ToolStripStatusLabel _sslGame = new()
        { Text = "Game Client: —", BorderSides = ToolStripStatusLabelBorderSides.Right };
    private readonly ToolStripStatusLabel _sslTime = new()
        { Spring = true, TextAlign = ContentAlignment.MiddleRight };

    // ──────────────────────────── TABS ────────────────────────────────────
    private readonly TabControl _tabs = new() { Dock = DockStyle.Fill };

    // --- Status tab ---
    private readonly Label _stLeagueState = V(), _stLeaguePort = V(), _stLeagueToken = V();
    private readonly Label _stGameState = V(), _stGamePlayer = V(), _stGameMode = V();

    // --- League Client tab (inner sub-tabs) ---
    private readonly TabControl _lcSubTabs = new() { Dock = DockStyle.Fill };
    // Lobby & Party
    private readonly Label _lcMode = V(), _lcQueue = V(), _lcMap = V(), _lcType = V(), _lcStart = V();
    private readonly Label _lcPartyId = V(), _lcScarcePos = V();
    private readonly DataGridView _dgvMembers = new();
    private readonly DataGridView _dgvInvitations = new();
    // Ready Check
    private readonly Label _rcState = V(), _rcTimer = V(), _rcResponse = V(), _rcDodge = V();
    // Pick & Ban
    private readonly Label _csPhase = V(), _csCell = V();
    private readonly Label _lcsCurrentAction = new()
        { AutoSize = true, Font = new Font("Segoe UI", 10f, FontStyle.Bold), ForeColor = Color.Navy };
    private readonly DataGridView _dgvCsTeam = new();
    private readonly DataGridView _dgvCsEnemy = new();
    private readonly DataGridView _dgvCsBans = new();

    // --- Game Client tab ---
    private readonly Label _gcMode = V(), _gcMap = V(), _gcTerrain = V();
    private readonly Label _gcTimeLabel = new() { AutoSize = true, Font = new Font("Segoe UI", 14f, FontStyle.Bold) };
    private readonly Label _apName = V(), _apLevel = V(), _apGold = V();
    private readonly Label _apHP = V(), _apRes = V(), _apResType = V();
    private readonly Label _apAD = V(), _apAP = V(), _apArmor = V(), _apMR = V();
    private readonly Label _apMS = V(), _apCrit = V(), _apAS = V();
    private readonly Label _apKeystone = V(), _apPrimTree = V(), _apSecTree = V();
    private readonly Label _apAbilities = new()
        { AutoSize = false, Dock = DockStyle.Fill, TextAlign = ContentAlignment.TopLeft };
    private readonly DataGridView _dgvPlayers = new();
    private readonly ListBox _lstEvents = new()
        { HorizontalScrollbar = true, Font = new Font("Consolas", 8f) };

    // --- Raw JSON tab ---
    private readonly ComboBox _cmbEndpoint = new() { DropDownStyle = ComboBoxStyle.DropDownList };
    private readonly Button _btnFetch = new() { Text = "Fetch", AutoSize = true };
    private readonly Button _btnCopy = new() { Text = "Copy", AutoSize = true };
    private readonly RichTextBox _rtbJson = new()
    {
        ReadOnly = true, Font = new Font("Consolas", 9f), Dock = DockStyle.Fill,
        BackColor = Color.FromArgb(28, 28, 28), ForeColor = Color.FromArgb(215, 215, 170)
    };

    private static Label V() => new() { AutoSize = true };

    // ═══════════════════════════════════ CONSTRUCTOR ═══════════════════════════════════
    public MainForm()
    {
        SuspendLayout();
        BuildUi();
        ResumeLayout(true);

        _refreshTimer.Tick += async (_, _) => await RefreshAllAsync();
        _btnRefresh.Click += async (_, _) => await RefreshAllAsync();
        _btnAutoToggle.CheckedChanged += (_, _) =>
        {
            _refreshTimer.Enabled = _btnAutoToggle.Checked;
            _btnAutoToggle.Text = _btnAutoToggle.Checked ? "⏱  Auto: ON" : "⏱  Auto: OFF";
        };
        _btnAccept.Click += async (_, _) => await ActionReadyCheckAsync(accept: true);
        _btnDecline.Click += async (_, _) => await ActionReadyCheckAsync(accept: false);
        _btnFetch.Click += async (_, _) => await FetchRawJsonAsync();
        _btnCopy.Click += (_, _) => { if (!string.IsNullOrEmpty(_rtbJson.Text)) Clipboard.SetText(_rtbJson.Text); };

        _refreshTimer.Start();
        _ = RefreshAllAsync();
    }

    // ═══════════════════════════════════ UI BUILDER ════════════════════════════════════
    private void BuildUi()
    {
        Text = "League of Legends API Showcase — BE.League.Desktop";
        Size = new Size(1280, 900);
        MinimumSize = new Size(1000, 700);
        StartPosition = FormStartPosition.CenterScreen;

        // ToolStrip
        _toolStrip.Items.AddRange([
            _btnRefresh,
            _btnAutoToggle,
            new ToolStripSeparator(),
            _btnAccept,
            _btnDecline,
            new ToolStripSeparator(),
            new ToolStripLabel("  Riot Dev Docs: https://developer.riotgames.com/docs/lol")
                { ForeColor = Color.Gray }
        ]);

        // StatusStrip
        _statusStrip.Items.AddRange([_sslLeague, _sslGame, _sslTime]);

        // Tabs
        _tabs.TabPages.Add(BuildStatusTab());
        _tabs.TabPages.Add(BuildLeagueClientTab());
        _tabs.TabPages.Add(BuildGameClientTab());
        _tabs.TabPages.Add(BuildRawJsonTab());

        Controls.AddRange([(Control)_toolStrip, _tabs, _statusStrip]);
    }

    // ────────────────────────────── TAB 1: STATUS ─────────────────────────────────────
    private TabPage BuildStatusTab()
    {
        var page = Tab("🟢  Dashboard");
        var outer = new TableLayoutPanel
        {
            Dock = DockStyle.Fill, Padding = new Padding(10),
            ColumnCount = 2, RowCount = 2
        };
        outer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        outer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        outer.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        outer.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        // League Client group
        var gbLeague = Group("🖥️  League Client API  (Dynamic Port + Basic Auth)");
        gbLeague.Controls.Add(InfoGrid(
            ("Status", _stLeagueState),
            ("Port", _stLeaguePort),
            ("Auth Token", _stLeagueToken)));
        outer.Controls.Add(gbLeague, 0, 0);

        // Game Client group
        var gbGame = Group("🎮  Game Client API  (Port 2999 · No Auth)");
        gbGame.Controls.Add(InfoGrid(
            ("Status", _stGameState),
            ("Active Player", _stGamePlayer),
            ("Game Mode", _stGameMode)));
        outer.Controls.Add(gbGame, 1, 0);

        // Info panel
        var info = new RichTextBox
        {
            ReadOnly = true, Dock = DockStyle.Fill, BorderStyle = BorderStyle.None,
            Font = new Font("Segoe UI", 9f), BackColor = SystemColors.Control,
            Text =
                "League Client API (LCU)\r\n" +
                "  • Runs on a dynamic port discovered from the lockfile\r\n" +
                "  • Requires Basic Auth: Authorization: Basic base64(riot:<token>)\r\n" +
                "  • Available while the League of Legends desktop client is running\r\n" +
                "  • Docs: https://developer.riotgames.com/docs/lol#league-client-api\r\n" +
                "  • Community: https://hextechdocs.dev/\r\n\r\n" +
                "Game Client API\r\n" +
                "  • Fixed port 2999 — no authentication required\r\n" +
                "  • Available only while an active game is running\r\n" +
                "  • Docs: https://developer.riotgames.com/docs/lol#game-client-api\r\n" +
                "  • OpenAPI spec: https://static.developer.riotgames.com/docs/lol/liveclientdata_sample.json"
        };
        outer.SetColumnSpan(info, 2);
        outer.Controls.Add(info, 0, 1);

        page.Controls.Add(outer);
        return page;
    }

    // ─────────────────────────── TAB 2: LEAGUE CLIENT ────────────────────────────────
    private TabPage BuildLeagueClientTab()
    {
        var page = Tab("🖥️  League Client API");

        SetupDgv(_dgvMembers, [
            ("#", 30), ("Summoner", 160), ("Level", 50),
            ("Role 1", 80), ("Role 2", 80), ("Leader", 50), ("Ready", 50), ("Subteam", 65)]);
        SetupDgv(_dgvInvitations, [("To Summoner", 160), ("State", 110), ("Type", 90)]);
        SetupDgv(_dgvCsTeam,  [("Cell", 40), ("Champ ID", 85), ("Intent", 85), ("Spell 1", 70), ("Spell 2", 70)]);
        SetupDgv(_dgvCsEnemy, [("Cell", 40), ("Champ ID", 85), ("Intent", 85), ("Spell 1", 70), ("Spell 2", 70)]);
        SetupDgv(_dgvCsBans,  [("Turn", 40), ("Cell", 45), ("Champion ID", 100), ("Status", 110)]);

        _lcSubTabs.TabPages.Add(BuildLobbySubTab());
        _lcSubTabs.TabPages.Add(BuildReadyCheckSubTab());
        _lcSubTabs.TabPages.Add(BuildPickBanSubTab());

        page.Controls.Add(_lcSubTabs);
        return page;
    }

    // ── Sub-tab A: Lobby & Party ──────────────────────────────────────────────────────
    private TabPage BuildLobbySubTab()
    {
        var page = Tab("🏠  Lobby & Party");

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill, Padding = new Padding(6),
            RowCount = 3, ColumnCount = 1
        };
        layout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 60));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 40));

        // Lobby config info
        var gbConfig = Group("Lobby Config  ·  GET /lol-lobby/v2/lobby");
        gbConfig.Controls.Add(InfoGrid(
            ("Game Mode",        _lcMode),
            ("Queue ID",         _lcQueue),
            ("Map ID",           _lcMap),
            ("Party Type",       _lcType),
            ("Can Start",        _lcStart),
            ("Party ID",         _lcPartyId),
            ("Scarce Positions", _lcScarcePos)));
        layout.Controls.Add(gbConfig, 0, 0);

        // Members
        var gbMembers = Group("Party Members  (lobby.members — leader ★ gold · you 🔵 blue)");
        _dgvMembers.Dock = DockStyle.Fill;
        gbMembers.Controls.Add(_dgvMembers);
        layout.Controls.Add(gbMembers, 0, 1);

        // Invitations
        var gbInvites = Group("Invitations  (lobby.invitations — pending/accepted/declined)");
        _dgvInvitations.Dock = DockStyle.Fill;
        gbInvites.Controls.Add(_dgvInvitations);
        layout.Controls.Add(gbInvites, 0, 2);

        page.Controls.Add(layout);
        return page;
    }

    // ── Sub-tab B: Ready Check ────────────────────────────────────────────────────────
    private TabPage BuildReadyCheckSubTab()
    {
        var page = Tab("⚡  Ready Check");

        var outer = new TableLayoutPanel
        {
            Dock = DockStyle.Fill, Padding = new Padding(12),
            RowCount = 1, ColumnCount = 1
        };
        outer.RowStyles.Add(new RowStyle(SizeType.AutoSize));

        var gbRc = Group("Ready Check  ·  GET /lol-matchmaking/v1/ready-check");
        var rcLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill, RowCount = 2, ColumnCount = 1
        };
        rcLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        rcLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));

        rcLayout.Controls.Add(InfoGrid(
            ("State",         _rcState),
            ("Your Response", _rcResponse),
            ("Timer",         _rcTimer),
            ("Dodge Warning", _rcDodge)), 0, 0);

        var rcBtns = new FlowLayoutPanel { AutoSize = true, Padding = new Padding(4, 8, 4, 4) };
        var btnAcc = new Button
            { Text = "✔  Accept Ready Check", BackColor = Color.FromArgb(40, 140, 40),
              ForeColor = Color.White, AutoSize = true, Padding = new Padding(6) };
        var btnDec = new Button
            { Text = "✘  Decline Ready Check", BackColor = Color.FromArgb(180, 40, 40),
              ForeColor = Color.White, AutoSize = true, Padding = new Padding(6) };
        btnAcc.Click += async (_, _) => await ActionReadyCheckAsync(accept: true);
        btnDec.Click += async (_, _) => await ActionReadyCheckAsync(accept: false);
        rcBtns.Controls.AddRange([btnAcc, btnDec]);
        rcLayout.Controls.Add(rcBtns, 0, 1);

        gbRc.Controls.Add(rcLayout);
        outer.Controls.Add(gbRc, 0, 0);
        page.Controls.Add(outer);
        return page;
    }

    // ── Sub-tab C: Pick & Ban ─────────────────────────────────────────────────────────
    private TabPage BuildPickBanSubTab()
    {
        var page = Tab("🎯  Pick & Ban");

        var outer = new TableLayoutPanel
        {
            Dock = DockStyle.Fill, Padding = new Padding(6),
            RowCount = 3, ColumnCount = 1
        };
        outer.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        outer.RowStyles.Add(new RowStyle(SizeType.Percent, 55));
        outer.RowStyles.Add(new RowStyle(SizeType.Percent, 45));

        // ── Header: Phase + current action banner ──
        var hdr = new TableLayoutPanel
        {
            AutoSize = true, Dock = DockStyle.Fill,
            ColumnCount = 2, RowCount = 1, Padding = new Padding(2)
        };
        hdr.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        hdr.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        hdr.Controls.Add(InfoGrid(("Phase", _csPhase), ("My Cell ID", _csCell)), 0, 0);
        _lcsCurrentAction.Dock = DockStyle.Fill;
        _lcsCurrentAction.TextAlign = ContentAlignment.MiddleLeft;
        _lcsCurrentAction.Padding = new Padding(14, 0, 0, 0);
        hdr.Controls.Add(_lcsCurrentAction, 1, 0);
        outer.Controls.Add(hdr, 0, 0);

        // ── Picks: Blue team (my team) + Red team (enemy) ──
        var picksRow = new TableLayoutPanel
        {
            Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 1
        };
        picksRow.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));
        picksRow.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50));

        var gbBlue = Group("🔵 Blue Team Picks  (session.myTeam)");
        _dgvCsTeam.Dock = DockStyle.Fill;
        gbBlue.Controls.Add(_dgvCsTeam);
        picksRow.Controls.Add(gbBlue, 0, 0);

        var gbRed = Group("🔴 Red Team Picks  (session.theirTeam)");
        _dgvCsEnemy.Dock = DockStyle.Fill;
        gbRed.Controls.Add(_dgvCsEnemy);
        picksRow.Controls.Add(gbRed, 1, 0);

        outer.Controls.Add(picksRow, 0, 1);

        // ── Bans ──
        var gbBans = Group("🚫 Bans  (session.actions where type = \"ban\"  ·  yellow = in progress, red = locked)");
        _dgvCsBans.Dock = DockStyle.Fill;
        gbBans.Controls.Add(_dgvCsBans);
        outer.Controls.Add(gbBans, 0, 2);

        page.Controls.Add(outer);
        return page;
    }

    // ─────────────────────────── TAB 3: GAME CLIENT ──────────────────────────────────
    private TabPage BuildGameClientTab()
    {
        var page = Tab("🎮  Game Client API");

        SetupDgv(_dgvPlayers, [
            ("Team", 60), ("Champion", 120), ("Summoner", 150), ("Lv", 40),
            ("K", 40), ("D", 40), ("A", 40), ("CS", 50),
            ("Spell 1", 90), ("Spell 2", 90), ("Keystone", 110)]);

        var outer = new TableLayoutPanel
        {
            Dock = DockStyle.Fill, Padding = new Padding(6),
            RowCount = 2, ColumnCount = 1
        };
        outer.RowStyles.Add(new RowStyle(SizeType.Absolute, 280));
        outer.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        // ── Top info row ──
        var topRow = new TableLayoutPanel
        {
            Dock = DockStyle.Fill, ColumnCount = 3
        };
        topRow.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33));
        topRow.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 33));
        topRow.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 34));

        // Active Player
        var gbAp = Group("Active Player  ·  GET /liveclientdata/activeplayer");
        var apLayout = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 2, ColumnCount = 1 };
        apLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        apLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        var apInfo = InfoGrid(
            ("Name", _apName), ("Level", _apLevel), ("Gold", _apGold),
            ("HP", _apHP), ("Resource", _apRes), ("Res. Type", _apResType));
        apLayout.Controls.Add(apInfo, 0, 0);

        var apStatsGrid = InfoGrid(
            ("AD", _apAD), ("AP", _apAP), ("Armor", _apArmor), ("MR", _apMR),
            ("Move Spd", _apMS), ("Crit%", _apCrit), ("Atk Spd", _apAS));
        apLayout.Controls.Add(apStatsGrid, 0, 1);
        gbAp.Controls.Add(apLayout);
        topRow.Controls.Add(gbAp, 0, 0);

        // Abilities + Runes
        var gbAbil = Group("Abilities & Runes  ·  GET /liveclientdata/activeplayerrunes");
        _apAbilities.Font = new Font("Consolas", 8.5f);
        _apAbilities.BackColor = SystemColors.Window;
        _apAbilities.Padding = new Padding(4);
        var abilScroll = new Panel { Dock = DockStyle.Fill, AutoScroll = true };
        abilScroll.Controls.Add(_apAbilities);
        _apAbilities.Dock = DockStyle.Fill;
        gbAbil.Controls.Add(abilScroll);
        topRow.Controls.Add(gbAbil, 1, 0);

        // Game Stats + Events
        var gbGame = Group("Game Stats & Events  ·  GET /liveclientdata/gamestats  +  /eventdata");
        var gameLayout = new TableLayoutPanel { Dock = DockStyle.Fill, RowCount = 2, ColumnCount = 1 };
        gameLayout.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        gameLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

        var gameInfo = new TableLayoutPanel { AutoSize = true, ColumnCount = 2 };
        gameInfo.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        gameInfo.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        AddInfoRow(gameInfo, "Mode", _gcMode);
        AddInfoRow(gameInfo, "Map", _gcMap);
        AddInfoRow(gameInfo, "Terrain", _gcTerrain);
        AddInfoRow(gameInfo, "Game Time", _gcTimeLabel);
        gameLayout.Controls.Add(gameInfo, 0, 0);

        _lstEvents.Dock = DockStyle.Fill;
        gameLayout.Controls.Add(_lstEvents, 0, 1);
        gbGame.Controls.Add(gameLayout);
        topRow.Controls.Add(gbGame, 2, 0);

        outer.Controls.Add(topRow, 0, 0);

        // ── Bottom: All Players ──
        var gbPlayers = Group("All Players  ·  GET /liveclientdata/playerlist");
        _dgvPlayers.Dock = DockStyle.Fill;
        gbPlayers.Controls.Add(_dgvPlayers);
        outer.Controls.Add(gbPlayers, 0, 1);

        page.Controls.Add(outer);
        return page;
    }

    // ─────────────────────────── TAB 4: RAW JSON ─────────────────────────────────────
    private TabPage BuildRawJsonTab()
    {
        var page = Tab("📋  Raw JSON");

        // Endpoint list — both APIs
        _cmbEndpoint.Width = 500;
        _cmbEndpoint.Items.AddRange([
            // Game Client API
            "🎮  [Game]  /liveclientdata/allgamedata",
            "🎮  [Game]  /liveclientdata/activeplayer",
            "🎮  [Game]  /liveclientdata/activeplayername",
            "🎮  [Game]  /liveclientdata/activeplayerabilities",
            "🎮  [Game]  /liveclientdata/activeplayerrunes",
            "🎮  [Game]  /liveclientdata/playerlist",
            "🎮  [Game]  /liveclientdata/eventdata",
            "🎮  [Game]  /liveclientdata/gamestats",
            // League Client API
            "🖥️  [LCU]   /lol-lobby/v2/lobby",
            "🖥️  [LCU]   /lol-champ-select/v1/session",
            "🖥️  [LCU]   /lol-matchmaking/v1/ready-check"
        ]);
        _cmbEndpoint.SelectedIndex = 0;

        var topBar = new FlowLayoutPanel
        {
            Dock = DockStyle.Top, AutoSize = true, Padding = new Padding(6),
            FlowDirection = FlowDirection.LeftToRight
        };
        topBar.Controls.AddRange([
            new Label { Text = "Endpoint:", AutoSize = true, Anchor = AnchorStyles.Left },
            _cmbEndpoint,
            _btnFetch,
            _btnCopy
        ]);

        var outer = new TableLayoutPanel
        {
            Dock = DockStyle.Fill, Padding = new Padding(6), RowCount = 2, ColumnCount = 1
        };
        outer.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        outer.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        outer.Controls.Add(topBar, 0, 0);
        outer.Controls.Add(_rtbJson, 0, 1);

        page.Controls.Add(outer);
        return page;
    }

    // ═══════════════════════════════════ REFRESH LOGIC ═════════════════════════════════

    private async Task RefreshAllAsync()
    {
        await Task.WhenAll(RefreshLeagueClientAsync(), RefreshGameClientAsync());
        _sslTime.Text = $"Last refresh: {DateTime.Now:HH:mm:ss}";
    }


    // ── League Client ──────────────────────────────────────────────────────────────────
    private async Task RefreshLeagueClientAsync()
    {
        // ── Step 1: Connectivity check via lockfile ─────────────────────────────────────
        // The lockfile exists exactly while the League Client process is running.
        // We NEVER judge connectivity by whether lobby/rc/cs endpoints return data —
        // they return null on the home screen too, and that must NOT mean "disconnected".
        if (!LeagueClientConnectionInfo.IsLeagueClientRunning())
        {
            _leagueClient?.Api.Dispose();
            _leagueClient = null;
            SetLeagueStatus(connected: false);
            ClearLeagueClientTab();
            _prevLobbyActive = _prevRcActive = _prevCsActive = false;
            return;
        }

        // ── Step 2: (Re)create client if needed ─────────────────────────────────────────
        if (_leagueClient == null)
        {
            try
            {
                var conn = LeagueClientConnectionInfo.GetFromRunningClient();
                if (conn == null) { SetLeagueStatus(connected: false); return; }
                _leagueClient = new LeagueClientApiReader(
                    new LeagueClientApiClient(conn, TimeSpan.FromSeconds(5)));
            }
            catch { SetLeagueStatus(connected: false); return; }
        }

        SetLeagueStatus(connected: true);   // ✔ client is running — regardless of lobby state

        // ── Step 3: Fetch data ──────────────────────────────────────────────────────────
        var lobbyTask = _leagueClient.GetLobbyAsync();
        var rcTask    = _leagueClient.GetReadyCheckAsync();
        var csTask    = _leagueClient.GetChampSelectSessionAsync();
        await Task.WhenAll(lobbyTask, rcTask, csTask);

        var lobby = await lobbyTask;
        var rc    = await rcTask;
        var cs    = await csTask;

        // ── Step 4: Lobby & Party ───────────────────────────────────────────────────────
        var inLobbyNow = lobby?.Members.Length > 0;

        if (lobby != null)
        {
            _lcMode.Text      = lobby.GameConfig?.GameMode ?? "—";
            _lcQueue.Text     = lobby.GameConfig?.QueueId.ToString() ?? "—";
            _lcMap.Text       = lobby.GameConfig?.MapId.ToString() ?? "—";
            _lcType.Text      = lobby.PartyType ?? "—";
            _lcStart.Text     = lobby.CanStartActivity ? "Yes ✔" : "No";
            _lcPartyId.Text   = !string.IsNullOrEmpty(lobby.PartyId)
                                ? lobby.PartyId[..Math.Min(16, lobby.PartyId.Length)] + "…"
                                : "—";
            _lcScarcePos.Text = lobby.ScarcePositions is { Count: > 0 }
                                ? string.Join(", ", lobby.ScarcePositions)
                                : "—";

            _dgvMembers.Rows.Clear();
            var localId = lobby.LocalMember?.SummonerId ?? -1;
            foreach (var m in lobby.Members)
            {
                var idx = _dgvMembers.Rows.Add();
                var row = _dgvMembers.Rows[idx];
                row.Cells[0].Value = idx + 1;
                row.Cells[1].Value = m.SummonerName ?? "—";
                row.Cells[2].Value = m.SummonerLevel;
                row.Cells[3].Value = m.FirstPositionPreference ?? "FILL";
                row.Cells[4].Value = m.SecondPositionPreference ?? "FILL";
                row.Cells[5].Value = m.IsLeader ? "★" : "";
                row.Cells[6].Value = m.Ready ? "✔" : "—";
                row.Cells[7].Value = m.SubteamIndex.HasValue ? $"#{m.SubteamIndex}" : "—";
                row.DefaultCellStyle.BackColor = m.IsLeader
                    ? Color.FromArgb(255, 255, 200)
                    : m.SummonerId == localId
                        ? Color.FromArgb(220, 240, 255)
                        : SystemColors.Window;
            }

            _dgvInvitations.Rows.Clear();
            if (lobby.Invitations is { Count: > 0 })
            {
                foreach (var inv in lobby.Invitations)
                {
                    var row = _dgvInvitations.Rows[_dgvInvitations.Rows.Add()];
                    row.Cells[0].Value = inv.ToSummonerName ?? "—";
                    row.Cells[1].Value = inv.State ?? "—";
                    row.Cells[2].Value = inv.InvitationType ?? "—";
                    row.DefaultCellStyle.BackColor = inv.State switch
                    {
                        "Accepted" => Color.FromArgb(220, 255, 220),
                        "Declined" => Color.FromArgb(255, 220, 220),
                        "Pending"  => Color.FromArgb(255, 250, 210),
                        _          => SystemColors.Window
                    };
                }
            }
        }
        else
        {
            _lcMode.Text = _lcQueue.Text = _lcMap.Text = _lcType.Text =
            _lcStart.Text = _lcPartyId.Text = _lcScarcePos.Text = "—";
            _dgvMembers.Rows.Clear();
            _dgvInvitations.Rows.Clear();
        }

        // Navigate to Lobby sub-tab only on ENTERING a lobby (state transition)
        if (inLobbyNow && !_prevLobbyActive)
        {
            _tabs.SelectedIndex = 1;          // League Client API tab
            _lcSubTabs.SelectedIndex = 0;     // Lobby & Party sub-tab
        }
        _prevLobbyActive = inLobbyNow;

        // ── Step 5: Ready Check ─────────────────────────────────────────────────────────
        var rcActive = rc?.State == "InProgress";
        _btnAccept.Enabled = _btnDecline.Enabled = rcActive;
        if (rc != null)
        {
            _rcState.Text      = rc.State ?? "—";
            _rcState.ForeColor = rcActive ? Color.OrangeRed : SystemColors.ControlText;
            _rcTimer.Text      = rc.Timer > 0 ? $"{rc.Timer:F1} s" : "—";
            _rcResponse.Text   = rc.PlayerResponse ?? "—";
            _rcDodge.Text      = rc.DodgeWarning ?? "None";
        }
        else { _rcState.Text = _rcTimer.Text = _rcResponse.Text = _rcDodge.Text = "—"; }

        // Navigate to Ready Check sub-tab only on ENTERING a ready check
        if (rcActive && !_prevRcActive)
        {
            _tabs.SelectedIndex = 1;
            _lcSubTabs.SelectedIndex = 1;     // Ready Check sub-tab
        }
        _prevRcActive = rcActive;

        // ── Step 6: Champion Select ─────────────────────────────────────────────────────
        var csActive = cs != null;
        if (cs != null)
        {
            _csPhase.Text = cs.Timer?.Phase ?? "—";
            _csCell.Text  = cs.LocalPlayerCellId.ToString();

            var allActions  = cs.Actions.SelectMany(a => a).ToList();
            var currentAct  = allActions.FirstOrDefault(a => a.IsInProgress);

            if (currentAct != null)
            {
                var actType = currentAct.Type == "ban" ? "🚫 Banning" : "🎯 Picking";
                _lcsCurrentAction.Text = $"{actType}  —  Cell #{currentAct.ActorCellId}" +
                    (currentAct.ChampionId > 0 ? $"  →  #{currentAct.ChampionId}" : "  (choosing…)");
                _lcsCurrentAction.ForeColor = currentAct.Type == "ban" ? Color.DarkRed : Color.DarkBlue;
            }
            else
            {
                _lcsCurrentAction.Text      = cs.Timer?.Phase == null ? "No active action" : "⏳ Waiting…";
                _lcsCurrentAction.ForeColor = Color.Gray;
            }

            // Blue team picks
            _dgvCsTeam.Rows.Clear();
            foreach (var tm in cs.MyTeam)
            {
                var row = _dgvCsTeam.Rows[_dgvCsTeam.Rows.Add()];
                row.Cells[0].Value = tm.CellId;
                row.Cells[1].Value = tm.ChampionId == 0 ? "—" : $"#{tm.ChampionId}";
                row.Cells[2].Value = (tm.ChampionPickIntent ?? 0) == 0 ? "—" : $"#{tm.ChampionPickIntent}";
                row.Cells[3].Value = tm.Spell1Id ?? 0;
                row.Cells[4].Value = tm.Spell2Id ?? 0;
                row.DefaultCellStyle.BackColor = tm.CellId == cs.LocalPlayerCellId
                    ? Color.FromArgb(210, 235, 255)
                    : Color.FromArgb(230, 242, 255);
            }

            // Red team picks
            _dgvCsEnemy.Rows.Clear();
            foreach (var tm in cs.TheirTeam)
            {
                var row = _dgvCsEnemy.Rows[_dgvCsEnemy.Rows.Add()];
                row.Cells[0].Value = tm.CellId;
                row.Cells[1].Value = tm.ChampionId == 0 ? "—" : $"#{tm.ChampionId}";
                row.Cells[2].Value = (tm.ChampionPickIntent ?? 0) == 0 ? "—" : $"#{tm.ChampionPickIntent}";
                row.Cells[3].Value = tm.Spell1Id ?? 0;
                row.Cells[4].Value = tm.Spell2Id ?? 0;
                row.DefaultCellStyle.BackColor = Color.FromArgb(255, 235, 232);
            }

            // Bans
            var bans = allActions.Where(a => a.Type == "ban").ToList();
            _dgvCsBans.Rows.Clear();
            for (int i = 0; i < bans.Count; i++)
            {
                var b   = bans[i];
                var row = _dgvCsBans.Rows[_dgvCsBans.Rows.Add()];
                row.Cells[0].Value = i + 1;
                row.Cells[1].Value = b.ActorCellId;
                row.Cells[2].Value = b.ChampionId > 0 ? $"#{b.ChampionId}"
                                   : b.IsInProgress    ? "choosing…" : "—";
                row.Cells[3].Value = b.IsInProgress ? "⚡ In Progress"
                                   : b.ChampionId > 0 ? "✔ Locked" : "—";
                row.DefaultCellStyle.BackColor = b.IsInProgress
                    ? Color.FromArgb(255, 245, 200)
                    : b.ChampionId > 0 ? Color.FromArgb(255, 218, 215) : SystemColors.Window;
            }

            // Navigate to Pick & Ban sub-tab only on ENTERING champion select
            if (!_prevCsActive)
            {
                _tabs.SelectedIndex = 1;
                _lcSubTabs.SelectedIndex = 2;
            }
        }
        else
        {
            _csPhase.Text = _csCell.Text = "—";
            _lcsCurrentAction.Text      = "Not in champion select";
            _lcsCurrentAction.ForeColor = Color.Gray;
            _dgvCsTeam.Rows.Clear();
            _dgvCsEnemy.Rows.Clear();
            _dgvCsBans.Rows.Clear();
        }
        _prevCsActive = csActive;
    }

    // ── Game Client ───────────────────────────────────────────────────────────────────
    private async Task RefreshGameClientAsync()
    {
        // Run both in parallel
        var allDataTask = _gameClient.GetAllGameDataAsync();
        var activeNameTask = _gameClient.GetActivePlayerNameAsync();
        await Task.WhenAll(allDataTask, activeNameTask);

        var allData = await allDataTask;
        var activeName = await activeNameTask;

        var inGame = allData != null;
        SetGameStatus(inGame, allData?.ActivePlayer?.SummonerName ?? activeName);

        // Auto-switch to Game Client tab only when the game STARTS (state transition)
        if (inGame && !_prevInGame)
            _tabs.SelectedIndex = 2;
        _prevInGame = inGame;

        if (!inGame) { ClearGameClientTab(); return; }

        // Game stats
        var gd = allData!.GameData;
        _gcMode.Text = gd?.GameMode ?? "—";
        _gcMap.Text = gd?.MapName ?? "—";
        _gcTerrain.Text = gd?.MapTerrain ?? "—";
        if (gd != null)
        {
            var ts = TimeSpan.FromSeconds(gd.GameTime);
            _gcTimeLabel.Text = $"{(int)ts.TotalMinutes:D2}:{ts.Seconds:D2}";
        }

        // Active player
        var ap = allData.ActivePlayer;
        if (ap != null)
        {
            _apName.Text = ap.SummonerName ?? "—";
            _apLevel.Text = ap.Level.ToString();
            _apGold.Text = $"{ap.CurrentGold:N0} g";
            var cs = ap.ChampionStats;
            if (cs != null)
            {
                _apHP.Text = $"{cs.CurrentHealth:N0} / {cs.MaxHealth:N0}";
                _apRes.Text = $"{cs.ResourceValue:N0} / {cs.ResourceMax:N0}";
                _apResType.Text = cs.ResourceType ?? "—";
                _apAD.Text = $"{cs.AttackDamage:N1}";
                _apAP.Text = $"{cs.AbilityPower:N1}";
                _apArmor.Text = $"{cs.Armor:N1}";
                _apMR.Text = $"{cs.MagicResist:N1}";
                _apMS.Text = $"{cs.MoveSpeed:N1}";
                _apCrit.Text = $"{cs.CritChance * 100:N0}%";
                _apAS.Text = $"{cs.AttackSpeed:N2}";
            }

            // Abilities
            var abil = ap.Abilities;
            var runes = ap.FullRunes;
            var sb = new StringBuilder();
            if (abil != null)
            {
                sb.AppendLine("Abilities:");
                AppendAbility(sb, "P", abil.Passive);
                AppendAbility(sb, "Q", abil.Q);
                AppendAbility(sb, "W", abil.W);
                AppendAbility(sb, "E", abil.E);
                AppendAbility(sb, "R", abil.R);
            }
            if (runes != null)
            {
                sb.AppendLine();
                sb.AppendLine("Runes:");
                sb.AppendLine($"  Keystone : {runes.Keystone?.DisplayName ?? "—"}");
                sb.AppendLine($"  Primary  : {runes.PrimaryRuneTree?.DisplayName ?? "—"}");
                sb.AppendLine($"  Secondary: {runes.SecondaryRuneTree?.DisplayName ?? "—"}");
            }
            _apAbilities.Text = sb.ToString();
        }

        // All players
        _dgvPlayers.Rows.Clear();
        foreach (var p in allData.AllPlayers ?? [])
        {
            var row = _dgvPlayers.Rows[_dgvPlayers.Rows.Add()];
            row.Cells[0].Value = p.Team;
            row.Cells[1].Value = p.ChampionName;
            row.Cells[2].Value = p.SummonerName;
            row.Cells[3].Value = p.Level;
            row.Cells[4].Value = p.Scores?.Kills;
            row.Cells[5].Value = p.Scores?.Deaths;
            row.Cells[6].Value = p.Scores?.Assists;
            row.Cells[7].Value = p.Scores?.CreepScore;
            row.Cells[8].Value = p.SummonerSpells?.SummonerSpellOne?.DisplayName ?? "—";
            row.Cells[9].Value = p.SummonerSpells?.SummonerSpellTwo?.DisplayName ?? "—";
            row.Cells[10].Value = p.Runes?.Keystone?.DisplayName ?? "—";
            row.DefaultCellStyle.BackColor = p.Team == "ORDER"
                ? Color.FromArgb(220, 235, 255) : Color.FromArgb(255, 225, 220);
            if (p.IsDead)
                row.DefaultCellStyle.ForeColor = Color.Gray;
        }

        // Events
        var events = allData.Events?.EventsList;
        _lstEvents.Items.Clear();
        if (events != null)
        {
            // Show newest first, cap at 50
            foreach (var ev in events.AsEnumerable().Reverse().Take(50))
            {
                var ts = TimeSpan.FromSeconds(ev.EventTime);
                var line = $"[{(int)ts.TotalMinutes:D2}:{ts.Seconds:D2}] {FormatEvent(ev)}";
                _lstEvents.Items.Add(line);
            }
        }
    }

    // ── Raw JSON ──────────────────────────────────────────────────────────────────────
    private async Task FetchRawJsonAsync()
    {
        if (_cmbEndpoint.SelectedItem is not string item) return;
        _rtbJson.Text = "Fetching…";

        try
        {
            string? raw;
            if (item.Contains("[Game]"))
            {
                var endpoint = item.Split("  ").Last().Trim();
                raw = await _gameClient.Api.GetAllGameDataJsonAsync(); // generic fallback
                raw = endpoint switch
                {
                    "/liveclientdata/allgamedata" => await _gameClient.Api.GetAllGameDataJsonAsync(),
                    "/liveclientdata/activeplayer" => await _gameClient.Api.GetActivePlayerJsonAsync(),
                    "/liveclientdata/activeplayername" => await _gameClient.Api.GetActivePlayerNameJsonAsync(),
                    "/liveclientdata/activeplayerabilities" => await _gameClient.Api.GetActivePlayerAbilitiesJsonAsync(),
                    "/liveclientdata/activeplayerrunes" => await _gameClient.Api.GetActivePlayerRunesJsonAsync(),
                    "/liveclientdata/playerlist" => await _gameClient.Api.GetPlayerListJsonAsync(),
                    "/liveclientdata/eventdata" => await _gameClient.Api.GetEventDataJsonAsync(),
                    "/liveclientdata/gamestats" => await _gameClient.Api.GetGameStatsJsonAsync(),
                    _ => null
                };
            }
            else
            {
                if (_leagueClient == null) { _rtbJson.Text = "League Client is not running."; return; }
                var endpoint = item.Split("   ").Last().Trim();
                raw = endpoint switch
                {
                    "/lol-lobby/v2/lobby" => await _leagueClient.Api.GetLobbyJsonAsync(),
                    "/lol-champ-select/v1/session" => await _leagueClient.Api.GetChampSelectSessionJsonAsync(),
                    "/lol-matchmaking/v1/ready-check" => await _leagueClient.Api.GetReadyCheckJsonAsync(),
                    _ => null
                };
            }

            if (raw == null) { _rtbJson.Text = "null — API returned no data (check prerequisites)."; return; }
            _rtbJson.Text = PrettyJson(raw);
        }
        catch (Exception ex)
        {
            _rtbJson.Text = $"Error: {ex.Message}";
        }
    }

    // ── Ready Check actions ───────────────────────────────────────────────────────────
    private async Task ActionReadyCheckAsync(bool accept)
    {
        if (_leagueClient == null) return;
        bool ok = accept
            ? await _leagueClient.AcceptReadyCheckAsync()
            : await _leagueClient.DeclineReadyCheckAsync();
        if (!ok)
            MessageBox.Show($"Ready check {(accept ? "accept" : "decline")} failed.",
                "API Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
    }

    // ═══════════════════════════════════ STATUS HELPERS ════════════════════════════════
    private void SetLeagueStatus(bool connected)
    {
        _sslLeague.Text = connected ? "🖥️  League Client: ✔ Connected" : "🖥️  League Client: ✘ Not running";
        _sslLeague.ForeColor = connected ? Color.DarkGreen : Color.DarkRed;
        _stLeagueState.Text = connected ? "✔ Running" : "✘ Not running";
        _stLeagueState.ForeColor = connected ? Color.DarkGreen : Color.DarkRed;

        if (connected && _leagueClient?.Api is LeagueClientApiClient c)
        {
            // show port via reflection-style info from connection — accessible via the connection field
            _stLeaguePort.Text = "Dynamic (lockfile)";
            _stLeagueToken.Text = "riot:******* (Basic Auth)";
        }
        else if (!connected)
        {
            _stLeaguePort.Text = "—";
            _stLeagueToken.Text = "—";
        }
    }

    private void SetGameStatus(bool inGame, string? playerName)
    {
        _sslGame.Text = inGame ? $"🎮  In-Game: ✔ {playerName}" : "🎮  Game Client: ✘ No active game";
        _sslGame.ForeColor = inGame ? Color.DarkGreen : Color.DarkGray;
        _stGameState.Text = inGame ? $"✔ In game" : "✘ No active game";
        _stGameState.ForeColor = inGame ? Color.DarkGreen : Color.DarkGray;
        _stGamePlayer.Text = playerName ?? "—";
    }

    private void ClearLeagueClientTab()
    {
        _lcMode.Text = _lcQueue.Text = _lcMap.Text = _lcType.Text =
        _lcStart.Text = _lcPartyId.Text = _lcScarcePos.Text = "—";
        _rcState.Text = _rcTimer.Text = _rcResponse.Text = _rcDodge.Text = "—";
        _csPhase.Text = _csCell.Text = "—";
        _lcsCurrentAction.Text = "Not in champion select";
        _lcsCurrentAction.ForeColor = Color.Gray;
        _dgvMembers.Rows.Clear();
        _dgvInvitations.Rows.Clear();
        _dgvCsTeam.Rows.Clear();
        _dgvCsEnemy.Rows.Clear();
        _dgvCsBans.Rows.Clear();
        _btnAccept.Enabled = _btnDecline.Enabled = false;
    }

    private void ClearGameClientTab()
    {
        _gcMode.Text = _gcMap.Text = _gcTerrain.Text = "—";
        _gcTimeLabel.Text = "—";
        _apName.Text = _apLevel.Text = _apGold.Text = "—";
        _apHP.Text = _apRes.Text = _apResType.Text = "—";
        _apAD.Text = _apAP.Text = _apArmor.Text = _apMR.Text = "—";
        _apMS.Text = _apCrit.Text = _apAS.Text = "—";
        _apAbilities.Text = "Not in game";
        _dgvPlayers.Rows.Clear();
        _lstEvents.Items.Clear();
        _stGameMode.Text = "—";
    }

    // ═══════════════════════════════════ UI HELPERS ════════════════════════════════════

    private static TabPage Tab(string text) =>
        new(text) { Padding = new Padding(6), UseVisualStyleBackColor = true };

    private static GroupBox Group(string text) =>
        new() { Text = text, Dock = DockStyle.Fill, Padding = new Padding(6) };

    private static TableLayoutPanel InfoGrid(params (string Key, Label Value)[] rows)
    {
        var tlp = new TableLayoutPanel
        {
            AutoSize = true, ColumnCount = 2, RowCount = rows.Length,
            Padding = new Padding(4)
        };
        tlp.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        tlp.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
        for (int i = 0; i < rows.Length; i++)
        {
            tlp.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            var keyLbl = new Label
            {
                Text = rows[i].Key + ":", AutoSize = true,
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                Anchor = AnchorStyles.Left | AnchorStyles.Top,
                Padding = new Padding(0, 2, 8, 2)
            };
            rows[i].Value.Anchor = AnchorStyles.Left | AnchorStyles.Top;
            rows[i].Value.Padding = new Padding(0, 2, 0, 2);
            tlp.Controls.Add(keyLbl, 0, i);
            tlp.Controls.Add(rows[i].Value, 1, i);
        }
        return tlp;
    }

    private static void AddInfoRow(TableLayoutPanel grid, string key, Control value)
    {
        grid.RowCount++;
        grid.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        var keyLbl = new Label
        {
            Text = key + ":", AutoSize = true,
            Font = new Font("Segoe UI", 9f, FontStyle.Bold),
            Padding = new Padding(0, 2, 8, 2)
        };
        grid.Controls.Add(keyLbl);
        grid.Controls.Add(value);
    }

    private static void SetupDgv(DataGridView dgv, (string Name, int Width)[] columns)
    {
        dgv.ReadOnly = true;
        dgv.AllowUserToAddRows = false;
        dgv.AllowUserToDeleteRows = false;
        dgv.RowHeadersVisible = false;
        dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
        dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        dgv.BackgroundColor = SystemColors.Window;
        dgv.BorderStyle = BorderStyle.Fixed3D;
        dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 8.5f, FontStyle.Bold);

        foreach (var (name, width) in columns)
        {
            var col = new DataGridViewTextBoxColumn
            {
                Name = name, HeaderText = name,
                FillWeight = width, MinimumWidth = 30
            };
            dgv.Columns.Add(col);
        }
    }

    private static void AppendAbility(StringBuilder sb, string key, Ability? a)
    {
        if (a == null) return;
        sb.AppendLine($"  {key}  Lv{a.AbilityLevel}  {a.DisplayName}");
    }

    private static string FormatEvent(GameEvent ev) => ev.EventName switch
    {
        "ChampionKill" => $"⚔  {ev.KillerName} killed {ev.VictimName}" +
                          (ev.Assisters?.Count > 0 ? $"  (+{string.Join(", ", ev.Assisters)})" : ""),
        "DragonKill" => $"🐉  Dragon ({ev.DragonType}) slain by {ev.KillerName}",
        "BaronKill" => $"👁  Baron slain by {ev.KillerName}",
        "TurretKilled" => $"🗼  Turret destroyed: {ev.TurretKilled}",
        "InhibitorKilled" => $"💥  Inhibitor destroyed: {ev.InhibKilled}",
        "InhibitorRespawned" => $"🔄  Inhibitor respawned",
        "Ace" => $"🏆  ACE! by {ev.Acer} ({ev.AcingTeam})",
        "GameStart" => $"🟢  Game started",
        "FirstBlood" => $"🩸  First Blood! {ev.KillerName}",
        "Multikill" => $"🔥  Multikill x{ev.KillStreak} by {ev.KillerName}",
        _ => $"  {ev.EventName}"
    };

    private static string PrettyJson(string raw)
    {
        try
        {
            using var doc = JsonDocument.Parse(raw);
            return JsonSerializer.Serialize(doc, new JsonSerializerOptions { WriteIndented = true });
        }
        catch { return raw; }
    }
}

