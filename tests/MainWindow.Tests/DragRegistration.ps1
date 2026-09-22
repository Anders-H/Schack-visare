# Run with Windows PowerShell -STA after building ChessEngine in Debug.
$ErrorActionPreference = 'Stop'
Add-Type -AssemblyName System.Windows.Forms
[void][Reflection.Assembly]::LoadFrom((Resolve-Path "$PSScriptRoot/../../ChessEngine/ChessEngine/bin/Debug/ChessEngine.exe").Path)
$flags = [Reflection.BindingFlags]'Instance,NonPublic'
$window = New-Object ChessEngine.MainWindow
$board = $window.GetType().GetField('boardControl1', $flags).GetValue($window)
function Invoke-Private($target, $name, [object[]]$values) {
    $target.GetType().GetMethod($name, $flags).Invoke($target, $values)
}
function Assert($condition, $message) {
    if (!$condition) { throw $message }
}
function Load-Game($moves) {
    $parser = New-Object ChessEngine.GameParser("Test;2026-09-22;W;B;$moves")
    $game = $parser.Parse()
    Assert $game.Success $game.Message
    $window.Moves = $game.Moves
    Invoke-Private $window 'RenderMoveList' @()
    Invoke-Private $window 'GoToMove' @(-1)
    $board.Size = New-Object Drawing.Size(480,480)
}
function Start-Drag {
    $start = New-Object Drawing.Point(4,1)
    Invoke-Private $board 'BeginPieceDrag' @($start.PSObject.BaseObject)
}
function Drop-At($x, $y, $own = $true) {
    $data = New-Object Windows.Forms.DataObject
    if ($own) { $data.SetData([ChessEngine.BoardControl], $board) }
    $screen = $board.PointToScreen((New-Object Drawing.Point($x,$y)))
    $event = New-Object Windows.Forms.DragEventArgs($data, 0, $screen.X, $screen.Y, [Windows.Forms.DragDropEffects]::Move, [Windows.Forms.DragDropEffects]::None)
    Invoke-Private $board 'OnDragDrop' @($event.PSObject.BaseObject)
}
try {
    foreach ($black in @($false,$true)) {
        foreach ($archon in @($false,$true)) {
            Load-Game 'E2-E4;'
            # Use an empty move list with the standard initial position.
            $window.Moves.Clear()
            Invoke-Private $window 'GoToMove' @(-1)
            $board.SetPerspective($black,$archon)
            Assert (Start-Drag) 'Drag did not start registration'
            if ($archon) {
                if ($black) { $x=4; $y=3 } else { $x=3; $y=4 }
            } else {
                if ($black) { $x=3; $y=3 } else { $x=4; $y=4 }
            }
            Drop-At ($x*60+30) ($y*60+30)
            Assert ($window.Moves.Count -eq 1) 'Drop did not store exactly one move'
            Assert ($window.Moves[0].ToString() -eq 'E2-E4') 'Incorrect drop coordinates'
            Assert ($window.CurrentMove -eq 0) 'New move was not selected'
        }
    }
    foreach ($cancel in @('same','outside','escape','foreign')) {
        Load-Game 'E2-E4;'
        $window.Moves.Clear()
        Invoke-Private $window 'GoToMove' @(-1)
        $board.SetPerspective($false,$false)
        Assert (Start-Drag) 'Drag did not start'
        switch ($cancel) {
            same { Drop-At 270 390 }
            outside { Drop-At -10 -10 }
            escape { Invoke-Private $board 'CancelPieceDrag' @() }
            foreign {
                Drop-At 270 270 $false
                Assert ($window.Moves.Count -eq 0) 'Foreign drop accepted'
                Invoke-Private $board 'CancelPieceDrag' @()
            }
        }
        Assert ($window.Moves.Count -eq 0) 'Cancellation stored a move'
        Assert (!$window.GetType().GetField('_registerMoveMode',$flags).GetValue($window)) 'Registration remained active'
    }
    Load-Game 'E2-E4;END=DRAW;'
    Assert (!(Start-Drag)) 'Drag accepted after game end'
    Load-Game 'E2-E4;'
    # Starting registration advances to the end, where E2 is now empty.
    Assert (!(Start-Drag)) 'Dragged a piece from a stale position'
    Assert ($window.Moves.Count -eq 1) 'Stale drag modified history'
    $timer = $window.GetType().GetField('_playbackTimer',$flags).GetValue($window)
    Invoke-Private $window 'GoToMove' @(-1)
    $timer.Start()
    Assert (!(Start-Drag)) 'Drag accepted during playback'
    $timer.Stop()
    $window.Moves.Clear()
    Invoke-Private $window 'GoToMove' @(-1)
    $board.SetPerspective($false,$false)
    $board.Size = New-Object Drawing.Size(480,480)
    $click = New-Object Windows.Forms.MouseEventArgs([Windows.Forms.MouseButtons]::Left,1,270,390,0)
    Invoke-Private $board 'OnMouseDown' @($click.PSObject.BaseObject)
    Invoke-Private $board 'OnMouseMove' @($click.PSObject.BaseObject)
    Assert (!$window.GetType().GetField('_registerMoveMode',$flags).GetValue($window)) 'Stationary mouse started a drag'
    Invoke-Private $board 'OnMouseUp' @($click.PSObject.BaseObject)
    Invoke-Private $board 'OnMouseClick' @($click.PSObject.BaseObject)
    Assert ($null -ne $board.SelectedPiece) 'Normal piece selection stopped working'
    Invoke-Private $window 'registerMoveToolStripMenuItem_Click' @($window,[EventArgs]::Empty)
    $destination = New-Object Windows.Forms.MouseEventArgs([Windows.Forms.MouseButtons]::Left,1,270,270,0)
    Invoke-Private $board 'OnMouseClick' @($destination.PSObject.BaseObject)
    Assert ($window.Moves.Count -eq 1) 'Click registration stopped working'
    'Passed drag registration, four perspectives, cancellation, foreign drop, game end, stale position and playback checks.'
}
finally {
    $window.GetType().GetField('_playbackTimer',$flags).GetValue($window).Dispose()
    $window.GetType().GetField('_boldMoveListFont',$flags).GetValue($window).Dispose()
    $window.Dispose()
}
