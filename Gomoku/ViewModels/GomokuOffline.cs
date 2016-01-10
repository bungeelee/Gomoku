using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Gomoku.ViewModels
{
    public class GomokuOffline : GomokuGame
    {
        public GomokuOffline() :base()
        {
        }

        public GomokuOffline(int gameSize) : base (gameSize)
        {

        }

        public override void OnlineUserPlayPlayAt(Canvas chessBoard, int row, int col)
        {
            throw new NotImplementedException();
        }
    }
}
