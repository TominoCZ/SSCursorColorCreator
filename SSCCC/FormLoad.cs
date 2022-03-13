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
    public partial class FormLoad : Form
    {
        public CursorColorData Result;

        public FormLoad()
        {
            InitializeComponent();
        }

        private bool GetData(out CursorColorData result)
        {
            result = null;

            var data = rtbInput.Text.Trim();
            var splits = data.Split(';').Select(s => s.Trim()).ToArray();

            if (splits.Length < 5)
                return false;

            var name = splits[0];
            var creator = splits[1];
            var colors = string.Join("\n", splits[2].Split('|'));

            if (!int.TryParse(splits[3], out var rarity) || !int.TryParse(splits[4], out var price))
                return false;

            result = new CursorColorData
            {
                Colors = colors,
                Name = name,
                Creator = creator,
                Price = price,
                Rarity = rarity
            };

            return true;
        }

        private void rtbInput_TextChanged(object sender, EventArgs e)
        {
            var ok = GetData(out Result);

            btnLoad.Enabled = ok;
            btnLoad.Text = ok ? "LOAD" : "[INVALID DATA]";
        }

        private void btnLoad_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;

            Close();
        }
    }
}
