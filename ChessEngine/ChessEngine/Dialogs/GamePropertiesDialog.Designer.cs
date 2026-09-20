namespace ChessEngine.Dialogs
{
    partial class GamePropertiesDialog
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
            this.label1 = new System.Windows.Forms.Label();
            this.txtGameTitle = new System.Windows.Forms.TextBox();
            this.lblGameDate = new System.Windows.Forms.Label();
            this.txtGameDate = new System.Windows.Forms.TextBox();
            this.txtWhitePlayerName = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.txtBlackPlayerName = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtMovesCount = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtGameEnding = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.btnOk = new System.Windows.Forms.Button();
            this.btnCancel = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(57, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Game title:";
            // 
            // txtGameTitle
            // 
            this.txtGameTitle.Location = new System.Drawing.Point(8, 24);
            this.txtGameTitle.MaxLength = 512;
            this.txtGameTitle.Name = "txtGameTitle";
            this.txtGameTitle.Size = new System.Drawing.Size(264, 20);
            this.txtGameTitle.TabIndex = 1;
            this.txtGameTitle.Validating += new System.ComponentModel.CancelEventHandler(this.txtGameTitle_Validating);
            // 
            // lblGameDate
            // 
            this.lblGameDate.AutoSize = true;
            this.lblGameDate.Location = new System.Drawing.Point(8, 48);
            this.lblGameDate.Name = "lblGameDate";
            this.lblGameDate.Size = new System.Drawing.Size(139, 13);
            this.lblGameDate.TabIndex = 2;
            this.lblGameDate.Text = "Game date (YYYY-MM-DD):";
            // 
            // txtGameDate
            // 
            this.txtGameDate.Location = new System.Drawing.Point(8, 64);
            this.txtGameDate.MaxLength = 512;
            this.txtGameDate.Name = "txtGameDate";
            this.txtGameDate.Size = new System.Drawing.Size(264, 20);
            this.txtGameDate.TabIndex = 3;
            this.txtGameDate.Validating += new System.ComponentModel.CancelEventHandler(this.txtGameDate_Validating);
            // 
            // txtWhitePlayerName
            // 
            this.txtWhitePlayerName.Location = new System.Drawing.Point(8, 104);
            this.txtWhitePlayerName.MaxLength = 512;
            this.txtWhitePlayerName.Name = "txtWhitePlayerName";
            this.txtWhitePlayerName.Size = new System.Drawing.Size(264, 20);
            this.txtWhitePlayerName.TabIndex = 5;
            this.txtWhitePlayerName.Validating += new System.ComponentModel.CancelEventHandler(this.txtWhitePlayerName_Validating);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(8, 88);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(98, 13);
            this.label3.TabIndex = 4;
            this.label3.Text = "White player name:";
            // 
            // txtBlackPlayerName
            // 
            this.txtBlackPlayerName.Location = new System.Drawing.Point(8, 144);
            this.txtBlackPlayerName.MaxLength = 512;
            this.txtBlackPlayerName.Name = "txtBlackPlayerName";
            this.txtBlackPlayerName.Size = new System.Drawing.Size(264, 20);
            this.txtBlackPlayerName.TabIndex = 7;
            this.txtBlackPlayerName.Validating += new System.ComponentModel.CancelEventHandler(this.txtBlackPlayerName_Validating);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(8, 128);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(97, 13);
            this.label4.TabIndex = 6;
            this.label4.Text = "Black player name:";
            // 
            // txtMovesCount
            // 
            this.txtMovesCount.Enabled = false;
            this.txtMovesCount.Location = new System.Drawing.Point(8, 184);
            this.txtMovesCount.MaxLength = 512;
            this.txtMovesCount.Name = "txtMovesCount";
            this.txtMovesCount.Size = new System.Drawing.Size(80, 20);
            this.txtMovesCount.TabIndex = 9;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(8, 168);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(72, 13);
            this.label5.TabIndex = 8;
            this.label5.Text = "Moves count:";
            // 
            // txtGameEnding
            // 
            this.txtGameEnding.Enabled = false;
            this.txtGameEnding.Location = new System.Drawing.Point(96, 184);
            this.txtGameEnding.MaxLength = 512;
            this.txtGameEnding.Name = "txtGameEnding";
            this.txtGameEnding.Size = new System.Drawing.Size(176, 20);
            this.txtGameEnding.TabIndex = 11;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(96, 168);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(43, 13);
            this.label6.TabIndex = 10;
            this.label6.Text = "Ending:";
            // 
            // btnOk
            // 
            this.btnOk.Location = new System.Drawing.Point(116, 212);
            this.btnOk.Name = "btnOk";
            this.btnOk.Size = new System.Drawing.Size(75, 23);
            this.btnOk.TabIndex = 12;
            this.btnOk.Text = "OK";
            this.btnOk.UseVisualStyleBackColor = true;
            this.btnOk.Click += new System.EventHandler(this.btnOk_Click);
            // 
            // btnCancel
            // 
            this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.btnCancel.Location = new System.Drawing.Point(196, 212);
            this.btnCancel.Name = "btnCancel";
            this.btnCancel.Size = new System.Drawing.Size(75, 23);
            this.btnCancel.TabIndex = 13;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseVisualStyleBackColor = true;
            // 
            // GamePropertiesDialog
            // 
            this.AcceptButton = this.btnOk;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.btnCancel;
            this.ClientSize = new System.Drawing.Size(278, 241);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.btnOk);
            this.Controls.Add(this.txtGameEnding);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.txtMovesCount);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.txtBlackPlayerName);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.txtWhitePlayerName);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.txtGameDate);
            this.Controls.Add(this.lblGameDate);
            this.Controls.Add(this.txtGameTitle);
            this.Controls.Add(this.label1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "GamePropertiesDialog";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Game properties";
            this.Load += new System.EventHandler(this.GamePropertiesDialog_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox txtGameTitle;
        private System.Windows.Forms.Label lblGameDate;
        private System.Windows.Forms.TextBox txtGameDate;
        private System.Windows.Forms.TextBox txtWhitePlayerName;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtBlackPlayerName;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtMovesCount;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtGameEnding;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Button btnOk;
        private System.Windows.Forms.Button btnCancel;
    }
}