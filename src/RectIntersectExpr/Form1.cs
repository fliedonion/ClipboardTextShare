using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RectIntersectExpr {
    public partial class Form1 : Form {
        public Form1() {
            InitializeComponent();

            label1.Paint += label1_Paint;
        }

        void label1_Paint(object sender, PaintEventArgs e) {


            if (rect1 != null) {
                if (checkBox1.Checked) {
                    e.Graphics.FillRectangle(Brushes.Blue, rect1.Value);
                }
                else {
                    e.Graphics.DrawRectangle(Pens.Blue, rect1.Value);
                }

            }

            if (rect2 != null) {
                if (checkBox1.Checked) {
                    e.Graphics.FillRectangle(Brushes.Red, rect2.Value);
                }
                else {
                    e.Graphics.DrawRectangle(Pens.Red, rect2.Value);
                }
            }


        }

        private Rectangle? rect1 = null;
        private Rectangle? rect2 = null;

        private void button1_Click(object sender, EventArgs e) {

            rect1 = CreateRect(textBox1.Text);
            rect2 = CreateRect(textBox2.Text);


            if (rect1 == null && rect2 == null) {
                return;
            }

            label1.Invalidate();

        }



        private Rectangle? CreateRect(string csvXYWH) {

            try {
                if (string.IsNullOrWhiteSpace(csvXYWH)) return null;
                var points = csvXYWH.Split(',').Select(x => int.Parse(x.Trim())).ToList();

                return new Rectangle(points[0], points[1], points[2], points[3]);
            }
            catch (Exception ex) {
                return null;
            }


            return null;
        }

        private void button2_Click(object sender, EventArgs e) {

            rect1 = CreateRect(textBox1.Text);
            rect2 = CreateRect(textBox2.Text);


            if (rect1 != null && rect2 != null) {
                var r = rect1.Value;

                r.Intersect(rect2.Value);
                rect1 = r;
                rect2 = null;
            }
            else {
                return;
            }

            label1.Invalidate();

        }
    }
}
