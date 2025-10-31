# 🎮 Live Monitor - Vollständige Integration

## ✨ Was wurde integriert?

Die Console-Anwendung zeigt jetzt **automatisch** alle relevanten Informationen an:

### 🏠 In der Lobby
- **Lobby-Mitglieder** - Wer ist mit dir in der Lobby
- **Queue-Typ** - Welcher Spielmodus (Ranked, Normal, ARAM, etc.)
- **Champion Select** - Live-Anzeige wer was picked
- **Pick-Intents** - Wer welchen Champion picken möchte (🎯 Symbol)
- **Auto-Accept** - Akzeptiert automatisch Ready Checks ⭐

### 🎮 Im Spiel
- **Spielinformationen** - Map, Modus, Spielzeit
- **Deine Stats** - Level, Gold, HP, AD, AP, etc.
- **Team-Übersicht** - Alle Spieler mit KDA und CS
- **Events** - Kills, Objectives, Multikills

---

## 🚀 Verwendung

### Starten

```cmd
cd C:\dev\Boas\BE.LeagueClient\src\BE.League\BE.League.Desktop.Console
dotnet run
```

### Was passiert?

Die Anwendung überwacht kontinuierlich:
1. **Wartet** auf League of Legends Client
2. **Erkennt Lobby** automatisch
3. **Zeigt Champion Select** mit allen Picks live
4. **Akzeptiert Ready Check** automatisch
5. **Wechselt zu Live Game** sobald das Spiel startet
6. **Zeigt Live-Stats** während des Spiels

---

## 📺 Anzeige-Beispiele

### 1. In der Lobby

```
╔════════════════════════════════════════════════════════════════╗
║             🏠 LOBBY - League of Legends                       ║
╚════════════════════════════════════════════════════════════════╝

┌─── LOBBY INFORMATIONEN ────────────────────────────────────────┐
│ Mitglieder: 5                                                  │
│ Queue: Ranked Solo/Duo                                         │
│                                                                │
│ Team:                                                          │
│   • Spieler1                                                   │
│   • Spieler2                                                   │
│   • Spieler3                                                   │
│   • Spieler4                                                   │
│   • Spieler5                                                   │
└────────────────────────────────────────────────────────────────┘

[13:45:22] Nächste Aktualisierung in 2 Sekunden...
```

### 2. Champion Select (Live Picks!)

```
╔════════════════════════════════════════════════════════════════╗
║             🏠 LOBBY - League of Legends                       ║
╚════════════════════════════════════════════════════════════════╝

┌─── LOBBY INFORMATIONEN ────────────────────────────────────────┐
│ Mitglieder: 5                                                  │
│ Queue: Ranked Solo/Duo                                         │
│                                                                │
│ Team:                                                          │
│   • Spieler1                                                   │
│   • Spieler2                                                   │
│   • Spieler3                                                   │
│   • Spieler4                                                   │
│   • Spieler5                                                   │
└────────────────────────────────────────────────────────────────┘

┌─── CHAMPION SELECT ────────────────────────────────────────────┐
│ Phase: BAN_PICK                                                │
│ Deine Cell ID: 2                                               │
│                                                                │
│ Team:                                                          │
│                                                                │
│      Cell 0: ✓ Jinx                                           │
│ 👤 DU Cell 2: 🎯 Intent: Thresh                                │
│      Cell 3: ✓ Yasuo                                           │
│      Cell 4: ⏳ Noch nicht gewählt                             │
│      Cell 7: 🎯 Intent: Ahri                                   │
└────────────────────────────────────────────────────────────────┘

[13:47:15] Nächste Aktualisierung in 2 Sekunden...
```

**Legende:**
- ✓ = Champion wurde gepickt
- 🎯 Intent = Spieler hat Intent gezeigt (möchte diesen Champion picken)
- ⏳ = Noch keine Auswahl
- 👤 DU = Das bist du!

### 3. Ready Check (Auto-Accept)

```
┌─── READY CHECK ────────────────────────────────────────────────┐
│ 🔔 SPIEL GEFUNDEN!                                             │
│                                                                │
│ Akzeptiere automatisch...                                      │
│ ✓ Ready Check erfolgreich akzeptiert!                          │
└────────────────────────────────────────────────────────────────┘
```

### 4. Im Spiel

```
╔════════════════════════════════════════════════════════════════╗
║             🎮 LIVE GAME - League of Legends                   ║
╚════════════════════════════════════════════════════════════════╝

┌─── SPIEL-INFORMATIONEN ────────────────────────────────────────┐
│ Map: Summoner's Rift                                           │
│ Spielmodus: CLASSIC                                            │
│ Spielzeit: 15:42                                               │
└────────────────────────────────────────────────────────────────┘

┌─── AKTIVER SPIELER ────────────────────────────────────────────┐
│ Name: DeinName                                                 │
│ Level: 12                                                      │
│ Gold: 8750                                                     │
│                                                                │
│ HP: 1820/2100 │ AD: 175 │ AP: 40                              │
│ Armor: 85 │ MR: 45 │ MS: 350                                  │
│ Angriffstempo: 1.23 │ Krit-Chance: 25%                        │
└────────────────────────────────────────────────────────────────┘

┌─── TEAM ÜBERSICHT ─────────────────────────────────────────────┐
│                                                                │
│ VERBÜNDETE (Blau):                                            │
│ ✓ Jinx         │ Spieler1             │ 5/2/3   │ CS:142 │
│ ✓ Thresh       │ DeinName             │ 0/1/8   │ CS:45  │
│ ✓ Yasuo        │ Spieler3             │ 3/4/2   │ CS:128 │
│ ✓ Vi           │ Spieler4             │ 4/3/6   │ CS:98  │
│ ✓ Ahri         │ Spieler5             │ 6/2/4   │ CS:156 │
│                                                                │
│ GEGNER (Rot):                                                  │
│ ✓ Vayne        │ Enemy1               │ 4/3/2   │ CS:135 │
│ 💀 Lux          │ Enemy2               │ 1/5/6   │ CS:42  │
│ ✓ Zed          │ Enemy3               │ 5/4/3   │ CS:148 │
│ ✓ Lee Sin      │ Enemy4               │ 3/3/7   │ CS:92  │
│ ✓ Darius       │ Enemy5               │ 2/4/5   │ CS:142 │
└────────────────────────────────────────────────────────────────┘

Nächste Aktualisierung in 5 Sekunden...
```

---

## 🎯 Features im Detail

### 1. Automatische Zustandserkennung

Die Anwendung erkennt automatisch, in welchem Zustand du bist:

```
Warte auf Lobby → In Lobby → Champion Select → Ready Check → Im Spiel
```

### 2. Live Champion-Picks

Während der Champion-Select-Phase siehst du **in Echtzeit**:
- Wer hat bereits gepickt (✓)
- Wer hat seinen Intent gezeigt (🎯)
- Wer muss noch picken (⏳)
- Welche Cell-ID du selbst hast (👤 DU)

### 3. Auto-Accept Ready Check ⭐

**Verpasst nie wieder ein Spiel!**
- Erkennt Ready Check automatisch
- Akzeptiert sofort
- Zeigt Bestätigung an

### 4. Nahtloser Übergang

Die Anwendung wechselt automatisch zwischen:
- Lobby-Ansicht (Update alle 2 Sekunden)
- Live-Game-Ansicht (Update alle 5 Sekunden)
- Wartezustand (wenn nichts aktiv ist)

---

## ⚙️ Technische Details

### Update-Intervalle

```csharp
Lobby/Champion Select: 2 Sekunden  // Schneller für Pick-Updates
Live Game:             5 Sekunden  // Langsamer, da sich weniger ändert
Wartezustand:          2 Sekunden  // Mittlerer Wert
```

### Fehlerbehandlung

Die Anwendung ist robust und behandelt:
- ✅ League Client nicht gestartet
- ✅ Keine Lobby aktiv
- ✅ Kein Spiel läuft
- ✅ Verbindungsfehler
- ✅ Timeout-Probleme

### Champion-Namen

Die Anwendung kennt **alle aktiven Champions** (150+ Champions):
- Annie, Olaf, Galio, Twisted Fate, ...
- Jinx, Thresh, Yasuo, Ahri, Zed, ...
- Akshan, Bel'Veth, Zeri, Viego, ...
- Hwei, Naafiri, K'Sante, Milio, ...

Unbekannte Champions werden als `Champion #ID` angezeigt.

---

## 🎮 Queue-Typen

Erkannte Queue-Typen:
- **420** - Ranked Solo/Duo
- **440** - Ranked Flex
- **450** - ARAM
- **400** - Normal Draft
- **430** - Normal Blind
- **490** - Normal Quickplay
- **700** - Clash
- **900** - ARURF
- **1020** - One for All
- **1300** - Nexus Blitz
- **1400** - Ultimate Spellbook

---

## 💡 Verwendungsszenarien

### 1. Queue Popper
Starte die Anwendung, bevor du in die Queue gehst:
- Sie akzeptiert automatisch das Spiel
- Du verpasst keinen Ready Check mehr!

### 2. Live Stats Tracker
Während des Spiels:
- Sieh deine Stats in Echtzeit
- Überprüfe Team-KDA
- Verfolge Objectives

### 3. Champion Select Helper
In der Champion-Select-Phase:
- Sieh sofort, wer was picked
- Erkenne Intents deiner Teammitglieder
- Plane deine Team-Composition

---

## ⌨️ Steuerung

- **Strg+C** - Anwendung beenden
- Die Anwendung läuft automatisch und passt sich an den aktuellen Zustand an

---

## 🔧 Anpassungen

### Update-Intervall ändern

**Lobby:**
```csharp
// Zeile ~88 in Program.cs
await Task.Delay(2000);  // → Ändern auf gewünschte Millisekunden
```

**Live Game:**
```csharp
// Zeile ~54 in Program.cs
await Task.Delay(5000);  // → Ändern auf gewünschte Millisekunden
```

### Auto-Accept deaktivieren

Kommentiere diese Zeilen aus (~83-88 in Program.cs):
```csharp
// var readyCheck = await reader.GetReadyCheckAsync();
// if (readyCheck != null)
// {
//     await HandleReadyCheck(reader, readyCheck);
//     Console.WriteLine();
// }
```

---

## ✅ Zusammenfassung

Die Console-Anwendung ist jetzt ein **vollständiger Live-Monitor** für League of Legends:

✅ **Lobby-Überwachung** - Sieh wer mit dir spielt  
✅ **Champion Select** - Live-Picks mit Intents  
✅ **Auto-Accept** - Verpasst nie wieder ein Spiel  
✅ **Live Game Stats** - Alle Daten in Echtzeit  
✅ **Automatische Erkennung** - Passt sich an deinen Status an  
✅ **Fehlerresistent** - Funktioniert zuverlässig  

**Einfach starten und läuft!** 🚀

---

**Erstellt**: 2025-10-30  
**Version**: 2.0 - Vollständige Lobby-Integration  
**Status**: ✅ Produktionsbereit

