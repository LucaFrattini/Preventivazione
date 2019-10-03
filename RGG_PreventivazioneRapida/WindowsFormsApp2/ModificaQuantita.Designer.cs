namespace Preventivazione_RGG
{
    partial class ModificaQuantita
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
            this.Accedi = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.numericUpDown1 = new System.Windows.Forms.NumericUpDown();
            this.numericUpDown2 = new System.Windows.Forms.NumericUpDown();
            this.numericUpDown3 = new System.Windows.Forms.NumericUpDown();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown2)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown3)).BeginInit();
            this.SuspendLayout();
            // 
            // Accedi
            // 
            this.Accedi.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.Accedi.BackColor = System.Drawing.Color.GreenYellow;
            this.Accedi.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.Accedi.Font = new System.Drawing.Font("Arial Narrow", 15F);
            this.Accedi.Location = new System.Drawing.Point(13, 147);
            this.Accedi.Margin = new System.Windows.Forms.Padding(5, 6, 5, 6);
            this.Accedi.Name = "Accedi";
            this.Accedi.Size = new System.Drawing.Size(182, 46);
            this.Accedi.TabIndex = 16;
            this.Accedi.Text = "Conferma";
            this.Accedi.UseVisualStyleBackColor = false;
            this.Accedi.Click += new System.EventHandler(this.Accedi_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Arial Narrow", 15F);
            this.label1.Location = new System.Drawing.Point(9, 9);
            this.label1.Margin = new System.Windows.Forms.Padding(5, 0, 5, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(186, 24);
            this.label1.TabIndex = 14;
            this.label1.Text = "Inserisci nuove quantità:";
            // 
            // numericUpDown1
            // 
            this.numericUpDown1.Font = new System.Drawing.Font("Arial Narrow", 15F);
            this.numericUpDown1.Location = new System.Drawing.Point(13, 36);
            this.numericUpDown1.Maximum = new decimal(new int[] {
            999999,
            0,
            0,
            0});
            this.numericUpDown1.Name = "numericUpDown1";
            this.numericUpDown1.Size = new System.Drawing.Size(182, 30);
            this.numericUpDown1.TabIndex = 17;
            // 
            // numericUpDown2
            // 
            this.numericUpDown2.Font = new System.Drawing.Font("Arial Narrow", 15F);
            this.numericUpDown2.Location = new System.Drawing.Point(13, 72);
            this.numericUpDown2.Maximum = new decimal(new int[] {
            999999,
            0,
            0,
            0});
            this.numericUpDown2.Name = "numericUpDown2";
            this.numericUpDown2.Size = new System.Drawing.Size(182, 30);
            this.numericUpDown2.TabIndex = 18;
            // 
            // numericUpDown3
            // 
            this.numericUpDown3.Font = new System.Drawing.Font("Arial Narrow", 15F);
            this.numericUpDown3.Location = new System.Drawing.Point(13, 108);
            this.numericUpDown3.Maximum = new decimal(new int[] {
            999999,
            0,
            0,
            0});
            this.numericUpDown3.Name = "numericUpDown3";
            this.numericUpDown3.Size = new System.Drawing.Size(182, 30);
            this.numericUpDown3.TabIndex = 19;
            // 
            // ModificaQuantita
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(204, 211);
            this.Controls.Add(this.numericUpDown3);
            this.Controls.Add(this.numericUpDown2);
            this.Controls.Add(this.numericUpDown1);
            this.Controls.Add(this.Accedi);
            this.Controls.Add(this.label1);
            this.MaximumSize = new System.Drawing.Size(220, 250);
            this.MinimumSize = new System.Drawing.Size(220, 250);
            this.Name = "ModificaQuantita";
            this.Text = "Modifica quantità";
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown2)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.numericUpDown3)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button Accedi;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown numericUpDown1;
        private System.Windows.Forms.NumericUpDown numericUpDown2;
        private System.Windows.Forms.NumericUpDown numericUpDown3;
    }
}