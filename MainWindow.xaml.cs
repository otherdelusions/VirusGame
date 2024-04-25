using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace VirusGame
{
    public partial class MainWindow : Window
    {
        private List<Button> Cells = new List<Button>();
        private List<string> Players = new List<string>();
        private List<string> DeadPlayers = new List<string>();
        private int currentPlayerIndex;
        SolidColorBrush playerBrush;
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
                cell.Content = string.Empty;
            }

            for (int i = 0; i <= playerComboBox.SelectedIndex + 1; i++)
            {
                Players.Add(playerColors[i]);
                Button startCell = Cells[corners[i]];
                startCell.Background = new BrushConverter().ConvertFromString(playerColors[i]) as SolidColorBrush;
            }

            skipTurn(true);
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
                    newCell.FontSize = 36;
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
                MessageBox.Show(Players[currentPlayerIndex] + " Was Surrounded!");
                DeadPlayers.Add(Players[currentPlayerIndex]);
                skipTurn();
                return;
            }

            if (clickedCell.Background == playerBrush)
            {
                return;
            }

            if (isAdjacentColor(cellIndex))
            {
                if (clickedCell.Background != Brushes.LightGray)
                {
                    clickedCell.IsEnabled = false;
                    clickedCell.Content = "♜";
                    clickedCell.Foreground = playerBrush;
                }
                clickedCell.Background = playerBrush;
                cellLimit--;

                if (cellLimit == 0)
                {
                    skipTurn();
                }
            }
        }

        private List<int> adjacentCells = new List<int>();
        private void updateAdjacent(int index)
        {
            adjacentCells.Clear();
            if (index % 10 > 0) adjacentCells.Add(index - 1); // Left
            if (index % 10 < 9) adjacentCells.Add(index + 1); // Right
            if (index / 10 > 0) adjacentCells.Add(index - 10); // Above
            if (index / 10 < 9) adjacentCells.Add(index + 10); // Below
        }
                    

        private bool isAdjacentColor(int index)
        {
            updateAdjacent(index);
            foreach (int cellIndex in adjacentCells)
            {
                if (Cells[cellIndex].Background == playerBrush)
                {
                    return true;
                }
            }

            return false;
        }

        private bool isSurrounded(string playerColor)
        {
            for (int i = 0; i < Cells.Count; i++) {
                if (Cells[i].Background == playerBrush) {
                    updateAdjacent(i);
                    foreach (int cellIndex in adjacentCells)
                    {
                        if (Cells[cellIndex].Background == Brushes.LightGray || (Cells[cellIndex].Background != playerBrush && Cells[cellIndex].IsEnabled == true))
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

        private void skipTurn(bool newGame = false)
        {
            if (newGame) { currentPlayerIndex = 0; } else
            { currentPlayerIndex = (currentPlayerIndex + 1) % Players.Count; }

            cellLimit = 3;
            turnLabel.Content = "Turn: " + Players[currentPlayerIndex];
            playerBrush = new BrushConverter().ConvertFromString(Players[currentPlayerIndex]) as SolidColorBrush;

            if (DeadPlayers.Count == Players.Count - 1)
            {
                MessageBox.Show("Game Over!");
                gameStarted = false;
                return;
            }

            if (DeadPlayers.Contains(Players[currentPlayerIndex]))
            {
                skipTurn();
                return;
            }
        }
    }
}
