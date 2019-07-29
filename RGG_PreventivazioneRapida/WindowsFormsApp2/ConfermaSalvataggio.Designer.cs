namespace Preventivazione_RGG
{
    partial class ConfermaSalvataggio
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
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxArticolo = new System.Windows.Forms.TextBox();
            this.buttonConferma = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.Font = new System.Drawing.Font("Arial Narrow", 15F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.Location = new System.Drawing.Point(14, 9);
            this.label2.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(258, 68);
            this.label2.TabIndex = 2;
            this.label2.Text = "Prima di confermare il salvataggio indicare il nuovo codice articolo:";
            // 
            // textBoxArticolo
            // 
            this.textBoxArticolo.Font = new System.Drawing.Font("Arial Narrow", 15F);
            this.textBoxArticolo.Location = new System.Drawing.Point(14, 62);
            this.textBoxArticolo.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.textBoxArticolo.MaxLength = 18;
            this.textBoxArticolo.Name = "textBoxArticolo";
            this.textBoxArticolo.Size = new System.Drawing.Size(258, 30);
            this.textBoxArticolo.TabIndex = 3;
            // 
            // buttonConferma
            // 
            this.buttonConferma.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.buttonConferma.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.buttonConferma.BackColor = System.Drawing.Color.GreenYellow;
            this.buttonConferma.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonConferma.Font = new System.Drawing.Font("Arial Narrow", 15F);
            this.buttonConferma.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.buttonConferma.Location = new System.Drawing.Point(14, 99);
            this.buttonConferma.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.buttonConferma.Name = "buttonConferma";
            this.buttonConferma.Size = new System.Drawing.Size(258, 37);
            this.buttonConferma.TabIndex = 21;
            this.buttonConferma.Text = "Conferma inserimento";
            this.buttonConferma.UseVisualStyleBackColor = false;
            this.buttonConferma.Click += new System.EventHandler(this.buttonConferma_Click);
            // 
            // ConfermaSalvataggio
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(10F, 24F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 148);
            this.Controls.Add(this.buttonConferma);
            this.Controls.Add(this.textBoxArticolo);
            this.Controls.Add(this.label2);
            this.Font = new System.Drawing.Font("Arial Narrow", 15F);
            this.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.MaximumSize = new System.Drawing.Size(300, 187);
            this.MinimumSize = new System.Drawing.Size(300, 187);
            this.Name = "ConfermaSalvataggio";
            this.Text = "Conferma";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxArticolo;
        private System.Windows.Forms.Button buttonConferma;
    }
}