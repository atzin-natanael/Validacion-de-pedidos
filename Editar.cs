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
    public partial class Editar : Form
    {
        public delegate void EnviarVariableDelegate2(decimal cantidad, int id);
        public event EnviarVariableDelegate2 EnviarVariableEvent2;
        public Editar()
        {
            InitializeComponent();
        }
        public void FuncionEditar(string codigo, string descripcion, decimal solicitado, decimal recibido, int id)
        {
            flowLayoutPanel1.Controls.Add(LbCodigo);
            LbCodigo.Text = codigo + " " + descripcion;
            LbSolicitado.Text = solicitado.ToString();
            LbRecibido.Text = recibido.ToString();
            if (recibido > solicitado)
                LbRecibido.BackColor = Color.Red;
            Cantidad.Select(0, Cantidad.Value.ToString().Length);
            Cantidad.Focus();
            GlobalSettings.Instance.Id = id;
        }
        private void Cantidad_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                GlobalSettings.Instance.Editar = true;
                decimal mul = Cantidad.Value;
                EnviarVariableEvent2(Cantidad.Value, GlobalSettings.Instance.Id);
                this.Close();
            }
            if (e.KeyCode == Keys.Escape)
            {
                this.Close();
            }
        }
    }
}
