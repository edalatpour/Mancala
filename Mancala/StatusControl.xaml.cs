using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;

namespace Mancala
{
    public partial class StatusControl : UserControl
    {

        private Color _color;
        
        public StatusControl()
        {
            InitializeComponent();
        }

        public Color Color
        {
            get
            {
                return (_color);
            }
            set
            {
                _color = value;
                MyRectangle.Fill = new SolidColorBrush(value);
            }
        }

        public string Text
        {
            get
            {
                return (this.MyTextBlock.Text);
            }
            set
            {
                myStoryboard.Stop();
                this.MyTextBlock.Text = value;
                myStoryboard.Begin();
            }
        }

    }
}
