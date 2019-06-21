namespace WindowsFormsApp2
{
    partial class CaricaPreventivo
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CaricaPreventivo));
            this.textBoxCliente = new System.Windows.Forms.TextBox();
            this.buttonHelpClienti = new System.Windows.Forms.Button();
            this.labelCliente = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.textBoxID = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.labelNote = new System.Windows.Forms.Label();
            this.buttonConferma = new System.Windows.Forms.Button();
            this.buttonHelpID = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // textBoxCliente
            // 
            resources.ApplyResources(this.textBoxCliente, "textBoxCliente");
            this.textBoxCliente.Name = "textBoxCliente";
            this.textBoxCliente.TextChanged += new System.EventHandler(this.textBoxCliente_TextChanged);
            // 
            // buttonHelpClienti
            // 
            resources.ApplyResources(this.buttonHelpClienti, "buttonHelpClienti");
            this.buttonHelpClienti.BackColor = System.Drawing.Color.GreenYellow;
            this.buttonHelpClienti.Name = "buttonHelpClienti";
            this.buttonHelpClienti.UseVisualStyleBackColor = false;
            this.buttonHelpClienti.Click += new System.EventHandler(this.buttonHelpClienti_Click);
            // 
            // labelCliente
            // 
            resources.ApplyResources(this.labelCliente, "labelCliente");
            this.labelCliente.Name = "labelCliente";
            // 
            // label2
            // 
            resources.ApplyResources(this.label2, "label2");
            this.label2.Name = "label2";
            // 
            // textBoxID
            // 
            resources.ApplyResources(this.textBoxID, "textBoxID");
            this.textBoxID.Name = "textBoxID";
            this.textBoxID.TextChanged += new System.EventHandler(this.textBoxID_TextChanged);
            // 
            // label1
            // 
            resources.ApplyResources(this.label1, "label1");
            this.label1.Name = "label1";
            // 
            // labelNote
            // 
            resources.ApplyResources(this.labelNote, "labelNote");
            this.labelNote.Name = "labelNote";
            // 
            // buttonConferma
            // 
            resources.ApplyResources(this.buttonConferma, "buttonConferma");
            this.buttonConferma.BackColor = System.Drawing.Color.GreenYellow;
            this.buttonConferma.Name = "buttonConferma";
            this.buttonConferma.UseVisualStyleBackColor = false;
            this.buttonConferma.Click += new System.EventHandler(this.buttonConferma_Click);
            // 
            // buttonHelpID
            // 
            resources.ApplyResources(this.buttonHelpID, "buttonHelpID");
            this.buttonHelpID.BackColor = System.Drawing.Color.GreenYellow;
            this.buttonHelpID.Name = "buttonHelpID";
            this.buttonHelpID.UseVisualStyleBackColor = false;
            this.buttonHelpID.Click += new System.EventHandler(this.buttonHelpID_Click);
            // 
            // CaricaPreventivo
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            resources.ApplyResources(this, "$this");
            this.Controls.Add(this.buttonHelpID);
            this.Controls.Add(this.buttonConferma);
            this.Controls.Add(this.labelNote);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.textBoxID);
            this.Controls.Add(this.textBoxCliente);
            this.Controls.Add(this.buttonHelpClienti);
            this.Controls.Add(this.labelCliente);
            this.Controls.Add(this.label2);
            this.MaximizeBox = false;
            this.Name = "CaricaPreventivo";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBoxCliente;
        private System.Windows.Forms.Button buttonHelpClienti;
        private System.Windows.Forms.Label labelCliente;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox textBoxID;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label labelNote;
        private System.Windows.Forms.Button buttonConferma;
        private System.Windows.Forms.Button buttonHelpID;
    }
}