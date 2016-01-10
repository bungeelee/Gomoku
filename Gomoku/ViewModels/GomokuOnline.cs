using Quobject.SocketIoClientDotNet.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Configuration;
using Newtonsoft.Json.Linq;
using System.Windows;
using Gomoku.Models;

namespace Gomoku.ViewModels
{
    public class GomokuOnline : GomokuGame
    {
        private Socket socket;
        public string playerName { get; set; }
        private GomokuGame gomoku;

        public GomokuOnline()
        {
            playerName = "Guest";
        }

        public GomokuOnline(string name)
        {
            playerName = name;
        }
        public void StartGame(StackPanel chatBox, Canvas chessBoard)
        {
            var server = ConfigurationManager.ConnectionStrings["serverIP"].ConnectionString;
            socket = IO.Socket(server);
            TextBlock chat = new TextBlock();
            chat.TextWrapping = System.Windows.TextWrapping.Wrap;
            gomoku = new GomokuGame();
            string message = null;
            socket.On(Socket.EVENT_CONNECT, () =>
            {

            });

            socket.On(Socket.EVENT_MESSAGE, (data) =>
            {

            });

            socket.On(Socket.EVENT_CONNECT_ERROR, (data) =>
            {
                message = data.ToString();
            });
            chat.Text = message;
            chatBox.Children.Add(chat);

            string fromA = null;
            string mess = null;
            socket.On("ChatMessage", (data) =>
            {
                if(((JObject)data)["message"].ToString() == "Wellcome!")
                {
                    socket.Emit("MyNameIs", playerName);
                    socket.Emit("ConnectToOtherPlayer");
                }

                var dt = JObject.Parse(data.ToString());
                fromA = (string)dt["from"];
                if (fromA == null)
                    fromA = "Server";
                mess = (string)dt["message"];
            });
            chat.Text = fromA + ": " + mess;
            chatBox.Children.Add(chat);
            socket.On(Socket.EVENT_ERROR, (data) =>
            {

            });

            Point nextStep = new Point();
            socket.On("NextStepIs", (data) =>
            {
                var o = JObject.Parse(data.ToString());
                nextStep.X = (double)o["row"];
                nextStep.Y = (double)o["col"];

                if ((int)o["player"] == 1)
                {
                    gomoku.activePlayer = CellState.red;
                    gomoku.PlayAt(chessBoard, (int)o["row"], (int)o["col"]);
                }

                if ((int)o["player"] == 0)
                {
                    gomoku.activePlayer = CellState.black;
                    gomoku.PlayAt(chessBoard, (int)o["row"], (int)o["col"]);
                }
            });

        }

        public void MyStepIs(Canvas chessBoard, Point pos)
        {
            double x = chessBoard.ActualWidth / gomoku.gameSize;
            double y = chessBoard.ActualHeight / gomoku.gameSize;
            Point myStep = new Point();
            socket.Emit("MyStepIs", JObject.FromObject(new { row = (int)myStep.X, col = (int)myStep.Y }));
        }

        public void SendMessage(string mess)
        {
            socket.Emit("ChatMessage", mess);
        }

        public void ChangeName(string name)
        {
            socket.Emit("MyNameIs", name);
        }


    }
}
