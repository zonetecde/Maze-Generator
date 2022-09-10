using Microsoft.WindowsAPICodePack.Dialogs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Maze_Generator
{
    /// <summary>
    /// Logique d'interaction pour Window_MassGen.xaml
    /// </summary>
    public partial class Window_MassGen : Window
    {

        public Window_MassGen()
        {
            InitializeComponent();
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void Button_Path_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;
            CommonFileDialogResult result = dialog.ShowDialog();

            if (result == CommonFileDialogResult.Ok)
            {
                button_path.Content = dialog.FileName;
                this.Focus();
            }

        }

        BackgroundWorker BackgroundWorker1 = new BackgroundWorker();


        private void button_generate_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed && button_path.Content.ToString() != "Ouvrir un dossier" && !String.IsNullOrEmpty(txtBox_nbGen.Text) && !String.IsNullOrEmpty(txtBox_size.Text))
            {
                List<Cell[,]> gameBoards = new List<Cell[,]>();

                BackgroundWorker1.DoWork += new DoWorkEventHandler((sender, e) =>
                {
                    int nbGen = 0;
                    int boardSize = 0;
                    Dispatcher.Invoke(() =>
                    {
                        nbGen = Convert.ToInt32(txtBox_nbGen.Text);
                        boardSize = Convert.ToInt32(txtBox_size.Text);

                    });

                    for (int i = 0; i < nbGen; i++)
                    {
                        Dispatcher.Invoke(() =>
                        {
                            gameBoards.Add(Generator.Gen(boardSize, checkBox_moreRdn.IsChecked));
                        });

                        int p = Convert.ToInt32(((double)i / nbGen) * 100);

                        if (i == nbGen - 1)
                            BackgroundWorker1.ReportProgress(100);
                        else
                            BackgroundWorker1.ReportProgress(p - 1);
                    }
                });

                BackgroundWorker1.ProgressChanged += new ProgressChangedEventHandler((sender, e) =>
                {
                    Dispatcher.Invoke(() =>
                    {
                        progressBar.Value = e.ProgressPercentage;

                        if (e.ProgressPercentage == 100)
                        {
                            progressBar.Value = 0;

                            File.WriteAllText(button_path.Content + @"\gen_" + txtBox_nbGen.Text + "_" + DateTime.Now.ToString("HH mm ss") + ".json", JsonConvert.SerializeObject(gameBoards, Formatting.None));
                            MessageBox.Show("Succès");
                        }
                    });
                });

                BackgroundWorker1.WorkerReportsProgress = true;
                BackgroundWorker1.WorkerSupportsCancellation = true;

                BackgroundWorker1.RunWorkerAsync();

            }
            else
                button_path.Foreground = Brushes.Red;
        }
    }

    public class Cell
    {
        [JsonIgnore]
        public string Tag = "0"; // isCell
        [JsonIgnore]
        public string Uid = "0"; // direction

        [JsonIgnore]
        public bool LeftBorder = false;
        [JsonIgnore]
        public bool TopBorder = false;
        [JsonIgnore]
        public bool RightBorder = false;
        [JsonIgnore]
        public bool BottomBorder = false;

        public int[] Borders;

        internal void AllBorder()
        {
            this.LeftBorder = true;
            this.TopBorder = true;
            this.RightBorder = true;
            this.BottomBorder = true;
        }
    }
}
