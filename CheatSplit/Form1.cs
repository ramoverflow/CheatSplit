using System.Text;
using System.Text.RegularExpressions;

namespace CheatSplit
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private async void button1_Click(object sender, EventArgs e)
        {
            var dlg = new OpenFileDialog
            {
                Filter = "CheatFiles(*.txt)|*.txt"
            };

            if (dlg.ShowDialog() == DialogResult.OK)
            {
                var file = dlg.FileName;
                textBox1.Text = await File.ReadAllTextAsync(file, Encoding.UTF8);
            }
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            var cheatFiles = new List<CheatFile>();

            foreach (var textLine in textBox1.Lines)
            {
                if (Regex.Match(textLine, @"^\{.*?\}$").Success || Regex.Match(textLine, @"^\[.*?\]$").Success) //cheat title
                {
                    cheatFiles.Add(new CheatFile
                    {
                        FileName = $"{textLine.Substring(1, textLine.Length - 2)}.txt"
                    });
                }
                else //cheat codes
                {
                    if (cheatFiles.LastOrDefault() is not null)
                    {
                        cheatFiles.Last().Codes.Add(textLine);
                    }
                }
            }

            var saveFolderDialog = new FolderBrowserDialog
            {
                RootFolder = Environment.SpecialFolder.Desktop
            };

            if (saveFolderDialog.ShowDialog() == DialogResult.OK)
            {
                var saveFolder = saveFolderDialog.SelectedPath;

            }
        }

        private class CheatFile
        {
            public required string FileName { get; set; }

            public List<string> Codes { get; set; } = [];
        }
    }
}
