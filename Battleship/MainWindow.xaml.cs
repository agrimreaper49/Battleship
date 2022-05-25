using Eurasia;
using Oceania;
using System;
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
using System.Windows.Threading;
using Torpedo;

namespace Battleship
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private PlayerOceania oceania = new PlayerOceania();
        private PlayerEurasia eurasia = new PlayerEurasia();
        private TorpedoShot torpedoShot;
        private SortedSet<string> oceaniaShipHits;
        private SortedSet<string> eurasiaShipHits;
        private SortedSet<string> eurasiaLocations;
        private SortedSet<string> oceaniaLocations;
        private string[,] oceaniaState = new string[10, 10];
        private string[,] eurasiaState = new string[10, 10];
        private List<Ships> oceaniaShipsSunk;
        private List<Ships> eurasiaShipsSunk;
        private Dictionary<Ships, string[]> oceaniaShipLocations;
        private Dictionary<Ships, string[]> eurasiaShipLocations;
        private bool isOceaniaTurn;
        private bool winner;
        private bool wasHit;
        private List<string[]> oceaniaMoves;
        private List<string[]> eurasiaMoves;
        private DispatcherTimer timer = new DispatcherTimer();
        private Button playGame;
        private List<Ellipse> ellipsesonBoard;

        private Label eurasiaBattleship;
        private Label eurasiaCruiser;
        private Label eurasiaCarrier;
        private Label eurasiaSubmarine;
        private Label eurasiaDestroyer;

        private Label oceaniaBattleship;
        private Label oceaniaCruiser;
        private Label oceaniaCarrier;
        private Label oceaniaSubmarine;
        private Label oceaniaDestroyer;

        private Label lastShotLabel;
        private Label otherShotLabel;
        private string shipSunk;
        private bool carrierNeverSunk = true;
        private bool destroyerNeverSunk = true; 
        private bool submarineNeverSunk = true;
        private bool battleshipNeverSunk = true;
        private bool cruiserNeverSunk = true;

        public MainWindow()
        {
            InitializeComponent();
            setupButtonAndLabels();
            setupFields();
            drawAllEurasiaShips();
            drawAllOceaniaShips();
            setupShipLabels();
        }
        public void setupFields()
        {
            oceaniaMoves = new List<string[]>();
            eurasiaMoves = new List<string[]>();
            oceaniaShipHits = new SortedSet<string>();
            eurasiaShipHits = new SortedSet<string>();
            ellipsesonBoard = new List<Ellipse>();
            isOceaniaTurn = false;
     
            eurasiaShipLocations = new Dictionary<Ships, string[]>();
            eurasiaShipLocations[Ships.AircraftCarrier] = eurasia.GetAircraftCarrier();
            eurasiaShipLocations[Ships.Battleship] = eurasia.GetBattleship();
            eurasiaShipLocations[Ships.Cruiser] = eurasia.GetCruiser();
            eurasiaShipLocations[Ships.Submarine] = eurasia.GetSubmarine();
            eurasiaShipLocations[Ships.Destroyer] = eurasia.GetDestroyer();
            oceaniaShipsSunk = new List<Ships>();
            eurasiaShipsSunk = new List<Ships>();
            oceaniaShipLocations = new Dictionary<Ships, string[]>();
            oceaniaShipLocations[Ships.AircraftCarrier] = oceania.GetAircraftCarrier();
            oceaniaShipLocations[Ships.Battleship] = oceania.GetBattleship();
            oceaniaShipLocations[Ships.Cruiser] = oceania.GetCruiser();
            oceaniaShipLocations[Ships.Submarine] = oceania.GetSubmarine();
            oceaniaShipLocations[Ships.Destroyer] = oceania.GetDestroyer();
            oceania.Reset();
            eurasia.Reset();
            shipSunk = "";
            winner = false;
            wasHit = false;
            eurasiaLocations = new SortedSet<string>();
            oceaniaLocations = new SortedSet<string>();
            addLocations("e");
            addLocations("o");
        }
        private void addLocations(string player)
        {
            if (player == "e")
            {
                foreach (string s in eurasia.GetAircraftCarrier())
                {
                    eurasiaLocations.Add(s);
                }
                foreach (string s in eurasia.GetBattleship())
                {
                    eurasiaLocations.Add(s);
                }
                foreach (string s in eurasia.GetDestroyer())
                {
                    eurasiaLocations.Add(s);
                }
                foreach (string s in eurasia.GetSubmarine())
                {
                    eurasiaLocations.Add(s);
                }
                foreach (string s in eurasia.GetCruiser())
                {
                    eurasiaLocations.Add(s);
                }
            } 
            else
            {
                foreach (string s in oceania.GetAircraftCarrier())
                {
                    oceaniaLocations.Add(s);
                }
                foreach (string s in oceania.GetBattleship())
                {
                    oceaniaLocations.Add(s);
                }
                foreach (string s in oceania.GetDestroyer())
                {
                    oceaniaLocations.Add(s);
                }
                foreach (string s in oceania.GetSubmarine())
                {
                    oceaniaLocations.Add(s);
                }
                foreach (string s in oceania.GetCruiser())
                {
                    oceaniaLocations.Add(s);
                }
            }
        }
       
        public void createShotsAlternateTurns()
        {
            Grid o = oceaniaa;
            Grid e = eurasiaa;
            TorpedoShot turn = new TorpedoShot();
            if (isOceaniaTurn)
            {
                turn = oceania.NextMove();
                //Convert row into char array and grab 1st value to avoid error
                char c = turn.Row.ToCharArray()[0];
                int column = Int32.Parse(turn.Column);
                int row = (c - 'A') + 1;
                Ellipse shot = new Ellipse();
                shot.Stroke = new SolidColorBrush(Colors.Red);
                shot.Fill = new SolidColorBrush(Colors.Red);
                shot.Width = 12;
                shot.Height = 12;
                ellipsesonBoard.Add(shot);
                Grid.SetColumn(shot, column);
                Grid.SetRow(shot, row);
                string locationOfShot = "";
                locationOfShot += c;
                locationOfShot += column;
                //If it is oceania turn then go ahead and display it on the eurasia board
                lastShotLabel.FontSize = 10;
                lastShotLabel.Content = "Last Shot: " + locationOfShot;
                if (eurasiaLocations.Contains(locationOfShot))
                {
                    wasHit = true;
                    shot.Fill = new SolidColorBrush(Colors.Red);
                    shot.Stroke = new SolidColorBrush(Colors.Red);
                    eurasiaShipHits.Add(locationOfShot);
                    e.Children.Add(shot);
                    foreach (Ships s in eurasiaShipLocations.Keys)
                    {
                        updateShipsSunkLabel(s, "e");
                    }
                    TorpedoResult result = new TorpedoResult(turn, true, shipSunk);
                    oceania.ResultOfShot(result);
                }
                else
                {
                    wasHit = false;
                    shot.Stroke = new SolidColorBrush(Colors.Blue);
                    shot.Fill = new SolidColorBrush(Colors.Blue);
                    e.Children.Add(shot);
                    TorpedoResult result = new TorpedoResult(turn, false, "");
                    oceania.ResultOfShot(result);
                }
                isOceaniaTurn = false;
            }
            else
            {
                turn = eurasia.NextMove();
                //Convert row into char array and grab 1st value to avoid error
                char c = turn.Row.ToCharArray()[0];
                int column = Int32.Parse(turn.Column);
                int row = (c - 'A') + 1;
                Ellipse shot = new Ellipse();
                shot.Stroke = new SolidColorBrush(Colors.Red);
                shot.Fill = new SolidColorBrush(Colors.Red);
                shot.Width = 12;
                shot.Height = 12;
                ellipsesonBoard.Add(shot);
                Grid.SetColumn(shot, column);
                Grid.SetRow(shot, row);
                string locationOfShot = "";
                locationOfShot += c;
                locationOfShot += column;
                otherShotLabel.FontSize = 10;
                otherShotLabel.Content = "Last Shot: " + locationOfShot;
                if (oceaniaLocations.Contains(locationOfShot))
                {
                    wasHit = true;
                    shot.Fill = new SolidColorBrush(Colors.Red);
                    shot.Stroke = new SolidColorBrush(Colors.Red);
                    oceaniaShipHits.Add(locationOfShot);
                    foreach (Ships s in oceaniaShipLocations.Keys)
                    {
                        updateShipsSunkLabel(s, "o");
                    }
                    TorpedoResult result = new TorpedoResult(turn, true, shipSunk);
                    eurasia.ResultOfShot(result);
                }
                else
                {
                    wasHit = false;
                    shot.Stroke = new SolidColorBrush(Colors.Blue);
                    shot.Fill = new SolidColorBrush(Colors.Blue);
                    TorpedoResult result = new TorpedoResult(turn, false, shipSunk);
                    eurasia.ResultOfShot(result);
                }
                o.Children.Add(shot);
                isOceaniaTurn = true;
            }
            //Use set equals to accurately determine the winner
            if (oceaniaShipHits.SetEquals(oceaniaLocations))
            {
                winner = true;
            }
            if (eurasiaShipHits.SetEquals(eurasiaLocations))
            {
                winner = true;
            }

        }
        //This method is to update the label that shows which ships are sunk and which ones 
        //are still afloat. You pass in an "e" if you want to update eurasia, and an "o" for oceania
        private void updateShipsSunkLabel(Ships s, string player)
        {
            if (player == "e")
            {
                //Switch case to alternate between the different ships
                switch (s)
                {
                    case Ships.AircraftCarrier:
                        //reset counter every time the case is called
                        int counter = 0;
                        foreach (string str in eurasia.GetAircraftCarrier())
                        {
                            if (eurasiaShipHits.Contains(str))
                            {
                                counter++;
                            }
                            if (counter == 5)
                            {
                                shipSunk = "Aircraft Carrier";
                                eurasiaCarrier.Foreground = new SolidColorBrush(Colors.Red);
                                eurasiaShipsSunk.Add(Ships.AircraftCarrier);
                                return;
                            }
                            else
                            {
                                shipSunk = "";
                            }
                        }
                        break;
                    case Ships.Battleship:
                        counter = 0;
                        foreach (string str in eurasia.GetBattleship())
                        {
                            if (eurasiaShipHits.Contains(str))
                            {
                                counter++;
                            }
                            if (counter == 4)
                            {
                                shipSunk = "Battleship";
                                eurasiaShipsSunk.Add(Ships.Battleship);
                                eurasiaBattleship.Foreground = new SolidColorBrush(Colors.Red);
                                return;
                            }
                            else
                            {
                                shipSunk = "";
                            }
                        }
                        break;
                    case Ships.Submarine:
                        counter = 0;
                        foreach (string str in eurasia.GetSubmarine())
                        {
                            if (eurasiaShipHits.Contains(str))
                            {
                                counter++;
                            }
                            if (counter == 3)
                            {
                                shipSunk = "Submarine";
                                eurasiaShipsSunk.Add(Ships.Submarine);
                                eurasiaSubmarine.Foreground = new SolidColorBrush(Colors.Red);
                                return;
                            }
                            else
                            {
                                shipSunk = "";
                            }
                        }
                        break;
                    case Ships.Cruiser:
                        counter = 0;
                        foreach (string str in eurasia.GetCruiser())
                        {
                            if (eurasiaShipHits.Contains(str))
                            {
                                counter++;
                            }
                            if (counter == 3)
                            {
                                shipSunk = "Cruiser";
                                eurasiaShipsSunk.Add(Ships.Cruiser);
                                eurasiaCruiser.Foreground = new SolidColorBrush(Colors.Red);
                                return;
                            }
                            else
                            {
                                shipSunk = "";
                            }
                        }
                        break;
                    case Ships.Destroyer:
                        counter = 0;
                        foreach (string str in eurasia.GetDestroyer())
                        {
                            if (eurasiaShipHits.Contains(str))
                            {
                                counter++;
                            }
                            if (counter == 2)
                            {
                                shipSunk = "Destroyer";
                                eurasiaShipsSunk.Add(Ships.Destroyer);
                                eurasiaDestroyer.Foreground = new SolidColorBrush(Colors.Red);
                                return;
                            } 
                            else
                            {
                                shipSunk = "";
                            }
                        }
                        break;
                }
            }
            else
            {
                switch (s)
                {
                    case Ships.AircraftCarrier:
                        int counter = 0;
                        if (!carrierNeverSunk)
                        {
                            shipSunk = "";
                            return;
                        }
                        foreach (string str in oceania.GetAircraftCarrier())
                        {
                            if (oceaniaShipHits.Contains(str))
                            {
                                counter++;
                            }
                            if (counter == 5)
                            {
                                carrierNeverSunk = false;
                                shipSunk = "Aircraft Carrier";
                                oceaniaShipsSunk.Add(Ships.AircraftCarrier);
                                oceaniaCarrier.Foreground = new SolidColorBrush(Colors.Red);
                                return;
                            }
                            else
                            {
                                shipSunk = "";
                            }
                        }
                        break;
                    case Ships.Battleship:
                        counter = 0;
                        if (!battleshipNeverSunk)
                        {
                            shipSunk = "";
                            return;
                        }
                        foreach (string str in oceania.GetBattleship())
                        {
                            if (oceaniaShipHits.Contains(str))
                            {
                                counter++;
                            }
                            if (counter == 4)
                            {
                                shipSunk = "Battleship";
                                oceaniaShipsSunk.Add(Ships.Battleship);
                                oceaniaBattleship.Foreground = new SolidColorBrush(Colors.Red);
                                return;
                            }
                            else
                            {
                                shipSunk = "";
                            }
                        }
                        break;
                    case Ships.Submarine:
                        counter = 0;
                        if (!submarineNeverSunk)
                        {
                            shipSunk = "";
                            return;
                        }
                        foreach (string str in oceania.GetSubmarine())
                        {
                            if (oceaniaShipHits.Contains(str))
                            {
                                counter++;
                            }
                            if (counter == 3)
                            {
                                shipSunk = "Submarine";
                                oceaniaShipsSunk.Add(Ships.Submarine);
                                oceaniaSubmarine.Foreground = new SolidColorBrush(Colors.Red);
                                return;
                            }
                            else
                            {
                                shipSunk = "";
                            }
                        }
                        break;
                    case Ships.Cruiser:
                        if (!cruiserNeverSunk)
                        {
                            shipSunk = "";
                            return;
                        }
                        counter = 0;
                        foreach (string str in oceania.GetCruiser())
                        {
                            if (oceaniaShipHits.Contains(str))
                            {
                                counter++;
                            }
                            if (counter == 3)
                            {
                                shipSunk = "Cruiser";
                                oceaniaShipsSunk.Add(Ships.Cruiser);
                                oceaniaCruiser.Foreground = new SolidColorBrush(Colors.Red);
                                return;
                            }
                            else
                            {
                                shipSunk = "";
                            }
                        }
                        break;
                    case Ships.Destroyer:
                        if (!destroyerNeverSunk)
                        {
                            shipSunk = "";
                            return;
                        }
                        counter = 0;
                        foreach (string str in oceania.GetDestroyer())
                        {
                            if (oceaniaShipHits.Contains(str))
                            {
                                counter++;
                            }
                            if (counter == 2)
                            {
                                destroyerNeverSunk = false;
                                shipSunk = "Destroyer";
                                oceaniaShipsSunk.Add(Ships.Destroyer);
                                oceaniaDestroyer.Foreground = new SolidColorBrush(Colors.Red);
                                return;
                            }
                            else
                            {
                                shipSunk = "";
                            }
                        }
                        break;
                }
            }
           
        }

        //helper function to return the current player 
        public string returnCurrentPlayer()
        {
            if (!isOceaniaTurn)
            {
                return "Eurasia";
            }
            return "Oceania";
        }
      
        public void drawAllEurasiaShips()
        {
            Grid g = eurasiaa;
            //Go through all the ship locations and draw an ellipse for each ship
            foreach (KeyValuePair<Ships, string []> entry in eurasiaShipLocations)
            {
                Ellipse b = new Ellipse();
                b.Stroke = new SolidColorBrush(Colors.Green);
                b.StrokeThickness = 3;
                String[] battleShip = new string[entry.Value.Length];
                battleShip = entry.Value;
                b.Visibility = Visibility.Visible;
                //If setup horizontally
                if (battleShip[0][0] == battleShip[1][0])
                {
                    char[] locs = battleShip[0].ToCharArray();
                    int column = 0;
                    if (battleShip[0].Length == 3)
                    {
                        column = 10;
                    } 
                    else
                    {
                        column = Int32.Parse(locs[1].ToString());
                    }
                    int row = (locs[0] - 'A') + 1;
                    Grid.SetColumn(b, column);
                    Grid.SetRow(b, row);
                    Grid.SetColumnSpan(b, battleShip.Length);
                }
                //If setup vertically
                else if (battleShip[0][1] == battleShip[1][1])
                {
                    char[] locs = battleShip[0].ToCharArray();
                    int column = 0;
                    if (battleShip[0].Length == 3)
                    {
                        column = 10;
                    }
                    else
                    {
                        column = Int32.Parse(locs[1].ToString());
                    }
                    int row = (locs[0] - 'A') + 1;
                    Grid.SetColumn(b, column);
                    Grid.SetRow(b, row);
                    Grid.SetRowSpan(b, battleShip.Length);
                }
                g.Children.Add(b);
            }
        }

        public void drawAllOceaniaShips()
        {
            Grid o = oceaniaa;
            //For each ship that has been created draw it
            foreach (KeyValuePair<Ships, string[]> entry in oceaniaShipLocations)
            {
                Ellipse b = new Ellipse();
                b.Stroke = new SolidColorBrush(Colors.Green);
                b.StrokeThickness = 3;
                String[] battleShip = new string[entry.Value.Length];
                battleShip = entry.Value;
                b.Visibility = Visibility.Visible;
                //If setup horizontally
                if (battleShip[0][0] == battleShip[1][0])
                {
                    char[] locs = battleShip[0].ToCharArray();
                    int column = 0;
                    if (battleShip[0].Length == 3)
                    {
                        column = 10;
                    }
                    else
                    {
                        column = Int32.Parse(locs[1].ToString());
                    }
                    int row = (locs[0] - 'A') + 1;
                    Grid.SetColumn(b, column);
                    Grid.SetRow(b, row);
                    Grid.SetColumnSpan(b, battleShip.Length);
                }
                //If setup vertically
                else if (battleShip[0][1] == battleShip[1][1])
                {
                    char[] locs = battleShip[0].ToCharArray();
                    int column = 0;
                    if (battleShip[0].Length == 3)
                    {
                        column = 10;
                    }
                    else
                    {
                        column = Int32.Parse(locs[1].ToString());
                    }
                    int row = (locs[0] - 'A') + 1;
                    Grid.SetColumn(b, column);
                    Grid.SetRow(b, row);
                    Grid.SetRowSpan(b, battleShip.Length);
                }
                o.Children.Add(b);
            }
        }
        
        //This adds the buttons and labels to the board. 
        public void setupButtonAndLabels()
        {
            Grid background = bg;
            playGame = new Button();
            Label eurasia = new Label();
            //Setup eurasia label
            eurasia.Content = "EURASIA";
            eurasia.Visibility = Visibility.Visible;
            eurasia.FontSize = 15;
            eurasia.HorizontalAlignment = HorizontalAlignment.Center;
            Grid.SetColumn(eurasia, 0);
            Grid.SetRow(eurasia, 3);
            background.Children.Add(eurasia);
            //Setup oceania label
            Label oceania = new Label();
            oceania.Content = "OCEANIA";
            oceania.Visibility = Visibility.Visible;
            oceania.FontSize = 15;
            oceania.HorizontalAlignment = HorizontalAlignment.Center;
            Grid.SetColumn(oceania, 0);
            Grid.SetRow(oceania, 1);
            background.Children.Add(oceania);
            //Setup button to play game
            playGame.Content = "START GAME";
            playGame.Width = 80;
            playGame.Height = 30;
            playGame.Click += new RoutedEventHandler(btnPlayGame);
            Grid.SetColumn(playGame, 2);
            Grid.SetRow(playGame, 1);
            background.Children.Add(playGame);
        }
        
        //Button click to play game
        private void btnPlayGame(object sender, RoutedEventArgs e)
        {
            timer.Tick += new EventHandler(dispatcherTimer_Tick);
            timer.Interval = new TimeSpan(0, 0, 0, 0, 100);
            timer.Start();
            playGame.Content = "RESET";
            playGame.Click += new RoutedEventHandler(btnResetGame);
        }

        //Button click to reset game
        private void btnResetGame(object sender, RoutedEventArgs e)
        {
            foreach (Ellipse ell in ellipsesonBoard)
            {
                ell.Visibility = Visibility.Hidden;
            }
            ellipsesonBoard.Clear();
            //Re-setup all the fields
            setupFields();
            //Setup the content on the buttons and labels
            playGame.Content = "START GAME";
            lastShotLabel.Content = "Last Shot";
            otherShotLabel.Content = "Last Shot";
            oceaniaSubmarine.Foreground = new SolidColorBrush(Colors.Black);
            oceaniaDestroyer.Foreground = new SolidColorBrush(Colors.Black);
            oceaniaBattleship.Foreground = new SolidColorBrush(Colors.Black);
            oceaniaCruiser.Foreground = new SolidColorBrush(Colors.Black);
            oceaniaCarrier.Foreground = new SolidColorBrush(Colors.Black);
            eurasiaCruiser.Foreground = new SolidColorBrush(Colors.Black);
            eurasiaSubmarine.Foreground = new SolidColorBrush(Colors.Black);
            eurasiaBattleship.Foreground = new SolidColorBrush(Colors.Black);
            eurasiaDestroyer.Foreground = new SolidColorBrush(Colors.Black);
            eurasiaCarrier.Foreground = new SolidColorBrush(Colors.Black);
            oceania.Reset();
            eurasia.Reset();
            winner = false;
            wasHit = false; 
            //Redo the timer
            timer.Stop();
            timer = new DispatcherTimer();
            timer.Tick += new EventHandler(dispatcherTimer_Tick);
            timer.Interval = new TimeSpan(0, 0, 0, 0, 75);
            DispatcherTimer dt = sender as DispatcherTimer;
            //Add a new routed event handler for the reset button
            playGame.Click += new RoutedEventHandler(btnPlayGame);
        }

        //Dispatcher timer to play game until winner is determined
        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            if (!winner)
            {
                createShotsAlternateTurns();
            }
            if (winner)
            {
                playGame.Content = "RESTART?";
                DispatcherTimer dt = sender as DispatcherTimer;
                dt.Stop();
            }
        }

        //Helper function that foes through and sets up labels for ship
        private void setupShipLabels()
        {
            Grid background = labeleurasia;
            Grid otherBackground = labeloceania;
            Grid lastShot = lastshot;
            Grid otherlast = otherlastshot;

            eurasiaBattleship = new Label();
            eurasiaBattleship.Content = "Battleship";
            eurasiaBattleship.Visibility = Visibility.Visible;
            eurasiaBattleship.FontSize = 12;
            eurasiaBattleship.HorizontalAlignment = HorizontalAlignment.Center;
            Grid.SetColumn(eurasiaBattleship, 2);
            Grid.SetRow(eurasiaBattleship, 1);
            background.Children.Add(eurasiaBattleship);

            eurasiaCruiser = new Label();
            eurasiaCruiser.Content = "Cruiser";
            eurasiaCruiser.Visibility = Visibility.Visible;
            eurasiaCruiser.FontSize = 12;
            eurasiaCruiser.HorizontalAlignment = HorizontalAlignment.Center;
            Grid.SetColumn(eurasiaCruiser, 2);
            Grid.SetRow(eurasiaCruiser, 2);
            background.Children.Add(eurasiaCruiser);

            eurasiaSubmarine = new Label();
            eurasiaSubmarine.Content = "Submarine";
            eurasiaSubmarine.Visibility = Visibility.Visible;
            eurasiaSubmarine.FontSize = 12;
            eurasiaSubmarine.HorizontalAlignment = HorizontalAlignment.Center;
            Grid.SetColumn(eurasiaSubmarine, 2);
            Grid.SetRow(eurasiaSubmarine, 3);
            background.Children.Add(eurasiaSubmarine);

            eurasiaDestroyer = new Label();
            eurasiaDestroyer.Content = "Destroyer";
            eurasiaDestroyer.Visibility = Visibility.Visible;
            eurasiaDestroyer.FontSize = 12;
            eurasiaDestroyer.HorizontalAlignment = HorizontalAlignment.Center;
            Grid.SetColumn(eurasiaDestroyer, 2);
            Grid.SetRow(eurasiaDestroyer, 4);
            background.Children.Add(eurasiaDestroyer);

            eurasiaCarrier = new Label();
            eurasiaCarrier.Content = "Carrier";
            eurasiaCarrier.Visibility = Visibility.Visible;
            eurasiaCarrier.FontSize = 12;
            eurasiaCarrier.HorizontalAlignment = HorizontalAlignment.Center;
            Grid.SetColumn(eurasiaCarrier, 2);
            Grid.SetRow(eurasiaCarrier, 5);
            background.Children.Add(eurasiaCarrier);

            oceaniaBattleship = new Label();
            oceaniaBattleship.Content = "Battleship";
            oceaniaBattleship.Visibility = Visibility.Visible;
            oceaniaBattleship.FontSize = 12;
            oceaniaBattleship.HorizontalAlignment = HorizontalAlignment.Center;
            Grid.SetColumn(oceaniaBattleship, 2);
            Grid.SetRow(oceaniaBattleship, 1);
            otherBackground.Children.Add(oceaniaBattleship);

            oceaniaCruiser = new Label();
            oceaniaCruiser.Content = "Cruiser";
            oceaniaCruiser.Visibility = Visibility.Visible;
            oceaniaCruiser.FontSize = 12;
            oceaniaCruiser.HorizontalAlignment = HorizontalAlignment.Center;
            Grid.SetColumn(oceaniaCruiser, 2);
            Grid.SetRow(oceaniaCruiser, 2);
            otherBackground.Children.Add(oceaniaCruiser);

            oceaniaSubmarine = new Label();
            oceaniaSubmarine.Content = "Submarine";
            oceaniaSubmarine.Visibility = Visibility.Visible;
            oceaniaSubmarine.FontSize = 12;
            oceaniaSubmarine.HorizontalAlignment = HorizontalAlignment.Center;
            Grid.SetColumn(oceaniaSubmarine, 2);
            Grid.SetRow(oceaniaSubmarine, 3);
            otherBackground.Children.Add(oceaniaSubmarine);

            oceaniaDestroyer = new Label();
            oceaniaDestroyer.Content = "Destroyer";
            oceaniaDestroyer.Visibility = Visibility.Visible;
            oceaniaDestroyer.FontSize = 12;
            oceaniaDestroyer.HorizontalAlignment = HorizontalAlignment.Center;
            Grid.SetColumn(oceaniaDestroyer, 2);
            Grid.SetRow(oceaniaDestroyer, 4);
            otherBackground.Children.Add(oceaniaDestroyer);

            oceaniaCarrier = new Label();
            oceaniaCarrier.Content = "Carrier";
            oceaniaCarrier.Visibility = Visibility.Visible;
            oceaniaCarrier.FontSize = 12;
            oceaniaCarrier.HorizontalAlignment = HorizontalAlignment.Center;
            Grid.SetColumn(oceaniaCarrier, 2);
            Grid.SetRow(oceaniaCarrier, 5);
            otherBackground.Children.Add(oceaniaCarrier);

            lastShotLabel = new Label();
            lastShotLabel.Content = "Last Shot";
            lastShotLabel.Visibility = Visibility.Visible;
            lastShotLabel.FontSize = 12;
            lastShotLabel.HorizontalAlignment = HorizontalAlignment.Center;
            Grid.SetColumn(lastShotLabel, 3);
            Grid.SetColumn(lastShotLabel, 3);
            lastShot.Children.Add(lastShotLabel);

            otherShotLabel = new Label();
            otherShotLabel.Content = "Last Shot";
            otherShotLabel.Visibility = Visibility.Visible;
            otherShotLabel.FontSize = 12;
            otherShotLabel.HorizontalAlignment = HorizontalAlignment.Center;
            Grid.SetColumn(otherShotLabel, 1);
            Grid.SetRow(otherShotLabel, 1);
            otherlast.Children.Add(otherShotLabel);
        }
      

    }
   
}
