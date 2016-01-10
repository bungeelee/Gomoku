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
using Quobject.SocketIoClientDotNet.Client;
using Newtonsoft.Json.Linq;
using System.Configuration;

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
        Socket socket;
        Point myStep;

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

            if (cbGameMode.SelectedIndex == 0 || cbGameMode.SelectedIndex == 1)
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
            if (cbGameMode.SelectedIndex == 2 || cbGameMode.SelectedIndex == 3)
                gomoku.PlayAt(cvChessBoard, e.GetPosition(cvChessBoard));
            else
            {
                if (cbGameMode.SelectedIndex == 0)
                {// online.MyStepIs(cvChessBoard, e.GetPosition(cvChessBoard));
                    {
                        myStep = new Point();
                        double x = cvChessBoard.ActualWidth / 12;
                        double y = cvChessBoard.ActualHeight / 12;

                        if (cbGameMode.SelectedIndex == 0)
                        {
                            myStep.X = e.GetPosition(cvChessBoard).X / x;
                            myStep.Y = e.GetPosition(cvChessBoard).Y / y;
                        }
                        //mouseDownded = true;
                        socket.Emit("MyStepIs", JObject.FromObject(new { row = (int)myStep.X, col = (int)myStep.Y }));


                    }
                }
             }

        }

        private void chessBoard_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (cvChessBoard.Children.Count != 0)
            {
                if (gomoku != null)
                    gomoku.UpdateChessBoard(cvChessBoard);
                
                
            }
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
                case 0:
                    NewOnlineGame();
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
            spChatBox.Children.Clear();
            NewGame(cbGameMode.SelectedIndex);
        }

        private void btnSendMessage_Click(object sender, RoutedEventArgs e)
        {
            if (cbGameMode.SelectedIndex == 2 || cbGameMode.SelectedIndex == 3)
            {
                TextBlock mess = new TextBlock();
                string date = "# <" + DateTime.Now.ToString("hh:mm:ss") + "> ";
                string user = "Guest:\n";
                mess.Text = date + user + tbMessage.Text + "\n"; 
                mess.TextWrapping = TextWrapping.Wrap;
                spChatBox.Children.Add(mess);
                scrvChatBox.ScrollToEnd();
            }
            else
                socket.Emit("ChatMessage", tbMessage.Text);
            scrvChatBox.ScrollToEnd();
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

        private void btnChangeName_Click(object sender, RoutedEventArgs e)
        {
            if (cbGameMode.SelectedIndex == 0 || cbGameMode.SelectedIndex == 1)
                if (socket != null)
                    socket.Emit("MyNameIs", tbName.Text);
        }

        //Online 
        private void NewOnlineGame()
        {
            var server = ConfigurationManager.ConnectionStrings["serverIP"].ConnectionString;
            socket = IO.Socket(server);
            gomoku = new GomokuGame();
            gomoku.DrawChessBoard(cvChessBoard);
            gomoku.OnPlayerWin += OnPlayerWin;

            socket.On(Socket.EVENT_CONNECT, () =>
            {
                this.Dispatcher.Invoke((Action)(() =>
                {
                    TextBlock chat = new TextBlock();
                    chat.Text = "Conneted";
                    spChatBox.Children.Add(chat);
                }));
            });

            socket.On(Socket.EVENT_MESSAGE, (data) =>
            {
                this.Dispatcher.Invoke((Action)(() =>
                {
                    AddMessageToChatbox(JObject.Parse(data.ToString()));
                }));
            });

            socket.On(Socket.EVENT_CONNECT_ERROR, (data) =>
            {
                this.Dispatcher.Invoke((Action)(() =>
                {
                    AddMessageToChatbox(JObject.Parse(data.ToString()));
                }));
            });

            socket.On("ChatMessage", (data) =>
            {
                this.Dispatcher.Invoke((Action)(() =>
                {
                    if (((JObject)data)["message"].ToString() == "Welcome!")
                    {
                        socket.Emit("MyNameIs", tbName.Text);
                        socket.Emit("ConnectToOtherPlayer");
                    }
                    AddMessageToChatbox(JObject.Parse(data.ToString()));
                }));
            });

            socket.On(Socket.EVENT_ERROR, (data) =>
            {
                this.Dispatcher.Invoke((Action)(() =>
                {
                    AddMessageToChatbox(JObject.Parse(data.ToString()));
                }));
            });

            socket.On("NextStepIs", (data) =>
            {
                this.Dispatcher.Invoke((Action)(() =>
                {
                    var o = JObject.Parse(data.ToString());
                    //player2.X = (double)o["row"];
                    //player2.Y = (double)o["col"];

                    if ((int)o["player"] == 1)
                    {
                        gomoku.activePlayer = CellState.red;
                        gomoku.PlayAt(cvChessBoard, (int)o["row"], (int)o["col"]);
                    }

                    if ((int)o["player"] == 0)
                    {
                        gomoku.activePlayer = CellState.black;
                        gomoku.PlayAt(cvChessBoard, (int)o["row"], (int)o["col"]);
                    }
                }));
            });
            scrvChatBox.ScrollToEnd();
        }

        private void AddMessageToChatbox(JObject data)
        {
            string name = null;
            string mess = null;
            if (data.Children().Count() == 1)
            {
                name = "Server";
                mess = data["message"].ToString();
            }
            else
            {
                name = data["from"].ToString();
                mess = data["message"].ToString();
            }

            TextBlock chat = new TextBlock();
            chat.TextWrapping = TextWrapping.Wrap;
            string date = "# <" + DateTime.Now.ToString("hh:mm:ss") + "> ";
            chat.Text = date + name + ":\n>> " + mess + "\n";
            spChatBox.Children.Add(chat);
            scrvChatBox.ScrollToEnd();
        }
    }
}
