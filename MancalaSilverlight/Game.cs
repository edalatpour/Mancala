using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace MancalaSilverlight
{
    public class Game
    {
        public StoneBag Bag { get; set; }
        public Board Board { get; set; }
        public StoneBag Hand { get; set; }
        public bool InGame { get; set; }
        public bool InMove { get; set; }
        public bool InGrab { get; set; }
        public bool MyTurn { get; set; }
        public bool Activated { get; set; }
        
        public Game()
        {
            Bag = new StoneBag();
            Board = new Board();
            Hand = new StoneBag();
            InGame = false;
            InMove = false;
            InGrab = false;
            Activated = false;
        }

    }

}
