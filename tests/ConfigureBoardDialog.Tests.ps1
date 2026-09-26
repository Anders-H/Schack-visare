# Run after building ChessEngine, using Windows PowerShell (.NET Framework):
# powershell.exe -NoProfile -STA -File tests/ConfigureBoardDialog.Tests.ps1
param(
    [string]$AssemblyPath = "$PSScriptRoot/../ChessEngine/ChessEngine/bin/Debug/ChessEngine.exe"
)

$ErrorActionPreference = 'Stop'
Add-Type -AssemblyName System.Windows.Forms
[void][Reflection.Assembly]::LoadFrom((Resolve-Path $AssemblyPath).Path)
$validatePosition = [ChessEngine.Dialogs.ConfigureBoardDialog].GetMethod('GetInitialPosition', [Reflection.BindingFlags]'Instance,NonPublic')
$forcePosition = [ChessEngine.Dialogs.ConfigureBoardDialog].GetMethod('GetInitialPositionForced', [Reflection.BindingFlags]'Instance,NonPublic')

# Asymmetric position: detects transposition, reflection and rotation independently.
$parser = [ChessEngine.ExternalParsers.FenParser]::new('4k2r/1p6/2n5/8/5B2/8/P7/R2QK3 w Qk - 0 1')
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
                if ($validatePosition.Invoke($dialog, @()).Fen -ne $parser.ParsePosition().Fen) {
                    throw 'Dialog export changed the position'
                }
            }
            finally { $dialog.Dispose() }
        }
    }
    Write-Output 'PASS: all 64 squares are correct in all four perspectives.'
}
finally { $board.Dispose() }

# Exercise the menu handler and real OK/Cancel buttons through the modal dialog.
$window = [ChessEngine.MainWindow]::new()
$timer = [System.Windows.Forms.Timer]::new()
$timer.Interval = 100
$flags = [Reflection.BindingFlags]'Instance,NonPublic'
$handler = $window.GetType().GetMethod('configureBoardToolStripMenuItem_Click', $flags)
$script:cancelConfiguration = $false
$script:dialogFailure = $null
$timer.add_Tick({
    $timer.Stop()
    $dialog = @([System.Windows.Forms.Application]::OpenForms |
        Where-Object { $_ -is [ChessEngine.Dialogs.ConfigureBoardDialog] })[0]
    try {
        # Populate every combo from a known asymmetric position, then edit a square.
        $source = [ChessEngine.BoardControl]::new()
        try {
            $source.SetPosition([ChessEngine.BoardData]::new($parser.ParsePosition()))
            $dialog.SetBoardData($source.GetBasicBoardData())
        }
        finally { $source.Dispose() }
        $squares = @($dialog.Controls | Where-Object { $_ -is [System.Windows.Forms.ComboBox] } | Sort-Object Top, Left)
        $squares[37].SelectedItem = [ChessEngine.Dialogs.DialogControls.PieceAtSquare]::None
        $squares[30].SelectedItem = [ChessEngine.Dialogs.DialogControls.PieceAtSquare]::WhiteBishop
        $button = if ($script:cancelConfiguration) { 'btnCancel' } else { 'btnOk' }
        $dialog.Controls[$button].PerformClick()
    }
    catch {
        $script:dialogFailure = $_
        $dialog.Close()
    }
})
try {
    $timer.Start()
    [void]$handler.Invoke($window, @($window, [EventArgs]::Empty))
    if ($script:dialogFailure) { throw $script:dialogFailure }
    $expectedFen = '4k2r/1p6/2n5/6B1/8/8/P7/R2QK3 w Qk - 0 1'
    if ($window.Moves.InitialPosition.Fen -ne $expectedFen) { throw 'OK did not apply the edited start position' }
    $visibleBoard = $window.Controls.Find('boardControl1', $true)[0]
    if ($visibleBoard.GetPieceAt(6, 4).Type.ToString() -ne 'Bishop' -or
        $null -ne $visibleBoard.GetPieceAt(5, 3)) { throw 'Displayed board was not rebuilt' }
    $saved = [ChessEngine.GameFileFormat]::Serialize('Test', [DateTime]::Today, 'White', 'Black', $window.Moves)
    $loaded = [ChessEngine.GameParser]::new($saved).Parse()
    if (!$loaded.Success -or $loaded.Moves.InitialPosition.Fen -ne $expectedFen) { throw 'Save/load lost the start position' }

    # Play a move, then rewind through MainWindow's normal replay path.
    $piece = $visibleBoard.GetPieceAt(0, 1)
    $move = [ChessEngine.Moves.Move]::new([Drawing.Point]::new(0, 1), [Drawing.Point]::new(0, 2), $piece, 0, $null)
    $window.Moves.Add($move)
    $replay = $window.GetType().GetMethod('GoToMove', $flags)
    [void]$replay.Invoke($window, @([int]0))
    if ($visibleBoard.GetPieceAt(0, 2).Type.ToString() -ne 'Pawn') { throw 'Replay failed' }
    [void]$replay.Invoke($window, @([int]-1))
    if ($visibleBoard.GetPieceAt(0, 1).Type.ToString() -ne 'Pawn' -or
        $visibleBoard.GetPieceAt(6, 4).Type.ToString() -ne 'Bishop') { throw 'Rewind lost configured position' }
    $window.Moves.Clear()

    $previousMoves = $window.Moves
    $script:cancelConfiguration = $true
    $timer.Start()
    [void]$handler.Invoke($window, @($window, [EventArgs]::Empty))
    if ($script:dialogFailure) { throw $script:dialogFailure }
    if (![object]::ReferenceEquals($previousMoves, $window.Moves)) { throw 'Cancel modified the game' }
    Write-Output 'PASS: OK, Cancel, board rebuild, save/load and replay preserve the configured start position.'
}
finally {
    $timer.Dispose()
    $window.Dispose()
}

$dialog = [ChessEngine.Dialogs.ConfigureBoardDialog]::new()
try {
    try {
        [void]$validatePosition.Invoke($dialog, @())
        throw 'An empty board was accepted'
    }
    catch [System.Management.Automation.MethodInvocationException] {
        if ($_.Exception.GetBaseException() -isnot [FormatException]) { throw }
    }
    Write-Output 'PASS: an invalid start position is rejected.'

    $forced = $forcePosition.Invoke($dialog, @())
    if ($forced.Fen -ne '8/8/8/8/8/8/8/8 w - - 0 1') { throw 'Forced empty board changed' }
    $squares = @($dialog.Controls | Where-Object { $_ -is [System.Windows.Forms.ComboBox] } | Sort-Object Top, Left)
    # Include all piece types, extra kings, back-rank pawns and empty squares.
    for ($i = 0; $i -lt 12; $i++) {
        $squares[$i].SelectedItem = [ChessEngine.Dialogs.DialogControls.PieceAtSquare]($i + 1)
    }
    $squares[63].SelectedItem = [ChessEngine.Dialogs.DialogControls.PieceAtSquare]::WhiteKing
    $forced = $forcePosition.Invoke($dialog, @())
    $data = [ChessEngine.BoardData]::new($forced)
    $types = @('Pawn', 'Rook', 'Knight', 'Bishop', 'Queen', 'King')
    $ids = @{}
    for ($i = 0; $i -lt 64; $i++) {
        $piece = $data[(7 - [int][Math]::Floor($i / 8)), ($i % 8)]
        if ($i -ge 12 -and $i -ne 63) {
            if ($null -ne $piece) { throw "Forced position filled empty square $i" }
            continue
        }
        $type = if ($i -eq 63) { 'King' } else { $types[$i % 6] }
        $color = if ($i -lt 6 -or $i -eq 63) { 'White' } else { 'Black' }
        if ($piece.Type.ToString() -ne $type -or $piece.Color.ToString() -ne $color) {
            throw "Forced position changed square $i"
        }
        if ($ids.ContainsKey($piece.PieceId)) { throw 'Duplicate piece ID' }
        $ids[$piece.PieceId] = $true
    }
    if (!$forced.IsWhitesTurn -or $null -ne $forced.EnPassantTarget -or $forced.FullmoveNumber -ne 1) {
        throw 'Incorrect forced position metadata'
    }
    Write-Output 'PASS: forced positions preserve empty squares, every piece type, extra kings and back-rank pawns.'
}
finally { $dialog.Dispose() }

# Both creation paths must give the rules engine and saved game the same rights.
$dialog = [ChessEngine.Dialogs.ConfigureBoardDialog]::new()
$source = [ChessEngine.BoardControl]::new()
try {
    $position = [ChessEngine.ExternalParsers.FenParser]::new('r3k2r/8/8/8/8/8/8/R3K2R w - - 0 1').ParsePosition()
    $source.SetPosition([ChessEngine.BoardData]::new($position))
    $dialog.SetBoardData($source.GetBasicBoardData())
    foreach ($factory in @($validatePosition, $forcePosition)) {
        $initial = $factory.Invoke($dialog, @())
        if ($initial.Fen -ne 'r3k2r/8/8/8/8/8/8/R3K2R w KQkq - 0 1') { throw 'Missing castling rights' }
        $moves = [ChessEngine.Moves.MoveList]::new()
        $moves.InitialPosition = $initial
        $saved = [ChessEngine.GameFileFormat]::Serialize('Castling', [DateTime]::Today, 'White', 'Black', $moves)
        $loaded = [ChessEngine.GameParser]::new($saved).Parse()
        if (!$loaded.Success) { throw 'Could not reload castling position' }
        foreach ($game in @($moves, $loaded.Moves)) {
            $data = [ChessEngine.BoardData]::new($game.InitialPosition)
            foreach ($row in @(0, 7)) {
                $king = $data[$row, 4]
                if ($king.MoveCount -ne 0) { throw 'Unmoved king has a move count' }
                $rules = [ChessEngine.ChessRules]::new(($row -eq 0), $game)
                foreach ($column in @(2, 6)) {
                    if (!$rules.IsMoveLegal($king, [Drawing.Point]::new(4, $row), [Drawing.Point]::new($column, $row), $null)) {
                        throw "Castling rejected: row=$row column=$column"
                    }
                }
            }
        }
        # Moving the king away and back must permanently revoke its rights.
        $data = [ChessEngine.BoardData]::new($initial)
        foreach ($step in @(@(4,0,4,1), @(4,1,4,0))) {
            $move = [ChessEngine.Moves.Move]::new([Drawing.Point]::new($step[0],$step[1]), [Drawing.Point]::new($step[2],$step[3]), ($data[($step[1]),($step[0])]), $moves.Count, $null)
            $moves.Add($move)
            $data.ApplyMove($move)
        }
        $rules = [ChessEngine.ChessRules]::new($true, $moves)
        if ($rules.IsMoveLegal(($data[0,4]), [Drawing.Point]::new(4,0), [Drawing.Point]::new(6,0), $null)) {
            throw 'Moved king retained castling rights'
        }
    }
    Write-Output 'PASS: both creation paths support all four castlings before/after save, and reject castling after a king move.'
}
finally { $source.Dispose(); $dialog.Dispose() }
