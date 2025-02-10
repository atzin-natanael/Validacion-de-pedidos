namespace Validación_de_Pedidos
{
    partial class ControlAcceso
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
            Aceptar = new Button();
            TxtContrasenia = new TextBox();
            label1 = new Label();
            Exit = new Button();
            SuspendLayout();
            // 
            // Aceptar
            // 
            Aceptar.Anchor = AnchorStyles.Top;
            Aceptar.BackColor = Color.FromArgb(50, 50, 50);
            Aceptar.Cursor = Cursors.Hand;
            Aceptar.FlatStyle = FlatStyle.Flat;
            Aceptar.Font = new Font("Cambria", 14.25F, FontStyle.Regular, GraphicsUnit.Point);
            Aceptar.ForeColor = Color.White;
            Aceptar.Location = new Point(258, 72);
            Aceptar.Name = "Aceptar";
            Aceptar.Size = new Size(119, 41);
            Aceptar.TabIndex = 2;
            Aceptar.Text = "Aceptar";
            Aceptar.UseVisualStyleBackColor = false;
            Aceptar.Click += Aceptar_Click;
            // 
            // TxtContrasenia
            // 
            TxtContrasenia.Anchor = AnchorStyles.Top;
            TxtContrasenia.Font = new Font("Cambria", 14.25F, FontStyle.Regular, GraphicsUnit.Point);
            TxtContrasenia.Location = new Point(44, 79);
            TxtContrasenia.Name = "TxtContrasenia";
            TxtContrasenia.PlaceholderText = "Contraseña";
            TxtContrasenia.Size = new Size(191, 30);
            TxtContrasenia.TabIndex = 1;
            TxtContrasenia.UseSystemPasswordChar = true;
            TxtContrasenia.KeyDown += TxtContrasenia_KeyDown;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Font = new Font("Cambria", 14.25F, FontStyle.Regular, GraphicsUnit.Point);
            label1.Location = new Point(149, 19);
            label1.Name = "label1";
            label1.Size = new Size(102, 22);
            label1.TabIndex = 2;
            label1.Text = "Contraseña";
            // 
            // Exit
            // 
            Exit.Anchor = AnchorStyles.Top;
            Exit.BackColor = Color.FromArgb(50, 50, 50);
            Exit.Cursor = Cursors.Hand;
            Exit.FlatStyle = FlatStyle.Flat;
            Exit.Font = new Font("Cambria", 14.25F, FontStyle.Regular, GraphicsUnit.Point);
            Exit.ForeColor = Color.White;
            Exit.Location = new Point(336, 3);
            Exit.Name = "Exit";
            Exit.Size = new Size(76, 29);
            Exit.TabIndex = 3;
            Exit.Text = "Salir";
            Exit.TextAlign = ContentAlignment.TopCenter;
            Exit.UseVisualStyleBackColor = false;
            Exit.Click += Exit_Click;
            // 
            // ControlAcceso
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(413, 169);
            Controls.Add(Exit);
            Controls.Add(label1);
            Controls.Add(TxtContrasenia);
            Controls.Add(Aceptar);
            FormBorderStyle = FormBorderStyle.None;
            Name = "ControlAcceso";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "ControlAcceso";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button Aceptar;
        private TextBox TxtContrasenia;
        private Label label1;
        private Button Exit;
    }
}