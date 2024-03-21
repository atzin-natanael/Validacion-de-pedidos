namespace Validación_de_Pedidos
{
    partial class Mensaje
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
            LblTitulo = new Label();
            panel1 = new Panel();
            panel2 = new Panel();
            Btn_Aceptar = new Button();
            richTextBox1 = new RichTextBox();
            panel1.SuspendLayout();
            panel2.SuspendLayout();
            SuspendLayout();
            // 
            // LblTitulo
            // 
            LblTitulo.Anchor = AnchorStyles.Top;
            LblTitulo.AutoSize = true;
            LblTitulo.Font = new Font("Arial", 12F, FontStyle.Bold, GraphicsUnit.Point);
            LblTitulo.ForeColor = Color.CadetBlue;
            LblTitulo.Location = new Point(12, 9);
            LblTitulo.Name = "LblTitulo";
            LblTitulo.Size = new Size(54, 19);
            LblTitulo.TabIndex = 1;
            LblTitulo.Text = "label2";
            LblTitulo.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // panel1
            // 
            panel1.BackColor = Color.FromArgb(30, 30, 30);
            panel1.Controls.Add(LblTitulo);
            panel1.Cursor = Cursors.Hand;
            panel1.Dock = DockStyle.Top;
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(343, 34);
            panel1.TabIndex = 3;
            panel1.MouseDown += panel1_MouseDown;
            panel1.MouseMove += panel1_MouseMove;
            panel1.MouseUp += panel1_MouseUp;
            // 
            // panel2
            // 
            panel2.Controls.Add(Btn_Aceptar);
            panel2.Dock = DockStyle.Bottom;
            panel2.Location = new Point(0, 160);
            panel2.Name = "panel2";
            panel2.Size = new Size(343, 43);
            panel2.TabIndex = 5;
            // 
            // Btn_Aceptar
            // 
            Btn_Aceptar.Anchor = AnchorStyles.Top;
            Btn_Aceptar.BackColor = Color.FromArgb(30, 30, 30);
            Btn_Aceptar.Cursor = Cursors.Hand;
            Btn_Aceptar.FlatStyle = FlatStyle.Flat;
            Btn_Aceptar.Font = new Font("Arial", 12F, FontStyle.Regular, GraphicsUnit.Point);
            Btn_Aceptar.ForeColor = Color.White;
            Btn_Aceptar.Location = new Point(112, 3);
            Btn_Aceptar.Name = "Btn_Aceptar";
            Btn_Aceptar.Size = new Size(117, 34);
            Btn_Aceptar.TabIndex = 2;
            Btn_Aceptar.Text = "Aceptar";
            Btn_Aceptar.UseVisualStyleBackColor = false;
            Btn_Aceptar.Click += Btn_Aceptar_Click;
            // 
            // richTextBox1
            // 
            richTextBox1.BackColor = Color.FromArgb(33, 33, 33);
            richTextBox1.BorderStyle = BorderStyle.None;
            richTextBox1.Font = new Font("Arial", 12F, FontStyle.Regular, GraphicsUnit.Point);
            richTextBox1.ForeColor = Color.White;
            richTextBox1.Location = new Point(5, 49);
            richTextBox1.Name = "richTextBox1";
            richTextBox1.Size = new Size(326, 105);
            richTextBox1.TabIndex = 6;
            richTextBox1.Text = "PRUEBA";
            // 
            // Mensaje
            // 
            AutoScaleDimensions = new SizeF(11F, 22F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(33, 33, 33);
            ClientSize = new Size(343, 203);
            ControlBox = false;
            Controls.Add(richTextBox1);
            Controls.Add(panel2);
            Controls.Add(panel1);
            Font = new Font("Arial", 14.25F, FontStyle.Regular, GraphicsUnit.Point);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Margin = new Padding(5, 4, 5, 4);
            Name = "Mensaje";
            StartPosition = FormStartPosition.CenterScreen;
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            panel2.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion
        private Label LblTitulo;
        private Panel panel1;
        private Panel panel2;
        private Button Btn_Aceptar;
        private RichTextBox richTextBox1;
    }
}