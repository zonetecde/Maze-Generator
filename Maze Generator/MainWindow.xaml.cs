using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.WindowsAPICodePack.Dialogs;
using Newtonsoft.Json;

namespace Maze_Generator
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static Border[,] GameBoard
        {
            get;
            set;
        }

        public double borderThickness = 2.5;
        public int BoardSize = 12;
        public int NumbersOfCells = 0;

        Random Rdn = new Random();

        private Timer Timer;

        public MainWindow() { InitializeComponent(); }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            // Change la taille du gameBoard, les cases ne sont pas ajouté au lancement
            uniformGrid_gameBoard.Width = e.NewSize.Height - (double)90;
            uniformGrid_gameBoard.Height = e.NewSize.Height - (double)90;

            label_gameBoard_information.Width = e.NewSize.Height - (double)90;
            label_gameBoard_information.Height = e.NewSize.Height - (double)90;

            //// Change la taille des cases du gameBoard
            //if (GameBoard != null)
            //{
            //    for (int x = 0; x < BoardSize; x++)
            //    {
            //        for (int y = 0; y < BoardSize; y++)
            //        {
            //            double cellSize = wrapPanel_gameBoard.Width / (double)BoardSize - 0.001;
            //            GameBoard[x, y].Width = cellSize;
            //            GameBoard[x, y].Height = cellSize;
            //        }
            //    }
            //}
        }

        private void DrawGameBoard()
        {
            // Taille
            GameBoard = new Border[BoardSize, BoardSize];
            uniformGrid_gameBoard.Columns = BoardSize;
            uniformGrid_gameBoard.Rows = BoardSize;

            // Cases ajoutées
            for (int x = 0; x < BoardSize; x++)
            {
                for (int y = 0; y < BoardSize; y++)
                {
                    //double cellSize = wrapPanel_gameBoard.ActualHeight / (double)BoardSize -0.001;

                    // #777777
                    GameBoard[x, y] = new Border()
                    {
                        //Width = cellSize,
                        //Height = cellSize,
                        BorderThickness = new Thickness(borderThickness),
                        BorderBrush = Brushes.Black,
                        Background = Brushes.DimGray,
                        Tag = "0"  // 0 = not a cell, 1 = cell
                    };

                    uniformGrid_gameBoard.Children.Add(GameBoard[x, y]);
                }
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e) { DrawGameBoard(); }

        private void TextBox_KeyDown(object sender, KeyEventArgs e)
        {
            string txtBoxText = (sender as TextBox).Text.Replace(" ", string.Empty);

            if (!String.IsNullOrWhiteSpace(txtBoxText))
            {
                if (e.Key == Key.Enter)
                {
                    int wantedSize = Convert.ToInt32(txtBoxText);
                    if (wantedSize != BoardSize && wantedSize>0)
                    {
                        // Changement de la taille du gameBoard
                        BoardSize = wantedSize;

                        uniformGrid_gameBoard.Children.Clear();
                        DrawGameBoard();

                        label_gameBoard_information.Visibility = Visibility.Hidden;
                    }
                }
            }
        }

        // Vérifie si uniquement des chiffres sont écris dans la txtBox qui définit la taille du maze
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            string txtBoxText = (sender as TextBox).Text.Replace(" ", string.Empty);

            if (!String.IsNullOrWhiteSpace(txtBoxText))
            {
                // Si on a changé la taille du gameBoard dans la txtBox on demande de reload
                if (Convert.ToInt32(txtBoxText) != BoardSize)
                {
                    label_gameBoard_information.Visibility = Visibility.Visible;
                    label_gameBoard_information.Content = "Appuyez sur entrée\npour valider les changements";
                }
                else
                    label_gameBoard_information.Visibility = Visibility.Hidden;
            }
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            DrawGameBoard();

            Timer = new Timer(40);
            Timer.Elapsed += new ElapsedEventHandler(NextAlgoStep);

            // ouvrir un maze générer en json

            //List<Cell[,]> cells = JsonConvert.DeserializeObject<List<Cell[,]>>(File.ReadAllText(@"C:\Users\zonedetec\Desktop\Divers\gen_1_17 54 20.json"));
            //for (int i = 0; i < cells[0].GetLength(0); i++)
            //{
            //    for (int v = 0;v < cells[0].GetLength(0); v++)
            //    {
            //        GameBoard[i, v].BorderThickness = new Thickness(
            //            cells[0][i, v].Borders[0] == 0 ? 0 : 2,
            //            cells[0][i, v].Borders[1] == 0 ? 0 : 2,
            //            cells[0][i, v].Borders[2] == 0 ? 0 : 2,
            //            cells[0][i, v].Borders[3] == 0 ? 0 : 2

            //            );

            //        GameBoard[i, v].Background = Brushes.Wheat;
            //    }
            //}
        }

        private bool IsInRandomWalk = true;  // si on doit choisir une nouvelle random case ou si c'est une rnadom walk
        private bool IsInAddingPath = false;
        private List<int[]> RandomWalkPos = new List<int[]>();
        private int[] CurrentPathPos = new int[2];
        // Y:X

        private int lastdir = -1;
        private bool stopInstantMode = false;

        private bool skipVisual = false;

        private void NextAlgoStep(object? sender, ElapsedEventArgs e)
        {
            Dispatcher.Invoke(() => {

                if (sender == null)
                    sender = "0";
                else
                    sender = "1";

                if (checkBox_instant.IsChecked == true)
                {
                    Timer.Stop();

                    stopInstantMode = false;
                    checkBox_instant.IsChecked = false;

                    skipVisual = true;

                    while (!stopInstantMode)
                        NextAlgoStep(true, null);

                    skipVisual = false;
                    checkBox_instant.IsChecked = true;
                    button_save.IsEnabled = true;
                    button_saveJ.IsEnabled = true;
                }
                else
                
                if (IsInRandomWalk)
                {
                    RandomWalkPos.Clear();

                    // step 1. Pick a random case who is not a cell
                    int rdnX;
                    int rdnY;
                    do
                    {
                        if (NumbersOfCells != BoardSize * BoardSize)
                        {
                            rdnX = Rdn.Next(BoardSize);
                            rdnY = Rdn.Next(BoardSize);
                        }
                        else
                        {
                            stopInstantMode = true;
                            rdnX = -1;
                            rdnY = -1;

                            button_next.IsEnabled = false;
                            button_pause.IsEnabled = false;
                            txtBox_boardSize.IsEnabled = false;
                            button_save.IsEnabled = true;
                            button_saveJ.IsEnabled = true;
                            Timer.Stop();
                            break;
                        }

                    } while (GameBoard[rdnY, rdnX].Tag == "1");

                    // supprime les anciens background flèche
                    if(!skipVisual)
                    for (int x = 0; x < BoardSize; x++)
                    {
                        for (int y = 0; y < BoardSize; y++)
                        {
                            if (GameBoard[x,y].Tag.ToString() == "0")
                                GameBoard[x, y].Background = Brushes.DimGray;
                        }
                    }

                    if (rdnX != -1)
                    {
                        // Y, X
                        // UID = direction
                        RandomWalkPos.Add(new int[2] { rdnY, rdnX });
                        GameBoard[rdnY, rdnX].Uid = "1";

                        // step 2. Random walk
                        IsInRandomWalk = false;
                        IsInAddingPath = false;
                    }

                }
                else if (!IsInAddingPath)
                {
                    // move d'une des 4 directions randomly
                    // 0. gauche
                    // 1. haut
                    // 2. droite
                    // 3. bas

                    bool isPossible = false;
                    int[] lastPos = RandomWalkPos.Last();
                    int direction;
                    do
                    {
                        MORERANDOM:
                        direction = Rdn.Next(4);
                        if (lastdir != direction || checkBox_moreRandom.IsChecked == false)
                        {
                            // check si le est move possible
                            switch (direction)
                            {
                                case 0:
                                    if (lastPos[1] - 1 >= 0)
                                    {
                                        if (checkBox_moreRandom.IsChecked == true && NumbersOfCells > 1)
                                            if (GameBoard[lastPos[0], lastPos[1] - 1].Tag == "1")

                                                //75% de chance de ne pas le faire si autre choix possible
                                                try
                                                {
                                                    if (GameBoard[lastPos[0] - 1, lastPos[1]].Tag == "0"
                                                    || GameBoard[lastPos[0], lastPos[1] + 1].Tag == "0"
                                                    || GameBoard[lastPos[0] + 1, lastPos[1]].Tag == "0")

                                                        if (Rdn.Next(100) < 75)
                                                            goto MORERANDOM;
                                                }
                                                catch { }

                                        RandomWalkPos.Add(new int[] { lastPos[0], lastPos[1] - 1 });
                                        GameBoard[lastPos[0], lastPos[1]].Uid = "0";

                                        if (!skipVisual)
                                            GameBoard[lastPos[0], lastPos[1]].Background = border_left.Background;

                                        isPossible = true;
                                    }
                                    break;
                                case 1:
                                    if (lastPos[0] - 1 >= 0)
                                    {
                                        if (checkBox_moreRandom.IsChecked == true && NumbersOfCells > 1)
                                            if (GameBoard[lastPos[0] - 1, lastPos[1]].Tag == "1")

                                                //75% de chance de ne pas le faire si autre choix possible
                                                try
                                                {
                                                    if (GameBoard[lastPos[0] + 1, lastPos[1]].Tag == "0"
                                                    || GameBoard[lastPos[0], lastPos[1] + 1].Tag == "0"
                                                    || GameBoard[lastPos[0], lastPos[1] - 1].Tag == "0")

                                                        if (Rdn.Next(100) < 75)
                                                            goto MORERANDOM;
                                                }
                                                catch { }

                                        RandomWalkPos.Add(new int[] { lastPos[0] - 1, lastPos[1] });
                                        GameBoard[lastPos[0], lastPos[1]].Uid = "1";

                                        if (!skipVisual)
                                            GameBoard[lastPos[0], lastPos[1]].Background = border_top.Background;

                                        isPossible = true;
                                    }
                                    break;
                                case 2:
                                    if (lastPos[1] + 1 < BoardSize)
                                    {
                                        if (checkBox_moreRandom.IsChecked == true && NumbersOfCells > 1)
                                            if (GameBoard[lastPos[0], lastPos[1] + 1].Tag == "1")

                                                //75% de chance de ne pas le faire si autre choix possible
                                                try
                                                {
                                                    if (GameBoard[lastPos[0] - 1, lastPos[1]].Tag == "0"
                                                    || GameBoard[lastPos[0] + 1, lastPos[1] ].Tag == "0"
                                                    || GameBoard[lastPos[0], lastPos[1] - 1].Tag == "0")

                                                        if (Rdn.Next(100) < 75)
                                                            goto MORERANDOM;
                                                }
                                                catch { }

                                        RandomWalkPos.Add(new int[] { lastPos[0], lastPos[1] + 1 });
                                        GameBoard[lastPos[0], lastPos[1]].Uid = "2";

                                        if (!skipVisual)
                                            GameBoard[lastPos[0], lastPos[1]].Background = border_right.Background;

                                        isPossible = true;
                                    }
                                    break;
                                case 3:
                                    if (lastPos[0] + 1 < BoardSize)
                                    {
                                        if (checkBox_moreRandom.IsChecked == true && NumbersOfCells > 1)
                                            if (GameBoard[lastPos[0] + 1, lastPos[1]].Tag == "1")

                                                //75% de chance de ne pas le faire si autre choix possible
                                                try{
                                                    if (GameBoard[lastPos[0] - 1, lastPos[1]].Tag == "0"
                                                    || GameBoard[lastPos[0], lastPos[1] + 1].Tag == "0"
                                                    || GameBoard[lastPos[0], lastPos[1] - 1].Tag == "0")

                                                        if (Rdn.Next(100) < 75)
                                                            goto MORERANDOM;
                                                }
                                                catch { }

                                                
                                            

                                        RandomWalkPos.Add(new int[] { lastPos[0] + 1, lastPos[1] });
                                        GameBoard[lastPos[0], lastPos[1]].Uid = "3";

                                        if (!skipVisual)
                                            GameBoard[lastPos[0], lastPos[1]].Background = border_bottom.Background;

                                        isPossible = true;
                                    }
                                    break;
                            }
                        }

                    } while (!isPossible);

                    GameBoard[RandomWalkPos.Last()[0], RandomWalkPos.Last()[1]].Background = Brushes.DarkGray;

                    // est-ce que il a attéri sur une cell?
                    if (GameBoard[RandomWalkPos.Last()[0], RandomWalkPos.Last()[1]].Tag == "1")
                    {
                        // on prend donc le chemin et on le créé
                        // on refait le chemin

                        IsInAddingPath = true;
                        IsInRandomWalk = false;

                        CurrentPathPos = RandomWalkPos[0];
                        GameBoard[CurrentPathPos[0], CurrentPathPos[1]].Tag = "1";
                        GameBoard[CurrentPathPos[0], CurrentPathPos[1]].Background = Brushes.White;
                        GameBoard[CurrentPathPos[0], CurrentPathPos[1]].BorderThickness = new Thickness(borderThickness);

                        NumbersOfCells++; 
                    }

                }
                else
                {
                    string lastTag = "0";

                    switch (Convert.ToInt32(GameBoard[CurrentPathPos[0], CurrentPathPos[1]].Uid))
                    {
                        case 0:

                            GameBoard[CurrentPathPos[0], CurrentPathPos[1]].BorderThickness = new Thickness(

                                0, GameBoard[CurrentPathPos[0], CurrentPathPos[1]].BorderThickness.Top, GameBoard[CurrentPathPos[0], CurrentPathPos[1]].BorderThickness.Right, GameBoard[CurrentPathPos[0], CurrentPathPos[1]].BorderThickness.Bottom

                            );

                            CurrentPathPos = new int[] { CurrentPathPos[0], CurrentPathPos[1] - 1 };

                            lastTag = GameBoard[CurrentPathPos[0], CurrentPathPos[1]].Tag.ToString();
                            GameBoard[CurrentPathPos[0], CurrentPathPos[1]].Tag = "1";
                            GameBoard[CurrentPathPos[0], CurrentPathPos[1]].Background = Brushes.White;

                            if (lastTag != "1")
                            {
                                GameBoard[CurrentPathPos[0], CurrentPathPos[1]].BorderThickness = new Thickness(borderThickness, borderThickness, 0, borderThickness);
                            }
                            else
                            {
                                GameBoard[CurrentPathPos[0], CurrentPathPos[1]].BorderThickness = new Thickness(

                                    GameBoard[CurrentPathPos[0], CurrentPathPos[1]].BorderThickness.Left, GameBoard[CurrentPathPos[0], CurrentPathPos[1]].BorderThickness.Top, 0, GameBoard[CurrentPathPos[0], CurrentPathPos[1]].BorderThickness.Bottom

                                );
                            }

                            break;
                        case 1:
                            GameBoard[CurrentPathPos[0], CurrentPathPos[1]].BorderThickness = new Thickness(

                                GameBoard[CurrentPathPos[0], CurrentPathPos[1]].BorderThickness.Left, 0, GameBoard[CurrentPathPos[0], CurrentPathPos[1]].BorderThickness.Right, GameBoard[CurrentPathPos[0], CurrentPathPos[1]].BorderThickness.Bottom

                            );

                            CurrentPathPos = new int[] { CurrentPathPos[0] - 1, CurrentPathPos[1] };
                            lastTag = GameBoard[CurrentPathPos[0], CurrentPathPos[1]].Tag.ToString();

                            GameBoard[CurrentPathPos[0], CurrentPathPos[1]].Tag = "1";
                            ;
                            GameBoard[CurrentPathPos[0], CurrentPathPos[1]].Background = Brushes.White;
                            if (lastTag != "1")
                            {
                                GameBoard[CurrentPathPos[0], CurrentPathPos[1]].BorderThickness = new Thickness(borderThickness, borderThickness, borderThickness, 0);
                            }
                            else
                            {
                                GameBoard[CurrentPathPos[0], CurrentPathPos[1]].BorderThickness = new Thickness(

                                    GameBoard[CurrentPathPos[0], CurrentPathPos[1]].BorderThickness.Left, GameBoard[CurrentPathPos[0], CurrentPathPos[1]].BorderThickness.Top,

                                    GameBoard[CurrentPathPos[0], CurrentPathPos[1]].BorderThickness.Right, 0

                                );
                            }
                            break;
                        case 2:

                            GameBoard[CurrentPathPos[0], CurrentPathPos[1]].BorderThickness = new Thickness(

                                GameBoard[CurrentPathPos[0], CurrentPathPos[1]].BorderThickness.Left, GameBoard[CurrentPathPos[0], CurrentPathPos[1]].BorderThickness.Top, 0, GameBoard[CurrentPathPos[0], CurrentPathPos[1]].BorderThickness.Bottom

                            );

                            CurrentPathPos = new int[] { CurrentPathPos[0], CurrentPathPos[1] + 1 };
                            lastTag = GameBoard[CurrentPathPos[0], CurrentPathPos[1]].Tag.ToString();

                            GameBoard[CurrentPathPos[0], CurrentPathPos[1]].Tag = "1";
                            ;
                            GameBoard[CurrentPathPos[0], CurrentPathPos[1]].Background = Brushes.White;

                            if (lastTag != "1")
                            {
                                GameBoard[CurrentPathPos[0], CurrentPathPos[1]].BorderThickness = new Thickness(0, borderThickness, borderThickness, borderThickness);
                            }
                            else
                            {
                                GameBoard[CurrentPathPos[0], CurrentPathPos[1]].BorderThickness = new Thickness(

                                    0, GameBoard[CurrentPathPos[0], CurrentPathPos[1]].BorderThickness.Top, GameBoard[CurrentPathPos[0], CurrentPathPos[1]].BorderThickness.Right, GameBoard[CurrentPathPos[0], CurrentPathPos[1]].BorderThickness.Bottom

                                );
                            }
                            break;
                        case 3:
                            GameBoard[CurrentPathPos[0], CurrentPathPos[1]].BorderThickness = new Thickness(

                                GameBoard[CurrentPathPos[0], CurrentPathPos[1]].BorderThickness.Left, GameBoard[CurrentPathPos[0], CurrentPathPos[1]].BorderThickness.Top, GameBoard[CurrentPathPos[0], CurrentPathPos[1]].BorderThickness.Right, 0

                            );

                            CurrentPathPos = new int[] { CurrentPathPos[0] + 1, CurrentPathPos[1] };
                            lastTag = GameBoard[CurrentPathPos[0], CurrentPathPos[1]].Tag.ToString();

                            GameBoard[CurrentPathPos[0], CurrentPathPos[1]].Tag = "1";
                            ;
                            GameBoard[CurrentPathPos[0], CurrentPathPos[1]].Background = Brushes.White;
                            if (lastTag != "1")
                            {
                                GameBoard[CurrentPathPos[0], CurrentPathPos[1]].BorderThickness = new Thickness(borderThickness, 0, borderThickness, borderThickness);
                            }
                            else
                            {
                                GameBoard[CurrentPathPos[0], CurrentPathPos[1]].BorderThickness = new Thickness(

                                    GameBoard[CurrentPathPos[0], CurrentPathPos[1]].BorderThickness.Left, 0, GameBoard[CurrentPathPos[0], CurrentPathPos[1]].BorderThickness.Right, GameBoard[CurrentPathPos[0], CurrentPathPos[1]].BorderThickness.Bottom

                                );
                            }
                            break;
                    }

                    if (lastTag == "1")
                    {
                        IsInRandomWalk = true;

                    }
                    else
                        NumbersOfCells++;
                }


            });
        }

        private void Button_StartMazeGenerator_Click(object sender, RoutedEventArgs e)
        {
            if (button_start.Content.ToString() == "Start")
            {
                button_next.IsEnabled = true;
                button_pause.IsEnabled = true;
                txtBox_boardSize.IsEnabled = false;
                button_save.IsEnabled = false;
                button_saveJ.IsEnabled = false;
                button_openJ.IsEnabled = false;


                // start l'algo
                NumbersOfCells = 0;

                // step 1. Add a random cell to the maze
                int rdnY = Rdn.Next(BoardSize);
                int rdnX = Rdn.Next(BoardSize);
                GameBoard[rdnY, rdnX].Tag = "1";
                GameBoard[rdnY, rdnX].Background = Brushes.White;
                GameBoard[rdnY, rdnX].BorderThickness = new Thickness(borderThickness);

                NumbersOfCells++;
                RandomWalkPos.Clear();
                IsInRandomWalk = true;
                IsInAddingPath = false;

                button_start.Content = "Reset";

                if (checkBox_instant.IsChecked == true)
                {
                    // gen en utilisant le Generator car plus rapide
                    actualMazeIndex = 0;
                    mazes = new List<Cell[,]>();
                    var cells = Generator.Gen(BoardSize, checkBox_moreRandom.IsChecked);
                    mazes.Add(cells);

                    ShowActualMaze();

                    button_next.IsEnabled = false;
                    button_pause.IsEnabled = false;
                    button_save.IsEnabled = true;
                    button_saveJ.IsEnabled = true;
                }
            }
            else
            {
                button_start.Content = "Start";
                Timer.Stop();
                button_pause.Content = "Play";

                button_next.IsEnabled = false;
                button_pause.IsEnabled = false;
                button_save.IsEnabled = false;
                button_saveJ.IsEnabled = false;
                txtBox_boardSize.IsEnabled = true;
                button_openJ.IsEnabled = true;

                for (int x = 0; x < BoardSize; x++)
                {
                    for (int y = 0; y < BoardSize; y++)
                    {
                        GameBoard[x, y].Background = Brushes.DimGray;
                        GameBoard[x, y].Tag = "0";
                        GameBoard[x, y].Uid = "-1";
                        GameBoard[x, y].BorderThickness = new Thickness(borderThickness);

                    }
                }
            }
        }

        private void Button_pause_Click(object sender, RoutedEventArgs e)
        {
            if ((sender as Button).Content.ToString() == "Pause")
            {
                Timer.Stop();
                (sender as Button).Content = "Play";
            }
            else
            {
                Timer.Start();
                (sender as Button).Content = "Pause";
            }
        }

        private void button_NextStep_click(object sender, RoutedEventArgs e)
        {
            Timer.Stop();
            NextAlgoStep(this, null);
        }

        private void Slider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if(Timer != null)
            Timer.Interval = e.NewValue;
        }

        private void button_save_Click(object sender, RoutedEventArgs e)
        {

            var dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;
            CommonFileDialogResult result = dialog.ShowDialog();

            if (result == CommonFileDialogResult.Ok)
            {
                GetJpgImage(uniformGrid_gameBoard, 2, 100, dialog.FileName + @"\Labyrinthe " + DateTime.Now.ToString("dd MM yyyy HH mm ss") + ".png");

            }
        }

        public static void GetJpgImage(UIElement source, double scale, int quality, string path)
        {
            double actualHeight = source.RenderSize.Height;
            double actualWidth = source.RenderSize.Width;

            double renderHeight = actualHeight * scale;
            double renderWidth = actualWidth * scale;

            RenderTargetBitmap renderTarget = new RenderTargetBitmap((int)renderWidth, (int)renderHeight, 96, 96, PixelFormats.Pbgra32);
            VisualBrush sourceBrush = new VisualBrush(source);

            DrawingVisual drawingVisual = new DrawingVisual();
            DrawingContext drawingContext = drawingVisual.RenderOpen();

            using (drawingContext)
            {
                drawingContext.PushTransform(new ScaleTransform(scale, scale));
                drawingContext.DrawRectangle(sourceBrush, null, new Rect(new Point(0, 0), new Point(actualWidth, actualHeight)));
            }
            renderTarget.Render(drawingVisual);

            JpegBitmapEncoder jpgEncoder = new JpegBitmapEncoder();
            jpgEncoder.QualityLevel = quality;
            jpgEncoder.Frames.Add(BitmapFrame.Create(renderTarget));

            using (var fs = new FileStream(path, FileMode.Create, FileAccess.Write))
            {
                jpgEncoder.Save(fs);
            }
        }


        private void Button_Click_Gen(object sender, RoutedEventArgs e)
        {
            Window_MassGen window_MassGen = new Window_MassGen();
            window_MassGen.Show();  
        }

        private void button_save_Json_Click(object sender, RoutedEventArgs e)
        {
            List<Cell[,]> cells = new List<Cell[,]>();
            cells.Add(new Cell[BoardSize, BoardSize]);
            for (int x = 0; x < BoardSize; x++)
            {
                for (int y = 0; y < BoardSize; y++)
                {
                    cells[0][x, y] = new Cell();
                    cells[0][x, y].Borders = new int[4]
                    {
                        GameBoard[x,y].BorderThickness.Left == 0 ? 0 : 1,
                        GameBoard[x,y].BorderThickness.Top == 0 ? 0 : 1,
                        GameBoard[x,y].BorderThickness.Right == 0 ? 0 : 1,
                        GameBoard[x,y].BorderThickness.Bottom == 0 ? 0 : 1,
                    };
                }
            }

            var dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;
            CommonFileDialogResult result = dialog.ShowDialog();

            if (result == CommonFileDialogResult.Ok)
            {
                File.WriteAllText(dialog.FileName + @"\maze " + DateTime.Now.ToString("dd MM yyyy HH mm ss") + ".json", JsonConvert.SerializeObject(cells));
            }
        }

        private int actualMazeIndex = 0;
        List<Cell[,]> mazes;

        private void button_openJ_click(object sender, RoutedEventArgs e)
        {
            var dialog = new CommonOpenFileDialog();
            dialog.Filters.Add(new CommonFileDialogFilter("Json file", "json"));

            CommonFileDialogResult result = dialog.ShowDialog();

            if (result == CommonFileDialogResult.Ok)
            {
                string json = File.ReadAllText(dialog.FileName);
                try
                {
                    mazes = JsonConvert.DeserializeObject<List<Cell[,]>>(json);

                    // ouvre les labys
                    BoardSize = mazes[0].GetLength(0);
                    txtBox_boardSize.Text=  mazes[0].GetLength(0).ToString();
                    uniformGrid_gameBoard.Children.Clear();
                    DrawGameBoard();

                    button_previousMaze.IsEnabled = false;

                    if(mazes.Count > 1)
                        button_nextMaze.IsEnabled = true;
                    else
                        button_nextMaze.IsEnabled = false;


                    button_start.Content = "Reset";
                    button_pause.IsEnabled = false;
                    button_save.IsEnabled = true;
                    button_saveJ.IsEnabled = true;
                    button_next.IsEnabled = false;
                    actualMazeIndex = 0;

                    ShowActualMaze();
                }
                catch
                {
                    MessageBox.Show("Fichier Json invalide");
                }
            }
        }

        private void ShowActualMaze()
        {
            for (int x = 0; x < mazes[actualMazeIndex].GetLength(0); x++)
            {
                for (int y = 0; y < mazes[actualMazeIndex].GetLength(1); y++)
                {
                    GameBoard[x, y].Background = Brushes.White;
                    GameBoard[x, y].BorderThickness = new Thickness(

                        mazes[actualMazeIndex][x,y].Borders[0] == 0 ? 0 : borderThickness,
                        mazes[actualMazeIndex][x, y].Borders[1] == 0 ? 0 : borderThickness,
                        mazes[actualMazeIndex][x, y].Borders[2] == 0 ? 0 : borderThickness,
                        mazes[actualMazeIndex][x, y].Borders[3] == 0 ? 0 : borderThickness

                        );
                }
            }
        }

        private void button_nextMaze_Click(object sender, RoutedEventArgs e)
        {
            actualMazeIndex++;
            ShowActualMaze();
            if (actualMazeIndex == mazes.Count - 1)
                button_nextMaze.IsEnabled = false;

            button_previousMaze.IsEnabled = true;

        }

        private void button_previousMaze_Click(object sender, RoutedEventArgs e)
        {
            actualMazeIndex--;
            ShowActualMaze();
            if (actualMazeIndex == 0)
                button_previousMaze.IsEnabled = false;

            button_nextMaze.IsEnabled = true;

        }

        private void Slider_DragCompleted(object sender, DragCompletedEventArgs e)
        {

            string str = Convert.ToInt32(slider_thickness.Value).ToString();
            if (str.Length == 1)

                borderThickness = Convert.ToDouble(str.Insert(0, "0,"));
            else
                borderThickness = Convert.ToDouble(str.Insert(1, ","));

            if (GameBoard != null)
                for (int x = 0; x < BoardSize; x++)
                {
                    for (int y = 0; y < BoardSize; y++)
                    {
                        if (GameBoard[x, y].Background == Brushes.White)
                        {
                            if (GameBoard[x, y].BorderThickness.Left > 0)
                                GameBoard[x, y].BorderThickness = new Thickness(borderThickness, GameBoard[x, y].BorderThickness.Top, GameBoard[x, y].BorderThickness.Right, GameBoard[x, y].BorderThickness.Bottom);
                            if (GameBoard[x, y].BorderThickness.Top > 0)
                                GameBoard[x, y].BorderThickness = new Thickness(GameBoard[x, y].BorderThickness.Left, borderThickness, GameBoard[x, y].BorderThickness.Right, GameBoard[x, y].BorderThickness.Bottom);
                            if (GameBoard[x, y].BorderThickness.Right > 0)
                                GameBoard[x, y].BorderThickness = new Thickness(GameBoard[x, y].BorderThickness.Left, GameBoard[x, y].BorderThickness.Top, borderThickness, GameBoard[x, y].BorderThickness.Bottom);
                            if (GameBoard[x, y].BorderThickness.Bottom > 0)
                                GameBoard[x, y].BorderThickness = new Thickness(GameBoard[x, y].BorderThickness.Left, GameBoard[x, y].BorderThickness.Top, GameBoard[x, y].BorderThickness.Right, borderThickness);
                        }
                        else
                        {
                            GameBoard[x, y].BorderThickness = new Thickness(borderThickness);
                        }
                    }
                }
        }
    }
}
