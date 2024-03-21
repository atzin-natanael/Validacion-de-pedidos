using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Validación_de_Pedidos
{
    public partial class Mensaje : Form
    {
        private bool dragging = false;
        private Point dragCursorPoint;
        private Point dragFormPoint;
        public Mensaje()
        {
            InitializeComponent();
            Btn_Aceptar.Focus();
        }
        public void SetMensaje(string mensaje, string valor)
        {
            if (valor == "ubicacion")
                LblTitulo.Text = "Ubicación";
            else if (valor == "ubicacion2")
            {
                LblTitulo.Text = "Ubicación";
                //label1.Location = new Point(70, 54);
            }
            else if (valor == "nota")
            {
                LblTitulo.Text = "Nota";
            }
            else if (valor == "nota2")
            {
                LblTitulo.Text = "Nota";
                //label1.Location = new Point(83, 54);
            }
            else if (valor == "existencia")
                LblTitulo.Text = "Existencia";
            richTextBox1.Text = mensaje;
            //label1.Anchor = AnchorStyles.None; // Centrar el control dentro del FlowLayoutPanel
            //label1.AutoSize = true; // Para que ocupe todo el espacio dentro del FlowLayoutPanel
            //label1.Location = new Point((flowLayoutPanel1.ClientSize.Width - label1.Width) / 2, label1.Location.Y);
            //flowLayoutPanel1.Controls.Add(LblTexto);
            //flowLayoutPanel1.Controls.Add(Btn_Aceptar);
            //label2.AutoSize = true; // Para que ocupe todo el espacio dentro del FlowLayoutPanel
            //panel1.Controls.Add(LblTitulo);
            //label2.Anchor = AnchorStyles.None; // Centrar el control dentro del FlowLayoutPanel
            //LblTexto.AutoSize = true;
            //int labelHeight = LblTexto.Height;

            //flowLayoutPanel1.AutoSize = true;
            //flowLayoutPanel1.Height = labelHeight;

            //// Establece la altura del formulario
            //this.Height = flowLayoutPanel1.Height;

            // Asegura que el ancho del formulario se ajuste automáticamente (si es necesario)
            //this.AutoSize = true;
        }

        private void Btn_Aceptar_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            if (dragging)
            {
                Point dif = Point.Subtract(Cursor.Position, new Size(dragCursorPoint));
                this.Location = Point.Add(dragFormPoint, new Size(dif));
            }
        }

        private void panel1_MouseUp(object sender, MouseEventArgs e)
        {
            dragging = false;
        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                dragging = true;
                dragCursorPoint = Cursor.Position;
                dragFormPoint = this.Location;
            }
        }
    }
}
