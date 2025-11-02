$lcuTestFiles = @(
    "WhenDeserializingLobby.cs",
    "WhenDeserializingChampSelectSession.cs",
    "WhenDeserializingReadyCheck.cs"
)

$basePath = "C:\dev\Boas\League.Desktop\src\BE.League.Desktop.Tests\LiveClientObjectReaderTests"

foreach ($fileName in $lcuTestFiles) {
    $filePath = Join-Path $basePath $fileName
    if (Test-Path $filePath) {
        $content = Get-Content $filePath -Raw
        
        # Replace Gateway references with LcuApi
        $content = $content -replace 'Gateway\.', 'LcuApi.'
        
        # Replace Sut references with LcuReader
        $content = $content -replace 'await Sut\.', 'await LcuReader.'
        
        Set-Content -Path $filePath -Value $content -NoNewline
        Write-Host "Updated: $fileName"
    }
}

