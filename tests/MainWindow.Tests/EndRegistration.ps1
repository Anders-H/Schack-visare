# Run with Windows PowerShell -STA after building ChessEngine in Debug.
$ErrorActionPreference = 'Stop'
Add-Type -AssemblyName System.Windows.Forms
$assemblyPath = Join-Path $PSScriptRoot '../../ChessEngine/ChessEngine/bin/Debug/ChessEngine.exe'
[void][Reflection.Assembly]::LoadFrom((Resolve-Path $assemblyPath).Path)
$flags = [Reflection.BindingFlags]'Instance,NonPublic'
$window = New-Object ChessEngine.MainWindow
$checks = 0
function Invoke-WindowMethod($name, [object[]]$arguments) {
    $window.GetType().GetMethod($name, $flags).Invoke($window, $arguments)
}
function Get-WindowField($name) {
    $window.GetType().GetField($name, $flags).GetValue($window)
}
function Assert-Registration($enabled) {
    foreach ($name in @('registerMoveToolStripMenuItem', 'btnRegistrera', 'registerGameEndingToolStripMenuItem')) {
        if ((Get-WindowField $name).Enabled -ne $enabled) {
            throw "Unexpected registration state: $name"
        }
        $script:checks++
    }
}
try {
    Assert-Registration $true
    foreach ($ending in @('WHITE', 'BLACK', 'DRAW')) {
        foreach ($prefix in @('', 'E2-E4;', 'E2-E4;E7-E5;')) {
            $parser = New-Object ChessEngine.GameParser("Test;2026-09-19;W;B;${prefix}END=$ending;")
            $game = $parser.Parse()
            if (!$game.Success) { throw $game.Message }
            $window.Moves = $game.Moves
            Invoke-WindowMethod 'RenderMoveList' @()
            foreach ($position in @(-1, ($window.Moves.Count - 1))) {
                Invoke-WindowMethod 'GoToMove' @($position)
                Assert-Registration $false
                if (!(Get-WindowField 'deleteLastMoveToolStripMenuItem').Enabled) {
                    throw 'Deleting the ending must remain enabled'
                }
                # Direct calls must also be blocked, without opening a dialog.
                Invoke-WindowMethod 'registerMoveToolStripMenuItem_Click' @($window, [EventArgs]::Empty)
                Invoke-WindowMethod 'registerGameEndingToolStripMenuItem_Click' @($window, [EventArgs]::Empty)
                if (Get-WindowField '_registerMoveMode') { throw 'Registration started after game end' }
                $countBefore = $window.Moves.Count
                $window.GetType().GetField('_registerMoveMode', $flags).SetValue($window, $true)
                Invoke-WindowMethod 'boardControl1_MoveSelected' @($window, $null)
                $window.GetType().GetField('_registerMoveMode', $flags).SetValue($window, $false)
                if ($window.Moves.Count -ne $countBefore) { throw 'Registered a move after game end' }
            }
            # Reproduce the post-confirmation deletion and refresh sequence.
            $window.Moves.RemoveAt($window.Moves.Count - 1)
            Invoke-WindowMethod 'RenderMoveList' @()
            Invoke-WindowMethod 'GoToMove' @(($window.Moves.Count - 1))
            Assert-Registration $true
            Invoke-WindowMethod 'registerMoveToolStripMenuItem_Click' @($window, [EventArgs]::Empty)
            if (!(Get-WindowField '_registerMoveMode')) { throw 'Cannot register after deleting ending' }
            Assert-Registration $false
            Invoke-WindowMethod 'cancelRegisterMoveToolStripMenuItem_Click' @($window, [EventArgs]::Empty)
            Assert-Registration $true
        }
    }
    Write-Output "Passed $checks registration control checks and handler guards."
}
finally {
    (Get-WindowField '_playbackTimer').Dispose()
    (Get-WindowField '_boldMoveListFont').Dispose()
    $window.Dispose()
}
