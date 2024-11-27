namespace Validación_de_Pedidos
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle3 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle4 = new DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            Tabla = new DataGridView();
            Id = new DataGridViewTextBoxColumn();
            Código = new DataGridViewTextBoxColumn();
            Descripcion = new DataGridViewTextBoxColumn();
            Cantidad_Solicitada = new DataGridViewTextBoxColumn();
            Cantidad_Revisada = new DataGridViewTextBoxColumn();
            Nota = new DataGridViewTextBoxColumn();
            Cantidad_Pendiente = new DataGridViewTextBoxColumn();
            label1 = new Label();
            BtnPedido = new Button();
            BtnCodigo = new Button();
            TxtPedido = new TextBox();
            TxtCodigo = new TextBox();
            label2 = new Label();
            label3 = new Label();
            BtnPDF = new Button();
            label4 = new Label();
            Cb_Surtidor = new ComboBox();
            label5 = new Label();
            label6 = new Label();
            Lb_renglones = new Label();
            Lb_Incompletos = new Label();
            label8 = new Label();
            Cargar = new Button();
            Cancelado = new TextBox();
            Cb_Descuentos = new CheckBox();
            ((System.ComponentModel.ISupportInitialize)Tabla).BeginInit();
            SuspendLayout();
            // 
            // Tabla
            // 
            Tabla.AllowUserToAddRows = false;
            Tabla.AllowUserToDeleteRows = false;
            Tabla.AllowUserToOrderColumns = true;
            dataGridViewCellStyle1.BackColor = Color.LightGray;
            Tabla.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            Tabla.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            Tabla.BackgroundColor = Color.FromArgb(40, 40, 40);
            Tabla.BorderStyle = BorderStyle.None;
            Tabla.CellBorderStyle = DataGridViewCellBorderStyle.RaisedHorizontal;
            Tabla.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = Color.CadetBlue;
            dataGridViewCellStyle2.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Bold, GraphicsUnit.Point);
            dataGridViewCellStyle2.ForeColor = Color.White;
            dataGridViewCellStyle2.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = DataGridViewTriState.True;
            Tabla.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            Tabla.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            Tabla.Columns.AddRange(new DataGridViewColumn[] { Id, Código, Descripcion, Cantidad_Solicitada, Cantidad_Revisada, Nota, Cantidad_Pendiente });
            Tabla.Cursor = Cursors.Hand;
            dataGridViewCellStyle3.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = Color.White;
            dataGridViewCellStyle3.Font = new Font("Arial", 14.25F, FontStyle.Regular, GraphicsUnit.Point);
            dataGridViewCellStyle3.ForeColor = Color.Black;
            dataGridViewCellStyle3.SelectionBackColor = Color.CadetBlue;
            dataGridViewCellStyle3.SelectionForeColor = Color.White;
            dataGridViewCellStyle3.WrapMode = DataGridViewTriState.False;
            Tabla.DefaultCellStyle = dataGridViewCellStyle3;
            Tabla.EnableHeadersVisualStyles = false;
            Tabla.Location = new Point(14, 234);
            Tabla.Margin = new Padding(5, 4, 5, 4);
            Tabla.Name = "Tabla";
            Tabla.ReadOnly = true;
            dataGridViewCellStyle4.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = SystemColors.ActiveCaption;
            dataGridViewCellStyle4.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Bold, GraphicsUnit.Point);
            dataGridViewCellStyle4.ForeColor = SystemColors.WindowText;
            dataGridViewCellStyle4.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle4.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle4.WrapMode = DataGridViewTriState.True;
            Tabla.RowHeadersDefaultCellStyle = dataGridViewCellStyle4;
            Tabla.RowHeadersVisible = false;
            Tabla.RowTemplate.Height = 35;
            Tabla.Size = new Size(980, 494);
            Tabla.TabIndex = 0;
            Tabla.CellClick += Tabla_CellClick;
            Tabla.CellMouseDown += Tabla_CellMouseDown;
            Tabla.KeyDown += Tabla_KeyDown;
            // 
            // Id
            // 
            Id.HeaderText = "Id";
            Id.Name = "Id";
            Id.ReadOnly = true;
            Id.Width = 50;
            // 
            // Código
            // 
            Código.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            Código.HeaderText = "Código";
            Código.Name = "Código";
            Código.ReadOnly = true;
            // 
            // Descripcion
            // 
            Descripcion.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            Descripcion.HeaderText = "Descripción";
            Descripcion.MinimumWidth = 220;
            Descripcion.Name = "Descripcion";
            Descripcion.ReadOnly = true;
            // 
            // Cantidad_Solicitada
            // 
            Cantidad_Solicitada.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            Cantidad_Solicitada.HeaderText = "Cantidad Solicitada";
            Cantidad_Solicitada.Name = "Cantidad_Solicitada";
            Cantidad_Solicitada.ReadOnly = true;
            // 
            // Cantidad_Revisada
            // 
            Cantidad_Revisada.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
            Cantidad_Revisada.HeaderText = "Cantidad Revisada";
            Cantidad_Revisada.Name = "Cantidad_Revisada";
            Cantidad_Revisada.ReadOnly = true;
            // 
            // Nota
            // 
            Nota.HeaderText = "Nota";
            Nota.Name = "Nota";
            Nota.ReadOnly = true;
            // 
            // Cantidad_Pendiente
            // 
            Cantidad_Pendiente.HeaderText = "Cantidad Pendiente";
            Cantidad_Pendiente.Name = "Cantidad_Pendiente";
            Cantidad_Pendiente.ReadOnly = true;
            // 
            // label1
            // 
            label1.Anchor = AnchorStyles.Top;
            label1.AutoSize = true;
            label1.BackColor = Color.Transparent;
            label1.Font = new Font("Verdana", 18F, FontStyle.Bold, GraphicsUnit.Point);
            label1.ForeColor = Color.CadetBlue;
            label1.Location = new Point(318, 3);
            label1.Margin = new Padding(5, 0, 5, 0);
            label1.Name = "label1";
            label1.Size = new Size(363, 29);
            label1.TabIndex = 1;
            label1.Text = "VALIDACIÓN DE PEDIDOS";
            label1.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // BtnPedido
            // 
            BtnPedido.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            BtnPedido.BackColor = Color.CadetBlue;
            BtnPedido.Cursor = Cursors.Hand;
            BtnPedido.FlatStyle = FlatStyle.Flat;
            BtnPedido.Font = new Font("Arial Black", 11.25F, FontStyle.Bold, GraphicsUnit.Point);
            BtnPedido.Location = new Point(825, 20);
            BtnPedido.Margin = new Padding(5, 4, 5, 4);
            BtnPedido.Name = "BtnPedido";
            BtnPedido.Size = new Size(169, 54);
            BtnPedido.TabIndex = 2;
            BtnPedido.Text = "Añadir Pedido";
            BtnPedido.UseVisualStyleBackColor = false;
            BtnPedido.Click += BtnPedido_Click;
            // 
            // BtnCodigo
            // 
            BtnCodigo.Anchor = AnchorStyles.Top;
            BtnCodigo.BackColor = Color.CadetBlue;
            BtnCodigo.Cursor = Cursors.Hand;
            BtnCodigo.FlatStyle = FlatStyle.Flat;
            BtnCodigo.Font = new Font("Arial Black", 11.25F, FontStyle.Bold, GraphicsUnit.Point);
            BtnCodigo.Location = new Point(670, 140);
            BtnCodigo.Margin = new Padding(5, 4, 5, 4);
            BtnCodigo.Name = "BtnCodigo";
            BtnCodigo.Size = new Size(169, 54);
            BtnCodigo.TabIndex = 5;
            BtnCodigo.Text = "Agregar";
            BtnCodigo.UseVisualStyleBackColor = false;
            BtnCodigo.Click += BtnCodigo_Click;
            // 
            // TxtPedido
            // 
            TxtPedido.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            TxtPedido.BorderStyle = BorderStyle.FixedSingle;
            TxtPedido.Font = new Font("Arial", 15.75F, FontStyle.Regular, GraphicsUnit.Point);
            TxtPedido.Location = new Point(680, 42);
            TxtPedido.Margin = new Padding(5, 4, 5, 4);
            TxtPedido.Name = "TxtPedido";
            TxtPedido.Size = new Size(135, 32);
            TxtPedido.TabIndex = 1;
            TxtPedido.KeyDown += TxtPedido_KeyDown;
            TxtPedido.KeyPress += TxtPedido_KeyPress;
            // 
            // TxtCodigo
            // 
            TxtCodigo.Anchor = AnchorStyles.Top;
            TxtCodigo.BorderStyle = BorderStyle.FixedSingle;
            TxtCodigo.Font = new Font("Arial", 15.75F, FontStyle.Regular, GraphicsUnit.Point);
            TxtCodigo.Location = new Point(153, 152);
            TxtCodigo.Margin = new Padding(5, 4, 5, 4);
            TxtCodigo.Name = "TxtCodigo";
            TxtCodigo.Size = new Size(481, 32);
            TxtCodigo.TabIndex = 4;
            TxtCodigo.KeyDown += TxtCodigo_KeyDown;
            // 
            // label2
            // 
            label2.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            label2.AutoSize = true;
            label2.Font = new Font("Tahoma", 14.25F, FontStyle.Bold, GraphicsUnit.Point);
            label2.ForeColor = Color.CadetBlue;
            label2.Location = new Point(582, 49);
            label2.Margin = new Padding(5, 0, 5, 0);
            label2.Name = "label2";
            label2.Size = new Size(82, 23);
            label2.TabIndex = 6;
            label2.Text = "Pedido:";
            // 
            // label3
            // 
            label3.Anchor = AnchorStyles.Top;
            label3.AutoSize = true;
            label3.Font = new Font("Tahoma", 14.25F, FontStyle.Bold, GraphicsUnit.Point);
            label3.ForeColor = Color.CadetBlue;
            label3.Location = new Point(14, 159);
            label3.Margin = new Padding(5, 0, 5, 0);
            label3.Name = "label3";
            label3.Size = new Size(102, 23);
            label3.TabIndex = 7;
            label3.Text = "Escanear:";
            // 
            // BtnPDF
            // 
            BtnPDF.BackColor = Color.CadetBlue;
            BtnPDF.Cursor = Cursors.Hand;
            BtnPDF.FlatStyle = FlatStyle.Flat;
            BtnPDF.Font = new Font("Arial Black", 11.25F, FontStyle.Bold, GraphicsUnit.Point);
            BtnPDF.Location = new Point(14, 36);
            BtnPDF.Margin = new Padding(5, 4, 5, 4);
            BtnPDF.Name = "BtnPDF";
            BtnPDF.Size = new Size(169, 54);
            BtnPDF.TabIndex = 8;
            BtnPDF.Text = "Terminar Pedido";
            BtnPDF.UseVisualStyleBackColor = false;
            BtnPDF.Click += BtnPDF_Click;
            // 
            // label4
            // 
            label4.Anchor = AnchorStyles.Top;
            label4.AutoSize = true;
            label4.Font = new Font("Tahoma", 14.25F, FontStyle.Bold, GraphicsUnit.Point);
            label4.ForeColor = Color.CadetBlue;
            label4.Location = new Point(153, 89);
            label4.Margin = new Padding(5, 0, 5, 0);
            label4.Name = "label4";
            label4.Size = new Size(95, 23);
            label4.TabIndex = 9;
            label4.Text = "Surtidor:";
            // 
            // Cb_Surtidor
            // 
            Cb_Surtidor.Anchor = AnchorStyles.Top;
            Cb_Surtidor.FlatStyle = FlatStyle.Flat;
            Cb_Surtidor.FormattingEnabled = true;
            Cb_Surtidor.Location = new Point(260, 84);
            Cb_Surtidor.Name = "Cb_Surtidor";
            Cb_Surtidor.Size = new Size(374, 30);
            Cb_Surtidor.TabIndex = 3;
            Cb_Surtidor.KeyDown += Cb_Surtidor_KeyDown;
            Cb_Surtidor.KeyPress += Cb_Surtidor_KeyPress;
            Cb_Surtidor.Leave += Cb_Surtidor_Leave;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.FlatStyle = FlatStyle.Flat;
            label5.ForeColor = Color.CadetBlue;
            label5.Location = new Point(14, 3);
            label5.Name = "label5";
            label5.Size = new Size(0, 22);
            label5.TabIndex = 10;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Font = new Font("Tahoma", 14.25F, FontStyle.Bold, GraphicsUnit.Point);
            label6.ForeColor = Color.CadetBlue;
            label6.Location = new Point(14, 206);
            label6.Name = "label6";
            label6.Size = new Size(228, 23);
            label6.TabIndex = 11;
            label6.Text = "Renglones pendientes:";
            // 
            // Lb_renglones
            // 
            Lb_renglones.AutoSize = true;
            Lb_renglones.Font = new Font("Tahoma", 14.25F, FontStyle.Bold, GraphicsUnit.Point);
            Lb_renglones.ForeColor = Color.CadetBlue;
            Lb_renglones.Location = new Point(260, 206);
            Lb_renglones.Name = "Lb_renglones";
            Lb_renglones.Size = new Size(22, 23);
            Lb_renglones.TabIndex = 12;
            Lb_renglones.Text = "0";
            // 
            // Lb_Incompletos
            // 
            Lb_Incompletos.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            Lb_Incompletos.AutoSize = true;
            Lb_Incompletos.Font = new Font("Tahoma", 14.25F, FontStyle.Bold, GraphicsUnit.Point);
            Lb_Incompletos.ForeColor = Color.CadetBlue;
            Lb_Incompletos.Location = new Point(960, 198);
            Lb_Incompletos.Name = "Lb_Incompletos";
            Lb_Incompletos.Size = new Size(22, 23);
            Lb_Incompletos.TabIndex = 14;
            Lb_Incompletos.Text = "0";
            // 
            // label8
            // 
            label8.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            label8.AutoSize = true;
            label8.Font = new Font("Tahoma", 14.25F, FontStyle.Bold, GraphicsUnit.Point);
            label8.ForeColor = Color.CadetBlue;
            label8.Location = new Point(689, 198);
            label8.Name = "label8";
            label8.Size = new Size(240, 23);
            label8.TabIndex = 13;
            label8.Text = "Renglones incompletos:";
            // 
            // Cargar
            // 
            Cargar.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            Cargar.BackColor = Color.CadetBlue;
            Cargar.Cursor = Cursors.Hand;
            Cargar.FlatStyle = FlatStyle.Flat;
            Cargar.Font = new Font("Arial Black", 11.25F, FontStyle.Bold, GraphicsUnit.Point);
            Cargar.Location = new Point(900, 140);
            Cargar.Margin = new Padding(5, 4, 5, 4);
            Cargar.Name = "Cargar";
            Cargar.Size = new Size(94, 42);
            Cargar.TabIndex = 15;
            Cargar.Text = "Cargar";
            Cargar.UseVisualStyleBackColor = false;
            Cargar.Click += Cargar_Click;
            // 
            // Cancelado
            // 
            Cancelado.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            Cancelado.BorderStyle = BorderStyle.FixedSingle;
            Cancelado.Font = new Font("Arial", 15.75F, FontStyle.Regular, GraphicsUnit.Point);
            Cancelado.Location = new Point(859, 100);
            Cancelado.Margin = new Padding(5, 4, 5, 4);
            Cancelado.Name = "Cancelado";
            Cancelado.Size = new Size(135, 32);
            Cancelado.TabIndex = 16;
            // 
            // Cb_Descuentos
            // 
            Cb_Descuentos.AutoSize = true;
            Cb_Descuentos.Checked = true;
            Cb_Descuentos.CheckState = CheckState.Checked;
            Cb_Descuentos.Font = new Font("Tahoma", 14.25F, FontStyle.Bold, GraphicsUnit.Point);
            Cb_Descuentos.ForeColor = Color.CadetBlue;
            Cb_Descuentos.Location = new Point(14, 121);
            Cb_Descuentos.Name = "Cb_Descuentos";
            Cb_Descuentos.Size = new Size(139, 27);
            Cb_Descuentos.TabIndex = 17;
            Cb_Descuentos.Text = "Descuentos";
            Cb_Descuentos.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(11F, 22F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(33, 33, 33);
            ClientSize = new Size(1008, 729);
            Controls.Add(Cb_Descuentos);
            Controls.Add(Cancelado);
            Controls.Add(Cargar);
            Controls.Add(Lb_Incompletos);
            Controls.Add(label8);
            Controls.Add(Lb_renglones);
            Controls.Add(label6);
            Controls.Add(label5);
            Controls.Add(Cb_Surtidor);
            Controls.Add(label4);
            Controls.Add(BtnPDF);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(TxtCodigo);
            Controls.Add(TxtPedido);
            Controls.Add(BtnCodigo);
            Controls.Add(BtnPedido);
            Controls.Add(label1);
            Controls.Add(Tabla);
            Font = new Font("Arial", 14.25F, FontStyle.Regular, GraphicsUnit.Point);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(5, 4, 5, 4);
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Developed by Atzin Not Found V13";
            WindowState = FormWindowState.Maximized;
            Load += Form1_Load;
            KeyDown += Form1_KeyDown;
            ((System.ComponentModel.ISupportInitialize)Tabla).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private DataGridView Tabla;
        private Label label1;
        private Button BtnPedido;
        private Button BtnCodigo;
        private TextBox TxtPedido;
        private TextBox TxtCodigo;
        private Label label2;
        private Label label3;
        private DataGridViewTextBoxColumn Id;
        private DataGridViewTextBoxColumn Código;
        private DataGridViewTextBoxColumn Descripcion;
        private DataGridViewTextBoxColumn Cantidad_Solicitada;
        private DataGridViewTextBoxColumn Cantidad_Revisada;
        private DataGridViewTextBoxColumn Nota;
        private DataGridViewTextBoxColumn Cantidad_Pendiente;
        private Button BtnPDF;
        private Label label4;
        private ComboBox Cb_Surtidor;
        private Label label5;
        private Label label6;
        private Label Lb_renglones;
        private Label Lb_Incompletos;
        private Label label8;
        private Button Cargar;
        private TextBox Cancelado;
        private CheckBox Cb_Descuentos;
    }
}