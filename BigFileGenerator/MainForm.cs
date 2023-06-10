using System;
using System.IO;
using System.Threading;
using System.Windows.Forms;

namespace BigFileGenerator {
    public partial class MainForm: Form {
        private readonly GenerateTask task = new GenerateTask();

        public MainForm() {
            InitializeComponent();
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
