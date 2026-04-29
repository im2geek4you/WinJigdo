namespace Jigdo.Gui
{
    partial class WinjigdoForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(WinjigdoForm));
            statusStrip1 = new StatusStrip();
            toolStripProgressBarDownloadProgress = new ToolStripProgressBar();
            toolStripStatusLabelCounter = new ToolStripStatusLabel();
            toolStripStatusLabelMain = new ToolStripStatusLabel();
            splitContainerMain = new SplitContainer();
            dataGridView_gridJobs = new DataGridView();
            toolStrip1 = new ToolStrip();
            toolStripButtonAddJob = new ToolStripButton();
            toolStripButtonRemoveJob = new ToolStripButton();
            toolStripSeparator1 = new ToolStripSeparator();
            toolStripButtonStart = new ToolStripButton();
            toolStripButtonStop = new ToolStripButton();
            textBox_log = new TextBox();
            menuStrip1 = new MenuStrip();
            fileToolStripMenuItem = new ToolStripMenuItem();
            loadJigdoFromURLToolStripMenuItem = new ToolStripMenuItem();
            settingsToolStripMenuItemSettings = new ToolStripMenuItem();
            aboutToolStripMenuItem = new ToolStripMenuItem();
            ColumnJigdoFile = new DataGridViewTextBoxColumn();
            ColumnTemplateFile = new DataGridViewTextBoxColumn();
            ColumnOutputISO = new DataGridViewTextBoxColumn();
            ColumnStatus = new DataGridViewTextBoxColumn();
            statusStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainerMain).BeginInit();
            splitContainerMain.Panel1.SuspendLayout();
            splitContainerMain.Panel2.SuspendLayout();
            splitContainerMain.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dataGridView_gridJobs).BeginInit();
            toolStrip1.SuspendLayout();
            menuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // statusStrip1
            // 
            statusStrip1.Items.AddRange(new ToolStripItem[] { toolStripProgressBarDownloadProgress, toolStripStatusLabelCounter, toolStripStatusLabelMain });
            statusStrip1.Location = new Point(0, 529);
            statusStrip1.Name = "statusStrip1";
            statusStrip1.Size = new Size(920, 22);
            statusStrip1.TabIndex = 0;
            statusStrip1.Text = "statusStrip1";
            // 
            // toolStripProgressBarDownloadProgress
            // 
            toolStripProgressBarDownloadProgress.Name = "toolStripProgressBarDownloadProgress";
            toolStripProgressBarDownloadProgress.Size = new Size(200, 16);
            // 
            // toolStripStatusLabelCounter
            // 
            toolStripStatusLabelCounter.Name = "toolStripStatusLabelCounter";
            toolStripStatusLabelCounter.Size = new Size(22, 17);
            toolStripStatusLabelCounter.Text = "-/-";
            // 
            // toolStripStatusLabelMain
            // 
            toolStripStatusLabelMain.Name = "toolStripStatusLabelMain";
            toolStripStatusLabelMain.Size = new Size(17, 17);
            toolStripStatusLabelMain.Text = "--";
            // 
            // splitContainerMain
            // 
            splitContainerMain.Dock = DockStyle.Fill;
            splitContainerMain.Location = new Point(0, 24);
            splitContainerMain.Name = "splitContainerMain";
            splitContainerMain.Orientation = Orientation.Horizontal;
            // 
            // splitContainerMain.Panel1
            // 
            splitContainerMain.Panel1.Controls.Add(dataGridView_gridJobs);
            splitContainerMain.Panel1.Controls.Add(toolStrip1);
            // 
            // splitContainerMain.Panel2
            // 
            splitContainerMain.Panel2.Controls.Add(textBox_log);
            splitContainerMain.Size = new Size(920, 505);
            splitContainerMain.SplitterDistance = 381;
            splitContainerMain.TabIndex = 1;
            // 
            // dataGridView_gridJobs
            // 
            dataGridView_gridJobs.AllowUserToAddRows = false;
            dataGridView_gridJobs.AllowUserToDeleteRows = false;
            dataGridView_gridJobs.AllowUserToResizeRows = false;
            dataGridView_gridJobs.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridView_gridJobs.Columns.AddRange(new DataGridViewColumn[] { ColumnJigdoFile, ColumnTemplateFile, ColumnOutputISO, ColumnStatus });
            dataGridView_gridJobs.Dock = DockStyle.Fill;
            dataGridView_gridJobs.Location = new Point(0, 25);
            dataGridView_gridJobs.Name = "dataGridView_gridJobs";
            dataGridView_gridJobs.RowHeadersVisible = false;
            dataGridView_gridJobs.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dataGridView_gridJobs.Size = new Size(920, 356);
            dataGridView_gridJobs.TabIndex = 1;
            // 
            // toolStrip1
            // 
            toolStrip1.Items.AddRange(new ToolStripItem[] { toolStripButtonAddJob, toolStripButtonRemoveJob, toolStripSeparator1, toolStripButtonStart, toolStripButtonStop });
            toolStrip1.Location = new Point(0, 0);
            toolStrip1.Name = "toolStrip1";
            toolStrip1.Size = new Size(920, 25);
            toolStrip1.TabIndex = 0;
            toolStrip1.Text = "toolStrip1";
            // 
            // toolStripButtonAddJob
            // 
            toolStripButtonAddJob.DisplayStyle = ToolStripItemDisplayStyle.Image;
            toolStripButtonAddJob.Image = (Image)resources.GetObject("toolStripButtonAddJob.Image");
            toolStripButtonAddJob.ImageTransparentColor = Color.Magenta;
            toolStripButtonAddJob.Name = "toolStripButtonAddJob";
            toolStripButtonAddJob.Size = new Size(23, 22);
            toolStripButtonAddJob.Text = "Add job";
            toolStripButtonAddJob.Click += toolStripButtonAddJob_Click;
            // 
            // toolStripButtonRemoveJob
            // 
            toolStripButtonRemoveJob.DisplayStyle = ToolStripItemDisplayStyle.Image;
            toolStripButtonRemoveJob.Image = (Image)resources.GetObject("toolStripButtonRemoveJob.Image");
            toolStripButtonRemoveJob.ImageTransparentColor = Color.Magenta;
            toolStripButtonRemoveJob.Name = "toolStripButtonRemoveJob";
            toolStripButtonRemoveJob.Size = new Size(23, 22);
            toolStripButtonRemoveJob.Text = "Remove selected job";
            toolStripButtonRemoveJob.Click += toolStripButtonRemoveJob_Click;
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new Size(6, 25);
            // 
            // toolStripButtonStart
            // 
            toolStripButtonStart.DisplayStyle = ToolStripItemDisplayStyle.Image;
            toolStripButtonStart.Image = (Image)resources.GetObject("toolStripButtonStart.Image");
            toolStripButtonStart.ImageTransparentColor = Color.Magenta;
            toolStripButtonStart.Name = "toolStripButtonStart";
            toolStripButtonStart.Size = new Size(23, 22);
            toolStripButtonStart.Text = "Start jobs";
            // 
            // toolStripButtonStop
            // 
            toolStripButtonStop.DisplayStyle = ToolStripItemDisplayStyle.Image;
            toolStripButtonStop.Image = (Image)resources.GetObject("toolStripButtonStop.Image");
            toolStripButtonStop.ImageTransparentColor = Color.Magenta;
            toolStripButtonStop.Name = "toolStripButtonStop";
            toolStripButtonStop.Size = new Size(23, 22);
            toolStripButtonStop.Text = "Stop execution";
            // 
            // textBox_log
            // 
            textBox_log.Dock = DockStyle.Fill;
            textBox_log.Location = new Point(0, 0);
            textBox_log.Multiline = true;
            textBox_log.Name = "textBox_log";
            textBox_log.ScrollBars = ScrollBars.Vertical;
            textBox_log.Size = new Size(920, 120);
            textBox_log.TabIndex = 0;
            // 
            // menuStrip1
            // 
            menuStrip1.Items.AddRange(new ToolStripItem[] { fileToolStripMenuItem, aboutToolStripMenuItem });
            menuStrip1.Location = new Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new Size(920, 24);
            menuStrip1.TabIndex = 2;
            menuStrip1.Text = "menuStrip1";
            // 
            // fileToolStripMenuItem
            // 
            fileToolStripMenuItem.DropDownItems.AddRange(new ToolStripItem[] { loadJigdoFromURLToolStripMenuItem, settingsToolStripMenuItemSettings });
            fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            fileToolStripMenuItem.Size = new Size(37, 20);
            fileToolStripMenuItem.Text = "File";
            // 
            // loadJigdoFromURLToolStripMenuItem
            // 
            loadJigdoFromURLToolStripMenuItem.Name = "loadJigdoFromURLToolStripMenuItem";
            loadJigdoFromURLToolStripMenuItem.Size = new Size(184, 22);
            loadJigdoFromURLToolStripMenuItem.Text = "Load Jigdo from URL";
            loadJigdoFromURLToolStripMenuItem.Click += loadJigdoFromURLToolStripMenuItem_ClickAsync;
            // 
            // settingsToolStripMenuItemSettings
            // 
            settingsToolStripMenuItemSettings.Name = "settingsToolStripMenuItemSettings";
            settingsToolStripMenuItemSettings.Size = new Size(184, 22);
            settingsToolStripMenuItemSettings.Text = "Settings";
            settingsToolStripMenuItemSettings.Click += settingsToolStripMenuItemSettings_Click;
            // 
            // aboutToolStripMenuItem
            // 
            aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            aboutToolStripMenuItem.Size = new Size(52, 20);
            aboutToolStripMenuItem.Text = "About";
            aboutToolStripMenuItem.Click += aboutToolStripMenuItem_Click;
            // 
            // ColumnJigdoFile
            // 
            ColumnJigdoFile.HeaderText = "Jigdo File";
            ColumnJigdoFile.Name = "ColumnJigdoFile";
            ColumnJigdoFile.Width = 250;
            // 
            // ColumnTemplateFile
            // 
            ColumnTemplateFile.HeaderText = "Template File";
            ColumnTemplateFile.Name = "ColumnTemplateFile";
            ColumnTemplateFile.Width = 300;
            // 
            // ColumnOutputISO
            // 
            ColumnOutputISO.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            ColumnOutputISO.HeaderText = "Output ISO";
            ColumnOutputISO.Name = "ColumnOutputISO";
            // 
            // ColumnStatus
            // 
            ColumnStatus.HeaderText = "Status";
            ColumnStatus.Name = "ColumnStatus";
            ColumnStatus.Resizable = DataGridViewTriState.True;
            ColumnStatus.Width = 120;
            // 
            // WinjigdoForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(920, 551);
            Controls.Add(splitContainerMain);
            Controls.Add(statusStrip1);
            Controls.Add(menuStrip1);
            Icon = (Icon)resources.GetObject("$this.Icon");
            MainMenuStrip = menuStrip1;
            Name = "WinjigdoForm";
            Text = "WinJigdo";
            DragDrop += WinjigdoForm_DragDrop;
            DragEnter += WinjigdoForm_DragEnter;
            statusStrip1.ResumeLayout(false);
            statusStrip1.PerformLayout();
            splitContainerMain.Panel1.ResumeLayout(false);
            splitContainerMain.Panel1.PerformLayout();
            splitContainerMain.Panel2.ResumeLayout(false);
            splitContainerMain.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)splitContainerMain).EndInit();
            splitContainerMain.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dataGridView_gridJobs).EndInit();
            toolStrip1.ResumeLayout(false);
            toolStrip1.PerformLayout();
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private StatusStrip statusStrip1;
        private SplitContainer splitContainerMain;
        private DataGridView dataGridView_gridJobs;
        private ToolStrip toolStrip1;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem aboutToolStripMenuItem;
        private TextBox textBox_log;
        private ToolStripStatusLabel toolStripStatusLabelMain;
        private ToolStripButton toolStripButtonAddJob;
        private ToolStripButton toolStripButtonRemoveJob;
        private ToolStripMenuItem settingsToolStripMenuItemSettings;
        private ToolStripProgressBar toolStripProgressBarDownloadProgress;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripButton toolStripButtonStart;
        private ToolStripButton toolStripButtonStop;
        private ToolStripMenuItem loadJigdoFromURLToolStripMenuItem;
        private ToolStripStatusLabel toolStripStatusLabelCounter;
        private DataGridViewTextBoxColumn ColumnJigdoFile;
        private DataGridViewTextBoxColumn ColumnTemplateFile;
        private DataGridViewTextBoxColumn ColumnOutputISO;
        private DataGridViewTextBoxColumn ColumnStatus;
    }
}