﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.WindowsAPICodePack.Dialogs;

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

        public double borderThickness = 1;
        public int BoardSize = 6;
        public int NumbersOfCells = 0;

        BrushConverter HexToBrushesConverter = new BrushConverter();
        Random Rdn = new Random();

        private Timer Timer;

        public MainWindow() { InitializeComponent(); }

        private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            // Change la taille du gameBoard, les cases ne sont pas ajouté au lancement
            wrapPanel_gameBoard.Width = e.NewSize.Height - (double)90;
            wrapPanel_gameBoard.Height = e.NewSize.Height - (double)90;

            label_gameBoard_information.Width = e.NewSize.Height - (double)90;
            label_gameBoard_information.Height = e.NewSize.Height - (double)90;

            // Change la taille des cases du gameBoard
            if (GameBoard != null)
            {
                for (int x = 0; x < BoardSize; x++)
                {
                    for (int y = 0; y < BoardSize; y++)
                    {
                        double cellSize = wrapPanel_gameBoard.Width / (double)BoardSize - 0.01;
                        GameBoard[x, y].Width = cellSize;
                        GameBoard[x, y].Height = cellSize;
                    }
                }
            }
        }

        private void DrawGameBoard()
        {
            // Taille
            GameBoard = new Border[BoardSize, BoardSize];

            // Cases ajoutées
            for (int x = 0; x < BoardSize; x++)
            {
                for (int y = 0; y < BoardSize; y++)
                {
                    double cellSize = wrapPanel_gameBoard.Width / (double)BoardSize - 0.01;

                    // #777777
                    Brush background = (Brush)HexToBrushesConverter.ConvertFromString("#777777");
                    GameBoard[x, y] = new Border()
                    {
                        Width = cellSize,
                        Height = cellSize,
                        BorderThickness = new Thickness(1, 1.5, 1, 1),
                        BorderBrush = Brushes.Black,
                        Background = background,
                        CornerRadius = new CornerRadius(0),
                        Tag = 0  // 0 = not a cell, 1 = cell
                    };

                    wrapPanel_gameBoard.Children.Add(GameBoard[x, y]);
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
                    if (wantedSize != BoardSize)
                    {
                        // Changement de la taille du gameBoard
                        BoardSize = wantedSize;

                        wrapPanel_gameBoard.Children.Clear();
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
        }

        private bool IsInRandomWalk = true;  // si on doit choisir une nouvelle random case ou si c'est une rnadom walk
        private bool IsInAddingPath = false;
        private List<int[]> RandomWalkPos = new List<int[]>();
        private int[] CurrentPathPos = new int[2];
        // Y:X

        private int lastdir = -1;

        private void NextAlgoStep(object? sender, ElapsedEventArgs e)
        {
            Dispatcher.Invoke(() => {

                if (checkBox_instant.IsChecked == true)
                {
                    Timer.Stop();

                    checkBox_instant.IsChecked = false;
                    while (NumbersOfCells != BoardSize * BoardSize)
                        NextAlgoStep(this, null);

                    checkBox_instant.IsChecked = true;
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
                            rdnX = -1;
                            rdnY = -1;

                            button_next.IsEnabled = false;
                            button_pause.IsEnabled = false;
                            txtBox_boardSize.IsEnabled = false;
                            button_save.IsEnabled = true;
                            Timer.Stop();
                            break;
                        }

                    } while (GameBoard[rdnY, rdnX].Tag == "1");

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

                    do
                    {
                        int direction = Rdn.Next(4);
                        if (lastdir != direction || checkBox_moreRandom.IsChecked == false)
                        {
                            // check si le est move possible
                            switch (direction)
                            {
                                case 0:
                                    if (lastPos[1] - 1 >= 0)
                                    {
                                        RandomWalkPos.Add(new int[] { lastPos[0], lastPos[1] - 1 });
                                        GameBoard[lastPos[0], lastPos[1]].Uid = "0";
                                        GameBoard[lastPos[0], lastPos[1]].Background = Brushes.Cyan;

                                        isPossible = true;
                                    }
                                    break;
                                case 1:
                                    if (lastPos[0] - 1 >= 0)
                                    {
                                        RandomWalkPos.Add(new int[] { lastPos[0] - 1, lastPos[1] });
                                        GameBoard[lastPos[0], lastPos[1]].Uid = "1";

                                        GameBoard[lastPos[0], lastPos[1]].Background = Brushes.Red;

                                        isPossible = true;
                                    }
                                    break;
                                case 2:
                                    if (lastPos[1] + 1 < BoardSize)
                                    {
                                        RandomWalkPos.Add(new int[] { lastPos[0], lastPos[1] + 1 });
                                        GameBoard[lastPos[0], lastPos[1]].Uid = "2";

                                        GameBoard[lastPos[0], lastPos[1]].Background = Brushes.Green;

                                        isPossible = true;
                                    }
                                    break;
                                case 3:
                                    if (lastPos[0] + 1 < BoardSize)
                                    {
                                        RandomWalkPos.Add(new int[] { lastPos[0] + 1, lastPos[1] });
                                        GameBoard[lastPos[0], lastPos[1]].Uid = "3";

                                        GameBoard[lastPos[0], lastPos[1]].Background = Brushes.Pink;

                                        isPossible = true;
                                    }
                                    break;
                            }
                        }

                    } while (!isPossible);

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
                    else  // effet visuel
                    {
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

                if(checkBox_instant.IsChecked == true)
                    while(NumbersOfCells != BoardSize * BoardSize)
                        NextAlgoStep(this, null);
            }
            else
            {
                button_start.Content = "Start";
                Timer.Stop();
                button_pause.Content = "Play";

                button_next.IsEnabled = false;
                button_pause.IsEnabled = false;
                button_save.IsEnabled = false;
                txtBox_boardSize.IsEnabled = true;
                wrapPanel_gameBoard.Children.Clear();
                DrawGameBoard();
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
            var image = CreateBitmapSourceFromVisual(wrapPanel_gameBoard.ActualWidth, wrapPanel_gameBoard.ActualHeight, wrapPanel_gameBoard, true);

            var dialog = new CommonOpenFileDialog();
            dialog.IsFolderPicker = true;
            CommonFileDialogResult result = dialog.ShowDialog();

            if (result == CommonFileDialogResult.Ok)
            {
                using (var fileStream = new FileStream(dialog.FileName + @"\Labyrinthe " + DateTime.Now.ToString("dd MM yyyy HH mm ss") + ".png", FileMode.Create))
                {
                    BitmapEncoder encoder = new PngBitmapEncoder();
                    encoder.Frames.Add(BitmapFrame.Create(image));
                    encoder.Save(fileStream);
                }
            }


        }

        internal static BitmapSource CreateBitmapSourceFromVisual(
                Double width,
                Double height,
                Visual visualToRender,
                Boolean undoTransformation)
        {
            if (visualToRender == null)
            {
                return null;
            }
            RenderTargetBitmap bmp = new RenderTargetBitmap((Int32)Math.Ceiling(width),
                (Int32)Math.Ceiling(height), 96, 96, PixelFormats.Pbgra32);

            if (undoTransformation)
            {
                DrawingVisual dv = new DrawingVisual();
                using (DrawingContext dc = dv.RenderOpen())
                {
                    VisualBrush vb = new VisualBrush(visualToRender);
                    dc.DrawRectangle(vb, null, new Rect(new Point(), new Size(width, height)));
                }
                bmp.Render(dv);
            }
            else
            {
                bmp.Render(visualToRender);
            }
            return bmp;
        }

        private void Slider_ValueChanged_1(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            string str = Convert.ToInt32( e.NewValue).ToString();
            if (str.Length == 1)

                borderThickness = Convert.ToDouble(str.Insert(0, "0,"));
            else
                borderThickness = Convert.ToDouble(str.Insert(1, ","));

            if(GameBoard != null)
            for (int x = 0; x < BoardSize; x++)
            {
                for (int y = 0; y < BoardSize; y++)
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
            }
        }
    }
}