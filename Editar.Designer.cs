namespace Validación_de_Pedidos
{
    partial class Editar
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
            flowLayoutPanel1 = new FlowLayoutPanel();
            LbCodigo = new Label();
            LbRecibido = new Label();
            label4 = new Label();
            label3 = new Label();
            Cantidad = new NumericUpDown();
            LbSolicitado = new Label();
            label1 = new Label();
            label2 = new Label();
            label5 = new Label();
            flowLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)Cantidad).BeginInit();
            SuspendLayout();
            // 
            // flowLayoutPanel1
            // 
            flowLayoutPanel1.Controls.Add(LbCodigo);
            flowLayoutPanel1.Location = new Point(69, 15);
            flowLayoutPanel1.Name = "flowLayoutPanel1";
            flowLayoutPanel1.Size = new Size(589, 61);
            flowLayoutPanel1.TabIndex = 18;
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
            // LbRecibido
            // 
            LbRecibido.AutoSize = true;
            LbRecibido.Font = new Font("Segoe UI", 14.25F, FontStyle.Bold, GraphicsUnit.Point);
            LbRecibido.ForeColor = Color.White;
            LbRecibido.Location = new Point(202, 120);
            LbRecibido.Name = "LbRecibido";
            LbRecibido.Size = new Size(103, 25);
            LbRecibido.TabIndex = 17;
            LbRecibido.Text = "RECIBIDO:";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Font = new Font("Segoe UI", 14.25F, FontStyle.Bold, GraphicsUnit.Point);
            label4.ForeColor = Color.White;
            label4.Location = new Point(72, 120);
            label4.Name = "label4";
            label4.Size = new Size(103, 25);
            label4.TabIndex = 16;
            label4.Text = "RECIBIDO:";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Font = new Font("Segoe UI", 14.25F, FontStyle.Bold, GraphicsUnit.Point);
            label3.ForeColor = Color.White;
            label3.Location = new Point(486, 202);
            label3.Name = "label3";
            label3.Size = new Size(76, 25);
            label3.TabIndex = 15;
            label3.Text = "PIEZAS";
            // 
            // Cantidad
            // 
            Cantidad.Font = new Font("Arial Black", 12F, FontStyle.Bold, GraphicsUnit.Point);
            Cantidad.Location = new Point(340, 197);
            Cantidad.Maximum = new decimal(new int[] { 10000, 0, 0, 0 });
            Cantidad.Name = "Cantidad";
            Cantidad.Size = new Size(120, 30);
            Cantidad.TabIndex = 14;
            Cantidad.KeyDown += Cantidad_KeyDown;
            // 
            // LbSolicitado
            // 
            LbSolicitado.AutoSize = true;
            LbSolicitado.Font = new Font("Segoe UI", 14.25F, FontStyle.Bold, GraphicsUnit.Point);
            LbSolicitado.ForeColor = Color.White;
            LbSolicitado.Location = new Point(202, 79);
            LbSolicitado.Name = "LbSolicitado";
            LbSolicitado.Size = new Size(127, 25);
            LbSolicitado.TabIndex = 12;
            LbSolicitado.Text = "SOLICITADO:";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Segoe UI", 14.25F, FontStyle.Bold, GraphicsUnit.Point);
            label1.ForeColor = Color.White;
            label1.Location = new Point(69, 79);
            label1.Name = "label1";
            label1.Size = new Size(127, 25);
            label1.TabIndex = 10;
            label1.Text = "SOLICITADO:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 14.25F, FontStyle.Bold, GraphicsUnit.Point);
            label2.ForeColor = Color.White;
            label2.Location = new Point(226, 197);
            label2.Name = "label2";
            label2.Size = new Size(103, 25);
            label2.TabIndex = 19;
            label2.Text = "RECIBIDO:";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Font = new Font("Segoe UI", 14.25F, FontStyle.Bold, GraphicsUnit.Point);
            label5.ForeColor = Color.Yellow;
            label5.Location = new Point(246, 155);
            label5.Name = "label5";
            label5.Size = new Size(282, 25);
            label5.TabIndex = 20;
            label5.Text = "Cantidad que desea modificar:";
            // 
            // Editar
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(33, 33, 33);
            ClientSize = new Size(763, 231);
            Controls.Add(label5);
            Controls.Add(label2);
            Controls.Add(flowLayoutPanel1);
            Controls.Add(LbRecibido);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(Cantidad);
            Controls.Add(LbSolicitado);
            Controls.Add(label1);
            Name = "Editar";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Editar";
            flowLayoutPanel1.ResumeLayout(false);
            flowLayoutPanel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)Cantidad).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private FlowLayoutPanel flowLayoutPanel1;
        private Label LbCodigo;
        private Label LbRecibido;
        private Label label4;
        private Label label3;
        private NumericUpDown Cantidad;
        private Label LbSolicitado;
        private Label label1;
        private Label label2;
        private Label label5;
    }
}