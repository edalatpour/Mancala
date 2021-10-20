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

namespace Mancala
{
    public class Board
    {
        public StoneBag[] Cups = new StoneBag[14];
        public int CurrentCupIndex = -1;

        public Board()
        {
            for(int i = 0; i < 14; i++)
            {
                Cups[i] = new StoneBag();
            }
        }

        public StoneBag CurrentCup()
        {
            if ((CurrentCupIndex >= 0) && (CurrentCupIndex <= 13))
            {
                StoneBag cup = Cups[CurrentCupIndex];
                return (cup);
            }
            else
            {
                return (null);
            }
        }

        public void NextCup(bool myTurn)
        {
            if (CurrentCupIndex > 0)
            {
                CurrentCupIndex = CurrentCupIndex - 1;
            }
            else
            {
                CurrentCupIndex = 13;
            }
            if (CurrentCupIndex == 0) 
            {
                if (!myTurn)
                {
                    CurrentCupIndex = 13;
                }
           }
            if (CurrentCupIndex == 7)
            {
                if (myTurn)
                {
                    CurrentCupIndex = 6;
                }
            }
        }

        public int OppositeCupIndex()
        {
            int oppositeCupIndex = 14 - CurrentCupIndex;

            return (oppositeCupIndex);
        }

    }

}
