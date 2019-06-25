namespace WindowsFormsApp2
{
    partial class InserisciRigo
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
            this.comboBox = new System.Windows.Forms.ComboBox();
            this.radioButtonAgilis = new System.Windows.Forms.RadioButton();
            this.radioButtonPreventivi = new System.Windows.Forms.RadioButton();
            this.radioButtonNuovo = new System.Windows.Forms.RadioButton();
            this.label3 = new System.Windows.Forms.Label();
            this.radioButtonArticolo = new System.Windows.Forms.RadioButton();
            this.radioButtonLavorazione = new System.Windows.Forms.RadioButton();
            this.buttonConferma = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.buttonHelp = new System.Windows.Forms.Button();
            this.groupBoxImporta = new System.Windows.Forms.GroupBox();
            this.textBoxQuantita = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.labelImportaDescrizione = new System.Windows.Forms.Label();
            this.textBoxArticolo = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.textBoxNome = new System.Windows.Forms.TextBox();
            this.textBoxDescrizione = new System.Windows.Forms.TextBox();
            this.textBoxQuantitaNuovo = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.textBoxCostoArticolo = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.textBoxSetupMac = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.textBoxCostoSetupMac = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.textBoxSetupUomo = new System.Windows.Forms.TextBox();
            this.label12 = new System.Windows.Forms.Label();
            this.textBoxTempoMac = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.textBoxTempoUomo = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.textBoxCostoSetupUomo = new System.Windows.Forms.TextBox();
            this.label15 = new System.Windows.Forms.Label();
            this.textBoxCostoTempoMac = new System.Windows.Forms.TextBox();
            this.label16 = new System.Windows.Forms.Label();
            this.textBoxCostoTempoUomo = new System.Windows.Forms.TextBox();
            this.groupBoxNuovo = new System.Windows.Forms.GroupBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBoxImporta.SuspendLayout();
            this.groupBoxNuovo.SuspendLayout();
            this.SuspendLayout();
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Arial Narrow", 15F);
            this.label2.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label2.Location = new System.Drawing.Point(11, 32);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(117, 30);
            this.label2.TabIndex = 11;
            this.label2.Text = "Rigo padre:";
            // 
            // comboBox
            // 
            this.comboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBox.Font = new System.Drawing.Font("Arial Narrow", 15F);
            this.comboBox.FormattingEnabled = true;
            this.comboBox.Location = new System.Drawing.Point(110, 29);
            this.comboBox.Name = "comboBox";
            this.comboBox.Size = new System.Drawing.Size(456, 38);
            this.comboBox.TabIndex = 12;
            // 
            // radioButtonAgilis
            // 
            this.radioButtonAgilis.AutoSize = true;
            this.radioButtonAgilis.Checked = true;
            this.radioButtonAgilis.Font = new System.Drawing.Font("Arial Narrow", 15F);
            this.radioButtonAgilis.Location = new System.Drawing.Point(6, 34);
            this.radioButtonAgilis.Name = "radioButtonAgilis";
            this.radioButtonAgilis.Size = new System.Drawing.Size(80, 34);
            this.radioButtonAgilis.TabIndex = 14;
            this.radioButtonAgilis.TabStop = true;
            this.radioButtonAgilis.Text = "Agilis";
            this.radioButtonAgilis.UseVisualStyleBackColor = true;
            this.radioButtonAgilis.CheckedChanged += new System.EventHandler(this.radioButton_CheckedChanged);
            // 
            // radioButtonPreventivi
            // 
            this.radioButtonPreventivi.AutoSize = true;
            this.radioButtonPreventivi.Font = new System.Drawing.Font("Arial Narrow", 15F);
            this.radioButtonPreventivi.Location = new System.Drawing.Point(111, 34);
            this.radioButtonPreventivi.Name = "radioButtonPreventivi";
            this.radioButtonPreventivi.Size = new System.Drawing.Size(336, 34);
            this.radioButtonPreventivi.TabIndex = 15;
            this.radioButtonPreventivi.Text = "Preventivi precedentemente creati";
            this.radioButtonPreventivi.UseVisualStyleBackColor = true;
            this.radioButtonPreventivi.CheckedChanged += new System.EventHandler(this.radioButton_CheckedChanged);
            // 
            // radioButtonNuovo
            // 
            this.radioButtonNuovo.AutoSize = true;
            this.radioButtonNuovo.Font = new System.Drawing.Font("Arial Narrow", 15F);
            this.radioButtonNuovo.Location = new System.Drawing.Point(422, 34);
            this.radioButtonNuovo.Name = "radioButtonNuovo";
            this.radioButtonNuovo.Size = new System.Drawing.Size(91, 34);
            this.radioButtonNuovo.TabIndex = 16;
            this.radioButtonNuovo.Text = "Nuovo";
            this.radioButtonNuovo.UseVisualStyleBackColor = true;
            this.radioButtonNuovo.CheckedChanged += new System.EventHandler(this.radioButton_CheckedChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Arial Narrow", 15F);
            this.label3.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label3.Location = new System.Drawing.Point(11, 190);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(0, 30);
            this.label3.TabIndex = 17;
            // 
            // radioButtonArticolo
            // 
            this.radioButtonArticolo.AutoSize = true;
            this.radioButtonArticolo.Checked = true;
            this.radioButtonArticolo.Font = new System.Drawing.Font("Arial Narrow", 15F);
            this.radioButtonArticolo.Location = new System.Drawing.Point(6, 30);
            this.radioButtonArticolo.Name = "radioButtonArticolo";
            this.radioButtonArticolo.Size = new System.Drawing.Size(100, 34);
            this.radioButtonArticolo.TabIndex = 18;
            this.radioButtonArticolo.TabStop = true;
            this.radioButtonArticolo.Text = "Articolo";
            this.radioButtonArticolo.UseVisualStyleBackColor = true;
            this.radioButtonArticolo.CheckedChanged += new System.EventHandler(this.radioButton_CheckedChanged);
            // 
            // radioButtonLavorazione
            // 
            this.radioButtonLavorazione.AutoSize = true;
            this.radioButtonLavorazione.Font = new System.Drawing.Font("Arial Narrow", 15F);
            this.radioButtonLavorazione.Location = new System.Drawing.Point(111, 30);
            this.radioButtonLavorazione.Name = "radioButtonLavorazione";
            this.radioButtonLavorazione.Size = new System.Drawing.Size(141, 34);
            this.radioButtonLavorazione.TabIndex = 19;
            this.radioButtonLavorazione.Text = "Lavorazione";
            this.radioButtonLavorazione.UseVisualStyleBackColor = true;
            this.radioButtonLavorazione.CheckedChanged += new System.EventHandler(this.radioButton_CheckedChanged);
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
            this.buttonConferma.Location = new System.Drawing.Point(12, 422);
            this.buttonConferma.Margin = new System.Windows.Forms.Padding(2, 3, 2, 3);
            this.buttonConferma.Name = "buttonConferma";
            this.buttonConferma.Size = new System.Drawing.Size(558, 37);
            this.buttonConferma.TabIndex = 20;
            this.buttonConferma.Text = "Conferma inserimento";
            this.buttonConferma.UseVisualStyleBackColor = false;
            this.buttonConferma.Click += new System.EventHandler(this.buttonConferma_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.radioButtonAgilis);
            this.groupBox1.Controls.Add(this.radioButtonPreventivi);
            this.groupBox1.Controls.Add(this.radioButtonNuovo);
            this.groupBox1.Font = new System.Drawing.Font("Arial Narrow", 15F);
            this.groupBox1.Location = new System.Drawing.Point(15, 74);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(551, 80);
            this.groupBox1.TabIndex = 21;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Dichiarare da dove importare il nuovo rigo oppure se crearne uno nuovo:";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.radioButtonArticolo);
            this.groupBox2.Controls.Add(this.radioButtonLavorazione);
            this.groupBox2.Font = new System.Drawing.Font("Arial Narrow", 15F);
            this.groupBox2.Location = new System.Drawing.Point(15, 160);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(551, 80);
            this.groupBox2.TabIndex = 0;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Dichiarare se caricare un articolo o una lavorazione";
            // 
            // buttonHelp
            // 
            this.buttonHelp.AutoSize = true;
            this.buttonHelp.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.buttonHelp.BackColor = System.Drawing.Color.GreenYellow;
            this.buttonHelp.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.buttonHelp.Font = new System.Drawing.Font("Arial Narrow", 15F);
            this.buttonHelp.Location = new System.Drawing.Point(233, 41);
            this.buttonHelp.Name = "buttonHelp";
            this.buttonHelp.Size = new System.Drawing.Size(131, 42);
            this.buttonHelp.TabIndex = 24;
            this.buttonHelp.Text = "Help Articoli";
            this.buttonHelp.UseVisualStyleBackColor = false;
            // 
            // groupBoxImporta
            // 
            this.groupBoxImporta.Controls.Add(this.textBoxQuantita);
            this.groupBoxImporta.Controls.Add(this.label4);
            this.groupBoxImporta.Controls.Add(this.labelImportaDescrizione);
            this.groupBoxImporta.Controls.Add(this.buttonHelp);
            this.groupBoxImporta.Controls.Add(this.textBoxArticolo);
            this.groupBoxImporta.Font = new System.Drawing.Font("Arial Narrow", 15F);
            this.groupBoxImporta.Location = new System.Drawing.Point(15, 246);
            this.groupBoxImporta.Name = "groupBoxImporta";
            this.groupBoxImporta.Size = new System.Drawing.Size(550, 163);
            this.groupBoxImporta.TabIndex = 25;
            this.groupBoxImporta.TabStop = false;
            this.groupBoxImporta.Text = "Dichiara articolo da importare:";
            // 
            // textBoxQuantita
            // 
            this.textBoxQuantita.Location = new System.Drawing.Point(461, 47);
            this.textBoxQuantita.MaxLength = 18;
            this.textBoxQuantita.Name = "textBoxQuantita";
            this.textBoxQuantita.Size = new System.Drawing.Size(77, 36);
            this.textBoxQuantita.TabIndex = 27;
            this.textBoxQuantita.Text = "1";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Arial Narrow", 15F);
            this.label4.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label4.Location = new System.Drawing.Point(379, 53);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(95, 30);
            this.label4.TabIndex = 26;
            this.label4.Text = "Quantità:";
            // 
            // labelImportaDescrizione
            // 
            this.labelImportaDescrizione.AutoSize = true;
            this.labelImportaDescrizione.Font = new System.Drawing.Font("Arial Narrow", 15F);
            this.labelImportaDescrizione.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.labelImportaDescrizione.Location = new System.Drawing.Point(5, 80);
            this.labelImportaDescrizione.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.labelImportaDescrizione.Name = "labelImportaDescrizione";
            this.labelImportaDescrizione.Size = new System.Drawing.Size(213, 30);
            this.labelImportaDescrizione.TabIndex = 25;
            this.labelImportaDescrizione.Text = "<Descrizione Articolo>";
            // 
            // textBoxArticolo
            // 
            this.textBoxArticolo.Font = new System.Drawing.Font("Arial Narrow", 15F);
            this.textBoxArticolo.Location = new System.Drawing.Point(6, 47);
            this.textBoxArticolo.MaxLength = 18;
            this.textBoxArticolo.Name = "textBoxArticolo";
            this.textBoxArticolo.Size = new System.Drawing.Size(221, 36);
            this.textBoxArticolo.TabIndex = 23;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Arial Narrow", 15F);
            this.label5.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label5.Location = new System.Drawing.Point(379, 50);
            this.label5.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(95, 30);
            this.label5.TabIndex = 26;
            this.label5.Text = "Quantità:";
            // 
            // textBoxNome
            // 
            this.textBoxNome.Font = new System.Drawing.Font("Arial Narrow", 15F);
            this.textBoxNome.Location = new System.Drawing.Point(105, 44);
            this.textBoxNome.MaxLength = 18;
            this.textBoxNome.Name = "textBoxNome";
            this.textBoxNome.Size = new System.Drawing.Size(221, 36);
            this.textBoxNome.TabIndex = 23;
            // 
            // textBoxDescrizione
            // 
            this.textBoxDescrizione.Font = new System.Drawing.Font("Arial Narrow", 15F);
            this.textBoxDescrizione.Location = new System.Drawing.Point(105, 80);
            this.textBoxDescrizione.MaxLength = 18;
            this.textBoxDescrizione.Name = "textBoxDescrizione";
            this.textBoxDescrizione.Size = new System.Drawing.Size(433, 36);
            this.textBoxDescrizione.TabIndex = 28;
            // 
            // textBoxQuantitaNuovo
            // 
            this.textBoxQuantitaNuovo.Location = new System.Drawing.Point(461, 44);
            this.textBoxQuantitaNuovo.MaxLength = 18;
            this.textBoxQuantitaNuovo.Name = "textBoxQuantitaNuovo";
            this.textBoxQuantitaNuovo.Size = new System.Drawing.Size(77, 36);
            this.textBoxQuantitaNuovo.TabIndex = 27;
            this.textBoxQuantitaNuovo.Text = "1";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Arial Narrow", 15F);
            this.label6.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label6.Location = new System.Drawing.Point(40, 50);
            this.label6.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(72, 30);
            this.label6.TabIndex = 29;
            this.label6.Text = "Nome:";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Arial Narrow", 15F);
            this.label7.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label7.Location = new System.Drawing.Point(2, 86);
            this.label7.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(123, 30);
            this.label7.TabIndex = 30;
            this.label7.Text = "Descrizione:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Arial Narrow", 15F);
            this.label8.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label8.Location = new System.Drawing.Point(35, 122);
            this.label8.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(82, 30);
            this.label8.TabIndex = 31;
            this.label8.Text = "Prezzo:";
            // 
            // textBoxCostoArticolo
            // 
            this.textBoxCostoArticolo.Font = new System.Drawing.Font("Arial Narrow", 15F);
            this.textBoxCostoArticolo.Location = new System.Drawing.Point(105, 116);
            this.textBoxCostoArticolo.MaxLength = 18;
            this.textBoxCostoArticolo.Name = "textBoxCostoArticolo";
            this.textBoxCostoArticolo.Size = new System.Drawing.Size(221, 36);
            this.textBoxCostoArticolo.TabIndex = 32;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Font = new System.Drawing.Font("Arial Narrow", 15F);
            this.label9.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label9.Location = new System.Drawing.Point(2, 158);
            this.label9.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(115, 30);
            this.label9.TabIndex = 33;
            this.label9.Text = "Setup Mac:";
            // 
            // textBoxSetupMac
            // 
            this.textBoxSetupMac.Font = new System.Drawing.Font("Arial Narrow", 15F);
            this.textBoxSetupMac.Location = new System.Drawing.Point(124, 152);
            this.textBoxSetupMac.MaxLength = 18;
            this.textBoxSetupMac.Name = "textBoxSetupMac";
            this.textBoxSetupMac.Size = new System.Drawing.Size(162, 36);
            this.textBoxSetupMac.TabIndex = 34;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Font = new System.Drawing.Font("Arial Narrow", 15F);
            this.label10.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label10.Location = new System.Drawing.Point(308, 158);
            this.label10.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(72, 30);
            this.label10.TabIndex = 35;
            this.label10.Text = "Costo:";
            // 
            // textBoxCostoSetupMac
            // 
            this.textBoxCostoSetupMac.Font = new System.Drawing.Font("Arial Narrow", 15F);
            this.textBoxCostoSetupMac.Location = new System.Drawing.Point(371, 152);
            this.textBoxCostoSetupMac.MaxLength = 18;
            this.textBoxCostoSetupMac.Name = "textBoxCostoSetupMac";
            this.textBoxCostoSetupMac.Size = new System.Drawing.Size(167, 36);
            this.textBoxCostoSetupMac.TabIndex = 36;
            // 
            // label11
            // 
            this.label11.Font = new System.Drawing.Font("Arial Narrow", 15F);
            this.label11.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label11.Location = new System.Drawing.Point(2, 194);
            this.label11.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(108, 24);
            this.label11.TabIndex = 37;
            this.label11.Text = "Setup Uomo:";
            // 
            // textBoxSetupUomo
            // 
            this.textBoxSetupUomo.Font = new System.Drawing.Font("Arial Narrow", 15F);
            this.textBoxSetupUomo.Location = new System.Drawing.Point(124, 188);
            this.textBoxSetupUomo.MaxLength = 18;
            this.textBoxSetupUomo.Name = "textBoxSetupUomo";
            this.textBoxSetupUomo.Size = new System.Drawing.Size(162, 36);
            this.textBoxSetupUomo.TabIndex = 38;
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Font = new System.Drawing.Font("Arial Narrow", 15F);
            this.label12.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label12.Location = new System.Drawing.Point(2, 230);
            this.label12.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(124, 30);
            this.label12.TabIndex = 39;
            this.label12.Text = "Tempo Mac:";
            // 
            // textBoxTempoMac
            // 
            this.textBoxTempoMac.Font = new System.Drawing.Font("Arial Narrow", 15F);
            this.textBoxTempoMac.Location = new System.Drawing.Point(124, 224);
            this.textBoxTempoMac.MaxLength = 18;
            this.textBoxTempoMac.Name = "textBoxTempoMac";
            this.textBoxTempoMac.Size = new System.Drawing.Size(162, 36);
            this.textBoxTempoMac.TabIndex = 40;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Font = new System.Drawing.Font("Arial Narrow", 15F);
            this.label13.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label13.Location = new System.Drawing.Point(2, 266);
            this.label13.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(139, 30);
            this.label13.TabIndex = 41;
            this.label13.Text = "Tempo Uomo:";
            // 
            // textBoxTempoUomo
            // 
            this.textBoxTempoUomo.Font = new System.Drawing.Font("Arial Narrow", 15F);
            this.textBoxTempoUomo.Location = new System.Drawing.Point(124, 260);
            this.textBoxTempoUomo.MaxLength = 18;
            this.textBoxTempoUomo.Name = "textBoxTempoUomo";
            this.textBoxTempoUomo.Size = new System.Drawing.Size(162, 36);
            this.textBoxTempoUomo.TabIndex = 42;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Font = new System.Drawing.Font("Arial Narrow", 15F);
            this.label14.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label14.Location = new System.Drawing.Point(308, 194);
            this.label14.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(72, 30);
            this.label14.TabIndex = 43;
            this.label14.Text = "Costo:";
            // 
            // textBoxCostoSetupUomo
            // 
            this.textBoxCostoSetupUomo.Font = new System.Drawing.Font("Arial Narrow", 15F);
            this.textBoxCostoSetupUomo.Location = new System.Drawing.Point(371, 188);
            this.textBoxCostoSetupUomo.MaxLength = 18;
            this.textBoxCostoSetupUomo.Name = "textBoxCostoSetupUomo";
            this.textBoxCostoSetupUomo.Size = new System.Drawing.Size(167, 36);
            this.textBoxCostoSetupUomo.TabIndex = 44;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Font = new System.Drawing.Font("Arial Narrow", 15F);
            this.label15.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label15.Location = new System.Drawing.Point(308, 230);
            this.label15.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(72, 30);
            this.label15.TabIndex = 45;
            this.label15.Text = "Costo:";
            // 
            // textBoxCostoTempoMac
            // 
            this.textBoxCostoTempoMac.Font = new System.Drawing.Font("Arial Narrow", 15F);
            this.textBoxCostoTempoMac.Location = new System.Drawing.Point(371, 224);
            this.textBoxCostoTempoMac.MaxLength = 18;
            this.textBoxCostoTempoMac.Name = "textBoxCostoTempoMac";
            this.textBoxCostoTempoMac.Size = new System.Drawing.Size(167, 36);
            this.textBoxCostoTempoMac.TabIndex = 46;
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Font = new System.Drawing.Font("Arial Narrow", 15F);
            this.label16.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.label16.Location = new System.Drawing.Point(308, 263);
            this.label16.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(72, 30);
            this.label16.TabIndex = 47;
            this.label16.Text = "Costo:";
            // 
            // textBoxCostoTempoUomo
            // 
            this.textBoxCostoTempoUomo.Font = new System.Drawing.Font("Arial Narrow", 15F);
            this.textBoxCostoTempoUomo.Location = new System.Drawing.Point(371, 257);
            this.textBoxCostoTempoUomo.MaxLength = 18;
            this.textBoxCostoTempoUomo.Name = "textBoxCostoTempoUomo";
            this.textBoxCostoTempoUomo.Size = new System.Drawing.Size(167, 36);
            this.textBoxCostoTempoUomo.TabIndex = 48;
            // 
            // groupBoxNuovo
            // 
            this.groupBoxNuovo.Controls.Add(this.textBoxCostoTempoUomo);
            this.groupBoxNuovo.Controls.Add(this.label16);
            this.groupBoxNuovo.Controls.Add(this.textBoxCostoTempoMac);
            this.groupBoxNuovo.Controls.Add(this.label15);
            this.groupBoxNuovo.Controls.Add(this.textBoxCostoSetupUomo);
            this.groupBoxNuovo.Controls.Add(this.label14);
            this.groupBoxNuovo.Controls.Add(this.textBoxTempoUomo);
            this.groupBoxNuovo.Controls.Add(this.label13);
            this.groupBoxNuovo.Controls.Add(this.textBoxTempoMac);
            this.groupBoxNuovo.Controls.Add(this.label12);
            this.groupBoxNuovo.Controls.Add(this.textBoxSetupUomo);
            this.groupBoxNuovo.Controls.Add(this.label11);
            this.groupBoxNuovo.Controls.Add(this.textBoxCostoSetupMac);
            this.groupBoxNuovo.Controls.Add(this.label10);
            this.groupBoxNuovo.Controls.Add(this.textBoxSetupMac);
            this.groupBoxNuovo.Controls.Add(this.label9);
            this.groupBoxNuovo.Controls.Add(this.textBoxCostoArticolo);
            this.groupBoxNuovo.Controls.Add(this.label8);
            this.groupBoxNuovo.Controls.Add(this.label7);
            this.groupBoxNuovo.Controls.Add(this.label6);
            this.groupBoxNuovo.Controls.Add(this.textBoxQuantitaNuovo);
            this.groupBoxNuovo.Controls.Add(this.textBoxDescrizione);
            this.groupBoxNuovo.Controls.Add(this.textBoxNome);
            this.groupBoxNuovo.Controls.Add(this.label5);
            this.groupBoxNuovo.Font = new System.Drawing.Font("Arial Narrow", 15F);
            this.groupBoxNuovo.Location = new System.Drawing.Point(15, 246);
            this.groupBoxNuovo.Name = "groupBoxNuovo";
            this.groupBoxNuovo.Size = new System.Drawing.Size(550, 332);
            this.groupBoxNuovo.TabIndex = 28;
            this.groupBoxNuovo.TabStop = false;
            this.groupBoxNuovo.Text = "Inserisci un nuovo articolo:";
            this.groupBoxNuovo.Visible = false;
            // 
            // InserisciRigo
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.WhiteSmoke;
            this.ClientSize = new System.Drawing.Size(581, 471);
            this.Controls.Add(this.groupBoxNuovo);
            this.Controls.Add(this.groupBoxImporta);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.buttonConferma);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.comboBox);
            this.Controls.Add(this.label2);
            this.Name = "InserisciRigo";
            this.Text = "Inserisci Rigo";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBoxImporta.ResumeLayout(false);
            this.groupBoxImporta.PerformLayout();
            this.groupBoxNuovo.ResumeLayout(false);
            this.groupBoxNuovo.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ComboBox comboBox;
        private System.Windows.Forms.RadioButton radioButtonAgilis;
        private System.Windows.Forms.RadioButton radioButtonPreventivi;
        private System.Windows.Forms.RadioButton radioButtonNuovo;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.RadioButton radioButtonArticolo;
        private System.Windows.Forms.RadioButton radioButtonLavorazione;
        private System.Windows.Forms.Button buttonConferma;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button buttonHelp;
        private System.Windows.Forms.GroupBox groupBoxImporta;
        private System.Windows.Forms.Label labelImportaDescrizione;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBoxArticolo;
        private System.Windows.Forms.TextBox textBoxQuantita;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textBoxNome;
        private System.Windows.Forms.TextBox textBoxDescrizione;
        private System.Windows.Forms.TextBox textBoxQuantitaNuovo;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.TextBox textBoxCostoArticolo;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox textBoxSetupMac;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox textBoxCostoSetupMac;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox textBoxSetupUomo;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox textBoxTempoMac;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox textBoxTempoUomo;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.TextBox textBoxCostoSetupUomo;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.TextBox textBoxCostoTempoMac;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.TextBox textBoxCostoTempoUomo;
        private System.Windows.Forms.GroupBox groupBoxNuovo;
    }
}