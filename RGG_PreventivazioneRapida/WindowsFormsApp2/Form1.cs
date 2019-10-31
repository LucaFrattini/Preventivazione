using HelperLibrary;
using Preventivazione_RGG;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
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
        Login formLogin;
        public bool creaArticolo = false;
        DataRow codiceArticoloDettagli;
        //UMprecedente serve per fare i calcoli quando si calcola la unità di misura e per fare le conversioni.
        //0 --> UM principale... 1--> UM secondaria, bisogna moltipliacare da quella principale... 2--> UM terziaria (confezione), bisogna dividere da quella principale
        private int zoom = 0, altezzaDataGrid = 0;
        private double precedenteQuantita = 1;
        public TextBox TbCLiente { get; set; }
        public TextBox TbArticolo { get; set; }
        public string cliente, articolo, quantita, variazione, variazionelav, user;


        public Form1(Login login, string utente)
        {
            formLogin = login;
            user = utente;
            //inizializzo la screen
            InitializeComponent();
            //FormBorderStyle = FormBorderStyle.None;
            WindowState = FormWindowState.Maximized;
            ToolTip ToolTip1 = new ToolTip();
            //Inserimento dei Tooltip per i bottoni del menù
            ToolTip1.SetToolTip(this.btnRefresh, "Aggiorna");
            ToolTip1.SetToolTip(this.buttonEspandi, "Espandi/comprimi griglia");
            ToolTip1.SetToolTip(btnNuovo, "Crea nuova distinta");
            ToolTip1.SetToolTip(btnModifica, "Modifica dati tabella");
            ToolTip1.SetToolTip(btnEsci, "Chiudi programma");
            ToolTip1.SetToolTip(btnConferma, "Salva");
            ToolTip1.SetToolTip(btnMeno, "Diminuisci zoom");
            ToolTip1.SetToolTip(btnPiu, "Ingrandisci zoom");
            ToolTip1.SetToolTip(btnReset, "Imposta zoom originale");
            ToolTip1.SetToolTip(buttonStampa, "Stampa");
            ToolTip1.SetToolTip(btnCarica, "Carica preventivo");
            ToolTip1.SetToolTip(buttonPulisci , "Pulisci");
            ToolTip1.SetToolTip(buttonModificaQuantita, "Impostazioni");
            //Settaggio delle quantità fisse
            buttonQuantita1.Text = Setting.Istance.Q1;
            buttonQuantita2.Text = Setting.Istance.Q2;
            buttonQuantita3.Text = Setting.Istance.Q3;
            textBoxVariazione.Text = Setting.Istance.P1;
            textBoxVariazioneLav.Text = Setting.Istance.P2;
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
            this.Load += Form1_Load;
        }

        /// <summary>
        /// Funzione utilizzata per settare l'altezza della DataGridView appena viene lanciato il programma
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Load(object sender, EventArgs e)
        {
            altezzaDataGrid = dataGridView.Height;
        }

        /// <summary>
        /// Restituisce il valore del codice articolo inserito
        /// </summary>
        public string Articolo { get { return this.textBoxArticolo.Text; } }

        /// <summary>
        /// Funzione utilizzata per salvarmi i valori delle textbox ogni volta che ci entro
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void textBox_Enter(object sender, EventArgs e)
        {
            cliente = textBoxCliente.Text;
            articolo = textBoxArticolo.Text;
            quantita = textBoxQuantita.Text;
            variazione = textBoxVariazione.Text;
            variazionelav = textBoxVariazioneLav.Text;
        }

        /// <summary>
        /// Funzione che permette di cancellare le righe nella DataGridView ed aggiornare conseguentemente il DataSet
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void dataGridView_UserDeletingRow(object sender, DataGridViewRowCancelEventArgs e)
        {
            DialogResult usersChoice =
                MessageBox.Show("Confermare la cancellazione della riga selezionata e dei relativi semi-lavorati?", "Conferma eliminazione", MessageBoxButtons.OKCancel, MessageBoxIcon.Question);

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
                        
                        //Controllo quanli figli hanno come padre la riga cancellata, di conseguenza elimino anche loro
                        if (datarow["Codice Padre"].ToString() == codiceart)
                        {
                            m.ds.Tables["DistintaBase"].Rows.Remove(datarow);
                            i--;
                            rowscount--;
                        }
                    }
                    catch { }                  
                }
                //Controllo se la tabella ha chiave primaria
                DataColumn[] key = new DataColumn[1];
                if (m.ds.Tables["DistintaBase"].PrimaryKey.Length < 1)
                {
                    key[0] = m.ds.Tables["DistintaBase"].Columns["Rigo"];
                    m.ds.Tables["DistintaBase"].PrimaryKey = key;
                }

                //Quindi mi faccio una copia della riga che è stata selezionata per essere cancellata, perchè poi mi servirà per fare l'aggiornamento del dataset
                //quando avrò cancellato la riga originale
                DataRow datarowselezionata = m.ds.Tables["DistintaBase"].Rows.Find(row.Cells["Rigo"].Value.ToString());
                DataRow copiadiriga = m.ds.Tables["DistintaBase"].NewRow();
                for(int j = 0; j < m.ds.Tables["DistintaBase"].Columns.Count; j++)
                {
                    copiadiriga[j] = datarowselezionata[j].ToString();
                }
                m.ds.Tables["DistintaBase"].Rows.Remove(datarowselezionata);
                if(dataGridView.Rows.Count <= 1)
                {
                    textBoxQuantita.Text = "0";
                    textBoxVariazioneLav.Text = "0";
                    textBoxVariazione.Text = "0";
                    textBoxQuantita.Enabled = false;
                    textBoxVariazioneLav.Enabled = false;
                    textBoxVariazione.Enabled = false;
                }
                //Quindi aggiorno il DataSet e tutti i calcoli
                AggiornaParentela(copiadiriga);
                BindingGrid();
            }
        }

        /// <summary>
        /// Funzione per impostare il font della Form
        /// </summary>
        /// <param name="font"></param>
        private void SetFont(String font)
        {

            font = font.Replace('#', ' ');

            String fontFamil = font.Split('-')[0];
            float fontSize;
            float.TryParse(font.Split('-')[1], out fontSize);

            Font f = new Font(fontFamil, fontSize);
            Font bold = new Font(fontFamil, fontSize, FontStyle.Bold);
            // this.Font = f;
            //MessageBox.Show( this.groupBox1.Controls.ToString());
            if (f != null)
            {
                foreach (Control c in this.Controls)
                {
                    /*if (c is Label )//&& Setting.Istance.Font == "FO")//se si tratta di una label e nel fil di configurazione il la label hanno font normale
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
                    }*/
                    c.Font = f;
                }
                foreach(Control c in groupBoxQI.Controls)
                {
                    if(c.Name=="label24" || c.Name == "QIRicavoSingolo")
                    {
                        c.Font = bold;
                    }
                    else
                    {
                        c.Font = f;
                    }
                }
                //this.dataGridView.Font = f;
            }
        }

        /// <summary>
        /// Funzione che utilizzo per richiamare l'"helpxml" con i dati relativi ai clienti
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonHelpClienti_Click(object sender, EventArgs e)
        {
            Helper h = new Helper();
            String[] par = { };
            textBoxCliente.Focus();
            string temp =  h.StartHelper(Setting.Istance.HelpCliente, Setting.Istance.Ip, Setting.Istance.Port, Setting.Istance.Database, Setting.Istance.User, Setting.Istance.Password, "", "", "", par, "", Setting.Istance.Font, Setting.Istance.FontLabel);

            if (!String.IsNullOrEmpty(temp))
            {
                this.textBoxCliente.Text = temp;
            }
            textBoxArticolo.Focus();
        }

        /// <summary>
        /// Funzione che utilizzo per richiamare l'"helpxml" con i dati relativi agli articoli
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonHelpArticoli_Click(object sender, EventArgs e)
        {
            Helper h = new Helper();
            String[] par = { };
            textBoxArticolo.Focus();
            string temp =  h.StartHelper(Setting.Istance.HelpArticolo, Setting.Istance.Ip, Setting.Istance.Port, Setting.Istance.Database, Setting.Istance.User, Setting.Istance.Password, "", "", "", par, "", Setting.Istance.Font, Setting.Istance.FontLabel);

            if (!String.IsNullOrEmpty(temp))
            {
                this.textBoxArticolo.Text = temp;
            }
            textBoxCliente.Focus();
            textBoxQuantita.Focus();
        }

        /// <summary>
        /// Funzione per ripulire la screen
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonPulisci_Click(object sender, EventArgs e)
        {
            PulisciSchermata();
            //Se siamo in modalità di "creazione articolo da nuovo" allora setto alcune textbox con certi valori
            if (creaArticolo)
            {
                creaArticolo = true;
                textBoxCliente.Text = "RGG";
                labelCliente.Text = "RGG";
                textBoxArticolo.Text = "CREA_ARTICOLO";
            }
        }

        /// <summary>
        /// Funzione che permette di verificare se l'ID del cliente sia corretto, quindi stampa la descrizione.
        /// E' stata modificata durante lo sviluppo del software. Ora viene richiamata con l'evento Leave invece del TextChanged.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBoxCliente_TextChanged(object sender, EventArgs e)
        {
            if (!creaArticolo)
            {
                if (textBoxCliente.Text != cliente)
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
                              
        }

        /// <summary>
        /// Funzione ricorsiva che serve per esplodere la distinta base dell'articolo padre che viene passato come primo parametro.
        /// Il secondo parametro (livello) è fondamentalmente inutilizzato, era stato previsto all'inizio dello sviluppo del software ma poi 
        /// è stato lasciato perchè potrebbe risultare utile in futuro. Riguarda il livello della generazione dei figli. Più è alto maggiore è il livello di dinastia gerarchica.
        /// </summary>
        /// <param name="padre"></param>
        /// <param name="livello"></param>
        void EsplodiDistintaBase(string padre, int livello)
        {
            //Faccio una query per ricavare la DataRow presente nel dataset partendo dalla codice padre.
            IEnumerable<DataRow> query = from row in m.ds.Tables["DistintaBase"].AsEnumerable() where row.Field<String>("Rigo") == padre select row;
            DataRow rowpadre = query.First();
            //Verifico se è un semilavorato.In caso positivo salvo in "rows" il numero dei figli.
            int rows = m.VerificaSemilavorato(rowpadre);
            try
            {
                if (Double.Parse(rowpadre["Codice Centro"].ToString()) > 499)
                {
                    //Verifico se è una lavorazione esterna, altrimenti la catch prende l'errore e si continua con il resto della funzione
                    m.GestisciLavorazioneEsterna(rowpadre, rowpadre["Codice Padre"].ToString());
                }
            }
            catch { }
            try
            {
                m.FromDecimalToTime(rowpadre);
                //Se "rows" è maggiore di zero significa che l'articolo ha una distinta base e quindi dei figli.
                if (rows > 0)
                {
                    //Inizializzo le variabili per poter estrarre il costo totale del padre dalla somma dei costi dei figli.
                    double totalePadre = 0, totalevarPadre = 0;
                    int countrowdt = m.ds.Tables["DistintaBase"].Rows.Count;
                    int index = 1;
                    for(int i = 0; i < countrowdt; i++)
                    {
                        DataRow figlio = m.ds.Tables["DistintaBase"].Rows[i];
                        //Ogni volta che trovo un figlio, quindi che il codice articolo "padre" corrisponde al codice padre delle DataRow che sto scorrendo nel Dataset,
                        //faccio il richiamo di questa stessa funzione in modalità ricorsiva. Inoltre setto il rowindex che mi permette di capire la gerarchia e i livelli
                        //dei vari figli e della distinta base, aggiorno il valore del costo del padre e quindi proseguo con la ricerca.
                        if (figlio["Codice Padre"].ToString() == rowpadre["CODICE ART"].ToString())
                        {
                            figlio["Rigo"] = rowpadre["Rigo"].ToString() + "," + index;
                            figlio["Quantita` 1"] = Math.Round(Double.Parse(figlio["Quantita` 1"].ToString()) * Double.Parse(rowpadre["Quantita` 1"].ToString()), 4);
                            if(figlio["Qta 2"].ToString() != "" && rowpadre["Qta 2"].ToString() != "")
                            {
                                figlio["Qta 2"] = Math.Round(Double.Parse(figlio["Qta 2"].ToString()) * Double.Parse(rowpadre["Qta 2"].ToString()), 4);
                            }
                            if (figlio["Qta 3"].ToString() != "" && rowpadre["Qta 3"].ToString() != "")
                            {
                                figlio["Qta 3"] = Math.Round(Double.Parse(figlio["Qta 3"].ToString()) * Double.Parse(rowpadre["Qta 3"].ToString()), 4);
                            }
                            EsplodiDistintaBase(figlio["Rigo"].ToString(), ++livello);
                            totalePadre += Double.Parse(figlio["Totale"].ToString());
                            totalevarPadre += Double.Parse(figlio["Totale + %Var"].ToString());
                            index++;
                        }
                    }
                    //Una volta terminato il ciclo vado a calcolarmi i vari costi del rigo del padre.
                    rowpadre["Totale"] = Math.Round(totalePadre,2);
                    rowpadre["Costo Art"] = Math.Round(Double.Parse(rowpadre["Totale"].ToString()) / Double.Parse(rowpadre["Quantita` 1"].ToString()),2);
                    rowpadre["Totale + %Var"] = Math.Round(totalevarPadre,2);
                }
                //Altrimenti significa che il rigo passato alla funzione non ha figli, e quindi posso procedere con il calcolo dei costi di questa stessa riga.
                else
                {
                    //In base se il "padre" è un articolo o una lavorazione effettuo dei calcoli diversi per poter calcolare i costi totali del rigo.
                    if(rowpadre["CODICE ART"].ToString() != "")
                    {
                    
                        rowpadre["Totale"] = Math.Round(Double.Parse(rowpadre["Costo Art"].ToString()) * Double.Parse(rowpadre["Quantita` 1"].ToString()), 3);
                        rowpadre["Costo Art"] = Math.Round(Double.Parse(rowpadre["Totale"].ToString()) / Double.Parse(rowpadre["Quantita` 1"].ToString()), 2);
                        //Controllo se il rigo sia una lavorazione, quindi dovrebbe essere trattata in maniera diverso da una materia prima.
                        //In caso negativo la catch raccoglie l'errore e quindi viene trattato come una semplice materia prima.
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
                                            + (Double.Parse(rowpadre["Costo Mac"].ToString()) * Double.Parse(rowpadre["Tempo Mac decimale"].ToString()) * Double.Parse(rowpadre["Quantita` 1"].ToString()))
                                            + (Double.Parse(rowpadre["Costo Uomo"].ToString()) * Double.Parse(rowpadre["Tempo Uomo decimale"].ToString()) * Double.Parse(rowpadre["Quantita` 1"].ToString())),2);
                        rowpadre["Totale + %Var"] = Math.Round((Convert.ToDouble(rowpadre["Totale"].ToString()) * Convert.ToDouble(textBoxVariazioneLav.Text) / 100) + Convert.ToDouble(rowpadre["Totale"].ToString()), 2);

                    }

                }
            }
            catch { }
            return;
        }



        //Fare il test con questo codice --> SB02AL1505.0-1_02
        /// <summary>
        /// Funzione utilizzata per verificare che il codice dell'articolo sia corretto. Quindi si andrà a pescare l'intero albero genialogico con tutte le generazione dei figli
        /// e sottofigli. Infine si calcoleranno i costi e si coloreranno le celle della griglia.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBoxArticolo_TextChanged(object sender, EventArgs e)
        {
            if (!creaArticolo)
            {
                textBoxArticolo.Text = textBoxArticolo.Text.ToUpper();
                if (textBoxArticolo.Text != articolo)
                {
                    //Per prima cosa ripulisco il Dataset dalle due tabelle in cui viene salvato l'intera distinta base.
                    try
                    {
                        m.ds.Tables.Remove("DistintaBase");
                        m.ds.Tables.Remove("CodDistBase");
                        dataGridView.DataSource = null;
                    }
                    catch { }
                    DataTable dtArt, dtDistBase;
                    dtArt = m.ds.Tables["Articoli"];
                    //Cerco la riga relativa al codice inserito
                    DataRow dr = dtArt.Rows.Find(textBoxArticolo.Text);
                    codiceArticoloDettagli = dr;
                    if (dr != null)
                    {
                        textBoxArticolo.BackColor = Color.LightGreen;
                        labelArticolo.Text = "";
                        //Preparo la query per la ricerca della distinta base
                        string getDistBase = Setting.Istance.QueryCodDistBase.Replace("@articolo", textBoxArticolo.Text);
                        m.EstraiRisultatoQuery(getDistBase, "CodDistBase");
                        //Se le righe della tabella sono maggiorji di 0 significa che ha una distinta base
                        if (m.ds.Tables["CodDistBase"].Rows.Count > 0)
                        {
                            string distintaBase = Setting.Istance.QueryDistintaBase.Replace("@CodDistBase", m.ds.Tables["CodDistBase"].Rows[0][0].ToString());
                            //Aggiungo alla DataTable i primi figli del codice inserito
                            m.EstraiRisultatoQuery(distintaBase, "DistintaBase");
                            //Collego la datagrid con i valori e i dati del dataset
                            BindingSource bindingSource1 = new BindingSource();
                            bindingSource1.DataSource = m.ds.Tables["DistintaBase"];
                            dataGridView.DataSource = bindingSource1;
                            dtDistBase = m.ds.Tables["DistintaBase"];
                            int righeDataGrid = m.ds.Tables["DistintaBase"].Rows.Count;
                            //Livello iniziale, che appartiene ai figli dell'articolo padre indicato dall'utente
                            int livello = 1;
                            //Quindi comincio ad esplodere i figli per poter trovare via via tutte le generazioni, se sono presenti
                            for (int i = 0; i < righeDataGrid; i++)
                            {
                                DataRow figlio = m.ds.Tables["DistintaBase"].Rows[i];
                                figlio["Quantita` 1"] = Math.Round(Double.Parse(figlio["Quantita` 1"].ToString()) * Double.Parse(textBoxQuantita.Text.Replace('.', ',')), 4);
                                if(figlio["Qta 2"].ToString() != "")
                                {
                                    figlio["Qta 2"] = Math.Round(Double.Parse(figlio["Qta 2"].ToString()) * Double.Parse(textBoxQuantita.Text.Replace('.', ',')), 4);
                                }
                                if (figlio["Qta 3"].ToString() != "")
                                {
                                    figlio["Qta 3"] = Math.Round(Double.Parse(figlio["Qta 3"].ToString()) * Double.Parse(textBoxQuantita.Text.Replace('.', ',')), 4);
                                }
                                string rowindex = figlio["Rigo"].ToString();
                                EsplodiDistintaBase(rowindex, livello);
                            }
                            //Conversione dei tempi, da decimali a sessantesimi
                            m.FromDecimalToTime();

                            //Ordino la cella e imposto alcune regole
                            dataGridView.Sort(dataGridView.Columns["Rigo"], ListSortDirection.Ascending);
                            foreach (DataGridViewColumn column in dataGridView.Columns)
                            {
                                column.SortMode = DataGridViewColumnSortMode.NotSortable;
                                column.ReadOnly = true;
                            }
                            //Fare il test con questo codice --> SB02AL1505.0-1_02
                            //Infine coloro la griglia e faccio i calcoli relativi ai costi
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
                        labelArticolo.Text = dr[1].ToString();
                    }
                    //Altrimenti pulisco la form
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
            else
            {
                try
                {
                    if (textBoxArticolo.Text != articolo && m.ds.Tables["DistintaBase"].Rows.Count > 0)
                    {
                        foreach (DataRow row in m.ds.Tables["DistintaBase"].Rows)
                        {
                            if (row["Codice Padre"].ToString() == articolo)
                            {
                                row["Codice Padre"] = textBoxArticolo.Text;
                            }
                        }
                    }
                }
                catch { }
            }
        }

        /// <summary>
        /// Funzione utilizzata per cololare la datagrid in base al libello dei figli e alle celle che si possono modificare o no
        /// </summary>
        private void ColoraDataGrid()
        {
            foreach (DataGridViewRow padre in dataGridView.Rows)
            {
                if (padre.Cells["Rigo"].Value != null)
                {
                    string indiceriga = padre.Cells["Rigo"].Value.ToString();
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
                        if (padre.Cells["CODICE ART"].Value.ToString() == figlio["Codice Padre"].ToString())
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

        /// <summary>
        /// Funzione per aumentare lo zoom della form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void brnPiu_Click(object sender, EventArgs e)
        {
            if (this.zoom < 4)
            {
                this.zoom++;
                this.ridimensione(1);
            }
        }

        /// <summary>
        /// Funzione per diminuire lo zoom della form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnMeno_Click(object sender, EventArgs e)
        {
            if (this.zoom > -4)
            {
                this.zoom--;
                this.ridimensione(-1);
            }
        }

        /// <summary>
        /// Funzione per resettare lo zoom della form
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnReset_Click(object sender, EventArgs e)
        {
            this.ridimensione(this.zoom * -1);
            this.zoom = 0;
        }

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
                    c.Location = new Point(c.Location.X + (size * 5), c.Location.Y);            
                    c.Font = new Font(c.Font.FontFamily, c.Font.Size + size);
                }
                if (c is Label)
                {
                    c.Height = c.Height + size;
                    c.Width = c.Width + size;
                    //c.Width += size * 15;
                    c.Font = new Font(c.Font.FontFamily, c.Font.Size + size);
                }
                if(c is Button)
                {
                    c.Location = new Point(c.Location.X + (size * 5), c.Location.Y);
                }
            }
            foreach (Control c in groupBoxTotPerQuant.Controls)
            {
                if (c is Label)
                {
                    c.Height = c.Height + size;
                    c.Width = c.Width + size;
                    c.Font = new Font(c.Font.FontFamily, c.Font.Size + size);
                    if (c.Name == "label15")
                    {
                        c.Font = new Font(c.Font.FontFamily, c.Font.Size + size, FontStyle.Bold);
                    }
                    else
                    {
                        c.Font = new Font(c.Font.FontFamily, c.Font.Size + size);
                    }
                }
            }
            foreach (Control c in groupBoxQI.Controls)
            {
                if (c is Label)
                {
                    c.Height = c.Height + size;
                    c.Width = c.Width + size;
                    if (c.Name == "label24" || c.Name == "QIRicavoSingolo")
                    {
                        c.Font = new Font(c.Font.FontFamily, c.Font.Size + size, FontStyle.Bold);
                    }
                    else
                    {
                        c.Font = new Font(c.Font.FontFamily, c.Font.Size + size);
                    }
                }
            }
            try
            {
                this.dataGridView.Font = new Font(this.dataGridView.Font.FontFamily, this.dataGridView.Font.Size + size * 2);
            }
            catch { }
        }

        /// <summary>
        /// Funzione utilizzata per chiudere il programma.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnEsci_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void TerminaProgramma(object sender, EventArgs e)
        {
            formLogin.Close();
        }

        /// <summary>
        /// Funzione utilizzata per pulire la form e il Dataset
        /// </summary>
        private void PulisciSchermata()
        {
            try
            {
                dataGridView.DataSource = null;
                m.ds.Tables.Remove("DistintaBase");
                m.ds.Tables.Remove("CodDistBase");                
            }
            catch { }
            textBoxCliente.Focus();
            labelCliente.Text = "<Descrizione cliente>";
            labelArticolo.Text = "<Descrizione articolo>";
            textBoxVariazione.Enabled = false;
            textBoxVariazioneLav.Enabled = false;
            textBoxQuantita.Enabled = false;
            textBoxCliente.Text = "";
            textBoxArticolo.Text = "";
            textBoxArticolo.BackColor = (textBoxArticolo.Text == "" ? Color.White : Color.OrangeRed);
            textBoxCliente.BackColor = (textBoxCliente.Text == "" ? Color.White : Color.OrangeRed);
            textBoxQuantita.Text = "1";
            textBoxVariazione.Text = "0";
            textBoxVariazioneLav.Text = "0";
            textBoxNote.Text = "";
            precedenteQuantita = 1;
            CostoMac1.Text = "0";
            CostoMac2.Text = "0";
            CostoMac3.Text = "0";
            CostoUomo1.Text = "0";
            CostoUomo2.Text = "0";
            CostoUomo3.Text = "0";
            Articoli1.Text = "0";
            Articoli2.Text = "0";
            Articoli3.Text = "0";
            CostoSing1.Text = "0";
            CostoSing2.Text = "0";
            CostoSing3.Text = "0";
            RicavoSing1.Text = "0";
            RicavoSing2.Text = "0";
            RicavoSing3.Text = "0";
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
            QICostoSingolo.Text = "0";
            QIRicavoSingolo.Text = "0";
            groupBoxQI.Text = "Quantità impostata: 1";
            labelIDpreventivo.Text = "Nuovo preventivo";
        }

        /// <summary>
        /// Funzione per cambiare modalità, tra "creazione articolo nuovo" e "modifica distinta base esistente"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnNuovo_Click(object sender, EventArgs e)
        {
            PulisciSchermata();
            if (!creaArticolo)
            {
                creaArticolo = true;
                this.BackColor = Color.NavajoWhite;               
                textBoxCliente.Text = "RGG";
                labelCliente.Text = "RGG";
                textBoxArticolo.Text = "CREA_ARTICOLO";
            }
            else
            {
                creaArticolo = false;
                this.BackColor = Color.WhiteSmoke;
            }

        }

        /// <summary>
        /// Funzione per eseguire di nuovo i calcoli e la colorazione
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnRefresh_Click(object sender, EventArgs e)
        {
            BindingGrid();
        }

        /// <summary>
        /// Effettivamento non so se sia utilizzata, ma serve per eseguire una pulizia dei campi quando vengono modificati.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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

        /// <summary>
        /// Anche questa funzione viene richiamata con l'evento Leave e non con TextChaged.
        /// Va a modificare tutti i ricavi delle righe che hanno codice articolo valorizzato, quindi che si trattano di materie prime, semilavorati o lavorazioni esterne.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBoxVariazione_TextChanged(object sender, EventArgs e)
        {
            if(textBoxVariazione.Text != variazione)
            {
                try
                {
                    double variazioneInserita = Double.Parse(textBoxVariazione.Text);
                    double Totale, variazione;
                    //Solito ciclo che scorre il dataset, cerca le righe interessate e va a modificare il valore del giusto campo
                    for (int rowcount = 0; rowcount <= dataGridView.Rows.Count - 2; rowcount++)
                    {
                        DataGridViewRow row = dataGridView.Rows[rowcount];
                        try
                        {
                            if (row.Cells["Totale"].ToString() != null && row.Cells["Codice Art"].Value.ToString() != "" && row.Cells["Codice centro"].Value.ToString() == "")
                            {
                                Totale = Double.Parse(row.Cells["Totale"].Value.ToString());
                                if (rowcount!= dataGridView.Rows.Count - 2)
                                {

                                    if (row.Cells["Codice Art"].Value.ToString() != dataGridView.Rows[rowcount + 1].Cells["Codice Padre"].Value.ToString())
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
                    //Infine si aggiornano i valori nella form ricalcolando il tutto
                    BindingGrid();
                }
                catch
                {
                    MessageBox.Show("Il valore inserito non è valido!");
                }
            }          
        }


        /// <summary>
        /// Anche questa funzione viene richiamata con l'evento Leave e non con TextChaged.
        /// Va a modificare tutti i ricavi delle righe che hanno codice articolo valorizzato, quindi che si trattano di lavorazioni interne.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBoxVariazioneLav_TextChanged(object sender, EventArgs e)
        {
            if(textBoxVariazioneLav.Text != variazionelav)
            {
                try
                {
                    double variazioneInserita = Double.Parse(textBoxVariazioneLav.Text);
                    double Totale, variazione;
                    //Solito ciclo che scorre il dataset, cerca le righe interessate e va a modificare il valore del giusto campo
                    for (int rowcount = 0; rowcount <= dataGridView.Rows.Count - 2; rowcount++)
                    {
                        DataGridViewRow row = dataGridView.Rows[rowcount];
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
                    //Infine si aggiornano i valori nella form ricalcolando il tutto
                    BindingGrid();
                }
                catch
                {
                    MessageBox.Show("Il valore inserito non è valido!");
                }
            }           
        }

        /// <summary>
        /// Funzione utilizzata per richiamare un'alrtra form per l'inserimento di un rigo aggiuntivo
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnModifica_Click(object sender, EventArgs e)
        {
            InserisciRigo inserisciRigo = new InserisciRigo(this, m);
            inserisciRigo.Show();
        }

        /// <summary>
        /// Funzione per modificare la quantità richiesta dall'utente, quindi vado a modificare ogni quantità delle lavorazioni, semilavorati e materie prime.
        /// Eseguo di nuovo tutti i calcoli per calcolarmi i costi e i totali e aggiorno la form.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
                    if (dataGridView.Rows.Count > 0)
                    {
                        //Solito ciclo che scorre il dataset, cerca le righe interessate e va a modificare il valore del giusto campo
                        for (int rowcount = 0; rowcount <= dataGridView.Rows.Count - 2; rowcount++)
                        {
                            DataGridViewRow row = dataGridView.Rows[rowcount];
                            quantitaPrecedente = Double.Parse(row.Cells["Quantita` 1"].Value.ToString());
                            row.Cells["Quantita` 1"].Value = Math.Round(Double.Parse(row.Cells["Quantita` 1"].Value.ToString()) / precedenteQuantita * quantitaNuova, 4);
                            if((row.Cells["Qta 2"].Value.ToString() != ""))
                            {
                                quantitaPrecedente = Double.Parse(row.Cells["Qta 2"].Value.ToString());
                                row.Cells["Qta 2"].Value = Math.Round(Double.Parse(row.Cells["Qta 2"].Value.ToString()) / precedenteQuantita * quantitaNuova, 4);
                            }
                            if ((row.Cells["Qta 3"].Value.ToString() != ""))
                            {
                                quantitaPrecedente = Double.Parse(row.Cells["Qta 3"].Value.ToString());
                                row.Cells["Qta 3"].Value = Math.Round(Double.Parse(row.Cells["Qta 3"].Value.ToString()) / precedenteQuantita * quantitaNuova, 4);
                            }
                        }
                        //Mi tengo salvato la quantità precedente in modo da eseguire i calcoli corretti.                        
                        BindingGrid();
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

        /// <summary>
        /// Funzione che riporta il valore della quantità fisso nella textbox della quantità dell'articolo selezionato dal cliente.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button1_Click(object sender, EventArgs e)
        {
            textBoxQuantita.Focus();
            textBoxQuantita.Text = buttonQuantita1.Text;          
            textBoxVariazione.Focus();
            textBoxQuantita.Focus();
        }

        /// <summary>
        /// Funzione che riporta il valore della quantità fisso nella textbox della quantità dell'articolo selezionato dal cliente.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button2_Click(object sender, EventArgs e)
        {
            textBoxQuantita.Focus();
            textBoxQuantita.Text = buttonQuantita2.Text;
            textBoxVariazione.Focus();
            textBoxQuantita.Focus();
        }

        /// <summary>
        /// Funzione che riporta il valore della quantità fisso nella textbox della quantità dell'articolo selezionato dal cliente.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void button3_Click(object sender, EventArgs e)
        {
            textBoxQuantita.Focus();
            textBoxQuantita.Text = buttonQuantita3.Text;
            textBoxVariazione.Focus();
            textBoxQuantita.Focus();
        }

        /// <summary>
        /// Funzione utilizata per nascondere oppure mostrare i figli della riga che ho cliccato.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            try
            {
                
                int rowcount = m.ds.Tables["DistintaBase"].Rows.Count;
                string valoreCella = dataGridView.CurrentCell.Value.ToString();
                if (dataGridView.CurrentCell.ColumnIndex == 2)
                {
                    for (int i = dataGridView.Rows.Count - 2; i >= 0; i--)
                    {
                        if (valoreCella == dataGridView["Codice Padre", i].Value.ToString() && i != rowcount - 1)
                        {
                            if (dataGridView.Rows[i].Visible == false)
                            {
                                dataGridView.Rows[i].Visible = true;
                            }
                            else
                            {
                                dataGridView.Rows[i].Visible = false;
                            }
                        }
                        else if (valoreCella == dataGridView["Codice Padre", i].Value.ToString() && dataGridView["Codice Padre", i].Value.ToString() != "")
                        {
                            if (dataGridView.Rows[i].Visible == false)
                            {
                                dataGridView.Rows[i].Visible = true;
                            }
                            else
                            {
                                dataGridView.Rows[i].Visible = false;
                            }
                        }
                    }
                }
                
            }
            catch { }
            
        }

        /// <summary>
        /// Questa funzione viene richiamata ogni volta che il valore di una cella nella dataGrid viene modificata.
        /// Nel caso venga modificato un tempo ci si occupa della conversione decimale/sessantesimi e di rieseguire i calcoli dei costi e dei totali.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            try            
            {
                //double valoreCella = Double.Parse(dataGridView.CurrentCell.Value.ToString());
                int colindex = e.ColumnIndex;// dataGridView.CurrentCell.ColumnIndex;

                int rowindex = e.RowIndex;//dataGridView.CurrentCell.RowIndex;
                //Verifico che la tabella abbia una chiave primaria.
                DataColumn[] key = new DataColumn[1];
                if (m.ds.Tables["DistintaBase"].PrimaryKey.Length < 1)
                {
                    key[0] = m.ds.Tables["DistintaBase"].Columns["Rigo"];
                    m.ds.Tables["DistintaBase"].PrimaryKey = key;

                }
                string indiceriga = dataGridView["Rigo", rowindex].Value.ToString();
                DataRow r = m.ds.Tables["DistintaBase"].Rows.Find(indiceriga);
                r[colindex] = r[colindex].ToString().Replace('.', ',');
                if (r[colindex].ToString().IndexOf(',') < 0)
                {
                    if(r[colindex].ToString().Length == 0)
                    {
                        r[colindex] = "0";
                    }
                    if(colindex < 11 || colindex == 16)
                    {
                        r[colindex] = r[colindex].ToString() + ",00";
                    }
                    else
                    {
                        /*r[colindex] = r[colindex].ToString() + ",0000";*/
                        //SE LENGTH = 6 ALLORA SPLITTA I PRIMI 2 CARATTERI E METTI LA VIRGOLA... ANCHE SE LENGTH = 4
                        if(r[colindex].ToString().Length == 4 || r[colindex].ToString().Length == 6)
                        {
                            r[colindex] = Int32.Parse(r[colindex].ToString().Substring(0, 2)).ToString() + "," + (r[colindex].ToString().Length == 6? r[colindex].ToString().Substring(2) : r[colindex].ToString().Substring(2) + "00");
                        }else if(r[colindex].ToString().Length == 2)
                        {
                            r[colindex] = Int32.Parse(r[colindex].ToString()).ToString() + ",0000";
                        }
                        else
                        {
                            r[colindex] = Int32.Parse(r[colindex].ToString()).ToString();
                        }
                    }
                }
                else if (r[colindex].ToString().IndexOf(',') != r[colindex].ToString().LastIndexOf(','))
                {
                    MessageBox.Show("Il valore inserito non è valido!\nI formati consentiti sono della forma:\nHH - HHMM - HHMMSS - HHH\nHH,MM - HH,MMSS - HHH,MM - HHH,MMSS\nHH.MM - HH.MMSS - HHH.MM - HHH.MMSS");
                    r[colindex] = "0";
                }
                else
                {
                    int aggiungizeri = r[colindex].ToString().Substring(r[colindex].ToString().IndexOf(',')).Length;
                    string decimaliAppoggio = r[colindex].ToString().Substring(r[colindex].ToString().IndexOf(','));
                    r[colindex] = Int32.Parse(r[colindex].ToString().Substring(0, r[colindex].ToString().IndexOf(','))).ToString() + r[colindex].ToString().Substring(r[colindex].ToString().IndexOf(','));
                    if (colindex > 11 && colindex != 16)
                    {
                        for(int z = aggiungizeri; z <= 4; z++)
                        {
                            r[colindex] += "0";
                        }
                    }
                }
                string nomeColonna = m.ds.Tables["DistintaBase"].Columns[colindex].ColumnName;
                //Nel caso fosse stato modificato un tempo eseguo la conversione, altrimenti proseguo.
                if (nomeColonna == "Setup Mac" || nomeColonna == "Setup Uomo" || nomeColonna == "Tempo Mac" || nomeColonna == "Tempo Uomo")
                {
                    m.FromTimeToDecimal(r);
                }
                //Se è un semilavorato il totale viene eseguito in una certa maniera altrimenti lo tratto come una lavorazione
                if (r["Codice Art"].ToString() != "")
                {
                    r["Totale"] = Math.Round(Double.Parse(r["Costo Art"].ToString()) * Double.Parse(r["Quantita` 1"].ToString()), 2);
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
                    if (Double.Parse(r["Quantita` 1"].ToString()) == 0)
                    {
                        r["Totale"] = 0;
                        r["Totale + %Var"] = 0;
                    }
                    else
                    {
                        r["Totale"] = Math.Round((Double.Parse(r["Costo Att Mac"].ToString()) * Double.Parse(r["Setup Mac decimale"].ToString()))
                        + (Double.Parse(r["Costo Att Uomo"].ToString()) * Double.Parse(r["Setup Uomo decimale"].ToString()))
                        + (Double.Parse(r["Costo Mac"].ToString()) * Double.Parse(r["Tempo Mac decimale"].ToString()) * Double.Parse(r["Quantita` 1"].ToString()))
                        + (Double.Parse(r["Costo Uomo"].ToString()) * Double.Parse(r["Tempo Uomo decimale"].ToString()) * Double.Parse(r["Quantita` 1"].ToString())), 2);
                        r["Totale + %Var"] = Math.Round((Convert.ToDouble(r["Totale"].ToString()) * Convert.ToDouble(textBoxVariazioneLav.Text) / 100) + Convert.ToDouble(r["Totale"].ToString()), 2);
                    }
                }
                //Quindi aggiorniamo tutta la distinta base partendo dalla riga modificata, se è possibile si cerca di saltare cicli e aggiornamenti in quanto richiedono tempo
                //e rallentano il software.
                AggiornaParentela(r);
                CalcolaPrezzoQuantitaImpostata();
                CalcolaPrezzoTotaliPerQuantita();
                if (nomeColonna != "Quantita` 1" && nomeColonna != "Totale + %Var")
                {
                    //BindingGrid();
                }             
            }
            catch
            {
                MessageBox.Show("Il valore inserito non è valido!");
            }
            
        }

        //Fare il test con questo codice --> SB02AL1505.0-1_02
        /// <summary>
        /// Accetta come parametro di ingresso una Datarow che dovrebbe rappresentare una riga figlio. La funzione ha il compito di individuare
        /// i fratelli della DataRow di ingresso e quindi aggiornare i costi del padre. Succewssivamente viene chiamata ricorsivamente passando come parametro di ingresso
        /// la riga padre per continuare il calcolo fino al primo livello in un sistema bottom-up.
        /// </summary>
        /// <param name="figlio"></param>
        private void AggiornaParentela(DataRow figlio)
        {

            double totalePadre = 0, totaleVarPadre = 0;
            if (figlio["Codice Padre"].ToString() != textBoxArticolo.Text)
            {
                //Classico ciclo per individuare i fratelli della dataRow passata in ingresso
                foreach (DataRow fratelli in m.ds.Tables["DistintaBase"].Rows)
                {
                    if (fratelli["Codice Padre"].ToString() == figlio["Codice Padre"].ToString())
                    {
                        totalePadre += Double.Parse(fratelli["Totale"].ToString());
                        totaleVarPadre += Double.Parse(fratelli["Totale + %Var"].ToString());
                    }
                }
                //Ricerco la riga padre
                IEnumerable<DataRow> query = from row in m.ds.Tables["DistintaBase"].AsEnumerable() where row.Field<String>("CODICE ART") == figlio["Codice Padre"].ToString() select row;
                int count = query.Count();
                //Quindi aggiorno e chiamo ricorsivamente la funzione 
                if (count > 0)
                {
                    DataRow rowpadre = query.First();
                    rowpadre["Totale"] = Math.Round(totalePadre, 2);
                    rowpadre["COSTO ART"] = Math.Round(totalePadre / Double.Parse(rowpadre["Quantita` 1"].ToString()), 2);
                    rowpadre["Totale + %Var"] = Math.Round(totaleVarPadre, 2);
                    AggiornaParentela(rowpadre);
                }               
            }
            return;
        }

        /// <summary>
        /// Permette di calcolare i costi totali e parziali del preventivo generato in base alla quantità impostata dall'utente.
        /// Questa funzione modifica i valori che sono in basso-destra nella form principale.
        /// Il funzionamento è il classico: si scorre il dataset, si verifica se la riga abbia figli o no, in base a questo si va a calcolare
        /// i totali considerando solo le righe che sonoo i figli principali dell'articolo inserito.
        /// </summary>
        private void CalcolaPrezzoQuantitaImpostata()
        {
            double colCostoMac = 0, colCostoUomo = 0, colCostoMat = 0,  colTotale = 0, colTotaleVar = 0;
            //Scorro il dataset
            foreach(DataRow row in m.ds.Tables["DistintaBase"].Rows)
            {
                bool hafiglio = false;
                //Controllo che la datarow presa in considerazione abbia figli
                foreach (DataRow figlio in m.ds.Tables["DistintaBase"].Rows)
                {
                    if (row["CODICE ART"].ToString() == figlio["Codice Padre"].ToString())
                    {
                        hafiglio = true;
                        break;
                    }
                }
                //Se non ha figli è una materia prima, quindi ne tengo conto
                if (!hafiglio)
                {
                    if (row["CODICE ART"].ToString() != "" && row["Codice Lav"].ToString() == "")
                    {
                        colCostoMat += (Double.Parse(row["Totale"].ToString()));
                    }
                }
                //Guardo la generazione del figlio ed il livello, se il rowindex contiene righe vuol dire che non è uno dei figli principale dell'articolo inserito
                int generazione = FindNumberOfChar(',', row["Rigo"].ToString());
                //Altrimenti si tratta di una lavorazione, quindi ne tengo conto riguardo ai costi delle lavorazioni
                try
                {
                    colCostoMac += (Double.Parse(row["Costo Att Mac"].ToString()) * Double.Parse(row["Setup Mac decimale"].ToString()));
                    colCostoMac += (Double.Parse(row["Costo Mac"].ToString()) * Double.Parse(row["Tempo Mac decimale"].ToString()) * Double.Parse(row["Quantita` 1"].ToString()));
                    colCostoUomo += (Double.Parse(row["Costo Att Uomo"].ToString()) * Double.Parse(row["Setup Uomo decimale"].ToString()));
                    colCostoUomo += (Double.Parse(row["Costo Uomo"].ToString()) * Double.Parse(row["Tempo Uomo decimale"].ToString()) * Double.Parse(row["Quantita` 1"].ToString()));
                }
                catch { }
                //Se la generazione è uguale a 0 allora ne tengo conto dei totali, i figli dei figli non li conto
                if (generazione == 0)
                {
                    
                    colTotale += Double.Parse(row["Totale"].ToString());
                    colTotaleVar += Double.Parse(row["Totale + %Var"].ToString());
                }
            }
            //Una volta finito il ciclo stampo nella form i relativi valori
            QIcostomac.Text = Math.Round(colCostoMac, 2).ToString();
            QIcostouomo.Text = Math.Round(colCostoUomo,2).ToString();
            QIarticoli.Text = Math.Round(colCostoMat, 2).ToString();
            QItotale.Text = Math.Round(colTotale,2).ToString();
            QItotalevar.Text = Math.Round(colTotaleVar,2).ToString();
            QICostoSingolo.Text = Math.Round(Double.Parse(QItotale.Text) / Double.Parse(textBoxQuantita.Text),2).ToString();
            QIRicavoSingolo.Text = Math.Round(Double.Parse(QItotalevar.Text) / Double.Parse(textBoxQuantita.Text),2).ToString();
        }

        /// <summary>
        /// Funzione simile alla precedente (CalcolaPrezzoQuantitaImpostata()), serve per aggiornare i valori che si trovano in basso a sinistra nella form principale.
        /// Funzionamento sempre simile: scorro il dataset, per ogni riga controllo se abbia figli, e in caso non li abbia tengo in considerazione la riga per
        /// sommare i suoi valori nei costi totali e poi stamparli nella form.
        /// </summary>
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
            //Scorro il dataset
            foreach(DataRow padre in m.ds.Tables["DistintaBase"].Rows)
            {
                bool hafiglio = false;
                //Controllo se la riga ha figli, basta scorrere di nuovo il padre e vedere per ogni rigo quali di questo ha il valore nella cella "Codice Padre" il valore
                //preso dalla datarow "padre"
                foreach(DataRow figlio in m.ds.Tables["DistintaBase"].Rows)
                {                      
                    if(padre["CODICE ART"].ToString() == figlio["Codice Padre"].ToString())
                    {
                        hafiglio = true;
                        break;
                    }
                }
                //Se non ha figli controllo se sia una lavorazione o un semilavorato
                if (!hafiglio)
                {
                    //In base alla verifica procedo eseguendo i calcoli
                    if (padre["CODICE ART"].ToString() != "" && padre["Codice lav"].ToString() == "")
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
                    else if(padre["CODICE ART"].ToString() == "" && padre["Codice lav"].ToString() != "")
                    {
                        colCostoMac1 += (Double.Parse(padre["Costo Att Mac"].ToString()) * Double.Parse(padre["Setup Mac decimale"].ToString()));
                        colCostoMac1 += (Double.Parse(padre["Costo Mac"].ToString()) * Double.Parse(padre["Tempo Mac decimale"].ToString()) * (Double.Parse(padre["Quantita` 1"].ToString()) / quantita) * Double.Parse(Setting.Istance.Q1));
                        colCostoMac2 += (Double.Parse(padre["Costo Att Mac"].ToString()) * Double.Parse(padre["Setup Mac decimale"].ToString()));
                        colCostoMac2 += (Double.Parse(padre["Costo Mac"].ToString()) * Double.Parse(padre["Tempo Mac decimale"].ToString()) * (Double.Parse(padre["Quantita` 1"].ToString()) / quantita) * Double.Parse(Setting.Istance.Q2));
                        colCostoMac3 += (Double.Parse(padre["Costo Att Mac"].ToString()) * Double.Parse(padre["Setup Mac decimale"].ToString()));
                        colCostoMac3 += (Double.Parse(padre["Costo Mac"].ToString()) * Double.Parse(padre["Tempo Mac decimale"].ToString()) * (Double.Parse(padre["Quantita` 1"].ToString()) / quantita) * Double.Parse(Setting.Istance.Q3));

                        colCostoUomo1 += (Double.Parse(padre["Costo Att Uomo"].ToString()) * Double.Parse(padre["Setup Uomo decimale"].ToString()));
                        colCostoUomo1 += (Double.Parse(padre["Costo Uomo"].ToString()) * Double.Parse(padre["Tempo Uomo decimale"].ToString()) * (Double.Parse(padre["Quantita` 1"].ToString()) / quantita) * Double.Parse(Setting.Istance.Q1));
                        colCostoUomo2 += (Double.Parse(padre["Costo Att Uomo"].ToString()) * Double.Parse(padre["Setup Uomo decimale"].ToString()));
                        colCostoUomo2 += (Double.Parse(padre["Costo Uomo"].ToString()) * Double.Parse(padre["Tempo Uomo decimale"].ToString()) * (Double.Parse(padre["Quantita` 1"].ToString()) / quantita) * Double.Parse(Setting.Istance.Q2));
                        colCostoUomo3 += (Double.Parse(padre["Costo Att Uomo"].ToString()) * Double.Parse(padre["Setup Uomo decimale"].ToString()));
                        colCostoUomo3 += (Double.Parse(padre["Costo Uomo"].ToString()) * Double.Parse(padre["Tempo Uomo decimale"].ToString()) * (Double.Parse(padre["Quantita` 1"].ToString()) / quantita ) * Double.Parse(Setting.Istance.Q3));

                        double totaleriga1 = 0;
                        totaleriga1 += (Double.Parse(padre["Costo Att Mac"].ToString()) * Double.Parse(padre["Setup Mac decimale"].ToString())) + (Double.Parse(padre["Costo Mac"].ToString()) * Double.Parse(padre["Tempo Mac decimale"].ToString()) * (Double.Parse(padre["Quantita` 1"].ToString()) / quantita * Double.Parse(Setting.Istance.Q1)));
                        totaleriga1 += (Double.Parse(padre["Costo Att Uomo"].ToString()) * Double.Parse(padre["Setup Uomo decimale"].ToString())) + (Double.Parse(padre["Costo Uomo"].ToString()) * Double.Parse(padre["Tempo Uomo decimale"].ToString()) * (Double.Parse(padre["Quantita` 1"].ToString()) / quantita) * Double.Parse(Setting.Istance.Q1));
                        double totaleriga2 = 0;
                        totaleriga2 += (Double.Parse(padre["Costo Att Mac"].ToString()) * Double.Parse(padre["Setup Mac decimale"].ToString())) + (Double.Parse(padre["Costo Mac"].ToString()) * Double.Parse(padre["Tempo Mac decimale"].ToString()) * (Double.Parse(padre["Quantita` 1"].ToString()) / quantita * Double.Parse(Setting.Istance.Q2)));
                        totaleriga2 += (Double.Parse(padre["Costo Att Uomo"].ToString()) * Double.Parse(padre["Setup Uomo decimale"].ToString())) + (Double.Parse(padre["Costo Uomo"].ToString()) * Double.Parse(padre["Tempo Uomo decimale"].ToString()) * (Double.Parse(padre["Quantita` 1"].ToString()) / quantita) * Double.Parse(Setting.Istance.Q2));
                        double totaleriga3 = 0;
                        totaleriga3 += (Double.Parse(padre["Costo Att Mac"].ToString()) * Double.Parse(padre["Setup Mac decimale"].ToString())) + (Double.Parse(padre["Costo Mac"].ToString()) * Double.Parse(padre["Tempo Mac decimale"].ToString()) * (Double.Parse(padre["Quantita` 1"].ToString()) / quantita * Double.Parse(Setting.Istance.Q3)));
                        totaleriga3 += (Double.Parse(padre["Costo Att Uomo"].ToString()) * Double.Parse(padre["Setup Uomo decimale"].ToString())) + (Double.Parse(padre["Costo Uomo"].ToString()) * Double.Parse(padre["Tempo Uomo decimale"].ToString()) * (Double.Parse(padre["Quantita` 1"].ToString()) / quantita) * Double.Parse(Setting.Istance.Q3));

                        colTotale1 += totaleriga1;
                        colTotaleVar1 += (totaleriga1 * Double.Parse(textBoxVariazioneLav.Text) / 100) + totaleriga1;
                        colTotale2 += totaleriga2;
                        colTotaleVar2 += (totaleriga2 * Double.Parse(textBoxVariazioneLav.Text) / 100) + totaleriga2;
                        colTotale3 += totaleriga3;
                        colTotaleVar3 += (totaleriga3 * Double.Parse(textBoxVariazioneLav.Text) / 100) + totaleriga3;
                    }
                    else
                    {
                        colTotale1 += (Double.Parse(padre["Totale"].ToString()) / quantita) * Double.Parse(Setting.Istance.Q1);
                        colTotale2 += (Double.Parse(padre["Totale"].ToString()) / quantita) * Double.Parse(Setting.Istance.Q2);
                        colTotale3 += (Double.Parse(padre["Totale"].ToString()) / quantita) * Double.Parse(Setting.Istance.Q3);
                        colTotaleVar1 += (Double.Parse(padre["Totale + %Var"].ToString()) / quantita) * Double.Parse(Setting.Istance.Q1);
                        colTotaleVar2 += (Double.Parse(padre["Totale + %Var"].ToString()) / quantita) * Double.Parse(Setting.Istance.Q2);
                        colTotaleVar3 += (Double.Parse(padre["Totale + %Var"].ToString()) / quantita) * Double.Parse(Setting.Istance.Q3);
                    }                   
                }
            }
            //Una volta completato il ciclo per tutte le righe del dataset, stampo nella form i risultati.
            CostoMac1.Text = Math.Round(colCostoMac1,2).ToString();
            CostoMac2.Text = Math.Round(colCostoMac2,2).ToString();
            CostoMac3.Text = Math.Round(colCostoMac3,2).ToString();
            CostoUomo1.Text = Math.Round(colCostoUomo1,2).ToString();
            CostoUomo2.Text = Math.Round(colCostoUomo2,2).ToString();
            CostoUomo3.Text = Math.Round(colCostoUomo3,2).ToString();
            Articoli1.Text = Math.Round(colArticoli1, 2).ToString();
            Articoli2.Text = Math.Round(colArticoli2, 2).ToString();
            Articoli3.Text = Math.Round(colArticoli3, 2).ToString();
            CostoSing1.Text = Math.Round(colTotale1 / Double.Parse(Setting.Istance.Q1), 2).ToString();
            CostoSing2.Text = Math.Round(colTotale2 / Double.Parse(Setting.Istance.Q2), 2).ToString();
            CostoSing3.Text = Math.Round(colTotale3 / Double.Parse(Setting.Istance.Q3), 2).ToString();
            RicavoSing1.Text = Math.Round(colTotaleVar1 / Double.Parse(Setting.Istance.Q1), 2).ToString();
            RicavoSing2.Text = Math.Round(colTotaleVar2 / Double.Parse(Setting.Istance.Q2), 2).ToString();
            RicavoSing3.Text = Math.Round(colTotaleVar3 / Double.Parse(Setting.Istance.Q3), 2).ToString();
            Tot1.Text = Math.Round(colTotale1,2).ToString();
            Tot2.Text = Math.Round(colTotale2,2).ToString();
            Tot3.Text = Math.Round(colTotale3,2).ToString();
            TotVar1.Text = Math.Round(colTotaleVar1,2).ToString();
            TotVar2.Text = Math.Round(colTotaleVar2,2).ToString();
            TotVar3.Text = Math.Round(colTotaleVar3,2).ToString();
        }
        //Fare il test con questo codice --> SB02AL1505.0-1_02

        /// <summary>
        /// Funzione ausiliaria per poter contare il numero di caratteri passato come parametro nella stringa passata come parametro.
        /// Ci permette di cercale le virgole all'interno del rowindex per capire di quale generazione fa parte la riga.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="searched"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Funzione che gestisce l'evento del button conferma.
        /// Salva i valori di testata in un array e richiama la funzione di salvataggio nel database del dataset corrente.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnConferma_Click(object sender, EventArgs e)
        {
            if (textBoxCliente.Text != "" && textBoxArticolo.Text != "" && textBoxNote.Text != "")
            {
                string[] valoriTestata = new string[40];
                valoriTestata[0] = textBoxCliente.Text;
                valoriTestata[1] = textBoxArticolo.Text;
                valoriTestata[2] = textBoxQuantita.Text;
                valoriTestata[3] = textBoxVariazione.Text;
                valoriTestata[4] = textBoxVariazioneLav.Text;
                valoriTestata[5] = QItotale.Text;
                valoriTestata[6] = QItotalevar.Text;
                valoriTestata[7] = textBoxNote.Text;

                valoriTestata[8] = QIarticoli.Text;
                valoriTestata[9] = QIcostomac.Text;
                valoriTestata[10] = QIcostouomo.Text;
                valoriTestata[11] = QICostoSingolo.Text;
                valoriTestata[12] = QIRicavoSingolo.Text;

                valoriTestata[13] = labelCliente.Text;
                valoriTestata[14] = labelArticolo.Text;

                valoriTestata[15] = user;

                valoriTestata[16] = buttonQuantita1.Text;
                valoriTestata[17] = CostoMac1.Text;
                valoriTestata[18] = CostoUomo1.Text;
                valoriTestata[19] = Articoli1.Text;
                valoriTestata[20] = CostoSing1.Text;
                valoriTestata[21] = RicavoSing1.Text;
                valoriTestata[22] = Tot1.Text;
                valoriTestata[23] = TotVar1.Text;
                valoriTestata[24] = buttonQuantita2.Text;
                valoriTestata[25] = CostoMac2.Text;
                valoriTestata[26] = CostoUomo2.Text;
                valoriTestata[27] = Articoli2.Text;
                valoriTestata[28] = CostoSing2.Text;
                valoriTestata[29] = RicavoSing2.Text;
                valoriTestata[30] = Tot2.Text;
                valoriTestata[31] = TotVar2.Text;
                valoriTestata[32] = buttonQuantita3.Text;
                valoriTestata[33] = CostoMac3.Text;
                valoriTestata[34] = CostoUomo3.Text;
                valoriTestata[35] = Articoli3.Text;
                valoriTestata[36] = CostoSing3.Text;
                valoriTestata[37] = RicavoSing3.Text;
                valoriTestata[38] = Tot3.Text;
                valoriTestata[39] = TotVar3.Text;

                ConfermaSalvataggio Conferma = new ConfermaSalvataggio(m, valoriTestata, false);
                Conferma.Show();
                //m.InsertPreventivo(valoriTestata);
            }
            else
            {
                MessageBox.Show("I campi \"Cliente\", \"Articolo\" e \"Note\" devono essere compilati per poter salvare il preventivo!");
            }
            return;
        }

        private void buttonModificaQuantita_Click(object sender, EventArgs e)
        {
            ModificaQuantita mq = new ModificaQuantita(this);
            mq.Show();
        }

        public void CaricaQuantita()
        {
            buttonQuantita1.Text = Setting.Istance.Q1;
            buttonQuantita2.Text = Setting.Istance.Q2;
            buttonQuantita3.Text = Setting.Istance.Q3;
            BindingGrid();
        }
       

        /// <summary>
        /// Richiama la form per poter permettere il caricamento di un preventivo salvato precedentemente nel database.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCarica_Click(object sender, EventArgs e)
        {
            /*
            CaricaPreventivo loadPreventivo = new CaricaPreventivo(this, m);
            loadPreventivo.Show();
            */
            Helper h = new Helper();
            String[] par = { };
            //Setting.Istance.CambiaValoreCliente(cliente);
            //public string StartHelper(string path, string ip, string port, string database, string username, string password, string _campofocus, string _utenteWSAI, string _passwordWSAI, string[] parametri, string valoreFocus, string font, string fontLabel)
            string idpreventivo = h.StartHelper(Setting.Istance.HelpPreventivo, Setting.Istance.Ip, Setting.Istance.Port, Setting.Istance.Database, Setting.Istance.User, Setting.Istance.Password, "", "", "", par, "", Setting.Istance.Font, Setting.Istance.FontLabel);

            if (!String.IsNullOrEmpty(idpreventivo))
            {
                try
                {
                    List<string> testata = m.OttieniTestata(idpreventivo);
                    testata.Add(idpreventivo);
                    labelCliente.Text = testata[0];
                    //ci vorrebbe anche qualcosa per distinguere l'articolo caricato dal preventivo da quello con la distinta base, per 
                    //far sì che la form principale visualizzi i dati corretti 
                    InserisciTestata(testata);
                    m.CaricaPreventivoRighi(idpreventivo, testata[0]);
                    SqlConnection sqlserverConn = new SqlConnection(Setting.Istance.ConnStr);
                    sqlserverConn.Open();
                    string query = "SELECT quantita1, quantita2, quantita3 FROM preventivi WHERE id = " + Int32.Parse(idpreventivo);
                    using (SqlCommand cmd = new SqlCommand(query, sqlserverConn))
                    {
                        SqlDataReader dr = cmd.ExecuteReader();
                        dr.Read();
                        if (dr.HasRows)
                        {
                            XmlDocument doc = new XmlDocument();
                            doc.Load("PreventivazioneRapidaConfig.xml");
                            XmlNode XMLquantita = doc.SelectSingleNode("configuration/Quantita/q1");
                            XMLquantita.InnerText = dr[0].ToString();
                            Setting.Istance.Q1 = dr[0].ToString();
                            XMLquantita = doc.SelectSingleNode("configuration/Quantita/q2");
                            XMLquantita.InnerText = dr[1].ToString();
                            Setting.Istance.Q2 = dr[1].ToString();
                            XMLquantita = doc.SelectSingleNode("configuration/Quantita/q3");
                            XMLquantita.InnerText = dr[2].ToString();
                            Setting.Istance.Q3 = dr[2].ToString();
                            doc.Save("PreventivazioneRapidaConfig.xml");
                            CaricaQuantita();
                        }

                    }

                    BindingGrid();
                }
                catch
                {
                    MessageBox.Show("Errore! Verificare di aver inserito un ID di preventivo corretto per il cliente selezionato.");
                }
            }


            return;
        }

        /// <summary>
        /// Inserisce nell'intestazione i valori che gli sono stati passati come parametro.
        /// </summary>
        /// <param name="testata"></param>
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
            labelCliente.Text = testata[7];
            labelArticolo.Text = testata[8];
            labelIDpreventivo.Text = testata[9];
            groupBoxQI.Text = "Quantità impostata: " + testata[2];
        }

        /// <summary>
        /// Funzione principale che riassume il processo di aggiornamento e di calcolo dei costi totali e dei ricavi.
        /// Inoltre procede alla colorazione delle celle della datagrid.
        /// </summary>
        public void BindingGrid()
        {
            try
            {
                dataGridView.Visible = false;
                progressBar1.Visible = true;
                progressBar1.Minimum = 1;
                progressBar1.Maximum = 5;
                progressBar1.Value = 1;
                progressBar1.Step = 1;
                dataGridView.DataSource = null;
                progressBar1.PerformStep();
                BindingSource bindingSource1 = new BindingSource();
                bindingSource1.DataSource = m.ds.Tables["DistintaBase"];
                dataGridView.DataSource = bindingSource1.DataSource;
                dataGridView.Sort(dataGridView.Columns["Rigo"], ListSortDirection.Ascending);
                progressBar1.PerformStep();
                foreach (DataGridViewColumn column in dataGridView.Columns)
                {
                    column.SortMode = DataGridViewColumnSortMode.NotSortable;
                    column.ReadOnly = true;
                }
                progressBar1.PerformStep();
                progressBar1.PerformStep();

                CalcolaPrezzoQuantitaImpostata();
                progressBar1.PerformStep();
                CalcolaPrezzoTotaliPerQuantita();
                progressBar1.PerformStep();
                ColoraDataGrid();
                progressBar1.PerformStep();

                progressBar1.PerformStep();
                progressBar1.Visible = false;
                dataGridView.Visible = true;
            }
            catch(Exception e) { Console.Write(e); }           
        }

        private void buttonEspandi_Click(object sender, EventArgs e)
        {
            int a = this.Height;
            int b = dataGridView.Location.Y;
            int c = dataGridView.Height;
            if(dataGridView.Height > altezzaDataGrid)
            {
                dataGridView.Height = altezzaDataGrid;
            }
            else
            {
                dataGridView.Height = (a - b) - 50;
            }
        }

        /// <summary>
        /// Funzione richiamata dalla form secondaria "InserisciRigo".
        /// In base alla tipologia di inserimento del rigo (passata come primo parametro", l'algoritmo si comporta in maniera differente per poter permettere di inserire il rigo
        /// nella posizione corretta e quindi rieseguire tutte le operazioni di aggiornamento per il totale dei costi e deti ricavi del preventivo.
        /// </summary>
        /// <param name="tipologiaInserimento"></param>
        /// <param name="padre"></param>
        /// <param name="datarow"></param>
        public void InserisciRigo(int tipologiaInserimento, string padre, DataRow datarow)
        {
            //Se la datatable principale non esiste allora procedo a crearne una nuova e a collegarla con la datagrid
            if (m.ds.Tables.IndexOf("DistintaBase") < 1)
            {
                DataTable distintaBase = new DataTable();
                distintaBase = datarow.Table.Clone();
                distintaBase.TableName = "DistintaBase";
                m.ds.Tables.Add(distintaBase);
                BindingSource bindingSource1 = new BindingSource();
                bindingSource1.DataSource = m.ds.Tables["DistintaBase"];
                dataGridView.DataSource = bindingSource1;
            }
            //All'interno di ogni case viene eseguito un primo controllo che verifica se la riga padre scelta nella form "InserisciRigo" coincide con il padre principale selezionato
            //dall'utente, quindi quello inserito nella testata.
            switch (tipologiaInserimento)
            {
                //1--> inserimento di una materia prima o semilavorato preso dal database delle distinte base di agilis
                case 1:
                    {
                        if(textBoxArticolo.Text == padre)
                        {
                            IEnumerable<int> query = from row in m.ds.Tables["DistintaBase"].AsEnumerable() where row.Field<String>("Codice Padre") == textBoxArticolo.Text select Int32.Parse(row["Rigo"].ToString());
                            
                            int risultato = 0;
                            List<int> qqqq = query.ToList();
                            if (qqqq.Count > 0)
                            {
                                risultato = query.Max();
                            }
                            datarow["Rigo"] = (risultato + 1).ToString();
                            datarow["Codice Padre"] = padre;
                            m.ds.Tables["DistintaBase"].ImportRow(datarow);
                            EsplodiDistintaBase(datarow["Rigo"].ToString(), 1);
                        }
                        else
                        {
                            IEnumerable<DataRow> query = from row in m.ds.Tables["DistintaBase"].AsEnumerable() where row.Field<String>("CODICE ART") == padre select row;
                            DataRow datarowpadre = query.First();
                            string rowindexpadre = datarowpadre["Rigo"].ToString();
                            int numerodifigli = 0;
                            //Controllo se il rigo da inserire abbia a sua volta dei figli, quindi inserisco anche quelli
                            foreach(DataRow r in m.ds.Tables["DistintaBase"].Rows)
                            {
                                if(r["Codice Padre"].ToString() == datarowpadre["Codice Art"].ToString())
                                {
                                    try
                                    {
                                        string prova = r["Rigo"].ToString().Substring(rowindexpadre.Length+1);
                                        int rowindexfigli = Int32.Parse(r["Rigo"].ToString().Substring(rowindexpadre.Length+1));
                                        if(rowindexfigli > numerodifigli)
                                        {
                                            numerodifigli = rowindexfigli;
                                        }
                                    }
                                    catch { }                                    
                                }
                            }
                            datarow["Rigo"] = rowindexpadre + "," + (numerodifigli+1).ToString();
                            datarow["Codice Padre"] = padre;
                            m.ds.Tables["DistintaBase"].ImportRow(datarow);
                            int livello = FindNumberOfChar(',', datarow["Rigo"].ToString()) + 1;
                            EsplodiDistintaBase(datarow["Rigo"].ToString(), livello);
                            //Eseguo i calcoli di aggiornamento
                            AggiornaParentela(datarow);
                        }
                        break;
                    }
                //Inserimento di una lavorazione presa dal database di agilis
                case 2:
                    {
                        if (textBoxArticolo.Text == padre)
                        {
                            IEnumerable<int> query = from row in m.ds.Tables["DistintaBase"].AsEnumerable() where row.Field<String>("Codice Padre") == textBoxArticolo.Text select Int32.Parse(row["Rigo"].ToString());
                            int risultato = 0;
                            List<int> qqqq = query.ToList();
                            if (qqqq.Count > 0)
                            {
                                risultato = query.Max();
                            }
                            datarow["Rigo"] = (risultato + 1).ToString();
                            datarow["Codice Padre"] = padre;
                            m.ds.Tables["DistintaBase"].ImportRow(datarow);
                            EsplodiDistintaBase(datarow["Rigo"].ToString(), 1);
                        }
                        else
                        {
                            IEnumerable<DataRow> query = from row in m.ds.Tables["DistintaBase"].AsEnumerable() where row.Field<String>("CODICE ART") == padre select row;
                            DataRow datarowpadre = query.First();
                            string rowindexpadre = datarowpadre["Rigo"].ToString();
                            int numerodifigli = 0;
                            foreach (DataRow r in m.ds.Tables["DistintaBase"].Rows)
                            {
                                if (r["Codice Padre"].ToString() == datarowpadre["Codice Art"].ToString())
                                {
                                    try
                                    {
                                        string prova = r["Rigo"].ToString().Substring(rowindexpadre.Length + 1);
                                        int rowindexfigli = Int32.Parse(r["Rigo"].ToString().Substring(rowindexpadre.Length + 1));
                                        if (rowindexfigli > numerodifigli)
                                        {
                                            numerodifigli = rowindexfigli;
                                        }
                                    }
                                    catch { }
                                }
                            }
                            datarow["Rigo"] = rowindexpadre + "," + (numerodifigli + 1).ToString();
                            datarow["Codice Padre"] = padre;
                            m.ds.Tables["DistintaBase"].ImportRow(datarow);

                            int livello = FindNumberOfChar(',', datarow["Rigo"].ToString()) + 1;
                            EsplodiDistintaBase(datarow["Rigo"].ToString(), livello);

                            AggiornaParentela(datarow);
                        }
                        break;
                    }
                //3--> Inserimento di un preventivo creato in precedenza tramite questo software, lo inserisco come rigo del preventivo attualmente in elaborazione
                case 3:
                    {
                        if (textBoxArticolo.Text == padre)
                        {
                            IEnumerable<int> query = from row in m.ds.Tables["DistintaBase"].AsEnumerable() where row.Field<String>("Codice Padre") == textBoxArticolo.Text select Int32.Parse(row["Rigo"].ToString());
                            int risultato = 0;
                            List<int> qqqq = query.ToList();
                            if (qqqq.Count > 0)
                            {
                                risultato = query.Max();
                            }
                            DataTable datatable = new DataTable();
                            SqlDataAdapter da;
                            SqlConnection sqlserverConn = new SqlConnection(Setting.Istance.ConnStr);
                            sqlserverConn.Open();
                            string selectrighipreventivo = "SELECT rowindex,codicepadre as CODICE_PADRE,codiceart AS 'Codice Art', codicecentro AS 'Codice Centro', codicelav AS 'Codice Lav', descrizione AS 'Descrizione art / Centro di Lavoro', um1 AS 'UM 1'," +
                                "quantita1 AS 'Quantita` 1', um2 AS 'UM 2', quantita2 AS 'Qta 2', um3 AS 'UM 3', quantita3 AS 'Qta 3', setupmac AS 'Setup Mac', setupuomo AS 'Setup Uomo', tempomac AS 'Tempo Mac', tempouomo AS 'Tempo Uomo', costoart AS 'Costo Art', " +
                                "costoattmac AS 'Costo Att Mac', costoattuomo AS 'Costo Att Uomo'," +
                                "costomac AS 'Costo Mac', costouomo AS 'Costo Uomo', totale AS 'Totale', totalevar AS 'Totale + %Var', setupmacdec AS 'setup mac decimale', setupuomodec AS 'setup uomo decimale'," +
                                " tempomacdec AS 'tempo mac decimale', tempouomodec AS 'tempo uomo decimale'  FROM preventivirighi WHERE idpreventivo = "+ Int32.Parse(datarow["id"].ToString());
                            da = new SqlDataAdapter(selectrighipreventivo, sqlserverConn);
                            da.Fill(datatable);
                            DataRow rigotestata = datatable.NewRow();
                            if(m.ds.Tables["DistintaBase"].Columns.Count < 20)
                            {
                                m.ds.Tables.Remove("DistintaBase");
                                DataTable distintaBase = new DataTable();
                                distintaBase = rigotestata.Table.Clone();
                                distintaBase.TableName = "DistintaBase";
                                m.ds.Tables.Add(distintaBase);
                                BindingSource bindingSource1 = new BindingSource();
                                bindingSource1.DataSource = m.ds.Tables["DistintaBase"];
                                dataGridView.DataSource = bindingSource1;
                            }
                            rigotestata["Rigo"] = (risultato + 1).ToString();
                            rigotestata["Codice Padre"] = padre;
                            rigotestata["Codice lav"] = "";
                            rigotestata["Codice centro"] = "";
                            rigotestata["UM 1"] = datarow["um1"];
                            rigotestata["Quantita` 1"] = datarow["quantita1"];
                            rigotestata["UM 2"] = datarow["um2"];
                            rigotestata["Qta 2"] = datarow["quantita2"];
                            rigotestata["UM 3"] = datarow["um3"];
                            rigotestata["Qta 3"] = datarow["quantita3"];
                            rigotestata["Setup Mac"] = "";
                            rigotestata["Setup Uomo"] = "";
                            rigotestata["Tempo Mac"] = "";
                            rigotestata["Tempo Uomo"] = "";
                            rigotestata["Costo Art"] = Double.Parse(datarow["totale"].ToString()) / Double.Parse(datarow["quantita"].ToString());
                            rigotestata["Costo Att Mac"] = "";
                            rigotestata["Costo Att Uomo"] = "";
                            rigotestata["Costo Mac"] = "";
                            rigotestata["Costo Uomo"] = "";
                            rigotestata["Totale"] = datarow["totale"];
                            rigotestata["Codice Art"] = datarow["articolo"];
                            rigotestata["Descrizione art / Centro di Lavoro"] = datarow["note"];
                            rigotestata["Totale + %Var"] = Double.Parse(rigotestata["Totale"].ToString()) + (Double.Parse(rigotestata["Totale"].ToString()) * Double.Parse(textBoxVariazione.Text) / 100);
                            rigotestata["setup mac decimale"] = "";
                            rigotestata["setup uomo decimale"] = "";
                            rigotestata["tempo mac decimale"] = "";
                            rigotestata["tempo uomo decimale"] = "";
                            DataRow r = m.ds.Tables["DistintaBase"].NewRow();
                            for(int i = 0; i < m.ds.Tables["DistintaBase"].Columns.Count; i++)
                            {
                                r[i] = rigotestata[i];
                            }
                            m.ds.Tables["DistintaBase"].Rows.Add(r);
                            foreach(DataRow dr in datatable.Rows)
                            {
                                dr["Rigo"] = rigotestata["Rigo"].ToString() + "," + dr["Rigo"].ToString();
                                m.ds.Tables["DistintaBase"].ImportRow(dr);
                            }
                        }
                        else
                        {
                            IEnumerable<DataRow> query = from row in m.ds.Tables["DistintaBase"].AsEnumerable() where row.Field<String>("CODICE ART") == padre select row;
                            DataRow datarowpadre = query.First();
                            string rowindexpadre = datarowpadre["Rigo"].ToString();
                            int numerodifigli = 0;
                            foreach (DataRow r in m.ds.Tables["DistintaBase"].Rows)
                            {
                                if (r["Codice Padre"].ToString() == datarowpadre["Codice Art"].ToString())
                                {
                                    try
                                    {
                                        string prova = r["Rigo"].ToString().Substring(rowindexpadre.Length + 1);
                                        int rowindexfigli = Int32.Parse(r["Rigo"].ToString().Substring(rowindexpadre.Length + 1));
                                        if (rowindexfigli > numerodifigli)
                                        {
                                            numerodifigli = rowindexfigli;
                                        }
                                    }
                                    catch { }
                                }
                            }
                            DataTable datatable = new DataTable();
                            SqlDataAdapter da;
                            SqlConnection sqlserverConn = new SqlConnection(Setting.Istance.ConnStr);
                            sqlserverConn.Open();
                            string selectrighipreventivo = "SELECT rowindex,codicepadre as CODICE_PADRE,codiceart AS 'Codice Art', codicecentro AS 'Codice Centro', codicelav AS 'Codice Lav', descrizione AS 'Descrizione art / Centro di Lavoro', um1 AS 'UM 1'" +
                                "quantita1 AS 'Quantita` 1', um2 AS 'UM 2', quantita2 AS 'Qta 2', um3 AS 'UM 3', quantita3 AS 'Qta 3', setupmac AS 'Setup Mac', setupuomo AS 'Setup Uomo', tempomac AS 'Tempo Mac', tempouomo AS 'Tempo Uomo', costoart AS 'Costo Art', " +
                                "costoattmac AS 'Costo Att Mac', costoattuomo AS 'Costo Att Uomo'," +
                                "costomac AS 'Costo Mac', costouomo AS 'Costo Uomo', totale AS 'Totale', totalevar AS 'Totale + %Var', setupmacdec AS 'setup mac decimale', setupuomodec AS 'setup uomo decimale'," +
                                " tempomacdec AS 'tempo mac decimale', tempouomodec AS 'tempo uomo decimale'  FROM preventivirighi WHERE idpreventivo = (SELECT id FROM(SELECT (ROW_NUMBER() OVER(ORDER BY id)) as rowindex, id FROM preventivi" +
                                " WHERE cliente = 'RGG') AS clientepreventivi WHERE rowindex = " + Int32.Parse(datarow["Rigo"].ToString()) + ")";
                            da = new SqlDataAdapter(selectrighipreventivo, sqlserverConn);
                            da.Fill(datatable);
                            DataRow rigotestata = m.ds.Tables["DistintaBase"].NewRow();
                            rigotestata["Rigo"] = rowindexpadre + "," + (numerodifigli + 1).ToString();
                            rigotestata["Codice Padre"] = padre;
                            rigotestata["UM 1"] = datarow["um1"];
                            rigotestata["Quantita` 1"] = datarow["quantita1"];
                            rigotestata["UM 2"] = datarow["um2"];
                            rigotestata["Qta 2"] = datarow["quantita2"];
                            rigotestata["UM 3"] = datarow["um3"];
                            rigotestata["Qta 3"] = datarow["quantita3"];
                            rigotestata["Totale"] = datarow["totale"];
                            rigotestata["Codice Art"] = datarow["articolo"];
                            rigotestata["Descrizione art / Centro di Lavoro"] = datarow["note"];
                            rigotestata["Totale + %Var"] = Double.Parse(rigotestata["Totale"].ToString()) + (Double.Parse(rigotestata["Totale"].ToString()) * Double.Parse(textBoxVariazione.Text) / 100);
                            m.ds.Tables["DistintaBase"].Rows.Add(rigotestata);
                            foreach (DataRow dr in datatable.Rows)
                            {
                                dr["Rigo"] = rigotestata["Rigo"].ToString() + "," + dr["Rigo"].ToString();
                                m.ds.Tables["DistintaBase"].ImportRow(dr);
                            }

                            AggiornaParentela(rigotestata);
                        }
                        break;
                    }
                //4--> Inserimento
                case 4:
                    {
                        if (textBoxArticolo.Text == padre)
                        {
                            IEnumerable<int> query = from row in m.ds.Tables["DistintaBase"].AsEnumerable() where row.Field<String>("Codice Padre") == textBoxArticolo.Text select Int32.Parse(row["Rigo"].ToString());
                            int risultato = 0;
                            List<int> qqqq = query.ToList();
                            if (qqqq.Count > 0)
                            {
                                risultato = query.Max();
                            }
                            datarow["Rigo"] = (risultato + 1).ToString();
                            datarow["Codice Padre"] = padre;
                            datarow["Totale"] = Double.Parse(datarow["Costo Art"].ToString()) * Double.Parse(datarow["Quantita` 1"].ToString());
                            datarow["Totale + %Var"] = Double.Parse(datarow["Totale"].ToString()) + (Double.Parse(datarow["Totale"].ToString()) * Double.Parse(textBoxVariazione.Text) / 100);
                            DataRow rigodainserire = m.ds.Tables["DistintaBase"].NewRow();

                            for(int i =0; i <m.ds.Tables["DistintaBase"].Columns.Count; i++)
                            {
                                rigodainserire[i] = datarow[i];
                            }
                            m.ds.Tables["DistintaBase"].Rows.Add(rigodainserire);                           
                        }
                        else
                        {
                            IEnumerable<DataRow> query = from row in m.ds.Tables["DistintaBase"].AsEnumerable() where row.Field<String>("CODICE ART") == padre select row;
                            DataRow datarowpadre = query.First();
                            string rowindexpadre = datarowpadre["Rigo"].ToString();
                            int numerodifigli = 0;
                            foreach (DataRow r in m.ds.Tables["DistintaBase"].Rows)
                            {
                                if (r["Codice Padre"].ToString() == datarowpadre["Codice Art"].ToString())
                                {
                                    try
                                    {
                                        string prova = r["Rigo"].ToString().Substring(rowindexpadre.Length + 1);
                                        int rowindexfigli = Int32.Parse(r["Rigo"].ToString().Substring(rowindexpadre.Length + 1));
                                        if (rowindexfigli > numerodifigli)
                                        {
                                            numerodifigli = rowindexfigli;
                                        }
                                    }
                                    catch { }
                                }
                            }
                            datarow["Rigo"] = rowindexpadre + "," + (numerodifigli + 1).ToString();
                            datarow["Codice Padre"] = padre;
                            datarow["Totale"] = Double.Parse(datarow["Costo Art"].ToString()) * Double.Parse(datarow["Quantita` 1"].ToString());
                            datarow["Totale + %Var"] = Double.Parse(datarow["Totale"].ToString()) + (Double.Parse(datarow["Totale"].ToString()) * Double.Parse(textBoxVariazione.Text) / 100);
                            DataRow rigodainserire = m.ds.Tables["DistintaBase"].NewRow();
                            for (int i = 0; i < m.ds.Tables["DistintaBase"].Columns.Count; i++)
                            {
                                rigodainserire[i] = datarow[i];
                            }
                            m.ds.Tables["DistintaBase"].Rows.Add(rigodainserire);
                            AggiornaParentela(rigodainserire);
                        }
                        break;
                    }
                case 5:
                    {
                        if (textBoxArticolo.Text == padre)
                        {
                            IEnumerable<int> query = from row in m.ds.Tables["DistintaBase"].AsEnumerable() where row.Field<String>("Codice Padre") == textBoxArticolo.Text select Int32.Parse(row["Rigo"].ToString());
                            int risultato = 0;
                            List<int> qqqq = query.ToList();
                            if (qqqq.Count > 0)
                            {
                                risultato = query.Max();
                            }
                            datarow["Rigo"] = (risultato + 1).ToString();
                            datarow["Codice Padre"] = padre;

                            datarow["Totale"] = (Double.Parse(datarow["Setup Mac"].ToString()) * Double.Parse(datarow["Costo Att Mac"].ToString())) + (Double.Parse(datarow["Setup Uomo"].ToString()) * Double.Parse(datarow["Costo Att Uomo"].ToString()))
                                + (Double.Parse(datarow["Tempo Mac"].ToString()) * Double.Parse(datarow["Costo Mac"].ToString()) * Double.Parse(datarow["Quantita` 1"].ToString()))
                                + (Double.Parse(datarow["Tempo Uomo"].ToString()) * Double.Parse(datarow["Costo Uomo"].ToString()) * Double.Parse(datarow["Quantita` 1"].ToString()));
                            datarow["Totale + %Var"] = Double.Parse(datarow["Totale"].ToString()) + (Double.Parse(datarow["Totale"].ToString()) * Double.Parse(textBoxVariazione.Text) / 100);
                            DataRow rigodainserire = m.ds.Tables["DistintaBase"].NewRow();
                            for (int i = 0; i < m.ds.Tables["DistintaBase"].Columns.Count; i++)
                            {
                                rigodainserire[i] = datarow[i];
                            }
                            m.ds.Tables["DistintaBase"].Rows.Add(rigodainserire);
                            m.FromTimeToDecimal(rigodainserire);
                        }
                        else
                        {
                            IEnumerable<DataRow> query = from row in m.ds.Tables["DistintaBase"].AsEnumerable() where row.Field<String>("CODICE ART") == padre select row;
                            DataRow datarowpadre = query.First();
                            string rowindexpadre = datarowpadre["Rigo"].ToString();
                            int numerodifigli = 0;
                            foreach (DataRow r in m.ds.Tables["DistintaBase"].Rows)
                            {
                                if (r["Codice Padre"].ToString() == datarowpadre["Codice Art"].ToString())
                                {
                                    try
                                    {
                                        string prova = r["Rigo"].ToString().Substring(rowindexpadre.Length + 1);
                                        int rowindexfigli = Int32.Parse(r["Rigo"].ToString().Substring(rowindexpadre.Length + 1));
                                        if (rowindexfigli > numerodifigli)
                                        {
                                            numerodifigli = rowindexfigli;
                                        }
                                    }
                                    catch { }
                                }
                            }
                            datarow["Rigo"] = rowindexpadre + "," + (numerodifigli + 1).ToString();
                            datarow["Codice Padre"] = padre;
                            datarow["Totale"] = (Double.Parse(datarow["Setup mac"].ToString()) * Double.Parse(datarow["Costo att mac"].ToString())) + (Double.Parse(datarow["Setup uomo"].ToString()) * Double.Parse(datarow["Costo att uomo"].ToString()))
                                + (Double.Parse(datarow["Tempo mac"].ToString()) * Double.Parse(datarow["Costo mac"].ToString()) * Double.Parse(datarow["Quantita` 1"].ToString()))
                                + (Double.Parse(datarow["Tempo uomo"].ToString()) * Double.Parse(datarow["Costo uomo"].ToString()) * Double.Parse(datarow["Quantita` 1"].ToString()));
                            datarow["Totale + %Var"] = Double.Parse(datarow["Totale"].ToString()) + (Double.Parse(datarow["Totale"].ToString()) * Double.Parse(textBoxVariazione.Text) / 100);
                            DataRow rigodainserire = m.ds.Tables["DistintaBase"].NewRow();
                            for (int i = 0; i < m.ds.Tables["DistintaBase"].Columns.Count; i++)
                            {
                                rigodainserire[i] = datarow[i];
                            }
                            m.ds.Tables["DistintaBase"].Rows.Add(rigodainserire);
                            AggiornaParentela(rigodainserire);
                            m.FromTimeToDecimal(rigodainserire);
                        }
                        break;
                    }
                case 6:
                    {
                        if (textBoxArticolo.Text == padre)
                        {
                            IEnumerable<int> query = from row in m.ds.Tables["DistintaBase"].AsEnumerable() where row.Field<String>("Codice Padre") == textBoxArticolo.Text select Int32.Parse(row["Rigo"].ToString());
                            int risultato = 0;
                            List<int> qqqq = query.ToList();
                            if (qqqq.Count > 0)
                            {
                                risultato = query.Max();
                            }
                            datarow["Rigo"] = (risultato + 1).ToString();
                            datarow["Codice Padre"] = padre;
                            datarow["Totale"] = Double.Parse(datarow["Costo Art"].ToString()) * Double.Parse(datarow["Quantita` 1"].ToString());
                            datarow["Totale + %Var"] = Double.Parse(datarow["Totale"].ToString()) + (Double.Parse(datarow["Totale"].ToString()) * Double.Parse(textBoxVariazioneLav.Text) / 100);
                            DataRow rigodainserire = m.ds.Tables["DistintaBase"].NewRow();
                            for (int i = 0; i < m.ds.Tables["DistintaBase"].Columns.Count; i++)
                            {
                                rigodainserire[i] = datarow[i];
                            }
                            m.ds.Tables["DistintaBase"].Rows.Add(rigodainserire);
                        }
                        else
                        {
                            IEnumerable<DataRow> query = from row in m.ds.Tables["DistintaBase"].AsEnumerable() where row.Field<String>("CODICE ART") == padre select row;
                            DataRow datarowpadre = query.First();
                            string rowindexpadre = datarowpadre["Rigo"].ToString();
                            int numerodifigli = 0;
                            foreach (DataRow r in m.ds.Tables["DistintaBase"].Rows)
                            {
                                if (r["Codice Padre"].ToString() == datarowpadre["Codice Art"].ToString())
                                {
                                    try
                                    {
                                        string prova = r["Rigo"].ToString().Substring(rowindexpadre.Length + 1);
                                        int rowindexfigli = Int32.Parse(r["Rigo"].ToString().Substring(rowindexpadre.Length + 1));
                                        if (rowindexfigli > numerodifigli)
                                        {
                                            numerodifigli = rowindexfigli;
                                        }
                                    }
                                    catch { }
                                }
                            }
                            datarow["Rigo"] = rowindexpadre + "," + (numerodifigli + 1).ToString();
                            datarow["Codice Padre"] = padre;
                            datarow["Totale"] = Double.Parse(datarow["Costo Art"].ToString()) * Double.Parse(datarow["Quantita` 1"].ToString());
                            datarow["Totale + %Var"] = Double.Parse(datarow["Totale"].ToString()) + (Double.Parse(datarow["Totale"].ToString()) * Double.Parse(textBoxVariazioneLav.Text) / 100);
                            DataRow rigodainserire = m.ds.Tables["DistintaBase"].NewRow();
                            for (int i = 0; i < m.ds.Tables["DistintaBase"].Columns.Count; i++)
                            {
                                rigodainserire[i] = datarow[i];
                            }
                            m.ds.Tables["DistintaBase"].Rows.Add(rigodainserire);
                            AggiornaParentela(rigodainserire);
                        }
                        break;
                    }
                default:
                    break;
                    
            }
            if (dataGridView.Rows.Count > 1)
            {
                textBoxQuantita.Enabled = true;
                textBoxVariazioneLav.Enabled = true;
                textBoxVariazione.Enabled = true;
            }
            //Aggiornamento della datagrid
            BindingGrid();
            dataGridView.Sort(dataGridView.Columns["Rigo"], ListSortDirection.Ascending);
        }


        private void buttonStampa_Click(object sender, EventArgs e)
        {
            if (textBoxCliente.Text != "" && textBoxArticolo.Text != "" && textBoxNote.Text != "")
            {
                try
                {
                    string fileSTAMPA = "StampaPreventivoRGG.xml";

                    File.Create(fileSTAMPA).Close();
                    string[] valoriTestata = new string[40];
                    valoriTestata[0] = textBoxCliente.Text;
                    valoriTestata[1] = textBoxArticolo.Text;
                    valoriTestata[2] = textBoxQuantita.Text;
                    valoriTestata[3] = textBoxVariazione.Text;
                    valoriTestata[4] = textBoxVariazioneLav.Text;
                    valoriTestata[5] = QItotale.Text;
                    valoriTestata[6] = QItotalevar.Text;
                    valoriTestata[7] = textBoxNote.Text;

                    valoriTestata[8] = QIarticoli.Text;
                    valoriTestata[9] = QIcostomac.Text;
                    valoriTestata[10] = QIcostouomo.Text;
                    valoriTestata[11] = QICostoSingolo.Text;
                    valoriTestata[12] = QIRicavoSingolo.Text;

                    valoriTestata[13] = labelCliente.Text;
                    valoriTestata[14] = labelArticolo.Text;

                    valoriTestata[15] = user;

                    valoriTestata[16] = buttonQuantita1.Text;
                    valoriTestata[17] = CostoMac1.Text;
                    valoriTestata[18] = CostoUomo1.Text;
                    valoriTestata[19] = Articoli1.Text;
                    valoriTestata[20] = CostoSing1.Text;
                    valoriTestata[21] = RicavoSing1.Text;
                    valoriTestata[22] = Tot1.Text;
                    valoriTestata[23] = TotVar1.Text;
                    valoriTestata[24] = buttonQuantita2.Text;
                    valoriTestata[25] = CostoMac2.Text;
                    valoriTestata[26] = CostoUomo2.Text;
                    valoriTestata[27] = Articoli2.Text;
                    valoriTestata[28] = CostoSing2.Text;
                    valoriTestata[29] = RicavoSing2.Text;
                    valoriTestata[30] = Tot2.Text;
                    valoriTestata[31] = TotVar2.Text;
                    valoriTestata[32] = buttonQuantita3.Text;
                    valoriTestata[33] = CostoMac3.Text;
                    valoriTestata[34] = CostoUomo3.Text;
                    valoriTestata[35] = Articoli3.Text;
                    valoriTestata[36] = CostoSing3.Text;
                    valoriTestata[37] = RicavoSing3.Text;
                    valoriTestata[38] = Tot3.Text;
                    valoriTestata[39] = TotVar3.Text;
                    string message = "\"Sì\" per salvare e stampare.\n\"No\" per stampare senza salvare.";
                    string caption = "Salvare il preventivo?";
                    MessageBoxButtons buttons = MessageBoxButtons.YesNo;
                    DialogResult result;
                    result = MessageBox.Show(this, message, caption, buttons,
                        MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
                    if (result == DialogResult.Yes)
                    {
                        ConfermaSalvataggio salva = new ConfermaSalvataggio(m, valoriTestata, true);
                        salva.Show();
                    }
                    else
                    {
                        m.ScriviXMLperStampa(fileSTAMPA, valoriTestata);
                    }
                    
                    //m.ScriviXMLperStampa(fileSTAMPA, valoriTestata);
                }
                catch(Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
            else
            {
                MessageBox.Show("I campi \"Cliente\", \"Articolo\" e \"Note\" devono essere compilati per poter stampare il preventivo!");
            }
            return;           
        }
    }
}
