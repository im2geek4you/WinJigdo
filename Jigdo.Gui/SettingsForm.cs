
namespace Jigdo.Gui
{
    public partial class SettingsForm : Form
    {
        WinjigdoSettings settingsjson;

        public SettingsForm(WinjigdoSettings settings)
        {
            InitializeComponent();
            this.settingsjson = settings;
        }

        private void SettingsForm_Load(object sender, EventArgs e)
        {
            textBoxPartsCachePath.Text = settingsjson.partsCachePath;
            checkBoxVerifyChecksum.Checked = settingsjson.verifyChecksum;
            if (settingsjson.mirrors != null)
            {
                List<DownloadMirror> mirrors = settingsjson.mirrors;
                foreach (DownloadMirror mirror in mirrors)
                {
                    dataGridViewDownloadMirrors.Rows.Add(mirror.alias, mirror.url, mirror.tryLast);
                }
            }
        }

        private void buttonOk_Click(object sender, EventArgs e)
        {
            settingsjson.partsCachePath = textBoxPartsCachePath.Text.Trim();
            settingsjson.verifyChecksum = checkBoxVerifyChecksum.Checked;
            List<DownloadMirror> mirrors = new List<DownloadMirror>();
            foreach (DataGridViewRow row in dataGridViewDownloadMirrors.Rows)
            {
                DownloadMirror mirror = new DownloadMirror()
                {
                    alias = row.Cells["ColumnAlias"].Value.ToString(),
                    url = row.Cells["ColumnUrl"].Value.ToString(),
                    tryLast = (bool)row.Cells["ColumnTryLast"].Value

                };
                mirrors.Add(mirror);
            }
            settingsjson.mirrors = mirrors;
            try
            {
                settingsjson.Save();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

            this.Close();
        }

        private void buttonBrowseCachePath_Click(object sender, EventArgs e)
        {
            using var d = new FolderBrowserDialog { SelectedPath = textBoxPartsCachePath.Text.Trim(), UseDescriptionForTitle = true };
            d.Description = "Select folder for downloaded jigdo parts (resume uses the same folder).";
            if (d.ShowDialog(this) == DialogResult.OK)
                textBoxPartsCachePath.Text = d.SelectedPath;
        }

        private void toolStripButtonAddMirror_Click(object sender, EventArgs e)
        {
            dataGridViewDownloadMirrors.Rows.Add();
        }

        private void toolStripButtonDeleteMirror_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewCell item in dataGridViewDownloadMirrors.SelectedCells)
            {
                dataGridViewDownloadMirrors.Rows.RemoveAt(item.RowIndex);
            }
            
        }
    }
}
