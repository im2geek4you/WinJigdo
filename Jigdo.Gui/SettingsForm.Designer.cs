namespace Jigdo.Gui
{
    partial class SettingsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SettingsForm));
            buttonOk = new Button();
            groupBox1 = new GroupBox();
            buttonBrowseCachePath = new Button();
            textBoxPartsCachePath = new TextBox();
            label1 = new Label();
            checkBoxVerifyChecksum = new CheckBox();
            groupBox2 = new GroupBox();
            toolStrip1 = new ToolStrip();
            toolStripButtonAddMirror = new ToolStripButton();
            toolStripButtonDeleteMirror = new ToolStripButton();
            dataGridViewDownloadMirrors = new DataGridView();
            ColumnAlias = new DataGridViewTextBoxColumn();
            ColumnUrl = new DataGridViewTextBoxColumn();
            ColumnTryLast = new DataGridViewCheckBoxColumn();
            groupBox1.SuspendLayout();
            groupBox2.SuspendLayout();
            toolStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridViewDownloadMirrors).BeginInit();
            SuspendLayout();
            // 
            // buttonOk
            // 
            buttonOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
            buttonOk.Location = new Point(713, 415);
            buttonOk.Name = "buttonOk";
            buttonOk.Size = new Size(75, 23);
            buttonOk.TabIndex = 0;
            buttonOk.Text = "Ok";
            buttonOk.UseVisualStyleBackColor = true;
            buttonOk.Click += buttonOk_Click;
            // 
            // groupBox1
            // 
            groupBox1.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            groupBox1.Controls.Add(buttonBrowseCachePath);
            groupBox1.Controls.Add(textBoxPartsCachePath);
            groupBox1.Controls.Add(label1);
            groupBox1.Controls.Add(checkBoxVerifyChecksum);
            groupBox1.Location = new Point(12, 12);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(776, 118);
            groupBox1.TabIndex = 1;
            groupBox1.TabStop = false;
            groupBox1.Text = "Parts";
            // 
            // buttonBrowseCachePath
            // 
            buttonBrowseCachePath.Location = new Point(673, 25);
            buttonBrowseCachePath.Name = "buttonBrowseCachePath";
            buttonBrowseCachePath.Size = new Size(75, 23);
            buttonBrowseCachePath.TabIndex = 5;
            buttonBrowseCachePath.Text = "Browse";
            buttonBrowseCachePath.UseVisualStyleBackColor = true;
            buttonBrowseCachePath.Click += buttonBrowseCachePath_Click;
            // 
            // textBoxPartsCachePath
            // 
            textBoxPartsCachePath.Location = new Point(82, 26);
            textBoxPartsCachePath.Name = "textBoxPartsCachePath";
            textBoxPartsCachePath.Size = new Size(573, 23);
            textBoxPartsCachePath.TabIndex = 4;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(6, 29);
            label1.Name = "label1";
            label1.Size = new Size(70, 15);
            label1.TabIndex = 3;
            label1.Text = "Cache path:";
            // 
            // checkBoxVerifyChecksum
            // 
            checkBoxVerifyChecksum.AutoSize = true;
            checkBoxVerifyChecksum.Location = new Point(6, 81);
            checkBoxVerifyChecksum.Name = "checkBoxVerifyChecksum";
            checkBoxVerifyChecksum.Size = new Size(138, 19);
            checkBoxVerifyChecksum.TabIndex = 2;
            checkBoxVerifyChecksum.Text = "Verify final checksum";
            checkBoxVerifyChecksum.UseVisualStyleBackColor = true;
            // 
            // groupBox2
            // 
            groupBox2.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            groupBox2.Controls.Add(toolStrip1);
            groupBox2.Controls.Add(dataGridViewDownloadMirrors);
            groupBox2.Location = new Point(12, 147);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(776, 250);
            groupBox2.TabIndex = 2;
            groupBox2.TabStop = false;
            groupBox2.Text = "Extra mirrors";
            // 
            // toolStrip1
            // 
            toolStrip1.Items.AddRange(new ToolStripItem[] { toolStripButtonAddMirror, toolStripButtonDeleteMirror });
            toolStrip1.Location = new Point(3, 19);
            toolStrip1.Name = "toolStrip1";
            toolStrip1.Size = new Size(770, 25);
            toolStrip1.TabIndex = 1;
            toolStrip1.Text = "toolStrip1";
            // 
            // toolStripButtonAddMirror
            // 
            toolStripButtonAddMirror.DisplayStyle = ToolStripItemDisplayStyle.Image;
            toolStripButtonAddMirror.Image = (Image)resources.GetObject("toolStripButtonAddMirror.Image");
            toolStripButtonAddMirror.ImageTransparentColor = Color.Magenta;
            toolStripButtonAddMirror.Name = "toolStripButtonAddMirror";
            toolStripButtonAddMirror.Size = new Size(23, 22);
            toolStripButtonAddMirror.Text = "Add Extra mirror servers (optional — tried before URLs in the .jigdo file). Format: mirror label + base URL ending with /";
            toolStripButtonAddMirror.Click += toolStripButtonAddMirror_Click;
            // 
            // toolStripButtonDeleteMirror
            // 
            toolStripButtonDeleteMirror.DisplayStyle = ToolStripItemDisplayStyle.Image;
            toolStripButtonDeleteMirror.Image = (Image)resources.GetObject("toolStripButtonDeleteMirror.Image");
            toolStripButtonDeleteMirror.ImageTransparentColor = Color.Magenta;
            toolStripButtonDeleteMirror.Name = "toolStripButtonDeleteMirror";
            toolStripButtonDeleteMirror.Size = new Size(23, 22);
            toolStripButtonDeleteMirror.Text = "Delete selected mirror";
            toolStripButtonDeleteMirror.Click += toolStripButtonDeleteMirror_Click;
            // 
            // dataGridViewDownloadMirrors
            // 
            dataGridViewDownloadMirrors.AllowUserToAddRows = false;
            dataGridViewDownloadMirrors.AllowUserToDeleteRows = false;
            dataGridViewDownloadMirrors.AllowUserToResizeRows = false;
            dataGridViewDownloadMirrors.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewDownloadMirrors.Columns.AddRange(new DataGridViewColumn[] { ColumnAlias, ColumnUrl, ColumnTryLast });
            dataGridViewDownloadMirrors.Location = new Point(3, 47);
            dataGridViewDownloadMirrors.Name = "dataGridViewDownloadMirrors";
            dataGridViewDownloadMirrors.RowHeadersVisible = false;
            dataGridViewDownloadMirrors.Size = new Size(770, 200);
            dataGridViewDownloadMirrors.TabIndex = 0;
            // 
            // ColumnAlias
            // 
            ColumnAlias.HeaderText = "Alias";
            ColumnAlias.Name = "ColumnAlias";
            // 
            // ColumnUrl
            // 
            ColumnUrl.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            ColumnUrl.HeaderText = "Url";
            ColumnUrl.Name = "ColumnUrl";
            // 
            // ColumnTryLast
            // 
            ColumnTryLast.HeaderText = "Try Last";
            ColumnTryLast.Name = "ColumnTryLast";
            ColumnTryLast.Resizable = DataGridViewTriState.True;
            ColumnTryLast.SortMode = DataGridViewColumnSortMode.Automatic;
            // 
            // SettingsForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(groupBox2);
            Controls.Add(groupBox1);
            Controls.Add(buttonOk);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "SettingsForm";
            Text = "SettingsForm";
            Load += SettingsForm_Load;
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            toolStrip1.ResumeLayout(false);
            toolStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridViewDownloadMirrors).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Button buttonOk;
        private GroupBox groupBox1;
        private Button buttonBrowseCachePath;
        private TextBox textBoxPartsCachePath;
        private Label label1;
        private CheckBox checkBoxVerifyChecksum;
        private GroupBox groupBox2;
        private ToolStrip toolStrip1;
        private ToolStripButton toolStripButtonAddMirror;
        private ToolStripButton toolStripButtonDeleteMirror;
        private DataGridView dataGridViewDownloadMirrors;
        private DataGridViewTextBoxColumn ColumnAlias;
        private DataGridViewTextBoxColumn ColumnUrl;
        private DataGridViewCheckBoxColumn ColumnTryLast;
    }
}