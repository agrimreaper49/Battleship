using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Torpedo;

namespace Eurasia
{
    public class PlayerEurasia
    {
        string[] aircraftCarrier;
        string[] battleship;
        string[] cruiser;
        string[] submarine;
        string[] destroyer;
        private List<string> preload;
        private Queue<string> newShots;
        private List<string> shotsAlreadyTaken;
        private List<TorpedoShot> torpedoShotsTaken;
        private HashSet<string> hitLocations;
        private SortedSet<string> locations;
        private SortedSet<Ships> shipsHit = new SortedSet<Ships>();
        private List<string> totalShotLocations;
        private string lastShotRow = "";
        private string shipSunk = "";
        private int lastShotCol = 0;
        private Boolean wasTheShotAHit = false;
        private Boolean wasAShipSunk = false;
        private string desiredAI = "";
        private string hitRow = "";
        private string hitCol = "";
        private bool notFirstRun = false;
        private bool wasThereEverAHit = false;
        private bool isVertical = false;
        private bool isHorizontal = false;
        private TorpedoShot shotToTake = new TorpedoShot();
        private bool isFirstTurn = false;


        /* The NextMove() method is called every time the main program needs a torpedo shot from this player.
         * Locations in this game always start with a letter A - J, and are followed by a number 1 - 10.
         * This is where most of your "artificial intelligence" will come into play.  As an example, I have
         * coded the Oceania player to have a valid, but very unintelligent targetting algorithms.  All it
         * does is pick a random valid square and shoot at it if it hasn't already targetted that square.  If
         * it has targetted that square already, it just tries again with a different randomly picked square.
         * It doesn't even use the ResultOfShot() method to help plan its next shot. */
        public TorpedoShot NextMove()
        {
            string row = "";
            string column = "";
            shotToTake = new TorpedoShot();
            if (row == "" && column == "")
            {
                //Protect against any potential error (generate random shot for safety)
                shotToTake = genericAI();
            }
            if (!wasAShipSunk && hitLocations.Count() == 0)
            {
                shotToTake = tryNewOptions();
            }
            if (wasAShipSunk || shipSunk != "")
            {
                hitLocations.Clear();
                hitLocations = new HashSet<string>();
                shotToTake = tryNewOptions();
            }
            if (hitLocations.Count == 1)
            {
                string r = hitLocations.ElementAt(0).Substring(0, 1);
                int c = Int32.Parse(hitLocations.ElementAt(0).Substring(1));
                createNewShots(r, c);
                shotToTake = huntPrey();
            }
            else if (hitLocations.Count > 1)
            {
                string originalHitCol = hitLocations.ElementAt(0).Substring(1);
                string originalHitRow = hitLocations.ElementAt(0).Substring(0, 1);
                string hitCol = hitLocations.ElementAt(hitLocations.Count() - 1).Substring(1);
                string hitRow = hitLocations.ElementAt(hitLocations.Count() - 1).Substring(0, 1);
                string hitColPrevious = hitLocations.ElementAt(hitLocations.Count() - 2).Substring(1);
                string hitRowPrevious = hitLocations.ElementAt(hitLocations.Count() - 2).Substring(0, 1);
                string lastShotCol = totalShotLocations.ElementAt(totalShotLocations.Count() - 1).Substring(1);
                string lastShotRow = totalShotLocations.ElementAt(totalShotLocations.Count() - 1).Substring(0, 1);
                //TODO add measure to make sure that there is a case when there are two locations and it missed last shot
                if (hitLocations.Count() > 1 && !wasThereEverAHit)
                {
                    if (isVertical)
                    {
                        if (lastShotRow.ToCharArray()[0] > originalHitCol.ToCharArray()[0])
                        {

                            createVerticalShotsUpward(originalHitRow, originalHitCol);

                        }
                        //If the row we just is less than the row before it
                        else
                        {
                            createVerticalShotsDownward(originalHitRow, originalHitCol);

                        }
                    }
                    else if (isHorizontal)
                    {
                        //If the column we hit is greater than the one before it
                        if (Int32.Parse(lastShotCol) > Int32.Parse(originalHitCol))
                        {
                            createHorizontalShotsLeftward(originalHitRow, originalHitCol);

                        }
                        //If the column we hit is less than the row before it
                        else
                        {
                            createHorizontalShotsRightward(originalHitRow, originalHitCol);
                        }
                    }
                }
                //If the last two shots that were hit were vertical 
                else if (hitCol.Equals(hitColPrevious))
                {
                    isVertical = true;
                    if (!wasAShipSunk)
                    {
                        //If the row we just hit is greater than the row before it
                        if (hitRow.ToCharArray()[0] > hitRowPrevious.ToCharArray()[0])
                        {
                            if (hitRow.ToCharArray()[0] == 'J')
                            {
                                createVerticalShotsUpward(originalHitRow, originalHitCol);
                            }
                            else
                            {
                                createVerticalShotsDownward(hitRow, hitCol);
                            }
                        }
                        //If the row we just is less than the row before it
                        else
                        {
                            if (hitRow.ToCharArray()[0] == 'A')
                            {
                                createVerticalShotsDownward(originalHitRow, originalHitCol);
                            }
                            else
                            {
                                createVerticalShotsUpward(hitRow, hitCol);
                            }
                        }
                    }
                    else
                    {
                        hitLocations.Clear();
                        shotToTake = tryNewOptions();
                    }
                }
                //If the last two shots that were hit were horizontal
                else if (hitRow.Equals(hitRowPrevious))
                {
                    isHorizontal = true;
                    if (!wasAShipSunk)
                    {
                        //If the column we hit is greater than the one before it
                        if (Int32.Parse(hitCol) > Int32.Parse(hitColPrevious))
                        {
                            if (Int32.Parse(hitCol) > 9)
                            {
                                createHorizontalShotsLeftward(originalHitRow, originalHitCol);
                            }
                            else
                            {
                                createHorizontalShotsRightward(hitRow, hitCol);
                            }
                        }
                        //If the column we hit is less than the row before it
                        else
                        {
                            if (Int32.Parse(hitCol) == 1)
                            {
                                createHorizontalShotsRightward(originalHitRow, originalHitCol);
                            }
                            else
                            {
                                createHorizontalShotsLeftward(hitRow, hitCol);
                            }
                        }
                    }
                    else
                    {
                        hitLocations.Clear();
                        shotToTake = tryNewOptions();
                    }
                }
                shotToTake = huntPrey();
            }
            if (shotToTake.Row == "A" || shotToTake.Row == "B" || shotToTake.Row == "C" || shotToTake.Row == "D" 
                || shotToTake.Row == "E" || shotToTake.Row == "F" || shotToTake.Row == "G" 
                || shotToTake.Row == "H" || shotToTake.Row == "I" || shotToTake.Row == "J")
            {
                
            } 
            else
            {
                shotToTake = genericAI();
            }
            if (shotToTake.Column == "1" || shotToTake.Column == "2" || shotToTake.Column == "3" || shotToTake.Column == "4"
                || shotToTake.Column == "5" || shotToTake.Column == "6" || shotToTake.Column == "7"
                || shotToTake.Column == "8" || shotToTake.Column == "9" || shotToTake.Column == "10")
            {
                
            }
            else
            {
                shotToTake = genericAI();
            }
            if (shotToTake.Row == "" || shotToTake.Column == "")
            {
                shotToTake = genericAI();
            }
            return shotToTake;
        }
        public TorpedoShot tryNewOptions()
        {
            string item = preload.ElementAt(0);
            string r = item[0] + "";
            string c = item[1] + "";
            if (item.Length > 2)
            {
                if (Int32.Parse((item[2] + "")) == 0)
                {
                    c = "10";
                }
            }
            preload.Remove(item);
            TorpedoShot shot = new TorpedoShot(r, c);
            return shot;
        }
        public void createVerticalShotsDownward(string row, string column)
        {
            char c = (char)(row.ToCharArray()[0] + 1);
            string newrow = c + "";
            newShots.Clear();
            newShots.Enqueue(newrow + column);
        }
        public void createVerticalShotsUpward(string row, string column)
        {
            char c = (char)(row.ToCharArray()[0] - 1);
            string newrow = c + "";
            newShots.Clear();
            newShots.Enqueue(newrow + column);
        }
        public void createHorizontalShotsLeftward(string row, string column)
        {
            string newcolumn = (Int32.Parse(column) - 1) + "";
            newShots.Clear();
            newShots.Enqueue(row + newcolumn);
        }
        public void createHorizontalShotsRightward(string row, string column)
        {
            string newcolumn = (Int32.Parse(column) + 1) + "";
            newShots.Clear();
            newShots.Enqueue(row + newcolumn);
        }

        public void createNewShots(string row, int column)
        {
            //
            // IF SOMETHING BREAKS HERE I AM GOING TO KILL SOMEONE
            //

            if (row.Equals("A") && column == 1)
            {
                if (!shotsAlreadyTaken.Contains("B1"))
                {
                    newShots.Enqueue("B1");
                }
                if (!shotsAlreadyTaken.Contains("A2"))
                {
                    newShots.Enqueue("A2");
                }
            }
            else if (row.Equals("J") && column == 1)
            {
                if (!shotsAlreadyTaken.Contains("I1"))
                {
                    newShots.Enqueue("I1");
                }
                if (!shotsAlreadyTaken.Contains("J2"))
                {
                    newShots.Enqueue("J2");
                }
            }
            else if (row.Equals("J") && column == 10)
            {
                if (!shotsAlreadyTaken.Contains("I10"))
                {
                    newShots.Enqueue("I10");
                }
                if (!shotsAlreadyTaken.Contains("J9"))
                {
                    newShots.Enqueue("J9");
                }
            }
            else if (row.Equals("A") && column == 10)
            {
                if (!shotsAlreadyTaken.Contains("A9"))
                {
                    newShots.Enqueue("A9");
                }
                if (!shotsAlreadyTaken.Contains("B10"))
                {
                    newShots.Enqueue("B10");
                }
            }
            else if (!(row == "A") && !(row == "J") && (column != 10) && column != 1)
            {
                checkAlreadyShotOtherwiseAdd(row + (column + 1));
                checkAlreadyShotOtherwiseAdd(row + (column - 1));
                char c = (char)(row.ToCharArray()[0] + 1);
                char d = (char)(row.ToCharArray()[0] - 1);
                checkAlreadyShotOtherwiseAdd(c + "" + column);
                checkAlreadyShotOtherwiseAdd(d + "" + column);
            }
            else if ((!(row == "A") || !(row == ("J"))) && (column == 1))
            {
                checkAlreadyShotOtherwiseAdd(row + (column + 1));
                char c = (char)(row.ToCharArray()[0] + 1);
                char d = (char)(row.ToCharArray()[0] - 1);
                checkAlreadyShotOtherwiseAdd(c + "" + column);
                checkAlreadyShotOtherwiseAdd(d + "" + column);
            }
            else if ((!(row == "A") || !(row == "J")) && (column == 10))
            {
                checkAlreadyShotOtherwiseAdd(row + (column - 1));
                char c = (char)(row.ToCharArray()[0] + 1);
                char d = (char)(row.ToCharArray()[0] - 1);
                checkAlreadyShotOtherwiseAdd(c + "" + column);
                checkAlreadyShotOtherwiseAdd(d + "" + column);
            }
            else if (row == "A" && (column == 1 || column == 2 || column == 3 || column == 4 || column == 5 || column == 6 || column == 7 || column == 8 || column == 9))
            {
                checkAlreadyShotOtherwiseAdd(row + (column + 1));
                checkAlreadyShotOtherwiseAdd(row + (column - 1));
                char c = (char)(row.ToCharArray()[0] + 1);
                checkAlreadyShotOtherwiseAdd(c + "" + column);
            }
            else if (row == "J" && (column == 1 || column == 2 || column == 3 || column == 4 || column == 5 || column == 6 || column == 7 || column == 8 || column == 9))
            {
                checkAlreadyShotOtherwiseAdd(row + (column + 1));
                checkAlreadyShotOtherwiseAdd(row + (column - 1));
                char d = (char)(row.ToCharArray()[0] - 1);
                checkAlreadyShotOtherwiseAdd(d + "" + column);
            }

        }
        //helper method to make sure I'm not duplicating shots
        public void checkAlreadyShotOtherwiseAdd(string item)
        {
            if (!shotsAlreadyTaken.Contains(item))
            {
                newShots.Enqueue(item);
            }
        }
        public TorpedoShot huntPrey()
        {
            string s = newShots.Dequeue();
            string row = s.Substring(0, 1);
            string column = s.Substring(1);
            TorpedoShot shot = new TorpedoShot(row, column);
            if (row + column == "")
            {
                shot = genericAI();
            }
            return shot;
        }

        /* The ResultOfShot() method must contain all the code you need to adjust your internal data
         * in response to the result of your latest shot. */
        public void ResultOfShot(TorpedoResult result)
        {
            TorpedoResult res = result;
            wasThereEverAHit = res.WasHit;
            if (wasThereEverAHit)
            {
                hitLocations.Add(res.Shot.Row + res.Shot.Column);
            }
            lastShotRow = res.Shot.Row;
            lastShotCol = Int32.Parse(res.Shot.Column);
            shipSunk = res.Sunk;
            if (res.Sunk == "")
            {
                wasAShipSunk = false;
            }
            else
            {
                wasAShipSunk = true;
                addShipSunk(res.Sunk);
            }
            if (wasTheShotAHit)
            {
                desiredAI = "Go on the hunt";
            }
            else
            {
                desiredAI = "";
            }
            totalShotLocations.Add(res.Shot.Row + res.Shot.Column);

        }

        private TorpedoShot advancedAI()
        {
            return new TorpedoShot("B", "2");
        }
        private void addShipSunk(string s)
        {
            if (s == "Aircraft Carrier")
            {
                shipsHit.Add(Ships.AircraftCarrier);
            }
            else if (s == "Battleship")
            {
                shipsHit.Add(Ships.Battleship);
            }
            else if (s == "Cruiser")
            {
                shipsHit.Add(Ships.Cruiser);
            }
            else if (s == "Submarine")
            {
                shipsHit.Add(Ships.Submarine);
            }
            else if (s == "Destroyer")
            {
                shipsHit.Add(Ships.Destroyer);
            }
            return;
        }

        private TorpedoShot genericAI()
        {
            TorpedoShot shot = new TorpedoShot();
            string stringShot = "";

            while (stringShot == "")
            {
                Random random = new Random();
                int row = random.Next(10);
                int column = random.Next(10) + 1;
                shot = new TorpedoShot(((char)('A' + row)).ToString(), column.ToString());
                stringShot = shot.Row + shot.Column;
                if (totalShotLocations.Contains(stringShot))
                    stringShot = "";
            }

            return shot;
        }

        /* The Reset() method must contain all the code you need to prepare for a new game, including
         * resetting your internal data for a "rematch". */
        public void Reset()
        {
            totalShotLocations = new List<string>();
            hitLocations = new HashSet<string>();
            shotsAlreadyTaken = new List<string>();
            torpedoShotsTaken = new List<TorpedoShot>();
            aircraftCarrier = new string[5] { "E3", "E4", "E5", "E6", "E7" };
            battleship = new string[4] { "G7", "G8", "G9", "G10" };
            cruiser = new string[3] { "J1", "J2", "J3" };
            submarine = new string[3] { "A10", "B10", "C10" };
            destroyer = new string[2] { "A4", "A5" };
            preload = new List<string> { "B1", "I10", "A2", "J9", "D1", "G10", "C2", "H9", "B3", "I8", "A4",
            "J8", "F1", "E10", "E2", "F9", "D3", "G8", "C4", "H7", "B5", "I6", "A6", "J5", "H1", "C10",
            "G2", "D9", "F3", "E8", "E4", "F7", "G6", "D5", "C6", "H5", "B7", "J3", "A10", "J1", "B9",
            "I2", "C8", "H3", "D7", "G4", "E6", "F5" };
            // preload = new List<string> { "G4", "I8", "C4" };
            newShots = new Queue<string>();
        }

        /* Ship locations are only valid if the ship is positioned horizontally or vertically.
         * This means that either all the letters in the position are constant (horizontal) or
         * all the numbers in the position are constant (vertical).  Furthermore, the sequential
         * information must be in order, from lowest to highest.  Finally, the string array
         * representing the position must be exactly the predefined size of the ship whose position
         * it is representing.  For instance:
         * 
         *      Submarine:  {"E5", "F5", "G5"}          // valid
         *      Submarine:  {"E5", "F5", "G6"}          // invalid
         *      Cruiser:    {"E5", "F5", "G5"}          // invalid
         *      Battleship: {"E5", "F5", "G5"}          // invalid
         *      Battleship: {"J6", "J7", "J8", "J9"}    // valid
         *      Battleship: {"J9", "J8", "J7", "J6"}    // invalid
         *      Destroyer:  {"K1", "L1"}                // valid
         *      Destroyer:  {"L1", "K1"}                // invalid
         */

        /* Return the location of your Aircraft Carrier */
        public string[] GetAircraftCarrier()
        {
            aircraftCarrier = new string[5] { "E3", "E4", "E5", "E6", "E7" };
            return aircraftCarrier;
        }

        /* Return the location of your Battleship */
        public string[] GetBattleship()
        {
            battleship = new string[4] { "G7", "G8", "G9", "G10" };
            return battleship;
        }

        /* Return the location of your Cruiser */
        public string[] GetCruiser()
        {
            cruiser = new string[3] { "J1", "J2", "J3" };
            return cruiser;
        }

        /* Return the location of your Submarine */
        public string[] GetSubmarine()
        {
            submarine = new string[3] { "A10", "B10", "C10" };
            return submarine;
        }

        /* Return the location of your Destroyer */
        public string[] GetDestroyer()
        {
            destroyer = new string[2] { "A4", "A5" };
            return destroyer;
        }

    }
}
