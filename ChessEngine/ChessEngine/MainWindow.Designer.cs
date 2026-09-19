using ChessEngine.Events;

namespace ChessEngine
{
    partial class MainWindow
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainWindow));
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newGameToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openFromClipboardToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.portableGameNotationPGNToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.forsythEdwardsNotationFENToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.saveAsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem2 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.viewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fromWhitesPerspectiveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fromBlacksPerspectiveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem5 = new System.Windows.Forms.ToolStripSeparator();
            this.archonViewToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem4 = new System.Windows.Forms.ToolStripSeparator();
            this.showWhiteCoverageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showBlackCoverageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.showSelectedPieceCoverageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.moveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.registerMoveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.cancelRegisterMoveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.deleteLastMoveToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem3 = new System.Windows.Forms.ToolStripSeparator();
            this.firstToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.previousToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.playToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pauseToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.nextToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.lastToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.helpToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openTheManualToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.reportABugToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.statusStrip1 = new System.Windows.Forms.StatusStrip();
            this.lblStatus = new System.Windows.Forms.ToolStripStatusLabel();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.btnNewGame = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.btnOpen = new System.Windows.Forms.ToolStripButton();
            this.btnSave = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
            this.btnRotateBoard = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator5 = new System.Windows.Forms.ToolStripSeparator();
            this.btnRegistrera = new System.Windows.Forms.ToolStripButton();
            this.btnAvbrytRegistrering = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.btnFirst = new System.Windows.Forms.ToolStripButton();
            this.btnPrevious = new System.Windows.Forms.ToolStripButton();
            this.btnPlay = new System.Windows.Forms.ToolStripButton();
            this.btnPause = new System.Windows.Forms.ToolStripButton();
            this.btnNext = new System.Windows.Forms.ToolStripButton();
            this.btnLast = new System.Windows.Forms.ToolStripButton();
            this.listView1 = new System.Windows.Forms.ListView();
            this.imageList1 = new System.Windows.Forms.ImageList(this.components);
            this.lvProperties = new System.Windows.Forms.ListView();
            this.panel1 = new System.Windows.Forms.Panel();
            this.boardControl1 = new ChessEngine.BoardControl();
            this.registerGameEndingToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.menuStrip1.SuspendLayout();
            this.statusStrip1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.viewToolStripMenuItem,
            this.moveToolStripMenuItem,
            this.helpToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(867, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newGameToolStripMenuItem,
            this.toolStripSeparator2,
            this.openToolStripMenuItem,
            this.openFromClipboardToolStripMenuItem,
            this.saveToolStripMenuItem,
            this.saveAsToolStripMenuItem,
            this.toolStripMenuItem2,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(37, 20);
            this.fileToolStripMenuItem.Text = "&File";
            // 
            // newGameToolStripMenuItem
            // 
            this.newGameToolStripMenuItem.Image = global::ChessEngine.Properties.Resources.NewDocumentHS;
            this.newGameToolStripMenuItem.Name = "newGameToolStripMenuItem";
            this.newGameToolStripMenuItem.Size = new System.Drawing.Size(185, 22);
            this.newGameToolStripMenuItem.Text = "&New game";
            this.newGameToolStripMenuItem.Click += new System.EventHandler(this.newGameToolStripMenuItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(182, 6);
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Image = global::ChessEngine.Properties.Resources.openHS;
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.Size = new System.Drawing.Size(185, 22);
            this.openToolStripMenuItem.Text = "Open...";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.openToolStripMenuItem_Click);
            // 
            // openFromClipboardToolStripMenuItem
            // 
            this.openFromClipboardToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.portableGameNotationPGNToolStripMenuItem,
            this.forsythEdwardsNotationFENToolStripMenuItem});
            this.openFromClipboardToolStripMenuItem.Image = global::ChessEngine.Properties.Resources.PasteHS;
            this.openFromClipboardToolStripMenuItem.Name = "openFromClipboardToolStripMenuItem";
            this.openFromClipboardToolStripMenuItem.Size = new System.Drawing.Size(185, 22);
            this.openFromClipboardToolStripMenuItem.Text = "Open from clipboard";
            // 
            // portableGameNotationPGNToolStripMenuItem
            // 
            this.portableGameNotationPGNToolStripMenuItem.Name = "portableGameNotationPGNToolStripMenuItem";
            this.portableGameNotationPGNToolStripMenuItem.Size = new System.Drawing.Size(253, 22);
            this.portableGameNotationPGNToolStripMenuItem.Text = "Portable Game Notation (PGN)...";
            this.portableGameNotationPGNToolStripMenuItem.Click += new System.EventHandler(this.portableGameNotationPGNToolStripMenuItem_Click);
            // 
            // forsythEdwardsNotationFENToolStripMenuItem
            // 
            this.forsythEdwardsNotationFENToolStripMenuItem.Name = "forsythEdwardsNotationFENToolStripMenuItem";
            this.forsythEdwardsNotationFENToolStripMenuItem.Size = new System.Drawing.Size(253, 22);
            this.forsythEdwardsNotationFENToolStripMenuItem.Text = "Forsyth-Edwards Notation (FEN)...";
            this.forsythEdwardsNotationFENToolStripMenuItem.Click += new System.EventHandler(this.forsythEdwardsNotationFENToolStripMenuItem_Click);
            // 
            // saveToolStripMenuItem
            // 
            this.saveToolStripMenuItem.Image = global::ChessEngine.Properties.Resources.saveHS;
            this.saveToolStripMenuItem.Name = "saveToolStripMenuItem";
            this.saveToolStripMenuItem.Size = new System.Drawing.Size(185, 22);
            this.saveToolStripMenuItem.Text = "Save";
            this.saveToolStripMenuItem.Click += new System.EventHandler(this.saveToolStripMenuItem_Click);
            // 
            // saveAsToolStripMenuItem
            // 
            this.saveAsToolStripMenuItem.Name = "saveAsToolStripMenuItem";
            this.saveAsToolStripMenuItem.Size = new System.Drawing.Size(185, 22);
            this.saveAsToolStripMenuItem.Text = "Save as...";
            this.saveAsToolStripMenuItem.Click += new System.EventHandler(this.saveAsToolStripMenuItem_Click);
            // 
            // toolStripMenuItem2
            // 
            this.toolStripMenuItem2.Name = "toolStripMenuItem2";
            this.toolStripMenuItem2.Size = new System.Drawing.Size(182, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(185, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.exitToolStripMenuItem_Click);
            // 
            // viewToolStripMenuItem
            // 
            this.viewToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fromWhitesPerspectiveToolStripMenuItem,
            this.fromBlacksPerspectiveToolStripMenuItem,
            this.toolStripMenuItem5,
            this.archonViewToolStripMenuItem,
            this.toolStripMenuItem4,
            this.showWhiteCoverageToolStripMenuItem,
            this.showBlackCoverageToolStripMenuItem,
            this.showSelectedPieceCoverageToolStripMenuItem});
            this.viewToolStripMenuItem.Name = "viewToolStripMenuItem";
            this.viewToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.viewToolStripMenuItem.Text = "&View";
            // 
            // fromWhitesPerspectiveToolStripMenuItem
            // 
            this.fromWhitesPerspectiveToolStripMenuItem.Checked = true;
            this.fromWhitesPerspectiveToolStripMenuItem.CheckState = System.Windows.Forms.CheckState.Checked;
            this.fromWhitesPerspectiveToolStripMenuItem.Name = "fromWhitesPerspectiveToolStripMenuItem";
            this.fromWhitesPerspectiveToolStripMenuItem.Size = new System.Drawing.Size(231, 22);
            this.fromWhitesPerspectiveToolStripMenuItem.Text = "From white\'s perspective";
            this.fromWhitesPerspectiveToolStripMenuItem.Click += new System.EventHandler(this.fromWhitesPerspectiveToolStripMenuItem_Click);
            // 
            // fromBlacksPerspectiveToolStripMenuItem
            // 
            this.fromBlacksPerspectiveToolStripMenuItem.Name = "fromBlacksPerspectiveToolStripMenuItem";
            this.fromBlacksPerspectiveToolStripMenuItem.Size = new System.Drawing.Size(231, 22);
            this.fromBlacksPerspectiveToolStripMenuItem.Text = "From black\'s perspective";
            this.fromBlacksPerspectiveToolStripMenuItem.Click += new System.EventHandler(this.fromBlacksPerspectiveToolStripMenuItem_Click);
            // 
            // toolStripMenuItem5
            // 
            this.toolStripMenuItem5.Name = "toolStripMenuItem5";
            this.toolStripMenuItem5.Size = new System.Drawing.Size(228, 6);
            // 
            // archonViewToolStripMenuItem
            // 
            this.archonViewToolStripMenuItem.Name = "archonViewToolStripMenuItem";
            this.archonViewToolStripMenuItem.Size = new System.Drawing.Size(231, 22);
            this.archonViewToolStripMenuItem.Text = "Archon view";
            this.archonViewToolStripMenuItem.Click += new System.EventHandler(this.archonViewToolStripMenuItem_Click);
            // 
            // toolStripMenuItem4
            // 
            this.toolStripMenuItem4.Name = "toolStripMenuItem4";
            this.toolStripMenuItem4.Size = new System.Drawing.Size(228, 6);
            // 
            // showWhiteCoverageToolStripMenuItem
            // 
            this.showWhiteCoverageToolStripMenuItem.Name = "showWhiteCoverageToolStripMenuItem";
            this.showWhiteCoverageToolStripMenuItem.Size = new System.Drawing.Size(231, 22);
            this.showWhiteCoverageToolStripMenuItem.Text = "Show white coverage";
            this.showWhiteCoverageToolStripMenuItem.Click += new System.EventHandler(this.showWhiteCoverageToolStripMenuItem_Click);
            // 
            // showBlackCoverageToolStripMenuItem
            // 
            this.showBlackCoverageToolStripMenuItem.Name = "showBlackCoverageToolStripMenuItem";
            this.showBlackCoverageToolStripMenuItem.Size = new System.Drawing.Size(231, 22);
            this.showBlackCoverageToolStripMenuItem.Text = "Show black coverage";
            this.showBlackCoverageToolStripMenuItem.Click += new System.EventHandler(this.showBlackCoverageToolStripMenuItem_Click);
            // 
            // showSelectedPieceCoverageToolStripMenuItem
            // 
            this.showSelectedPieceCoverageToolStripMenuItem.Name = "showSelectedPieceCoverageToolStripMenuItem";
            this.showSelectedPieceCoverageToolStripMenuItem.Size = new System.Drawing.Size(231, 22);
            this.showSelectedPieceCoverageToolStripMenuItem.Text = "Show selected piece coverage";
            this.showSelectedPieceCoverageToolStripMenuItem.Click += new System.EventHandler(this.showSelectedPieceCoverageToolStripMenuItem_Click);
            // 
            // moveToolStripMenuItem
            // 
            this.moveToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.registerMoveToolStripMenuItem,
            this.cancelRegisterMoveToolStripMenuItem,
            this.registerGameEndingToolStripMenuItem,
            this.toolStripMenuItem1,
            this.deleteLastMoveToolStripMenuItem,
            this.toolStripMenuItem3,
            this.firstToolStripMenuItem,
            this.previousToolStripMenuItem,
            this.playToolStripMenuItem,
            this.pauseToolStripMenuItem,
            this.nextToolStripMenuItem,
            this.lastToolStripMenuItem});
            this.moveToolStripMenuItem.Name = "moveToolStripMenuItem";
            this.moveToolStripMenuItem.Size = new System.Drawing.Size(49, 20);
            this.moveToolStripMenuItem.Text = "&Move";
            this.moveToolStripMenuItem.DropDownOpening += new System.EventHandler(this.moveToolStripMenuItem_DropDownOpening);
            // 
            // registerMoveToolStripMenuItem
            // 
            this.registerMoveToolStripMenuItem.Image = global::ChessEngine.Properties.Resources._112_Plus_Green_16x16_72;
            this.registerMoveToolStripMenuItem.Name = "registerMoveToolStripMenuItem";
            this.registerMoveToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.Insert;
            this.registerMoveToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
            this.registerMoveToolStripMenuItem.Text = "Register move";
            this.registerMoveToolStripMenuItem.Click += new System.EventHandler(this.registerMoveToolStripMenuItem_Click);
            // 
            // cancelRegisterMoveToolStripMenuItem
            // 
            this.cancelRegisterMoveToolStripMenuItem.Enabled = false;
            this.cancelRegisterMoveToolStripMenuItem.Image = global::ChessEngine.Properties.Resources.Cancel__Red;
            this.cancelRegisterMoveToolStripMenuItem.Name = "cancelRegisterMoveToolStripMenuItem";
            this.cancelRegisterMoveToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
            this.cancelRegisterMoveToolStripMenuItem.Text = "Cancel register move";
            this.cancelRegisterMoveToolStripMenuItem.Click += new System.EventHandler(this.cancelRegisterMoveToolStripMenuItem_Click);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(186, 6);
            // 
            // deleteLastMoveToolStripMenuItem
            // 
            this.deleteLastMoveToolStripMenuItem.Image = global::ChessEngine.Properties.Resources.DeleteRed;
            this.deleteLastMoveToolStripMenuItem.Name = "deleteLastMoveToolStripMenuItem";
            this.deleteLastMoveToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
            this.deleteLastMoveToolStripMenuItem.Text = "Delete last move";
            this.deleteLastMoveToolStripMenuItem.Click += new System.EventHandler(this.deleteLastMoveToolStripMenuItem_Click);
            // 
            // toolStripMenuItem3
            // 
            this.toolStripMenuItem3.Name = "toolStripMenuItem3";
            this.toolStripMenuItem3.Size = new System.Drawing.Size(186, 6);
            // 
            // firstToolStripMenuItem
            // 
            this.firstToolStripMenuItem.Image = global::ChessEngine.Properties.Resources.DataContainer_MoveFirstHS;
            this.firstToolStripMenuItem.Name = "firstToolStripMenuItem";
            this.firstToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
            this.firstToolStripMenuItem.Text = "First";
            this.firstToolStripMenuItem.Click += new System.EventHandler(this.firstToolStripMenuItem_Click);
            // 
            // previousToolStripMenuItem
            // 
            this.previousToolStripMenuItem.Image = global::ChessEngine.Properties.Resources.DataContainer_MovePreviousHS;
            this.previousToolStripMenuItem.Name = "previousToolStripMenuItem";
            this.previousToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
            this.previousToolStripMenuItem.Text = "Previous";
            this.previousToolStripMenuItem.Click += new System.EventHandler(this.previousToolStripMenuItem_Click);
            // 
            // playToolStripMenuItem
            // 
            this.playToolStripMenuItem.Image = global::ChessEngine.Properties.Resources.FormRunHS;
            this.playToolStripMenuItem.Name = "playToolStripMenuItem";
            this.playToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
            this.playToolStripMenuItem.Text = "Play";
            this.playToolStripMenuItem.Click += new System.EventHandler(this.playToolStripMenuItem_Click);
            // 
            // pauseToolStripMenuItem
            // 
            this.pauseToolStripMenuItem.Image = global::ChessEngine.Properties.Resources.PauseHS;
            this.pauseToolStripMenuItem.Name = "pauseToolStripMenuItem";
            this.pauseToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
            this.pauseToolStripMenuItem.Text = "Pause";
            this.pauseToolStripMenuItem.Click += new System.EventHandler(this.pauseToolStripMenuItem_Click);
            // 
            // nextToolStripMenuItem
            // 
            this.nextToolStripMenuItem.Image = global::ChessEngine.Properties.Resources.DataContainer_MoveNextHS;
            this.nextToolStripMenuItem.Name = "nextToolStripMenuItem";
            this.nextToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
            this.nextToolStripMenuItem.Text = "Next";
            this.nextToolStripMenuItem.Click += new System.EventHandler(this.nextToolStripMenuItem_Click);
            // 
            // lastToolStripMenuItem
            // 
            this.lastToolStripMenuItem.Image = global::ChessEngine.Properties.Resources.DataContainer_MoveLastHS;
            this.lastToolStripMenuItem.Name = "lastToolStripMenuItem";
            this.lastToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
            this.lastToolStripMenuItem.Text = "Last";
            this.lastToolStripMenuItem.Click += new System.EventHandler(this.lastToolStripMenuItem_Click);
            // 
            // helpToolStripMenuItem
            // 
            this.helpToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem,
            this.openTheManualToolStripMenuItem,
            this.reportABugToolStripMenuItem});
            this.helpToolStripMenuItem.Name = "helpToolStripMenuItem";
            this.helpToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.helpToolStripMenuItem.Text = "&Help";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(194, 22);
            this.aboutToolStripMenuItem.Text = "About...";
            this.aboutToolStripMenuItem.Click += new System.EventHandler(this.aboutToolStripMenuItem_Click);
            // 
            // openTheManualToolStripMenuItem
            // 
            this.openTheManualToolStripMenuItem.Image = global::ChessEngine.Properties.Resources.Help;
            this.openTheManualToolStripMenuItem.Name = "openTheManualToolStripMenuItem";
            this.openTheManualToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.F1;
            this.openTheManualToolStripMenuItem.Size = new System.Drawing.Size(194, 22);
            this.openTheManualToolStripMenuItem.Text = "Open the manual...";
            this.openTheManualToolStripMenuItem.Click += new System.EventHandler(this.openTheManualToolStripMenuItem_Click);
            // 
            // reportABugToolStripMenuItem
            // 
            this.reportABugToolStripMenuItem.Name = "reportABugToolStripMenuItem";
            this.reportABugToolStripMenuItem.Size = new System.Drawing.Size(194, 22);
            this.reportABugToolStripMenuItem.Text = "Report a bug...";
            this.reportABugToolStripMenuItem.Click += new System.EventHandler(this.reportABugToolStripMenuItem_Click);
            // 
            // statusStrip1
            // 
            this.statusStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.lblStatus});
            this.statusStrip1.Location = new System.Drawing.Point(0, 728);
            this.statusStrip1.Name = "statusStrip1";
            this.statusStrip1.Size = new System.Drawing.Size(867, 22);
            this.statusStrip1.TabIndex = 1;
            this.statusStrip1.Text = "statusStrip1";
            // 
            // lblStatus
            // 
            this.lblStatus.Name = "lblStatus";
            this.lblStatus.Size = new System.Drawing.Size(19, 17);
            this.lblStatus.Text = "    ";
            // 
            // toolStrip1
            // 
            this.toolStrip1.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.btnNewGame,
            this.toolStripSeparator4,
            this.btnOpen,
            this.btnSave,
            this.toolStripSeparator3,
            this.btnRotateBoard,
            this.toolStripSeparator5,
            this.btnRegistrera,
            this.btnAvbrytRegistrering,
            this.toolStripSeparator1,
            this.btnFirst,
            this.btnPrevious,
            this.btnPlay,
            this.btnPause,
            this.btnNext,
            this.btnLast});
            this.toolStrip1.Location = new System.Drawing.Point(0, 24);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(867, 25);
            this.toolStrip1.TabIndex = 3;
            this.toolStrip1.Text = "toolStrip1";
            // 
            // btnNewGame
            // 
            this.btnNewGame.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnNewGame.Image = global::ChessEngine.Properties.Resources.NewDocumentHS;
            this.btnNewGame.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnNewGame.Name = "btnNewGame";
            this.btnNewGame.Size = new System.Drawing.Size(23, 22);
            this.btnNewGame.Text = "New game";
            this.btnNewGame.Click += new System.EventHandler(this.btnNewGame_Click);
            // 
            // toolStripSeparator4
            // 
            this.toolStripSeparator4.Name = "toolStripSeparator4";
            this.toolStripSeparator4.Size = new System.Drawing.Size(6, 25);
            // 
            // btnOpen
            // 
            this.btnOpen.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnOpen.Image = global::ChessEngine.Properties.Resources.openHS;
            this.btnOpen.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnOpen.Name = "btnOpen";
            this.btnOpen.Size = new System.Drawing.Size(23, 22);
            this.btnOpen.Text = "Open...";
            this.btnOpen.Click += new System.EventHandler(this.btnOpen_Click);
            // 
            // btnSave
            // 
            this.btnSave.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnSave.Image = global::ChessEngine.Properties.Resources.saveHS;
            this.btnSave.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnSave.Name = "btnSave";
            this.btnSave.Size = new System.Drawing.Size(23, 22);
            this.btnSave.Text = "Save";
            this.btnSave.Click += new System.EventHandler(this.btnSave_Click);
            // 
            // toolStripSeparator3
            // 
            this.toolStripSeparator3.Name = "toolStripSeparator3";
            this.toolStripSeparator3.Size = new System.Drawing.Size(6, 25);
            // 
            // btnRotateBoard
            // 
            this.btnRotateBoard.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnRotateBoard.Image = global::ChessEngine.Properties.Resources.rotate;
            this.btnRotateBoard.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnRotateBoard.Name = "btnRotateBoard";
            this.btnRotateBoard.Size = new System.Drawing.Size(23, 22);
            this.btnRotateBoard.Text = "Rotate board";
            this.btnRotateBoard.Click += new System.EventHandler(this.btnRotateBoard_Click);
            // 
            // toolStripSeparator5
            // 
            this.toolStripSeparator5.Name = "toolStripSeparator5";
            this.toolStripSeparator5.Size = new System.Drawing.Size(6, 25);
            // 
            // btnRegistrera
            // 
            this.btnRegistrera.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnRegistrera.Image = global::ChessEngine.Properties.Resources._112_Plus_Green_16x16_72;
            this.btnRegistrera.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnRegistrera.Name = "btnRegistrera";
            this.btnRegistrera.Size = new System.Drawing.Size(23, 22);
            this.btnRegistrera.Text = "Register move";
            this.btnRegistrera.Click += new System.EventHandler(this.btnRegistrera_Click);
            // 
            // btnAvbrytRegistrering
            // 
            this.btnAvbrytRegistrering.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnAvbrytRegistrering.Enabled = false;
            this.btnAvbrytRegistrering.Image = global::ChessEngine.Properties.Resources.Cancel__Red;
            this.btnAvbrytRegistrering.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnAvbrytRegistrering.Name = "btnAvbrytRegistrering";
            this.btnAvbrytRegistrering.Size = new System.Drawing.Size(23, 22);
            this.btnAvbrytRegistrering.Text = "Cancel register move";
            this.btnAvbrytRegistrering.Click += new System.EventHandler(this.btnAvbrytRegistrering_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // btnFirst
            // 
            this.btnFirst.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnFirst.Image = global::ChessEngine.Properties.Resources.DataContainer_MoveFirstHS;
            this.btnFirst.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnFirst.Name = "btnFirst";
            this.btnFirst.Size = new System.Drawing.Size(23, 22);
            this.btnFirst.Text = "First";
            this.btnFirst.Click += new System.EventHandler(this.btnFirst_Click);
            // 
            // btnPrevious
            // 
            this.btnPrevious.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnPrevious.Image = global::ChessEngine.Properties.Resources.DataContainer_MovePreviousHS;
            this.btnPrevious.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnPrevious.Name = "btnPrevious";
            this.btnPrevious.Size = new System.Drawing.Size(23, 22);
            this.btnPrevious.Text = "Previous";
            this.btnPrevious.Click += new System.EventHandler(this.btnPrevious_Click);
            // 
            // btnPlay
            // 
            this.btnPlay.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnPlay.Image = global::ChessEngine.Properties.Resources.FormRunHS;
            this.btnPlay.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnPlay.Name = "btnPlay";
            this.btnPlay.Size = new System.Drawing.Size(23, 22);
            this.btnPlay.Text = "Play";
            this.btnPlay.Click += new System.EventHandler(this.btnPlay_Click);
            // 
            // btnPause
            // 
            this.btnPause.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnPause.Image = global::ChessEngine.Properties.Resources.PauseHS;
            this.btnPause.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnPause.Name = "btnPause";
            this.btnPause.Size = new System.Drawing.Size(23, 22);
            this.btnPause.Text = "Pause";
            this.btnPause.Click += new System.EventHandler(this.btnPause_Click);
            // 
            // btnNext
            // 
            this.btnNext.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnNext.Image = global::ChessEngine.Properties.Resources.DataContainer_MoveNextHS;
            this.btnNext.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnNext.Name = "btnNext";
            this.btnNext.Size = new System.Drawing.Size(23, 22);
            this.btnNext.Text = "Next";
            this.btnNext.Click += new System.EventHandler(this.btnNext_Click);
            // 
            // btnLast
            // 
            this.btnLast.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.btnLast.Image = global::ChessEngine.Properties.Resources.DataContainer_MoveLastHS;
            this.btnLast.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnLast.Name = "btnLast";
            this.btnLast.Size = new System.Drawing.Size(23, 22);
            this.btnLast.Text = "Last";
            this.btnLast.Click += new System.EventHandler(this.btnLast_Click);
            // 
            // listView1
            // 
            this.listView1.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.listView1.Dock = System.Windows.Forms.DockStyle.Left;
            this.listView1.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.listView1.HideSelection = false;
            this.listView1.Location = new System.Drawing.Point(0, 49);
            this.listView1.MultiSelect = false;
            this.listView1.Name = "listView1";
            this.listView1.Size = new System.Drawing.Size(88, 679);
            this.listView1.SmallImageList = this.imageList1;
            this.listView1.TabIndex = 4;
            this.listView1.UseCompatibleStateImageBehavior = false;
            this.listView1.View = System.Windows.Forms.View.List;
            this.listView1.Enter += new System.EventHandler(this.listView1_Enter);
            // 
            // imageList1
            // 
            this.imageList1.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imageList1.ImageStream")));
            this.imageList1.TransparentColor = System.Drawing.Color.Transparent;
            this.imageList1.Images.SetKeyName(0, "Dot.png");
            this.imageList1.Images.SetKeyName(1, "Circle_Grey.png");
            this.imageList1.Images.SetKeyName(2, "Circle_Blue.png");
            // 
            // lvProperties
            // 
            this.lvProperties.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lvProperties.Dock = System.Windows.Forms.DockStyle.Right;
            this.lvProperties.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.lvProperties.HideSelection = false;
            this.lvProperties.Location = new System.Drawing.Point(779, 49);
            this.lvProperties.MultiSelect = false;
            this.lvProperties.Name = "lvProperties";
            this.lvProperties.Size = new System.Drawing.Size(88, 679);
            this.lvProperties.SmallImageList = this.imageList1;
            this.lvProperties.TabIndex = 5;
            this.lvProperties.UseCompatibleStateImageBehavior = false;
            this.lvProperties.View = System.Windows.Forms.View.List;
            // 
            // panel1
            // 
            this.panel1.BackColor = System.Drawing.Color.DarkGreen;
            this.panel1.Controls.Add(this.boardControl1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.panel1.Location = new System.Drawing.Point(88, 49);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(691, 679);
            this.panel1.TabIndex = 6;
            // 
            // boardControl1
            // 
            this.boardControl1.BlackPlayerName = "Black";
            this.boardControl1.GameDate = new System.DateTime(2026, 8, 30, 15, 21, 22, 123);
            this.boardControl1.Location = new System.Drawing.Point(-136, 8);
            this.boardControl1.Name = "boardControl1";
            this.boardControl1.SelectedPiece = null;
            this.boardControl1.ShowBlackCoverage = false;
            this.boardControl1.ShowSelectedPieceCoverage = false;
            this.boardControl1.ShowWhiteCoverage = false;
            this.boardControl1.Size = new System.Drawing.Size(591, 413);
            this.boardControl1.TabIndex = 2;
            this.boardControl1.WhitePlayerName = "White";
            this.boardControl1.PieceSelected += new System.EventHandler<ChessEngine.Events.PieceSelectedEventArgs>(this.boardControl1_PieceSelected);
            // 
            // registerGameEndingToolStripMenuItem
            // 
            this.registerGameEndingToolStripMenuItem.Name = "registerGameEndingToolStripMenuItem";
            this.registerGameEndingToolStripMenuItem.Size = new System.Drawing.Size(189, 22);
            this.registerGameEndingToolStripMenuItem.Text = "Register game ending";
            // 
            // MainWindow
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(867, 750);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.lvProperties);
            this.Controls.Add(this.listView1);
            this.Controls.Add(this.toolStrip1);
            this.Controls.Add(this.statusStrip1);
            this.Controls.Add(this.menuStrip1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.KeyPreview = true;
            this.MainMenuStrip = this.menuStrip1;
            this.MinimumSize = new System.Drawing.Size(500, 400);
            this.Name = "MainWindow";
            this.Text = "Chess Engine";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainWindow_FormClosing);
            this.Load += new System.EventHandler(this.MainWindow_Load);
            this.KeyDown += new System.Windows.Forms.KeyEventHandler(this.MainWindow_KeyDown);
            this.Resize += new System.EventHandler(this.MainWindow_Resize);
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.statusStrip1.ResumeLayout(false);
            this.statusStrip1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem helpToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.StatusStrip statusStrip1;
        private BoardControl boardControl1;
        private System.Windows.Forms.ToolStripMenuItem moveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem registerMoveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem cancelRegisterMoveToolStripMenuItem;
        private System.Windows.Forms.ToolStripStatusLabel lblStatus;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ListView listView1;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem1;
        private System.Windows.Forms.ToolStripMenuItem firstToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem previousToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem playToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pauseToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem nextToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem lastToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton btnRegistrera;
        private System.Windows.Forms.ToolStripButton btnAvbrytRegistrering;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton btnFirst;
        private System.Windows.Forms.ToolStripButton btnPrevious;
        private System.Windows.Forms.ToolStripButton btnPlay;
        private System.Windows.Forms.ToolStripButton btnPause;
        private System.Windows.Forms.ToolStripButton btnNext;
        private System.Windows.Forms.ToolStripButton btnLast;
        private System.Windows.Forms.ToolStripMenuItem fileToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem newGameToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem openToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem saveAsToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem2;
        private System.Windows.Forms.ToolStripMenuItem exitToolStripMenuItem;
        private System.Windows.Forms.ToolStripButton btnNewGame;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
        private System.Windows.Forms.ToolStripButton btnOpen;
        private System.Windows.Forms.ToolStripButton btnSave;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
        private System.Windows.Forms.ImageList imageList1;
        private System.Windows.Forms.ListView lvProperties;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.ToolStripMenuItem viewToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showWhiteCoverageToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showBlackCoverageToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem showSelectedPieceCoverageToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteLastMoveToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem3;
        private System.Windows.Forms.ToolStripMenuItem fromWhitesPerspectiveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem fromBlacksPerspectiveToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem openFromClipboardToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem portableGameNotationPGNToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem forsythEdwardsNotationFENToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem archonViewToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem4;
        private System.Windows.Forms.ToolStripSeparator toolStripMenuItem5;
        private System.Windows.Forms.ToolStripMenuItem reportABugToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator5;
        private System.Windows.Forms.ToolStripButton btnRotateBoard;
        private System.Windows.Forms.ToolStripMenuItem openTheManualToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem registerGameEndingToolStripMenuItem;
    }
}

