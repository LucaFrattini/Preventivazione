using HelperLibrary;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using WindowsFormsApp2;

namespace PreventivazioneRapida
{
    public partial class Form1 : Form
    {
        //modello dei dati
        Model m;
        private int zoom = 0;
        private double precedenteQuantita = 1;
        public TextBox TbCLiente { get; set; }
        public TextBox TbArticolo { get; set; }
        public string cliente, articolo, quantita, variazione, variazionelav;
        public Form1()
        {
            //inizializzo la screen
            InitializeComponent();
            //FormBorderStyle = FormBorderStyle.None;
            WindowState = FormWindowState.Maximized;
            ToolTip ToolTip1 = new ToolTip();
            ToolTip1.SetToolTip(this.btnRefresh, "Aggiorna");
            ToolTip1.SetToolTip(this.buttonEspandi, "Ridimensiona griglia");
            ToolTip1.SetToolTip(btnNuovo, "Pulisci tutto");
            ToolTip1.SetToolTip(btnModifica, "Modifica dati tabella");
            ToolTip1.SetToolTip(btnEsci, "Chiudi programma");
            ToolTip1.SetToolTip(btnConferma, "Conferma");
            ToolTip1.SetToolTip(btnMeno, "Diminuisci zoom");
            ToolTip1.SetToolTip(btnPiu, "Ingrandisci zoom");
            ToolTip1.SetToolTip(btnReset, "Imposta zoom originale");
            buttonQuantita1.Text = Setting.Istance.Q1;
            buttonQuantita2.Text = Setting.Istance.Q2;
            buttonQuantita3.Text = Setting.Istance.Q3;
            SetFont(Setting.Istance.Font);
             //inizializzo i dati clienti e articoli
             m = new Model();
            //this.dataGridView.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventArgs(this.dataGridView_CellDoubleClick);
            this.dataGridView.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.dataGridView_CellDoubleClick);
            TbCLiente = textBoxCliente;
            TbArticolo = textBoxArticolo;
            dataGridView.UserDeletingRow += dataGridView_UserDeletingRow;
            textBoxCliente.Enter += textBox_Enter;
            textBoxArticolo.Enter += textBox_Enter;
            textBoxQuantita.Enter += textBox_Enter;
            textBoxVariazioneLav.Enter += textBox_Enter;
            textBoxVariazione.Enter += textBox_Enter;
        }

        public void textBox_Enter(object sender, EventArgs e)
        {
            cliente = textBoxCliente.Text;
            articolo = textBoxArticolo.Text;
            quantita = textBoxQuantita.Text;
            variazione = textBoxVariazione.Text;
            variazionelav = textBoxVariazioneLav.Text;
        }

        public void dataGridView_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            DialogResult usersChoice =
                MessageBox.Show("Confermare la cancellazione delle righe selezionate e dei relativi semi-lavorati?", "Conferma eliminazione", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);

            // cancel the delete event
            if (usersChoice == DialogResult.OK)
            {
                e.Cancel = true;
                DataGridViewRow row = e.Row;
                int rowscount = m.ds.Tables["DistintaBase"].Rows.Count;
                string codiceart = row.Cells["Codice Art"].Value.ToString();
                for (int i = 0; i < rowscount; i++)
                {
                    try
                    {
                        DataRow datarow = m.ds.Tables["DistintaBase"].Rows[i];
                        
                        if (datarow["Codice Art"].ToString() == codiceart || datarow["Codice_Padre"].ToString() == codiceart)
                        {
                            m.ds.Tables["DistintaBase"].Rows.Remove(datarow);
                            i--;
                            rowscount--;
                        }
                    }
                    catch { }                  
                }
                CalcolaPrezzoQuantitaImpostata();
                CalcolaPrezzoTotaliPerQuantita();
            }
        }

        public void SetFont(String font)
        {

            font = font.Replace('#', ' ');

            String fontFamil = font.Split('-')[0];
            int fontSize;
            int.TryParse(font.Split('-')[1], out fontSize);

            Font f = new Font(fontFamil, fontSize);
            // this.Font = f;
            //MessageBox.Show( this.groupBox1.Controls.ToString());
            if (f != null)
            {
                foreach (Control c in this.Controls)
                {
                    if (c is Label )//&& Setting.Istance.Font == "FO")//se si tratta di una label e nel fil di configurazione il la label hanno font normale
                    {
                        c.Font = f;
                    }
                    if (c is TextBox )//&& Setting.Istance.Font == "FO")//se si tratta di una label e nel fil di configurazione il la label hanno font normale
                    {
                        c.Font = f;
                    }
                    if (c is Button)
                    {
                        c.Font = f;
                    }

                }
                //this.dataGridView.Font = f;
            }


        }

        private void buttonHelpClienti_Click(object sender, EventArgs e)
        {
            Helper h = new Helper();
            String[] par = { };
            //public string StartHelper(string path, string ip, string port, string database, string username, string password, string _campofocus, string _utenteWSAI, string _passwordWSAI, string[] parametri, string valoreFocus, string font, string fontLabel)
            string temp =  h.StartHelper(Setting.Istance.HelpCliente, Setting.Istance.Ip, Setting.Istance.Port, Setting.Istance.Database, Setting.Istance.User, Setting.Istance.Password, "", "", "", par, "", Setting.Istance.Font, Setting.Istance.FontLabel);

            if (!String.IsNullOrEmpty(temp))
            {
                this.textBoxCliente.Text = temp;
            }
        }

        private void buttonHelpArticoli_Click(object sender, EventArgs e)
        {
            Helper h = new Helper();
            String[] par = { };
            string temp =  h.StartHelper(Setting.Istance.HelpArticolo, Setting.Istance.Ip, Setting.Istance.Port, Setting.Istance.Database, Setting.Istance.User, Setting.Istance.Password, "", "", "", par, "", Setting.Istance.Font, Setting.Istance.FontLabel);

            if (!String.IsNullOrEmpty(temp))
            {
                this.textBoxArticolo.Text = temp;
            }
        }

        private void textBoxCliente_TextChanged(object sender, EventArgs e)
        {
            if(textBoxCliente.Text != cliente)
            {
                DataTable dt;
                dt = m.ds.Tables["Clienti"];
                try
                {
                    DataRow dr = dt.Rows.Find(textBoxCliente.Text);
                    if (dr != null)
                    {
                        textBoxCliente.BackColor = Color.LightGreen;
                        labelCliente.Text = dr[1].ToString();
                    }
                    else
                    {
                        textBoxCliente.BackColor = (textBoxCliente.Text == "" ? Color.White : Color.OrangeRed);
                        labelCliente.Text = "<Descrizione cliente>";
                    }
                }
                catch { }
            }                      
        }

        void EsplodiDistintaBase(string padre, int livello)
        {
            //NON DEVO CERCARE PER CODICE ARTICOLO, NEL CASO FOSSE UNA LAVORAZIONE COME LO CERCO!?!?!?!?!?!
            IEnumerable<DataRow> query = from row in m.ds.Tables["DistintaBase"].AsEnumerable() where row.Field<String>("rowindex") == padre select row;
            DataRow rowpadre = query.First();
            int rows = m.VerificaSemilavorato(rowpadre["CODICE ART"].ToString());
            try
            {
                if (Double.Parse(rowpadre["Codice Centro"].ToString()) > 499)
                {
                    m.GestisciLavorazioneEsterna(rowpadre, textBoxArticolo.Text);
                }
            }
            catch { }
            try
            {
                if (rows > 0)
                {
                    m.InizializzaColonneTempo();
                    double totalePadre = 0;
                    int countrowdt = m.ds.Tables["DistintaBase"].Rows.Count;
                    for(int i = 0; i < countrowdt; i++)
                    {
                        DataRow figlio = m.ds.Tables["DistintaBase"].Rows[i];
                        if (figlio["CODICE_PADRE"].ToString() == rowpadre["CODICE ART"].ToString())
                        {
                            figlio["rowindex"] = rowpadre["rowindex"].ToString() + "," + rows;
                            figlio["Quantita`"] = Math.Round(Double.Parse(figlio["Quantita`"].ToString()) * Double.Parse(rowpadre["Quantita`"].ToString()), 4);
                            EsplodiDistintaBase(figlio["rowindex"].ToString(), ++livello);
                            totalePadre += Double.Parse(figlio["Totale"].ToString());
                            rows--;
                        }
                    }
                    
                    rowpadre["Totale"] = Math.Round(totalePadre,2);
                    rowpadre["Costo Art"] = Math.Round(Double.Parse(rowpadre["Totale"].ToString()) / Double.Parse(rowpadre["Quantita`"].ToString()),2);
                    rowpadre["Totale + %Var"] = Math.Round((Convert.ToDouble(rowpadre["Totale"].ToString()) * Convert.ToDouble(textBoxVariazione.Text) / 100) + Convert.ToDouble(rowpadre["Totale"].ToString()),2);

                }
                else
                {
                    if(rowpadre["CODICE ART"].ToString() != "")
                    {
                    
                        rowpadre["Totale"] = Math.Round(Double.Parse(rowpadre["Costo Art"].ToString()) * Double.Parse(rowpadre["Quantita`"].ToString()), 3);
                        rowpadre["Costo Art"] = Math.Round(Double.Parse(rowpadre["Totale"].ToString()) / Double.Parse(rowpadre["Quantita`"].ToString()), 2);
                        try {
                            if (Double.Parse(rowpadre["Codice Centro"].ToString()) < 499)
                            {
                                rowpadre["Totale + %Var"] = Math.Round((Convert.ToDouble(rowpadre["Totale"].ToString()) * Convert.ToDouble(textBoxVariazione.Text) / 100) + Convert.ToDouble(rowpadre["Totale"].ToString()), 2);
                            }
                            else
                            {
                                rowpadre["Totale + %Var"] = Math.Round((Convert.ToDouble(rowpadre["Totale"].ToString()) * Convert.ToDouble(textBoxVariazioneLav.Text) / 100) + Convert.ToDouble(rowpadre["Totale"].ToString()), 2);
                            }
                        }
                        catch
                        {
                            rowpadre["Totale + %Var"] = Math.Round((Convert.ToDouble(rowpadre["Totale"].ToString()) * Convert.ToDouble(textBoxVariazione.Text) / 100) + Convert.ToDouble(rowpadre["Totale"].ToString()), 2);
                        }
                    }
                    else
                    {
                        rowpadre["Totale"] = Math.Round((Double.Parse(rowpadre["Costo Att Mac"].ToString()) * Double.Parse(rowpadre["Setup Mac decimale"].ToString()))
                                            + (Double.Parse(rowpadre["Costo Att Uomo"].ToString()) * Double.Parse(rowpadre["Setup Uomo decimale"].ToString()))
                                            + (Double.Parse(rowpadre["Costo Mac"].ToString()) * Double.Parse(rowpadre["Tempo Mac decimale"].ToString()) * Double.Parse(rowpadre["Quantita`"].ToString()))
                                            + (Double.Parse(rowpadre["Costo Uomo"].ToString()) * Double.Parse(rowpadre["Tempo Uomo decimale"].ToString()) * Double.Parse(rowpadre["Quantita`"].ToString())),2);
                        rowpadre["Totale + %Var"] = Math.Round((Convert.ToDouble(rowpadre["Totale"].ToString()) * Convert.ToDouble(textBoxVariazioneLav.Text) / 100) + Convert.ToDouble(rowpadre["Totale"].ToString()), 2);

                    }

                }
            }
            catch { }
            return;
        }



        //Fare il test con questo codice --> SB02AL1505.0-1_02
        private void textBoxArticolo_TextChanged(object sender, EventArgs e)
        {
            if(textBoxArticolo.Text != articolo)
            {
                DataTable dtArt, dtDistBase;
                dtArt = m.ds.Tables["Articoli"];
                DataRow dr = dtArt.Rows.Find(textBoxArticolo.Text);
                if (dr != null)
                {
                    textBoxArticolo.BackColor = Color.LightGreen;
                    labelArticolo.Text = dr[1].ToString();
                    string getDistBase = Setting.Istance.QueryCodDistBase.Replace("@articolo", textBoxArticolo.Text);
                    m.EstraiRisultatoQuery(getDistBase, "CodDistBase");
                    if (m.ds.Tables["CodDistBase"].Rows.Count > 0)
                    {
                        string distintaBase = Setting.Istance.QueryDistintaBase.Replace("@CodDistBase", m.ds.Tables["CodDistBase"].Rows[0][0].ToString());
                        m.EstraiRisultatoQuery(distintaBase, "DistintaBase");
                        BindingSource bindingSource1 = new BindingSource();
                        bindingSource1.DataSource = m.ds.Tables["DistintaBase"];
                        dataGridView.DataSource = bindingSource1;
                        dtDistBase = m.ds.Tables["DistintaBase"];
                        int righeDataGrid = m.ds.Tables["DistintaBase"].Rows.Count;
                        m.InizializzaColonneTempo();
                        int livello = 1;
                        for (int i = 0; i < righeDataGrid; i++)
                        {
                            DataRow figlio = m.ds.Tables["DistintaBase"].Rows[i];
                            figlio["Quantita`"] = Math.Round(Double.Parse(figlio["Quantita`"].ToString()) * Double.Parse(textBoxQuantita.Text.Replace('.', ',')), 4);
                            string rowindex = figlio["rowindex"].ToString();
                            EsplodiDistintaBase(rowindex, livello);
                        }
                        m.FromDecimalToTime();

                        //dataGridView.Columns["rowindex"].Visible = false;
                        dataGridView.Sort(dataGridView.Columns["rowindex"], ListSortDirection.Ascending);
                        foreach (DataGridViewColumn column in dataGridView.Columns)
                        {
                            column.SortMode = DataGridViewColumnSortMode.NotSortable;
                            column.ReadOnly = true;
                        }
                        //Fare il test con questo codice --> SB02AL1505.0-1_02
                        DataTable table = m.ds.Tables["DistintaBase"];
                        ColoraDataGrid();
                        CalcolaPrezzoQuantitaImpostata();
                        CalcolaPrezzoTotaliPerQuantita();
                        //Fare il test con questo codice --> SB02AL1505.0-1_02                 
                    }
                    else
                    {
                        MessageBox.Show("L'articolo non ha una distinta base");
                    }
                    groupBoxQI.Text = "Quantità impostata: " + textBoxQuantita.Text;
                    textBoxVariazione.Enabled = true;
                    textBoxVariazioneLav.Enabled = true;
                    textBoxQuantita.Enabled = true;
                }
                else
                {
                    try
                    {
                        if (m.ds.Tables.Count > 2)
                        {
                            m.ds.Tables.Remove("DistintaBase");
                            m.ds.Tables.Remove("CodDistBase");
                            dataGridView.DataSource = null;
                        }
                        textBoxArticolo.BackColor = (textBoxArticolo.Text == "" ? Color.White : Color.OrangeRed);
                        labelArticolo.Text = "<Descrizione articolo>";
                        textBoxVariazione.Enabled = false;
                        textBoxVariazioneLav.Enabled = false;
                        textBoxQuantita.Enabled = false;
                        //dataGridView.Rows.Clear();
                    }
                    catch
                    {

                    }
                }
            }
        }

        private void ColoraDataGrid()
        {
            foreach (DataGridViewRow padre in dataGridView.Rows)
            {
                if (padre.Cells["rowindex"].Value != null)
                {
                    string indiceriga = padre.Cells["rowindex"].Value.ToString();
                    int generazione = FindNumberOfChar(',', indiceriga);
                    
                    switch (generazione)
                    {
                        case 0:
                            if (padre.Cells["Codice Art"].Value.ToString() != "")
                            {
                                padre.Cells["Codice Art"].Style.BackColor = Color.ForestGreen;
                                padre.Cells["Costo Art"].Style.BackColor = Color.LimeGreen;
                            }
                            else
                            {
                                padre.Cells["Codice Lav"].Style.BackColor = Color.ForestGreen;
                                padre.Cells["Costo Att Mac"].Style.BackColor = Color.LimeGreen;
                                padre.Cells["Costo Att Uomo"].Style.BackColor = Color.LimeGreen;
                                padre.Cells["Costo Mac"].Style.BackColor = Color.LimeGreen;
                                padre.Cells["Costo Uomo"].Style.BackColor = Color.LimeGreen;
                            }
                            padre.Cells["Totale"].Style.BackColor = Color.ForestGreen;
                            padre.Cells["Totale + %Var"].Style.BackColor = Color.ForestGreen;
                            break;
                        case 1:
                            //padre.Visible = false;
                            if (padre.Cells["Codice Art"].Value.ToString() != "")
                            {
                                padre.Cells["Codice Art"].Style.BackColor = Color.LimeGreen;
                                padre.Cells["Costo Art"].Style.BackColor = Color.Lime;
                            }
                            else
                            {
                                padre.Cells["Codice Lav"].Style.BackColor = Color.LimeGreen;
                                padre.Cells["Costo Att Mac"].Style.BackColor = Color.Lime;
                                padre.Cells["Costo Att Uomo"].Style.BackColor = Color.Lime;
                                padre.Cells["Costo Mac"].Style.BackColor = Color.Lime;
                                padre.Cells["Costo Uomo"].Style.BackColor = Color.Lime;
                            }
                            padre.Cells["Totale"].Style.BackColor = Color.LimeGreen;
                            padre.Cells["Totale + %Var"].Style.BackColor = Color.LimeGreen;
                            break;
                        case 2:
                            //padre.Visible = false;
                            if (padre.Cells["Codice Art"].Value.ToString() != "")
                            {
                                padre.Cells["Codice Art"].Style.BackColor = Color.Lime;
                                padre.Cells["Costo Art"].Style.BackColor = Color.LawnGreen;
                            }
                            else
                            {
                                padre.Cells["Codice Lav"].Style.BackColor = Color.Lime;
                                padre.Cells["Costo Att Mac"].Style.BackColor = Color.LawnGreen;
                                padre.Cells["Costo Att Uomo"].Style.BackColor = Color.LawnGreen;
                                padre.Cells["Costo Mac"].Style.BackColor = Color.LawnGreen;
                                padre.Cells["Costo Uomo"].Style.BackColor = Color.LawnGreen;
                            }
                            padre.Cells["Totale"].Style.BackColor = Color.Lime;
                            padre.Cells["Totale + %Var"].Style.BackColor = Color.Lime;
                            break;
                        default:
                            //padre.Visible = false;
                            if (padre.Cells["Codice Art"].Value.ToString() != "")
                            {
                                padre.Cells["Codice Art"].Style.BackColor = Color.LawnGreen;
                                padre.Cells["Costo Art"].Style.BackColor = Color.GreenYellow;
                            }
                            else
                            {
                                padre.Cells["Codice Lav"].Style.BackColor = Color.LawnGreen;
                                padre.Cells["Costo Att Mac"].Style.BackColor = Color.GreenYellow;
                                padre.Cells["Costo Att Uomo"].Style.BackColor = Color.GreenYellow;
                                padre.Cells["Costo Mac"].Style.BackColor = Color.GreenYellow;
                                padre.Cells["Costo Uomo"].Style.BackColor = Color.GreenYellow;
                            }
                            padre.Cells["Totale"].Style.BackColor = Color.LawnGreen;
                            padre.Cells["Totale + %Var"].Style.BackColor = Color.LawnGreen;
                            break;
                    }
                    bool hafiglio = false;
                    foreach (DataRow figlio in m.ds.Tables["DistintaBase"].Rows)
                    {                       
                        if (padre.Cells["CODICE ART"].Value.ToString() == figlio["CODICE_PADRE"].ToString())
                        {
                            hafiglio = true;
                            break;
                        }
                    }
                    if (!hafiglio)
                    {
                        if (padre.Cells["Codice Art"].Value.ToString() != "")
                        {
                            padre.Cells["Costo Art"].ReadOnly = false;
                            padre.Cells["Costo Art"].Style.BackColor = Color.LightSkyBlue;
                            padre.Cells["Costo Art"].Style.SelectionBackColor = Color.DeepSkyBlue;
                        }
                        else
                        {
                            padre.Cells["Costo Att Mac"].Style.SelectionBackColor = Color.DeepSkyBlue;
                            padre.Cells["Costo Att Mac"].Style.BackColor = Color.LightSkyBlue;
                            padre.Cells["Costo Att Mac"].ReadOnly = false;
                            padre.Cells["Costo Att Uomo"].Style.SelectionBackColor = Color.DeepSkyBlue;
                            padre.Cells["Costo Att Uomo"].Style.BackColor = Color.LightSkyBlue;
                            padre.Cells["Costo Att Uomo"].ReadOnly = false;
                            padre.Cells["Costo Mac"].Style.SelectionBackColor = Color.DeepSkyBlue;
                            padre.Cells["Costo Mac"].Style.BackColor = Color.LightSkyBlue;
                            padre.Cells["Costo Mac"].ReadOnly = false;
                            padre.Cells["Costo Uomo"].Style.SelectionBackColor = Color.DeepSkyBlue;
                            padre.Cells["Costo Uomo"].Style.BackColor = Color.LightSkyBlue;
                            padre.Cells["Costo Uomo"].ReadOnly = false;
                            padre.Cells["Setup Mac"].Style.SelectionBackColor = Color.DeepSkyBlue;
                            padre.Cells["Setup Mac"].Style.BackColor = Color.LightSkyBlue;
                            padre.Cells["Setup Mac"].ReadOnly = false;
                            padre.Cells["Setup Uomo"].Style.SelectionBackColor = Color.DeepSkyBlue;
                            padre.Cells["Setup Uomo"].Style.BackColor = Color.LightSkyBlue;
                            padre.Cells["Setup Uomo"].ReadOnly = false;
                            padre.Cells["Tempo Mac"].Style.SelectionBackColor = Color.DeepSkyBlue;
                            padre.Cells["Tempo Mac"].Style.BackColor = Color.LightSkyBlue;
                            padre.Cells["Tempo Mac"].ReadOnly = false;
                            padre.Cells["Tempo Uomo"].Style.SelectionBackColor = Color.DeepSkyBlue;
                            padre.Cells["Tempo Uomo"].Style.BackColor = Color.LightSkyBlue;
                            padre.Cells["Tempo Uomo"].ReadOnly = false;
                        }
                    }
                }
            
            }
        }
        //Queste funzioni erano usate quando si utilizzava l'approccio ad oggetti
        /*
        private bool checkClienteID(Cliente c)
        {
            return c.Conto == textBoxCliente.Text;
        }

        private bool checkArticoloID(Articolo a)
        {
            return a.Codart == textBoxArticolo.Text;
        }*/

        private void brnPiu_Click(object sender, EventArgs e)
        {
            if (this.zoom < 4)
            {
                this.zoom++;
                this.ridimensione(1);
            }
        }

        private void btnMeno_Click(object sender, EventArgs e)
        {
            if (this.zoom > -4)
            {
                this.zoom--;
                /*float currentSize = Font.Size;
                currentSize += 1.0F;
                Font = new Font(Font.Name, currentSize);
                Size = new Size(1070, 605);*/

                this.ridimensione(-1);
            }
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            this.ridimensione(this.zoom * -1);
            this.zoom = 0;
        }

        //prova provae
        /// funzione che presa in input un intero modifica la dimensione di tutti i controlli visivi presenti nella vista
        private void ridimensione(int size)
        {
            //MessageBox.Show( this.groupBox1.Controls.ToString());
            foreach (Control c in this.Controls)
            {
                if (c is TextBox)
                {
                    c.Height = c.Height + size;
                    c.Width = c.Width + size;
                    c.Font = new Font(c.Font.FontFamily, c.Font.Size + size);
                }
                if (c is Label)
                {
                    c.Height = c.Height + size;
                    c.Width += size * 15;
                    c.Font = new Font(c.Font.FontFamily, c.Font.Size + size);
                }

            }
            
            this.dataGridView.Font = new Font(this.dataGridView.Font.FontFamily, this.dataGridView.Font.Size + size * 2);
            
        }

        private void btnEsci_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void btnNuovo_Click(object sender, EventArgs e)
        {
            if (!String.IsNullOrEmpty(textBoxArticolo.Text) || !String.IsNullOrEmpty(textBoxCliente.Text) || !String.IsNullOrEmpty(textBoxQuantita.Text) ||
                !String.IsNullOrEmpty(textBoxVariazione.Text) || !String.IsNullOrEmpty(textBoxNote.Text))
            {
                textBoxCliente.Text = "";
                textBoxArticolo.Text = "";
                textBoxQuantita.Text = "1";
                textBoxVariazione.Text = "0";
                textBoxVariazioneLav.Text = "0";
                textBoxNote.Text = "";
                dataGridView.Rows.Clear();
                textBoxCliente.Focus();
            }
            CostoMac1.Text = "0";
            CostoMac2.Text = "0";
            CostoMac3.Text = "0";
            CostoUomo1.Text = "0";
            CostoUomo2.Text = "0";
            CostoUomo3.Text = "0";
            Articoli1.Text = "0";
            Articoli2.Text = "0";
            Articoli3.Text = "0";
            Tot1.Text = "0";
            Tot2.Text = "0";
            Tot3.Text = "0";
            TotVar1.Text = "0";
            TotVar2.Text = "0";
            TotVar3.Text = "0";
            QIcostomac.Text = "0";
            QIcostouomo.Text = "0";
            QIarticoli.Text = "0";
            QItotale.Text = "0";
            QItotalevar.Text = "0";
        }

        private void btnRefresh_Click(object sender, EventArgs e)
        {
            string articolotemp = textBoxArticolo.Text;
            string clientetemp = textBoxCliente.Text;
            string quantitatemp = textBoxQuantita.Text;
            string variazionetemp = textBoxVariazione.Text;
            string notetemp = textBoxNote.Text;

            textBoxArticolo.Text = "";
            textBoxCliente.Text = " ";
            textBoxQuantita.Text = "";
            textBoxVariazione.Text = "";
            textBoxNote.Text = "";

            m = new Model();

            textBoxArticolo.Text = articolotemp;
            textBoxCliente.Text = clientetemp;
            textBoxQuantita.Text = quantitatemp;
            textBoxVariazione.Text = variazionetemp;
            textBoxNote.Text = notetemp;
        }

        private void textBoxVariazione_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar != ',' && e.KeyChar != '.')
            {
                if(e.KeyChar == '.')
                {
                    e.KeyChar = ',';
                }
                e.Handled = !char.IsDigit(e.KeyChar) && !char.IsControl(e.KeyChar);
            }
            else
            {
                int virgola = textBoxVariazione.Text.IndexOf(',');
                int punto = textBoxVariazione.Text.IndexOf('.');
                if(virgola > 0 || punto > 0)
                {
                    e.Handled = true;
                }
            }            
        }

        private void textBoxVariazione_TextChanged(object sender, EventArgs e)
        {
            if(textBoxVariazione.Text != variazione)
            {
                try
                {
                    double variazioneInserita = Double.Parse(textBoxVariazione.Text);
                    double Totale, variazione;
                    for (int rowcount = 0; rowcount <= dataGridView.Rows.Count - 2; rowcount++)
                    {
                        DataGridViewRow row = dataGridView.Rows[rowcount];
                        string indiceriga = row.Cells["rowindex"].Value.ToString();
                        int generazione = FindNumberOfChar(',', indiceriga);
                        try
                        {
                            if (row.Cells["Totale"].ToString() != null && row.Cells["Codice Art"].Value.ToString() != "" && row.Cells["Codice centro"].Value.ToString() == "")
                            {
                                Totale = Double.Parse(row.Cells["Totale"].Value.ToString());
                                if (rowcount!= dataGridView.Rows.Count - 2)
                                {

                                    if (row.Cells["Codice Art"].Value.ToString() != dataGridView.Rows[rowcount + 1].Cells["Codice_PADRE"].Value.ToString())
                                    {
                                        variazione = Double.Parse(textBoxVariazione.Text.Replace('.', ','));
                                        row.Cells["Totale + %Var"].Value = Math.Round((Totale + (variazione * Totale / 100)), 2);
                                    }
                                }
                                else
                                {
                                    variazione = Double.Parse(textBoxVariazione.Text.Replace('.', ','));
                                    row.Cells["Totale + %Var"].Value = Math.Round((Totale + (variazione * Totale / 100)), 2);
                                }                                     
                            }
                        }
                        catch { }
                    }
                }
                catch
                {
                    MessageBox.Show("Il valore inserito non è valido!");
                }
            }          
        }

        private void textBoxVariazioneLav_TextChanged(object sender, EventArgs e)
        {
            if(textBoxVariazioneLav.Text != variazionelav)
            {
                try
                {
                    double variazioneInserita = Double.Parse(textBoxVariazioneLav.Text);
                    double Totale, variazione;
                    int ultimaGenerazione = FindNumberOfChar(',', m.ds.Tables["DistintaBase"].Rows[m.ds.Tables["DistintaBase"].Rows.Count - 1]["rowindex"].ToString());
                    for (int rowcount = 0; rowcount <= dataGridView.Rows.Count - 2; rowcount++)
                    {
                        DataGridViewRow row = dataGridView.Rows[rowcount];
                        string indiceriga = row.Cells["rowindex"].Value.ToString();
                        int generazione = FindNumberOfChar(',', indiceriga);
                        try
                        {
                            if ((row.Cells["Totale"].ToString() != null && row.Cells["Codice Art"].Value.ToString() == "") || Double.Parse(row.Cells["Codice Centro"].Value.ToString()) > 499)
                            {
                                Totale = Double.Parse(row.Cells["Totale"].Value.ToString());
                                if (textBoxVariazioneLav.Text != "")
                                {

                                    variazione = Double.Parse(textBoxVariazioneLav.Text.Replace('.', ','));
                                }
                                else
                                {
                                    variazione = 0;
                                }
                                double toto = Math.Round((Totale + (variazione * Totale / 100)), 2);
                                row.Cells["Totale + %Var"].Value = Math.Round((Totale + (variazione * Totale / 100)), 2);
                            }
                        }
                        catch { }
                    }
                }
                catch
                {
                    MessageBox.Show("Il valore inserito non è valido!");
                }
            }           
        }

        private void btnModifica_Click(object sender, EventArgs e)
        {
            /*foreach(XmlNode c in Setting.Istance.CampiModificabili)
            {
                string campo = c.InnerText;
                dataGridView.Columns[campo].ReadOnly = false;
                dataGridView.Columns[campo].DefaultCellStyle.BackColor = Color.LightBlue;
            }*/
            dataGridView.Columns["Quantita`"].ReadOnly = false;
            dataGridView.Columns["Setup Mac"].ReadOnly = false;
            dataGridView.Columns["Setup Uomo"].ReadOnly = false;
            dataGridView.Columns["Tempo Mac"].ReadOnly = false;
            dataGridView.Columns["Tempo Uomo"].ReadOnly = false;
            dataGridView.Columns["Costo Att Mac"].ReadOnly = false;
            dataGridView.Columns["Costo Att Uomo"].ReadOnly = false;
            dataGridView.Columns["Costo Mac"].ReadOnly = false;
            dataGridView.Columns["Costo Uomo"].ReadOnly = false;
            dataGridView.Columns["Quantita`"].DefaultCellStyle.BackColor = Color.LightSkyBlue;
            dataGridView.Columns["Setup Mac"].DefaultCellStyle.BackColor = Color.LightSkyBlue;
            dataGridView.Columns["Setup Uomo"].DefaultCellStyle.BackColor = Color.LightSkyBlue;
            dataGridView.Columns["Tempo Mac"].DefaultCellStyle.BackColor = Color.LightSkyBlue;
            dataGridView.Columns["Tempo Uomo"].DefaultCellStyle.BackColor = Color.LightSkyBlue;
            dataGridView.Columns["Costo Att Uomo"].DefaultCellStyle.BackColor = Color.LightSkyBlue;
            dataGridView.Columns["Costo Att Mac"].DefaultCellStyle.BackColor = Color.LightSkyBlue;
            dataGridView.Columns["Costo Mac"].DefaultCellStyle.BackColor = Color.LightSkyBlue;
            dataGridView.Columns["Costo Uomo"].DefaultCellStyle.BackColor = Color.LightSkyBlue;
            dataGridView.Columns["Quantita`"].DefaultCellStyle.SelectionBackColor = Color.DeepSkyBlue;
            dataGridView.Columns["Setup Mac"].DefaultCellStyle.SelectionBackColor = Color.DeepSkyBlue;
            dataGridView.Columns["Setup Uomo"].DefaultCellStyle.SelectionBackColor = Color.DeepSkyBlue;
            dataGridView.Columns["Tempo Mac"].DefaultCellStyle.SelectionBackColor = Color.DeepSkyBlue;
            dataGridView.Columns["Tempo Uomo"].DefaultCellStyle.SelectionBackColor = Color.DeepSkyBlue;
            dataGridView.Columns["Costo Att Uomo"].DefaultCellStyle.SelectionBackColor = Color.DeepSkyBlue;
            dataGridView.Columns["Costo Att Mac"].DefaultCellStyle.SelectionBackColor = Color.DeepSkyBlue;
            dataGridView.Columns["Costo Mac"].DefaultCellStyle.SelectionBackColor = Color.DeepSkyBlue;
            dataGridView.Columns["Costo Uomo"].DefaultCellStyle.SelectionBackColor = Color.DeepSkyBlue;

        }

        private void textBoxQuantita_TextChanged(object sender, EventArgs e)
        {
            if(textBoxQuantita.Text != quantita)
            {
                try
                {
                    double quantitaNuova;
                    if (textBoxQuantita.Text == "" || textBoxQuantita.Text == "0" || textBoxQuantita.Text == "0.")
                    {
                        quantitaNuova = 1;
                    }
                    else
                    {
                        quantitaNuova = Double.Parse(textBoxQuantita.Text.Replace('.', ','));
                    }
                    double quantitaPrecedente = 1;
                    for (int rowcount = 0; rowcount <= dataGridView.Rows.Count - 2; rowcount++)
                    {
                        DataGridViewRow row = dataGridView.Rows[rowcount];
                        quantitaPrecedente = Double.Parse(row.Cells["Quantita`"].Value.ToString());
                        row.Cells["Quantita`"].Value = Math.Round(Double.Parse(row.Cells["Quantita`"].Value.ToString()) / precedenteQuantita * quantitaNuova, 4);
                        //row.Cells["Quantita`"].Value = quantitaPrecedente * quantitaNuova;                            
                    }
                    precedenteQuantita = quantitaNuova;
                    groupBoxQI.Text = "Quantità impostata: " + quantitaNuova;
                }
                catch
                {
                    MessageBox.Show("Il valore inserito non è valido!");
                }
            }                           
        }

        private void button1_Click(object sender, EventArgs e)
        {
            textBoxQuantita.Text = buttonQuantita1.Text;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            textBoxQuantita.Text = buttonQuantita2.Text;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            textBoxQuantita.Text = buttonQuantita3.Text;
        }

        /*private void dataGridView_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
            MessageBox.Show("Il valore inserito non è valido!");
        }*/
        //Fare il test con questo codice --> SB02AL1505.0-1_02
        private void dataGridView_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            int rowcount = m.ds.Tables["DistintaBase"].Rows.Count;
            string valoreCella = dataGridView.CurrentCell.Value.ToString();
            for (int i = dataGridView.Rows.Count - 2; i >= 0; i--)
            {
                if (valoreCella == dataGridView["CODICE_PADRE", i].Value.ToString())
                {
                    if(dataGridView.Rows[i].Visible == false)
                    {
                        dataGridView.Rows[i].Visible = true;
                    }
                    else
                    {
                        dataGridView.Rows[i].Visible = false;
                    }                  
                }else if(valoreCella == dataGridView["CODICE_PADRE", i].Value.ToString() && dataGridView["CODICE_PADRE", i].Value.ToString() != "")
                {
                    dataGridView.Rows[i].Visible = false;
                }
            }
        }

        private void dataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            try            
            {
                //double valoreCella = Double.Parse(dataGridView.CurrentCell.Value.ToString());
                int colindex = e.ColumnIndex;// dataGridView.CurrentCell.ColumnIndex;
                int rowindex = e.RowIndex;//dataGridView.CurrentCell.RowIndex;
                DataColumn[] key = new DataColumn[1];
                if (m.ds.Tables["DistintaBase"].PrimaryKey.Length < 1)
                {
                    key[0] = m.ds.Tables["DistintaBase"].Columns["rowindex"];
                    m.ds.Tables["DistintaBase"].PrimaryKey = key;

                }
                string indiceriga = dataGridView["rowindex", rowindex].Value.ToString();
                DataRow r = m.ds.Tables["DistintaBase"].Rows.Find(indiceriga);
                int generazione = FindNumberOfChar(',', indiceriga);
                int ultimaGenerazione = FindNumberOfChar(',', m.ds.Tables["DistintaBase"].Rows[m.ds.Tables["DistintaBase"].Rows.Count-1]["rowindex"].ToString());

                if (generazione==ultimaGenerazione || r["CODICE ART"].ToString() == "")
                {
                    if (r["Codice Art"].ToString() != "")
                    {
                        r["Totale"] = Math.Round(Double.Parse(r["Costo Art"].ToString()) * Double.Parse(r["Quantita`"].ToString()), 2);
                        try
                        {
                            if (Double.Parse(r["Codice Centro"].ToString()) > 499)
                            {
                                r["Totale + %Var"] = Math.Round((Convert.ToDouble(r["Totale"].ToString()) * Convert.ToDouble(textBoxVariazioneLav.Text) / 100) + Convert.ToDouble(r["Totale"].ToString()), 2);
                            }
                            else
                            {
                                r["Totale + %Var"] = Math.Round((Convert.ToDouble(r["Totale"].ToString()) * Convert.ToDouble(textBoxVariazione.Text) / 100) + Convert.ToDouble(r["Totale"].ToString()), 2);
                            }
                        }
                        catch
                        {
                            r["Totale + %Var"] = Math.Round((Convert.ToDouble(r["Totale"].ToString()) * Convert.ToDouble(textBoxVariazione.Text) / 100) + Convert.ToDouble(r["Totale"].ToString()), 2);
                        }
                    }
                    else
                    {
                        if (Double.Parse(r["Quantita`"].ToString()) == 0)
                        {
                            r["Totale"] = 0;
                            r["Totale + %Var"] = 0;
                        }
                        else
                        {
                            r["Totale"] = Math.Round((Double.Parse(r["Costo Att Mac"].ToString()) * Double.Parse(r["Setup Mac decimale"].ToString()))
                            + (Double.Parse(r["Costo Att Uomo"].ToString()) * Double.Parse(r["Setup Uomo decimale"].ToString()))
                            + (Double.Parse(r["Costo Mac"].ToString()) * Double.Parse(r["Tempo Mac decimale"].ToString()) * Double.Parse(r["Quantita`"].ToString()))
                            + (Double.Parse(r["Costo Uomo"].ToString()) * Double.Parse(r["Tempo Uomo decimale"].ToString()) * Double.Parse(r["Quantita`"].ToString())), 2);
                            r["Totale + %Var"] = Math.Round((Convert.ToDouble(r["Totale"].ToString()) * Convert.ToDouble(textBoxVariazioneLav.Text) / 100) + Convert.ToDouble(r["Totale"].ToString()), 2);
                        }
                    }
                    AggiornaParentela(r);
                }

                m.FromTimeToDecimal();
                BindingGrid();

            }
            catch
            {
                MessageBox.Show("Il valore inserito non è valido!");
            }
            
        }

        //Fare il test con questo codice --> SB02AL1505.0-1_02
        private void AggiornaParentela(DataRow figlio)
        {

            double totalePadre = 0, totaleVarPadre = 0;
            if (figlio["CODICE_PADRE"].ToString() != textBoxArticolo.Text)
            {
                foreach (DataRow fratelli in m.ds.Tables["DistintaBase"].Rows)
                {
                    if (fratelli["CODICE_PADRE"].ToString() == figlio["CODICE_PADRE"].ToString())
                    {
                        totalePadre += Double.Parse(fratelli["Totale"].ToString());
                        totaleVarPadre += Double.Parse(fratelli["Totale + %Var"].ToString());
                    }
                }
                IEnumerable<DataRow> query = from row in m.ds.Tables["DistintaBase"].AsEnumerable() where row.Field<String>("CODICE ART") == figlio["CODICE_PADRE"].ToString() select row;
                int count = query.Count();
                if (count > 0)
                {
                    DataRow rowpadre = query.First();
                    rowpadre["Totale"] = Math.Round(totalePadre, 2);
                    rowpadre["COSTO ART"] = Math.Round(totalePadre / Double.Parse(rowpadre["Quantita`"].ToString()), 2);
                    rowpadre["Totale + %Var"] = Math.Round(totaleVarPadre, 2);
                    AggiornaParentela(rowpadre);
                }               
            }
            return;
        }

        private void CalcolaPrezzoQuantitaImpostata()
        {
            double colCostoMac = 0, colCostoUomo = 0, colCostoMat = 0,  colTotale = 0, colTotaleVar = 0;
            foreach(DataRow row in m.ds.Tables["DistintaBase"].Rows)
            {
                bool hafiglio = false;
                foreach (DataRow figlio in m.ds.Tables["DistintaBase"].Rows)
                {
                    if (row["CODICE ART"].ToString() == figlio["CODICE_PADRE"].ToString())
                    {
                        hafiglio = true;
                        break;
                    }
                }
                if (!hafiglio)
                {
                    if (row["CODICE ART"].ToString() != "")
                    {
                        colCostoMat += (Double.Parse(row["Totale"].ToString()));
                    }
                }
                int generazione = FindNumberOfChar(',', row["rowindex"].ToString());
                try
                {
                    colCostoMac += (Double.Parse(row["Costo Att Mac"].ToString()) * Double.Parse(row["Setup Mac decimale"].ToString()));
                    colCostoMac += (Double.Parse(row["Costo Mac"].ToString()) * Double.Parse(row["Tempo Mac decimale"].ToString()) * Double.Parse(row["Quantita`"].ToString()));
                    colCostoUomo += (Double.Parse(row["Costo Att Uomo"].ToString()) * Double.Parse(row["Setup Uomo decimale"].ToString()));
                    colCostoUomo += (Double.Parse(row["Costo Uomo"].ToString()) * Double.Parse(row["Tempo Uomo decimale"].ToString()) * Double.Parse(row["Quantita`"].ToString()));
                }
                catch { }
                if (generazione == 0)
                {
                    
                    colTotale += Double.Parse(row["Totale"].ToString());
                    colTotaleVar += Double.Parse(row["Totale + %Var"].ToString());
                }
            }
            QIcostomac.Text = Math.Round(colCostoMac, 2).ToString();
            QIcostouomo.Text = Math.Round(colCostoUomo,2).ToString();
            QIarticoli.Text = Math.Round(colCostoMat, 2).ToString();
            QItotale.Text = Math.Round(colTotale,2).ToString();
            QItotalevar.Text = Math.Round(colTotaleVar,2).ToString();
            QICostoSingolo.Text = (Double.Parse(QItotale.Text) / Double.Parse(textBoxQuantita.Text)).ToString();
            QIRicavoSingolo.Text = (Double.Parse(QItotalevar.Text) / Double.Parse(textBoxQuantita.Text)).ToString();
        }

        private void CalcolaPrezzoTotaliPerQuantita()
        {
            double quantita = 0;
            if (textBoxQuantita.Text == "" || textBoxQuantita.Text == "0")
            {
                quantita = 1;
            }
            else
            {
                quantita = Double.Parse(textBoxQuantita.Text.Replace('.',','));
            }            
            double colCostoMac1 = 0, colCostoUomo1 = 0, colArticoli1 = 0, colTotale1 = 0, colTotaleVar1 = 0;
            double colCostoMac2 = 0, colCostoUomo2 = 0, colArticoli2 = 0, colTotale2 = 0, colTotaleVar2 = 0;
            double colCostoMac3 = 0, colCostoUomo3 = 0, colArticoli3 = 0, colTotale3 = 0, colTotaleVar3 = 0;
            int righedt = m.ds.Tables["DistintaBase"].Rows.Count;
            int generazione = FindNumberOfChar(',', m.ds.Tables["DistintaBase"].Rows[righedt-1].ToString());
            foreach(DataRow padre in m.ds.Tables["DistintaBase"].Rows)
            {
                bool hafiglio = false;
                foreach(DataRow figlio in m.ds.Tables["DistintaBase"].Rows)
                {                      
                    if(padre["CODICE ART"].ToString() == figlio["CODICE_PADRE"].ToString())
                    {
                        hafiglio = true;
                        break;
                    }
                }
                if (!hafiglio)
                {
                    if (padre["CODICE ART"].ToString() != "")
                    {
                        colArticoli1 += (Double.Parse(padre["Totale"].ToString()) / quantita) * Double.Parse(Setting.Istance.Q1);
                        colArticoli2 += (Double.Parse(padre["Totale"].ToString()) / quantita) * Double.Parse(Setting.Istance.Q2);
                        colArticoli3 += (Double.Parse(padre["Totale"].ToString()) / quantita) * Double.Parse(Setting.Istance.Q3);
                        colTotale1 += (Double.Parse(padre["Totale"].ToString()) / quantita) * Double.Parse(Setting.Istance.Q1);
                        colTotale2 += (Double.Parse(padre["Totale"].ToString()) / quantita) * Double.Parse(Setting.Istance.Q2);
                        colTotale3 += (Double.Parse(padre["Totale"].ToString()) / quantita) * Double.Parse(Setting.Istance.Q3);
                        colTotaleVar1 += (Double.Parse(padre["Totale + %Var"].ToString()) / quantita) * Double.Parse(Setting.Istance.Q1);
                        colTotaleVar2 += (Double.Parse(padre["Totale + %Var"].ToString()) / quantita) * Double.Parse(Setting.Istance.Q2);
                        colTotaleVar3 += (Double.Parse(padre["Totale + %Var"].ToString()) / quantita) * Double.Parse(Setting.Istance.Q3);

                    }
                    else
                    {
                        colCostoMac1 += (Double.Parse(padre["Costo Att Mac"].ToString()) * Double.Parse(padre["Setup Mac decimale"].ToString()));
                        colCostoMac1 += (Double.Parse(padre["Costo Mac"].ToString()) * Double.Parse(padre["Tempo Mac decimale"].ToString()) * (Double.Parse(padre["Quantita`"].ToString()) / quantita) * Double.Parse(Setting.Istance.Q1));
                        colCostoMac2 += (Double.Parse(padre["Costo Att Mac"].ToString()) * Double.Parse(padre["Setup Mac decimale"].ToString()));
                        colCostoMac2 += (Double.Parse(padre["Costo Mac"].ToString()) * Double.Parse(padre["Tempo Mac decimale"].ToString()) * (Double.Parse(padre["Quantita`"].ToString()) / quantita) * Double.Parse(Setting.Istance.Q2));
                        colCostoMac3 += (Double.Parse(padre["Costo Att Mac"].ToString()) * Double.Parse(padre["Setup Mac decimale"].ToString()));
                        colCostoMac3 += (Double.Parse(padre["Costo Mac"].ToString()) * Double.Parse(padre["Tempo Mac decimale"].ToString()) * (Double.Parse(padre["Quantita`"].ToString()) / quantita) * Double.Parse(Setting.Istance.Q3));

                        colCostoUomo1 += (Double.Parse(padre["Costo Att Uomo"].ToString()) * Double.Parse(padre["Setup Uomo decimale"].ToString()));
                        colCostoUomo1 += (Double.Parse(padre["Costo Uomo"].ToString()) * Double.Parse(padre["Tempo Uomo decimale"].ToString()) * (Double.Parse(padre["Quantita`"].ToString()) / quantita) * Double.Parse(Setting.Istance.Q1));
                        colCostoUomo2 += (Double.Parse(padre["Costo Att Uomo"].ToString()) * Double.Parse(padre["Setup Uomo decimale"].ToString()));
                        colCostoUomo2 += (Double.Parse(padre["Costo Uomo"].ToString()) * Double.Parse(padre["Tempo Uomo decimale"].ToString()) * (Double.Parse(padre["Quantita`"].ToString()) / quantita) * Double.Parse(Setting.Istance.Q2));
                        colCostoUomo3 += (Double.Parse(padre["Costo Att Uomo"].ToString()) * Double.Parse(padre["Setup Uomo decimale"].ToString()));
                        colCostoUomo3 += (Double.Parse(padre["Costo Uomo"].ToString()) * Double.Parse(padre["Tempo Uomo decimale"].ToString()) * (Double.Parse(padre["Quantita`"].ToString()) / quantita ) * Double.Parse(Setting.Istance.Q3));

                        double totaleriga1 = 0;
                        totaleriga1 += (Double.Parse(padre["Costo Att Mac"].ToString()) * Double.Parse(padre["Setup Mac decimale"].ToString())) + (Double.Parse(padre["Costo Mac"].ToString()) * Double.Parse(padre["Tempo Mac decimale"].ToString()) * (Double.Parse(padre["Quantita`"].ToString()) / quantita * Double.Parse(Setting.Istance.Q1)));
                        totaleriga1 += (Double.Parse(padre["Costo Att Uomo"].ToString()) * Double.Parse(padre["Setup Uomo decimale"].ToString())) + (Double.Parse(padre["Costo Uomo"].ToString()) * Double.Parse(padre["Tempo Uomo decimale"].ToString()) * (Double.Parse(padre["Quantita`"].ToString()) / quantita) * Double.Parse(Setting.Istance.Q1));
                        double totaleriga2 = 0;
                        totaleriga2 += (Double.Parse(padre["Costo Att Mac"].ToString()) * Double.Parse(padre["Setup Mac decimale"].ToString())) + (Double.Parse(padre["Costo Mac"].ToString()) * Double.Parse(padre["Tempo Mac decimale"].ToString()) * (Double.Parse(padre["Quantita`"].ToString()) / quantita * Double.Parse(Setting.Istance.Q2)));
                        totaleriga2 += (Double.Parse(padre["Costo Att Uomo"].ToString()) * Double.Parse(padre["Setup Uomo decimale"].ToString())) + (Double.Parse(padre["Costo Uomo"].ToString()) * Double.Parse(padre["Tempo Uomo decimale"].ToString()) * (Double.Parse(padre["Quantita`"].ToString()) / quantita) * Double.Parse(Setting.Istance.Q2));
                        double totaleriga3 = 0;
                        totaleriga3 += (Double.Parse(padre["Costo Att Mac"].ToString()) * Double.Parse(padre["Setup Mac decimale"].ToString())) + (Double.Parse(padre["Costo Mac"].ToString()) * Double.Parse(padre["Tempo Mac decimale"].ToString()) * (Double.Parse(padre["Quantita`"].ToString()) / quantita * Double.Parse(Setting.Istance.Q3)));
                        totaleriga3 += (Double.Parse(padre["Costo Att Uomo"].ToString()) * Double.Parse(padre["Setup Uomo decimale"].ToString())) + (Double.Parse(padre["Costo Uomo"].ToString()) * Double.Parse(padre["Tempo Uomo decimale"].ToString()) * (Double.Parse(padre["Quantita`"].ToString()) / quantita) * Double.Parse(Setting.Istance.Q3));

                        colTotale1 += totaleriga1;
                        colTotaleVar1 += (totaleriga1 * Double.Parse(textBoxVariazioneLav.Text) / 100) + totaleriga1;
                        colTotale2 += totaleriga2;
                        colTotaleVar2 += (totaleriga2 * Double.Parse(textBoxVariazioneLav.Text) / 100) + totaleriga2;
                        colTotale3 += totaleriga3;
                        colTotaleVar3 += (totaleriga3 * Double.Parse(textBoxVariazioneLav.Text) / 100) + totaleriga3;
                                            //SISTEMARE PREZZO PER QUANTITA NELLA PARTE DELLA VARIAZIONE PERCENTUALE....
                    }
                    
                }
            }
            CostoMac1.Text = Math.Round(colCostoMac1,2).ToString();
            CostoMac2.Text = Math.Round(colCostoMac2,2).ToString();
            CostoMac3.Text = Math.Round(colCostoMac3,2).ToString();
            CostoUomo1.Text = Math.Round(colCostoUomo1,2).ToString();
            CostoUomo2.Text = Math.Round(colCostoUomo2,2).ToString();
            CostoUomo3.Text = Math.Round(colCostoUomo3,2).ToString();
            Articoli1.Text = Math.Round(colArticoli1, 2).ToString();
            Articoli2.Text = Math.Round(colArticoli2, 2).ToString();
            Articoli3.Text = Math.Round(colArticoli3, 2).ToString();
            Tot1.Text = Math.Round(colTotale1,2).ToString();
            Tot2.Text = Math.Round(colTotale2,2).ToString();
            Tot3.Text = Math.Round(colTotale3,2).ToString();
            //colTotaleVar1 += Math.Round((colTotale1 * Convert.ToDouble(textBoxVariazione.Text) / 100) + colTotale1,2);
            //colTotaleVar2 += Math.Round((colTotale2 * Convert.ToDouble(textBoxVariazione.Text) / 100) + colTotale2,2);
            //colTotaleVar3 += Math.Round((colTotale3 * Convert.ToDouble(textBoxVariazione.Text) / 100) + colTotale3,2);
            TotVar1.Text = Math.Round(colTotaleVar1,2).ToString();
            TotVar2.Text = Math.Round(colTotaleVar2,2).ToString();
            TotVar3.Text = Math.Round(colTotaleVar3,2).ToString();
        }
        //Fare il test con questo codice --> SB02AL1505.0-1_02

        private int FindNumberOfChar(Char target, String searched)
        {
            Console.Write(
                target);

            int startIndex = -1;
            int hitCount = 0;

            // Search for all occurrences of the target.
            while (true)
            {
                startIndex = searched.IndexOf(
                    target, startIndex + 1,
                    searched.Length - startIndex - 1);

                // Exit the loop if the target is not found.
                if (startIndex < 0)
                    break;

                hitCount++;
            }
            return hitCount;

        }

        private void btnConferma_Click(object sender, EventArgs e)
        {
            if (textBoxCliente.Text != "" && textBoxArticolo.Text != "" && textBoxNote.Text != "")
            {
                string[] valoriTestata = new string[8];
                valoriTestata[0] = textBoxCliente.Text;
                valoriTestata[1] = textBoxArticolo.Text;
                valoriTestata[2] = textBoxQuantita.Text;
                valoriTestata[3] = textBoxVariazione.Text;
                valoriTestata[4] = textBoxVariazioneLav.Text;
                valoriTestata[5] = QItotale.Text;
                valoriTestata[6] = QItotalevar.Text;
                valoriTestata[7] = textBoxNote.Text;
                m.InsertPreventivo(valoriTestata);
            }
            else
            {
                MessageBox.Show("I campi \"Cliente\", \"Articolo\" e \"Note\" devono essere compilati per poter salvare il preventivo!");
            }
            return;
        }

        private void btnCarica_Click(object sender, EventArgs e)
        {
            CaricaPreventivo loadPreventivo = new CaricaPreventivo(this, m);
            loadPreventivo.Show();
            return;
        }

        public void InserisciTestata(List<string> testata)
        {
            textBoxCliente.Text = testata[0];
            textBoxArticolo.Text = testata[1];
            textBoxQuantita.Text = testata[2];
            textBoxQuantita.Enabled = true;
            textBoxNote.Text = testata[3];
            textBoxVariazione.Text = testata[4];
            textBoxVariazione.Enabled = true;
            textBoxVariazioneLav.Text = testata[5];
            textBoxVariazioneLav.Enabled = true;
        }

        public void BindingGrid()
        {
            dataGridView.Visible = false;
            progressBar1.Visible = true;
            progressBar1.Minimum = 1;
            progressBar1.Maximum = 7;
            progressBar1.Value = 1;
            progressBar1.Step = 1;
            dataGridView.DataSource = null;
            progressBar1.PerformStep();
            BindingSource bindingSource1 = new BindingSource();
            bindingSource1.DataSource = m.ds.Tables["DistintaBase"];
            dataGridView.DataSource = bindingSource1.DataSource;
            dataGridView.Sort(dataGridView.Columns["rowindex"], ListSortDirection.Ascending);
            progressBar1.PerformStep();
            foreach (DataGridViewColumn column in dataGridView.Columns)
            {
                column.SortMode = DataGridViewColumnSortMode.NotSortable;
                column.ReadOnly = true;
            }
            progressBar1.PerformStep();
            CalcolaPrezzoQuantitaImpostata();
            progressBar1.PerformStep();
            CalcolaPrezzoTotaliPerQuantita();
            progressBar1.PerformStep();
            ColoraDataGrid();
            progressBar1.PerformStep();
            progressBar1.Visible = false;
            dataGridView.Visible = true;
        }

        private void buttonEspandi_Click(object sender, EventArgs e)
        {
            int a = this.MaximumSize.Height;
            int b = dataGridView.Location.Y;
            if(dataGridView.Height > 550)
            {
                dataGridView.Height = a-b-10;
            }
            else
            {
                dataGridView.Height = 560;
            }
        }
    }
}
