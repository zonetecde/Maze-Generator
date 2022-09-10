using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Maze_Generator
{
    internal static class Generator
    {
        internal static Random Rdn = new Random();

        internal static Cell[,] Gen(int BoardSize, bool? moreRdn)
        {
            Cell[,] GameBoard = new Cell[BoardSize, BoardSize];


            // start l'algo
            int NumbersOfCells = 0;

            // step 1. Add a random cell to the maze
            int rdnY = Rdn.Next(BoardSize);
            int rdnX = Rdn.Next(BoardSize);

            for (int x = 0; x < BoardSize; x++)
            {
                for (int y = 0; y < BoardSize; y++)
                {
                    GameBoard[x, y] = new Cell()
                    {
                        BottomBorder = false,
                        LeftBorder = false,
                        RightBorder = false,
                        TopBorder = false,

                        Tag = "0",
                        Uid = "-1"
                    };
                }
            }

            GameBoard[rdnY, rdnX].Tag = "1";
            GameBoard[rdnY, rdnX].AllBorder();
            NumbersOfCells++;

            bool IsInRandomWalk = true;
            bool IsInAddingPath = false;
            List<int[]> RandomWalkPos = new List<int[]>();
            int[] CurrentPathPos = new int[2];
            int lastdir = -1;

            while (NumbersOfCells != BoardSize * BoardSize)
            {

                if (IsInRandomWalk)
                {
                    RandomWalkPos.Clear();

                    // step 1. Pick a random case who is not a cell
                    do
                    {

                        rdnX = Rdn.Next(BoardSize);
                        rdnY = Rdn.Next(BoardSize);


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
                        if (lastdir != direction || moreRdn == false)
                        {
                            // check si le est move possible
                            switch (direction)
                            {
                                case 0:
                                    if (lastPos[1] - 1 >= 0)
                                    {
                                        RandomWalkPos.Add(new int[] { lastPos[0], lastPos[1] - 1 });
                                        GameBoard[lastPos[0], lastPos[1]].Uid = "0";

                                        isPossible = true;
                                    }
                                    break;
                                case 1:
                                    if (lastPos[0] - 1 >= 0)
                                    {
                                        RandomWalkPos.Add(new int[] { lastPos[0] - 1, lastPos[1] });
                                        GameBoard[lastPos[0], lastPos[1]].Uid = "1";
                                        isPossible = true;
                                    }
                                    break;
                                case 2:
                                    if (lastPos[1] + 1 < BoardSize)
                                    {
                                        RandomWalkPos.Add(new int[] { lastPos[0], lastPos[1] + 1 });
                                        GameBoard[lastPos[0], lastPos[1]].Uid = "2";
                                        isPossible = true;
                                    }
                                    break;
                                case 3:
                                    if (lastPos[0] + 1 < BoardSize)
                                    {
                                        RandomWalkPos.Add(new int[] { lastPos[0] + 1, lastPos[1] });
                                        GameBoard[lastPos[0], lastPos[1]].Uid = "3";
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
                        GameBoard[CurrentPathPos[0], CurrentPathPos[1]].AllBorder();
                        NumbersOfCells++;
                    }
                }
                else
                {
                    string lastTag = "0";

                    switch (Convert.ToInt32(GameBoard[CurrentPathPos[0], CurrentPathPos[1]].Uid))
                    {
                        case 0:

                            GameBoard[CurrentPathPos[0], CurrentPathPos[1]].LeftBorder = false; 

                            CurrentPathPos = new int[] { CurrentPathPos[0], CurrentPathPos[1] - 1 };

                            lastTag = GameBoard[CurrentPathPos[0], CurrentPathPos[1]].Tag.ToString();
                            GameBoard[CurrentPathPos[0], CurrentPathPos[1]].Tag = "1";

                            if (lastTag != "1")
                            {
                                GameBoard[CurrentPathPos[0], CurrentPathPos[1]].LeftBorder = true;
                                GameBoard[CurrentPathPos[0], CurrentPathPos[1]].TopBorder = true; 
                                GameBoard[CurrentPathPos[0], CurrentPathPos[1]].RightBorder = false; 
                                GameBoard[CurrentPathPos[0], CurrentPathPos[1]].BottomBorder = true; 
                            }
                            else
                            {
                                GameBoard[CurrentPathPos[0], CurrentPathPos[1]].RightBorder = false;
                                
                            }

                            break;
                        case 1:
                            GameBoard[CurrentPathPos[0], CurrentPathPos[1]].TopBorder = false;

                            CurrentPathPos = new int[] { CurrentPathPos[0] - 1, CurrentPathPos[1] };
                            lastTag = GameBoard[CurrentPathPos[0], CurrentPathPos[1]].Tag.ToString();

                            GameBoard[CurrentPathPos[0], CurrentPathPos[1]].Tag = "1";
                            ;
                            if (lastTag != "1")
                            {
                                GameBoard[CurrentPathPos[0], CurrentPathPos[1]].LeftBorder = true;
                                GameBoard[CurrentPathPos[0], CurrentPathPos[1]].TopBorder = true;
                                GameBoard[CurrentPathPos[0], CurrentPathPos[1]].RightBorder = true;
                                GameBoard[CurrentPathPos[0], CurrentPathPos[1]].BottomBorder = false;
                            }
                            else
                            {
                                GameBoard[CurrentPathPos[0], CurrentPathPos[1]].BottomBorder = false;

                            }
                            break;
                        case 2:

                            GameBoard[CurrentPathPos[0], CurrentPathPos[1]].RightBorder = false;


                            CurrentPathPos = new int[] { CurrentPathPos[0], CurrentPathPos[1] + 1 };
                            lastTag = GameBoard[CurrentPathPos[0], CurrentPathPos[1]].Tag.ToString();

                            GameBoard[CurrentPathPos[0], CurrentPathPos[1]].Tag = "1";
                            ;

                            if (lastTag != "1")
                            {
                                GameBoard[CurrentPathPos[0], CurrentPathPos[1]].LeftBorder = false;
                                GameBoard[CurrentPathPos[0], CurrentPathPos[1]].TopBorder = true;
                                GameBoard[CurrentPathPos[0], CurrentPathPos[1]].RightBorder = true;
                                GameBoard[CurrentPathPos[0], CurrentPathPos[1]].BottomBorder = true;
                            }
                            else
                            {
                                GameBoard[CurrentPathPos[0], CurrentPathPos[1]].LeftBorder = false;

                            }
                            break;
                        case 3:
                            GameBoard[CurrentPathPos[0], CurrentPathPos[1]].BottomBorder = false;


                            CurrentPathPos = new int[] { CurrentPathPos[0] + 1, CurrentPathPos[1] };
                            lastTag = GameBoard[CurrentPathPos[0], CurrentPathPos[1]].Tag.ToString();

                            GameBoard[CurrentPathPos[0], CurrentPathPos[1]].Tag = "1";
                            ;

                            if (lastTag != "1")
                            {
                                GameBoard[CurrentPathPos[0], CurrentPathPos[1]].LeftBorder = true;
                                GameBoard[CurrentPathPos[0], CurrentPathPos[1]].TopBorder = false;
                                GameBoard[CurrentPathPos[0], CurrentPathPos[1]].RightBorder = true;
                                GameBoard[CurrentPathPos[0], CurrentPathPos[1]].BottomBorder = true;
                            }
                            else
                            {
                                GameBoard[CurrentPathPos[0], CurrentPathPos[1]].TopBorder = false;

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
            }

            for (int x = 0; x < BoardSize; x++)
            {
                for (int y = 0; y < BoardSize; y++)
                {
                    GameBoard[x, y].Borders = new int[4]
                    {
                       Convert.ToInt16( GameBoard[x, y].LeftBorder),
                        Convert.ToInt16( GameBoard[x, y].TopBorder),
                        Convert.ToInt16( GameBoard[x, y].RightBorder),
                        Convert.ToInt16( GameBoard[x, y].BottomBorder),
                    };
                }
            }

            return GameBoard;
        }
    }
}
