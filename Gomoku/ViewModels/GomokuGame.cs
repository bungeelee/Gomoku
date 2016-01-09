using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Gomoku.Models;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using System.Windows.Shapes;

namespace Gomoku.ViewModels
{
    public class GomokuGame
    {
        public int gameSize { get; set; }
        public ChessBoard board { get; set; }
        public CellState activePlayer;
        public bool isGameEnded { get; set; }

        public event PlayerWinHandler OnPlayerWin;
        public delegate void PlayerWinHandler(CellState player);

        public GomokuGame()
        {
            this.gameSize = 12;
            board = new ChessBoard(gameSize);
            activePlayer = CellState.black;
        }

        public GomokuGame(int gameSize)
        {
            this.gameSize = gameSize;
            board = new ChessBoard(gameSize);
            activePlayer = CellState.black;
        }

        //Delegate for the winner
        public bool WinnerChecker(int row, int col)
        {
            if (board.CountPlayerItem(row, col, 1, 0, activePlayer) >= 5
                    || board.CountPlayerItem(row, col, 0, 1, activePlayer) >= 5
                    || board.CountPlayerItem(row, col, 1, 1, activePlayer) >= 5
                    || board.CountPlayerItem(row, col, 1, -1, activePlayer) >= 5)
            {
                if (OnPlayerWin != null)
                    OnPlayerWin(player: activePlayer);
                return true;
            }
                return false;
        }


        /// <summary>
        /// Draw chess board
        /// </summary>
        /// <param name="chessBoard"></param>
        public void DrawChessBoard(Canvas chessBoard)
        {
            double x = chessBoard.ActualWidth / (double)gameSize;
            double y = chessBoard.ActualHeight / (double)gameSize;
            double setTop = 0;

            //chesstype = true => background/fill = white
            //chessType = false => background/fill = lightgray
            bool chessType = true;

            for (int i = 0; i < gameSize; i++)
            {
                double setLeft = 0;
                for (int j = 0; j < gameSize; j++)
                {
                    if (chessType)
                    {
                        Grid whiteRec = new Grid()
                        {
                            Height = y,
                            Width = x,
                            Background = Brushes.White,

                        };
                        Canvas.SetLeft(whiteRec, setLeft);
                        Canvas.SetTop(whiteRec, setTop);
                        chessBoard.Children.Add(whiteRec);
                        chessType = false;

                    }
                    else
                    {
                        Grid grayRec = new Grid()
                        {
                            Height = y,
                            Width = x,
                            Background = Brushes.LightGray,
                        };
                        Canvas.SetTop(grayRec, setTop);
                        Canvas.SetLeft(grayRec, setLeft);
                        chessBoard.Children.Add(grayRec);
                        chessType = true;
                    }
                    setLeft += x;
                }
                setTop += y;
                if (i % 2 == 0)
                    chessType = false;
                else
                    chessType = true;
            }


        }

        /// <summary>
        /// Auto resize chessBoard
        /// </summary>
        /// <param name="chessBoard"></param>
        public void UpdateChessBoard(Canvas chessBoard)
        {
            if (chessBoard.Children.Count == 0)
                return;
            double x = chessBoard.ActualWidth / gameSize;
            double y = chessBoard.ActualHeight / gameSize;
            double setTop = 0, setLeft = 0;
            for (int i = 0; i < chessBoard.Children.Count; i++)
            {
                if (i % gameSize == 0)
                {
                    if (i > 0)
                    {
                        setTop += y;
                        setLeft = 0;
                    }
                }
                else
                    setLeft += x;
                Grid temp = (Grid)chessBoard.Children[i];
                temp.Height = y;
                temp.Width = x;
                Canvas.SetTop(temp, setTop);
                Canvas.SetLeft(temp, setLeft);
                chessBoard.Children[i] = temp;
            }
        }

        /// <summary>
        /// Draw chessman to chessboard
        /// </summary>
        /// <param name="chessBoard"></param>
        /// <param name="row"></param>
        /// <param name="col"></param>
        public void DrawChessman(Canvas chessBoard, int row, int col)
        {
            //Calculate position current click
            int chessManPos = (int)row + (int)col * gameSize;

            Grid temp = (Grid)chessBoard.Children[chessManPos];
            //Chessman
            Ellipse chessMan = new Ellipse();
            if (activePlayer == CellState.black)
                chessMan.Fill = Brushes.Black;
            else
                chessMan.Fill = Brushes.Red;

            temp.Children.Add(chessMan);
            chessBoard.Children[chessManPos] = temp;
        }

        public void PlayAt(Canvas chessBoard, Point pos)
        {
            double x = chessBoard.ActualWidth / gameSize;
            double y = chessBoard.ActualHeight / gameSize;

            double row = pos.X / x;
            double col = pos.Y / y;

            if (board.GetChessman((int)row, (int)col) == CellState.none)
            {
                DrawChessman(chessBoard, (int)row, (int)col);
                board.SetChessman(activePlayer, (int)row, (int)col);
                //Change turn to next player
                WinnerChecker((int)row, (int)col);
                if (activePlayer == CellState.black)
                    activePlayer = CellState.red;
                else activePlayer = CellState.black;
               // return 
            }
            //return true;
        }

        public void PlayAt(Canvas chessBoard, int row, int col)
        {
            if (board.GetChessman(row, col) == CellState.none)
            {
                DrawChessman(chessBoard, row, col);
                board.SetChessman(activePlayer, row, col);
                WinnerChecker(row, col);
                //change turn to next player
                if (activePlayer == CellState.black)
                    activePlayer = CellState.red;
                else activePlayer = CellState.black;
            }
            //return false;

        }

    }
}
