using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Gomoku.ViewModels
{
    public class GomokuOnline : GomokuGame
    {
        public GomokuOnline() : base()
        {

        }

        public GomokuOnline(int gameSize) : base(gameSize)
        {

        }
        public override void OnlineUserPlayPlayAt(Canvas chessBoard, int row, int col)
        {
            throw new NotImplementedException();
        }

    }
}
