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
using Gomoku.Models;
using Gomoku.ViewModels;

namespace Gomoku
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class GomokuWindow : Window
    {
        //GomokuOffline gomokuOffline;
        GomokuGame gomoku;

        public GomokuWindow()
        {
            InitializeComponent();
            NewGame(cbGameMode.SelectedIndex);
        }

        private void OnPlayerWin(CellState player)
        {
            MessageBox.Show(player.ToString() + " win !");
            //throw new NotImplementedException();
        }

        private void cvChessBoard_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            gomoku.PlayAt(cvChessBoard, e.GetPosition(cvChessBoard));
        }

        private void chessBoard_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if(cvChessBoard.Children.Count !=0)
                //gomokuOffline.UpdateChessBoard(cvChessBoard);
                gomoku.UpdateChessBoard(cvChessBoard);
        }

        private void NewGame(int mode)
        {
            switch (mode)
            {
                //online
                case 0: //Player vs player

                    break;
                case 1: //Player vs machine
                    break;

                //offline
                case 2: //Player vs player
                    gomoku = new GomokuOffline();
                    gomoku.DrawChessBoard(cvChessBoard);
                    gomoku.OnPlayerWin += OnPlayerWin;
                    break;
                case 3: //Machine vs player
                    break;
                default:
                    break;
            }

        }

        private void btnNewGame_Click(object sender, RoutedEventArgs e)
        {
            NewGame(cbGameMode.SelectedIndex);
        }
    }
}
