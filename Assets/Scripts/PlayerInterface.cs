using UnityEngine;
using RPS;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using System.Collections;

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

    //the size of the window the AI looks at + 1
    private int nValue = 4;

    //the list of moves the player has made equal to the AI's view window + 1 new move
    private List<char> playerMoves = new List<char>(4);

    //dictionary that stores KeyDataRecords with keys equal to a string of moves
    Dictionary<string, KeyDataRecord> data = new Dictionary<string, KeyDataRecord>();

    //unique variable which stores unique sets of moves made at a certain key
    KeyDataRecord keyData = new KeyDataRecord();

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

                //declare predicted variable
                char predicted;

                //until the player has made enough moves to fill the AI's view window
                if (playerMoves.Count < nValue)
                {
                    //add the player move to the list
                    playerMoves.Add(input);

                    //AI makes a random move 
                    predicted = RockPaperScissors.RandomMove();
                }
                //otherwise the AI has enough data to start predicting moves
                else
                {
                    //remove the oldest move by shuffling all current moves to the left by one
                    for (int i = 1; i < nValue; i++)
                    {
                        playerMoves[i-1] = playerMoves[i];
                    }

                    //set the last value in the list to the newest move
                    playerMoves[nValue-1] = input;
                    
                    //declare an empty variable to store the AI's view window
                    string window = "";

                    //add the 3 most current moves the player has made minus their most recent move
                    for(int i = 0; i < nValue-1; i++)
                    {
                        window += playerMoves[i];
                    }

                    //AI predicts what move the player will make based on this window excluding the most recent move
                    predicted = GetMostLikely(window);

                    //register the full sequence of moves the player has made including the most recent move
                    RegisterSequence(playerMoves);
                }

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
    //
    //method that registers the player's most recent sequence of moves
    private void RegisterSequence(List<char> moves)
    {
        //declare key and value variables to store these values
        string key = "";
        char value;

        //save the last three of the players most recent moves as a key
        for (int i = 0; i < nValue-1; i++)
        {
            key += moves[i];
        }

        //save the player's current move as the value stored at that key
        value = moves[nValue-1];

        //if the database of keys does not already contain this key
        if(!data.ContainsKey(key))
        {
            //create a new keyData variable at that key and save it
            keyData = data[key] = new KeyDataRecord();
        }
        else
        {
            //otherwise save the keyData at that key in the database
            keyData = data[key];
        }

        //if the keyData's list of known moves of this type does not contain this type yet
        if(!keyData.Counts.ContainsKey(value))
        {
            //create it and set the amount of known moves of this type to 1
            keyData.Counts.Add(value, 1);
        }
        else
        { 
            //otherwise increment the amount of known moves of this type
            keyData.Counts[value]++; 
        }
        
    }
    //
    //method the AI uses to predict the most likely move the player will make given a list of previous moves
    private char GetMostLikely(string moves)
    {
        //if the database already contains this set of moves
        if (data.ContainsKey(moves))
        {
            //grab the keyData stored at this key
            keyData = data[moves];
        }
        else
        {
            //otherwise this set of moves has yet to be made, so the AI cannot predict a move and chooses randomly
            return RockPaperScissors.RandomMove();
        }
        
        //declare variables to store the highest value and best action
        int highestValue = 0;
        char bestAction = ' ';

        //reset the moves
        moves = "";

        //list out each key as a string of moves
        foreach(char key in keyData.Counts.Keys)
        {
            moves += key;
        }

        //iterate through each character of the string
        for (int i = 0; i < moves.Length; i++)
        {
            //if the amount of this kind of move made is the current highest
            if (keyData.Counts[moves[i]] > highestValue)
            {
                //set the new highest value and update the best action to make
                highestValue = keyData.Counts[moves[i]];
                bestAction = moves[i];
            }
        }

        //return the best predicted action
        return bestAction;
    }
    //
    //custom class which stores unique dictionary of moves and amount of those moves made at a specific key
    private class KeyDataRecord
    {
        public Dictionary<char, int> Counts { get; set; } = new Dictionary<char, int>();
    }
}
