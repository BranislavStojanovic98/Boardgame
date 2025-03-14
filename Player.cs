using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Boardgame
{
    class Player
    {
        public string Name { get; set; }
        public int Position { get; set; }
        public int StartingPosition { get; set; }
        public int PreviousPosition { get; set; }
        public int Score { get; set; }

        public Player(string name, int startingPosition)
        {
            Name = name;
            Position = startingPosition;
            StartingPosition = startingPosition;
            PreviousPosition = startingPosition;
            Score = 0;
        }

        public void MoveToPosition(int newPosition)
        {
            PreviousPosition = Position;  // Save the current position as the previous position
            Position = newPosition;       // Move to the new position
        }
    }
}
