using System.Text;
using System.Text.RegularExpressions;

namespace CheatSplit
{
    public partial class Form1 : Form
    {
        private string? OriCheatFile { get; set; }

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
                OriCheatFile = dlg.FileName;
                textBox1.Text = await File.ReadAllTextAsync(OriCheatFile, Encoding.UTF8);
            }
        }

        private async void button2_Click(object sender, EventArgs e)
        {
            var cheatFiles = new List<CheatFile>();

            foreach (var textLine in textBox1.Lines)
            {
                if (Regex.Match(textLine, @"^\{.*?\}$").Success || Regex.Match(textLine, @"^\[.*?\]$").Success) //cheat title
                {
                    var fileName = textLine.Substring(1, textLine.Length - 2);
                    fileName = string.Join("_", fileName.Split(Path.GetInvalidPathChars()));
                    fileName = string.Join("_", fileName.Split(Path.GetInvalidFileNameChars()));

                    var cheatFile = new CheatFile
                    {
                        FileName = $"{fileName}/cheats/{Path.GetFileNameWithoutExtension(OriCheatFile)}.txt"
                    };
                    cheatFile.Codes.Add($"[{fileName}]");

                    cheatFiles.Add(cheatFile);
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
                foreach (var cheatFile in cheatFiles)
                {
                    var saveFile = Path.Combine(saveFolder, cheatFile.FileName);

                    if (!Directory.Exists(Path.GetDirectoryName(saveFile)))
                        Directory.CreateDirectory(Path.GetDirectoryName(saveFile)!);
                    if (File.Exists(saveFile))
                        File.Delete(saveFile);

                    await File.WriteAllLinesAsync(saveFile, cheatFile.Codes, new UTF8Encoding(false));
                }
            }

            MessageBox.Show("finish");
        }

        private class CheatFile
        {
            public required string FileName { get; set; }

            public List<string> Codes { get; set; } = [];
        }
    }
}
