using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Boardgame;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    private List<Player> players;
    private Random random;
    private int currentPlayerIndex;

    public MainWindow()
    {
        InitializeComponent();

        players = new List<Player>
            {
                new Player("Player 1", 0),      //Set start to field 0
                new Player("Player 2", 9),      //Set start to field 9
                new Player("Player 3", 18),     //Set start to field 18
                new Player("Player 4", 27)      //Set start to field 27
            };

        random = new Random();
        currentPlayerIndex = 0;

        UpdateScores();
        UpdateCurrentPlayer();
    }

    private async void RollButton_Click(object sender, RoutedEventArgs e)
    {
        Player currentPlayer = players[currentPlayerIndex];

        int roll = random.Next(1, 7);
        diceNumberDisplay.Content = $"Dice Number: {roll}";
        SetDiceImage(roll);

        int previousPosition = currentPlayer.Position;
        currentPlayer.MoveToPosition(currentPlayer.Position + roll);

        //Player passed entire board
        if (currentPlayer.Position >= 36)
        {
            currentPlayer.Position %= 36;
        }
        //If a Player1 passes a loop (board size is 36)
        if (currentPlayer == players[0])
        {
            //Check if Player1 has passed Position 0
            if (previousPosition >= 30 && currentPlayer.Position <= 5)
            {
                currentPlayer.Score += 1; //Player1 gets +1 point for passing Position 0
            }
        }
        //If a Player2 passes a loop
        if (currentPlayer == players[1])
        {
            //Check if Player 2 has passed Position 9
            if (previousPosition < 9 && currentPlayer.Position >= 9)
            {
                currentPlayer.Score += 1; // Player2 gets +1 point for passing Position 9
            }
        }
        //If a Player3 passes a loop
        if (currentPlayer == players[2])
        {
            //Check if Player3 has passed Position 18
            if (previousPosition < 18 && currentPlayer.Position >= 18)
            {
                currentPlayer.Score += 1; // Player3 gets +1 point for passing Position 18
            }
        }
        //If a Player4 passes a loop
        if (currentPlayer == players[3])
        {
            //Check if Player4 has passed Position 27
            if (previousPosition < 27 && currentPlayer.Position >= 27)
            {
                currentPlayer.Score += 1; // Player4 gets +1 point for passing Position 27
            }
        }

        //Check if the current player landed on another players start position
        for (int i = 0; i < players.Count; i++)
        {
            //Check the current players start position
            if (i != currentPlayerIndex && currentPlayer.Position == players[i].StartingPosition)
            {
                //If the current player is landing on another players start position
                if (currentPlayer.Position != currentPlayer.StartingPosition)
                {
                    //Check if the player owning the start position is already there
                    if (players[i].Position == players[i].StartingPosition)
                    {
                        //Current player is sent back to their start and decrease score
                        currentPlayer.Position = currentPlayer.StartingPosition;
                        currentPlayer.Score -= 1;

                        //The player who owns that start position gets a point
                        players[i].Score += 1;
                    }
                }
            }
        }
        
        //Check if the player lands on position with another player and reset other players position if needed
        for (int i = 0; i < players.Count; i++)
        {
            if (i != currentPlayerIndex && players[i].Position == currentPlayer.Position)
            {
                Player otherPlayer = players[i];

                // If the other player is not at their starting position
                if (otherPlayer.Position != otherPlayer.StartingPosition)
                {
                    // The player who was already on the position (other player) is sent back to their starting position
                    otherPlayer.Position = otherPlayer.StartingPosition;
                    otherPlayer.Score -= 1;

                    // Current player gains 1 point for landing on the other player's position
                    currentPlayer.Score += 1;
                }
            }
        }
        
        UpdateBoard();
        UpdateScores();
        UpdateCurrentPlayer();
        UpdateCurrentPositions();

        if (currentPlayer.Score >= 10)
        {

            MessageBox.Show($"{currentPlayer.Name} has won the game with {currentPlayer.Score} points!", "Game Over", MessageBoxButton.OK, MessageBoxImage.Information);

            RollButton.IsEnabled = false;
        }

        // If rolled a 6, the player gets another turn
        if (roll == 6)
        {
            return;
        }

        //Wait half a second untill another player turn (FOR TESTING PURPOSES AND AUTOMATIC PLAYING)
        //await Task.Delay(500);

        // Move to the next player
        currentPlayerIndex = (currentPlayerIndex + 1) % players.Count;
    }



    private void UpdateBoard()
    {
        for (int i = 0; i < players.Count; i++)
        {
            var player = players[i];

            var spaceButtonPrevious = (Button)this.BoardCanvas.Children[player.PreviousPosition];
            var spaceButtonCurrent = (Button)this.BoardCanvas.Children[player.Position];
            var spaceButtonStart = (Button)this.BoardCanvas.Children[player.StartingPosition];

            if (spaceButtonPrevious is Button previousButton)
            {
                previousButton.Background = Brushes.LightGray;
            }

            if (spaceButtonCurrent is Button currentButton && spaceButtonCurrent != spaceButtonPrevious)
            {
                currentButton.Background = null;
            }

            switch (i)
            {
                case 0:
                    if (spaceButtonCurrent is Button buttonPlayer1)
                        buttonPlayer1.Background = Brushes.Red;   //Player1 (Red)
                    break;
                case 1:
                    if (spaceButtonCurrent is Button buttonPlayer2)
                        buttonPlayer2.Background = Brushes.Blue;  //Player2 (Blue)
                    break;
                case 2:
                    if (spaceButtonCurrent is Button buttonPlayer3)
                        buttonPlayer3.Background = Brushes.Yellow; //Player3 (Yellow)
                    break;
                case 3:
                    if (spaceButtonCurrent is Button buttonPlayer4)
                        buttonPlayer4.Background = Brushes.Purple; //Player4 (Purple)
                    break;
                default:
                    if (spaceButtonCurrent is Button buttonDefault)
                        buttonDefault.Background = Brushes.Gray;   //Default
                    break;
            }

            //Highlight the starting position
            switch (i)
            {
                case 0:
                    if (spaceButtonStart is Button startButtonPlayer1)
                        startButtonPlayer1.Background = Brushes.LightPink;   //Player1 Starting Position (Light Pink)
                    break;
                case 1:
                    if (spaceButtonStart is Button startButtonPlayer2)
                        startButtonPlayer2.Background = Brushes.LightSkyBlue;  //Player2 Starting Position (Light Blue)
                    break;
                case 2:
                    if (spaceButtonStart is Button startButtonPlayer3)
                        startButtonPlayer3.Background = Brushes.LightGoldenrodYellow; //Player3 Starting Position (Light Yellow)
                    break;
                case 3:
                    if (spaceButtonStart is Button startButtonPlayer4)
                        startButtonPlayer4.Background = Brushes.Plum; //Player4 Starting Position (Plum)
                    break;
                default:
                    if (spaceButtonStart is Button startButtonDefault)
                        startButtonDefault.Background = Brushes.LightGray;   //Default starting color
                    break;
            }

            // If the player is on another players start position highlight it with that players color
            if (spaceButtonCurrent != spaceButtonStart && spaceButtonCurrent.Background == null)
            {
                switch (i)
                {
                    case 0:
                        if (spaceButtonCurrent is Button button1)
                            button1.Background = Brushes.Red;   //Player1 (Red)
                        break;
                    case 1:
                        if (spaceButtonCurrent is Button button2)
                            button2.Background = Brushes.Blue;  //Player2 (Blue)
                        break;
                    case 2:
                        if (spaceButtonCurrent is Button button3)
                            button3.Background = Brushes.Yellow; //Player3 (Yellow)
                        break;
                    case 3:
                        if (spaceButtonCurrent is Button button4)
                            button4.Background = Brushes.Purple; //Player4 (Purple)
                        break;
                }
            }


        }
    }

    private void UpdateScores()
    {
        Player1Score.Content = $"Player 1: {players[0].Score}";
        Player2Score.Content = $"Player 2: {players[1].Score}";
        Player3Score.Content = $"Player 3: {players[2].Score}";
        Player4Score.Content = $"Player 4: {players[3].Score}";
    }

    private void UpdateCurrentPositions()
    {
        player1CurrentPosition.Content = $"Player 1 Current Position: {players[0].Position}";
        player2CurrentPosition.Content = $"Player 2 Current Position: {players[1].Position}";
        player3CurrentPosition.Content = $"Player 3 Current Position: {players[2].Position}";
        player4CurrentPosition.Content = $"Player 4 Current Position: {players[3].Position}";
    }

    private void UpdateCurrentPlayer()
    {
        CurrentPlayerLabel.Content = $"{players[currentPlayerIndex].Name}'s Turn";

        if(currentPlayerIndex == 0)
        {
            CurrentPlayerLabel.Foreground = Brushes.Red;
        }
        else if(currentPlayerIndex == 1)
        {
            CurrentPlayerLabel.Foreground = Brushes.Blue;
        }
        else if(currentPlayerIndex == 2)
        {
            CurrentPlayerLabel.Foreground = Brushes.Yellow;
        }
        else if(currentPlayerIndex == 3)
        {
            CurrentPlayerLabel.Foreground = Brushes.Purple;
        }

            for (int i = 0; i < players.Count; i++)
            {
                var playerLabel = (Label)this.FindName($"Player{i + 1}Score");
                if (i == currentPlayerIndex)
                {
                    playerLabel.Foreground = Brushes.Blue;
                }
                else
                {
                    playerLabel.Foreground = Brushes.Black;
                }
            }
    }

    private void SetDiceImage(int roll)
    {
        string imagePath = $"images/diceRoll{roll}.png";

        diceImage.Source = new BitmapImage(new Uri(imagePath, UriKind.Relative));
    }
}