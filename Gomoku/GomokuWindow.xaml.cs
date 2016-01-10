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
using System.ComponentModel;
using System.Threading;

namespace Gomoku
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class GomokuWindow : Window
    {
        //GomokuOffline gomokuOffline;
        GomokuGame gomoku;
        Machine AI;
        Point AIPos;

        public GomokuWindow()
        {
            InitializeComponent();
        }

        private void OnPlayerWin(CellState player)
        {
            string winner = "";
            if (cbGameMode.SelectedIndex == 2)
                winner = gomoku.activePlayer.ToString();

            if (cbGameMode.SelectedIndex == 3)
            {
                if (gomoku.activePlayer == CellState.black)
                    winner = "Congratulation you";
                else
                    winner = "Too bad! Machine";
            }

            if(cbGameMode.SelectedIndex == 0 || cbGameMode.SelectedIndex == 1)
            {
                if (gomoku.activePlayer == CellState.black)
                    winner = "Congratulation you";
                else
                    winner = "Too bad! Machine";
            }
            MessageBox.Show(winner.ToString().ToUpper() + " WIN!!!" + "\n" + "Press new game to start a new game.", "Game Over",
                               MessageBoxButton.OK, MessageBoxImage.Asterisk);

            cvChessBoard.IsEnabled = false;
            
        }

        private void cvChessBoard_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
           // if(gomoku.activePlayer == CellState.black)
                gomoku.PlayAt(cvChessBoard, e.GetPosition(cvChessBoard));
            
        }

        private void chessBoard_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if(cvChessBoard.Children.Count !=0)
                gomoku.UpdateChessBoard(cvChessBoard);
        }

        private void cvChessBoard_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (cbGameMode.SelectedIndex == 3)
                AICalculateNextPoint();
        }

        private void NewGame(int mode)
        {
            cvChessBoard.Children.Clear();
            cvChessBoard.IsEnabled = true;
            switch (mode)
            {
                //online
                case 0: //Player vs player
                   // gomoku = new GomokuOnline();
                    
                    break;
                case 1: //Machine vs player Auto play
                    break;

                //offline
                case 2: //Player vs player
                    gomoku = new GomokuGame();
                    gomoku.DrawChessBoard(cvChessBoard);
                    gomoku.OnPlayerWin += OnPlayerWin;
                    break;
                case 3: //Player vs machine
                    gomoku = new GomokuGame();
                    gomoku.DrawChessBoard(cvChessBoard);
                    gomoku.OnPlayerWin += OnPlayerWin;
                    AI = new SimpleMachine(gomoku.board, gomoku.gameSize);
                    break;
                default:
                    break;
            }

        }

        private void btnNewGame_Click(object sender, RoutedEventArgs e)
        {
            NewGame(cbGameMode.SelectedIndex);
        }

        private void btnSendMessage_Click(object sender, RoutedEventArgs e)
        {
            if (cbGameMode.SelectedIndex == 2 || cbGameMode.SelectedIndex == 3)
            {
                TextBlock mess = new TextBlock();
                string date = "# <" + DateTime.Now.ToString("hh:mm:ss") + "> ";
                string user = "Guest:\n";
                mess.Text = date + user + tbMessage.Text + "\n";   //"\n-------------------------------------";
                mess.TextWrapping = TextWrapping.Wrap;
                spChatBox.Children.Add(mess);
                scrvChatBox.ScrollToEnd();
            }
        }


        #region Background Worker: Calculate next step
        private void AICalculateNextPoint()
        {
            BackgroundWorker worker = new BackgroundWorker();
            worker.WorkerReportsProgress = true;
            worker.DoWork += Worker_DoWork;
            worker.ProgressChanged += Worker_ProgressChanged;
            worker.RunWorkerCompleted += Worker_RunWorkerCompleted;
            worker.RunWorkerAsync();
        }

        private void Worker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            gomoku.PlayAt(cvChessBoard, (int)AIPos.X, (int)AIPos.Y);
        }

        private void Worker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            //Thread.Sleep(10000);
            AIPos = AI.SelectBestCell(gomoku.activePlayer);
        }

        #endregion


    }
}
