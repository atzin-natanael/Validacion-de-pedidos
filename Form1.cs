using FirebirdSql.Data.FirebirdClient;
using System;
using System.CodeDom.Compiler;
using System.Data;
using System.Drawing;
using System.Drawing.Printing;
using System.Reflection.Metadata;
using System.Windows.Forms;
using System.Xml.Linq;
using iTextSharp.text.pdf;
using Document = iTextSharp.text.Document;
using iTextSharp.text;
using System.Diagnostics;
using SpreadsheetLight;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using DocumentFormat.OpenXml.Spreadsheet;
using System.Runtime.CompilerServices;
using System.Media;
using DocumentFormat.OpenXml.Drawing.Charts;

namespace Validación_de_Pedidos
{
    public partial class Form1 : Form
    {
        List<string> nombresArray = new();
        private List<string> nombresValor = new() { };
        List<Articulo> Articulos = new();
        public Form1()
        {
            InitializeComponent();
            BtnPDF.Enabled = false;
            Leer_Datos();
            Cb_Surtidor.SelectedIndex = -1;
            Cb_Surtidor.DropDownHeight = 250;
            TxtPedido.Select();
        }
        public void Leer_Datos()
        {
            nombresArray.Clear();
            nombresValor.Clear();
            //string filePath = "\\\\192.168.0.2\\C$\\clavesSurtido\\Claves.xlsx";
            string filePath = "\\\\SRVPRINCIPAL\\clavesSurtido\\Claves.xlsx";
            
            using (SLDocument documento = new SLDocument(filePath))
            {
                int filas = documento.GetWorksheetStatistics().NumberOfRows;
                for (int i = 2; i < filas + 1; ++i)
                {
                    string temp_name = documento.GetCellValueAsString("A" + i);
                    string temp_value = documento.GetCellValueAsString("B" + i);
                    string temp_status = documento.GetCellValueAsString("C" + i);
                    string name = temp_name + " " + temp_value;
                    nombresArray.Add(name);
                    nombresValor.Add(temp_status);
                }
                documento.CloseWithoutSaving();
            }
            Cb_Surtidor.DataSource = nombresArray;
            Cb_Surtidor.AutoCompleteMode = AutoCompleteMode.Append;
            Cb_Surtidor.AutoCompleteSource = AutoCompleteSource.CustomSource;
            Cb_Surtidor.AutoCompleteCustomSource.AddRange(nombresArray.ToArray());
            //Cb_Empacador.Text = "";
        }
        static void ReproducirSonido(string ruta)
        {
            try
            {
                using (SoundPlayer soundPlayer = new SoundPlayer(ruta))
                {
                    soundPlayer.Play();
                }
            }
            catch (Exception ex)
            {
            }
        }
        private void BtnPedido_Click(object sender, EventArgs e)
        {
            if (Tabla.Rows.Count > 0)
            {
                ReproducirSonido("C:\\Windows\\Media\\Windows Critical Stop.wav");
                DialogResult result = MessageBox.Show("Primero termina el pedido", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;

            }
            if (TxtPedido.Text == string.Empty)
            {
                ReproducirSonido("C:\\Windows\\Media\\Windows Critical Stop.wav");
                DialogResult result = MessageBox.Show("Pedido no ingresado", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            Articulos.Clear();
            GlobalSettings.Instance.Name = TxtPedido.Text;
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            GlobalSettings.Instance.filepath = desktopPath + "\\" + TxtPedido.Text + ".txt";
            string Folio_Mod = TxtPedido.Text;
            if (Folio_Mod[1] == 'O' || Folio_Mod[1] == 'E')
            {
                int cont = 9 - Folio_Mod.Length;
                string prefix = Folio_Mod.Substring(0, 2);
                string suffix = Folio_Mod.Substring(2);
                string patch = "";
                for (int i = 0; i < cont; i++)
                {
                    patch = patch + "0";
                }
                Folio_Mod = prefix + patch + suffix;
            }
            else if (Folio_Mod[0] == 'P')
            {
                int cont = 9 - Folio_Mod.Length;
                string prefix = Folio_Mod.Substring(0, 1);
                string suffix = Folio_Mod.Substring(1);
                string patch = "";
                for (int i = 0; i < cont; i++)
                {
                    patch = patch + "0";
                }
                Folio_Mod = prefix + patch + suffix;
            }

            FbConnection con = new FbConnection("User=SYSDBA;" + "Password=C0r1b423;" + "Database=D:\\Microsip datos\\PAPELERIA CORIBA CORNEJO.fdb;" + "DataSource=192.168.0.11;" + "Port=3050;" + "Dialect=3;" + "Charset=UTF8;");
            try
            {
                // Crear un nuevo hilo y asignarle un método que se ejecutará en paralelo
                // Iniciar la ejecución del hilo

                con.Open();
                string query0 = "SELECT * FROM DOCTOS_VE WHERE FOLIO = '" + Folio_Mod + "' AND TIPO_DOCTO = 'P';";
                FbCommand command = new FbCommand(query0, con);
                bool Find = false;
                // Objeto para leer los datos obtenidos
                FbDataReader reader0 = command.ExecuteReader();
                if (reader0.Read())
                {
                    GlobalSettings.Instance.status = reader0.GetString(18);
                    GlobalSettings.Instance.FolioId = reader0.GetString(0);
                    GlobalSettings.Instance.Importe_Total = reader0.GetDecimal(26);
                    GlobalSettings.Instance.VendedorId = reader0.GetString(40);
                    Find = true;
                }
                else
                {
                    Find = false;
                }
                reader0.Close();
                if (Find == false)
                {
                    ReproducirSonido("C:\\Windows\\Media\\Windows Critical Stop.wav");
                    MessageBox.Show("FOLIO NO ENCONTRADO");
                    return;
                }
                if (GlobalSettings.Instance.status == "S")
                {
                    ReproducirSonido("C:\\Windows\\Media\\Windows Critical Stop.wav");
                    MessageBox.Show("Este pedido ya está facturado", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                if (GlobalSettings.Instance.status == "C")
                {
                    ReproducirSonido("C:\\Windows\\Media\\Windows Critical Stop.wav");
                    MessageBox.Show("Este pedido está cancelado", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                string query1 = "SELECT * FROM DOCTOS_VE_DET  WHERE DOCTO_VE_ID =" + GlobalSettings.Instance.FolioId + ";";
                FbCommand command1 = new FbCommand(query1, con);
                FbDataReader reader1 = command1.ExecuteReader();

                while (reader1.Read())
                {
                    Articulo variables = new Articulo
                    {
                        Codigo = reader1.GetString(2),
                        ArticuloId = reader1.GetString(3),
                        Descripcion = "0",
                        Solicitado = reader1.GetDecimal(4),
                        Importe_neto_articulo = reader1.GetDecimal(8),
                        Descuento_porcentaje = reader1.GetDecimal(9),
                        Importe_total_articuloeliminado = reader1.GetDecimal(15),
                        Recibido = 0,
                        Nota = reader1.GetString(18),
                        Id = reader1.GetInt32(20),
                        Pendiente = reader1.GetDecimal(4)
                    };
                    Articulos.Add(variables);
                    //GlobalSettings.Instance.ListaContador.Add(variables.Codigo);
                    //int repeticiones = GlobalSettings.Instance.ListaContador.Count(c => c == variables.Codigo);
                    //if (repeticiones > 1)
                    //{
                    //    MessageBox.Show("Codigo repetido: " + variables.Codigo);
                    //    variables.Contador +=1; 
                    //}

                }
                reader1.Close();
                //Inicio vendedor
                string querynew = "SELECT * FROM VENDEDORES WHERE VENDEDOR_ID = '" + GlobalSettings.Instance.VendedorId + "';";
                FbCommand commandonew = new FbCommand(querynew, con);

                // Objeto para leer los datos obtenidos
                FbDataReader readernew = commandonew.ExecuteReader();
                if (readernew.Read())
                {
                    GlobalSettings.Instance.Vendedora = readernew.GetString(1);
                }
                readernew.Close();
                //fin

                string query = "SELECT * FROM CLAVES_ARTICULOS ORDER BY CLAVE_ARTICULO_ID";
                FbCommand commando = new FbCommand(query, con);

                // Objeto para leer los datos obtenidos
                FbDataReader reader = commando.ExecuteReader();
                while (reader.Read())
                {
                    // Acceder a los valores de cada columna por su índice o nombre
                    string temp = reader.GetString(2);
                    string col2 = reader.GetString(1);
                    for (int i = 0; i < Articulos.Count; ++i)
                    {
                        if (Articulos[i].Codigo == col2 && Articulos[i].Clave == null)
                        {
                            GlobalSettings.Instance.OficialCodigo.Add(temp);
                            Articulos[i].Clave = temp.ToString();
                        }

                    }
                }
                reader.Close();
                for (int i = 0; i < Articulos.Count; ++i)
                {
                    string queryprecio = "SELECT * FROM PRECIOS_ARTICULOS WHERE ARTICULO_ID = '" + Articulos[i].Clave + "' AND PRECIO_EMPRESA_ID = '42'";
                    FbCommand commandp = new FbCommand(queryprecio, con);
                    FbDataReader readerp = commandp.ExecuteReader();
                    if (readerp.Read())
                    {
                        //GlobalSettings.Instance.Clave_articulo_id = reader12.GetString(2);
                        Articulos[i].Importe = readerp.GetDecimal(3);
                    }
                    readerp.Close();
                }


                //reader2.Close();
                string query3 = "SELECT * FROM ARTICULOS ORDER BY ARTICULO_ID";
                FbCommand command3 = new FbCommand(query3, con);
                FbDataReader reader3 = command3.ExecuteReader();

                // Iterar sobre los registros y mostrar los valores
                while (reader3.Read())
                {
                    string columna11 = reader3.GetString(0);
                    string descripcion = reader3.GetString(1);
                    decimal CajaCompra = reader3.GetDecimal(12);
                    for (int i = 0; i < Articulos.Count; ++i)
                    {
                        if (columna11 == GlobalSettings.Instance.OficialCodigo[i])
                        {
                            for (int j = 0; j < Articulos.Count; ++j)
                            {
                                if (GlobalSettings.Instance.OficialCodigo[i].ToString() == Articulos[j].Clave)
                                {
                                    Articulos[j].Descripcion = descripcion;
                                }
                            }
                            break;
                        }

                    }

                }

                reader3.Close();

                for (int i = 0; i < Articulos.Count; ++i)
                {
                    GlobalSettings.Instance.Articuloid = Articulos[i].ArticuloId;
                    Ubicacion(i);
                }

                DataGridViewRowCollection rows = Tabla.Rows;
                string comentario;
                for (int i = 0; i < Articulos.Count; ++i)
                {
                    //AL AGREGAR AL PEDIDO
                    if (Articulos[i].Codigo.Length > 6)
                    {
                        FbConnection con3 = new FbConnection("User=SYSDBA;" + "Password=C0r1b423;" + "Database=D:\\Microsip datos\\PAPELERIA CORIBA CORNEJO.fdb;" + "DataSource=192.168.0.11;" + "Port=3050;" + "Dialect=3;" + "Charset=UTF8;");
                        try
                        {
                            con3.Open();
                            string query12 = "SELECT * FROM CLAVES_ARTICULOS WHERE ARTICULO_ID = '" + Articulos[i].Clave + "' AND ROL_CLAVE_ART_ID = '17'";
                            FbCommand command12 = new FbCommand(query12, con);
                            FbDataReader reader12 = command12.ExecuteReader();
                            if (reader12.Read())
                            {
                                //GlobalSettings.Instance.Clave_articulo_id = reader12.GetString(2);
                                Articulos[i].Codigo = reader12.GetString(1);
                            }
                            else
                            {
                                ReproducirSonido("C:\\Windows\\Media\\Windows Critical Stop.wav");
                                MessageBox.Show("Código no encontrado", "¡Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                TxtCodigo.Focus();
                                TxtCodigo.Select(0, TxtCodigo.Text.Length);
                                return;
                            }
                            reader12.Close();


                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Se perdió la conexión :( , contacta a 06 o intenta de nuevo", "¡Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            MessageBox.Show(ex.ToString());
                            return;
                        }
                        finally
                        {
                            con3.Close();
                        }
                    }
                    if (Articulos[i].Nota != "")
                    {
                        comentario = "Ver";
                    }
                    else
                    {
                        comentario = string.Empty;
                    }
                    rows.Add(Articulos[i].Id, Articulos[i].Codigo, Articulos[i].Descripcion, Articulos[i].Solicitado, Articulos[i].Recibido, comentario, Articulos[i].Pendiente);
                    GlobalSettings.Instance.Renglones = Tabla.RowCount;
                    Lb_renglones.Text = GlobalSettings.Instance.Renglones.ToString();
                }
                for (int i = 0; i < Tabla.Columns.Count; i++)
                {
                    if (i != 8)
                    {
                        Tabla.Columns[i].ReadOnly = true;
                    }
                }
                Tabla.Columns[1].DefaultCellStyle.Font = new System.Drawing.Font("Arial", 14.25F, FontStyle.Bold, GraphicsUnit.Point);
                Tabla.Columns[0].DefaultCellStyle.Font = new System.Drawing.Font("Calibri", 12F, FontStyle.Regular, GraphicsUnit.Point);

                //Articulos.Clear();
                GlobalSettings.Instance.OficialCodigo.Clear();
                //Btn_PDF.Enabled = true;
                //Txt_Folio.Text = string.Empty;
                TxtPedido.Enabled = false;
                BtnPedido.Enabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Se perdió la conexión :( , contacta a 06 o intenta de nuevo", "¡Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                MessageBox.Show(ex.ToString());
                return;
            }
            finally
            {
                Tabla.ClearSelection();
                //Colorear();
                con.Close();
                BtnPDF.Enabled = true;
            }
        }

        private void Tabla_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
        }


        private void Tabla_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0 && e.ColumnIndex == 0)
            {
                Tabla.ClearSelection();
                Tabla.Rows[e.RowIndex].Selected = true;
            }
            if (e.ColumnIndex == 5 && e.RowIndex >= 0)
            {
                if (Tabla.Rows[e.RowIndex].Cells[5].Value.ToString() == "Ver")
                {
                    // Accede a la nota directamente desde la propiedad Nota del objeto Articulo asociado a la fila
                    int valorPrimeraColumna = int.Parse(Tabla.Rows[e.RowIndex].Cells[0].Value.ToString());
                    int indice = 0;
                    //for (int i = 0; i < Articulos.Count; i++)
                    //{
                    //    if (Articulos[i].Id == valorPrimeraColumna)
                    //    {
                    //        indice = i; break;
                    //    }
                    //}
                    //string nota = Articulos[indice].Nota;

                    //if (!string.IsNullOrEmpty(nota))
                    //{
                    //    MessageBox.Show(Articulos[indice].Nota, "Nota del artículo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    //}
                    //else
                    //{
                    //    MessageBox.Show("No hay nota disponible para este artículo.", "Sin nota", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    //}
                    for (int i = 0; i < Articulos.Count; ++i)
                    {
                        if (Articulos[i].Id == valorPrimeraColumna)
                        {
                            if (Articulos[i].Nota == "")
                            {
                                var customMessageBox = new Mensaje();
                                customMessageBox.SetMensaje("Artículo sin nota", "nota2");
                                customMessageBox.ShowDialog();
                            }
                            else
                            {
                                var customMessageBox = new Mensaje();
                                customMessageBox.SetMensaje(Articulos[i].Nota + "\n", "nota");
                                customMessageBox.ShowDialog();
                            }
                        }


                    }
                }

            }
        }

        private void TxtCodigo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true; // Evita que se produzca el sonido de Windows al presionar Enter
                BtnCodigo.Focus();
            }
        }
        private void BtnCodigo_Click(object sender, EventArgs e)
        {
            if (Tabla.RowCount > 0)
            {
                if (TxtCodigo.Text != string.Empty)
                {
                    if (TxtCodigo.Text.Length == 6)
                    {
                        bool encontrado = false;
                        //clave
                        List<decimal> LUnidades = new List<decimal>();
                        List<decimal> LRecibido = new List<decimal>();
                        List<int> index = new List<int>();
                        GlobalSettings.Instance.Contador_Codigos = 0;
                        for (int i = 0; i < Articulos.Count; ++i)
                        {
                            //DataGridViewRow fila = Tabla.Rows[i];
                            string valorColumna = Articulos[i].Codigo;
                            if (valorColumna == TxtCodigo.Text)
                            {
                                LUnidades.Add(Articulos[i].Solicitado);
                                LRecibido.Add(Articulos[i].Recibido);
                                index.Add(i);
                                GlobalSettings.Instance.Contador_Codigos += 1;
                                //MessageBox.Show(GlobalSettings.Instance.Contador_Codigos.ToString());
                            }
                        }
                        bool bander = false;
                        if (GlobalSettings.Instance.Contador_Codigos > 1)
                        {
                            bander = true;
                            for (int i = 0; i < index.Count; ++i)
                            {
                                if (LUnidades[i] != LRecibido[i])
                                {
                                    if (i == 0)
                                        bander = false;
                                    GlobalSettings.Instance.Contador_Codigos = index[i];
                                    break;
                                }
                                GlobalSettings.Instance.Contador_Codigos = index[i];
                            }

                        }
                        for (int i = 0; i < Articulos.Count; ++i)
                        {
                            if (bander == true)
                                i = GlobalSettings.Instance.Contador_Codigos;
                            if (Articulos[i].Codigo == TxtCodigo.Text)
                            {
                                FbConnection con4 = new FbConnection("User=SYSDBA;" + "Password=C0r1b423;" + "Database=D:\\Microsip datos\\PAPELERIA CORIBA CORNEJO.fdb;" + "DataSource=192.168.0.11;" + "Port=3050;" + "Dialect=3;" + "Charset=UTF8;");
                                try
                                {
                                    con4.Open();
                                    string query00 = "SELECT * FROM CLAVES_ARTICULOS WHERE CLAVE_ARTICULO = '" + TxtCodigo.Text + "';";
                                    FbCommand command00 = new FbCommand(query00, con4);
                                    FbDataReader reader00 = command00.ExecuteReader();
                                    if (reader00.Read())
                                    {
                                        GlobalSettings.Instance.Contenido = reader00.GetDecimal(4);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    MessageBox.Show("Se perdió la conexión :( , contacta a 06 o intenta de nuevo", "¡Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                    MessageBox.Show(ex.ToString());
                                    return;
                                }
                                finally
                                {
                                    con4.Close();
                                }
                                Tabla.FirstDisplayedScrollingRowIndex = i;
                                GlobalSettings.Instance.Current = i;
                                Tabla.ClearSelection();
                                Tabla.Rows[GlobalSettings.Instance.Current].Cells[0].Selected = true;
                                Tabla.Rows[GlobalSettings.Instance.Current].Cells[1].Selected = true;
                                encontrado = true;
                                //red mode
                                if (BtnPedido.BackColor == System.Drawing.Color.Red || BtnPedido.BackColor == System.Drawing.Color.Fuchsia)
                                {
                                    ejecutar(GlobalSettings.Instance.Contenido, GlobalSettings.Instance.Current);
                                    return;
                                }
                                Menu Control = new Menu();
                                Control.FuncionRecibir(TxtCodigo.Text, Articulos[i].Descripcion, Articulos[i].Solicitado, Articulos[i].Recibido, i, Articulos[i].Nota);
                                Control.EnviarVariableEvent += new Menu.EnviarVariableDelegate(ejecutar);
                                Control.Show();
                                return;
                            }
                        }
                        if (encontrado == false)
                        {
                            //ReproducirSonido("C:\\Windows\\Media\\Windows Critical Stop.wav");
                            //MessageBox.Show("Código no encontrado en el pedido", "¡Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            //TxtCodigo.Focus();
                            //TxtCodigo.Select(0, TxtCodigo.Text.Length);
                            //return;
                            GlobalSettings.Instance.lastchance = true;
                        }
                    }
                    if (TxtCodigo.Text.Length > 6 || GlobalSettings.Instance.lastchance == true)
                    {
                        FbConnection con3 = new FbConnection("User=SYSDBA;" + "Password=C0r1b423;" + "Database=D:\\Microsip datos\\PAPELERIA CORIBA CORNEJO.fdb;" + "DataSource=192.168.0.11;" + "Port=3050;" + "Dialect=3;" + "Charset=UTF8;");
                        try
                        {
                            GlobalSettings.Instance.Contador_Codigos = 0;
                            string template;
                            con3.Open();
                            string query12 = "SELECT * FROM CLAVES_ARTICULOS WHERE CLAVE_ARTICULO = '" + TxtCodigo.Text + "';";
                            FbCommand command121 = new FbCommand(query12, con3);
                            FbDataReader reader121 = command121.ExecuteReader();
                            if (reader121.Read())
                            {
                                //GlobalSettings.Instance.Clave_articulo_id = reader12.GetString(2);
                                template = reader121.GetString(2);
                                GlobalSettings.Instance.Contenido = reader121.GetDecimal(4);
                            }
                            else
                            {
                                ReproducirSonido("C:\\Windows\\Media\\Windows Critical Stop.wav");
                                MessageBox.Show("Código no encontrado", "¡Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                                TxtCodigo.Focus();
                                TxtCodigo.Select(0, TxtCodigo.Text.Length);
                                return;
                            }
                            reader121.Close();
                            string variableclave;
                            string query122 = "SELECT * FROM CLAVES_ARTICULOS WHERE ARTICULO_ID = '" + template + "' AND ROL_CLAVE_ART_ID = '17'";
                            FbCommand command122 = new FbCommand(query122, con3);
                            FbDataReader reader122 = command122.ExecuteReader();
                            if (reader122.Read())
                            {
                                //GlobalSettings.Instance.Clave_articulo_id = reader12.GetString(2);
                                TxtCodigo.Text = reader122.GetString(1);
                                //if (TxtCodigo.Text.Length > 6)
                                //{
                                //    string Cod_Mod = TxtCodigo.Text;
                                //    do
                                //    {
                                //        int tam = Cod_Mod.Length;
                                //        Cod_Mod = Cod_Mod.Remove(tam - 1);
                                //    } while (Cod_Mod.Length > 6);
                                //    TxtCodigo.Text = Cod_Mod;
                                //}

                            }
                            reader122.Close();
                            bool encontrado = false;
                            //clave
                            List<decimal> LUnidades = new List<decimal>();
                            List<decimal> LRecibido = new List<decimal>();
                            List<int> index = new List<int>();
                            for (int i = 0; i < Articulos.Count; ++i)
                            {
                                //DataGridViewRow fila = Tabla.Rows[i];
                                string valorColumna = Articulos[i].Codigo;
                                if (valorColumna == TxtCodigo.Text)
                                {
                                    LUnidades.Add(Articulos[i].Solicitado);
                                    LRecibido.Add(Articulos[i].Recibido);
                                    index.Add(i);
                                    GlobalSettings.Instance.Contador_Codigos += 1;
                                    //MessageBox.Show(GlobalSettings.Instance.Contador_Codigos.ToString());
                                }
                            }
                            bool bander = false;
                            if (GlobalSettings.Instance.Contador_Codigos > 1)
                            {
                                bander = true;
                                for (int i = 0; i < index.Count; ++i)
                                {
                                    if (LUnidades[i] != LRecibido[i])
                                    {
                                        if (i == 0)
                                            bander = false;
                                        GlobalSettings.Instance.Contador_Codigos = index[i];
                                        break;
                                    }
                                    GlobalSettings.Instance.Contador_Codigos = index[i];
                                }

                            }
                            else if (GlobalSettings.Instance.Contador_Codigos == 0)
                            {
                                ReproducirSonido("C:\\Windows\\Media\\Windows Critical Stop.wav");
                                MessageBox.Show("Código no relacionado al pedido", "¡Error!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                TxtCodigo.Focus();
                                TxtCodigo.Select(0, TxtCodigo.Text.Length);
                                return;
                            }

                            for (int i = 0; i < Articulos.Count; ++i)
                            {
                                if (bander == true)
                                    i = GlobalSettings.Instance.Contador_Codigos;
                                if (Articulos[i].Codigo == TxtCodigo.Text)
                                {
                                    encontrado = true;
                                    Menu Control = new Menu();
                                    //MOD
                                    Tabla.FirstDisplayedScrollingRowIndex = i;
                                    GlobalSettings.Instance.Current = i;
                                    Tabla.ClearSelection();
                                    Tabla.Rows[GlobalSettings.Instance.Current].Cells[0].Selected = true;
                                    Tabla.Rows[GlobalSettings.Instance.Current].Cells[1].Selected = true;
                                    //red mode
                                    if (BtnPedido.BackColor == System.Drawing.Color.Red || BtnPedido.BackColor == System.Drawing.Color.Fuchsia)
                                    {
                                        ejecutar(GlobalSettings.Instance.Contenido, GlobalSettings.Instance.Current);
                                        return;
                                    }
                                    Control.FuncionRecibir(TxtCodigo.Text, Articulos[i].Descripcion, Articulos[i].Solicitado, Articulos[i].Recibido, i, Articulos[i].Nota);
                                    Control.EnviarVariableEvent += new Menu.EnviarVariableDelegate(ejecutar);
                                    Control.Show();
                                    return;
                                }
                                //modificar
                            }
                            if (encontrado == false)
                            {
                                ReproducirSonido("C:\\Windows\\Media\\Windows Critical Stop.wav");
                                MessageBox.Show("Código no relacionado al pedido", "¡Error!", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                                TxtCodigo.Focus();
                                TxtCodigo.Select(0, TxtCodigo.Text.Length);
                                return;
                            }

                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show("Se perdió la conexión :( , contacta a 06 o intenta de nuevo", "¡Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                            MessageBox.Show(ex.ToString());
                            return;
                        }
                        finally
                        {
                            con3.Close();
                            GlobalSettings.Instance.lastchance = false;
                        }
                        //MessageBox.Show("Todo en orden");
                    }
                }
            }
        }
        public void ejecutar(decimal cantidad, int id)
        {
            if (GlobalSettings.Instance.Editar == true)
            {
                DialogResult result = MessageBox.Show("¿Estás seguro que deseas editar este artículo?\n ¿Deseas continuar?", "Advertencia", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);
                if (result == DialogResult.Cancel)
                {
                    GlobalSettings.Instance.Editar = false;
                    TxtCodigo.Focus();
                    TxtCodigo.Select(0, TxtCodigo.Text.Length);
                    return;
                }

            }
            else
            {
                if (cantidad + Articulos[id].Recibido > Articulos[id].Solicitado)
                {
                    ReproducirSonido("C:\\Windows\\Media\\Windows Critical Stop.wav");
                    DialogResult result = MessageBox.Show("Te estás pasando la cantidad solicitada \n ¿Deseas continuar?", "Advertencia", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);
                    if (result == DialogResult.Cancel)
                    {
                        TxtCodigo.Focus();
                        TxtCodigo.Select(0, TxtCodigo.Text.Length);
                        return;
                    }
                }
            }
            bool banderaincompleto = false;
            bool temporal = false;
            bool temporal2 = false;
            decimal prueba;
            bool bandera = false;
            bool regresar = false;
            if (GlobalSettings.Instance.Editar == true)
            {
                prueba = Articulos[id].Solicitado - cantidad;
                if (Articulos[id].Pendiente == 0 && prueba != 0)
                    temporal2 = true;
                if (prueba == 0 && Articulos[id].Recibido > 0)
                    temporal = true;
                if (prueba == 0)
                    bandera = true;
                if ((prueba) >= 1 && Articulos[id].Recibido == 0)
                    banderaincompleto = true;
                if (Articulos[id].Recibido > 0 && cantidad == 0)
                    temporal = true;
                if (prueba < 0 && Articulos[id].Recibido > 0)
                    regresar = true;
                if (prueba < 0 && Articulos[id].Recibido == Articulos[id].Solicitado)
                    bandera = true;
                if (prueba < 0 && Articulos[id].Recibido == 0)
                    bandera = true;
                if (prueba == 0 && Articulos[id].Recibido > Articulos[id].Solicitado)
                {
                    bandera = false;
                    temporal = false;
                }
                if (prueba < 0 && Articulos[id].Recibido == Articulos[id].Solicitado)
                {
                    temporal = false;
                    bandera = false;
                    temporal2 = false;
                    regresar = false;
                }
                Articulos[id].Recibido = cantidad;
                Articulos[id].Pendiente = Articulos[id].Solicitado - cantidad;
                GlobalSettings.Instance.Editar = false;

            }
            else
            {
                prueba = Articulos[id].Pendiente - cantidad;
                if (prueba == 0 && Articulos[id].Recibido > 0)
                    temporal = true;
                if (prueba == 0)
                    bandera = true;
                if ((prueba) >= 1 && Articulos[id].Recibido == 0)
                    banderaincompleto = true;
                Articulos[id].Recibido += cantidad;
                Articulos[id].Pendiente -= cantidad;
            }
            if (Articulos[id].Pendiente < 0)
                Articulos[id].Pendiente = 0;
            List<int> ListaTabla = new List<int>();
            if (bandera == true)
            {
                GlobalSettings.Instance.Renglones--;
                Lb_renglones.Text = GlobalSettings.Instance.Renglones.ToString();
                bandera = false;
            }
            if (temporal == true)
            {
                GlobalSettings.Instance.Incompletos--;
                Lb_Incompletos.Text = GlobalSettings.Instance.Incompletos.ToString();
                GlobalSettings.Instance.Renglones++;
                Lb_renglones.Text = GlobalSettings.Instance.Renglones.ToString();
                bandera = false;
            }
            if (regresar == true)
            {
                GlobalSettings.Instance.Incompletos--;
                Lb_Incompletos.Text = GlobalSettings.Instance.Incompletos.ToString();
                regresar = false;
            }
            if (temporal2 == true)
            {
                GlobalSettings.Instance.Incompletos++;
                Lb_Incompletos.Text = GlobalSettings.Instance.Incompletos.ToString();
            }

            //ORDENAR
            for (int i = 0; i < Articulos.Count; ++i)
            {
                ListaTabla.Add(int.Parse(Tabla.Rows[i].Cells[0].Value.ToString()));
            }
            Tabla.Rows.Clear();
            DataGridViewRowCollection rows = Tabla.Rows;
            string comentario;
            for (int i = 0; i < Articulos.Count; ++i)
            {
                int a = 1;
                if (ListaTabla[i] != i + 1)
                {
                    a = ListaTabla[i] - i;
                }
                if (Articulos[ListaTabla[i] - a].Nota != "")
                {
                    comentario = "Ver";
                }
                else
                {
                    comentario = string.Empty;
                }
                rows.Add(Articulos[ListaTabla[i] - a].Id, Articulos[ListaTabla[i] - a].Codigo, Articulos[ListaTabla[i] - a].Descripcion, Articulos[ListaTabla[i] - a].Solicitado, Articulos[ListaTabla[i] - a].Recibido, comentario, Articulos[ListaTabla[i] - a].Pendiente);
                DataGridViewRow row = Tabla.Rows[i];
                if (Articulos[ListaTabla[i] - a].Solicitado - Articulos[ListaTabla[i] - a].Recibido > 0 && Articulos[ListaTabla[i] - a].Recibido != 0)
                {
                    row.DefaultCellStyle.BackColor = System.Drawing.Color.LightBlue;
                    if (banderaincompleto == true && temporal == false)
                    {
                        GlobalSettings.Instance.Incompletos++;
                        Lb_Incompletos.Text = GlobalSettings.Instance.Incompletos.ToString();
                        banderaincompleto = false;
                        GlobalSettings.Instance.Renglones--;
                        Lb_renglones.Text = GlobalSettings.Instance.Renglones.ToString();
                        bandera = false;
                    }
                }
                else if (Articulos[ListaTabla[i] - a].Solicitado - Articulos[ListaTabla[i] - a].Recibido == 0)
                    row.DefaultCellStyle.BackColor = System.Drawing.Color.LightGreen;
                else if (Articulos[ListaTabla[i] - a].Solicitado - Articulos[ListaTabla[i] - a].Recibido < 0)
                {
                    row.DefaultCellStyle.BackColor = System.Drawing.Color.Red;
                    row.DefaultCellStyle.ForeColor = System.Drawing.Color.White;
                }
            }
            //if (bandera == true)
            //{
            //    GlobalSettings.Instance.Renglones--;
            //    Lb_renglones.Text = GlobalSettings.Instance.Renglones.ToString();
            //    bandera = false;
            //}
            Tabla.FirstDisplayedScrollingRowIndex = GlobalSettings.Instance.Current;
            Tabla.ClearSelection();
            Tabla.Rows[GlobalSettings.Instance.Current].Cells[0].Selected = true;
            Tabla.Rows[GlobalSettings.Instance.Current].Cells[1].Selected = true;
            TxtCodigo.Text = string.Empty;
            ListaTabla.Clear();
            TxtCodigo.Focus();
        }
        public void Ubicacion(int index)
        {
            FbConnection con = new FbConnection("User=SYSDBA;" + "Password=C0r1b423;" + "Database=D:\\Microsip datos\\PAPELERIA CORIBA CORNEJO.fdb;" + "DataSource=192.168.0.11;" + "Port=3050;" + "Dialect=3;" + "Charset=UTF8;");
            try
            {
                con.Open();
                string query4 = "SELECT * FROM NIVELES_ARTICULOS WHERE ARTICULO_ID = " + GlobalSettings.Instance.Articuloid + ";";
                FbCommand command4 = new FbCommand(query4, con);
                FbDataReader reader4 = command4.ExecuteReader();

                // Iterar sobre los registros y mostrar los valores
                while (reader4.Read())
                {
                    string columna3 = reader4.GetString(3);
                    string columna2 = reader4.GetString(2);
                    if (columna3 != "")
                    {
                        if (columna2 == "108403")
                            Articulos[index].Ubicacion += "TIENDA:  " + columna3 + "\n";
                        if (columna2 == "108402")
                            Articulos[index].Ubicacion += "ALMACÉN:  " + columna3 + "\n";
                        if (columna2 == "108401")
                            Articulos[index].Ubicacion += "ISLA:  " + columna3 + "\n";
                    }

                }
                if (Articulos[index].Ubicacion == null)
                {
                    Articulos[index].Ubicacion = "No tiene registrada una ubicación";
                }
                reader4.Close();

            }
            catch (Exception ex)
            {
                MessageBox.Show("Se perdió la conexión :( , contacta a 06 o intenta de nuevo", "¡Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                MessageBox.Show(ex.ToString());
                return;
            }
            finally
            {
                con.Close();
            }
        }

        private void Tabla_CellMouseDown(object sender, DataGridViewCellMouseEventArgs e)
        {

            if (e.Button == MouseButtons.Right && e.RowIndex >= 0 && e.ColumnIndex == 2)
            {
                Tabla.ClearSelection();
                Tabla.Rows[e.RowIndex].Selected = true;
                int codigo = int.Parse(Tabla.Rows[e.RowIndex].Cells[0].Value.ToString());
                string clave = (Tabla.Rows[e.RowIndex].Cells[1].Value.ToString());
                ContextMenuStrip menu = new ContextMenuStrip();

                // Agregar opciones al menú
                ToolStripMenuItem verUbicacionMenuItem = new ToolStripMenuItem("Ver Ubicación");
                ToolStripMenuItem verNotaMenuItem = new ToolStripMenuItem("Ver Nota");
                ToolStripMenuItem verExistenciaMenuItem = new ToolStripMenuItem("Ver Existencia");
                menu.Items.Add(verUbicacionMenuItem);
                menu.Items.Add(verNotaMenuItem);
                menu.Items.Add(verExistenciaMenuItem);
                // Manejar el evento ItemClicked del menú contextual
                menu.ItemClicked += (s, args) =>
                {
                    // Verificar la opción seleccionada

                    if (args.ClickedItem == verUbicacionMenuItem)
                    {
                        // Lógica para la opción "Ver Ubicación"
                        // Aquí puedes ejecutar la acción correspondiente
                        menu.Close();
                        for (int i = 0; i < Articulos.Count; ++i)
                        {
                            if (Articulos[i].Id == codigo)
                            {
                                if (Articulos[i].Ubicacion == "No tiene registrada una ubicación")
                                {
                                    var customMessageBox = new Mensaje();
                                    customMessageBox.SetMensaje("Artículo sin ubicacion", "ubicacion2");
                                    customMessageBox.ShowDialog();
                                }
                                else
                                {
                                    var customMessageBox = new Mensaje();
                                    customMessageBox.SetMensaje(Articulos[i].Ubicacion + "\n", "ubicacion");
                                    customMessageBox.ShowDialog();
                                }
                            }
                        }
                    }
                    else if (args.ClickedItem == verNotaMenuItem)
                    {
                        // Lógica para la opción "Ver Ubicación"
                        // Aquí puedes ejecutar la acción correspondiente
                        menu.Close();
                        for (int i = 0; i < Articulos.Count; ++i)
                        {
                            if (Articulos[i].Id == codigo)
                            {
                                if (Articulos[i].Nota == "")
                                {
                                    var customMessageBox = new Mensaje();
                                    customMessageBox.SetMensaje("Artículo sin nota", "nota2");
                                    customMessageBox.ShowDialog();
                                }
                                else
                                {
                                    var customMessageBox = new Mensaje();
                                    customMessageBox.SetMensaje(Articulos[i].Nota + "\n", "nota");
                                    customMessageBox.ShowDialog();
                                }
                            }


                        }
                    }
                    else if (args.ClickedItem == verExistenciaMenuItem)
                    {
                        // Lógica para la opción "Ver Ubicación"
                        // Aquí puedes ejecutar la acción correspondiente
                        menu.Close();
                        for (int i = 0; i < Articulos.Count; ++i)
                        {
                            if (Articulos[i].Id == codigo)
                            {
                                Existencia(int.Parse(Articulos[i].ArticuloId));
                            }
                        }
                    }
                };

                menu.Show(Cursor.Position);

            }
            if (e.Button == MouseButtons.Right && e.RowIndex >= 0 && e.ColumnIndex == 4)
            {
                Tabla.ClearSelection();
                Tabla.Rows[e.RowIndex].Selected = true;
                int codigo = int.Parse(Tabla.Rows[e.RowIndex].Cells[0].Value.ToString());
                ContextMenuStrip menu2 = new ContextMenuStrip();

                // Agregar opciones al menú
                ToolStripMenuItem ModificarMenuItem = new ToolStripMenuItem("Modificar");
                menu2.Items.Add(ModificarMenuItem);

                // Manejar el evento ItemClicked del menú contextual
                menu2.ItemClicked += (s, args) =>
                {
                    // Verificar la opción seleccionada
                    if (args.ClickedItem == ModificarMenuItem)
                    {
                        // Lógica para la opción "Ver Ubicación"
                        // Aquí puedes ejecutar la acción correspondiente
                        menu2.Close();
                        for (int i = 0; i < Articulos.Count; ++i)
                        {
                            if (Articulos[i].Id == codigo)
                            {
                                Editar Control2 = new Editar();
                                Control2.FuncionEditar(TxtCodigo.Text, Articulos[i].Descripcion, Articulos[i].Solicitado, Articulos[i].Recibido, i);
                                Control2.EnviarVariableEvent2 += new Editar.EnviarVariableDelegate2(ejecutar);
                                Control2.Show();
                            }
                        }
                    }
                }; menu2.Show(Cursor.Position);
            }
        }
        public void Existencia(int articulo_id)
        {
            FbConnection con = new FbConnection("User=SYSDBA;" + "Password=C0r1b423;" + "Database=D:\\Microsip datos\\PAPELERIA CORIBA CORNEJO.fdb;" + "DataSource=192.168.0.11;" + "Port=3050;" + "Dialect=3;" + "Charset=UTF8;");
            try
            {
                con.Open();
                FbCommand command = new FbCommand("EXIVAL_ART", con);
                command.CommandType = CommandType.StoredProcedure;

                // Parámetros de entrada
                command.Parameters.Add("V_ARTICULO_ID", FbDbType.Integer).Value = articulo_id;
                command.Parameters.Add("V_ALMACEN_ID", FbDbType.Integer).Value = 108404;
                command.Parameters.Add("V_FECHA", FbDbType.Date).Value = DateTime.Today;
                command.Parameters.Add("V_ES_ULTIMO_COSTO", FbDbType.Char).Value = 'S';
                command.Parameters.Add("V_SUCURSAL_ID", FbDbType.Integer).Value = 0;

                // Parámetro de salida
                FbParameter paramARTICULO = new FbParameter("ARTICULO_ID", FbDbType.Numeric);
                paramARTICULO.Direction = ParameterDirection.Output;
                command.Parameters.Add(paramARTICULO);
                FbParameter paramEXISTENCIA = new FbParameter("EXISTENCIAS", FbDbType.Numeric);
                paramEXISTENCIA.Direction = ParameterDirection.Output;
                command.Parameters.Add(paramEXISTENCIA);
                // Ejecutar el procedimiento almacenado
                command.ExecuteNonQuery();
                int Existencia = Convert.ToInt32(command.Parameters[6].Value);

                FbCommand command2 = new FbCommand("EXIVAL_ART", con);
                command2.CommandType = CommandType.StoredProcedure;

                // Parámetros de entrada
                command2.Parameters.Add("V_ARTICULO_ID", FbDbType.Integer).Value = articulo_id;
                command2.Parameters.Add("V_ALMACEN_ID", FbDbType.Integer).Value = 108403;
                command2.Parameters.Add("V_FECHA", FbDbType.Date).Value = DateTime.Today;
                command2.Parameters.Add("V_ES_ULTIMO_COSTO", FbDbType.Char).Value = 'S';
                command2.Parameters.Add("V_SUCURSAL_ID", FbDbType.Integer).Value = 0;

                // Parámetro de salida
                FbParameter paramARTICULO2 = new FbParameter("ARTICULO_ID", FbDbType.Numeric);
                paramARTICULO2.Direction = ParameterDirection.Output;
                command2.Parameters.Add(paramARTICULO2);
                FbParameter paramEXISTENCIA2 = new FbParameter("EXISTENCIAS", FbDbType.Numeric);
                paramEXISTENCIA2.Direction = ParameterDirection.Output;
                command2.Parameters.Add(paramEXISTENCIA2);
                // Ejecutar el procedimiento almacenadoienda
                command2.ExecuteNonQuery();
                decimal ExistenciaTienda = Convert.ToInt32(command2.Parameters[6].Value);
                //MessageBox.Show("ALMACÉN: "+ Existencia.ToString() +"\n TIENDA: "+ ExistenciaTienda.ToString());
                if (GlobalSettings.Instance.ExistenciaQuery == false)
                {
                    var customMessageBox = new Mensaje();
                    // Establece el mensaje que deseas mostrar
                    customMessageBox.SetMensaje("TIENDA: " + ExistenciaTienda.ToString(), "existencia");
                    // Muestra el formulario como un cuadro de diálogo modal
                    customMessageBox.ShowDialog();
                }
                else
                    GlobalSettings.Instance.Existencia = ExistenciaTienda;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Se perdió la conexión :( , contacta a 06 o intenta de nuevo", "¡Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                MessageBox.Show(ex.ToString());
                return;
            }
            finally
            {
                con.Close();
            }
        }
        private void TxtPedido_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true; // Evita que se produzca el sonido de Windows al presionar Enter
                BtnPedido.Focus();
            }
        }

        private void Tabla_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.F9)
            {
                if (Tabla.CurrentRow != null)
                {
                    int codigo = int.Parse(Tabla.CurrentRow.Cells[0].Value.ToString());
                    for (int i = 0; i < Articulos.Count; ++i)
                    {
                        if (Articulos[i].Id == codigo)
                        {
                            Existencia(int.Parse(Articulos[i].ArticuloId));
                        }
                    }
                }
            }
            else if (e.Control && e.KeyCode == Keys.N)
            {
                if (Tabla.CurrentRow != null)
                {
                    int codigo = int.Parse(Tabla.CurrentRow.Cells[0].Value.ToString());
                    for (int i = 0; i < Articulos.Count; ++i)
                    {
                        if (Articulos[i].Id == codigo)
                        {
                            if (Articulos[i].Nota == "")
                            {
                                var customMessageBox = new Mensaje();
                                customMessageBox.SetMensaje("Artículo sin nota", "nota2");
                                customMessageBox.ShowDialog();
                            }
                            else
                            {
                                var customMessageBox = new Mensaje();
                                customMessageBox.SetMensaje(Articulos[i].Nota + "\n", "nota");
                                customMessageBox.ShowDialog();
                            }
                        }

                    }
                }
            }
            else if (e.Control && e.KeyCode == Keys.U)
            {
                if (Tabla.CurrentRow != null)
                {
                    int codigo = int.Parse(Tabla.CurrentRow.Cells[0].Value.ToString());
                    for (int i = 0; i < Articulos.Count; ++i)
                    {
                        if (Articulos[i].Id == codigo)
                        {
                            if (Articulos[i].Ubicacion == "No tiene registrada una ubicación")
                            {
                                var customMessageBox = new Mensaje();
                                customMessageBox.SetMensaje("Artículo sin ubicacion", "ubicacion2");
                                customMessageBox.ShowDialog();
                            }
                            else
                            {
                                var customMessageBox = new Mensaje();
                                customMessageBox.SetMensaje(Articulos[i].Ubicacion + "\n", "ubicacion");
                                customMessageBox.ShowDialog();
                            }
                        }
                    }
                }
            }
            else if (e.Control && e.KeyCode == Keys.G)
            {
                if (Tabla.CurrentRow != null)
                {
                    BtnPDF.PerformClick();
                }
            }
        }
        public void EliminarQuery()
        {
            FbConnection con9 = new FbConnection("User=SYSDBA;" + "Password=C0r1b423;" + "Database=D:\\Microsip datos\\PAPELERIA CORIBA CORNEJO.fdb;" + "DataSource=192.168.0.11;" + "Port=3050;" + "Dialect=3;" + "Charset=UTF8;");
            try
            {
                con9.Open();
                string query10 = "SELECT * FROM DOCTOS_VE WHERE DOCTO_VE_ID = '" + GlobalSettings.Instance.FolioId + "';";
                FbCommand command10 = new FbCommand(query10, con9);
                FbDataReader reader10 = command10.ExecuteReader();
                if (reader10.Read())
                {
                    GlobalSettings.Instance.Importe_Total = reader10.GetDecimal(26);
                    GlobalSettings.Instance.Impuesto_total = reader10.GetDecimal(29);
                }
                reader10.Close();

                //IMPUESTO
                string query111 = "SELECT * FROM IMPUESTOS_ARTICULOS WHERE ARTICULO_ID = '" + GlobalSettings.Instance.Clave_articulo_id + "'";
                FbCommand command111 = new FbCommand(query111, con9);
                FbDataReader reader111 = command111.ExecuteReader();
                if (reader111.Read())
                {
                    GlobalSettings.Instance.Clave_impuesto = reader111.GetString(2);
                    //MessageBox.Show(GlobalSettings.Instance.Clave_impuesto);
                }
                reader111.Close();
                //QUERI 4 PARA BUSCAR IMPORTE DEL ARTICULO

                string query123 = "SELECT * FROM IMPUESTOS WHERE IMPUESTO_ID = '" + GlobalSettings.Instance.Clave_impuesto + "'";
                FbCommand command123 = new FbCommand(query123, con9);
                FbDataReader reader123 = command123.ExecuteReader();
                if (reader123.Read())
                {
                    GlobalSettings.Instance.Impuesto = reader123.GetString(2);
                }
                reader123.Close();

                // Utiliza parámetros para evitar la inyección de SQL
                string query9 = "DELETE FROM DOCTOS_VE_DET WHERE DOCTO_VE_ID = @FolioId AND POSICION = @EliminarValue";
                FbCommand command9 = new FbCommand(query9, con9);
                // Agrega los parámetros
                //VALOR DE UNIDADES A ACTUALIZAR
                command9.Parameters.AddWithValue("@EliminarValue", GlobalSettings.Instance.Eliminar);
                //VALOR DE FOLIO ID A EDITAR EN DOCTOS_VE_DET
                command9.Parameters.AddWithValue("@FolioId", GlobalSettings.Instance.FolioId);

                // Ejecuta la consulta de actualización
                int rowsAffected = command9.ExecuteNonQuery();

                if (rowsAffected == 0)
                {
                    MessageBox.Show("No se pudo actualizar el pedido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                //ACTUALIZAR IMPUESTOS E IMPORTE EN EL PEDIDO
                string query92 = "UPDATE DOCTOS_VE SET IMPORTE_NETO = @Importe, TOTAL_IMPUESTOS = @Impuestos WHERE DOCTO_VE_ID = '" + GlobalSettings.Instance.FolioId + "';";
                FbCommand command92 = new FbCommand(query92, con9);

                // Agrega los parámetros
                //VALOR DE UNIDADES A ACTUALIZAR
                if (GlobalSettings.Instance.PrimerImporte >= 1)
                {
                    GlobalSettings.Instance.Importe_Total_Anterior = GlobalSettings.Instance.Importe_Total;
                    GlobalSettings.Instance.Impuesto_Total_Anterior = GlobalSettings.Instance.Impuesto_total;

                }
                GlobalSettings.Instance.Importe_Total -= GlobalSettings.Instance.Importearticuloeliminado;
                if (GlobalSettings.Instance.Impuesto == "16% IVA ")
                {
                    decimal actual = (GlobalSettings.Instance.Importearticuloeliminado * 0.16m);
                    decimal Impuesto = Math.Round(actual, 2);
                    //MessageBox.Show("Impuesto actual: " + Impuesto.ToString());
                    //MessageBox.Show("Impuesto actualizado: " + actual.ToString());
                    GlobalSettings.Instance.Impuesto_total -= Impuesto;
                }
                //else
                //{
                //    GlobalSettings.Instance.Impuesto_total = 0;
                //}
                //else
                //{
                //    decimal importe_impuesto = importe_neto_solicitado;
                //    decimal dif = (importe_impuesto) - (importe_neto);
                //    GlobalSettings.Instance.Impuesto_total = importe_impuesto - dif;
                //    GlobalSettings.Instance.Impuesto_total -= importe_neto;
                //}
                // GlobalSettings.Instance.Importe_Total += GlobalSettings.Instance.Importe_Total_Anterior;
                //GlobalSettings.Instance.Impuesto_total += GlobalSettings.Instance.Impuesto_Total_Anterior;
                command92.Parameters.AddWithValue("@Importe", GlobalSettings.Instance.Importe_Total);
                command92.Parameters.AddWithValue("@Impuestos", Math.Ceiling(GlobalSettings.Instance.Impuesto_total * 100) / 100);
                // Ejecuta la consulta de actualización
                rowsAffected = command92.ExecuteNonQuery();

                if (rowsAffected == 0)
                {
                    MessageBox.Show("No se pudo actualizar el importe");
                    return;
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Se perdió la conexión :( , contacta a 06 o intenta de nuevo", "¡Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                MessageBox.Show(ex.ToString());
                return;
            }
            finally
            {
                con9.Close();
            }
        }
        public void UpdateQuery()
        {
            FbConnection con8 = new FbConnection("User=SYSDBA;" + "Password=C0r1b423;" + "Database=D:\\Microsip datos\\PAPELERIA CORIBA CORNEJO.fdb;" + "DataSource=192.168.0.11;" + "Port=3050;" + "Dialect=3;" + "Charset=UTF8;");
            try
            {
                con8.Open();

                // Utiliza parámetros para evitar la inyección de SQL
                string query8 = "UPDATE DOCTOS_VE_DET SET UNIDADES = @UpdateValue, DSCTO_ART = @Descuento_articulo_neto, PRECIO_TOTAL_NETO = @Importe_total_neto WHERE DOCTO_VE_ID = @FolioId AND POSICION = @Posicion";
                FbCommand command8 = new FbCommand(query8, con8);

                // Agrega los parámetros
                //VALOR DE UNIDADES A ACTUALIZAR
                command8.Parameters.AddWithValue("@UpdateValue", GlobalSettings.Instance.Update);
                decimal descuento = (GlobalSettings.Instance.Importe_articulo_neto * GlobalSettings.Instance.Update) * (GlobalSettings.Instance.Descuento_articulo_neto / 100);
                decimal descuento_solicitado = (GlobalSettings.Instance.Importe_articulo_neto * GlobalSettings.Instance.UnidadesSolicitadas) * (GlobalSettings.Instance.Descuento_articulo_neto / 100);
                decimal importe_neto_solicitado = (GlobalSettings.Instance.UnidadesSolicitadas * GlobalSettings.Instance.Importe_articulo_neto) - descuento_solicitado;
                decimal importe_neto = (GlobalSettings.Instance.Update * GlobalSettings.Instance.Importe_articulo_neto) - descuento;
                decimal diferecia = importe_neto_solicitado - importe_neto;
                //VALOR DE FOLIO ID A EDITAR EN DOCTOS_VE_DET
                command8.Parameters.AddWithValue("@FolioId", GlobalSettings.Instance.FolioId);
                //VALOR DE LA POSICION DEL CODIGO
                command8.Parameters.AddWithValue("@Posicion", GlobalSettings.Instance.Posicion);
                //VALOR DE UNIDADES A ACTUALIZAR
                command8.Parameters.AddWithValue("@Descuento_articulo_neto", descuento);
                //VALOR DE UNIDADES A ACTUALIZAR
                command8.Parameters.AddWithValue("@Importe_total_neto", importe_neto);

                // Ejecuta la consulta de actualización
                int rowsAffected = command8.ExecuteNonQuery();

                if (rowsAffected == 0)
                {
                    MessageBox.Show("No se pudo actualizar el pedido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                string query10 = "SELECT * FROM DOCTOS_VE WHERE DOCTO_VE_ID = '" + GlobalSettings.Instance.FolioId + "';";
                FbCommand command10 = new FbCommand(query10, con8);
                FbDataReader reader10 = command10.ExecuteReader();
                if (reader10.Read())
                {
                    GlobalSettings.Instance.Importe_Total = reader10.GetDecimal(26);
                    GlobalSettings.Instance.Impuesto_total = reader10.GetDecimal(29);
                }
                reader10.Close();

                //IMPUESTO
                string query11 = "SELECT * FROM IMPUESTOS_ARTICULOS WHERE ARTICULO_ID = '" + GlobalSettings.Instance.Clave_articulo_id + "'";
                FbCommand command11 = new FbCommand(query11, con8);
                FbDataReader reader11 = command11.ExecuteReader();
                if (reader11.Read())
                {
                    GlobalSettings.Instance.Clave_impuesto = reader11.GetString(2);
                    //MessageBox.Show(GlobalSettings.Instance.Clave_impuesto);
                }
                reader11.Close();
                //QUERI 4 PARA BUSCAR IMPORTE DEL ARTICULO

                string query12 = "SELECT * FROM IMPUESTOS WHERE IMPUESTO_ID = '" + GlobalSettings.Instance.Clave_impuesto + "'";
                FbCommand command12 = new FbCommand(query12, con8);
                FbDataReader reader12 = command12.ExecuteReader();
                if (reader12.Read())
                {
                    GlobalSettings.Instance.Impuesto = reader12.GetString(2);
                }
                reader12.Close();

                string query9 = "UPDATE DOCTOS_VE SET IMPORTE_NETO = @Importe, TOTAL_IMPUESTOS = @Impuestos WHERE DOCTO_VE_ID = '" + GlobalSettings.Instance.FolioId + "';";
                FbCommand command9 = new FbCommand(query9, con8);

                // Agrega los parámetros
                //VALOR DE UNIDADES A ACTUALIZAR
                if (GlobalSettings.Instance.PrimerImporte >= 1)
                {
                    GlobalSettings.Instance.Importe_Total_Anterior = GlobalSettings.Instance.Importe_Total;
                    GlobalSettings.Instance.Impuesto_Total_Anterior = GlobalSettings.Instance.Impuesto_total;

                }
                GlobalSettings.Instance.Importe_Total -= diferecia;
                if (GlobalSettings.Instance.Impuesto == "16% IVA ")
                {
                    //decimal importe_impuesto = importe_neto_solicitado * 1.16m;
                    //decimal dif = (importe_impuesto) - (importe_neto * 1.16m);
                    decimal dif = (diferecia * 0.16m);
                    decimal difact = Math.Round(dif, 2);
                    GlobalSettings.Instance.Impuesto_total -= difact;
                    //GlobalSettings.Instance.Impuesto_total -= importe_neto;
                }
                //else
                //{
                //    GlobalSettings.Instance.Impuesto_total = 0;
                //}
                //else
                //{
                //    decimal importe_impuesto = importe_neto_solicitado;
                //    decimal dif = (importe_impuesto) - (importe_neto);
                //    GlobalSettings.Instance.Impuesto_total = importe_impuesto - dif;
                //    GlobalSettings.Instance.Impuesto_total -= importe_neto;
                //}
                //GlobalSettings.Instance.Importe_Total += GlobalSettings.Instance.Importe_Total_Anterior;
                //GlobalSettings.Instance.Impuesto_total += GlobalSettings.Instance.Impuesto_Total_Anterior;
                command9.Parameters.AddWithValue("@Importe", GlobalSettings.Instance.Importe_Total);
                command9.Parameters.AddWithValue("@Impuestos", GlobalSettings.Instance.Impuesto_total);
                // Ejecuta la consulta de actualización
                rowsAffected = command9.ExecuteNonQuery();

                if (rowsAffected == 0)
                {
                    MessageBox.Show("No se pudo actualizar el importe", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                //bool Find = false;
                //// Objeto para leer los datos obtenidos
                //FbDataReader reader0 = command9.ExecuteReader();
                //if (reader0.Read())
                //{
                //    GlobalSettings.Instance.status = reader0.GetString(18);
                //    GlobalSettings.Instance.FolioId = reader0.GetString(0);
                //    Find = true;
                //}
                //reader0.Close();

            }
            catch (Exception ex)
            {
                MessageBox.Show("Se perdió la conexión :( , contacta a 06 o intenta de nuevo", "¡Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                MessageBox.Show(ex.ToString());
                return;
            }
            finally
            {
                con8.Close();
            }
        }
        public decimal ExistenciaValor(string articulo_id)
        {
            FbConnection con = new FbConnection("User=SYSDBA;" + "Password=C0r1b423;" + "Database=D:\\Microsip datos\\PAPELERIA CORIBA CORNEJO.fdb;" + "DataSource=192.168.0.11;" + "Port=3050;" + "Dialect=3;" + "Charset=UTF8;");
            try
            {
                con.Open();
                FbCommand command = new FbCommand("EXIVAL_ART", con);
                command.CommandType = CommandType.StoredProcedure;

                // Parámetros de entrada
                command.Parameters.Add("V_ARTICULO_ID", FbDbType.Integer).Value = articulo_id;
                command.Parameters.Add("V_ALMACEN_ID", FbDbType.Integer).Value = 108404;
                command.Parameters.Add("V_FECHA", FbDbType.Date).Value = DateTime.Today;
                command.Parameters.Add("V_ES_ULTIMO_COSTO", FbDbType.Char).Value = 'S';
                command.Parameters.Add("V_SUCURSAL_ID", FbDbType.Integer).Value = 0;

                // Parámetro de salida
                FbParameter paramARTICULO = new FbParameter("ARTICULO_ID", FbDbType.Numeric);
                paramARTICULO.Direction = ParameterDirection.Output;
                command.Parameters.Add(paramARTICULO);
                FbParameter paramEXISTENCIA = new FbParameter("EXISTENCIAS", FbDbType.Numeric);
                paramEXISTENCIA.Direction = ParameterDirection.Output;
                command.Parameters.Add(paramEXISTENCIA);
                // Ejecutar el procedimiento almacenado
                command.ExecuteNonQuery();
                int Existencia = Convert.ToInt32(command.Parameters[6].Value);

                FbCommand command2 = new FbCommand("EXIVAL_ART", con);
                command2.CommandType = CommandType.StoredProcedure;

                // Parámetros de entrada
                command2.Parameters.Add("V_ARTICULO_ID", FbDbType.Integer).Value = articulo_id;
                command2.Parameters.Add("V_ALMACEN_ID", FbDbType.Integer).Value = 108403;
                command2.Parameters.Add("V_FECHA", FbDbType.Date).Value = DateTime.Today;
                command2.Parameters.Add("V_ES_ULTIMO_COSTO", FbDbType.Char).Value = 'S';
                command2.Parameters.Add("V_SUCURSAL_ID", FbDbType.Integer).Value = 0;

                // Parámetro de salida
                FbParameter paramARTICULO2 = new FbParameter("ARTICULO_ID", FbDbType.Numeric);
                paramARTICULO2.Direction = ParameterDirection.Output;
                command2.Parameters.Add(paramARTICULO2);
                FbParameter paramEXISTENCIA2 = new FbParameter("EXISTENCIAS", FbDbType.Numeric);
                paramEXISTENCIA2.Direction = ParameterDirection.Output;
                command2.Parameters.Add(paramEXISTENCIA2);
                // Ejecutar el procedimiento almacenadoienda
                command2.ExecuteNonQuery();
                decimal ExistenciaTienda = Convert.ToInt32(command2.Parameters[6].Value);
                //MessageBox.Show("ALMACÉN: "+ Existencia.ToString() +"\n TIENDA: "+ ExistenciaTienda.ToString());
                //if (GlobalSettings.Instance.ExistenciaQuery == false)
                //{
                //    var customMessageBox = new Mensaje();
                //    // Establece el mensaje que deseas mostrar
                //    customMessageBox.SetMensaje("TIENDA: " + ExistenciaTienda.ToString(), "existencia");
                //    // Muestra el formulario como un cuadro de diálogo modal
                //    customMessageBox.ShowDialog();
                //}
                //else
                return ExistenciaTienda;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Se perdió la conexión :( , contacta a 06 o intenta de nuevo", "¡Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                MessageBox.Show(ex.ToString());
                return 0;
            }
            finally
            {
                con.Close();
            }
        }
        private void BtnPDF_Click(object sender, EventArgs e)
        {
            if (Articulos.Count == 0)
            {
                MessageBox.Show("Primero ingresa un pedido", "¡Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (Cb_Surtidor.Text == string.Empty)
            {
                MessageBox.Show("Te falta asignar un surtidor", "¡Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            bool bandera = false;
            string mensajemax = "";
            for (int i = 0; i < Articulos.Count; ++i)
            {
                if (Articulos[i].Solicitado > Articulos[i].Recibido)
                {
                    decimal existencia = ExistenciaValor(Articulos[i].ArticuloId.ToString());
                    if(existencia >= Articulos[i].Pendiente)
                    {
                        bandera = true;
                        string mensajepred = Articulos[i].Codigo + " ------- " + "Existencia: " + existencia + "\n";
                        mensajemax += mensajepred;
                    }
                }
            }
            if (bandera == true)
            {
                string mensaje1 = "!Estos códigos tienen existencia aún!\n\n";
                string mensajefinal = mensaje1 + mensajemax;
                MessageBox.Show(mensajefinal, "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            DialogResult result = MessageBox.Show("¿Estás seguro que deseas terminar este pedido?\n Los artículos del pedido se van a modificar \n ¿Deseas continuar?", "Advertencia", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);
            if (result == DialogResult.Cancel)
            {
                return;
            }
            string Hoy = DateTime.Now.ToString("d-M-yy");
            string filePath = "\\\\SRVPRINCIPAL\\incompletosPedidos\\ArticulosIncompletos " + Hoy + ".xlsx";
            bool fileExist = File.Exists(filePath);
            Document doc = new Document();
            try
            {
                if (!fileExist)
                {
                    SLDocument oSLDocument = new();
                    oSLDocument.SaveAs(filePath);
                }
            }
            catch (IOException ex)
            {
                // Maneja la excepción
                MessageBox.Show("Se produjo un error al acceder a la ubicación de red: " + ex.Message);
                // Aquí puedes realizar cualquier acción adicional, como cerrar la aplicación o retornar
                return;
            }
            SLDocument sl = new(filePath);
            //SLDocument excel = new(@"\\192.168.0.2\\incompletosPedidos\\ArticulosIncompletos");
            SLStyle style = sl.CreateStyle();
            style.Font.FontSize = 15;
            style.Font.FontColor = System.Drawing.Color.Red;
            style.Font.Bold = true;
            style.Alignment.Horizontal = HorizontalAlignmentValues.Center;
            style.Alignment.Vertical = VerticalAlignmentValues.Center;
            sl.SetCellStyle("A1", style);
            sl.SetCellValue("A1", "REPORTE DE FALTANTES DE PEDIDOS");
            sl.MergeWorksheetCells("A1", "I1");
            sl.SetCellValue("A2", "FECHA");
            sl.SetCellValue("B2", "PEDIDO");
            sl.SetCellValue("C2", "SURTIDOR");
            sl.SetCellValue("D2", "CODIGO");
            sl.SetCellValue("E2", "DESCRIPCION");
            sl.SetCellValue("F2", "SOLICITADO");
            sl.SetCellValue("G2", "VERIFICADO");
            sl.SetCellValue("H2", "NOTA");
            sl.SetCellValue("I2", "IMPORTE");
            sl.SetCellValue("J2", "ESTATUS");
            sl.SetCellValue("K2", "EXISTENCIA");
            sl.SetCellValue("L2", "VENDEDOR");
            int[] columnas = { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            foreach (int columna in columnas)
            {
                if (columna == 1)
                    sl.SetColumnWidth(columna, 11);
                if (columna == 2)
                    sl.SetColumnWidth(columna, 11);
                if (columna == 3 || columna == 5)
                    sl.SetColumnWidth(columna, 30);
                if (columna == 4)
                    sl.SetColumnWidth(columna, 11);
                if (columna == 30)
                    sl.SetColumnWidth(columna, 11);
                if (columna == 9 || columna == 6)
                    sl.SetColumnWidth(columna, 11);
                if (columna == 7)
                    sl.SetColumnWidth(columna, 11);
                if (columna == 8)
                    sl.SetColumnWidth(columna, 11);
                if (columna == 9)
                    sl.SetColumnWidth(columna, 20);
            }
            int fila = 3;
            while (sl.HasCellValue("A" + fila))
            {
                fila++;
            }
            for (int i = 0; i < Articulos.Count(); i++)
            {
                if (Articulos[i].Solicitado > Articulos[i].Recibido)
                {
                    GlobalSettings.Instance.ExistenciaQuery = true;
                    Existencia(int.Parse(Articulos[i].ArticuloId));
                    sl.SetCellValue("A" + fila, DateTime.Now.ToShortDateString().ToString());
                    sl.SetCellValue("B" + fila, TxtPedido.Text);
                    sl.SetCellValue("C" + fila, Cb_Surtidor.Text);
                    sl.SetCellValue("D" + fila, Articulos[i].Codigo);
                    sl.SetCellValue("E" + fila, Articulos[i].Descripcion);
                    sl.SetCellValue("F" + fila, Articulos[i].Solicitado);
                    sl.SetCellValue("G" + fila, Articulos[i].Recibido);
                    sl.SetCellValue("H" + fila, Articulos[i].Nota);
                    sl.SetCellValue("I" + fila, Articulos[i].Importe * Articulos[i].Pendiente);
                    sl.SetCellValue("K" + fila, GlobalSettings.Instance.Existencia);
                    if (Articulos[i].Recibido == 0)
                        sl.SetCellValue("J" + fila, "FALTANTE");
                    else
                        sl.SetCellValue("J" + fila, "INCOMPLETO");
                    sl.SetCellValue("L" + fila, GlobalSettings.Instance.Vendedora);
                    fila++;
                }
            }
            GlobalSettings.Instance.ExistenciaQuery = false;

            sl.SaveAs(filePath);
            doc.SetMargins(0, 0, 20, 20);
            string fileName = "C:\\DatosPedidos\\" + TxtPedido.Text + ".pdf";
            //string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            //string filePath = Path.Combine(documentsPath, fileName);
            PdfWriter.GetInstance(doc, new FileStream(fileName, FileMode.Create));
            doc.Open();

            // Crear una tabla para los datos correctos
            //PdfPTable table = new PdfPTable(Tabla.Columns.Count - 1);
            //PdfPCell cell = new PdfPCell(new Phrase("ARTÍCULOS CORRECTOS", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 16f, iTextSharp.text.Font.BOLD)));
            //cell.Colspan = 6;
            //cell.HorizontalAlignment = 1;
            //cell.PaddingBottom = 10f;
            //cell.PaddingTop = 10f;
            //table.AddCell(cell);
            float[] columnWidths = new float[] { 10f, 15f, 78f, 20f, 19f, 20f }; // Asumiendo que la segunda columna tendrá un ancho personalizado
            //table.SetWidths(columnWidths);
            // Crear una tabla para los datos faltantes
            PdfPTable table2 = new PdfPTable(Tabla.Columns.Count - 1);
            PdfPCell cell2 = new PdfPCell(new Phrase("ARTÍCULOS INCOMPLETOS", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 16f, iTextSharp.text.Font.BOLD)));
            cell2.Colspan = 6;
            cell2.HorizontalAlignment = 1;
            cell2.PaddingBottom = 10f;
            cell2.PaddingTop = 10f;
            table2.AddCell(cell2);
            table2.SetWidths(columnWidths);

            PdfPTable table4 = new PdfPTable(Tabla.Columns.Count - 1);
            PdfPCell cell4 = new PdfPCell(new Phrase("ARTÍCULOS A ELIMINAR", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 16f, iTextSharp.text.Font.BOLD)));
            cell4.Colspan = 6;
            cell4.HorizontalAlignment = 1;
            cell4.PaddingBottom = 10f;
            cell4.PaddingTop = 10f;
            table4.AddCell(cell4);
            table4.SetWidths(columnWidths);

            // Crear una tabla para los datos sobrantes
            PdfPTable table3 = new PdfPTable(Tabla.Columns.Count - 1);
            PdfPCell cell3 = new PdfPCell(new Phrase("ARTÍCULOS SOBRANTES", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 16f, iTextSharp.text.Font.BOLD)));
            cell3.Colspan = 6;
            cell3.HorizontalAlignment = 1;
            cell3.PaddingBottom = 10f;
            cell3.PaddingTop = 10f;
            table3.AddCell(cell3);

            table3.SetWidths(columnWidths);



            // Agregar encabezados de columna
            for (int i = 0; i < Tabla.Columns.Count; i++)
            {
                if (i != 5)
                {
                    //table.AddCell(Tabla.Columns[i].HeaderText);
                    table2.AddCell(Tabla.Columns[i].HeaderText);
                    table3.AddCell(Tabla.Columns[i].HeaderText);
                    table4.AddCell(Tabla.Columns[i].HeaderText);

                }
            }
            bool tabla1 = false;
            bool tabla2 = false;
            bool tabla3 = false;
            bool tabla4 = false;
            int cont1 = 0;
            int cont2 = 0;
            int cont3 = 0;
            int cont4 = 0;
            // Agregar datos del DataGridView al PDF
            Tabla.Rows.Clear();
            DataGridViewRowCollection rows = Tabla.Rows;
            for (int i = 0; i < Articulos.Count; ++i)
            {
                int a = 1;
                if (Articulos[i].Id != (i + 1))
                {
                    a = Articulos[i].Id - i;
                }
                for (int j = 0; j < Articulos.Count; ++j)
                {
                    if (Articulos[j].Id == i + a)
                    {
                        rows.Add(Articulos[j].Id, Articulos[j].Codigo, Articulos[j].Descripcion, Articulos[j].Solicitado, Articulos[j].Recibido, Articulos[j].Nota, Articulos[j].Pendiente);
                        DataGridViewRow row = Tabla.Rows[i];

                    }
                }
            }

            for (int i = 0; i < Tabla.Rows.Count; i++)
            {
                double.TryParse(Tabla[3, i].Value.ToString(), out double valorColumna3);
                double.TryParse(Tabla[4, i].Value.ToString(), out double valorColumna4);
                for (int j = 0; j < Tabla.Columns.Count; j++)
                {
                    if (Tabla[3, i].Value.ToString() == Tabla[4, i].Value.ToString() && j != 5)
                    {
                        //table.AddCell(Tabla[j, i].Value.ToString());
                        //tabla1 = true;
                        //cont1++;
                    }
                    else if (valorColumna4 < valorColumna3 && j != 5)
                    {
                        if (valorColumna4 == 0)
                        {
                            if (j == 0)
                            {
                                GlobalSettings.Instance.Eliminar = int.Parse(Tabla[0, i].Value.ToString());
                                for (int k = 0; k < Articulos.Count; ++k)
                                {
                                    if (Articulos[k].Id == int.Parse(Tabla[0, i].Value.ToString()))
                                    {
                                        GlobalSettings.Instance.Descuento_articulo_neto = Articulos[k].Descuento_porcentaje;
                                        GlobalSettings.Instance.Importe_articulo_neto = Articulos[k].Importe_neto_articulo;
                                        GlobalSettings.Instance.UnidadesSolicitadas = Articulos[k].Solicitado;
                                        GlobalSettings.Instance.Clave_articulo_id = Articulos[k].Clave;
                                        GlobalSettings.Instance.Importearticuloeliminado = Articulos[k].Importe_total_articuloeliminado;
                                    }
                                }
                                EliminarQuery();
                            }
                            table4.AddCell(Tabla[j, i].Value.ToString());
                            tabla4 = true;
                            cont4++;
                        }
                        else
                        {
                            table2.AddCell(Tabla[j, i].Value.ToString());
                            if (j == 0)
                            {
                                //MessageBox.Show(Tabla[0, i].Value.ToString());
                                GlobalSettings.Instance.Posicion = int.Parse(Tabla[0, i].Value.ToString());
                                for (int k = 0; k < Articulos.Count; ++k)
                                {
                                    if (Articulos[k].Id == int.Parse(Tabla[0, i].Value.ToString()))
                                    {
                                        GlobalSettings.Instance.Descuento_articulo_neto = Articulos[k].Descuento_porcentaje;
                                        GlobalSettings.Instance.Importe_articulo_neto = Articulos[k].Importe_neto_articulo;
                                        GlobalSettings.Instance.UnidadesSolicitadas = Articulos[k].Solicitado;
                                        GlobalSettings.Instance.Clave_articulo_id = Articulos[k].Clave;
                                    }
                                }
                            }
                            if (j == 4)
                            {
                                //MessageBox.Show(Tabla[4, i].Value.ToString());
                                GlobalSettings.Instance.Update = Decimal.Parse(Tabla[4, i].Value.ToString());
                                UpdateQuery();
                                GlobalSettings.Instance.PrimerImporte += 1;

                            }
                            tabla2 = true;
                            cont2++;

                        }
                    }
                    else if (valorColumna3 < valorColumna4 && j != 5)
                    {
                        table3.AddCell(Tabla[j, i].Value.ToString());
                        if (j == 0)
                        {
                            //MessageBox.Show(Tabla[0, i].Value.ToString());
                            GlobalSettings.Instance.Posicion = int.Parse(Tabla[0, i].Value.ToString());
                            for (int k = 0; k < Articulos.Count; ++k)
                            {
                                if (Articulos[k].Id == int.Parse(Tabla[0, i].Value.ToString()))
                                {
                                    GlobalSettings.Instance.Descuento_articulo_neto = Articulos[k].Descuento_porcentaje;
                                    GlobalSettings.Instance.Importe_articulo_neto = Articulos[k].Importe_neto_articulo;
                                    GlobalSettings.Instance.UnidadesSolicitadas = Articulos[k].Solicitado;
                                    GlobalSettings.Instance.Clave_articulo_id = Articulos[k].Clave;
                                }
                            }
                        }
                        if (j == 4)
                        {
                            //MessageBox.Show(Tabla[4, i].Value.ToString());
                            GlobalSettings.Instance.Update = Decimal.Parse(Tabla[4, i].Value.ToString());
                            UpdateQuery();
                        }
                        tabla3 = true;
                        cont3++;
                    }
                }
            }
            Paragraph Name = new Paragraph("PEDIDO: " + TxtPedido.Text);
            Name.Alignment = Element.ALIGN_CENTER;
            Paragraph contador4 = new Paragraph("Articulos Pendientes: " + (cont4 / 6).ToString());
            contador4.Alignment = Element.ALIGN_CENTER;
            Paragraph contador3 = new Paragraph("Articulos Sobrantes: " + (cont3 / 6).ToString());
            contador3.Alignment = Element.ALIGN_CENTER;
            Paragraph contador2 = new Paragraph("Articulos Incompletos: " + (cont2 / 6).ToString());
            contador2.Alignment = Element.ALIGN_CENTER;
            Paragraph contador1 = new Paragraph("Articulos Correctos: " + (cont1 / 6).ToString());
            contador1.Alignment = Element.ALIGN_CENTER;
            iTextSharp.text.Font customFont = FontFactory.GetFont("Arial", 10);
            Paragraph emptyParagraph = new Paragraph();
            emptyParagraph.SpacingBefore = 80f;
            Paragraph emptyParagraph2 = new Paragraph();
            emptyParagraph2.SpacingBefore = 10f;
            doc.Add(Name);
            doc.Add(emptyParagraph2);
            // Agregar la tabla al documento PDF
            if (tabla1 == true)
            {
                //doc.Add(table);
                doc.Add(emptyParagraph2);
                doc.Add(contador1);
                doc.Add(emptyParagraph2);
            }
            if (tabla2 == true)
            {
                doc.Add(table2);
                doc.Add(emptyParagraph2);
                doc.Add(contador2);
                doc.Add(emptyParagraph2);

            }
            if (tabla3 == true)
            {
                doc.Add(table3);
                doc.Add(emptyParagraph2);
                doc.Add(contador3);
                doc.Add(emptyParagraph2);

            }
            if (tabla4 == true)
            {
                doc.Add(table4);
                doc.Add(emptyParagraph2);
                doc.Add(contador4);
                doc.Add(emptyParagraph2);
            }
            if (tabla2 == false && tabla3 == false && tabla4 == false)
            {
                Paragraph completo = new Paragraph("EL PEDIDO ESTÁ COMPLETO");
                completo.Alignment = Element.ALIGN_CENTER;
                doc.Add(completo);
            }
            //doc.Add(cell4);
            doc.Add(emptyParagraph);
            // Cerrar el documento
            doc.Close();
            Process.Start(new ProcessStartInfo(fileName) { UseShellExecute = true });
            TxtPedido.Text = string.Empty;
            TxtCodigo.Text = string.Empty;
            TxtPedido.Enabled = true;
            BtnPDF.Enabled = false;
            BtnPedido.Enabled = true;
            Lb_renglones.Text = "0";
            Lb_Incompletos.Text = "0";
            GlobalSettings.Instance.Incompletos = 0;
            Articulos.Clear();
            Tabla.Rows.Clear();
            Tabla.Refresh();
            TxtPedido.Focus();

        }

        private void Cb_Surtidor_Leave(object sender, EventArgs e)
        {
            if (!nombresArray.Contains(Cb_Surtidor.Text) && Cb_Surtidor.Text != "")
            {
                MessageBox.Show("Este usuario no está registrado", "¡Espera!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                Cb_Surtidor.Text = "";
                Cb_Surtidor.Focus();
            }
        }

        private void Cb_Surtidor_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                TxtCodigo.Focus();
            }
        }
        private void Cb_Surtidor_KeyPress(object sender, KeyPressEventArgs e)
        {

            if (Char.IsLower(e.KeyChar))
            {
                e.KeyChar = Char.ToUpper(e.KeyChar);
            }
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Control && e.KeyCode == Keys.G)
            {
                BtnPDF_Click(sender, e);
            }
            if (e.Control && e.KeyCode == Keys.R)
            {
                BtnCodigo.BackColor = System.Drawing.Color.Red;
                BtnPDF.BackColor = System.Drawing.Color.Red;
                BtnPedido.BackColor = System.Drawing.Color.Red;
                label1.ForeColor = System.Drawing.Color.Red;
                label2.ForeColor = System.Drawing.Color.Red;
                label3.ForeColor = System.Drawing.Color.Red;
                label4.ForeColor = System.Drawing.Color.Red;
                label5.ForeColor = System.Drawing.Color.Red;
                label6.ForeColor = System.Drawing.Color.Red;
                label8.ForeColor = System.Drawing.Color.Red;
                Lb_Incompletos.ForeColor = System.Drawing.Color.Red;
                Lb_renglones.ForeColor = System.Drawing.Color.Red;

            }
            if (e.Control && e.KeyCode == Keys.A)
            {
                BtnCodigo.BackColor = System.Drawing.Color.CadetBlue;
                BtnPDF.BackColor = System.Drawing.Color.CadetBlue;
                BtnPedido.BackColor = System.Drawing.Color.CadetBlue;
                label1.ForeColor = System.Drawing.Color.CadetBlue;
                label2.ForeColor = System.Drawing.Color.CadetBlue;
                label3.ForeColor = System.Drawing.Color.CadetBlue;
                label4.ForeColor = System.Drawing.Color.CadetBlue;
                label5.ForeColor = System.Drawing.Color.CadetBlue;
                label6.ForeColor = System.Drawing.Color.CadetBlue;
                label8.ForeColor = System.Drawing.Color.CadetBlue;
                Lb_Incompletos.ForeColor = System.Drawing.Color.CadetBlue;
                Lb_renglones.ForeColor = System.Drawing.Color.CadetBlue;

            }
            if (e.Control && e.KeyCode == Keys.P)
            {
                BtnCodigo.BackColor = System.Drawing.Color.Fuchsia;
                BtnPDF.BackColor = System.Drawing.Color.Fuchsia;
                BtnPedido.BackColor = System.Drawing.Color.Fuchsia;
                label1.ForeColor = System.Drawing.Color.Fuchsia;
                label2.ForeColor = System.Drawing.Color.Fuchsia;
                label3.ForeColor = System.Drawing.Color.Fuchsia;
                label4.ForeColor = System.Drawing.Color.Fuchsia;
                label5.ForeColor = System.Drawing.Color.Fuchsia;
                label6.ForeColor = System.Drawing.Color.Fuchsia;
                label8.ForeColor = System.Drawing.Color.Fuchsia;
                Lb_Incompletos.ForeColor = System.Drawing.Color.Fuchsia;
                Lb_renglones.ForeColor = System.Drawing.Color.Fuchsia;

            }
            if (e.Control && e.KeyCode == Keys.I)
            {
                string desktopFolder = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                string rutaAlAppRefMs = desktopFolder + "\\AppTickets.appref-ms";

                // Verifica si el archivo existe antes de intentar abrirlo
                if (System.IO.File.Exists(rutaAlAppRefMs))
                {
                    // Abrir el archivo .appref-ms utilizando rundll32.exe
                    Process.Start("rundll32.exe", $"dfshim.dll,ShOpenVerbShortcut {rutaAlAppRefMs}");
                }
                else
                {
                    MessageBox.Show("El archivo especificado no existe.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    //MessageBox.Show(rutaAlAppRefMs);
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.KeyPreview = true;
        }

        private void TxtPedido_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (Char.IsLower(e.KeyChar))
            {
                e.KeyChar = Char.ToUpper(e.KeyChar);
            }
        }
    }
}