using Microsoft.VisualBasic.Logging;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Jigdo.Gui
{
    public partial class WinjigdoForm : Form
    {
        WinjigdoSettings settingsjson;

        private readonly HttpClient _http = new() { Timeout = TimeSpan.FromHours(8) };
        private CancellationTokenSource? _cts;
        private Task? _runTask;

        private readonly HttpClient downloadJigdoUrlClient = new HttpClient();
        private string[] downloadedJigdosFromUrl;

        public WinjigdoForm()
        {
            InitializeComponent();

            this.AllowDrop = true;

            try
            {
                settingsjson = WinjigdoSettings.Load();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            toolStripButtonStart.Click += async (_, _) => await StartAsync();
            toolStripButtonStop.Click += (_, _) => Stop();

            FormClosing += (_, e) =>
            {
                if (_cts != null && !_cts.IsCancellationRequested && _runTask != null && !_runTask.IsCompleted)
                {
                    var r = MessageBox.Show(this, "A build is in progress. Stop and exit?", Text, MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
                    if (r != DialogResult.Yes)
                    {
                        e.Cancel = true;
                        return;
                    }

                    Stop();
                    _runTask?.Wait(TimeSpan.FromSeconds(5));
                }

                _http.Dispose();
            };

        }


        private void settingsToolStripMenuItemSettings_Click(object sender, EventArgs e)
        {
            SettingsForm fSettigns = new SettingsForm(settingsjson);
            fSettigns.StartPosition = FormStartPosition.CenterParent;
            fSettigns.ShowDialog();
        }

        private void toolStripButtonAddJob_Click(object sender, EventArgs e)
        {
            using var dj = new OpenFileDialog
            {
                Title = "Select .jigdo file",
                Filter = "Jigdo files (*.jigdo)|*.jigdo|All files (*.*)|*.*",
                CheckFileExists = true,
            };
            if (dj.ShowDialog(this) != DialogResult.OK)
                return;

            using var dt = new OpenFileDialog
            {
                Title = "Select .template file",
                Filter = "Template files (*.template)|*.template|All files (*.*)|*.*",
                CheckFileExists = true,
            };
            if (dt.ShowDialog(this) != DialogResult.OK)
                return;

            using var sav = new SaveFileDialog
            {
                Title = "Output ISO file",
                Filter = "ISO images (*.iso)|*.iso|All files (*.*)|*.*",
                FileName = Path.GetFileNameWithoutExtension(dj.FileName) + ".iso",
            };
            if (sav.ShowDialog(this) != DialogResult.OK)
                return;

            int rowIndex = dataGridView_gridJobs.Rows.Add();
            dataGridView_gridJobs.Rows[rowIndex].Cells["ColumnJigdoFile"].Value = Path.GetFileName(dj.FileName);
            dataGridView_gridJobs.Rows[rowIndex].Cells["ColumnJigdoFile"].Tag = dj.FileName;
            dataGridView_gridJobs.Rows[rowIndex].Cells["ColumnTemplateFile"].Value = Path.GetFileName(dt.FileName);
            dataGridView_gridJobs.Rows[rowIndex].Cells["ColumnTemplateFile"].Tag = dt.FileName;
            dataGridView_gridJobs.Rows[rowIndex].Cells["ColumnOutputISO"].Value = Path.GetFileName(sav.FileName);
            dataGridView_gridJobs.Rows[rowIndex].Cells["ColumnOutputISO"].Tag = sav.FileName;
            dataGridView_gridJobs.Rows[rowIndex].Cells["ColumnStatus"].Value = "Pending";


            //dataGridView_gridJobs.Rows.Add(dj.FileName, dt.FileName, sav.FileName, "Pending");
        }

        private void toolStripButtonRemoveJob_Click(object sender, EventArgs e)
        {
            foreach (DataGridViewRow r in dataGridView_gridJobs.SelectedRows.Cast<DataGridViewRow>().ToList())
                dataGridView_gridJobs.Rows.Remove(r);
        }


        private void Stop()
        {
            _cts?.Cancel();
            Log("Stop requested.");
        }

        private async Task StartAsync()
        {
            if (dataGridView_gridJobs.Rows.Count == 0)
            {
                MessageBox.Show(this, "Add at least one job (jigdo + template + output).", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string? cacheRoot = settingsjson.partsCachePath;
            if (string.IsNullOrEmpty(cacheRoot))
            {
                MessageBox.Show(this, "Set a parts cache directory.", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                Directory.CreateDirectory(cacheRoot);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, "Cannot create cache: " + ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            foreach (DataGridViewRow row in dataGridView_gridJobs.Rows)
            {
                if (row.IsNewRow)
                    continue;
                string j = (string)row.Cells[0].Tag! ?? "";
                string t = (string)row.Cells[1].Tag! ?? "";
                string o = (string)row.Cells[2].Tag! ?? "";
                if (!File.Exists(j) || !File.Exists(t) || string.IsNullOrWhiteSpace(o))
                {
                    MessageBox.Show(this, "Each job needs existing jigdo/template files and an output path.", Text, MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }
            }

            IReadOnlyList<JigdoServerLine> extra = CollectExtraServers();
            bool verify = settingsjson.verifyChecksum;

            _cts = new CancellationTokenSource();
            toolStripButtonStart.Enabled = false;
            toolStripButtonStop.Enabled = true;
            toolStripProgressBarDownloadProgress.Value = 0;
            Log("--- Started ---");
            Log("Parts cache: " + cacheRoot + " (each job uses a subfolder; restarting skips files already downloaded).");

            var token = _cts.Token;
            _runTask = Task.Run(async () =>
            {
                int currentJobIndex = -1;
                try
                {
                    for (int i = 0; i < dataGridView_gridJobs.Rows.Count; i++)
                    {
                        token.ThrowIfCancellationRequested();
                        DataGridViewRow row = dataGridView_gridJobs.Rows[i];
                        if (row.IsNewRow)
                            continue;

                        currentJobIndex = i;
                        RunOnUiThread(() => toolStripProgressBarDownloadProgress.Value = 0);
                        string jigdoPath = (string)row.Cells[0].Tag!;
                        string templatePath = (string)row.Cells[1].Tag!;
                        string outputPath = (string)row.Cells[2].Tag!;

                        SetRowStatus(i, "Running…");
                        RunOnUiThread(() => toolStripStatusLabelMain.Text = $"Job {i + 1}/{dataGridView_gridJobs.Rows.Count}: {Path.GetFileName(outputPath)}");

                        string jobCache = Path.Combine(cacheRoot, MakeJobCacheFolderName(jigdoPath, templatePath, outputPath));
                        Directory.CreateDirectory(jobCache);
                        Directory.CreateDirectory(Path.GetDirectoryName(outputPath)!);

                        JigdoFileDocument jigdo = JigdoFileReader.ReadFile(jigdoPath);
                        if (extra.Count > 0)
                            jigdo = jigdo.WithPrependedServers(extra);

                        var progress = new Progress<JigdoDownloadProgress>(p =>
                        {
                            RunOnUiThread(() =>
                            {
                                if (p.TotalBytes > 0)
                                    toolStripProgressBarDownloadProgress.Value = (int)Math.Min(100, 100.0 * p.BytesReceived / p.TotalBytes);
                                toolStripStatusLabelMain.Text = $"{p.BytesReceived:N0} / {p.TotalBytes:N0}  {p.RelativePath}";
                                toolStripStatusLabelCounter.Text = p.PartIndex + "/" + p.TotalParts;
                            });
                        });

                        await JigdoWebPipeline.DownloadAndBuildAsync(
                            jigdo,
                            templatePath,
                            jobCache,
                            outputPath,
                            progress,
                            _http,
                            verify,
                            token).ConfigureAwait(false);

                        RunOnUiThread(() => toolStripProgressBarDownloadProgress.Value = 100);
                        SetRowStatus(i, "Done");
                        Log($"Finished: {outputPath}");
                    }

                    RunOnUiThread(() =>
                    {
                        toolStripStatusLabelMain.Text = "All jobs completed.";
                        Log("--- All completed ---");
                    });
                }
                catch (OperationCanceledException)
                {
                    if (currentJobIndex >= 0)
                        SetRowStatus(currentJobIndex, "Stopped");
                    RunOnUiThread(() =>
                    {
                        toolStripStatusLabelMain.Text = "Stopped.";
                        Log("--- Stopped by user ---");
                    });
                }
                catch (Exception ex)
                {
                    if (currentJobIndex >= 0)
                        SetRowStatus(currentJobIndex, "Error");
                    RunOnUiThreadSync(() =>
                    {
                        toolStripStatusLabelMain.Text = "Error.";
                        Log("ERROR: " + ex.Message);
                        if (ex.InnerException != null)
                            Log(ex.InnerException.Message);
                        MessageBox.Show(this, ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                    });
                }
                finally
                {
                    RunOnUiThread(() =>
                    {
                        toolStripButtonStart.Enabled = true;
                        toolStripButtonStop.Enabled = false;
                        _cts?.Dispose();
                        _cts = null;
                    });
                }
            }, token);

            try
            {
                await _runTask.ConfigureAwait(true);
            }
            catch
            {
                // surfaced in task
            }
        }

        private void SetRowStatus(int rowIndex, string status)
        {
            if (rowIndex < 0 || rowIndex >= dataGridView_gridJobs.Rows.Count)
                return;
            RunOnUiThread(() => dataGridView_gridJobs.Rows[rowIndex].Cells[3].Value = status);
        }

        private static string MakeJobCacheFolderName(string jigdoPath, string templatePath, string outputPath)
        {
            string name = Path.GetFileNameWithoutExtension(outputPath);
            if (string.IsNullOrEmpty(name))
                name = "image";
            foreach (char c in Path.GetInvalidFileNameChars())
                name = name.Replace(c, '_');

            byte[] hash = SHA256.HashData(Encoding.UTF8.GetBytes(Path.GetFullPath(jigdoPath) + "|" + Path.GetFullPath(templatePath)));
            string h = Convert.ToHexString(hash.AsSpan(0, 8)).ToLowerInvariant();
            return $"{name}_{h}";
        }

        private List<JigdoServerLine> CollectExtraServers()
        {
            var list = new List<JigdoServerLine>();
            foreach (DownloadMirror mirror in settingsjson.mirrors)
            {
                string? label = mirror.alias;
                string? url = mirror.url;
                if (string.IsNullOrEmpty(label) || string.IsNullOrEmpty(url))
                    continue;
                if (!Uri.TryCreate(url, UriKind.Absolute, out Uri? uri) ||
                    (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
                    continue;

                bool tryLast = mirror.tryLast;
                list.Add(new JigdoServerLine { MirrorLabel = label, BaseUrl = uri, TryLast = tryLast });
            }

            return list;
        }


        private void Log(string line)
        {
            if (IsDisposed)
                return;
            void Append()
            {
                textBox_log.AppendText(line + Environment.NewLine);
                textBox_log.SelectionStart = textBox_log.TextLength;
                textBox_log.ScrollToCaret();
            }

            RunOnUiThread(Append);
        }

        private void RunOnUiThread(Action a)
        {
            if (IsDisposed)
                return;
            try
            {
                if (InvokeRequired)
                    BeginInvoke(a);
                else
                    a();
            }
            catch (ObjectDisposedException)
            {
                // ignore
            }
        }

        private void RunOnUiThreadSync(Action a)
        {
            if (IsDisposed)
                return;
            try
            {
                if (InvokeRequired)
                    Invoke(a);
                else
                    a();
            }
            catch (ObjectDisposedException)
            {
                // ignore
            }
        }

        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show(this, "WinJigdo 0.1", Text, MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void WinjigdoForm_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.Copy;
        }

        private void WinjigdoForm_DragDrop(object sender, DragEventArgs e)
        {
            string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
            ParseJigdoFilesToDVG_gridJobs(files);

        }

        private void ParseJigdoFilesToDVG_gridJobs(string[] files)
        {
            var fileSet = files.ToHashSet(StringComparer.OrdinalIgnoreCase);

            foreach (var jigdoFile in files.Where(f => Path.GetExtension(f).Equals(".jigdo", StringComparison.OrdinalIgnoreCase)))
            {
                string directory = Path.GetDirectoryName(jigdoFile);
                string baseName = Path.GetFileNameWithoutExtension(jigdoFile);

                // Build expected template file path
                string templateFile = Path.Combine(directory, baseName + ".template");

                // Check if matching template exists in dropped files
                if (fileSet.Contains(templateFile) || File.Exists(templateFile))
                {
                    int rowIndex = dataGridView_gridJobs.Rows.Add();
                    dataGridView_gridJobs.Rows[rowIndex].Cells["ColumnJigdoFile"].Value = Path.GetFileName(jigdoFile);
                    dataGridView_gridJobs.Rows[rowIndex].Cells["ColumnJigdoFile"].Tag = jigdoFile;
                    dataGridView_gridJobs.Rows[rowIndex].Cells["ColumnTemplateFile"].Value = Path.GetFileName(templateFile);
                    dataGridView_gridJobs.Rows[rowIndex].Cells["ColumnTemplateFile"].Tag = templateFile;
                    dataGridView_gridJobs.Rows[rowIndex].Cells["ColumnOutputISO"].Value = baseName + ".iso";
                    dataGridView_gridJobs.Rows[rowIndex].Cells["ColumnOutputISO"].Tag = Path.Combine(directory, baseName + ".iso");
                    dataGridView_gridJobs.Rows[rowIndex].Cells["ColumnStatus"].Value = "Pending";
                }
            }
        }


        string GetDownloadFolderPath()
        {
            return Registry.GetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Explorer\Shell Folders", "{374DE290-123F-4565-9164-39C4925E467B}", String.Empty).ToString();
        }

        private async void loadJigdoFromURLToolStripMenuItem_ClickAsync(object sender, EventArgs e)
        {
            string outputFolder = GetDownloadFolderPath();
            using (UrlInputForm fUrlInput = new UrlInputForm())
            {
                fUrlInput.StartPosition = FormStartPosition.CenterParent;
                if (fUrlInput.ShowDialog() == DialogResult.OK)
                {
                    string url = fUrlInput.EnteredUrl;
                    await DownloadAllJigdosAsync(url, outputFolder);

                }
            }
 
        }

        private async Task DownloadAllJigdosAsync(string baseUrl, string outputFolder)
        {
            //Directory.CreateDirectory(outputFolder);

            // Check if ouputFolder exists
            if (!Path.Exists(outputFolder))
            {
                MessageBox.Show(this, "Output folder does not exist: " + outputFolder, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            //Check url is not empty
            if (String.IsNullOrEmpty(baseUrl))
            {
                MessageBox.Show(this, "Please input url first!", Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            List<string> downloadedFilesFromUrl = new List<string>();

            // Get directory listing
            string html;
            try
            {
                html = await downloadJigdoUrlClient.GetStringAsync(baseUrl);
            }
            catch (Exception ex)
            {
                MessageBox.Show(this, ex.Message, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            

            // Find all .jigdo and .template links
            var matches = Regex.Matches(html, @"href\s*=\s*""([^""]+\.(jigdo|template))""",
                RegexOptions.IgnoreCase);

            var files = new HashSet<string>();

            foreach (Match match in matches)
            {
                string relativePath = match.Groups[1].Value;

                // Avoid duplicates
                if (!files.Add(relativePath))
                    continue;

                string fileUrl = baseUrl + relativePath;
                string localPath = Path.Combine(outputFolder, Path.GetFileName(relativePath));

                RunOnUiThread(() => toolStripStatusLabelMain.Text = $"Downloading: {fileUrl}");
                
                try
                {
                    byte[] data = await downloadJigdoUrlClient.GetByteArrayAsync(fileUrl);
                    await File.WriteAllBytesAsync(localPath, data);
                    Log($"Downloaded: {fileUrl} to {outputFolder}");
                    downloadedFilesFromUrl.Add(localPath);
                }
                catch (Exception ex)
                {
                    RunOnUiThread(() => toolStripStatusLabelMain.Text = $"Failed: {fileUrl} - {ex.Message}");
                    Log($"Failed: {fileUrl} - {ex.Message}");
                }
            }
            downloadedJigdosFromUrl = downloadedFilesFromUrl.ToArray<string>();
            ParseJigdoFilesToDVG_gridJobs(downloadedJigdosFromUrl);
            RunOnUiThread(() => toolStripStatusLabelMain.Text = "--");
            Log("Download complete.");
        }


    }
}
