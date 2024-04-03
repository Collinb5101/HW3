using UnityEngine;
using RPS;

/// <summary>
/// This class receives input from the player, queries the AI for predictions, and updates the total wins/losses.
/// </summary>
public class PlayerInterface : MonoBehaviour
{
    // Records the input from the user as a char: 'r', 'p', or 's'
    private char input = '0';

    // Records the number of times the player has won.
    private int playerWins = 0;

    // Records the number of times the AI has won.
    private int aiWins = 0;

    // Update is called once per frame
    void Update()
    {
        // Check if the mouse button was pressed.
        if (Input.GetMouseButtonDown(0))
        {
            // Grab the position that was clicked by the mouse.
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 mousePos2D = new Vector2(mousePos.x, mousePos.y);

            // Use a raycast to determine whether a tile was clicked.
            RaycastHit2D hit = Physics2D.Raycast(mousePos2D, Vector2.zero);

            // If a tile with a collider was clicked...
            if (hit.collider != null)
            {
                // Begin an output string we will print to the log.
                string output = "You selected: " + hit.collider.gameObject.name;

                // Convert the collider clicked by the user to the r/p/s character in the input variable.
                if (hit.collider.gameObject.name == "Rock") input = 'r';
                else if (hit.collider.gameObject.name == "Paper") input = 'p';
                else if (hit.collider.gameObject.name == "Scissors") input = 's';
                else return;

                // Ask the ngram AI to predict what the player will choose..
                // You will need to implement this code and any history tracking it requires.
                // For now, we will predict a move at random.
                char predicted = RockPaperScissors.RandomMove();
                RPSMove predMove = RockPaperScissors.CharToMove(predicted);
                output += "\nThe NGram AI predicts you will play: " + predMove;

                // Given the predicted user move, choose the move that will win against it.
                RPSMove aiMove = RockPaperScissors.GetWinner(predMove);
                output += "\nThe NGram AI plays: " + aiMove;

                // Get the result of playing the user and AI moves.
                int result = RockPaperScissors.Play(RockPaperScissors.CharToMove(input), aiMove);

                // If the result is 1, the player wins.
                if (result > 0)
                {
                    output += "\nYou win!";
                    playerWins++;
                }
                // If the result is -1, the AI wins.
                else if (result < 0)
                {
                    output += "\nYou lose...";
                    aiWins++;
                }
                // If the result is 0, there is a tie.
                else output += "\nTie";
                
                // Print the total wins to the log.
                output += "\nPlayer Wins: " + playerWins;
                output += "\nAI Wins: " + aiWins;

                // Output the combined output string to the log.
                Debug.Log(output);
            }
        }
    }
}
