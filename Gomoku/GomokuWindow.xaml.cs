﻿using System;
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
        GomokuOffline gomoku;
        public GomokuWindow()
        {
            InitializeComponent();
            gomoku = new GomokuOffline();
            gomoku.DrawChessBoard(cvChessBoard);
            gomoku.OnPlayerWin += OnPlayerWin;
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
                gomoku.UpdateChessBoard(cvChessBoard);
        }


    }
}
