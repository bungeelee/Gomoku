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
        bool isGameEnded = false;
        bool isValidStep = false;


        public GomokuWindow()
        {
            InitializeComponent();
        }

        private void OnPlayerWin(CellState player)
        {
            isGameEnded = true;
            string winner = "";
            string isWin = " win!!!";
            if (cbGameMode.SelectedIndex == 2)
                winner = gomoku.activePlayer.ToString();

            if (cbGameMode.SelectedIndex == 3)
            {
                if (gomoku.activePlayer == CellState.black)
                    winner = "Congratulation you";
                else
                    winner = "Sorry! Machine";
            }

            if (cbGameMode.SelectedIndex == 0 || cbGameMode.SelectedIndex == 1)
            {
                if (gomoku.activePlayer == CellState.black)
                    winner = "Congratulation you";
                else
                {
                    winner = "Sorry! you";
                    isWin = " lose!!!";
                }
            }
            MessageBox.Show(winner.ToString() + isWin + "\n" + "Press new game to start a new game.", "Game Over",
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
                {
                    {
                        var myStep = new Point();
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
                case 0:
                    NewOnlineGame();
                    break;
                case 1: //Machine vs player Auto play
                    isGameEnded = false;
                    cvChessBoard.IsEnabled = false;
                    NewOnlineGame();
                    //AI = new SimpleMachine(gomoku.board, 12);
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
            }
            else
                if (gomoku != null)
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
            //throw new NotImplementedException();
            if (cbGameMode.SelectedIndex == 3)
                gomoku.PlayAt(cvChessBoard, (int)AIPos.X, (int)AIPos.Y);
            else
                return;
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
            //Get ServerIP
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
                    chat.Text = ">>>>>>>>>>>> Conneted. Start New Game <<<<<<<<<<<<\n";
                    spChatBox.Children.Add(chat);
                }));
            });

            socket.On(Socket.EVENT_MESSAGE, (data) =>
            {
                this.Dispatcher.Invoke((Action)(() =>
                {
                    ProgressMessage(JObject.Parse(data.ToString()));
                }));
            });

            socket.On(Socket.EVENT_CONNECT_ERROR, (data) =>
            {
                this.Dispatcher.Invoke((Action)(() =>
                {
                    TextBlock error = new TextBlock();
                    error.Text = "Connec error";
                    spChatBox.Children.Add(error);
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
                    ProgressMessage(JObject.Parse(data.ToString()));
                }));
            });

            socket.On(Socket.EVENT_ERROR, (data) =>
            {
                this.Dispatcher.Invoke((Action)(() =>
                {
                    TextBlock error = new TextBlock();
                    error.Text = "Error";
                    spChatBox.Children.Add(error);
                    //AddMessageToChatbox(JObject.Parse(data.ToString()));
                }));
            });


            socket.On("NextStepIs", (data) =>
            {
                this.Dispatcher.Invoke((Action)(() =>
                {
                    var o = JObject.Parse(data.ToString());
                    //Client turn
                    if ((int)o["player"] == 1)
                    {
                        gomoku.activePlayer = CellState.red;
                        gomoku.PlayAt(cvChessBoard, (int)o["row"], (int)o["col"]);
                        AI = new SimpleMachine(gomoku.board, 12);
                        AICalculateNextPoint();
                        if (cbGameMode.SelectedIndex == 1 && isGameEnded == false)
                        {
                            socket.Emit("MyStepIs", JObject.FromObject(new { row = (int)AIPos.X, col = (int)AIPos.Y }));
                        }
                    }
                    //My turn
                    if ((int)o["player"] == 0)
                    {
                        gomoku.activePlayer = CellState.black;
                        if (cbGameMode.SelectedIndex != 1)
                        {
                            AI = new SimpleMachine(gomoku.board, 12);
                            AICalculateNextPoint();
                        }
                        gomoku.PlayAt(cvChessBoard, (int)o["row"], (int)o["col"]);
                    }
                }));
            });

            socket.On("EndGame", (data) =>
            {
                this.Dispatcher.Invoke((Action)(() =>
                {
                    ProgressMessage(JObject.Parse(data.ToString()));
                    TextBlock chat = new TextBlock();
                    chat.TextWrapping = TextWrapping.Wrap;
                    chat.Text = ">>>>>>>> Game over. Press New Game to start new game\n";
                    spChatBox.Children.Add(chat);
                }));
            });
        }

        private void ProgressMessage(JObject data)
        {
            string name = null;
            string mess = null;
            try
            {
                name = data["from"].ToString();
            }
            catch
            {

            }
            finally
            {
                if (name == null)
                    name = "Server";
                mess = data["message"].ToString();
                int indexbr = mess.IndexOf("<br />");
                if (indexbr != -1)
                    mess = mess.Insert(indexbr + 6, " ").Remove(indexbr, 6);
            }

            #region Random step for Machine vs Player in Online mode
            if (mess.IndexOf("You are the first player!") != -1 && cbGameMode.SelectedIndex == 1)
            {
                DumbMachine a = new DumbMachine();
                socket.Emit("MyStepIs", JObject.FromObject(new { row = a.RanColOrRow(), col = a.RanColOrRow() }));
            }
            isValidStep = true;
            if (mess == "Invalid step." && cbGameMode.SelectedIndex == 1 && isGameEnded == false)
            {
                AI = new SimpleMachine(gomoku.board, 12);
                AICalculateNextPoint();
                socket.Emit("MyStepIs", JObject.FromObject(new { row = (int)AIPos.X, col = (int)AIPos.Y }));
            }

            #endregion

            TextBlock chat = new TextBlock();
            chat.TextWrapping = TextWrapping.Wrap;
            string date = "# <" + DateTime.Now.ToString("hh:mm:ss tt") + "> ";
            chat.Text = date + name + ":\n>>> " + mess + "\n";
            spChatBox.Children.Add(chat);
            scrvChatBox.ScrollToEnd();
        }

        private void tbMessage_MouseDown(object sender, MouseButtonEventArgs e)
        {
            tbMessage.Text = "";
        }


    }
}
