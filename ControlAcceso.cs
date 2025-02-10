using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LiteDB;

namespace Validación_de_Pedidos
{
    public partial class ControlAcceso : Form
    {
        public delegate void EnviarVariableDelegate3();
        public event EnviarVariableDelegate3 EnviarVariableEvent3;
        public delegate void EnviarVariableDelegate5();
        public event EnviarVariableDelegate5 EnviarVariableEvent5;
        public ControlAcceso()
        {
            InitializeComponent();
            TxtContrasenia.Focus();
            TxtContrasenia.Select();
        }

        private void Aceptar_Click(object sender, EventArgs e)
        {
            if (TxtContrasenia.Text != string.Empty)
            {

                using (var db = new LiteDatabase("\\\\SRVPRINCIPAL\\ConfigDB\\USUARIOS_TRASPASOS.db"))
                {
                    //    // Obtener la colección (equivalente a una tabla en SQL)
                    //    var usuarios = db.GetCollection<Usuario>("USUARIOS");

                    //    // Crear un nuevo objeto de usuario
                    //    var listaUsuarios = new[]
                    //{
                    //    new Usuario { Id = 1, UsuarioName = "ATZIN", Password = "9243" },
                    //    new Usuario { Id = 2, UsuarioName = "MARLENE", Password = "120825" },
                    //    new Usuario { Id = 3, UsuarioName = "LAURA LOPEZ", Password = "941083" },
                    //    new Usuario { Id = 4, UsuarioName = "RUBI LOPEZ", Password = "361228" },
                    //    new Usuario { Id = 5, UsuarioName = "JORGE GERENTE", Password = "07160148" },
                    //    new Usuario { Id = 6, UsuarioName = "ABRAHAM SANTIAGO", Password = "25031979" },
                    //    new Usuario { Id = 7, UsuarioName = "JOSE LUIS MEJIA", Password = "191974" },
                    //    new Usuario { Id = 8, UsuarioName = "ANGELICA BELTRAN", Password = "071187" }
                    //};

                    //    // Insertar cada usuario en la base de datos
                    //    foreach (var usuario in listaUsuarios)
                    //    {
                    //        usuarios.Insert(usuario); // Insertar el usuario en la colección
                    //    }
                    //    Console.WriteLine("Usuario insertado correctamente.");
                    //}
                    var usuarios = db.GetCollection<Usuario>("USUARIOS");

                    // Buscar el usuario que coincida con el nombre de usuario ingresado
                    var usuario = usuarios.FindOne(x => x.Password == TxtContrasenia.Text);

                    if (usuario != null)
                    {
                        // Comprobar si la contraseña coincide
                        if (usuario.Password == TxtContrasenia.Text)
                        {
                            this.Close();

                            GlobalSettings.Instance.aceptado = true;
                            GlobalSettings.Instance.Usuario = usuario.UsuarioName;
                            EnviarVariableEvent3();
                        }
                        else
                        {
                            MessageBox.Show("Contraseña incorrecta");
                            TxtContrasenia.Focus();
                            TxtContrasenia.Select(0, TxtContrasenia.TextLength);
                        }
                    }
                    else
                    {
                        MessageBox.Show("Contraseña incorrecta");
                        TxtContrasenia.Focus();
                        TxtContrasenia.Select(0, TxtContrasenia.TextLength);
                    }


                }
            }
            else
            {
                MessageBox.Show("Contraseña incorrecta", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                TxtContrasenia.Focus();
                TxtContrasenia.Select(0, TxtContrasenia.TextLength);
            }
        }

        private void Exit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void TxtContrasenia_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                Aceptar.Focus();
            }
        }

    }
}
