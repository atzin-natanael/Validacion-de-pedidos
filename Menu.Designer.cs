namespace Validación_de_Pedidos
{
    partial class Menu
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
            LbCodigo = new Label();
            label1 = new Label();
            label2 = new Label();
            LbSolicitado = new Label();
            LbPendiente = new Label();
            Cantidad = new NumericUpDown();
            label3 = new Label();
            label4 = new Label();
            LbRecibido = new Label();
            flowLayoutPanel1 = new FlowLayoutPanel();
            flowLayoutPanel2 = new FlowLayoutPanel();
            label5 = new Label();
            Lb_Nota = new Label();
            ((System.ComponentModel.ISupportInitialize)Cantidad).BeginInit();
            flowLayoutPanel1.SuspendLayout();
            flowLayoutPanel2.SuspendLayout();
            SuspendLayout();
            // 
            // LbCodigo
            // 
            LbCodigo.Anchor = AnchorStyles.Top;
            LbCodigo.AutoSize = true;
            LbCodigo.Font = new Font("Arial", 14.25F, FontStyle.Regular, GraphicsUnit.Point);
            LbCodigo.ForeColor = Color.White;
            LbCodigo.Location = new Point(3, 0);
            LbCodigo.Name = "LbCodigo";
            LbCodigo.Size = new Size(43, 22);
            LbCodigo.TabIndex = 0;
            LbCodigo.Text = "222";
            LbCodigo.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 14.25F, FontStyle.Bold, GraphicsUnit.Point);
            label1.ForeColor = Color.White;
            label1.Location = new Point(267, 162);
            label1.Name = "label1";
            label1.Size = new Size(127, 25);
            label1.TabIndex = 1;
            label1.Text = "SOLICITADO:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 14.25F, FontStyle.Bold, GraphicsUnit.Point);
            label2.ForeColor = Color.White;
            label2.Location = new Point(274, 233);
            label2.Name = "label2";
            label2.Size = new Size(120, 25);
            label2.TabIndex = 2;
            label2.Text = "PENDIENTE:";
            // 
            // LbSolicitado
            // 
            LbSolicitado.AutoSize = true;
            LbSolicitado.Font = new Font("Segoe UI", 14.25F, FontStyle.Bold, GraphicsUnit.Point);
            LbSolicitado.ForeColor = Color.White;
            LbSolicitado.Location = new Point(409, 162);
            LbSolicitado.Name = "LbSolicitado";
            LbSolicitado.Size = new Size(127, 25);
            LbSolicitado.TabIndex = 3;
            LbSolicitado.Text = "SOLICITADO:";
            // 
            // LbPendiente
            // 
            LbPendiente.AutoSize = true;
            LbPendiente.Font = new Font("Segoe UI", 14.25F, FontStyle.Bold, GraphicsUnit.Point);
            LbPendiente.ForeColor = Color.White;
            LbPendiente.Location = new Point(409, 233);
            LbPendiente.Name = "LbPendiente";
            LbPendiente.Size = new Size(127, 25);
            LbPendiente.TabIndex = 4;
            LbPendiente.Text = "SOLICITADO:";
            // 
            // Cantidad
            // 
            Cantidad.Font = new Font("Arial Black", 12F, FontStyle.Bold, GraphicsUnit.Point);
            Cantidad.Location = new Point(307, 273);
            Cantidad.Maximum = new decimal(new int[] { 10000, 0, 0, 0 });
            Cantidad.Name = "Cantidad";
            Cantidad.Size = new Size(120, 30);
            Cantidad.TabIndex = 5;
            Cantidad.KeyDown += Cantidad_KeyDown;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 14.25F, FontStyle.Bold, GraphicsUnit.Point);
            label3.ForeColor = Color.White;
            label3.Location = new Point(433, 278);
            label3.Name = "label3";
            label3.Size = new Size(76, 25);
            label3.TabIndex = 6;
            label3.Text = "PIEZAS";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Segoe UI", 14.25F, FontStyle.Bold, GraphicsUnit.Point);
            label4.ForeColor = Color.White;
            label4.Location = new Point(291, 198);
            label4.Name = "label4";
            label4.Size = new Size(103, 25);
            label4.TabIndex = 7;
            label4.Text = "RECIBIDO:";
            // 
            // LbRecibido
            // 
            LbRecibido.AutoSize = true;
            LbRecibido.Font = new Font("Segoe UI", 14.25F, FontStyle.Bold, GraphicsUnit.Point);
            LbRecibido.ForeColor = Color.White;
            LbRecibido.Location = new Point(409, 198);
            LbRecibido.Name = "LbRecibido";
            LbRecibido.Size = new Size(103, 25);
            LbRecibido.TabIndex = 8;
            LbRecibido.Text = "RECIBIDO:";
            // 
            // flowLayoutPanel1
            // 
            flowLayoutPanel1.Controls.Add(LbCodigo);
            flowLayoutPanel1.Location = new Point(12, 21);
            flowLayoutPanel1.Name = "flowLayoutPanel1";
            flowLayoutPanel1.Size = new Size(773, 61);
            flowLayoutPanel1.TabIndex = 9;
            // 
            // flowLayoutPanel2
            // 
            flowLayoutPanel2.Controls.Add(label5);
            flowLayoutPanel2.Controls.Add(Lb_Nota);
            flowLayoutPanel2.Location = new Point(12, 87);
            flowLayoutPanel2.Name = "flowLayoutPanel2";
            flowLayoutPanel2.Size = new Size(773, 72);
            flowLayoutPanel2.TabIndex = 10;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(3, 0);
            label5.Name = "label5";
            label5.Size = new Size(0, 15);
            label5.TabIndex = 0;
            // 
            // Lb_Nota
            // 
            Lb_Nota.Anchor = AnchorStyles.Top;
            Lb_Nota.AutoSize = true;
            Lb_Nota.Font = new Font("Arial", 14.25F, FontStyle.Regular, GraphicsUnit.Point);
            Lb_Nota.ForeColor = Color.White;
            Lb_Nota.Location = new Point(9, 0);
            Lb_Nota.Name = "Lb_Nota";
            Lb_Nota.Size = new Size(43, 22);
            Lb_Nota.TabIndex = 1;
            Lb_Nota.Text = "222";
            Lb_Nota.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // Menu
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(33, 33, 33);
            ClientSize = new Size(797, 315);
            Controls.Add(flowLayoutPanel2);
            Controls.Add(flowLayoutPanel1);
            Controls.Add(LbRecibido);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(Cantidad);
            Controls.Add(LbPendiente);
            Controls.Add(LbSolicitado);
            Controls.Add(label2);
            Controls.Add(label1);
            FormBorderStyle = FormBorderStyle.None;
            Name = "Menu";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Menu";
            ((System.ComponentModel.ISupportInitialize)Cantidad).EndInit();
            flowLayoutPanel1.ResumeLayout(false);
            flowLayoutPanel1.PerformLayout();
            flowLayoutPanel2.ResumeLayout(false);
            flowLayoutPanel2.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label LbCodigo;
        private Label label1;
        private Label label2;
        private Label LbSolicitado;
        private Label LbPendiente;
        private NumericUpDown Cantidad;
        private Label label3;
        private Label label4;
        private Label LbRecibido;
        private FlowLayoutPanel flowLayoutPanel1;
        private FlowLayoutPanel flowLayoutPanel2;
        private Label label5;
        private Label Lb_Nota;
    }
}