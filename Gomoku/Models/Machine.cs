using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Gomoku.Models
{
    public abstract class Machine
    {
        public readonly static Point Draw = new Point(-1, -1);
        ChessBoard board;
        protected int gameSize;


        public Machine(ChessBoard board, int gameSize)
        {
            this.board = board;
            this.gameSize = gameSize;
        }

        public ChessBoard Board { get { return board; } }

        public bool HaveVictoryAt(int x, int y, CellState color)
        {
            return Board.GetChessman(x, y) == color && CalcScore(x, y, color) >= 11; ;
        }
        public bool HaveVictoryAt(Point pos, CellState color)
        {
            return true;
        }

        public abstract Point SelectBestCell(CellState color);

        public CellState GetOpponentColor(CellState myColor)
        {
            if (myColor == CellState.red)
                return CellState.black;
            if (myColor == CellState.black)
                return CellState.red;
            throw new ArgumentException();
        }

        protected int CalcScore(int row, int col, CellState color)
        {
            int[] counts = new int[]
            {
                Board.CountPlayerItem(row, col, 1, 0, color),
                Board.CountPlayerItem(row, col, 0, 1, color),
                Board.CountPlayerItem(row, col, 1, 1, color),
                Board.CountPlayerItem(row, col, 1, -1, color)
            };
            int result = 0;
            for (int i = 0; i < counts.Length; i++)
            {
                result = Math.Max(result, counts[i]);
            }
            return result;
        }

    }
}
