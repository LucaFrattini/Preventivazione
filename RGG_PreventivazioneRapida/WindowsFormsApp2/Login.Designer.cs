namespace Preventivazione_RGG
{
    partial class Login
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Login));
            this.textBoxUtente = new System.Windows.Forms.TextBox();
            this.Accedi = new System.Windows.Forms.Button();
            this.textBoxPassword = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // textBoxUtente
            // 
            this.textBoxUtente.Location = new System.Drawing.Point(104, 28);
            this.textBoxUtente.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.textBoxUtente.MaxLength = 18;
            this.textBoxUtente.Name = "textBoxUtente";
            this.textBoxUtente.Size = new System.Drawing.Size(197, 30);
            this.textBoxUtente.TabIndex = 10;
            // 
            // Accedi
            // 
            this.Accedi.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Accedi.BackColor = System.Drawing.Color.GreenYellow;
            this.Accedi.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Accedi.Location = new System.Drawing.Point(14, 117);
            this.Accedi.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.Accedi.Name = "Accedi";
            this.Accedi.Size = new System.Drawing.Size(287, 46);
            this.Accedi.TabIndex = 13;
            this.Accedi.Text = "Accedi";
            this.Accedi.UseVisualStyleBackColor = false;
            this.Accedi.Click += new System.EventHandler(this.Accedi_Click);
            // 
            // textBoxPassword
            // 
            this.textBoxPassword.Location = new System.Drawing.Point(104, 75);
            this.textBoxPassword.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.textBoxPassword.MaxLength = 18;
            this.textBoxPassword.Name = "textBoxPassword";
            this.textBoxPassword.Size = new System.Drawing.Size(197, 30);
            this.textBoxPassword.TabIndex = 12;
            this.textBoxPassword.UseSystemPasswordChar = true;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(10, 31);
            this.label2.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(64, 24);
            this.label2.TabIndex = 11;
            this.label2.Text = "Utente:";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(10, 78);
            this.label1.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(87, 24);
            this.label1.TabIndex = 9;
            this.label1.Text = "Password:";
            // 
            // Login
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.ClientSize = new System.Drawing.Size(314, 191);
            this.Controls.Add(this.textBoxUtente);
            this.Controls.Add(this.Accedi);
            this.Controls.Add(this.textBoxPassword);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("Arial Narrow", 15F);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.Name = "Login";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Login";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxUtente;
        private System.Windows.Forms.Button Accedi;
        private System.Windows.Forms.TextBox textBoxPassword;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label1;
    }
}