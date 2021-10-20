using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Microsoft.Xna.Framework.Audio;

namespace Mancala
{
    public partial class MancalaPage : PhoneApplicationPage
    {

        private const int NUMBER_OF_STONES_PER_CUP = 4;
        private const int NUMBER_OF_PAUSE_TICKS = 3;

        private int _pauseTicks = NUMBER_OF_PAUSE_TICKS;
        private DispatcherTimer _timer = new DispatcherTimer();

        private SoundEffect _extraMoveSE;
        private SoundEffect _captureSE;
        private SoundEffect _gameOverSE;

        private Game Game
        {
            get
            {
                System.Windows.Application theApplication = Application.Current;
                Mancala.App theApp = (Mancala.App)theApplication;
                Game game = theApp.Game;
                return (game);
            }
        }

        public MancalaPage()
        {
            InitializeComponent();
            SetupCupBinding();
            _timer.Tick += GameLoop;
            if (Game.Activated)
            {
                theirHomeCup.DrawCupContents();
                their1Cup.DrawCupContents();
                their2Cup.DrawCupContents();
                their3Cup.DrawCupContents();
                their4Cup.DrawCupContents();
                their5Cup.DrawCupContents();
                their6Cup.DrawCupContents();
                myHomeCup.DrawCupContents();
                my1Cup.DrawCupContents();
                my2Cup.DrawCupContents();
                my3Cup.DrawCupContents();
                my4Cup.DrawCupContents();
                my5Cup.DrawCupContents();
                my6Cup.DrawCupContents();
                _timer.Interval = new TimeSpan(0, 0, 0, 0, 300); // fast for setting up board
                _timer.Start();
            }
            else
            {
                StartNewGame();
            }
         }

        private void StartNewGame()
        {
            _timer.Stop();
            Game.InGame = false;
            Game.InMove = false;
            Game.InGrab = false;
            Game.Board.CurrentCupIndex = -1;
            foreach (StoneBag cup in Game.Board.Cups)
            {
                cup.TakeAll();
            }
            FillBag();
            Game.MyTurn = true;
            _timer.Interval = new TimeSpan(0, 0, 0, 0, 100); // fast for setting up board
            _timer.Start();
        }

        private void SetupCupBinding()
        {
            myHomeCup.Cup = Game.Board.Cups[0];
            my1Cup.Cup = Game.Board.Cups[1];
            my2Cup.Cup = Game.Board.Cups[2];
            my3Cup.Cup = Game.Board.Cups[3];
            my4Cup.Cup = Game.Board.Cups[4];
            my5Cup.Cup = Game.Board.Cups[5];
            my6Cup.Cup = Game.Board.Cups[6];
            theirHomeCup.Cup = Game.Board.Cups[7];
            their1Cup.Cup = Game.Board.Cups[8];
            their2Cup.Cup = Game.Board.Cups[9];
            their3Cup.Cup = Game.Board.Cups[10];
            their4Cup.Cup = Game.Board.Cups[11];
            their5Cup.Cup = Game.Board.Cups[12];
            their6Cup.Cup = Game.Board.Cups[13];
        }

        void FillBag()
        {

            AddStonesToBag(Colors.Red);
            AddStonesToBag(Colors.Orange);
            AddStonesToBag(Colors.Yellow);
            AddStonesToBag(Colors.Green);
            AddStonesToBag(Colors.Blue);
            AddStonesToBag(Colors.Purple);

        }

        void AddStonesToBag(Color stoneColor)
        {
            for (int i = 0; i < 8; i++)
            {
                Stone newStone = new Stone();
                newStone.Color = stoneColor;
                Game.Bag.Add(newStone);
            }
        }

        private void GameLoop(object sender, EventArgs e)
        {

            if (!Game.InGame)
            {
                if (!Game.Bag.IsEmpty())
                {
                    SetupBoard();
                }
                else
                {
                    Game.Board.CurrentCupIndex = -1;
                    Game.InGame = true;
                    ShowWhoseTurn();
                }
            }
            else
            {
                _timer.Interval = new TimeSpan(0, 0, 0, 0, 300); // slow down for normal play

                if (Game.InMove)
                {
                    DoMove();
                }
                else if (Game.InGrab)
                {
                    DoGrab();
                }
                else if (Game.Board.CurrentCupIndex != -1)
                {
                    AfterMove();
                }
                else if (!Game.MyTurn)
                {
                    if (_pauseTicks > 0)
                    {
                        _pauseTicks--;
                    }
                    else
                    {
                        _pauseTicks = NUMBER_OF_PAUSE_TICKS;
                        DoTheirMove();
                    }
                }

            }

        }

        void SetupBoard()
        {

            this.IsEnabled = false;

            if (Game.Board.CurrentCupIndex == -1)
            {
                Game.Board.CurrentCupIndex = 8;
            }
            else
            {
                if (Game.Board.CurrentCup().Count == 4)
                {
                    if (Game.Board.CurrentCupIndex == 8) Game.Board.CurrentCupIndex = 6;
                    else if (Game.Board.CurrentCupIndex == 6) Game.Board.CurrentCupIndex = 9;
                    else if (Game.Board.CurrentCupIndex == 9) Game.Board.CurrentCupIndex = 5;
                    else if (Game.Board.CurrentCupIndex == 5) Game.Board.CurrentCupIndex = 10;
                    else if (Game.Board.CurrentCupIndex == 10) Game.Board.CurrentCupIndex = 4;
                    else if (Game.Board.CurrentCupIndex == 4) Game.Board.CurrentCupIndex = 11;
                    else if (Game.Board.CurrentCupIndex == 11) Game.Board.CurrentCupIndex = 3;
                    else if (Game.Board.CurrentCupIndex == 3) Game.Board.CurrentCupIndex = 12;
                    else if (Game.Board.CurrentCupIndex == 12) Game.Board.CurrentCupIndex = 2;
                    else if (Game.Board.CurrentCupIndex == 2) Game.Board.CurrentCupIndex = 13;
                    else if (Game.Board.CurrentCupIndex == 13) Game.Board.CurrentCupIndex = 1;
                    else if (Game.Board.CurrentCupIndex == 1) Game.Board.CurrentCupIndex = -1;
                }
            }

            if (Game.Board.CurrentCupIndex != -1)
            {
                Stone pulledStone = Game.Bag.TakeOne();
                Game.Board.CurrentCup().Add(pulledStone);
                if (Game.Bag.IsEmpty())
                {
                    Game.Board.CurrentCupIndex = -1;
                    this.IsEnabled = true;
                }
            }

        }

        private void StatusMessage(string message)
        {
            Color color;
            if (Game.MyTurn)
            {
                color = Colors.Green;
            }
            else
            {
                color = Colors.Red;
            }
            StatusMessage(message, color);

        }

        private void StatusMessage(string message, Color color)
        {
            statusBlock.Color = color;
            statusBlock.Text = message;
            statusBlock.Visibility = System.Windows.Visibility.Visible;

        }
        private void DoMove()
        {
            if (Game.Board.CurrentCupIndex != -1)
            {
                if (Game.Hand.IsEmpty())
                {
                    StoneBag cup = Game.Board.CurrentCup();
                    if (cup.Count > 0)
                    {
                        Game.Hand.Add(cup.TakeAll());
                    }
                }
                else
                {
                    Game.Board.NextCup(Game.MyTurn);
                    Game.Board.CurrentCup().Add(Game.Hand.TakeOne());
                    if (Game.Hand.IsEmpty())
                    {
                        Game.InMove = false;
                    }
                }
            }
        }

        private void DoGrab()
        {

            if (Game.Board.CurrentCupIndex != -1)
            {
                if (Game.Hand.IsEmpty())
                {
                    StoneBag cup = Game.Board.CurrentCup();
                    if (cup.Count > 0)
                    {
                        Game.Hand.Add(cup.TakeAll());
                    }
                    int oppositeCupIndex = Game.Board.OppositeCupIndex();
                    StoneBag oppositeCup = Game.Board.Cups[oppositeCupIndex];
                    if (oppositeCup.Count > 0)
                    {
                        Game.Hand.Add(oppositeCup.TakeAll());
                    }                
                }
                else
                {
                    StoneBag homeCup;
                    if (Game.MyTurn)
                    {
                        homeCup = Game.Board.Cups[0];
                    }
                    else
                    {
                        homeCup = Game.Board.Cups[7];
                    }

                    homeCup.Add(Game.Hand.TakeOne());
                    if (Game.Hand.IsEmpty())
                    {
                        Game.InGrab = false;
                    }
                }

            }

        }

        private void AfterMove()
        {

            if (CheckForCapture())
            {
                StatusMessage("CAPTURE");
                PlayCaptureSound();
                StartGrab(Game.Board.CurrentCupIndex);
            }
            else
            {
                if (CheckForGameOver())
                {
                    // Do some winning stuff!
                    if (myHomeCup.Cup.Count > theirHomeCup.Cup.Count)
                    {
                       StatusMessage("YOU WIN!", Colors.Green);
                    }
                    if (myHomeCup.Cup.Count < theirHomeCup.Cup.Count)
                    {
                        StatusMessage("YOU LOSE!", Colors.Red);
                    }
                    if (myHomeCup.Cup.Count == theirHomeCup.Cup.Count)
                    {
                        StatusMessage("YOU TIED!", Colors.Orange);
                    }
                    PlayGameOverSound();
                }
                else if (CheckForExtraTurn())
                {
                    StatusMessage("EXTRA MOVE");
                    PlayExtraMoveSound();
                }
                else
                {
                    Game.MyTurn = !Game.MyTurn;
                    ShowWhoseTurn();
                }
                Game.Board.CurrentCupIndex = -1;
            }

        }

        private void PlayExtraMoveSound()
        {
            if (_extraMoveSE == null)
            {
                //Microsoft.Xna.Framework.FrameworkDispatcher.Update();
                Stream stream = Microsoft.Xna.Framework.TitleContainer.OpenStream(@"Assets\Windows User Account Control.wav");
                _extraMoveSE = SoundEffect.FromStream(stream);
            }
            _extraMoveSE.Play();
        }

        private void PlayCaptureSound()
        {
            if (_captureSE == null)
            {
                //Microsoft.Xna.Framework.FrameworkDispatcher.Update();
                Stream stream = Microsoft.Xna.Framework.TitleContainer.OpenStream(@"Assets\Windows User Account Control.wav");
                _captureSE = SoundEffect.FromStream(stream);
            }
            _captureSE.Play();
        }

        private void PlayGameOverSound()
        {
            if (_gameOverSE == null)
            {
                //Microsoft.Xna.Framework.FrameworkDispatcher.Update();
                Stream stream = Microsoft.Xna.Framework.TitleContainer.OpenStream(@"Assets\tada.wav");
                _gameOverSE = SoundEffect.FromStream(stream);
            }
            _gameOverSE.Play();
        }

        private void ShowWhoseTurn()
        {
            if (Game.MyTurn)
                StatusMessage("YOUR MOVE");
            else
                StatusMessage("THEIR MOVE");
        }

        private void OnCupClicked(object sender, EventArgs e)
        {
            if (Game.MyTurn && !Game.InMove)
            {
                CupControl ctl = (CupControl)sender;
                StartMove(ctl.CupIndex);
            }
        }

        private void StartMove(int cupIndex)
        {
            statusBlock.Visibility = System.Windows.Visibility.Collapsed;
            //adBlock.Visibility = System.Windows.Visibility.Visible; 
            Game.InMove = true;
            Game.Board.CurrentCupIndex = cupIndex;
        }

        private void StartGrab(int cupIndex)
        {
            Game.InGrab = true;
            Game.Board.CurrentCupIndex = cupIndex;
        }

        private bool CheckForExtraTurn()
        {
            bool playersHomeCup = false;
            if (Game.MyTurn)
            {
                if (Game.Board.CurrentCupIndex == 0)
                    playersHomeCup = true;
            }
            else
            {
                if (Game.Board.CurrentCupIndex == 7)
                    playersHomeCup = true;
            }
            return (playersHomeCup);
        }

        private bool CheckForCapture()
        {
            bool playersCup = false;
            bool emptyCup = false;
            bool capture = false;
            if (Game.MyTurn)
            {
                if (Game.Board.CurrentCupIndex >= 1 &&
                    Game.Board.CurrentCupIndex <= 6)
                    playersCup = true;
            }
            else
            {
                if (Game.Board.CurrentCupIndex >= 8 &&
                    Game.Board.CurrentCupIndex <= 13)
                    playersCup = true;
            }
            // Figure out the cup across from this one.
            if ((Game.Board.CurrentCupIndex != -1) && (Game.Board.CurrentCup().Count == 1))
            {
                emptyCup = true;
            }
            if (playersCup && emptyCup)
            {
                int oppositeCupIndex = Game.Board.OppositeCupIndex();
                StoneBag oppositeCup = Game.Board.Cups[oppositeCupIndex];
                if (oppositeCup.Count > 0)
                {
                    capture = true;
                }
            }
            return (capture);
        }

        private void DoTheirMove()
        {
            int cupToMoveIndex = -1;

            for(int i = 1; i <=6; i++)
            {
                int cupIndex = 7 + i;
                StoneBag cup = Game.Board.Cups[cupIndex];
                if(cup.Count == i)
                {
                    cupToMoveIndex = cupIndex;
                    break;
                }
            }

            if(cupToMoveIndex == -1)
            {
                for(int i = 1; i <=6; i++)
                {
                    int cupIndex = 7 + i;
                    StoneBag cup = Game.Board.Cups[cupIndex];
                    if(cup.Count > 0)
                    {
                        cupToMoveIndex = cupIndex;
                        break;
                    }
                }
            }

            if (cupToMoveIndex != -1)
            {
                StartMove(cupToMoveIndex);
            }
        }

        private bool CheckForGameOver()
        {
            int totalStonesOnMySide = 0;
            int totalStonesOnTheirSide = 0;

            totalStonesOnMySide = Game.Board.Cups[1].Count +
                                    Game.Board.Cups[2].Count +
                                    Game.Board.Cups[3].Count +
                                    Game.Board.Cups[4].Count +
                                    Game.Board.Cups[5].Count +
                                    Game.Board.Cups[6].Count;

            totalStonesOnTheirSide = Game.Board.Cups[8].Count +
                                           Game.Board.Cups[9].Count +
                                           Game.Board.Cups[10].Count +
                                           Game.Board.Cups[11].Count +
                                           Game.Board.Cups[12].Count +
                                           Game.Board.Cups[13].Count;

            if ((totalStonesOnMySide == 0) || (totalStonesOnTheirSide == 0))
                return (true);
            else
                return (false);
        }

        private void newGameButton_Click(object sender, RoutedEventArgs e)
        {
            //System.Windows.Application thisApp = Application.Current;
            //Mancala.App thisMancalaApp = (Mancala.App)thisApp;
            ////if (!thisMancalaApp.CheckTrialExpired())
            //{
            //    StartNewGame();
            //}

            statusBlock.Visibility = System.Windows.Visibility.Collapsed;
            //adBlock.Visibility = System.Windows.Visibility.Visible;
            StartNewGame();
       
        }

        private void helpButton_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.Navigate(new Uri("/HelpPage.xaml", UriKind.Relative));
        }

    }

}