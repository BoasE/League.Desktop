# Publish Guide - Auto Accept

Anleitung zum Erstellen einer verteilbaren Windows-Anwendung ohne erforderliche .NET Installation.

## 🎯 Empfohlene Konfiguration

Für die beste Balance zwischen Größe und Kompatibilität:

```cmd
dotnet publish -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true /p:PublishTrimmed=true /p:TrimMode=partial
```

### Ergebnis
- 📦 **Größe**: ~17 MB (einzelne .exe Datei)
- ✅ **Keine .NET Installation erforderlich**
- ✅ **Funktioniert auf jedem Windows x64 PC**
- ✅ **Einfache Distribution** (nur eine Datei)

### Output-Pfad
```
bin\Release\net9.0\win-x64\publish\BE.League.Desktop.AutoAccept.exe
```

## 🚀 Weitere Optionen

### Option 1: Maximale Kompression (kleinste Größe)

```cmd
dotnet publish -c Release -r win-x64 --self-contained true ^
  /p:PublishSingleFile=true ^
  /p:PublishTrimmed=true ^
  /p:TrimMode=full ^
  /p:EnableCompressionInSingleFile=true ^
  /p:DebugType=none ^
  /p:DebugSymbols=false
```

**Ergebnis**: ~12-15 MB

⚠️ **Warnung**: `TrimMode=full` kann zu Laufzeitfehlern führen, wenn Code dynamisch geladen wird.

### Option 2: Native AOT (experimentell)

```cmd
dotnet publish -c Release -r win-x64 /p:PublishAot=true
```

**Vorteile**:
- ✅ Sehr klein (~8-12 MB)
- ✅ Sehr schneller Start
- ✅ Keine .NET Runtime

**Nachteile**:
- ⚠️ Erfordert AOT-kompatiblen Code
- ⚠️ Spectre.Console könnte Probleme machen
- ⚠️ Längere Kompilierzeit

### Option 3: Framework-Dependent (benötigt .NET Runtime)

```cmd
dotnet publish -c Release -r win-x64 --self-contained false /p:PublishSingleFile=true
```

**Ergebnis**: ~5 MB

⚠️ Benutzer müssen .NET 9.0 Runtime installiert haben:
https://dotnet.microsoft.com/download/dotnet/9.0

## 📊 Größenvergleich

| Konfiguration | Größe | Runtime benötigt? | Kompatibilität |
|--------------|-------|-------------------|----------------|
| **Self-Contained + Trimmed (Empfohlen)** | **~17 MB** | ❌ **Nein** | ✅ **Hoch** |
| Self-Contained + Full Trim | ~12-15 MB | ❌ Nein | ⚠️ Mittel |
| Native AOT | ~8-12 MB | ❌ Nein | ⚠️ Niedrig |
| Self-Contained (ohne Trim) | ~60-80 MB | ❌ Nein | ✅ Sehr hoch |
| Framework-Dependent | ~5 MB | ✅ Ja | ✅ Hoch |

## 🎯 Empfehlung nach Anwendungsfall

### Für private Nutzung / kleine Gruppe
```cmd
dotnet publish -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true /p:PublishTrimmed=true /p:TrimMode=partial
```
- Balance zwischen Größe und Zuverlässigkeit
- Keine Installation beim Benutzer nötig

### Für öffentliche Distribution / viele Benutzer
```cmd
dotnet publish -c Release -r win-x64 --self-contained false /p:PublishSingleFile=true
```
- Kleinste Dateigröße für Downloads
- Benutzer installiert .NET Runtime einmalig
- Alle Updates nutzen dieselbe Runtime

### Für maximale Kompatibilität (ältere Windows-Versionen)
```cmd
dotnet publish -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true
```
- Größer, aber funktioniert garantiert
- Keine Trim-Optimierungen = keine Überraschungen

## 🔧 Troubleshooting

### Trim Warnings (IL2026)

Diese Warnungen sind normal und können ignoriert werden, solange die Anwendung korrekt funktioniert.

Um sie zu beheben, müsste man die JSON-Serialisierung auf Source Generators umstellen (bereits im Code vorbereitet mit `LeagueJsonContext`).

### Größere Dateigröße als erwartet

1. Prüfe ob `/p:PublishTrimmed=true` aktiv ist
2. Nutze `/p:EnableCompressionInSingleFile=true`
3. Entferne Debug-Symbole: `/p:DebugType=none`

### Anwendung startet nicht

1. Stelle sicher, dass Windows x64 ist (nicht x86 oder ARM)
2. Prüfe Windows Defender / Antivirus
3. Versuche ohne Trimming: `--self-contained true /p:PublishSingleFile=true`

## 📝 Weitere Runtime Identifiers

Für andere Zielplattformen:

```cmd
# Windows 32-bit
-r win-x86

# Windows ARM64 (Surface Pro X, etc.)
-r win-arm64

# Portable (funktioniert auf allen Windows-Systemen, aber größer)
-r win
```

## ✅ Finale Empfehlung

**Für dein Auto-Accept Tool:**

```cmd
dotnet publish -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true /p:PublishTrimmed=true /p:TrimMode=partial
```

**Resultat**: 
- ✅ Einzelne 17 MB .exe Datei
- ✅ Funktioniert ohne Installation
- ✅ Einfach zu teilen
- ✅ Zuverlässig

**Distribution**:
1. Erstelle die .exe mit obigem Befehl
2. Kopiere `BE.League.Desktop.AutoAccept.exe` aus dem publish-Ordner
3. Fertig! Die Datei kann direkt ausgeführt werden

Die .exe enthält:
- Die komplette .NET 9.0 Runtime
- Alle benötigten Bibliotheken (Spectre.Console, etc.)
- Deine Anwendung

**Keine weitere Installation oder Konfiguration nötig!** 🎉

