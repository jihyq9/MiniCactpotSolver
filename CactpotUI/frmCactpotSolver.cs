using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace CactpotUI
{
    public partial class frmCactpotSolver : Form
    {
        private MiniCactpot _cactpot;
        public frmCactpotSolver()
        {
            InitializeComponent();
            _cactpot = new MiniCactpot();
        }

        private void cboSquare_SelectedIndexChanged(object sender, EventArgs e)
        {
            var cbo = sender as ComboBox;
            int square = 0;
            int val = 0;
            bool success = false;
            try {
                square = (int)cbo.Tag;
                val = (int)cbo.SelectedItem;
                success = true;
            } catch (Exception) { }
            // Display the values of choices
            if (success) {
                _cactpot.Choose(square, val);
                Label maxLabel = lblVal0;
                double maxVal = 0;
                Label[] labels = { lblVal0, lblVal1, lblVal2, lblVal3, lblVal4, lblVal5, lblVal6, lblVal7, lblVal8 };
                for (int i = 0; i < labels.Length; i++) {
                    labels[i].BackColor = SystemColors.Control;
                    var choiceVal = _cactpot.ChosenSquareValue(i);
                    labels[i].Text = Math.Round(choiceVal).ToString();
                    if (choiceVal > maxVal) {
                        maxLabel = labels[i];
                        maxVal = choiceVal;
                    }
                }
                maxLabel.BackColor = Color.LightGreen;
            }
            // Display line values if no choices left
            int choicesRemaining = _cactpot.ChoicesRemaining;
            if (choicesRemaining == 0) {
                Label maxLabel = lblLine0;
                double maxVal = 0;
                var lineVals = _cactpot.LineValues().ToArray();
                Label[] labels = { lblLine0, lblLine1, lblLine2, lblLine3, lblLine4, lblLine5, lblLine6, lblLine7 };
                for (int i = 0; i < labels.Length; i++) {
                    labels[i].BackColor = SystemColors.Control;
                    var choiceVal = lineVals[i];
                    labels[i].Text = Math.Round(choiceVal).ToString();
                    if (choiceVal > maxVal) {
                        maxLabel = labels[i];
                        maxVal = choiceVal;
                    }
                }
                maxLabel.BackColor = Color.LightGreen;
            }
        }
    }
}
