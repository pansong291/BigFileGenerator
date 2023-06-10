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
        }

        private void startBtn_Click(object sender, EventArgs e) {
            if (task.IsRunning()) {
                if (MessageBox.Show("Are you sure you want to stop generating?", "Stop", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes) {
                    task.Stop();
                    startBtn.Text = "Start";
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

    class GenerateTask {
        private bool running = false;

        public void Generate() {
            try {
                string path = Path.Combine(Directory.GetCurrentDirectory(), "big_file");
                using (FileStream stream = new FileStream(path, FileMode.Create)) {
                    byte[] buffer = new byte[1024];
                    while (running) {
                        stream.Write(buffer, 0, buffer.Length);
                    }
                }
            } catch (Exception e) {
                MessageBox.Show(e.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            } finally {
                running = false;
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
    }
}
