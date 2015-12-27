using Gomoku.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Gomoku.ViewModels
{
    public class GomokuOffline: GomokuGame
    {
        public GomokuOffline()
        {
            this.gameSize = 12;
            board = new ChessBoard(gameSize);
            activePlayer = CellState.black;

        }
        public GomokuOffline(int gameSize)
        {
            this.gameSize = gameSize;
            board = new ChessBoard(gameSize);
            activePlayer = CellState.black;
        }

        //for player vs player on local
        public override bool PlayAt(Canvas chessBoard, Point pos)
        {
            double x = chessBoard.ActualWidth / gameSize;
            double y = chessBoard.ActualHeight / gameSize;

            double row = pos.X / x;
            double col = pos.Y / y;
            //Calculate position current click
            int chessManPos = (int)row + (int)col * gameSize;
            if (chessManPos > gameSize * gameSize || chessManPos < 0) return false;
            //Add chessman to chessBoard
            Grid temp = (Grid)chessBoard.Children[chessManPos];
            Ellipse chessMan = new Ellipse();
            if (activePlayer == CellState.black)
                chessMan.Fill = Brushes.Black;
            else
                chessMan.Fill = Brushes.Red;
            if (temp.Children.Count != 0) return false;

            if (board.GetChessman((int)row, (int)col) == CellState.none)
            {
                temp.Children.Add(chessMan);
                chessBoard.Children[chessManPos] = temp;

                board.SetChessman(activePlayer, (int)row, (int)col);

                if (board.CountPlayerItem((int)row, (int)col, 1, 0, activePlayer) >= 5
                    || board.CountPlayerItem((int)row, (int)col, 0, 1, activePlayer) >= 5
                    || board.CountPlayerItem((int)row, (int)col, 1, 1, activePlayer) >= 5
                    || board.CountPlayerItem((int)row, (int)col, 1, -1, activePlayer) >= 5)
                {
                    isGameEnded = true;
                    MessageBox.Show(activePlayer.ToString());
                    return true;
                }
            }
            return true;
        }

        //For machine
        public override bool PlayAt(Canvas chessBoard, int X, int Y)
        {
            double x = chessBoard.ActualWidth / gameSize;
            double y = chessBoard.ActualHeight / gameSize;

            double row = X;
            double col = Y;
            //Calculate position current click
            int chessManPos = (int)row + (int)col * gameSize;

            //Add chessman to chessBoard
            Grid temp = (Grid)chessBoard.Children[chessManPos];
            Ellipse chessMan = new Ellipse();
            if (activePlayer == CellState.black)
                chessMan.Fill = Brushes.Black;
            else
                chessMan.Fill = Brushes.Red;
            if (temp.Children.Count != 0) return true;

            if (board.GetChessman((int)row, (int)col) == CellState.none)
            {
                temp.Children.Add(chessMan);
                chessBoard.Children[chessManPos] = temp;

                board.SetChessman(activePlayer, (int)row, (int)col);

                if (board.CountPlayerItem((int)row, (int)col, 1, 0, activePlayer) >= 5
                    || board.CountPlayerItem((int)row, (int)col, 0, 1, activePlayer) >= 5
                    || board.CountPlayerItem((int)row, (int)col, 1, 1, activePlayer) >= 5
                    || board.CountPlayerItem((int)row, (int)col, 1, -1, activePlayer) >= 5)
                {

                    MessageBox.Show(activePlayer.ToString());
                    return true;
                }
                if (activePlayer == CellState.black)
                    activePlayer = CellState.red;
                else activePlayer = CellState.black;
            }
            return true;
        }

    }
}
