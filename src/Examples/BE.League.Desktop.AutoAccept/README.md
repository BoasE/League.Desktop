# League of Legends - Auto Accept

Automatically accepts ready checks in League of Legends with a clean, modular architecture.

## Features

- 🔔 **Automatic Ready Check Detection** - Continuously monitors for ready checks in the lobby
- ⏱️ **Configurable Delay** - Set a custom delay before accepting (default: 2 seconds)
- 📊 **Live Status Display** - Shows lobby information using Spectre.Console
- ⌨️ **Keyboard Control** - Press ESC to exit monitoring
- 🎯 **Modular Architecture** - Clean separation of concerns

## Requirements

- .NET 9.0
- Windows
- League of Legends Client must be running
- An active lobby (custom game, ranked, normal, etc.)

## Usage

### Quick Start

1. Start League of Legends and create/join a lobby
2. Run the application:
   ```cmd
   dotnet run
   ```
3. The application will automatically accept the ready check when a game is found
4. Press **ESC** to exit at any time

### Build & Publish

> 📘 **Detaillierte Publish-Anleitung**: Siehe [PUBLISH.md](PUBLISH.md) für eine vollständige Übersicht aller Optionen und Empfehlungen.

**Build for development:**
```cmd
dotnet build -c Release
```

#### Quick Build (Windows)

Doppelklick auf `build.bat` für automatisches Publishing mit empfohlenen Einstellungen.

Das Skript erstellt automatisch eine optimierte, verteilbare .exe Datei.

#### Recommended: Schlanke Distribution mit Trimming (Empfohlen)

**Best Practice für Windows-Distribution ohne .NET Installation:**
```cmd
dotnet publish -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true /p:PublishTrimmed=true /p:TrimMode=partial
```
- ✅ **Klein** (~17 MB einzelne .exe Datei)
- ✅ **Keine .NET Runtime erforderlich**
- ✅ **Maximale Kompatibilität**
- ✅ **Einfache Distribution**

#### Weitere Optionen

**Option 1: Native AOT (Kleinste & Schnellste Variante)**
```cmd
dotnet publish -c Release -r win-x64 /p:PublishAot=true
```
- ✅ **Sehr klein** (~8-15 MB)
- ✅ **Sehr schneller Start** (keine JIT-Kompilierung)
- ✅ **Keine .NET Runtime erforderlich**
- ✅ **Einzelne .exe Datei**
- ⚠️ Funktioniert nur mit AOT-kompatiblem Code

**Option 2: Self-Contained + Trimmed (Empfohlen für Kompatibilität)**
```cmd
dotnet publish -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true /p:PublishTrimmed=true /p:TrimMode=partial
```
- ✅ **Klein** (~20-30 MB)
- ✅ **Keine .NET Runtime erforderlich**
- ✅ **Einzelne .exe Datei**
- ✅ **Maximale Kompatibilität**

**Option 3: Framework-Dependent (Benötigt .NET 9.0 Runtime)**
```cmd
dotnet publish -c Release -r win-x64 --self-contained false /p:PublishSingleFile=true
```
- ✅ **Sehr klein** (~5 MB)
- ⚠️ Benutzer muss .NET 9.0 Runtime installiert haben

#### Output-Verzeichnisse

Die ausführbare Datei befindet sich in:
```
bin\Release\net9.0\win-x64\publish\BE.League.Desktop.AutoAccept.exe
```

#### Größenvergleich

| Methode | Größe | .NET benötigt? | Startzeit |
|---------|-------|----------------|-----------|
| Native AOT | ~8-15 MB | ❌ Nein | Sehr schnell |
| Self-Contained + Trimmed | ~20-30 MB | ❌ Nein | Schnell |
| Self-Contained (ohne Trim) | ~60-80 MB | ❌ Nein | Normal |
| Framework-Dependent | ~5 MB | ✅ Ja | Normal |

#### Empfehlung für Distribution

Für die beste Balance zwischen Größe und Kompatibilität:
```cmd
dotnet publish -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true /p:PublishTrimmed=true /p:TrimMode=partial /p:EnableCompressionInSingleFile=true
```

Weitere Runtime Identifiers für andere Plattformen:
- `win-x64` - Windows 64-bit (Standard)
- `win-x86` - Windows 32-bit
- `win-arm64` - Windows ARM64 (Surface Pro X, etc.)

## Architecture

The application follows clean code principles with a modular structure:

### Project Structure

```
BE.League.Desktop.AutoAccept/
├── Program.cs           # Application entry point
├── MonitorLoop.cs       # Main monitoring logic
└── Displays.cs          # UI rendering and table creation
```

### Core Components

#### 1. **Program.cs**
The entry point that:
- Sets up cancellation token handling (Ctrl+C)
- Displays the application header
- Starts the monitoring loop

```csharp
var cts = new CancellationTokenSource();
Console.CancelKeyPress += (_, e) =>
{
    AnsiConsole.WriteLine("Ending...");
    e.Cancel = true;
    cts.Cancel();
};

Displays.WriteHeader();
await MonitorLoop.Run(cts.Token);
```

#### 2. **MonitorLoop.cs**
Main business logic that:
- Monitors the lobby status
- Detects ready checks
- Handles the accept flow with configurable delay
- Manages keyboard input (ESC to exit)

Key methods:
- `Run(CancellationToken)` - Main entry point
- `MonitorLobby(CancellationToken)` - Lobby monitoring loop
- `ReadCheck(...)` - Ready check detection and handling
- `Accept(...)` - Accept logic
- `CanClickAccept(ReadyCheckDto?)` - Validation logic

#### 3. **Displays.cs**
UI rendering layer that:
- Creates all visual elements using Spectre.Console
- Provides consistent styling across the application
- Separates presentation from business logic

Available methods:
- `WriteHeader()` - App header with ASCII art
- `CreateStatusTable(...)` - Main status display
- `GameFoundNotification(int)` - Game found notification
- `WriteAcceptingNotification(...)` - Accepting status
- `WriteSuccessNotification(...)` - Success message
- `WriteError(...)` - Error message

### Flow Diagram

```
Start
  │
  ├─> Display Header (Displays.WriteHeader)
  │
  ├─> Start Monitor Loop (MonitorLoop.Run)
  │    │
  │    ├─> Poll Lobby Status every 500ms
  │    │
  │    ├─> Check for ESC key press → Exit
  │    │
  │    ├─> Update Status Display (Displays.CreateStatusTable)
  │    │
  │    ├─> Detect Ready Check (ReadCheck)
  │    │    │
  │    │    ├─> Game Found? → Show Notification
  │    │    │
  │    │    ├─> Wait 2 seconds (configurable delay)
  │    │    │
  │    │    ├─> Validate Ready Check still active
  │    │    │
  │    │    ├─> Accept Ready Check
  │    │    │    │
  │    │    │    ├─> Success → Show Success Message
  │    │    │    └─> Failed → Show Error Message
  │    │    │
  │    │    └─> Return to monitoring
  │    │
  │    └─> Repeat until ESC or Ctrl+C
  │
  └─> End
```

## Display Information

### Status Display

The status table shows:
- **Lobby Information** - Game mode and player count
- **Status** - Current monitoring state:
  - "Warte auf Lobby start..." - Lobby ready to start
  - "Prüfe auf Ready Check..." - Actively monitoring
  - "Keine Lobby gefunden..." - Not in a lobby

### Example Output

**Application Start:**
```
   _         _              _                         _   
  /_\ _   _| |_ ___       /_\   ___ ___ ___ _ __ | |_ 
 //_\\| | | | __/ _ \    //_\\ / __/ __/ _ \ '_ \| __|
/  _  \ |_| | || (_) |  /  _  \ (_| (_|  __/ |_) | |_ 
\_/ \_/\__,_|\__\___/   \_/ \_/\___\___\___| .__/ \__|
                                            |_|        

━━━━━ Simple Ready Check and autoaccepting new games ━━━━━

╭────────────────────────────────────╮
│           STATUS                   │
├────────────────────────────────────┤
│ Lobby: ARAM 5 Spieler              │
│  Prüfe auf Ready Check...          │
╰────────────────────────────────────╯
```

**Game Found:**
```
╔════════════════════════════════════╗
║           STATUS                   ║
╠════════════════════════════════════╣
║ 🔔 SPIEL GEFUNDEN! (#1)            ║
║ 14:23:45                           ║
║ Akzeptiere das Spiel in 2 Sekun...║
╚════════════════════════════════════╝
```

**Accepting:**
```
╭────────────────────────────────────╮
│        AKZEPTIERE...               │
├────────────────────────────────────┤
│ ✓ Sende Accept-Befehl...           │
╰────────────────────────────────────╯
```

**Success:**
```
╔════════════════════════════════════╗
║           ERFOLG                   ║
╠════════════════════════════════════╣
║ ✓ ERFOLGREICH AKZEPTIERT!          ║
║ 14:23:47                           ║
╚════════════════════════════════════╝
```

**Error (if accept fails):**
```
╭────────────────────────────────────╮
│           FEHLER                   │
├────────────────────────────────────┤
│ ✗ Konnte nicht akzeptieren         │
╰────────────────────────────────────╯
```

### Color Coding

The application uses color-coded messages:
- 🟢 **Green** - Success states (game found, accepted)
- 🟡 **Yellow** - Normal monitoring state
- 🔴 **Red** - Error states
- ⚪ **Grey** - Information text (timestamps, details)

## Configuration

### Delay Configuration

Currently, the delay is hardcoded in `MonitorLoop.cs`:

```csharp
// Wait 2 seconds before accepting
await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);
```

To customize the delay, modify this value in the `ReadCheck` method.

### Future Enhancements

Potential configuration options:
- Command line arguments for delay
- Settings file (JSON/YAML)
- Interactive configuration on startup
- Auto-exit after accept option

## Technical Details

### API Integration

The application uses the unified `LiveClientObjectReader` which provides access to:
- **LCU API** (League Client Update) - For lobby and ready check operations
- **Live Client Data API** - For in-game data (not used in this example)

### LCU Endpoints Used

1. **`GET /lol-lobby/v2/lobby`**
   - Retrieves current lobby information
   - Returns: Game mode, player count, can start status

2. **`GET /lol-matchmaking/v1/ready-check`**
   - Gets ready check state
   - Returns: State ("InProgress", etc.), player accept status

3. **`POST /lol-matchmaking/v1/ready-check/accept`**
   - Accepts the ready check
   - Returns: Boolean success status

### Connection Details

- **LCU Port**: Dynamic (read from lockfile)
- **LCU Protocol**: HTTPS with self-signed certificate
- **Authentication**: Basic Auth (riot:password from lockfile)
- **Lockfile Location**: `%LOCALAPPDATA%\Riot Games\League of Legends\lockfile`

### Error Handling

The application handles errors gracefully:
- **Network errors**: Silently continues monitoring
- **API errors**: Displays error message, continues monitoring
- **Cancellation**: Properly cleans up on Ctrl+C or ESC

## Dependencies

- **BE.League.Desktop** - Core library for League of Legends API integration
- **Spectre.Console** (v0.49.1) - Rich console UI framework

## Troubleshooting

### "Keine Lobby gefunden"
- Ensure League of Legends client is running
- Create or join a lobby before running the application
- Check that the lockfile exists in `%LOCALAPPDATA%\Riot Games\League of Legends\`

### Ready Check Not Detected
- Make sure you're in queue (not just in lobby)
- The application polls every 500ms, there may be a slight delay
- Check that the League client is responding

### Application Won't Exit
- Press **ESC** to gracefully exit the monitoring loop
- Use **Ctrl+C** for immediate termination

## Development

### Adding New Features

To add new functionality:

1. **Business Logic** → Add to `MonitorLoop.cs`
2. **UI Elements** → Add to `Displays.cs`
3. **Configuration** → Update `Program.cs`

Example: Adding sound notifications

```csharp
// In Displays.cs
public static void PlaySound()
{
    Console.Beep(800, 300);
}

// In MonitorLoop.cs, after game found
Displays.PlaySound();
```

### Code Style

The codebase follows:
- **Clean Code** principles (readable, self-documenting)
- **Single Responsibility** (each class/method has one job)
- **Separation of Concerns** (UI, logic, data access separated)

## License

This is an example application demonstrating the `BE.League.Desktop` library usage.

1. Detects running League Client process
2. Reads `lockfile` for connection details (port, password)
3. Builds HTTPS URL with Basic Auth: `https://127.0.0.1:{port}`
4. Polls LCU API every 500ms for ready check state
5. Accepts ready check with configured delay
6. Optionally exits or continues monitoring

## Error Handling

The application handles:
- ❌ **No lobby found** - Continues monitoring
- ❌ **League Client not running** - Waits and retries
- ❌ **API timeout** - Continues monitoring
- ❌ **Accept failed** - Shows error and continues

## Exit Options

- **ESC** - Exit immediately
- **Ctrl+C** - Graceful shutdown
- **Auto-Exit** - Automatically exits after accepting (if enabled)

## Example Output

```
   _         _             _                         _   
  /_\  _   _| |_ ___      /_\   ___ ___ ___ _ __ | |_ 
 //_\\| | | | __/ _ \    //_\\ / __/ __/ _ \ '_ \| __|
/  _  \ |_| | || (_) |  /  _  \ (_| (_|  __/ |_) | |_ 
\_/ \_/\__,_|\__\___/   \_/ \_/\___\___\___| .__/ \__|
                                            |_|        

━━━━━━━ League of Legends - Ready Check Auto-Accept ━━━━━━━

╭────────────────────────────────────────╮
│            STATUS                      │
├────────────────────────────────────────┤
│ 🔍 Prüfe auf Ready Check...           │
│ Lobby: CLASSIC 5 Spieler               │
│   Kann starten                         │
│ 15s                                    │
│ 14:23:45                               │
╰────────────────────────────────────────╯
```

## Troubleshooting

### "Reflection-based serialization has been disabled"

This error occurs when the application is published with AOT or trimming enabled. The solution is already implemented via `LeagueJsonContext` (JSON Source Generator).

### "Connection refused" or "401 Unauthorized"

- Ensure League Client is running
- Check that the lockfile exists
- Verify the application has permission to read the lockfile

### Ready check not detected

- Ensure you are in an active lobby
- Check that matchmaking has started (queue is active)
- Verify the League Client is unlocked (not in game)

## Notes

- The application only accepts ready checks, it does NOT automatically queue
- You must manually start the queue in League Client
- The application will keep monitoring until a ready check appears
- Self-signed certificates are accepted for local HTTPS to LCU

## License

This project is part of **BE.League** and follows the same license terms.

