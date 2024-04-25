﻿using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace VirusGame
{
    public partial class MainWindow : Window
    {
        private List<Button> Cells = new List<Button>();
        private List<string> Players = new List<string>();
        private int currentPlayerIndex;
        private int cellLimit = 3;

        private string[] playerColors = { "Blue", "Red", "Green", "Yellow" };
        private int[] corners = { 0, 99, 90, 9 };

        private bool gameStarted = false;

        public MainWindow()
        {
            InitializeComponent();
            InitializeGrid();
        }

        private void newGameButton_Click(object sender, RoutedEventArgs e)
        {
            Players.Clear();

            foreach (Button cell in Cells)
            {
                cell.Background = Brushes.LightGray;
                cell.IsEnabled = true;
                cell.FontSize = 14;
                cell.Content = string.Empty;
            }

            for (int i = 0; i <= playerComboBox.SelectedIndex + 1; i++)
            {
                Players.Add(playerColors[i]);
                Button startCell = Cells[corners[i]];
                startCell.Background = new BrushConverter().ConvertFromString(playerColors[i]) as SolidColorBrush;
            }
            currentPlayerIndex = 0;
            cellLimit = 3;
            turnLabel.Content = "Turn: " + Players[currentPlayerIndex];
            gameStarted = true;
        }

        private void InitializeGrid()
        {
            for (int row = 0; row < 10; row++)
            {
                for (int col = 0; col < 10; col++)
                {
                    Button newCell = new Button();
                    newCell.Width = 50;
                    newCell.Height = 50;
                    newCell.Background = Brushes.LightGray;
                    newCell.Click += (sender, e) => Cell_Click(sender);

                    Grid.SetRow(newCell, row);
                    Grid.SetColumn(newCell, col);

                    CellGrid.Children.Add(newCell);
                    Cells.Add(newCell);
                }
            }
        }

        private void Cell_Click(object sender)
        {
            int cellIndex = Cells.IndexOf((Button)sender);
            Button clickedCell = Cells[cellIndex];

            if (!gameStarted)
            {
                return;
            }

            if (isSurrounded(Players[currentPlayerIndex]))
            {
                skipTurn();

                if (Players.Count == 1)
                {
                    MessageBox.Show(Players[0] + " Wins!");
                    Players.Clear();
                    InitializeGrid();
                    gameStarted = false;
                    return;
                }
            }

            if (clickedCell.Background.ToString() == new BrushConverter().ConvertFromString(Players[currentPlayerIndex]).ToString())
            {
                return;
            }

            if (isAdjacentColor(cellIndex))
            {
                if (clickedCell.Background != Brushes.LightGray)
                {
                    clickedCell.IsEnabled = false;
                    clickedCell.Content = "♜";
                    clickedCell.FontSize = 36;
                    clickedCell.Foreground = new BrushConverter().ConvertFromString(Players[currentPlayerIndex]) as SolidColorBrush;
                }
                clickedCell.Background = new BrushConverter().ConvertFromString(Players[currentPlayerIndex]) as SolidColorBrush;
                cellLimit--;

                if (cellLimit == 0)
                {
                    skipTurn();
                }
            }
        }

        private bool isAdjacentColor(int index)
        {
            List<int> adjacentCells = new List<int>();
            if (index % 10 > 0) adjacentCells.Add(index - 1); // Left
            if (index % 10 < 9) adjacentCells.Add(index + 1); // Right
            if (index / 10 > 0) adjacentCells.Add(index - 10); // Above
            if (index / 10 < 9) adjacentCells.Add(index + 10); // Below

            foreach (int cellIndex in adjacentCells)
            {
                if (Cells[cellIndex].Background.ToString() == new BrushConverter().ConvertFromString(Players[currentPlayerIndex]).ToString())
                {
                    return true;
                }
            }

            return false;
        }

        private bool isSurrounded(string playerColor)
        {
            SolidColorBrush playerBrush = new BrushConverter().ConvertFromString(Players[currentPlayerIndex]) as SolidColorBrush;

            for (int i = 0; i < Cells.Count; i++) {
                if (Cells[i].Background == playerBrush) {
                    List<int> adjacentCells = new List<int>();
                    if (i % 10 > 0) adjacentCells.Add(i - 1); // Left
                    if (i % 10 < 9) adjacentCells.Add(i + 1); // Right
                    if (i / 10 > 0) adjacentCells.Add(i - 10); // Above
                    if (i / 10 < 9) adjacentCells.Add(i + 10); // Below

                    foreach (int cellIndex in adjacentCells)
                    {
                        if (Cells[cellIndex].Background == Brushes.LightGray)
                        {
                            return false;
                        }
                    }
                }      
            }

            return true;
        }

        private void skipTurnButton_Click(object sender, RoutedEventArgs e)
        {
            skipTurn();
        }

        private void skipTurn()
        {
            currentPlayerIndex = (currentPlayerIndex + 1) % Players.Count;
            cellLimit = 3;
            turnLabel.Content = "Turn: " + Players[currentPlayerIndex];
        }
    }
}
