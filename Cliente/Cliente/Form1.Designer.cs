namespace WindowsFormsApplication1
{
    partial class Form1
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
            this.buttonRegistrarte = new System.Windows.Forms.Button();
            this.buttonIniciarSesion = new System.Windows.Forms.Button();
            this.textBoxUsuario = new System.Windows.Forms.TextBox();
            this.textBoxContraseña = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.dataGridView1 = new System.Windows.Forms.DataGridView();
            this.buttonCerrarSesion = new System.Windows.Forms.Button();
            this.buttonInvitacionAJugar = new System.Windows.Forms.Button();
            this.checkedListBox1 = new System.Windows.Forms.CheckedListBox();
            this.buttonInvitar = new System.Windows.Forms.Button();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.label3 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).BeginInit();
            this.SuspendLayout();
            // 
            // buttonRegistrarte
            // 
            this.buttonRegistrarte.Location = new System.Drawing.Point(158, 381);
            this.buttonRegistrarte.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.buttonRegistrarte.Name = "buttonRegistrarte";
            this.buttonRegistrarte.Size = new System.Drawing.Size(140, 49);
            this.buttonRegistrarte.TabIndex = 0;
            this.buttonRegistrarte.Text = "Registrarte";
            this.buttonRegistrarte.UseVisualStyleBackColor = true;
            this.buttonRegistrarte.Click += new System.EventHandler(this.buttonRegistrarte_Click);
            // 
            // buttonIniciarSesion
            // 
            this.buttonIniciarSesion.Location = new System.Drawing.Point(158, 235);
            this.buttonIniciarSesion.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.buttonIniciarSesion.Name = "buttonIniciarSesion";
            this.buttonIniciarSesion.Size = new System.Drawing.Size(140, 52);
            this.buttonIniciarSesion.TabIndex = 1;
            this.buttonIniciarSesion.Text = "Iniciar sesion";
            this.buttonIniciarSesion.UseVisualStyleBackColor = true;
            this.buttonIniciarSesion.Click += new System.EventHandler(this.buttonIniciarSesion_Click);
            // 
            // textBoxUsuario
            // 
            this.textBoxUsuario.Location = new System.Drawing.Point(250, 98);
            this.textBoxUsuario.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.textBoxUsuario.Name = "textBoxUsuario";
            this.textBoxUsuario.Size = new System.Drawing.Size(140, 26);
            this.textBoxUsuario.TabIndex = 2;
            // 
            // textBoxContraseña
            // 
            this.textBoxContraseña.Location = new System.Drawing.Point(250, 163);
            this.textBoxContraseña.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.textBoxContraseña.Name = "textBoxContraseña";
            this.textBoxContraseña.Size = new System.Drawing.Size(140, 26);
            this.textBoxContraseña.TabIndex = 3;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(86, 94);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(102, 29);
            this.label1.TabIndex = 4;
            this.label1.Text = "Usuario:";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(86, 159);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(130, 29);
            this.label2.TabIndex = 5;
            this.label2.Text = "Contraeña:";
            // 
            // dataGridView1
            // 
            this.dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dataGridView1.Location = new System.Drawing.Point(491, 94);
            this.dataGridView1.Name = "dataGridView1";
            this.dataGridView1.RowTemplate.Height = 28;
            this.dataGridView1.Size = new System.Drawing.Size(240, 150);
            this.dataGridView1.TabIndex = 8;
            // 
            // buttonCerrarSesion
            // 
            this.buttonCerrarSesion.Location = new System.Drawing.Point(158, 311);
            this.buttonCerrarSesion.Name = "buttonCerrarSesion";
            this.buttonCerrarSesion.Size = new System.Drawing.Size(140, 45);
            this.buttonCerrarSesion.TabIndex = 9;
            this.buttonCerrarSesion.Text = "Cerrar sesion";
            this.buttonCerrarSesion.UseVisualStyleBackColor = true;
            this.buttonCerrarSesion.Click += new System.EventHandler(this.buttonCerrarSesion_Click);
            // 
            // buttonInvitacionAJugar
            // 
            this.buttonInvitacionAJugar.Location = new System.Drawing.Point(158, 455);
            this.buttonInvitacionAJugar.Name = "buttonInvitacionAJugar";
            this.buttonInvitacionAJugar.Size = new System.Drawing.Size(140, 42);
            this.buttonInvitacionAJugar.TabIndex = 10;
            this.buttonInvitacionAJugar.Text = "Invitar a jugar";
            this.buttonInvitacionAJugar.UseVisualStyleBackColor = true;
            this.buttonInvitacionAJugar.Click += new System.EventHandler(this.buttonInvitacionAJugar_Click);
            // 
            // checkedListBox1
            // 
            this.checkedListBox1.FormattingEnabled = true;
            this.checkedListBox1.Location = new System.Drawing.Point(491, 289);
            this.checkedListBox1.Name = "checkedListBox1";
            this.checkedListBox1.Size = new System.Drawing.Size(223, 151);
            this.checkedListBox1.TabIndex = 11;
            // 
            // buttonInvitar
            // 
            this.buttonInvitar.Location = new System.Drawing.Point(491, 462);
            this.buttonInvitar.Name = "buttonInvitar";
            this.buttonInvitar.Size = new System.Drawing.Size(94, 35);
            this.buttonInvitar.TabIndex = 12;
            this.buttonInvitar.Text = "Invitar";
            this.buttonInvitar.UseVisualStyleBackColor = true;
            this.buttonInvitar.Click += new System.EventHandler(this.buttonInvitar_Click);
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(396, 163);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(22, 21);
            this.checkBox1.TabIndex = 13;
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(480, 31);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(51, 20);
            this.label3.TabIndex = 14;
            this.label3.Text = "label3";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(845, 559);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.buttonInvitar);
            this.Controls.Add(this.checkedListBox1);
            this.Controls.Add(this.buttonInvitacionAJugar);
            this.Controls.Add(this.buttonCerrarSesion);
            this.Controls.Add(this.dataGridView1);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBoxContraseña);
            this.Controls.Add(this.textBoxUsuario);
            this.Controls.Add(this.buttonIniciarSesion);
            this.Controls.Add(this.buttonRegistrarte);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.dataGridView1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button buttonRegistrarte;
        private System.Windows.Forms.Button buttonIniciarSesion;
        private System.Windows.Forms.TextBox textBoxUsuario;
        private System.Windows.Forms.TextBox textBoxContraseña;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.DataGridView dataGridView1;
        private System.Windows.Forms.Button buttonCerrarSesion;
        private System.Windows.Forms.Button buttonInvitacionAJugar;
        private System.Windows.Forms.CheckedListBox checkedListBox1;
        private System.Windows.Forms.Button buttonInvitar;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.Label label3;
    }
}

