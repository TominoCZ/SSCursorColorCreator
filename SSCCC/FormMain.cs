using SSCCC.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SSCCC
{
    public partial class FormMain : Form
    {
        Settings _settings;

        bool _bypass = true;

        public FormMain()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            _settings = Properties.Settings.Default;
            _settings.Reload();

            rtbInput.Text = _settings.Input;

            nudPrice.Value = _settings.Price;
            cbRarity.SelectedIndex = _settings.Rarity;

            tbName.Text = _settings.Name;
            tbCreator.Text = _settings.Creator;

            _bypass = false;

            UpdateOutput();

            Application.Idle += (_, __) =>
            {
                ccPreview.Invalidate();
            };
        }

        private void UpdateOutput()
        {
            var colors = ccPreview.GetColors();
            if (_bypass || colors == null)
                return;

            rtbOutput.Text = $"{tbName.Text.Trim()}; {tbCreator.Text.Trim()}; { string.Join("|", colors.Select(c => $"{c.R},{c.G},{c.B}")) }; {cbRarity.SelectedIndex}; {(long)nudPrice.Value}";

            _settings.Input = rtbInput.Text;

            _settings.Price = (long)nudPrice.Value;
            _settings.Rarity = cbRarity.SelectedIndex;

            _settings.Name = tbName.Text;
            _settings.Creator = tbCreator.Text;

            _settings.Save();
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            using (var form = new FormLoad())
            {
                if (form.ShowDialog() == DialogResult.OK)
                {
                    var cc = form.Result;

                    rtbInput.Text = cc.Colors;

                    nudPrice.Value = cc.Price;
                    cbRarity.SelectedIndex = Math.Max(0, Math.Min(4, cc.Rarity));

                    tbName.Text = cc.Name;
                    tbCreator.Text = cc.Creator;
                }
            }
        }

        private void rtbInput_TextChanged(object sender, EventArgs e)
        {
            var lines = rtbInput.Text.Split('\n');
            var colors = new List<Color>();

            foreach (var line in lines)
            {
                var values = new List<int>();
                var sb = new StringBuilder();

                for (int i = 0; i < line.Length; i++)
                {
                    var c = line[i];
                    var isNumber = c >= '0' && c <= '9';

                    if (isNumber)
                        sb.Append(c);

                    if (!isNumber || i == line.Length - 1)
                    {
                        var text = sb.ToString();
                        sb.Clear();

                        if (int.TryParse(text, out var value))
                        {
                            values.Add(Math.Max(0, Math.Min(255, value)));

                            if (values.Count >= 3)
                                break;
                        }
                    }
                }

                if (values.Count == 3)
                    colors.Add(Color.FromArgb(values[0], values[1], values[2]));
            }

            ccPreview.SetColors(colors.ToArray());

            UpdateOutput();
        }

        private void ValueChanged(object sender, EventArgs e) => UpdateOutput();
    }
}
