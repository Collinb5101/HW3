using System;

namespace RPS
{
    /// <summary>
    /// This is a C# class that provides helper methods for playing Rock Paper Scissors.
    /// </summary>
    public static class RockPaperScissors
    {
        // Given a character r/p/s, returns a corresponding RPSMove enum.
        public static RPSMove CharToMove (char c)
        {
            if (c == 'r') return RPSMove.Rock;
            else if (c == 'p') return RPSMove.Paper;
            else return RPSMove.Scissors;
        }

        // Given two RPSMoves, returns the winner.
        public static int Play (RPSMove player1, RPSMove player2)
        {
            if (player2 == GetWinner(player1)) return -1;
            else if (player1 == GetWinner(player2)) return 1;
            else return 0;
        }

        // Given a RPSMove, returns the move that would win against it.
        public static RPSMove GetWinner (RPSMove original)
        {
            if (original == RPSMove.Rock) return RPSMove.Paper;
            else if (original == RPSMove.Paper) return RPSMove.Scissors;
            else return RPSMove.Rock;
        }

        // Returns a random r/p/s move.
        public static char RandomMove()
        {
            Random rand = new Random();
            int roll = rand.Next(0,3);
            if (roll == 0) return 'r';
            else if (roll == 1) return 'p';
            else return 's';
        }
    }

    // Enumeration of possible RPS moves.
    public enum RPSMove
    {
        Rock = 0, Paper = 1, Scissors = 2
    }
}
