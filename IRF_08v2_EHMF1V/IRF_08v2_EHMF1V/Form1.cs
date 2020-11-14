using IRF_08v2_EHMF1V.Abstraction;
using IRF_08v2_EHMF1V.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IRF_08v2_EHMF1V
{
    public partial class Form1 : Form
    {
        private List<Abstraction.Toy> _toys = new List<Abstraction.Toy>();
        private Abstraction.Toy _nextToy;
        private IToyFactory _factory;

        public IToyFactory Factory
        {
            get { return _factory; }
            set 
            {
                _factory = value;
                DisplayNext();
            }
        }

        public Form1()
        {
            InitializeComponent();

            Factory = new BallFactory();
        }

        private void createTimer_Tick(object sender, EventArgs e)
        {
            var toy = Factory.CreateNew();
            _toys.Add(toy);
            toy.Left = -toy.Width;
            mainPanel.Controls.Add(toy);
        }

        private void conveyorTimer_Tick(object sender, EventArgs e)
        {
            var maxPosition = 0;
            foreach (var b in _toys)
            {
                b.MoveToy();
            }

            if (maxPosition>1000)
            {
                var oldestToy = _toys[0];
                mainPanel.Controls.Remove(oldestToy);
                _toys.Remove(oldestToy);
            }

        }

        private void carButton_Click(object sender, EventArgs e)
        {
            Factory = new CarFactory();
        }

        private void ballButton_Click(object sender, EventArgs e)
        {
            Factory = new BallFactory();
        }

        private void DisplayNext()
        {
            if (_nextToy != null) Controls.Remove(_nextToy);

            _nextToy = Factory.CreateNew();
            _nextToy.Top = nextLabel.Top + nextLabel.Height + 20;
            _nextToy.Left = nextLabel.Left;
            Controls.Add(_nextToy);
        }

        private void colorButton_Click(object sender, EventArgs e)
        {
            var button = (Button)sender;
            var colorPicker = new ColorDialog();
            colorPicker.Color = button.BackColor;
            if (colorPicker.ShowDialog() != DialogResult.OK) return;
            button.BackColor = colorPicker.Color;
        
        }
    }
}
