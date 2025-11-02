$testFiles = @(
    "WhenDeserializingAbilities.cs",
    "WhenDeserializingPlayerList.cs",
    "WhenDeserializingPlayerScores.cs",
    "WhenDeserializingPlayerItems.cs",
    "WhenDeserializingPlayerRunes.cs",
    "WhenDeserializingSummonerSpells.cs",
    "WhenDeserializingEvents.cs",
    "WhenDeserializingGameData.cs",
    "WhenDeserializingRunes.cs"
)

$basePath = "C:\dev\Boas\League.Desktop\src\BE.League.Desktop.Tests\LiveClientObjectReaderTests"

foreach ($fileName in $testFiles) {
    $filePath = Join-Path $basePath $fileName
    if (Test-Path $filePath) {
        $content = Get-Content $filePath -Raw
        
        # Replace Gateway references with LiveApi
        $content = $content -replace 'Gateway\.', 'LiveApi.'
        
        # Replace Sut references with LiveReader
        $content = $content -replace 'await Sut\.', 'await LiveReader.'
        
        Set-Content -Path $filePath -Value $content -NoNewline
        Write-Host "Updated: $fileName"
    }
}

