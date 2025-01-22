using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;

namespace BigFileGenerator {
    public partial class MainForm: Form {
        private readonly GenerateTask task = new GenerateTask();

        public MainForm() {
            InitializeComponent();
            var versionInfo = FileVersionInfo.GetVersionInfo(Assembly.GetEntryAssembly().Location);
            toolStripStatusLabel1.Text = versionInfo.CompanyName + "制作";
            toolStripStatusLabel2.Text = "v" + versionInfo.ProductVersion;
            toolStripStatusLabel3.Text = versionInfo.LegalCopyright;
            task.SetOnTaskFinished(() => {
                this.BeginInvoke((MethodInvoker) delegate {
                    startBtn.Text = "Start";
                });
            });
        }

        private void startBtn_Click(object sender, EventArgs e) {
            if (task.IsRunning()) {
                if (MessageBox.Show("Are you sure you want to stop generating?", "Stop", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes) {
                    task.Stop();
                }
            } else {
                task.Resume();
                startBtn.Text = "Stop";
                Thread thread = new Thread(task.Generate) {
                    IsBackground = true
                };
                thread.Start();
            }
        }

        private void toolStripStatusLabel1_Click(object sender, EventArgs e) {
            Process.Start("https://github.com/pansong291/BigFileGenerator");
        }
    }

    public delegate void TaskFinishedEventHandler();

    class GenerateTask {
        private bool running = false;

        private event TaskFinishedEventHandler finishHandler;

        public void Generate() {
            try {
                using (FileStream stream = new FileStream(GetUniqueFilePath(), FileMode.Create)) {
                    byte[] buffer = new byte[2 * 1024 * 1024];
                    while (running) {
                        stream.Write(buffer, 0, buffer.Length);
                    }
                }
            } catch (Exception e) {
                MessageBox.Show(e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            } finally {
                running = false;
                if (finishHandler!= null) {
                    finishHandler();
                }
            }
        }

        public void Stop() {
            running = false;
        }

        public void Resume() {
            running = true;
        }

        public bool IsRunning() {
            return running;
        }

        public void SetOnTaskFinished(TaskFinishedEventHandler handler) {
            finishHandler = handler;
        }

        private string GetUniqueFilePath()
        {
            string directory = Directory.GetCurrentDirectory();
            string fileName = "big_file";
            string extension = ".tmp";
            string newFilePath = Path.Combine(directory, $"{fileName}{extension}");
            int counter = 1;
            while (File.Exists(newFilePath)) {
                newFilePath = Path.Combine(directory, $"{fileName}({counter}){extension}");
                counter++;
            }
            return newFilePath;
        }
    }
}
