namespace Validación_de_Pedidos
{
    partial class MensajeCustom
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
            panel1 = new Panel();
            Back = new Button();
            Aclaracion = new Button();
            label1 = new Label();
            GridEx = new DataGridView();
            Column1 = new DataGridViewTextBoxColumn();
            Column6 = new DataGridViewTextBoxColumn();
            Column2 = new DataGridViewTextBoxColumn();
            Column3 = new DataGridViewTextBoxColumn();
            Column4 = new DataGridViewTextBoxColumn();
            Column5 = new DataGridViewTextBoxColumn();
            panel2 = new Panel();
            ((System.ComponentModel.ISupportInitialize)GridEx).BeginInit();
            panel2.SuspendLayout();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.BackColor = Color.CadetBlue;
            panel1.Dock = DockStyle.Top;
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(711, 35);
            panel1.TabIndex = 0;
            panel1.MouseDown += panel1_MouseDown;
            // 
            // Back
            // 
            Back.BackColor = SystemColors.ActiveCaptionText;
            Back.Cursor = Cursors.Hand;
            Back.FlatStyle = FlatStyle.Flat;
            Back.Font = new Font("Segoe UI", 11F, FontStyle.Regular, GraphicsUnit.Point);
            Back.ForeColor = Color.White;
            Back.Location = new Point(406, 3);
            Back.Name = "Back";
            Back.Size = new Size(101, 30);
            Back.TabIndex = 2;
            Back.Text = "Volver";
            Back.UseVisualStyleBackColor = false;
            Back.Click += Back_Click;
            // 
            // Aclaracion
            // 
            Aclaracion.BackColor = SystemColors.ActiveCaptionText;
            Aclaracion.Cursor = Cursors.Hand;
            Aclaracion.FlatStyle = FlatStyle.Flat;
            Aclaracion.Font = new Font("Segoe UI", 11F, FontStyle.Regular, GraphicsUnit.Point);
            Aclaracion.ForeColor = Color.White;
            Aclaracion.Location = new Point(174, 3);
            Aclaracion.Name = "Aclaracion";
            Aclaracion.Size = new Size(147, 30);
            Aclaracion.TabIndex = 1;
            Aclaracion.Text = "Solicitar Permiso";
            Aclaracion.UseVisualStyleBackColor = false;
            Aclaracion.Click += Aclaracion_Click;
            // 
            // label1
            // 
            label1.Anchor = AnchorStyles.Top;
            label1.AutoSize = true;
            label1.BackColor = SystemColors.ButtonShadow;
            label1.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point);
            label1.ForeColor = Color.Crimson;
            label1.Location = new Point(226, 38);
            label1.Name = "label1";
            label1.Size = new Size(253, 21);
            label1.TabIndex = 0;
            label1.Text = "Estos árticulos tienen existencia";
            // 
            // GridEx
            // 
            GridEx.AllowUserToAddRows = false;
            GridEx.AllowUserToDeleteRows = false;
            GridEx.AllowUserToResizeRows = false;
            GridEx.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            GridEx.BackgroundColor = SystemColors.ButtonShadow;
            GridEx.BorderStyle = BorderStyle.None;
            GridEx.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            GridEx.Columns.AddRange(new DataGridViewColumn[] { Column1, Column6, Column2, Column3, Column4, Column5 });
            dataGridViewCellStyle1.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = SystemColors.Window;
            dataGridViewCellStyle1.Font = new Font("Segoe UI", 9F, FontStyle.Regular, GraphicsUnit.Point);
            dataGridViewCellStyle1.ForeColor = SystemColors.ControlText;
            dataGridViewCellStyle1.SelectionBackColor = SystemColors.Window;
            dataGridViewCellStyle1.SelectionForeColor = SystemColors.ControlText;
            dataGridViewCellStyle1.WrapMode = DataGridViewTriState.False;
            GridEx.DefaultCellStyle = dataGridViewCellStyle1;
            GridEx.Enabled = false;
            GridEx.GridColor = SystemColors.ButtonShadow;
            GridEx.Location = new Point(50, 72);
            GridEx.Name = "GridEx";
            GridEx.ReadOnly = true;
            GridEx.RowHeadersVisible = false;
            GridEx.RowTemplate.Height = 25;
            GridEx.Size = new Size(620, 46);
            GridEx.TabIndex = 0;
            // 
            // Column1
            // 
            Column1.HeaderText = "Codigo";
            Column1.Name = "Column1";
            Column1.ReadOnly = true;
            Column1.Width = 90;
            // 
            // Column6
            // 
            Column6.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            Column6.HeaderText = "Descripcion";
            Column6.Name = "Column6";
            Column6.ReadOnly = true;
            // 
            // Column2
            // 
            Column2.HeaderText = "Cantidad";
            Column2.Name = "Column2";
            Column2.ReadOnly = true;
            Column2.Width = 60;
            // 
            // Column3
            // 
            Column3.HeaderText = "ExTienda";
            Column3.Name = "Column3";
            Column3.ReadOnly = true;
            Column3.Width = 60;
            // 
            // Column4
            // 
            Column4.HeaderText = "ExAlmacen";
            Column4.Name = "Column4";
            Column4.ReadOnly = true;
            Column4.Width = 70;
            // 
            // Column5
            // 
            Column5.HeaderText = "Total";
            Column5.Name = "Column5";
            Column5.ReadOnly = true;
            Column5.Width = 70;
            // 
            // panel2
            // 
            panel2.Controls.Add(Aclaracion);
            panel2.Controls.Add(Back);
            panel2.Dock = DockStyle.Bottom;
            panel2.Location = new Point(0, 184);
            panel2.Name = "panel2";
            panel2.Size = new Size(711, 40);
            panel2.TabIndex = 3;
            // 
            // MensajeCustom
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.ButtonShadow;
            ClientSize = new Size(711, 224);
            Controls.Add(panel2);
            Controls.Add(GridEx);
            Controls.Add(label1);
            Controls.Add(panel1);
            FormBorderStyle = FormBorderStyle.None;
            Name = "MensajeCustom";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "MensajeCustom";
            ((System.ComponentModel.ISupportInitialize)GridEx).EndInit();
            panel2.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Panel panel1;
        private Button Back;
        private Button Aclaracion;
        private Label label1;
        public DataGridView GridEx;
        private DataGridViewTextBoxColumn Column1;
        private DataGridViewTextBoxColumn Column6;
        private DataGridViewTextBoxColumn Column2;
        private DataGridViewTextBoxColumn Column3;
        private DataGridViewTextBoxColumn Column4;
        private DataGridViewTextBoxColumn Column5;
        private Panel panel2;
    }
}