using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Gomoku.Models
{
    public class SimpleMachine :Machine
    {
        delegate IComparable EstimateFunction(Point location, CellState color);

        static readonly Random rand = new Random();

        public SimpleMachine(ChessBoard board, int gameSize) : base(board, gameSize)
        {

        }

        public override Point SelectBestCell(CellState color)
        {
            var candidates = FilterCellsStageThree(color);

            if (candidates.Count == 0)
                return Draw;

            if (candidates.Count == 1)
                return candidates[0];

            int index = rand.Next(0, candidates.Count - 1);
            return candidates[index];
        }

        internal List<Point> FilterCellsStageOne(CellState color)
        {
            return FilterCellsCore(Board.EnumerateCells(), EstimateForStageOne, color);
        }


        List<Point> FilterCellsStageTwo(CellState color)
        {
            return FilterCellsCore(FilterCellsStageOne(color), EstimateForStageTwo, color);
        }

        private IComparable EstimateForStageTwo(Point location, CellState color)
        {
            int cx = (int)location.X;
            int cy = (int)location.Y;

            int selfCount = 0;
            int opponentCount = 0;

            for (int x = cx - 1; x <= cx + 1; x++)
            {
                for (int y = cy - 1; y <= cy + 1; y++)
                {
                    if (Board.GetChessman(x, y) == color)
                        selfCount++;
                    if (Board.GetChessman(x, y) == GetOpponentColor(color))
                        opponentCount++;
                }
            }

            return 2 * selfCount + opponentCount;
            //throw new NotImplementedException();
        }

        List<Point> FilterCellsStageThree(CellState color)
        {
            return FilterCellsCore(FilterCellsStageTwo(color), EstimateForStageThree, color);
        }

        private IComparable EstimateForStageThree(Point location, CellState color)
        {
            var dx = location.X - gameSize / 2;
            var dy = location.Y - gameSize / 2;
            return -Math.Sqrt(dx * dx + dy * dy);
            //throw new NotImplementedException();
        }

        List<Point> FilterCellsCore(IEnumerable<Point> source, EstimateFunction estimator, CellState color)
        {
            var result = new List<Point>();
            IComparable bestEstimate = null;

            foreach (Point pos in source)
            {
                if (Board.GetChessman(pos) != CellState.none)
                    continue;
                var estimate = estimator(pos, color);

                int compareResult = estimate.CompareTo(bestEstimate);
                if (compareResult < 0)
                    continue;
                if (compareResult > 0)
                {
                    result.Clear();
                    bestEstimate = estimate;
                }
                result.Add(pos);
            }

            return result;
        }
        private IComparable EstimateForStageOne(Point location, CellState color)
        {
            int selfScore = 1 + CalcScore((int)location.X, (int)location.Y, color);
            int opponentScore = 1 + CalcScore((int)location.X, (int)location.Y, GetOpponentColor(color));

            if (selfScore >= gameSize)
                selfScore = int.MaxValue;

            return Math.Max(selfScore, opponentScore);
            //throw new NotImplementedException();
        }
        internal IComparable EstimateForStageOne(int row, int col, CellState color)
        {
            return EstimateForStageOne(new Point(row, col), color);
        }

        internal IComparable EstimateForStageTwo(int row, int col, CellState color)
        {
            return EstimateForStageTwo(new Point(row, col), color);
        }

        internal IComparable EstimateForStageThree(int row, int col, CellState color)
        {

            return EstimateForStageThree(new Point(row, col), color);
        }
    }
}
