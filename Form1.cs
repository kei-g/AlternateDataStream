using Microsoft.Win32.SafeHandles;
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;

namespace AlternateDataStream
{
    public partial class Form1 : Form
    {
        #region Native methods

        [DllImport("kernel32", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern SafeFileHandle CreateFile(string fileName, NativeFileAccess access, FileShare share, IntPtr security, FileMode mode, NativeFileFlags flags, IntPtr template);

        #endregion

        private string BuildPath(string streamName)
        {
            return new UNCPath($"{this.textBox1.Text}:{streamName}:$DATA");
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void Button1_Click(object sender, EventArgs e)
        {
            using (var dialog = new OpenFileDialog())
            {
                dialog.CheckFileExists = true;
                dialog.CheckPathExists = true;
                dialog.Multiselect = false;
                if (dialog.ShowDialog() == DialogResult.OK)
                    this.textBox1.Text = dialog.FileName;
            }
        }

        private void TextBox1_TextChanged(object sender, EventArgs e)
        {
            this.tabControl1.TabPages.Clear();
            using (var backupStream = new BackupStream(this.textBox1.Text))
            using (var hName = new StreamName())
                foreach (var header in backupStream.Streams)
                {
                    if (0 < header.NameSize && !backupStream.Read(in header, hName))
                        break;
                    var name = 0 < header.NameSize ? hName.ReadStreamName(header.NameSize >> 1) : null;
                    if (string.IsNullOrEmpty(name))
                    {
                        var page = new TabPage($"ID: {header.Id}");
                        this.tabControl1.TabPages.Add(page);
                    }
                    else
                    {
                        var page = new TabPage($"ID: {header.Id} => {name}");
                        this.tabControl1.TabPages.Add(page);
                        var textBox = new TextBox();
                        page.Controls.Add(textBox);
                        textBox.Width = page.Width;
                        textBox.Height = page.Height;
                        textBox.Multiline = true;
                        page.Enter += (s, _) =>
                        {
                            using (var file = CreateFile(BuildPath(name), NativeFileAccess.GenericRead, FileShare.Read, IntPtr.Zero, FileMode.Open, 0, IntPtr.Zero))
                            using (var fileStream = new FileStream(file, FileAccess.Read, 4096, false))
                            {
                                var buf = new byte[4096];
                                var len = fileStream.Read(buf, 0, buf.Length);
                                textBox.Text = Encoding.ASCII.GetString(buf, 0, len);
                            }
                        };
                    }
                }
        }
    }
}
