using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Gomoku
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class GomokuWindow : Window
    {
        public GomokuWindow()
        {
            InitializeComponent();
        }

        private void cvChessBoard_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show(cvChessBoard.ActualHeight.ToString() + " " + cvChessBoard.ActualWidth.ToString());
        }

        private void chessBoard_SizeChanged(object sender, SizeChangedEventArgs e)
        {

        }
    }
}
