using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Gomoku.Models
{
    public class ChessBoard
    {
        public int gameSize { get; set; }
        private CellState[,] board;
        public CellState currentPlayer { get; set; }

        public ChessBoard(){
            gameSize = 12;
            ClearBoard();
        }

        public ChessBoard(int gameSize){
            this.gameSize = gameSize;
            ClearBoard();
        }

        private void ClearBoard(){
            board = new CellState[gameSize, gameSize];
        }

        public void SetChessman(CellState color, int x, int y)
        {
            if (IsInBoard(x, y))
                board[x, y] = color;
            else
                throw new ArgumentException();
        }

        public CellState GetChessman(Point pos)
        {
            return GetChessman((int)pos.X, (int)pos.Y);
        }

        public CellState GetChessman(int x, int y)
        {
            if (IsInBoard(x, y))
                return board[x, y];
            else return CellState.black;
            //throw new ArgumentException();


        }
        private bool IsInBoard(int row, int col)
        {
            return row >= 0 && row < gameSize && col >= 0 && col < gameSize;
        }

        public int CountPlayerItem(int row, int col, int drow, int dcol, CellState curPlayer)
        {
            if (curPlayer == CellState.none)
                return 0;
            int crow = row + drow;
            int ccol = col + dcol;
            int count = 1;
            while (IsInBoard(crow, ccol) && board[crow, ccol] == curPlayer)
            {
                count++;
                crow = crow + drow;
                ccol = ccol + dcol;
            }
            crow = row - drow;
            ccol = col - dcol;
            while (IsInBoard(crow, ccol) && board[crow, ccol] == curPlayer)
            {
                count++;
                crow = crow - drow;
                ccol = ccol - dcol;
            }
            return count;
        }


        public IEnumerable<Point> EnumerateCells()
        {
            for (int y = 0; y < gameSize; y++)
            {
                for (int x = 0; x < gameSize; x++)
                {
                    yield return new Point(x, y);
                }
            }
        }
    }
}
