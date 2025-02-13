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
using System.Reflection;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using System.Net.Mail;
using System.Net;
using DocumentFormat.OpenXml.Drawing.Diagrams;
using DocumentFormat.OpenXml.Presentation;

namespace Validación_de_Pedidos
{
    public partial class Form1 : Form
    {
        List<string> nombresArray = new();
        private List<string> nombresValor = new() { };
        List<Articulo> Articulos = new();
        string path = "";
        public Form1()
        {
            InitializeComponent();
            Config();
            BtnPDF.Enabled = false;
            Leer_Datos();
            CargarExcel();
            Cb_Surtidor.SelectedIndex = -1;
            Cb_Surtidor.DropDownHeight = 250;
            TxtPedido.Select();
            Tabla.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        }
        public void Config()
        {
            string filePath = "C:\\ConfigDB\\DB.txt"; // Ruta de tu archivo de texto
            List<string> lineas = new List<string>();

            // Verificar si el archivo existe
            if (File.Exists(filePath))
            {
                // Leer todas las líneas del archivo
                using (StreamReader sr = new StreamReader(filePath))
                {
                    string linea;
                    while ((linea = sr.ReadLine()) != null)
                    {
                        GlobalSettings.Instance.Config.Add(linea);
                    }

                }
                GlobalSettings.Instance.Ip = GlobalSettings.Instance.Config[0];
                GlobalSettings.Instance.Puerto = GlobalSettings.Instance.Config[1];
                GlobalSettings.Instance.Direccion = GlobalSettings.Instance.Config[2];
                GlobalSettings.Instance.User = GlobalSettings.Instance.Config[3];
                GlobalSettings.Instance.Pw = GlobalSettings.Instance.Config[4];
            }

        }
        public void CargarExcel()
        {
            string filePath = "\\\\SRVPRINCIPAL\\clavesSurtido\\Faltantes Excluidos.xlsx";
            //string filePath = "C:\\clavesSurtido\\Claves.xlsx";
            using (SLDocument documento = new SLDocument(filePath))
            {
                int filas = documento.GetWorksheetStatistics().NumberOfRows;
                for (int i = 2; i < filas + 1; ++i)
                {
                    GlobalSettings.Instance.Excluidos.Add(documento.GetCellValueAsString("A" + i));
                }
                documento.CloseWithoutSaving();
            }


        }
        public void Leer_Datos()
        {
            nombresArray.Clear();
            nombresValor.Clear();
            //string filePath = "\\\\192.168.0.2\\C$\\clavesSurtido\\Claves.xlsx";

            string filePath = "\\\\SRVPRINCIPAL\\clavesSurtido\\Claves.xlsx";
            // string filePath = "C:\\clavesSurtido\\Claves.xlsx";
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
        public decimal Impuesto(string Articulo_Id)
        {
            FbConnection con3 = new FbConnection("User=" + GlobalSettings.Instance.User + ";" + "Password=" + GlobalSettings.Instance.Pw + ";" + "Database=" + GlobalSettings.Instance.Direccion + ";" + "DataSource=" + GlobalSettings.Instance.Ip + ";" + "Port=" + GlobalSettings.Instance.Puerto + ";" + "Dialect=3;" + "Charset=UTF8;");
            try
            {
                con3.Open();
                string Clave_Impuesto = "";
                string impuesto = "";
                string query3 = "SELECT * FROM IMPUESTOS_ARTICULOS WHERE ARTICULO_ID = '" + Articulo_Id + "'";
                FbCommand command3 = new FbCommand(query3, con3);
                FbDataReader reader3 = command3.ExecuteReader();
                if (reader3.Read())
                {
                    Clave_Impuesto = reader3.GetString(2);
                    //MessageBox.Show(GlobalSettings.Instance.Clave_impuesto);
                }
                reader3.Close();
                //QUERI 4 PARA BUSCAR IMPORTE DEL ARTICULO

                string query4 = "SELECT * FROM IMPUESTOS WHERE IMPUESTO_ID = '" + Clave_Impuesto + "'";
                FbCommand command4 = new FbCommand(query4, con3);
                FbDataReader reader4 = command4.ExecuteReader();
                if (reader4.Read())
                {
                    impuesto = reader4.GetString(2);

                }
                reader4.Close();
                if (impuesto == "16% IVA ")
                    return (decimal)1.16;
                else if (impuesto == "IEPS 8%")
                    return (decimal)1.08;
                else
                    return 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Se perdió la conexión :( , contacta a 06 o intenta de nuevo", "¡Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                MessageBox.Show(ex.ToString());
                return (decimal)1.16;
            }
            finally
            {
                con3.Close();
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
            if (Folio_Mod[1] == 'O' || Folio_Mod[1] == 'E' || Folio_Mod[1] == 'P' || Folio_Mod[1] == 'M' || Folio_Mod[1] == 'A')
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

            FbConnection con = new FbConnection("User=" + GlobalSettings.Instance.User + ";" + "Password=" + GlobalSettings.Instance.Pw + ";" + "Database=" + GlobalSettings.Instance.Direccion + ";" + "DataSource=" + GlobalSettings.Instance.Ip + ";" + "Port=" + GlobalSettings.Instance.Puerto + ";" + "Dialect=3;" + "Charset=UTF8;");
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
                    GlobalSettings.Instance.Docto_Ve_Id = GlobalSettings.Instance.FolioId;
                    GlobalSettings.Instance.Importe_Total = reader0.GetDecimal(26);
                    GlobalSettings.Instance.VendedorId = reader0.GetString(40);
                    GlobalSettings.Instance.Desc_extra = reader0.GetDecimal(16);
                    GlobalSettings.Instance.Desc_extra_importe = reader0.GetDecimal(17);
                    GlobalSettings.Instance.Vendedor = reader0.GetString(40);
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
                        Descuento_extra_individual = reader1.GetDecimal(12),
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
                for (int i = 0; i < Articulos.Count; ++i)
                {
                    decimal imp = Impuesto(Articulos[i].ArticuloId);
                    if (imp == 1.16m)
                    {
                        GlobalSettings.Instance.Impuesto_real += Articulos[i].Importe_total_articuloeliminado * 0.16m;
                    }
                    GlobalSettings.Instance.Importe_real += Articulos[i].Importe_total_articuloeliminado;
                }
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
                        FbConnection con3 = new FbConnection("User=" + GlobalSettings.Instance.User + ";" + "Password=" + GlobalSettings.Instance.Pw + ";" + "Database=" + GlobalSettings.Instance.Direccion + ";" + "DataSource=" + GlobalSettings.Instance.Ip + ";" + "Port=" + GlobalSettings.Instance.Puerto + ";" + "Dialect=3;" + "Charset=UTF8;");
                        try
                        {
                            con3.Open();
                            string query12 = "SELECT * FROM CLAVES_ARTICULOS WHERE ARTICULO_ID = '" + Articulos[i].Clave + "' AND ROL_CLAVE_ART_ID = '17'";
                            FbCommand command12 = new FbCommand(query12, con3);
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
                                FbConnection con4 = new FbConnection("User=" + GlobalSettings.Instance.User + ";" + "Password=" + GlobalSettings.Instance.Pw + ";" + "Database=" + GlobalSettings.Instance.Direccion + ";" + "DataSource=" + GlobalSettings.Instance.Ip + ";" + "Port=" + GlobalSettings.Instance.Puerto + ";" + "Dialect=3;" + "Charset=UTF8;");
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
                                if (BtnPedido.BackColor == System.Drawing.Color.Red || BtnPedido.BackColor == System.Drawing.Color.Fuchsia || BtnPedido.BackColor == System.Drawing.Color.HotPink)
                                {
                                    ejecutar(GlobalSettings.Instance.Contenido, GlobalSettings.Instance.Current);
                                    return;
                                }
                                Menu Control = new Menu();
                                Control.FuncionRecibir(TxtCodigo.Text, Articulos[i].Descripcion, Articulos[i].Solicitado, Articulos[i].Recibido, i, Articulos[i].Nota);
                                Control.EnviarVariableEvent += new Menu.EnviarVariableDelegate(ejecutar);
                                Control.ShowDialog();
                                TxtCodigo.Focus();
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
                        FbConnection con3 = new FbConnection("User=" + GlobalSettings.Instance.User + ";" + "Password=" + GlobalSettings.Instance.Pw + ";" + "Database=" + GlobalSettings.Instance.Direccion + ";" + "DataSource=" + GlobalSettings.Instance.Ip + ";" + "Port=" + GlobalSettings.Instance.Puerto + ";" + "Dialect=3;" + "Charset=UTF8;");
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
                                    if (BtnPedido.BackColor == System.Drawing.Color.Red || BtnPedido.BackColor == System.Drawing.Color.Fuchsia || BtnPedido.BackColor == System.Drawing.Color.HotPink)
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
            FbConnection con = new FbConnection("User=" + GlobalSettings.Instance.User + ";" + "Password=" + GlobalSettings.Instance.Pw + ";" + "Database=" + GlobalSettings.Instance.Direccion + ";" + "DataSource=" + GlobalSettings.Instance.Ip + ";" + "Port=" + GlobalSettings.Instance.Puerto + ";" + "Dialect=3;" + "Charset=UTF8;");
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
                    //if (columna3 != "")
                    //{
                    //    if (columna2 == "108405")
                    //       Articulos[index].Ubicacion += "CULIACÁN:  " + columna3 + "\n";

                    //}

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
                                Control2.ShowDialog();
                                TxtCodigo.Focus();
                            }
                        }
                    }
                }; menu2.Show(Cursor.Position);
            }
        }
        public void Existencia(int articulo_id)
        {
            FbConnection con = new FbConnection("User=" + GlobalSettings.Instance.User + ";" + "Password=" + GlobalSettings.Instance.Pw + ";" + "Database=" + GlobalSettings.Instance.Direccion + ";" + "DataSource=" + GlobalSettings.Instance.Ip + ";" + "Port=" + GlobalSettings.Instance.Puerto + ";" + "Dialect=3;" + "Charset=UTF8;");
            try
            {
                con.Open();
                FbCommand command = new FbCommand("EXIVAL_ART", con);
                command.CommandType = CommandType.StoredProcedure;

                // Parámetros de entrada
                command.Parameters.Add("V_ARTICULO_ID", FbDbType.Integer).Value = articulo_id;
                command.Parameters.Add("V_ALMACEN_ID", FbDbType.Integer).Value = 108401; //peri
                //command.Parameters.Add("V_ALMACEN_ID", FbDbType.Integer).Value = 108405; culiacan
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
                int ExistenciaAlmacen = Convert.ToInt32(command.Parameters[6].Value);
                GlobalSettings.Instance.ExistenciaAl = ExistenciaAlmacen;
                FbCommand command2 = new FbCommand("EXIVAL_ART", con);
                command2.CommandType = CommandType.StoredProcedure;

                // Parámetros de entrada
                command2.Parameters.Add("V_ARTICULO_ID", FbDbType.Integer).Value = articulo_id;
                command2.Parameters.Add("V_ALMACEN_ID", FbDbType.Integer).Value = 108403; //PERI
                //command2.Parameters.Add("V_ALMACEN_ID", FbDbType.Integer).Value = 108405; culiacan
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
                    customMessageBox.SetMensaje("ALMACÉN: " + ExistenciaAlmacen.ToString() +"\nTIENDA: " + ExistenciaTienda.ToString(), "existencia");
                    //customMessageBox.SetMensaje("CULIACÁN: " + ExistenciaTienda.ToString(), "existencia");
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
            FbConnection con9 = new FbConnection("User=" + GlobalSettings.Instance.User + ";" + "Password=" + GlobalSettings.Instance.Pw + ";" + "Database=" + GlobalSettings.Instance.Direccion + ";" + "DataSource=" + GlobalSettings.Instance.Ip + ";" + "Port=" + GlobalSettings.Instance.Puerto + ";" + "Dialect=3;" + "Charset=UTF8;");
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
                    //GlobalSettings.Instance.Impuesto_real = GlobalSettings.Instance.Importe_Total * 0.16m;
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
                string query92 = "UPDATE DOCTOS_VE SET IMPORTE_NETO = @Importe, TOTAL_IMPUESTOS = @Impuestos, DSCTO_IMPORTE = @Desc WHERE DOCTO_VE_ID = '" + GlobalSettings.Instance.FolioId + "';";
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
                    decimal actual = GlobalSettings.Instance.Importearticuloeliminado * 0.16m;
                    //MessageBox.Show("Impuesto actual: " + Impuesto.ToString());
                    //MessageBox.Show("Impuesto actualizado: " + actual.ToString());
                    GlobalSettings.Instance.Impuesto_total -= actual;
                    GlobalSettings.Instance.Impuesto_real -= actual;
                }
                else if (GlobalSettings.Instance.Impuesto == "IEPS 8%")
                {
                    decimal actual = GlobalSettings.Instance.Importearticuloeliminado * 0.08m;
                    //MessageBox.Show("Impuesto actual: " + Impuesto.ToString());
                    //MessageBox.Show("Impuesto actualizado: " + actual.ToString());
                    GlobalSettings.Instance.Impuesto_total -= actual;
                    GlobalSettings.Instance.Impuesto_real -= actual;
                }
                if (GlobalSettings.Instance.Desc_extra_ind != 0)
                {
                    GlobalSettings.Instance.Desc_extra_importe -= GlobalSettings.Instance.Desc_extra_ind;
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
                command92.Parameters.AddWithValue("@Importe", Math.Round(GlobalSettings.Instance.Importe_Total, 2));
                command92.Parameters.AddWithValue("@Impuestos", Math.Round(((GlobalSettings.Instance.Impuesto_real * 100) / 100), 2));
                command92.Parameters.AddWithValue("@Desc", Math.Round(GlobalSettings.Instance.Desc_extra_importe, 2));
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
        public decimal SumarTodo()
        {
            FbConnection con9 = new FbConnection("User=" + GlobalSettings.Instance.User + ";" + "Password=" + GlobalSettings.Instance.Pw + ";" + "Database=" + GlobalSettings.Instance.Direccion + ";" + "DataSource=" + GlobalSettings.Instance.Ip + ";" + "Port=" + GlobalSettings.Instance.Puerto + ";" + "Dialect=3;" + "Charset=UTF8;");
            try
            {
                con9.Open();
                string query10 = "SELECT * FROM DOCTOS_VE_DET WHERE DOCTO_VE_ID = '" + GlobalSettings.Instance.Docto_Ve_Id + "';";
                FbCommand command10 = new FbCommand(query10, con9);
                FbDataReader reader10 = command10.ExecuteReader();
                decimal suma = 0;
                while (reader10.Read())
                {
                    suma += reader10.GetDecimal(15);
                    //GlobalSettings.Instance.Impuesto_real = GlobalSettings.Instance.Importe_Total * 0.16m;
                }
                return suma;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Se perdió la conexión :( , contacta a 06 o intenta de nuevo", "¡Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                MessageBox.Show(ex.ToString());
                return 0;
            }
            finally
            {
                con9.Close();
            }
        }
        public decimal SumarExtra()
        {
            FbConnection con9 = new FbConnection("User=" + GlobalSettings.Instance.User + ";" + "Password=" + GlobalSettings.Instance.Pw + ";" + "Database=" + GlobalSettings.Instance.Direccion + ";" + "DataSource=" + GlobalSettings.Instance.Ip + ";" + "Port=" + GlobalSettings.Instance.Puerto + ";" + "Dialect=3;" + "Charset=UTF8;");
            try
            {
                con9.Open();
                string query10 = "SELECT * FROM DOCTOS_VE_DET WHERE DOCTO_VE_ID = '" + GlobalSettings.Instance.Docto_Ve_Id + "';";
                FbCommand command10 = new FbCommand(query10, con9);
                FbDataReader reader10 = command10.ExecuteReader();
                decimal suma = 0;
                while (reader10.Read())
                {
                    suma += reader10.GetDecimal(12);

                    //GlobalSettings.Instance.Impuesto_real = GlobalSettings.Instance.Importe_Total * 0.16m;
                }
                return suma;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Se perdió la conexión :( , contacta a 06 o intenta de nuevo", "¡Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                MessageBox.Show(ex.ToString());
                return 0;
            }
            finally
            {
                con9.Close();
            }
        }
        public void ModificarTotales(decimal importe, decimal extra, decimal impuestos)
        {
            int tercerDecimal23 = (int)(Math.Floor(impuestos * 1000) % 10);
            if (tercerDecimal23 == 5)
                impuestos = Math.Ceiling(impuestos * 100) / 100;
            FbConnection con9 = new FbConnection("User=" + GlobalSettings.Instance.User + ";" + "Password=" + GlobalSettings.Instance.Pw + ";" + "Database=" + GlobalSettings.Instance.Direccion + ";" + "DataSource=" + GlobalSettings.Instance.Ip + ";" + "Port=" + GlobalSettings.Instance.Puerto + ";" + "Dialect=3;" + "Charset=UTF8;");
            try
            {
                con9.Open();
                //ACTUALIZAR IMPUESTOS E IMPORTE EN EL PEDIDO
                string query92 = "UPDATE DOCTOS_VE SET IMPORTE_NETO = @Importe, TOTAL_IMPUESTOS= @Impuestos,  DSCTO_IMPORTE = @Desc WHERE DOCTO_VE_ID = '" + GlobalSettings.Instance.Docto_Ve_Id + "';";
                FbCommand command9 = new FbCommand(query92, con9);
                //GlobalSettings.Instance.Impuesto_total += GlobalSettings.Instance.Impuesto_Total_Anterior;
                command9.Parameters.AddWithValue("@Importe", Math.Round(importe, 2));
                command9.Parameters.AddWithValue("@Desc", Math.Round(extra, 2));
                command9.Parameters.AddWithValue("@Impuestos", Math.Round(impuestos, 2));
                // Ejecuta la consulta de actualización
                int rowsAffected = command9.ExecuteNonQuery();

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
        public void ModificarExtra(string Docto_ve_ID, decimal redondeado, decimal importe_neto)
        {
            FbConnection con9 = new FbConnection("User=" + GlobalSettings.Instance.User + ";" + "Password=" + GlobalSettings.Instance.Pw + ";" + "Database=" + GlobalSettings.Instance.Direccion + ";" + "DataSource=" + GlobalSettings.Instance.Ip + ";" + "Port=" + GlobalSettings.Instance.Puerto + ";" + "Dialect=3;" + "Charset=UTF8;");
            try
            {
                con9.Open();
                //ACTUALIZAR IMPUESTOS E IMPORTE EN EL PEDIDO
                string query92 = "UPDATE DOCTOS_VE_DET SET PRECIO_TOTAL_NETO = @Importe, DSCTO_EXTRA = @Desc WHERE DOCTO_VE_DET_ID = '" + Docto_ve_ID + "';";
                FbCommand command9 = new FbCommand(query92, con9);

                //GlobalSettings.Instance.Impuesto_total += GlobalSettings.Instance.Impuesto_Total_Anterior;
                command9.Parameters.AddWithValue("@Importe", Math.Round(importe_neto, 2));
                command9.Parameters.AddWithValue("@Desc", Math.Round(redondeado, 2));
                // Ejecuta la consulta de actualización
                int rowsAffected = command9.ExecuteNonQuery();

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
        public void SumarTotales()
        {
            FbConnection con007 = new FbConnection("User=" + GlobalSettings.Instance.User + ";" + "Password=" + GlobalSettings.Instance.Pw + ";" + "Database=" + GlobalSettings.Instance.Direccion + ";" + "DataSource=" + GlobalSettings.Instance.Ip + ";" + "Port=" + GlobalSettings.Instance.Puerto + ";" + "Dialect=3;" + "Charset=UTF8;");
            try
            {
                con007.Open();
                string query007 = "SELECT * FROM DOCTOS_VE_DET WHERE DOCTO_VE_ID = '" + GlobalSettings.Instance.FolioId + "';";
                FbCommand command007 = new FbCommand(query007, con007);
                FbDataReader reader007 = command007.ExecuteReader();
                decimal extra = 0;
                decimal extra3 = 0;
                decimal extra4 = 0;
                decimal importes = 0;
                decimal importes2 = 0;
                decimal extra_redondeado = 0;
                decimal suma = 0;
                string Docto_ve_det = "";
                decimal redondeado = 0;
                decimal importe_neto = 0;
                decimal importe_neto2 = 0;
                string articulo_id = "";
                decimal impuesto = 0;
                decimal impuestos = 0;
                decimal importe_imp = 0;
                decimal importe_imp_8 = 0;
                decimal clear_suma_redondeada = 0;
                decimal clear_suma = 0;
                decimal clear_suma4 = 0;
                decimal extra2 = 0;
                while (reader007.Read())
                {
                    decimal unitario = 0;
                    decimal piezas = 0;
                    decimal descuento = 0;
                    decimal sumatoria_neta2 = 0;
                    decimal sumatoria_neta = 0;
                    decimal descuento_neto = 0;
                    Docto_ve_det = reader007.GetString(0);
                    articulo_id = reader007.GetString(3);
                    impuesto = ImpuestoC(articulo_id);
                    piezas = reader007.GetDecimal(4);
                    unitario = reader007.GetDecimal(8);
                    descuento = reader007.GetDecimal(9);
                    decimal desc_decimal = descuento / 100;
                    descuento_neto = piezas * unitario * desc_decimal;
                    sumatoria_neta = piezas * unitario - descuento_neto;
                    sumatoria_neta2 = piezas * unitario - descuento_neto;
                    decimal desc_decimal_extra = GlobalSettings.Instance.Desc_extra / 100;
                    if (GlobalSettings.Instance.Desc_extra == 0)
                    {
                        if (impuesto != 0)
                        {
                            decimal extranomral = sumatoria_neta;
                            decimal extranomral3 = sumatoria_neta;
                            decimal finalred2 = 0;
                            finalred2 = Math.Round(extranomral, 2);
                            int tercerDecimal5 = (int)(Math.Floor(extranomral * 1000) % 10);
                            if (tercerDecimal5 == 5)
                            {
                                extranomral = Math.Truncate(extranomral * 100) / 100;
                                decimal dif = extranomral3 - extranomral;
                                if (dif > 0.005m)
                                    extranomral = finalred2;
                            }
                            if (impuesto == 1.16m)
                            {
                                importe_imp += extranomral;
                            }
                            else if (impuesto == 1.08m)
                            {
                                importe_imp_8 += extranomral;

                            }
                        }
                    }
                    if (GlobalSettings.Instance.Desc_extra != 0)
                    {
                        decimal red = sumatoria_neta * desc_decimal_extra;
                        decimal clear = sumatoria_neta - Math.Round(red, 2);
                        //decimal clear2 = sumatoria_neta - red;
                        decimal clear3 = sumatoria_neta - Math.Round(red, 2);
                        //clear_suma += Math.Round(clear2, 6);
                        clear_suma4 += Math.Round(clear3, 3);
                        decimal finalred = 0;
                        finalred = Math.Round(clear, 2);
                        int tercerDecimal6 = (int)(Math.Floor(clear * 1000) % 10);
                        if (tercerDecimal6 == 5)
                        {
                            clear = Math.Truncate(clear * 100) / 100;
                            decimal dif = clear3 - clear;
                            if (dif > 0.005m)
                                clear = finalred;
                        }
                        decimal redondeado_importe = Math.Round(clear, 2); //TARGET
                        if (impuesto != 0)
                        {

                            if (impuesto == 1.16m)
                            {
                                importe_imp += redondeado_importe;
                            }
                            else if (impuesto == 1.08m)
                            {
                                importe_imp_8 += redondeado_importe;

                            }
                        }
                        clear_suma_redondeada += redondeado_importe;
                        redondeado = Math.Round(red, 2);
                        importe_neto = Math.Round(sumatoria_neta, 2);
                        extra_redondeado += redondeado;
                        extra += Math.Round(sumatoria_neta, 4) * desc_decimal_extra;
                        decimal pre = sumatoria_neta * desc_decimal_extra;
                        decimal pre1 = Math.Round(sumatoria_neta, 3) * desc_decimal_extra;
                        //decimal finalpred = 0;
                        //finalpred = Math.Round(pre, 2);
                        //int tercerDecimal66 = (int)(Math.Floor(pre * 1000) % 10);
                        //if (tercerDecimal66 == 5)
                        //{
                        //    pre = Math.Truncate(pre * 100) / 100;
                        //    decimal dif = pre1 - pre;
                        //    if (dif > 0.005m)
                        //        pre = finalpred;

                        //}
                        extra2 += Math.Round(pre, 3); //3
                        extra3 += pre;
                        extra4 += Math.Round(pre1, 3);
                        importes += redondeado_importe;

                    }
                    else if (GlobalSettings.Instance.Desc_extra == 0)
                    {
                        importe_neto = sumatoria_neta;
                        decimal finalred = 0;
                        finalred = Math.Round(importe_neto, 2);
                        int tercerDecimal66 = (int)(Math.Floor(importe_neto * 1000) % 10);
                        if (tercerDecimal66 == 5)
                        {
                            importe_neto = Math.Truncate(importe_neto * 100) / 100;
                            decimal dif = sumatoria_neta - importe_neto;
                            if (dif > 0.005m)
                                importe_neto = finalred;

                        }
                        decimal redondeado_importe = Math.Round(importe_neto, 2);
                        importes += redondeado_importe;
                        importe_neto2 = Math.Round(sumatoria_neta, 3);

                        //importes += Math.Round(importe_neto, 2);
                        importes2 += importe_neto2;
                    }
                    //GlobalSettings.Instance.Impuesto_real = GlobalSettings.Instance.Importe_Total * 0.16m;
                }
                if (GlobalSettings.Instance.Desc_extra != 0)
                {
                    int tercerDecimal223 = (int)(Math.Floor(extra4 * 1000) % 10);
                    if (tercerDecimal223 == 5)
                        extra4 = Math.Ceiling(extra4 * 100) / 100;
                    decimal diferencia = Math.Round(Math.Round(extra4, 2) - Math.Round(extra_redondeado, 2), 2);
                    decimal nice = importe_neto - redondeado;
                    redondeado += diferencia;
                    nice -= diferencia;
                    importes -= diferencia;
                    importe_imp -= diferencia;
                    //decimal nice = importe_neto - redondeado; 
                    //decimal diferencia = Math.Round(Math.Round(extra,2) - Math.Round(extra_redondeado,2), 2);
                    //nice += diferencia;
                    //redondeado -= diferencia;
                    //importes += diferencia;
                    impuestos = importe_imp * 0.16m;
                    impuestos += importe_imp_8 * 0.08m;
                    extra_redondeado += diferencia;
                    ModificarExtra(Docto_ve_det, redondeado, nice);
                }
                else if (GlobalSettings.Instance.Desc_extra == 0)
                {
                    impuestos = importe_imp * 0.16m;
                    impuestos += importe_imp_8 * 0.08m;
                }
                ModificarTotales(Math.Round(importes, 2), Math.Round(extra_redondeado, 2), impuestos);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Se perdió la conexión :( , contacta a 06 o intenta de nuevo", "¡Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                MessageBox.Show(ex.ToString());
                return;
            }
            finally
            {
                con007.Close();
            }
        }
        public void UpdateQuery()
        {
            FbConnection con8 = new FbConnection("User=" + GlobalSettings.Instance.User + ";" + "Password=" + GlobalSettings.Instance.Pw + ";" + "Database=" + GlobalSettings.Instance.Direccion + ";" + "DataSource=" + GlobalSettings.Instance.Ip + ";" + "Port=" + GlobalSettings.Instance.Puerto + ";" + "Dialect=3;" + "Charset=UTF8;");
            try
            {
                con8.Open();

                // Utiliza parámetros para evitar la inyección de SQL
                string query8 = "UPDATE DOCTOS_VE_DET SET UNIDADES = @UpdateValue, DSCTO_ART = @Descuento_articulo_neto, DSCTO_EXTRA = @Extra, PRECIO_TOTAL_NETO = @Importe_total_neto WHERE DOCTO_VE_ID = @FolioId AND POSICION = @Posicion";
                FbCommand command8 = new FbCommand(query8, con8);
                // Agrega los parámetros
                //VALOR DE UNIDADES A ACTUALIZAR
                command8.Parameters.AddWithValue("@UpdateValue", GlobalSettings.Instance.Update);
                decimal descuento = (GlobalSettings.Instance.Importe_articulo_neto * GlobalSettings.Instance.Update) * (GlobalSettings.Instance.Descuento_articulo_neto / 100);
                decimal descuento_solicitado = (GlobalSettings.Instance.Importe_articulo_neto * GlobalSettings.Instance.UnidadesSolicitadas) * (GlobalSettings.Instance.Descuento_articulo_neto / 100);
                decimal importe_neto_solicitado = (GlobalSettings.Instance.UnidadesSolicitadas * GlobalSettings.Instance.Importe_articulo_neto) - descuento_solicitado;
                decimal importe_neto = (GlobalSettings.Instance.Update * GlobalSettings.Instance.Importe_articulo_neto) - descuento;
                decimal chido = 0;
                decimal extra = 0;
                decimal bueno = 0;

                if (GlobalSettings.Instance.Desc_extra != 0)
                {
                    //Descuento 40/100 = 0.4
                    extra = (importe_neto / 100m) * GlobalSettings.Instance.Desc_extra;
                    // extra = importe_neto / GlobalSettings.Instance.Desc_extra;
                    int tercerDecimal22 = (int)(Math.Floor(extra * 1000) % 10);
                    if (tercerDecimal22 == 5)
                        extra = Math.Ceiling(extra * 100) / 100;
                    //decimal extrasoli = importe_neto_solicitado / GlobalSettings.Instance.Desc_extra;
                    decimal importe = GlobalSettings.Instance.Importe_articulo_neto * GlobalSettings.Instance.Update;
                    bueno = importe - extra - Math.Round(descuento, 2);
                    decimal des = GlobalSettings.Instance.Desc_extra / 100;
                    chido = importe_neto * des;
                    importe_neto *= 10000;
                    decimal Nprice = importe_neto * 100;
                    decimal nuevoImpuesto = Nprice - (importe_neto * GlobalSettings.Instance.Desc_extra);
                    importe_neto = nuevoImpuesto / 1000000;
                    //decimal imp_net_ = importe_neto_solicitado - extrasoli;
                }

                decimal importe_neto2 = GlobalSettings.Instance.Importearticuloeliminado - importe_neto;
                decimal diferecia = importe_neto_solicitado - Math.Round(importe_neto, 2);
                //decimal diferecia = importe_neto_solicitado - importe_neto;
                decimal diferencia_desc_extra = GlobalSettings.Instance.Desc_extra_importe - chido;
                //GlobalSettings.Instance.Desc_extra_importe -= diferencia_desc_extra;
                decimal dif_ex = GlobalSettings.Instance.Desc_extra_ind - chido;
                GlobalSettings.Instance.Desc_extra_importe -= dif_ex;
                //VALOR DE FOLIO ID A EDITAR EN DOCTOS_VE_DET
                command8.Parameters.AddWithValue("@FolioId", GlobalSettings.Instance.FolioId);
                //int tercerDecimal = (int)(Math.Floor(chido * 1000) % 10);
                //if (tercerDecimal == 5)
                //    chido += 0.005m;


                if (GlobalSettings.Instance.Desc_extra != 0)
                {
                    int tercerDecimal23 = (int)(Math.Floor(bueno * 1000) % 10);
                    if (tercerDecimal23 == 5)
                        bueno = Math.Ceiling(bueno * 100) / 100;
                    decimal a = Math.Round(extra, 2);
                    decimal b = Math.Round(descuento, 2);
                    decimal c = Math.Round(bueno, 2);


                    command8.Parameters.AddWithValue("@Extra", a);
                    //VALOR DE LA POSICION DEL CODIGO
                    command8.Parameters.AddWithValue("@Posicion", GlobalSettings.Instance.Posicion);
                    //VALOR DE UNIDADES A ACTUALIZAR
                    command8.Parameters.AddWithValue("@Descuento_articulo_neto", b);
                    //VALOR DE UNIDADES A ACTUALIZAR
                    command8.Parameters.AddWithValue("@Importe_total_neto", c);
                    // Ejecuta la consulta de actualización

                }
                else
                {
                    decimal a = Math.Round(chido, 2);
                    decimal b = Math.Round(descuento, 2);
                    decimal c = Math.Round(importe_neto, 2);

                    command8.Parameters.AddWithValue("@Extra", a);
                    //VALOR DE LA POSICION DEL CODIGO
                    command8.Parameters.AddWithValue("@Posicion", GlobalSettings.Instance.Posicion);
                    //VALOR DE UNIDADES A ACTUALIZAR
                    command8.Parameters.AddWithValue("@Descuento_articulo_neto", b);
                    //VALOR DE UNIDADES A ACTUALIZAR
                    command8.Parameters.AddWithValue("@Importe_total_neto", c);
                    // Ejecuta la consulta de actualización
                }

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
                    //GlobalSettings.Instance.Impuesto_real = GlobalSettings.Instance.Importe_Total * 0.16m;
                }
                reader10.Close();
                int tercerDecimal2 = (int)(Math.Floor(importe_neto2 * 1000) % 10);
                if (tercerDecimal2 == 5)
                    importe_neto2 = Math.Ceiling(importe_neto2 * 100) / 100;
                decimal diferecia3 = importe_neto2;
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

                string query9 = "UPDATE DOCTOS_VE SET IMPORTE_NETO = @Importe, DSCTO_IMPORTE = @Desc_e, TOTAL_IMPUESTOS = @Impuestos WHERE DOCTO_VE_ID = '" + GlobalSettings.Instance.FolioId + "';";
                FbCommand command9 = new FbCommand(query9, con8);

                // Agrega los parámetros
                //VALOR DE UNIDADES A ACTUALIZAR
                if (GlobalSettings.Instance.PrimerImporte >= 1)
                {
                    GlobalSettings.Instance.Importe_Total_Anterior = GlobalSettings.Instance.Importe_Total;
                    GlobalSettings.Instance.Impuesto_Total_Anterior = GlobalSettings.Instance.Impuesto_total;

                }
                decimal nice = 0;
                decimal extra2 = 0;
                decimal impuesto = 0;
                if (GlobalSettings.Instance.Desc_extra_ind != 0)
                {
                    GlobalSettings.Instance.Importe_Total -= diferecia3;
                }
                else
                {
                    GlobalSettings.Instance.Importe_Total -= Math.Round(diferecia, 2);
                }
                if (GlobalSettings.Instance.Impuesto == "16% IVA ")
                {
                    //decimal importe_impuesto = importe_neto_solicitado * 1.16m;
                    //decimal dif = (importe_impuesto) - (importe_neto * 1.16m);
                    decimal dif = 0;
                    decimal difact = 0;
                    if (GlobalSettings.Instance.Desc_extra_ind != 0)
                    {
                        dif = (diferecia3 * 0.16m);
                        difact = dif;
                    }
                    else
                    {
                        dif = (diferecia * 0.16m);
                        difact = dif;
                    }
                    GlobalSettings.Instance.Impuesto_total -= Math.Round(difact, 2);
                    //IMPUESTO SIN MODIFICACIONES EN EL DET
                    GlobalSettings.Instance.Impuesto_real -= difact;
                    //GlobalSettings.Instance.Impuesto_total -= importe_neto;
                }
                else if (GlobalSettings.Instance.Impuesto == "IEPS 8%")
                {
                    //decimal importe_impuesto = importe_neto_solicitado * 1.16m;
                    //decimal dif = (importe_impuesto) - (importe_neto * 1.16m);
                    decimal dif = 0;
                    decimal difact = 0;
                    if (GlobalSettings.Instance.Desc_extra_ind != 0)
                    {
                        dif = (diferecia3 * 0.08m);
                        difact = dif;
                    }
                    else
                    {
                        dif = (diferecia * 0.08m);
                        difact = dif;
                    }
                    GlobalSettings.Instance.Impuesto_total -= Math.Round(difact, 2);
                    //IMPUESTO SIN MODIFICACIONES EN EL DET
                    GlobalSettings.Instance.Impuesto_real -= difact;
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
                int tercerDecimal3 = (int)(Math.Floor(GlobalSettings.Instance.Desc_extra_importe * 1000) % 10);
                if (tercerDecimal3 == 5)
                    GlobalSettings.Instance.Desc_extra_importe = Math.Ceiling(GlobalSettings.Instance.Desc_extra_importe * 100) / 100;
                if (GlobalSettings.Instance.Desc_extra_importe != 0)
                {
                    nice = SumarTodo();
                    extra2 = SumarExtra();
                    command9.Parameters.AddWithValue("@Desc_e", Math.Round(extra2, 2));
                    command9.Parameters.AddWithValue("@Importe", Math.Round(nice, 2));
                }
                else
                {
                    command9.Parameters.AddWithValue("@Desc_e", Math.Round(GlobalSettings.Instance.Desc_extra_importe, 2));
                    command9.Parameters.AddWithValue("@Importe", Math.Round(GlobalSettings.Instance.Importe_Total, 2));

                }
                command9.Parameters.AddWithValue("@Impuestos", Math.Round(GlobalSettings.Instance.Impuesto_real, 2));
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
            FbConnection con = new FbConnection("User=" + GlobalSettings.Instance.User + ";" + "Password=" + GlobalSettings.Instance.Pw + ";" + "Database=" + GlobalSettings.Instance.Direccion + ";" + "DataSource=" + GlobalSettings.Instance.Ip + ";" + "Port=" + GlobalSettings.Instance.Puerto + ";" + "Dialect=3;" + "Charset=UTF8;");
            try
            {
                con.Open();
                FbCommand command = new FbCommand("EXIVAL_ART", con);
                command.CommandType = CommandType.StoredProcedure;

                // Parámetros de entrada
                command.Parameters.Add("V_ARTICULO_ID", FbDbType.Integer).Value = articulo_id;
                command.Parameters.Add("V_ALMACEN_ID", FbDbType.Integer).Value = 108401; //PERI
                //command.Parameters.Add("V_ALMACEN_ID", FbDbType.Integer).Value = 108405; culiacan
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
                GlobalSettings.Instance.ExistenciaAl = Convert.ToInt32(command.Parameters[6].Value);
                int Existencia = Convert.ToInt32(command.Parameters[6].Value);

                FbCommand command2 = new FbCommand("EXIVAL_ART", con);
                command2.CommandType = CommandType.StoredProcedure;

                // Parámetros de entrada
                command2.Parameters.Add("V_ARTICULO_ID", FbDbType.Integer).Value = articulo_id;
                command2.Parameters.Add("V_ALMACEN_ID", FbDbType.Integer).Value = 108403; //PERI
                //command2.Parameters.Add("V_ALMACEN_ID", FbDbType.Integer).Value = 108405; culiacan
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
        public void Validar()
        {
            FbConnection con = new FbConnection("User=" + GlobalSettings.Instance.User + ";" + "Password=" + GlobalSettings.Instance.Pw + ";" + "Database=" + GlobalSettings.Instance.Direccion + ";" + "DataSource=" + GlobalSettings.Instance.Ip + ";" + "Port=" + GlobalSettings.Instance.Puerto + ";" + "Dialect=3;" + "Charset=UTF8;");
            try
            {
                con.Open();
                // Utiliza parámetros para evitar la inyección de SQL
                string query8 = "UPDATE LIBRES_PED_VE SET VERIFICADO_X_SOFTWARE = @UpdateValue WHERE DOCTO_VE_ID = @FolioId";
                FbCommand command8 = new FbCommand(query8, con);

                // Agrega los parámetros
                //VALOR DE UNIDADES A ACTUALIZAR
                command8.Parameters.AddWithValue("@UpdateValue", 1);
                //VALOR DE FOLIO ID A EDITAR EN DOCTOS_VE_DET
                command8.Parameters.AddWithValue("@FolioId", GlobalSettings.Instance.FolioId);
                // Ejecuta la consulta de actualización
                int rowsAffected = command8.ExecuteNonQuery();

                if (rowsAffected == 0)
                {
                    MessageBox.Show("No se pudo actualizar el pedido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                con.Close();
            }
        }
        class Art_Ex
        {
            public string codigo { get; set; }
            public decimal cantidad { get; set; }
        }
        public void CrearExcelLunesADomingo()
        {
            DateTime actual = DateTime.Now;
            DateTime fechaActual = DateTime.Now;

            // Calcular el día de la semana en formato numérico (lunes = 1, domingo = 7)
            int valor_dia = (int)actual.DayOfWeek;
            if (valor_dia == 0) valor_dia = 7; // Para que domingo sea 7

            // Calcular cuántos días hay que restar para llegar al lunes
            int diasRestar = valor_dia - 1;  // Restamos el día actual (lunes es 1)

            // Si hoy es lunes, empezamos la semana con la fecha de hoy
            DateTime inicioSemana = fechaActual.AddDays(-diasRestar);

            // El final de la semana es el domingo de esa semana (7 días después del lunes)
            DateTime finSemana = inicioSemana.AddDays(6);

            // Crear el nombre del archivo Excel con el rango de fechas (lunes a domingo)
            path = "\\\\SRVPRINCIPAL\\incompletosPedidos\\ArticulosIncompletos " +
                          inicioSemana.ToString("yyyy-MM-dd") +
                          " a " +
                          finSemana.ToString("yyyy-MM-dd") +
                          ".xlsx";

            // Aquí puedes agregar el código para crear el archivo Excel si es necesario
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
            DialogResult result = MessageBox.Show("¿Estás seguro que deseas terminar este pedido?\n Los artículos del pedido se van a modificar \n ¿Deseas continuar?", "Advertencia", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);
            if (result == DialogResult.Cancel)
            {
                return;
            }
            bool bandera = false;
            string mensajemax = "";
            //List<Art_Ex> Articulo = new List<Art_Ex>();
            MensajeCustom candado = new MensajeCustom();
            List<string> unicos = new List<string>();
            for (int i = 0; i < Articulos.Count; ++i)
            {
                if (Articulos[i].Solicitado > Articulos[i].Recibido)
                {
                    decimal existencia = ExistenciaValor(Articulos[i].ArticuloId.ToString());
                    decimal existenciatotal = existencia + GlobalSettings.Instance.ExistenciaAl;
                    decimal minimo_existencia = Articulos[i].Importe_neto_articulo;
                    if (((existenciatotal >= Articulos[i].Pendiente && existenciatotal > 48) || (minimo_existencia > 100) && existenciatotal >= 1) && !unicos.Contains(Articulos[i].Codigo) && !GlobalSettings.Instance.Excluidos.Contains(Articulos[i].Codigo) && Articulos[i].Codigo.Substring(0, 3) != "232" && Articulos[i].Codigo.Substring(0, 3) != "214" && Articulos[i].Codigo.Substring(0, 3) != "243")
                    {
                        decimal contador = 0;
                        foreach (var ar in Articulos)
                        {
                            if (ar.Codigo == Articulos[i].Codigo)
                            {
                                contador += ar.Pendiente;
                            }
                        }
                        unicos.Add(Articulos[i].Codigo);
                        bandera = true;
                        candado.Height += 25;
                        candado.GridEx.Rows.Add(Articulos[i].Codigo, Articulos[i].Descripcion, contador, existencia, GlobalSettings.Instance.ExistenciaAl, existenciatotal);
                        candado.GridEx.ClearSelection();
                        //Articulo.Add(new Art_Ex { codigo = Articulos[i].Codigo, cantidad = existenciatotal });
                        //string mensajepred = Articulos[i].Codigo + " ------- " + "Existencia Tienda: " + existencia+ "\n\t       Existencia Almacén: " + GlobalSettings.Instance.ExistenciaAl+ "\n\n";
                        //mensajemax += mensajepred;
                    }
                }
            }
            if (bandera == true)
            {
                //string mensaje1 = "!Estos códigos tienen existencia aún!\n\n";
                //string mensajefinal = mensaje1 + mensajemax;
                //MessageBox.Show(mensajefinal, "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                candado.ShowDialog();
                if (!GlobalSettings.Instance.aceptado)
                {
                    return;
                }
                //else
                //{
                //    GlobalSettings.Instance.aceptado = false;
                //}
            }
            Validar();
            string Hoy = DateTime.Now.ToString("d-M-yy");
            string filePath = "\\\\SRVPRINCIPAL\\incompletosPedidos\\ArticulosIncompletos " + Hoy + ".xlsx"; //PERI
            //string filePath = "C:\\incompletosPedidos\\ArticulosIncompletos " + Hoy + ".xlsx"; culiacan
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
            //style.Alignment.Horizontal = HorizontalAlignmentValues.Center;
            //style.Alignment.Vertical = VerticalAlignmentValues.Center;
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
            sl.SetCellValue("M2", "AUTORIZÓ");

            int[] columnas = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13 };
            foreach (int columna in columnas)
            {
                if (columna == 1)
                    sl.SetColumnWidth(columna, 11);
                if (columna == 2)
                    sl.SetColumnWidth(columna, 11);
                if (columna == 3 || columna == 5 || columna == 13 || columna == 12)
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
                    sl.SetCellValue("K" + fila, GlobalSettings.Instance.Existencia + GlobalSettings.Instance.ExistenciaAl);
                    if (Articulos[i].Recibido == 0)
                        sl.SetCellValue("J" + fila, "FALTANTE");
                    else
                        sl.SetCellValue("J" + fila, "INCOMPLETO");
                    if (GlobalSettings.Instance.aceptado)
                        sl.SetCellValue("M" + fila, "AUTORIZÓ: " + GlobalSettings.Instance.Usuario);
                    sl.SetCellValue("L" + fila, GlobalSettings.Instance.Vendedora);
                    fila++;
                }
            }
            GlobalSettings.Instance.ExistenciaQuery = false;

            sl.SaveAs(filePath);


            CrearExcelLunesADomingo();
            //SEMANAL
            bool fileExist0 = File.Exists(path);
            Document doc0 = new Document();
            try
            {
                if (!fileExist0)
                {
                    SLDocument oSLDocument0 = new();
                    oSLDocument0.SaveAs(path);
                }
            }
            catch (IOException ex)
            {
                // Maneja la excepción
                MessageBox.Show("Se produjo un error al acceder a la ubicación de red: " + ex.Message);
                // Aquí puedes realizar cualquier acción adicional, como cerrar la aplicación o retornar
                return;
            }
            SLDocument sl0 = new(path);
            //SLDocument excel = new(@"\\192.168.0.2\\incompletosPedidos\\ArticulosIncompletos");
            SLStyle style0 = sl0.CreateStyle();
            style0.Font.FontSize = 15;
            style0.Font.FontColor = System.Drawing.Color.Red;
            style0.Font.Bold = true;
            //style.Alignment.Horizontal = HorizontalAlignmentValues.Center;
            //style.Alignment.Vertical = VerticalAlignmentValues.Center;
            sl0.SetCellStyle("A1", style0);
            sl0.SetCellValue("A1", "REPORTE DE FALTANTES DE PEDIDOS");
            sl0.MergeWorksheetCells("A1", "I1");
            sl0.SetCellValue("A2", "FECHA");
            sl0.SetCellValue("B2", "PEDIDO");
            sl0.SetCellValue("C2", "SURTIDOR");
            sl0.SetCellValue("D2", "CODIGO");
            sl0.SetCellValue("E2", "DESCRIPCION");
            sl0.SetCellValue("F2", "SOLICITADO");
            sl0.SetCellValue("G2", "VERIFICADO");
            sl0.SetCellValue("H2", "NOTA");
            sl0.SetCellValue("I2", "IMPORTE");
            sl0.SetCellValue("J2", "ESTATUS");
            sl0.SetCellValue("K2", "EXISTENCIA");
            sl0.SetCellValue("L2", "VENDEDOR");
            sl0.SetCellValue("M2", "AUTORIZÓ");

            int[] columnas0 = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13 };
            foreach (int columna in columnas0)
            {
                if (columna == 1)
                    sl0.SetColumnWidth(columna, 11);
                if (columna == 2)
                    sl0.SetColumnWidth(columna, 11);
                if (columna == 3 || columna == 5 || columna == 13 || columna == 12)
                    sl0.SetColumnWidth(columna, 30);
                if (columna == 4)
                    sl0.SetColumnWidth(columna, 11);
                if (columna == 30)
                    sl0.SetColumnWidth(columna, 11);
                if (columna == 9 || columna == 6)
                    sl0.SetColumnWidth(columna, 11);
                if (columna == 7)
                    sl0.SetColumnWidth(columna, 11);
                if (columna == 8)
                    sl0.SetColumnWidth(columna, 11);
                if (columna == 9)
                    sl0.SetColumnWidth(columna, 20);
            }
            int fila0 = 3;
            while (sl0.HasCellValue("A" + fila0))
            {
                fila0++;
            }
            for (int i = 0; i < Articulos.Count(); i++)
            {
                if (Articulos[i].Solicitado > Articulos[i].Recibido)
                {
                    GlobalSettings.Instance.ExistenciaQuery = true;
                    Existencia(int.Parse(Articulos[i].ArticuloId));
                    sl0.SetCellValue("A" + fila0, DateTime.Now.ToShortDateString().ToString());
                    sl0.SetCellValue("B" + fila0, TxtPedido.Text);
                    sl0.SetCellValue("C" + fila0, Cb_Surtidor.Text);
                    sl0.SetCellValue("D" + fila0, Articulos[i].Codigo);
                    sl0.SetCellValue("E" + fila0, Articulos[i].Descripcion);
                    sl0.SetCellValue("F" + fila0, Articulos[i].Solicitado);
                    sl0.SetCellValue("G" + fila0, Articulos[i].Recibido);
                    sl0.SetCellValue("H" + fila0, Articulos[i].Nota);
                    sl0.SetCellValue("I" + fila0, Articulos[i].Importe * Articulos[i].Pendiente);
                    sl0.SetCellValue("K" + fila0, GlobalSettings.Instance.Existencia + GlobalSettings.Instance.ExistenciaAl);
                    if (Articulos[i].Recibido == 0)
                        sl0.SetCellValue("J" + fila0, "FALTANTE");
                    else
                        sl0.SetCellValue("J" + fila0, "INCOMPLETO");
                    if (GlobalSettings.Instance.aceptado)
                        sl0.SetCellValue("M" + fila0, "AUTORIZÓ: " + GlobalSettings.Instance.Usuario);
                    sl0.SetCellValue("L" + fila0, GlobalSettings.Instance.Vendedora);
                    fila0++;
                }
            }
            GlobalSettings.Instance.ExistenciaQuery = false;

            sl0.SaveAs(path);
            //FIN SEMANAL
            doc.SetMargins(0, 0, 10, 10);
            string fileName = "C:\\DatosPedidos\\" + TxtPedido.Text + ".pdf";
            //string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            //string filePath = Path.Combine(documentsPath, fileName);
        bool faltantes = false;
        using (var fs = new FileStream(fileName, FileMode.Create))
        {
            PdfWriter.GetInstance(doc, fs);
            doc.Open();

            // Crear una tabla para los datos correctos
            //PdfPTable table = new PdfPTable(Tabla.Columns.Count - 1);
            //PdfPCell cell = new PdfPCell(new Phrase("ARTÍCULOS CORRECTOS", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 16f, iTextSharp.text.Font.BOLD)));
            //cell.Colspan = 6;
            //cell.HorizontalAlignment = 1;
            //cell.PaddingBottom = 10f;
            //cell.PaddingTop = 10f;
            //table.AddCell(cell);
            float[] columnWidths = new float[] { 8f, 18f, 60f, 20f, 22f, 20f, 22f }; // Asumiendo que la segunda columna tendrá un ancho personalizado
            //table.SetWidths(columnWidths);
            // Crear una tabla para los datos faltantes
            PdfPTable table2 = new PdfPTable(Tabla.Columns.Count);
            PdfPCell cell2 = new PdfPCell(new Phrase("ARTÍCULOS INCOMPLETOS", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 9f, iTextSharp.text.Font.BOLD)));
            cell2.Colspan = 7;
            cell2.HorizontalAlignment = 1;
            cell2.PaddingBottom = 10f;
            cell2.PaddingTop = 10f;
            table2.AddCell(cell2);
            table2.SetWidths(columnWidths);

            PdfPTable table4 = new PdfPTable(Tabla.Columns.Count);
            PdfPCell cell4 = new PdfPCell(new Phrase("ARTÍCULOS A ELIMINAR", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 9f, iTextSharp.text.Font.BOLD)));
            cell4.Colspan = 7;
            cell4.HorizontalAlignment = 1;
            cell4.PaddingBottom = 10f;
            cell4.PaddingTop = 10f;
            table4.AddCell(cell4);
            table4.SetWidths(columnWidths);

            // Crear una tabla para los datos sobrantes
            PdfPTable table3 = new PdfPTable(Tabla.Columns.Count);
            PdfPCell cell3 = new PdfPCell(new Phrase("ARTÍCULOS SOBRANTES", new iTextSharp.text.Font(iTextSharp.text.Font.FontFamily.HELVETICA, 9f, iTextSharp.text.Font.BOLD)));
            cell3.Colspan = 7;
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
                if (i == 5)
                {
                    table2.AddCell("Existencia");
                    table3.AddCell("Existencia");
                    table4.AddCell("Existencia");
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
            bool banderaModificacion = false;
            for (int i = 0; i < Tabla.Rows.Count; i++)
            {
                double.TryParse(Tabla[3, i].Value.ToString(), out double valorColumna3);
                double.TryParse(Tabla[4, i].Value.ToString(), out double valorColumna4);
                for (int j = 0; j < Tabla.Columns.Count; j++)
                {
                    if (Tabla[3, i].Value.ToString() == Tabla[4, i].Value.ToString())
                    {
                        //table.AddCell(Tabla[j, i].Value.ToString());
                        //tabla1 = true;
                        //cont1++;
                    }
                    else if (valorColumna4 < valorColumna3)
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
                                        GlobalSettings.Instance.Desc_extra_ind = Articulos[k].Descuento_extra_individual;
                                    }
                                }
                                banderaModificacion = true;
                                EliminarQuery();
                            }
                            if (j == 5)
                            {
                                GlobalSettings.Instance.ExistenciaQuery = true;
                                Existencia(int.Parse(Articulos[i].ArticuloId));
                                Tabla[j, i].Value = GlobalSettings.Instance.Existencia + GlobalSettings.Instance.ExistenciaAl;
                            }
                            GlobalSettings.Instance.ExistenciaQuery = false;
                            table4.AddCell(Tabla[j, i].Value.ToString());
                            tabla4 = true;
                            cont4++;
                        }
                        else
                        {
                            if (j == 5)
                            {
                                GlobalSettings.Instance.ExistenciaQuery = true;
                                Existencia(int.Parse(Articulos[i].ArticuloId));
                                Tabla[j, i].Value = GlobalSettings.Instance.Existencia + GlobalSettings.Instance.ExistenciaAl;
                            }
                            GlobalSettings.Instance.ExistenciaQuery = false;
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
                                        GlobalSettings.Instance.Importearticuloeliminado = Articulos[k].Importe_total_articuloeliminado;
                                        GlobalSettings.Instance.Desc_extra_ind = Articulos[k].Descuento_extra_individual;
                                    }
                                }
                            }
                            if (j == 4)
                            {
                                //MessageBox.Show(Tabla[4, i].Value.ToString());
                                GlobalSettings.Instance.Update = Decimal.Parse(Tabla[4, i].Value.ToString());
                                UpdateQuery();
                                GlobalSettings.Instance.PrimerImporte += 1;
                                banderaModificacion = true;

                            }
                            tabla2 = true;
                            cont2++;

                        }
                    }
                    else if (valorColumna3 < valorColumna4)
                    {
                        if (j == 5)
                        {
                            GlobalSettings.Instance.ExistenciaQuery = true;
                            Existencia(int.Parse(Articulos[i].ArticuloId));
                            Tabla[j, i].Value = GlobalSettings.Instance.Existencia + GlobalSettings.Instance.ExistenciaAl;
                        }
                        GlobalSettings.Instance.ExistenciaQuery = false;
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
                                    GlobalSettings.Instance.Importearticuloeliminado = Articulos[k].Importe_total_articuloeliminado;
                                    GlobalSettings.Instance.Clave_articulo_id = Articulos[k].Clave;
                                    GlobalSettings.Instance.Desc_extra_ind = Articulos[k].Descuento_extra_individual;
                                }
                            }
                        }
                        if (j == 4)
                        {
                            //MessageBox.Show(Tabla[4, i].Value.ToString());
                            GlobalSettings.Instance.Update = Decimal.Parse(Tabla[4, i].Value.ToString());
                            UpdateQuery();
                            banderaModificacion = true;
                        }
                        tabla3 = true;
                        cont3++;
                    }
                }
            }
            if (banderaModificacion == true)
            {
                SumarTotales();
            }
            Paragraph Name = new Paragraph("PEDIDO: " + TxtPedido.Text);
            Name.Alignment = Element.ALIGN_CENTER;
            Paragraph contador4 = new Paragraph("Articulos Pendientes: " + (cont4 / 7).ToString());
            contador4.Alignment = Element.ALIGN_CENTER;
            Paragraph contador3 = new Paragraph("Articulos Sobrantes: " + (cont3 / 7).ToString());
            contador3.Alignment = Element.ALIGN_CENTER;
            Paragraph contador2 = new Paragraph("Articulos Incompletos: " + (cont2 / 7).ToString());
            contador2.Alignment = Element.ALIGN_CENTER;
            Paragraph contador1 = new Paragraph("Articulos Correctos: " + (cont1 / 7).ToString());
            contador1.Alignment = Element.ALIGN_CENTER;
            iTextSharp.text.Font customFont = FontFactory.GetFont("Arial", 9);
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
            else
            {
                //ENVIAR CORREO 
                faltantes = true;
            }
            //doc.Add(cell4);
            doc.Add(emptyParagraph);
            // Cerrar el documento
            doc.Close();
        }
            //TXT
            string carpeta = @"C:\DatosPedidos";
            string rutaArchivo = Path.Combine(carpeta, TxtPedido.Text + ".txt");
            try
            {
                // Verificar si la carpeta existe, si no, crearla
                if (!Directory.Exists(carpeta))
                {
                    Directory.CreateDirectory(carpeta);
                }

                // Crear un nuevo archivo de texto y escribir el contenido
                using (StreamWriter writer = new StreamWriter(rutaArchivo))
                {
                    for (int i = 0; i < Articulos.Count; ++i)
                    {
                        writer.WriteLine(Articulos[i].Id + "," + Articulos[i].Recibido);
                    }
                }

            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al guardar el archivo Txt: " + ex.Message);
            }
            //FIN TXT
            if (Cb_Descuentos.Checked == true)
                ValidarC(GlobalSettings.Instance.Docto_Ve_Id);
            if (faltantes == true)
            {
                try
                {
                    // Configura los detalles del remitente y destinatario
                    MailMessage mensaje = new MailMessage();
                    mensaje.From = new MailAddress("faltantes@papeleriacornejo.com"); // Tu dirección de correo
                    string Vendedor = RevisarVendedor(GlobalSettings.Instance.Vendedor);
                    mensaje.To.Add(Vendedor); // Destinatario
                    mensaje.Subject = "Faltante de Pedido "+TxtPedido.Text;
                    if(!GlobalSettings.Instance.aceptado)
                        mensaje.Body = "SURTIDOR: "+Cb_Surtidor.Text;
                    else
                    {
                        mensaje.Body = "SURTIDOR: "+Cb_Surtidor.Text+"\nAUTORIZÓ: " + GlobalSettings.Instance.Usuario;
                    }
                    string Pdf = fileName;
                    Attachment adjunto = new Attachment(Pdf);
                    mensaje.Attachments.Add(adjunto);
                    // Desactiva la validación del certificado para pruebas (no recomendado para producción)
                    System.Net.ServicePointManager.ServerCertificateValidationCallback =
                        (sender, certificate, chain, sslPolicyErrors) => true;

                    // Configuración del cliente SMTP (usando Gmail como ejemplo)
                    SmtpClient clienteSmtp = new SmtpClient("smtp.papeleriacornejo.com", 587); // Servidor SMTP y puerto
                    clienteSmtp.Credentials = new NetworkCredential("faltantes@papeleriacornejo.com", "Cornejo2024@"); // Credenciales
                    clienteSmtp.EnableSsl = true; // SSL para una conexión segura
                    // Envía el correo
                    clienteSmtp.Send(mensaje);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error.", ex.Message);
                }
            }
            var process = Process.Start(new ProcessStartInfo(fileName) { UseShellExecute = true });
            process.WaitForExit();
            TxtPedido.Text = string.Empty;
            TxtCodigo.Text = string.Empty;
            TxtPedido.Enabled = true;
            BtnPDF.Enabled = false;
            BtnPedido.Enabled = true;
            Lb_renglones.Text = "0";
            Lb_Incompletos.Text = "0";
            GlobalSettings.Instance.Existencia = 0;
            GlobalSettings.Instance.aceptado = false;
            GlobalSettings.Instance.ExistenciaAl = 0;
            GlobalSettings.Instance.Usuario = "";
            GlobalSettings.Instance.Incompletos = 0;
            GlobalSettings.Instance.Impuesto_real = 0;
            GlobalSettings.Instance.Impuesto_total = 0;
            GlobalSettings.Instance.Impuesto = "";
            GlobalSettings.Instance.Desc_extra_importe = 0;
            GlobalSettings.Instance.Desc_extra_ind = 0;
            Cb_Descuentos.Checked = true;
            Articulos.Clear();
            Tabla.Rows.Clear();
            Tabla.Refresh();
            TxtPedido.Focus();

        }
        public string RevisarVendedor(string Vendedor)
        {
            string archivo = "\\\\SRVPRINCIPAL\\Vendedores\\VENDEDORES.xlsx";
            using (SLDocument documento3 = new SLDocument(archivo))
            {
                int filas = documento3.GetWorksheetStatistics().NumberOfRows;
                for (int i = 1; i < filas + 1; ++i)
                {
                    string id = documento3.GetCellValueAsString("A" + i);
                    string correo = documento3.GetCellValueAsString("C" + i);
                    if (Vendedor == id)
                    {
                        return correo;
                    }
                }
                return "npacheco@papeleriacornejo.com";
            }
            //using (var workbook = new XLWorkbook(archivo))
            //{
            //    // Seleccionar la primera hoja de trabajo
            //    var worksheet = workbook.Worksheet(1);

            //    // Leer las celdas
            //    int row = 1;

            //    // Loop mientras haya datos en la primera columna de cada fila
            //    while (!worksheet.Cell(row, 1).IsEmpty())
            //    {
            //        // Leer datos de una columna (por ejemplo, columna A y B)
            //        string id = worksheet.Cell(row, 1).GetString(); // Columna A
            //        string correo = worksheet.Cell(row, 3).GetString(); // Columna B
            //        if(Vendedor == id)
            //        {
            //            return correo;
            //        }
            //        row++; 
            //    }
            //    return "npacheco@papeleriacornejo.com";
            //}
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
            if (e.Control && e.Alt && e.KeyCode == Keys.C)
            {
                DialogResult result = MessageBox.Show("Vas a cargar un archivo Txt\n ¿Deseas continuar?", "Advertencia", MessageBoxButtons.OKCancel, MessageBoxIcon.Warning, MessageBoxDefaultButton.Button2);
                if (result == DialogResult.Cancel)
                {
                    return;
                }
                Cargar_Click(sender, e);
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
                Cb_Descuentos.ForeColor = System.Drawing.Color.Red;
                Cargar.BackColor = System.Drawing.Color.Red;
                Tabla.ColumnHeadersDefaultCellStyle.BackColor = System.Drawing.Color.Red;
                Tabla.Refresh();

            }
            if (e.Alt && e.KeyCode == Keys.J)
            {
                BtnCodigo.BackColor = System.Drawing.Color.HotPink;
                BtnPDF.BackColor = System.Drawing.Color.HotPink;
                BtnPedido.BackColor = System.Drawing.Color.HotPink;
                label1.ForeColor = System.Drawing.Color.HotPink;
                label2.ForeColor = System.Drawing.Color.HotPink;
                label3.ForeColor = System.Drawing.Color.HotPink;
                label4.ForeColor = System.Drawing.Color.HotPink;
                label5.ForeColor = System.Drawing.Color.HotPink;
                label6.ForeColor = System.Drawing.Color.HotPink;
                label8.ForeColor = System.Drawing.Color.HotPink;
                Lb_Incompletos.ForeColor = System.Drawing.Color.HotPink;
                Lb_renglones.ForeColor = System.Drawing.Color.HotPink;
                Cargar.BackColor = System.Drawing.Color.HotPink;
                Cb_Descuentos.ForeColor = System.Drawing.Color.HotPink;
                Tabla.ColumnHeadersDefaultCellStyle.BackColor = System.Drawing.Color.HotPink;
                Tabla.Refresh();

            }
            if (e.Alt && e.KeyCode == Keys.V)
            {
                BtnCodigo.BackColor = System.Drawing.Color.BlueViolet;
                BtnPDF.BackColor = System.Drawing.Color.BlueViolet;
                BtnPedido.BackColor = System.Drawing.Color.BlueViolet;
                label1.ForeColor = System.Drawing.Color.BlueViolet;
                label2.ForeColor = System.Drawing.Color.BlueViolet;
                label3.ForeColor = System.Drawing.Color.BlueViolet;
                label4.ForeColor = System.Drawing.Color.BlueViolet;
                label5.ForeColor = System.Drawing.Color.BlueViolet;
                label6.ForeColor = System.Drawing.Color.BlueViolet;
                label8.ForeColor = System.Drawing.Color.BlueViolet;
                Lb_Incompletos.ForeColor = System.Drawing.Color.BlueViolet;
                Lb_renglones.ForeColor = System.Drawing.Color.BlueViolet;
                Cb_Descuentos.ForeColor = System.Drawing.Color.BlueViolet;
                Cargar.BackColor = System.Drawing.Color.BlueViolet;
                Tabla.ColumnHeadersDefaultCellStyle.BackColor = System.Drawing.Color.BlueViolet;
                Tabla.Refresh();

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
                Cb_Descuentos.ForeColor = System.Drawing.Color.CadetBlue;
                Lb_Incompletos.ForeColor = System.Drawing.Color.CadetBlue;
                Lb_renglones.ForeColor = System.Drawing.Color.CadetBlue;
                Cargar.BackColor = System.Drawing.Color.CadetBlue;
                Tabla.ColumnHeadersDefaultCellStyle.BackColor = System.Drawing.Color.CadetBlue;
                Tabla.Refresh();
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
                Cb_Descuentos.ForeColor = System.Drawing.Color.Fuchsia;
                Cargar.BackColor = System.Drawing.Color.Fuchsia;
                Tabla.ColumnHeadersDefaultCellStyle.BackColor = System.Drawing.Color.Fuchsia;
                Tabla.Refresh();

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
        public string Cliente(string Cliente_Id)
        {
            FbConnection con1a = new FbConnection("User=" + GlobalSettings.Instance.User + ";" + "Password=" + GlobalSettings.Instance.Pw + ";" + "Database=" + GlobalSettings.Instance.Direccion + ";" + "DataSource=" + GlobalSettings.Instance.Ip + ";" + "Port=" + GlobalSettings.Instance.Puerto + ";" + "Dialect=3;" + "Charset=UTF8;");
            try
            {
                con1a.Open();
                string query1 = "SELECT * FROM CLIENTES WHERE CLIENTE_ID = '" + Cliente_Id + "'";
                FbCommand command1 = new FbCommand(query1, con1a);
                FbDataReader reader1 = command1.ExecuteReader();
                if (reader1.Read())
                {
                    string nombre = reader1.GetString(1);
                    reader1.Close();
                    return nombre;

                }
                else
                {
                    MessageBox.Show("Folio no encontrado", "¡Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return "CLIENTE NO ENCONTRADO";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Se perdió la conexión :( , contacta a 06 o intenta de nuevo", "¡Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                MessageBox.Show(ex.ToString());
                return "CLIENTE NO ENCONTRADO";
            }
            finally
            {
                con1a.Close();
            }
        }
        public decimal ImpuestoC(string Articulo_Id)
        {
            FbConnection con2a = new FbConnection("User=" + GlobalSettings.Instance.User + ";" + "Password=" + GlobalSettings.Instance.Pw + ";" + "Database=" + GlobalSettings.Instance.Direccion + ";" + "DataSource=" + GlobalSettings.Instance.Ip + ";" + "Port=" + GlobalSettings.Instance.Puerto + ";" + "Dialect=3;" + "Charset=UTF8;");
            try
            {
                con2a.Open();
                string Clave_Impuesto = "";
                string impuesto = "";
                string query3 = "SELECT * FROM IMPUESTOS_ARTICULOS WHERE ARTICULO_ID = '" + Articulo_Id + "'";
                FbCommand command3 = new FbCommand(query3, con2a);
                FbDataReader reader3 = command3.ExecuteReader();
                if (reader3.Read())
                {
                    Clave_Impuesto = reader3.GetString(2);
                    //MessageBox.Show(GlobalSettings.Instance.Clave_impuesto);
                }
                reader3.Close();
                //QUERI 4 PARA BUSCAR IMPORTE DEL ARTICULO

                string query4 = "SELECT * FROM IMPUESTOS WHERE IMPUESTO_ID = '" + Clave_Impuesto + "'";
                FbCommand command4 = new FbCommand(query4, con2a);
                FbDataReader reader4 = command4.ExecuteReader();
                if (reader4.Read())
                {
                    impuesto = reader4.GetString(2);

                }
                reader4.Close();
                if (impuesto == "16% IVA ")
                    return (decimal)1.16;
                else if (impuesto == "IEPS 8%")
                    return (decimal)1.08;
                else
                    return 0;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Se perdió la conexión :( , contacta a 06 o intenta de nuevo", "¡Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                MessageBox.Show(ex.ToString());
                return (decimal)1.16;
            }
            finally
            {
                con2a.Close();
            }
        }
        public void UpdateIndividual()
        {
            FbConnection con3a = new FbConnection("User=" + GlobalSettings.Instance.User + ";" + "Password=" + GlobalSettings.Instance.Pw + ";" + "Database=" + GlobalSettings.Instance.Direccion + ";" + "DataSource=" + GlobalSettings.Instance.Ip + ";" + "Port=" + GlobalSettings.Instance.Puerto + ";" + "Dialect=3;" + "Charset=UTF8;");
            try
            {

                con3a.Open();
                string query8 = "UPDATE DOCTOS_VE SET DSCTO_PCTJE = @Desc, DSCTO_IMPORTE = @Descimp  WHERE DOCTO_VE_ID = @Docto_Det";
                FbCommand command8 = new FbCommand(query8, con3a);

                //decimal round = Decimal.Round(importeImpuesto, 4);
                //decimal round2 = Decimal.Round(nuevoPrecio, 4);
                command8.Parameters.AddWithValue("@Descimp", 0);
                command8.Parameters.AddWithValue("@Desc", 0);
                //VALOR DE FOLIO ID A EDITAR EN DOCTOS_VE_DET
                command8.Parameters.AddWithValue("@Docto_Det", GlobalSettings.Instance.Docto_Ve_Id);
                // Ejecuta la consulta de actualización
                int rowsAffected = command8.ExecuteNonQuery();

                if (rowsAffected == 0)
                {
                    MessageBox.Show("No se pudo actualizar el pedido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                con3a.Close();
            }
        }
        public void Update(string Docto_Ve_Det_Id, decimal Precio_neto, decimal Piezas, decimal impuesto, decimal Precio_Total_Neto, decimal Descuento)
        {
            FbConnection con4a = new FbConnection("User=" + GlobalSettings.Instance.User + ";" + "Password=" + GlobalSettings.Instance.Pw + ";" + "Database=" + GlobalSettings.Instance.Direccion + ";" + "DataSource=" + GlobalSettings.Instance.Ip + ";" + "Port=" + GlobalSettings.Instance.Puerto + ";" + "Dialect=3;" + "Charset=UTF8;");
            try
            {
                con4a.Open();
                string query8 = "UPDATE DOCTOS_VE_DET SET PCTJE_DSCTO = @Porcentaje_descuento,PCTJE_DSCTO_CLI = @Porcentaje_descuento_cli, DSCTO_ART = @Desc_art, DSCTO_EXTRA = @Desc_extra, PRECIO_UNITARIO = @NuevoPrecio WHERE DOCTO_VE_DET_ID = @Docto_Det";
                FbCommand command8 = new FbCommand(query8, con4a);
                //VALOR DE UNIDADES A ACTUALIZAR
                //MessageBox.Show(Precio_Total_Neto.ToString());
                decimal nuevoPrecio = (Precio_Total_Neto / Piezas);
                decimal descuento_decimal = Descuento / 100;
                //Descuento 40/100 = 0.4
                //Precio_neto *= 10000;
                //decimal Nprice = Precio_neto * 100;
                //decimal nuevoImpuesto = Nprice - (Precio_neto * Descuento);
                //decimal chido = nuevoImpuesto / 1000000;
                decimal chido = Precio_Total_Neto / Piezas;
                decimal chido2 = 0;
                //if (GlobalSettings.Instance.Desc_extra != 0)
                //{
                //    chido *= 10000;
                //    decimal Nprice2 = chido * 100;
                //    decimal nuevoImpuesto2 = Nprice2 - (chido * GlobalSettings.Instance.Desc_extra);
                //    chido2 = nuevoImpuesto2 / 1000000;
                //}
                //decimal truncado = Math.Truncate(chido * 1000) / 1000;
                //decimal round = Decimal.Round(importeImpuesto, 4);
                //decimal round2 = Decimal.Round(nuevoPrecio, 4);
                command8.Parameters.AddWithValue("@Porcentaje_descuento", 0);
                command8.Parameters.AddWithValue("@Porcentaje_descuento_cli", 0);
                if (GlobalSettings.Instance.Desc_extra != 0)
                {
                    command8.Parameters.AddWithValue("@NuevoPrecio", chido);
                    UpdateIndividual();
                }
                else
                {
                    command8.Parameters.AddWithValue("@NuevoPrecio", chido);

                }
                //command8.Parameters.AddWithValue("@ImporteImpuesto", Math.Round(chido, 2));
                command8.Parameters.AddWithValue("@Desc_art", 0);
                command8.Parameters.AddWithValue("@Desc_extra", 0);
                //VALOR DE FOLIO ID A EDITAR EN DOCTOS_VE_DET
                command8.Parameters.AddWithValue("@Docto_Det", Docto_Ve_Det_Id);
                // Ejecuta la consulta de actualización
                int rowsAffected = command8.ExecuteNonQuery();

                if (rowsAffected == 0)
                {
                    MessageBox.Show("No se pudo actualizar el pedido", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                con4a.Close();
            }
        }
        public void ValidarC(string Docto_Ve_Id)
        {

            FbConnection con5a = new FbConnection("User=" + GlobalSettings.Instance.User + ";" + "Password=" + GlobalSettings.Instance.Pw + ";" + "Database=" + GlobalSettings.Instance.Direccion + ";" + "DataSource=" + GlobalSettings.Instance.Ip + ";" + "Port=" + GlobalSettings.Instance.Puerto + ";" + "Dialect=3;" + "Charset=UTF8;");
            try
            {
                con5a.Open();
                // Utiliza parámetros para evitar la inyección de SQL
                string query7 = "SELECT * FROM DOCTOS_VE_DET  WHERE DOCTO_VE_ID = '" + Docto_Ve_Id + "';";
                FbCommand command0 = new FbCommand(query7, con5a);
                FbDataReader reader0 = command0.ExecuteReader();
                string Docto_Ve_Det_Id = "0";
                string Articulo_Id;
                decimal Piezas = 0;
                decimal Precio_total_neto = 0;
                decimal impuesto = 0;
                decimal sumatoria = 0;
                decimal Precio_neto = 0;
                decimal sumatoriai = 0;
                decimal sumatorian = 0;
                decimal Descuento = 0;
                string Codigo = "";
                int contador = 0;
                string carpeta = @"C:\DatosVentasSurtido";
                string rutaArchivo = Path.Combine(carpeta, TxtPedido.Text + ".txt");
                try
                {
                    // Verificar si la carpeta existe, si no, crearla
                    if (!Directory.Exists(carpeta))
                    {
                        Directory.CreateDirectory(carpeta);
                    }

                    // Crear un nuevo archivo de texto y escribir el contenido
                    using (StreamWriter writer = new StreamWriter(rutaArchivo))
                    {
                        while (reader0.Read())
                        {
                            Docto_Ve_Det_Id = reader0.GetString(0);
                            Articulo_Id = reader0.GetString(3);
                            Codigo = reader0.GetString(2);
                            Piezas = reader0.GetDecimal(4);
                            Precio_neto = reader0.GetDecimal(8);
                            Descuento = reader0.GetDecimal(9);
                            Precio_total_neto = reader0.GetDecimal(15);
                            //impuesto = Impuesto(Articulo_Id);
                            Update(Docto_Ve_Det_Id, Precio_neto, Piezas, impuesto, Precio_total_neto, Descuento);
                            writer.WriteLine(Codigo + "," + Piezas);
                        }
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error al guardar el archivo Txt: " + ex.Message);
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
                con5a.Close();
            }
        }

        private void Cargar_Click(object sender, EventArgs e)
        {
            if (Cancelado.Text == string.Empty)
            {
                MessageBox.Show("Ingresa un folio correcto", "¡Error!", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            string carpeta = @"C:\DatosPedidos";
            string rutaArchivo = Path.Combine(carpeta, Cancelado.Text + ".txt");
            if (File.Exists(rutaArchivo))
            {
                using (StreamReader reader = new StreamReader(rutaArchivo))
                {
                    string line;

                    while ((line = reader.ReadLine()) != null)
                    {
                        // Dividir la línea por un separador (por ejemplo, una coma)
                        string[] parts = line.Split(',');
                        int id = int.Parse(parts[0]);
                        decimal cantidad = decimal.Parse(parts[1]);
                        for (int i = 0; i < Articulos.Count; ++i)
                        {
                            if (Articulos[i].Id == id)
                            {
                                //Articulos[i].Recibido = recibido;
                                //ejecutar(recibido, id);
                                bool banderaincompleto = false;
                                bool temporal = false;
                                bool temporal2 = false;
                                decimal prueba;
                                bool bandera = false;
                                bool regresar = false;
                                prueba = Articulos[i].Pendiente - cantidad;
                                if (prueba == 0 && Articulos[i].Recibido > 0)
                                    temporal = true;
                                if (prueba == 0)
                                    bandera = true;
                                if ((prueba) >= 1 && Articulos[i].Recibido == 0)
                                    banderaincompleto = true;
                                Articulos[i].Recibido += cantidad;
                                Articulos[i].Pendiente -= cantidad;

                                if (Articulos[i].Pendiente < 0)
                                    Articulos[i].Pendiente = 0;
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
                                for (int j = 0; j < Articulos.Count; ++j)
                                {
                                    ListaTabla.Add(int.Parse(Tabla.Rows[j].Cells[0].Value.ToString()));
                                }
                                Tabla.Rows.Clear();
                                DataGridViewRowCollection rows = Tabla.Rows;
                                string comentario;
                                for (int k = 0; k < Articulos.Count; ++k)
                                {
                                    int a = 1;
                                    if (ListaTabla[k] != k + 1)
                                    {
                                        a = ListaTabla[k] - k;
                                    }
                                    if (Articulos[ListaTabla[k] - a].Nota != "")
                                    {
                                        comentario = "Ver";
                                    }
                                    else
                                    {
                                        comentario = string.Empty;
                                    }
                                    rows.Add(Articulos[ListaTabla[k] - a].Id, Articulos[ListaTabla[k] - a].Codigo, Articulos[ListaTabla[k] - a].Descripcion, Articulos[ListaTabla[k] - a].Solicitado, Articulos[ListaTabla[k] - a].Recibido, comentario, Articulos[ListaTabla[k] - a].Pendiente);
                                    DataGridViewRow row = Tabla.Rows[k];
                                    if (Articulos[ListaTabla[k] - a].Solicitado - Articulos[ListaTabla[k] - a].Recibido > 0 && Articulos[ListaTabla[k] - a].Recibido != 0)
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
                                    else if (Articulos[ListaTabla[k] - a].Solicitado - Articulos[ListaTabla[k] - a].Recibido == 0)
                                        row.DefaultCellStyle.BackColor = System.Drawing.Color.LightGreen;
                                    else if (Articulos[ListaTabla[k] - a].Solicitado - Articulos[ListaTabla[k] - a].Recibido < 0)
                                    {
                                        row.DefaultCellStyle.BackColor = System.Drawing.Color.Red;
                                        row.DefaultCellStyle.ForeColor = System.Drawing.Color.White;
                                    }
                                }
                                break;
                            }
                        }

                    }
                }
            }
            else
                MessageBox.Show("No existe un archivo con ese folio", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

        }
    }
}