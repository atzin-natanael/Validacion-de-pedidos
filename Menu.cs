using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Validación_de_Pedidos
{
    public partial class Menu : Form
    {
        public delegate void EnviarVariableDelegate(decimal cantidad, int id);
        public event EnviarVariableDelegate EnviarVariableEvent;
        public Menu()
        {
            InitializeComponent();
        }
        public void FuncionRecibir(string codigo, string descripcion, decimal solicitado, decimal recibido, int id, string Nota)
        {
            flowLayoutPanel1.Controls.Add(LbCodigo);
            flowLayoutPanel2.Controls.Add(Lb_Nota);
            LbCodigo.Text = codigo + " - " + descripcion;
            LbSolicitado.Text = solicitado.ToString();
            LbRecibido.Text = recibido.ToString();
            Lb_Nota.Text = Nota.ToString();
            if (recibido > solicitado)
                LbRecibido.BackColor = Color.Red;
            LbPendiente.Text = (solicitado - recibido).ToString();
            if (solicitado - recibido < 0)
                LbPendiente.Text = "0";
            Cantidad.Value = GlobalSettings.Instance.Contenido;
            Cantidad.Select(0, Cantidad.Value.ToString().Length);
            Cantidad.Focus();
            GlobalSettings.Instance.Id = id;
        }

        private void Cantidad_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                decimal mul = Cantidad.Value;
                EnviarVariableEvent(Cantidad.Value, GlobalSettings.Instance.Id);
                this.Close();
            }
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
        }
    }
}
