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
        public event PlayerWinHandler OnPlayerWin;
        public delegate void PlayerWinHandler(CellState player);


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

            if (board.GetChessman((int)row, (int)col) == CellState.none)
            {
                DrawChessman(chessBoard, (int)row, (int)col);
                board.SetChessman(activePlayer, (int)row, (int)col);

                if (board.CountPlayerItem((int)row, (int)col, 1, 0, activePlayer) >= 5
                    || board.CountPlayerItem((int)row, (int)col, 0, 1, activePlayer) >= 5
                    || board.CountPlayerItem((int)row, (int)col, 1, 1, activePlayer) >= 5
                    || board.CountPlayerItem((int)row, (int)col, 1, -1, activePlayer) >= 5)
                {
                    if (OnPlayerWin != null)
                        OnPlayerWin(player: activePlayer);
                    return true;
                }
            }
            return true;
        }

        //For machine
        public override bool PlayAt(Canvas chessBoard, int row, int col)
        {
            if (board.GetChessman(row, col) == CellState.none)
            {
                DrawChessman(chessBoard, row, col);

                board.SetChessman(activePlayer, row, col);
                if (board.CountPlayerItem(row, col, 1, 0, activePlayer) >= 5
                    || board.CountPlayerItem(row, col, 0, 1, activePlayer) >= 5
                    || board.CountPlayerItem(row, col, 1, 1, activePlayer) >= 5
                    || board.CountPlayerItem(row, col, 1, -1, activePlayer) >= 5)
                {
                    if (OnPlayerWin != null)
                        OnPlayerWin(player: activePlayer);
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
