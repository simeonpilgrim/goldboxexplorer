using System.Drawing;
using System.Windows.Forms;

namespace GoldBoxExplorer.Lib.Plugins
{
    public static class ViewerHelper
    {
        static readonly Font normalFont = new Font("Courier New", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);

        public static Control CreateTextBox(bool readonlystatus = true)
        {
            var control = new TextBox {
                Dock = DockStyle.Left,
                //Anchor = AnchorStyles.Left,
                //Top = 0,
                //Left = 0,
                //Multiline = true,
                //ReadOnly = readonlystatus,
                ReadOnly = true,
                BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D,
                Font = normalFont,
                //ScrollBars = ScrollBars.Both,
            };
            return control;
        }

        public static Control CreateTextBoxMultiline(bool readonlystatus = true)
        {
            var control = new TextBox {
                Dock = DockStyle.Fill,
                //Anchor = AnchorStyles.Left,
                //Top = 0,
                //Left = 0,
                Multiline = true,
                //ReadOnly = readonlystatus,
                ReadOnly = true,
                BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D,
                Font = normalFont,
                ScrollBars = ScrollBars.Both,
            };

            return control;
        }

        public static Control CreateButton()
        {
            var control = new Button
            {
                Dock = DockStyle.Top,
                Top = 0,
                Left = 0,
                Height = 20,
                Width = 200,
                Font = normalFont,
            };

            return control;
        }
        public static Control CreatePanel()
        {
            var control = new Panel
            {
                Dock = DockStyle.Fill,
                Top = 0,
                Left = 0,
                Font = normalFont,
                AutoScroll = true,
            };

            return control;
        }
        public static Control CreateRow()
        {
            var control = new Panel
            {
                Dock = DockStyle.Top,
             
                Top = 0,
                Left = 0,
                Height = 24,
                BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D,

                Font = normalFont,
            };

            return control;
        }
    }
}