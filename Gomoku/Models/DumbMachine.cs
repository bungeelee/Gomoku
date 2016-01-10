using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Gomoku.Models
{
    public class DumbMachine
    {
        public Point NextStep()
        {
            Random ran = new Random();
            int x = ran.Next(0, 11);
            int y = ran.Next(0, 11);
            return new Point(x, y);
        } 

        public int RanColOrRow()
        {
            Random ran = new Random();
            return ran.Next(0, 11);
        }
        
    }
}
