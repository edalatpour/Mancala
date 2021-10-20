using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Xna.Framework.Audio;

namespace Mancala
{
    public partial class CupControl : UserControl
    {

        private StoneBag _cup;
        private string _text = "";
        private SoundEffect[] _se = new SoundEffect[21];

        public event EventHandler Click;

        public CupControl()
        {
            InitializeComponent();
        }

        public int CupIndex { get; set; }

        public StoneBag Cup
        {
            get
            {
                return (_cup);
            }
            set
            {
                _cup = value;
                _cup.OnContentsChanged += this.OnContentsChanged;
            }
        }

        
        public string Text
        {
            get
            {
                return (_text);
            }
            set
            {
                _text = value;
            }
        }

        public System.Windows.Media.ImageSource ImageSource
        {
            get
            {
                return (MyImage.Source);
            }
            set
            {
                MyImage.Source = value;
            }
        }

        public void OnContentsChanged(object sender, EventArgs e)
        {

            StoneBag cup = (StoneBag)sender;
            DrawCupContents();

            Random rnd = new Random();
            int soundFileIndex = (int)rnd.Next(9);
            String soundFileName = String.Format(@"Assets\UI_Clicks{0:00}.wav", soundFileIndex + 1);

            if (_se[soundFileIndex] == null)
            {
                Stream stream = Microsoft.Xna.Framework.TitleContainer.OpenStream(soundFileName);
                _se[soundFileIndex] = SoundEffect.FromStream(stream);
            }
            _se[soundFileIndex].Play();
        }

        public void DrawCupContents()
        {

            Stone[] stones = _cup.Stones;

            leftStack.Children.Clear();
            centerStack.Children.Clear();
            rightStack.Children.Clear();

            countLabel.Text = stones.Length.ToString();

            if (stones.Length > 0)
                this.IsEnabled = true;
            else
                this.IsEnabled = false;

            for (int i = stones.Length - 1; i >= 0; i--)
            {
                Stone stone = stones[i];

                Rectangle rect = new Rectangle();
                rect.HorizontalAlignment = System.Windows.HorizontalAlignment.Center;
                rect.VerticalAlignment = System.Windows.VerticalAlignment.Bottom;
                rect.Width = 28; //centerStack.ActualWidth;
                rect.Height = 28; // centerStack.ActualWidth;
                rect.RadiusX = 5;
                rect.RadiusY = 5;
                Color strokeColor = stone.Color;
                Brush strokeBrush = new SolidColorBrush(strokeColor);
                //strokeBrush.Opacity = 0.2;
                rect.Stroke = strokeBrush;
                rect.StrokeThickness = 5;
                Color fillColor = stone.Color;
                Brush fillBrush = new SolidColorBrush(fillColor);
                fillBrush.Opacity = 0.5;
                rect.Fill = fillBrush;
                int stackIndex = i % 3;
                if (stackIndex == 0)
                    centerStack.Children.Add(rect);
                else if (stackIndex == 1)
                    leftStack.Children.Add(rect);
                else if (stackIndex == 2)
                    rightStack.Children.Add(rect);

            }
        }

        //private void UserControl_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        //{
        //    if (Click != null)
        //    {
        //        Click(this, EventArgs.Empty);
        //    }
        //}

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (Click != null)
            {
                Click(this, EventArgs.Empty);
            }
        }

    }

}
