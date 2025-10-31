# League of Legends - Auto Accept

Automatically accepts ready checks in League of Legends with configurable delay and auto-exit options.

## Features

- 🔔 **Automatic Ready Check Detection** - Continuously monitors for ready checks
- ⏱️ **Configurable Delay** - Set a custom delay before accepting (default: 2000ms)
- 🚪 **Auto-Exit** - Optionally exit after accepting (default: enabled)
- 📊 **Live Status Display** - Shows lobby information using Spectre.Console
- ⚙️ **Settings** - Configurable via command line or interactive prompts

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

### Build & Publish

**Build for development:**
```cmd
dotnet build -c Release
```

**Publish as single-file executable:**
```cmd
dotnet publish -c Release -r win-x64 --self-contained false /p:PublishSingleFile=true
```

The executable will be located at:
```
bin\Release\net9.0\win-x64\publish\BE.League.Desktop.AutoAccept.exe
```

**Publish as self-contained (no .NET runtime required):**
```cmd
dotnet publish -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true
```

## Configuration

### Default Settings

```csharp
var settings = new AcceptSettings(
    DelayMs: 2000,      // Wait 2 seconds before accepting
    AutoExit: true      // Exit after accepting
);
```

### Customize Settings

Edit `Program.cs` to change the default behavior:

```csharp
// No delay, stay running after accept
var settings = new AcceptSettings(DelayMs: 0, AutoExit: false);

// 5 second delay, exit after accept
var settings = new AcceptSettings(DelayMs: 5000, AutoExit: true);
```

## Display Information

The application shows:
- **Ready Check Status** - Current state of ready check detection
- **Lobby Information** - Game mode and player count
- **Start Status** - Whether the lobby can start a game
- **Elapsed Time** - Time since last check
- **Timestamp** - Current time

## Technical Details

### Architecture

The application uses:
- `LiveClientObjectReader` - Main API client for LCU (League Client API)
- `Spectre.Console` - Rich console UI with tables and formatting
- `LeagueJsonContext` - JSON Source Generator for AOT/Trimming support

### API Endpoints Used

- `GET /lol-lobby/v2/lobby` - Current lobby information
- `GET /lol-matchmaking/v1/ready-check` - Ready check state
- `POST /lol-matchmaking/v1/ready-check/accept` - Accept ready check

### Connection Flow

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

