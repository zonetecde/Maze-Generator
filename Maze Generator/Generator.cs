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

            // Tableau de cells vierge
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

            // start l'algo
            int NumbersOfCells = 0;

            // Ajout d'une cellule random au lab
            int rdnY = Rdn.Next(BoardSize);
            int rdnX = Rdn.Next(BoardSize);

            GameBoard[rdnY, rdnX].Tag = "1"; // id d'une cell = 1
            GameBoard[rdnY, rdnX].AllBorder(); // 4 bordures
            NumbersOfCells++; 

            bool IsInRandomWalk = true;
            bool IsInAddingPath = false;
            bool stopInstantMode = false;

            List<int[]> RandomWalkPos = new List<int[]>();
            int[] CurrentPathPos = new int[2];
            int lastdir = -1;

            while (!stopInstantMode) // génération fini 
            {
                if (IsInRandomWalk)
                {
                    RandomWalkPos.Clear();

                    // Choisi une case random qui n'est pas une celle
                    do
                    {
                        if (NumbersOfCells != BoardSize * BoardSize) // Si il reste des cases non-cells
                        {
                            rdnX = Rdn.Next(BoardSize);
                            rdnY = Rdn.Next(BoardSize);
                        }
                        else // Laby fini
                        {
                            stopInstantMode = true;
                            rdnX = -1;
                            rdnY = -1;
                            break;
                        }

                    } while (GameBoard[rdnY, rdnX].Tag == "1");

                    if (rdnX != -1) // Si une case vide est trouvé
                    {
                        // Y, X
                        // UID = direction
                        RandomWalkPos.Add(new int[2] { rdnY, rdnX }); // On commence notre chemin d'ici

                        // set la random walk
                        IsInRandomWalk = false;
                        IsInAddingPath = false;
                    }

                    while (!IsInAddingPath)
                    {
                        // move d'une des 4 directions randomly à partir de la dernière case de chemin placé
                        // 0. gauche
                        // 1. haut
                        // 2. droite
                        // 3. bas

                        bool isPossible = false;
                        int[] lastPos = RandomWalkPos.Last();

                        do
                        {
                            MORERANDOM:

                            int direction = Rdn.Next(4); // direction random
                            if (lastdir != direction || moreRdn == false) // si on a le mode "moreRdn" alors on prend une direction diff que la dernière
                            {
                                // check si le est move possible
                                switch (direction)
                                {
                                    case 0:
                                        if (lastPos[1] - 1 >= 0) // testeur de limite du gameBoard
                                        {
                                            if (moreRdn == true && NumbersOfCells > 1)
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

                                            RandomWalkPos.Add(new int[] { lastPos[0], lastPos[1] - 1 }); // ajoute la prochaine pos
                                            GameBoard[lastPos[0], lastPos[1]].Uid = "0"; // set la direction actuelle vers la prochaine pos

                                            isPossible = true;
                                        }
                                        break;
                                    case 1:
                                        if (lastPos[0] - 1 >= 0)
                                        {
                                            if (moreRdn == true && NumbersOfCells > 1)
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
                                            isPossible = true;
                                        }
                                        break;
                                    case 2:
                                        if (lastPos[1] + 1 < BoardSize)
                                        {
                                            if (moreRdn == true && NumbersOfCells > 1)
                                                if (GameBoard[lastPos[0], lastPos[1] + 1].Tag == "1")

                                                    //75% de chance de ne pas le faire si autre choix possible
                                                    try
                                                    {
                                                        if (GameBoard[lastPos[0] - 1, lastPos[1]].Tag == "0"
                                                        || GameBoard[lastPos[0] + 1, lastPos[1]].Tag == "0"
                                                        || GameBoard[lastPos[0], lastPos[1] - 1].Tag == "0")

                                                            if (Rdn.Next(100) < 75)
                                                                goto MORERANDOM;
                                                    }
                                                    catch { }

                                            RandomWalkPos.Add(new int[] { lastPos[0], lastPos[1] + 1 });
                                            GameBoard[lastPos[0], lastPos[1]].Uid = "2";
                                            isPossible = true;
                                        }
                                        break;
                                    case 3:
                                        if (lastPos[0] + 1 < BoardSize)
                                        {
                                            if (moreRdn == true && NumbersOfCells > 1)
                                                if (GameBoard[lastPos[0] + 1, lastPos[1]].Tag == "1")

                                                    //75% de chance de ne pas le faire si autre choix possible
                                                    try
                                                    {
                                                        if (GameBoard[lastPos[0] - 1, lastPos[1]].Tag == "0"
                                                        || GameBoard[lastPos[0], lastPos[1] + 1].Tag == "0"
                                                        || GameBoard[lastPos[0], lastPos[1] - 1].Tag == "0")

                                                            if (Rdn.Next(100) < 75)
                                                                goto MORERANDOM;
                                                    }
                                                    catch { }


                                            RandomWalkPos.Add(new int[] { lastPos[0] + 1, lastPos[1] });
                                            GameBoard[lastPos[0], lastPos[1]].Uid = "3";
                                            isPossible = true;
                                        }
                                        break;
                                }
                            }

                        } while (!isPossible);

                        if (GameBoard[RandomWalkPos.Last()[0], RandomWalkPos.Last()[1]].Tag == "1") // Si on rejoint le laby
                        {
                            // Prend et créé le chemin le plus cours
                            IsInAddingPath = true;
                            IsInRandomWalk = false;

                            CurrentPathPos = RandomWalkPos[0];
                            GameBoard[CurrentPathPos[0], CurrentPathPos[1]].Tag = "1";
                            GameBoard[CurrentPathPos[0], CurrentPathPos[1]].AllBorder();
                            NumbersOfCells++;
                        }

                        while (IsInAddingPath && !IsInRandomWalk)
                        {
                            string lastTag = "0"; // si le chemin est terminé est = à 1

                            switch (Convert.ToInt32(GameBoard[CurrentPathPos[0], CurrentPathPos[1]].Uid)) // Prend la direction de la premiere cellule
                            {
                                case 0: // direction

                                    GameBoard[CurrentPathPos[0], CurrentPathPos[1]].LeftBorder = false;  // la sortie sera forcèmeent à gauche

                                    CurrentPathPos = new int[] { CurrentPathPos[0], CurrentPathPos[1] - 1 }; // nouvelle pos du lab

                                    lastTag = GameBoard[CurrentPathPos[0], CurrentPathPos[1]].Tag.ToString();
                                    GameBoard[CurrentPathPos[0], CurrentPathPos[1]].Tag = "1";

                                    if (lastTag != "1") // si on rejoint pas le lab on met une entré à la direction opposé
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
