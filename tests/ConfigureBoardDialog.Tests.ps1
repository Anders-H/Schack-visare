# Run after building ChessEngine, using Windows PowerShell (.NET Framework):
# powershell.exe -NoProfile -STA -File tests/ConfigureBoardDialog.Tests.ps1
param(
    [string]$AssemblyPath = "$PSScriptRoot/../ChessEngine/ChessEngine/bin/Debug/ChessEngine.exe"
)

$ErrorActionPreference = 'Stop'
Add-Type -AssemblyName System.Windows.Forms
[void][Reflection.Assembly]::LoadFrom((Resolve-Path $AssemblyPath).Path)

# Asymmetric position: detects transposition, reflection and rotation independently.
$parser = [ChessEngine.ExternalParsers.FenParser]::new('4k2r/1p6/2n5/8/5B2/8/P7/R2QK3 w - - 0 1')
$board = [ChessEngine.BoardControl]::new()
try {
    $board.SetPosition([ChessEngine.BoardData]::new($parser.ParsePosition()))
    $expected = @{
        4 = 'BlackKing'; 7 = 'BlackRook'; 9 = 'BlackPawn'; 18 = 'BlackKnight'
        37 = 'WhiteBishop'; 48 = 'WhitePawn'; 56 = 'WhiteRook'
        59 = 'WhiteQueen'; 60 = 'WhiteKing'
    }
    foreach ($black in @($false, $true)) {
        foreach ($archon in @($false, $true)) {
            $board.SetPerspective($black, $archon)
            $dialog = [ChessEngine.Dialogs.ConfigureBoardDialog]::new()
            try {
                $dialog.SetBoardData($board.GetBasicBoardData())
                # Inspect actual on-screen positions, independent of GetComboBox's mapping.
                $squares = @($dialog.Controls |
                    Where-Object { $_ -is [System.Windows.Forms.ComboBox] } |
                    Sort-Object Top, Left)
                if ($squares.Count -ne 64) { throw 'Expected 64 squares' }
                for ($i = 0; $i -lt 64; $i++) {
                    $piece = if ($expected.ContainsKey($i)) { $expected[$i] } else { 'None' }
                    if ([string]$squares[$i].SelectedItem -ne $piece) {
                        throw "Square $i (black=$black, archon=$archon): expected $piece, got $($squares[$i].SelectedItem)"
                    }
                }
            }
            finally { $dialog.Dispose() }
        }
    }
    Write-Output 'PASS: all 64 squares are correct in all four perspectives.'
}
finally { $board.Dispose() }
