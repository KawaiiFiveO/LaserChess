# Starts the local server, one game window, and the bot as your opponent.

$bin = Join-Path $PSScriptRoot 'bin\Release'
if (-not (Test-Path (Join-Path $bin 'LaserChess.exe'))) {
    $bin = Join-Path $PSScriptRoot 'bin\Debug'
}
if (-not (Test-Path (Join-Path $bin 'LaserChess.exe'))) {
    Write-Host 'LaserChess.exe not found. Build LaserChess.sln first.'
    Read-Host 'Press Enter to exit'
    exit 1
}

Start-Process -FilePath (Join-Path $bin 'LaserChessServer.exe') `
    -ArgumentList '--bot', '--name1', 'You', '--name2', 'Bot' `
    -WorkingDirectory $bin
Start-Process -FilePath (Join-Path $bin 'LaserChess.exe') -WorkingDirectory $bin
