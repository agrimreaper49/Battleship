using System;

/* This namespace contains all the common classes we need to play our Battleship game.
 * 
 * YOU MAY NOT CHANGE ANYTHING IN THIS FILE!!!
 */
namespace Torpedo
{
    /* This enum lets you specify to which ship you are referring when accessing data such as Dictionaries. */
    public enum Ships
    {
        AircraftCarrier,
        Battleship,
        Cruiser,
        Submarine,
        Destroyer
    }

    /* TorpedoShot defines an attacking shot in the game.  It specifies the row and column being attacked, both as strings. */
    public class TorpedoShot
    {
        public TorpedoShot()
        {

        }

        public TorpedoShot(string r, string c)
        {
            row = r;
            column = c;
        }

        private string row;
        public string Row
        {
            get
            {
                return row;
            }
            set
            {
                row = value;
            }
        }

        private string column;
        public string Column
        {
            get
            {
                return column;
            }
            set
            {
                column = value;
            }
        }
    }

    /* TorpedoResult is what the main program sends back to the player, letting them know the reult of their show.
     * It first indicates the original shot in the Shot field, then whether it was a hit or miss in the WasHit field
     * and, finally, whether a ship was sunk by the shot.  If no ship was sunk, then Sunk will be an empty string.
     * If a ship was successsfully sunk by the shot, then Sunk will indicate the sunken ship with either "Aircraft Carrier",
     * "Battleship", "Cruiser", Submarine" or "Destroyer". */
    public class TorpedoResult
    {
        public TorpedoResult(TorpedoShot ts, bool wh, string s)
        {
            shot = ts;
            wasHit = wh;
            sunk = s;
        }
        private TorpedoShot shot;
        public TorpedoShot Shot
        {
            get
            {
                return shot;
            }
        }

        private bool wasHit;
        public bool WasHit
        {
            get
            {
                return wasHit;
            }
        }

        private string sunk;
        public string Sunk
        {
            get
            {
                return sunk;
            }
        }
    }
}