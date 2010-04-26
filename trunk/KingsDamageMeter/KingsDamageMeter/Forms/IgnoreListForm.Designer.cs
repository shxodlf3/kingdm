/**************************************************************************\
 * 
    This file is part of KingsDamageMeter.

    KingsDamageMeter is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    KingsDamageMeter is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with KingsDamageMeter. If not, see <http://www.gnu.org/licenses/>.
 * 
\**************************************************************************/

namespace KingsDamageMeter.Forms
{
    partial class IgnoreListForm
    {
        private System.ComponentModel.IContainer components = null;

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
            this.TextPlayer = new System.Windows.Forms.TextBox();
            this.ButtonFind = new System.Windows.Forms.Button();
            this.ListIgnored = new System.Windows.Forms.ListBox();
            this.MenuPlayers = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.MenuPlayersRemove = new System.Windows.Forms.ToolStripMenuItem();
            this.MenuPlayers.SuspendLayout();
            this.SuspendLayout();
            // 
            // TextPlayer
            // 
            this.TextPlayer.Location = new System.Drawing.Point(12, 12);
            this.TextPlayer.Name = "TextPlayer";
            this.TextPlayer.Size = new System.Drawing.Size(100, 20);
            this.TextPlayer.TabIndex = 0;
            this.TextPlayer.KeyDown += new System.Windows.Forms.KeyEventHandler(this.TextPlayer_KeyDown);
            // 
            // ButtonFind
            // 
            this.ButtonFind.Location = new System.Drawing.Point(118, 12);
            this.ButtonFind.Name = "ButtonFind";
            this.ButtonFind.Size = new System.Drawing.Size(60, 20);
            this.ButtonFind.TabIndex = 1;
            this.ButtonFind.Text = "Find";
            this.ButtonFind.UseVisualStyleBackColor = true;
            this.ButtonFind.Click += new System.EventHandler(this.ButtonFind_Click);
            // 
            // ListIgnored
            // 
            this.ListIgnored.ContextMenuStrip = this.MenuPlayers;
            this.ListIgnored.FormattingEnabled = true;
            this.ListIgnored.IntegralHeight = false;
            this.ListIgnored.Location = new System.Drawing.Point(12, 38);
            this.ListIgnored.Name = "ListIgnored";
            this.ListIgnored.Size = new System.Drawing.Size(100, 244);
            this.ListIgnored.TabIndex = 2;
            // 
            // MenuPlayers
            // 
            this.MenuPlayers.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.MenuPlayersRemove});
            this.MenuPlayers.Name = "MenuPlayers";
            this.MenuPlayers.Size = new System.Drawing.Size(125, 26);
            // 
            // MenuPlayersRemove
            // 
            this.MenuPlayersRemove.Name = "MenuPlayersRemove";
            this.MenuPlayersRemove.Size = new System.Drawing.Size(124, 22);
            this.MenuPlayersRemove.Text = "Remove";
            this.MenuPlayersRemove.Click += new System.EventHandler(this.MenuPlayersRemove_Click);
            // 
            // IgnoreListForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(192, 294);
            this.Controls.Add(this.ListIgnored);
            this.Controls.Add(this.ButtonFind);
            this.Controls.Add(this.TextPlayer);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "IgnoreListForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Ignore List";
            this.TopMost = true;
            this.MenuPlayers.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox TextPlayer;
        private System.Windows.Forms.Button ButtonFind;
        private System.Windows.Forms.ListBox ListIgnored;
        private System.Windows.Forms.ContextMenuStrip MenuPlayers;
        private System.Windows.Forms.ToolStripMenuItem MenuPlayersRemove;
    }
}