using System;
using System.CodeDom.Compiler;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Validación_de_Pedidos
{
    public partial class MensajeCustom : Form
    {
        public MensajeCustom()
        {
            InitializeComponent();
            GridEx.ClearSelection();
        }
        [DllImport("user32.DLL", EntryPoint = "ReleaseCapture")]
        private extern static void ReleaseCapture();
        [DllImport("user32.DLL", EntryPoint = "SendMessage")]
        private extern static void SendMessage(System.IntPtr hwnd, int wmsg, int wparam, int lparam);
        private void Back_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Aclaracion_Click(object sender, EventArgs e)
        {
            ControlAcceso control = new ControlAcceso();
            control.EnviarVariableEvent3 += new ControlAcceso.EnviarVariableDelegate3(ejecutar);
            control.ShowDialog();
        }
        public void ejecutar()
        {
            if (GlobalSettings.Instance.aceptado == true)
            {
                this.Close();
            }
        }

        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            ReleaseCapture();
            SendMessage(this.Handle, 0x112, 0xf012, 0);
        }
    }
}
