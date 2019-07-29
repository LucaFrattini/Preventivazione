using HelperLibrary;
using PreventivazioneRapida;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp2
{
    public partial class InserisciRigo : Form
    {
        Form1 f;
        Model m;

        //Funzione costruttore
        public InserisciRigo(Form1 form1, Model model)
        {
            f = form1;
            m = model;
            InitializeComponent();
            PopolaComboBox();
            this.textBoxArticolo.Leave += new System.EventHandler(this.textBoxArticolo_Leave);
            this.textBoxCentro.Leave += new System.EventHandler(this.textBoxCentro_Leave);
        }

        /// <summary>
        /// Funzione per popolare la combobox con tutti i padri.
        /// Nel caso si vada a modificare una distinta base già presente in agilis i padri potranno essere solo i semilavorati e il padre principale.
        /// Se invece si sta facendo un preventivo da zero nei padri sono compresi tutti gli articoli
        /// </summary>
        private void PopolaComboBox()
        {
            try
            {
                List<String> query = new List<string>();
                if (f.creaArticolo)
                {
                    comboBox.Items.Add(f.Articolo);
                    try
                    {
                        query = (from row in m.ds.Tables["DistintaBase"].AsEnumerable() where row["Codice Art"].ToString() != "" && row["Codice centro"].ToString() == "" select row["Codice Art"].ToString()).Distinct().ToList();
                    }
                    catch { }
                }
                else
                {
                    try
                    {
                        query = (from row in m.ds.Tables["DistintaBase"].AsEnumerable() /*where row["Codice Art"].ToString() != "" && row["Codice centro"].ToString() == ""*/ select row["CODICE_PADRE"].ToString()).Distinct().ToList();
                    }
                    catch { }
                }
                int count = query.Count();
                foreach (String padre in query)
                {
                    comboBox.Items.Add(padre);
                }
            }
            catch
            {
                MessageBox.Show("Errore nel popolamento della combobox.");
            }
        }

        /// <summary>
        /// Funzione che serve solo per la convalida degli input nelle textbox e trasforma il '.' in ','
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ControllaValiditaTextBox(object sender, EventArgs e)
        {
            try
            {
                if (sender is TextBox)
                {
                    TextBox t = (TextBox)sender;
                    t.Text = t.Text.Replace('.', ',');
                    Double prova = double.Parse(t.Text);
                }
            }
            catch
            {
                MessageBox.Show("Errore nella compilazione del campo. Inserire solo valori numerici.");
            }
        }

        /// <summary>
        /// Evento checkchanged. Ad ogni selezione diversa la form farà visualizzare dettagli specifici.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void radioButton_CheckedChanged(object sender, EventArgs e)
        {
            textBoxArticolo.Text = "";
            textBoxCentro.Text = "";
            textBoxArticolo.BackColor = Color.White;
            textBoxCentro.BackColor = Color.White;
            textBoxNome.Text = "";
            textBoxDescrizione.Text = "";
            textBoxCostoArticolo.Text = "";
            textBoxCostoArticolo.Enabled = true;
            textBoxSetupMac.Text = "";
            textBoxSetupUomo.Text = "";
            textBoxTempoMac.Text = "";
            textBoxTempoUomo.Text = "";
            textBoxCostoSetupMac.Text = "";
            textBoxCostoSetupUomo.Text = "";
            textBoxCostoTempoMac.Text = "";
            textBoxCostoTempoUomo.Text = "";
            labelCostoLavEst.Visible = false;
            textBoxCostoLavEst.Visible = false;
            labelCentro.Visible = false;
            textBoxCentro.Visible = false;
            buttonHelpCentri.Visible = false;
            buttonConferma.Enabled = false;
            if (radioButtonAgilis.Checked == true && radioButtonArticolo.Checked == true)
            {
                this.Height = 530;
                groupBox2.Enabled = true;
                groupBoxImporta.Visible = true;
                groupBoxNuovo.Visible = false;
                groupBoxImporta.Text = "Dichiara articolo da importare";
                labelImportaDescrizione.Text = "<Descrizione articolo>";
                buttonHelp.Text = "Help articoli";
                textBoxQuantita.Enabled = true;
            }
            else if(radioButtonAgilis.Checked == true && (radioButtonLavorazione.Checked == true || radioButtonLavorazioneEsterna.Checked == true))
            {
                this.Height = 530;
                groupBox2.Enabled = true;
                groupBoxImporta.Visible = true;
                groupBoxNuovo.Visible = false;
                groupBoxImporta.Text = "Dichiara lavorazione da importare";
                labelImportaDescrizione.Text = "<Descrizione lavorazione>";
                buttonHelp.Text = "Help lavorazione";
                textBoxQuantita.Enabled = true;
                labelCentro.Visible = true;
                textBoxCentro.Visible = true;
                buttonHelpCentri.Visible = true;
            }
            else if(radioButtonPreventivi.Checked == true)
            {
                this.Height = 530;
                groupBox2.Enabled = false;
                groupBoxImporta.Visible = true;
                groupBoxNuovo.Visible = false;
                groupBoxImporta.Text = "Dichiara preventivo da importare";
                labelImportaDescrizione.Text = "<Note preventivo>";
                buttonHelp.Text = "Help preventivi";
                textBoxQuantita.Text = "1";
                textBoxQuantita.Enabled = false;
            }
            else if(radioButtonNuovo.Checked == true && radioButtonArticolo.Checked == true)
            {
                this.Height = 680;
                groupBox2.Enabled = true;
                groupBoxImporta.Visible = false;
                groupBoxNuovo.Visible = true;
                textBoxCostoArticolo.Enabled = true;
                textBoxSetupMac.Enabled = false;
                textBoxSetupUomo.Enabled = false;
                textBoxTempoMac.Enabled = false;
                textBoxTempoUomo.Enabled = false;
                textBoxCostoSetupMac.Enabled = false;
                textBoxCostoSetupUomo.Enabled = false;
                textBoxCostoTempoMac.Enabled = false;
                textBoxCostoTempoUomo.Enabled = false;
                labelPrezzo.Text = "Prezzo:";
                buttonConferma.Enabled = true;
            }
            else if(radioButtonNuovo.Checked == true && radioButtonLavorazione.Checked == true)
            {
                this.Height = 680;
                groupBox2.Enabled = true;
                groupBoxImporta.Visible = false;
                groupBoxNuovo.Visible = true;
                //textBoxCostoArticolo.Enabled = false;
                textBoxSetupMac.Enabled = true;
                textBoxSetupUomo.Enabled = true;
                textBoxTempoMac.Enabled = true;
                textBoxTempoUomo.Enabled = true;
                textBoxCostoSetupMac.Enabled = true;
                textBoxCostoSetupUomo.Enabled = true;
                textBoxCostoTempoMac.Enabled = true;
                textBoxCostoTempoUomo.Enabled = true;
                labelPrezzo.Text = "Centro:";
                buttonConferma.Enabled = true;
            }
            else if (radioButtonNuovo.Checked == true && radioButtonLavorazioneEsterna.Checked == true)
            {
                this.Height = 680;
                groupBox2.Enabled = true;
                groupBoxImporta.Visible = false;
                groupBoxNuovo.Visible = true;
                //textBoxCostoArticolo.Enabled = false;
                textBoxSetupMac.Enabled = false;
                textBoxSetupUomo.Enabled = false;
                textBoxTempoMac.Enabled = false;
                textBoxTempoUomo.Enabled = false;
                textBoxCostoSetupMac.Enabled = false;
                textBoxCostoSetupUomo.Enabled = false;
                textBoxCostoTempoMac.Enabled = false;
                textBoxCostoTempoUomo.Enabled = false;
                textBoxCostoLavEst.Visible = true;
                labelCostoLavEst.Visible = true;
                labelPrezzo.Text = "Centro:";
                buttonConferma.Enabled = true;
            }
        }

        /// <summary>
        /// Evento Leave sulla textbox dell'articolo.
        /// In base al radiobutton selezionato il comportamento dell'evento sarà differente.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBoxArticolo_Leave(object sender, EventArgs e)
        {
            if (radioButtonAgilis.Checked == true && radioButtonArticolo.Checked == true)
            {
                //string query = Setting.Istance.QueryCercaArticolo.Replace("@CodArticolo", textBoxArticolo.Text);
                DataRow dr = m.ds.Tables["Articoli"].Rows.Find(textBoxArticolo.Text);
                if (dr != null)
                {
                    textBoxArticolo.BackColor = Color.LightGreen;
                    labelImportaDescrizione.Text = dr[1].ToString();
                    buttonConferma.Enabled = true;
                }
                else
                {
                    textBoxArticolo.BackColor = Color.White;
                    labelImportaDescrizione.Text = "<Descrizione articolo>";
                    if(textBoxArticolo.Text != "") {
                        MessageBox.Show("Inserire un codice articolo corretto.");
                        textBoxArticolo.BackColor = Color.OrangeRed;
                    }                  
                    buttonConferma.Enabled = false;
                }
            }
            else if (radioButtonAgilis.Checked == true && (radioButtonLavorazione.Checked == true || radioButtonLavorazioneEsterna.Checked == true))
            {
                try
                {
                    if(radioButtonLavorazione.Checked == true)
                    {
                        m.EstraiRisultatoQuery(Setting.Istance.QueryLavorazione, "Lavorazioni");
                    }
                    else
                    {
                        m.EstraiRisultatoQuery(Setting.Istance.QueryLavorazione.Replace("<", ">="), "Lavorazioni");
                    }
                    DataRow dr = m.ds.Tables["Lavorazioni"].Rows.Find(Int32.Parse(textBoxArticolo.Text));
                    if (dr != null)
                    {
                        textBoxArticolo.BackColor = Color.LightGreen;
                        labelImportaDescrizione.Text = dr[1].ToString();
                        if(textBoxCentro.BackColor == Color.LightGreen)
                        {
                            buttonConferma.Enabled = true;
                        }
                    }
                    else
                    {
                        textBoxArticolo.BackColor = Color.White;
                        labelImportaDescrizione.Text = "<Descrizione Lavorazione>";
                        if (textBoxArticolo.Text != "")
                        { MessageBox.Show("Inserire un codice di lavorazione corretto."); }
                        buttonConferma.Enabled = false;
                    }
                }
                catch
                {
                    textBoxArticolo.BackColor = Color.White;
                    if (textBoxArticolo.Text != "")
                    {
                        MessageBox.Show("Inserire un codice di lavorazione corretto.");
                        textBoxArticolo.BackColor = Color.OrangeRed;
                    }                    
                    buttonConferma.Enabled = false;
                }

            }
            else if (radioButtonPreventivi.Checked == true)
            {
                try
                {
                    string query = Setting.Istance.QueryCercaPreventivo.Replace("@idpreventivo", textBoxArticolo.Text);
                    SqlConnection connection = new SqlConnection(Setting.Istance.ConnStr);
                    connection.Open();
                    using (SqlCommand cmd = new SqlCommand(query, connection))
                    {
                        SqlDataReader dr = cmd.ExecuteReader();
                        if (dr.HasRows)
                        {
                            dr.Read();
                            textBoxArticolo.BackColor = Color.LightGreen;
                            labelImportaDescrizione.Text = dr["note"].ToString();
                            buttonConferma.Enabled = true;
                        }
                        else
                        {
                            textBoxArticolo.BackColor = Color.White;
                            if (textBoxArticolo.Text != "")
                            {
                                MessageBox.Show("Inserire un id preventivo corretto.");
                                textBoxArticolo.BackColor = Color.OrangeRed;
                            }
                            buttonConferma.Enabled = false;
                        }
                    }
                    connection.Close();
                }
                catch(Exception ex)
                {
                    MessageBox.Show("Errore di connessione.\n" + ex.Message);
                }             
            }
        }

        /// <summary>
        /// Evento leave della textbox del centro di lavorazione.
        /// Controlla che sia una lavorazione interna o esterna.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBoxCentro_Leave(object sender, EventArgs e)
        {
            string query = Setting.Istance.QueryCercaCentro.Replace("@CodCent", textBoxCentro.Text);

            if (radioButtonLavorazione.Checked == true)
            {
                //string query = Setting.Istance.QueryCercaArticolo.Replace("@CodArticolo", textBoxArticolo.Text);
                query = query + " and tb_codcent < 500";
                
            }
            else
            {
                query = query + " and tb_codcent >= 500";
            }
            SqlConnection connection = new SqlConnection(Setting.Istance.ConnStr);

            connection.Open();
            using (SqlCommand cmd = new SqlCommand(query, connection))
            {
                SqlDataReader dr = cmd.ExecuteReader();
                if (dr.HasRows)
                {
                    dr.Read();
                    textBoxCentro.BackColor = Color.LightGreen;
                    labelCentro.Text = dr["tb_descent"].ToString();
                    if(textBoxArticolo.BackColor == Color.LightGreen)
                    {
                        buttonConferma.Enabled = true;
                    }
                }
                else
                {
                    labelCentro.Text = "<Descrizione centro>";
                    textBoxCentro.BackColor = Color.White;
                    if (textBoxCentro.Text != "")
                    {
                        textBoxCentro.BackColor = Color.OrangeRed;
                        MessageBox.Show("Inserire un codice centro corretto.");
                    }
                    buttonConferma.Enabled = false;
                }
            }
            connection.Close();
        }

        /// <summary>
        /// Evento Click del button di conferma. In base al radiobutton selezionato si comporta in maniera diversa.
        /// Così richiama la funzione "inseriscirigo" della form principale e aggiunge i corretti paramentri da passare alla funzione
        /// in base al rigo da gestire (lavorazione o articolo)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonConferma_Click(object sender, EventArgs e)
        {
            try
            {
                string query;
                DataRow datarow;
                int tipologiaInserimento;
                DataTable datatable = new DataTable();
                SqlConnection connection = new SqlConnection(Setting.Istance.ConnStr);
                if (comboBox.Text == "")
                {
                    MessageBox.Show("Indicare l'articolo padre.");
                }
                else
                {
                    if (radioButtonAgilis.Checked == true && radioButtonArticolo.Checked == true)
                    {
                        tipologiaInserimento = 1;
                        query = Setting.Istance.QueryCercaArticolo.Replace("@CodArticolo", textBoxArticolo.Text);
                        SqlDataAdapter da = new SqlDataAdapter(query, connection);
                        da.Fill(datatable);
                        datarow = datatable.Rows[0];
                        datarow["Quantita`"] = Double.Parse(textBoxQuantita.Text);
                        f.InserisciRigo(tipologiaInserimento, comboBox.Text, datarow);
                        datatable.Reset();
                        //InserisciRiga(tipologiaInserimento, textBoxArticolo.Text, textBoxQuantita.Text);
                    }
                    else if (radioButtonAgilis.Checked == true && (radioButtonLavorazione.Checked == true || radioButtonLavorazioneEsterna.Checked == true))
                    {
                        tipologiaInserimento = 2;
                        query = Setting.Istance.QueryCercaLavorazione.Replace("@CodLav", textBoxArticolo.Text);
                        if (radioButtonLavorazioneEsterna.Checked == true)
                        {
                            query = query + " and tb_codlavo >= 500";
                        }
                        SqlDataAdapter da = new SqlDataAdapter(query, connection);
                        da.Fill(datatable);
                        datarow = datatable.Rows[0];
                        datarow["Quantita`"] = textBoxQuantita.Text;
                        connection = new SqlConnection(Setting.Istance.ConnStr);
                        connection.Open();
                        query = Setting.Istance.QueryCercaCentro.Replace("@CodCent", textBoxCentro.Text);
                        if (radioButtonLavorazioneEsterna.Checked == true)
                        {
                            query = query + " and tb_codcent >= 500";
                        }
                        using (SqlCommand cmd = new SqlCommand(query, connection))
                        {
                            SqlDataReader dr = cmd.ExecuteReader();
                            if (dr.HasRows)
                            {
                                dr.Read();

                                datarow["Codice Centro"] = dr["tb_codcent"].ToString();
                                datarow["Descrizione art / Centro di Lavoro"] = datarow["Descrizione art / Centro di Lavoro"].ToString() + dr["tb_descent"].ToString();
                                datarow["Costo Att Mac"] = dr["tb_cmacoratt"].ToString();
                                datarow["Costo Att Uomo"] = dr["tb_pagaoratt"].ToString();
                                datarow["Costo Mac"] = dr["tb_cmacora"].ToString();
                                datarow["Costo Uomo"] = dr["tb_pagaora"].ToString();
                                if (radioButtonLavorazioneEsterna.Checked == true)
                                {
                                    datarow["Codice Art"] = datarow["Descrizione art / Centro di Lavoro"].ToString() + dr["tb_descent"].ToString();
                                    datarow["Costo Att Mac"] = "";
                                    datarow["Costo Att Uomo"] = "";
                                    datarow["Costo Mac"] = "";
                                    datarow["Costo Uomo"] = "";
                                    datarow["Setup Mac"] = "";
                                    datarow["Setup Uomo"] = "";
                                    datarow["Tempo Mac"] = "";
                                    datarow["Tempo Uomo"] = "";
                                    datarow["Costo art"] = "0";
                                }
                            }
                            dr.Close();
                        }
                        connection.Close();
                        f.InserisciRigo(tipologiaInserimento, comboBox.Text, datarow);
                        datatable.Reset();
                    }
                    else if (radioButtonPreventivi.Checked == true)
                    {
                        tipologiaInserimento = 3;
                        query = Setting.Istance.QueryCercaPreventivo.Replace("@idpreventivo", textBoxArticolo.Text);
                        SqlDataAdapter da = new SqlDataAdapter(query, connection);
                        da.Fill(datatable);
                        datarow = datatable.Rows[0];
                        datarow["quantita"] = double.Parse(textBoxQuantita.Text);
                        f.InserisciRigo(tipologiaInserimento, comboBox.Text, datarow);
                        datatable.Reset();
                    }
                    else if (radioButtonNuovo.Checked == true && radioButtonArticolo.Checked == true)
                    {
                        tipologiaInserimento = 4;
                        datatable.Columns.Add("rowindex");
                        datatable.Columns.Add("CODICE_PADRE");
                        datatable.Columns.Add("Codice Art");
                        datatable.Columns.Add("Codice Centro");
                        datatable.Columns.Add("Codice Lav");
                        datatable.Columns.Add("Descrizione art / Centro di Lavoro");
                        datatable.Columns.Add("Quantita`");
                        datatable.Columns.Add("Setup Mac");
                        datatable.Columns.Add("Setup Uomo");
                        datatable.Columns.Add("Tempo Mac");
                        datatable.Columns.Add("Tempo Uomo");
                        datatable.Columns.Add("Costo Art");
                        datatable.Columns.Add("Costo Att Mac");
                        datatable.Columns.Add("Costo Att Uomo");
                        datatable.Columns.Add("Costo Mac");
                        datatable.Columns.Add("Costo Uomo");
                        datatable.Columns.Add("Totale");
                        datatable.Columns.Add("Totale + %Var");
                        datatable.Columns.Add("setup mac decimale");
                        datatable.Columns.Add("setup uomo decimale");
                        datatable.Columns.Add("tempo mac decimale");
                        datatable.Columns.Add("tempo uomo decimale");

                        datarow = datatable.NewRow();
                        datarow["rowindex"] = "";
                        datarow["CODICE_PADRE"] = "";
                        datarow["Codice Art"] = textBoxNome.Text;
                        datarow["Descrizione art / Centro di Lavoro"] = textBoxDescrizione.Text;
                        datarow["Costo Art"] = textBoxCostoArticolo.Text;
                        datarow["Quantita`"] = textBoxQuantitaNuovo.Text;
                        datarow["Codice Centro"] = "";
                        datarow["Codice Lav"] = "";
                        datarow["Setup Mac"] = "";
                        datarow["Setup Uomo"] = "";
                        datarow["Tempo Mac"] = "";
                        datarow["Tempo Uomo"] = "";
                        datarow["Costo Att mac"] = "";
                        datarow["Costo Att Uomo"] = "";
                        datarow["Costo Mac"] = "";
                        datarow["Costo Uomo"] = "";
                        datarow["Totale"] = "";
                        datarow["Totale + %Var"] = "";
                        datarow["setup mac decimale"] = "";
                        datarow["Setup Uomo decimale"] = "";
                        datarow["Tempo mac decimale"] = "";
                        datarow["Tempo Uomo decimale"] = "";

                        f.InserisciRigo(tipologiaInserimento, comboBox.Text, datarow);
                        datatable.Reset();
                    }
                    else if (radioButtonNuovo.Checked == true && radioButtonLavorazione.Checked == true)
                    {
                        tipologiaInserimento = 5;
                        datatable.Columns.Add("rowindex");
                        datatable.Columns.Add("CODICE_PADRE");
                        datatable.Columns.Add("Codice Art");
                        datatable.Columns.Add("Codice Centro");
                        datatable.Columns.Add("Codice Lav");
                        datatable.Columns.Add("Descrizione art / Centro di Lavoro");
                        datatable.Columns.Add("Quantita`");                       
                        datatable.Columns.Add("Setup Mac");
                        datatable.Columns.Add("Setup Uomo");
                        datatable.Columns.Add("Tempo Mac");
                        datatable.Columns.Add("Tempo Uomo");
                        datatable.Columns.Add("Costo Art");
                        datatable.Columns.Add("Costo Att Mac");
                        datatable.Columns.Add("Costo Att Uomo");
                        datatable.Columns.Add("Costo Mac");
                        datatable.Columns.Add("Costo Uomo"); 
                        datatable.Columns.Add("Totale");
                        datatable.Columns.Add("Totale + %Var");
                        datatable.Columns.Add("setup mac decimale");
                        datatable.Columns.Add("setup uomo decimale");
                        datatable.Columns.Add("tempo mac decimale");
                        datatable.Columns.Add("tempo uomo decimale");

                        datarow = datatable.NewRow();
                        datarow["rowindex"] = "";
                        datarow["CODICE_PADRE"] = "";
                        datarow["Codice Art"] = "";
                        datarow["Codice Lav"] = textBoxNome.Text;
                        datarow["Codice Centro"] = textBoxCostoArticolo.Text;
                        datarow["Descrizione art / Centro di Lavoro"] = textBoxDescrizione.Text;
                        datarow["Costo Art"] = "";
                        datarow["Quantita`"] = textBoxQuantitaNuovo.Text;
                        datarow["Setup Mac"] = textBoxSetupMac.Text;
                        datarow["Setup Uomo"] = textBoxSetupUomo.Text;
                        datarow["Tempo Mac"] = textBoxTempoMac.Text;
                        datarow["Tempo Uomo"] = textBoxTempoUomo.Text;
                        datarow["Costo Att Mac"] = textBoxCostoSetupMac.Text;
                        datarow["Costo Att Uomo"] = textBoxCostoSetupUomo.Text;
                        datarow["Costo Mac"] = textBoxCostoTempoMac.Text;
                        datarow["Costo Uomo"] = textBoxCostoTempoUomo.Text;
                        datarow["Totale"] = "";
                        datarow["Totale + %Var"] = "";
                        datarow["setup mac decimale"] = "";
                        datarow["Setup Uomo decimale"] = "";
                        datarow["Tempo mac decimale"] = "";
                        datarow["Tempo Uomo decimale"] = "";


                        f.InserisciRigo(tipologiaInserimento, comboBox.Text, datarow);
                        datatable.Reset();
                    }
                    else if(radioButtonNuovo.Checked == true && radioButtonLavorazioneEsterna.Checked == true)
                    {
                        tipologiaInserimento = 6;
                        datatable.Columns.Add("rowindex");
                        datatable.Columns.Add("CODICE_PADRE");
                        datatable.Columns.Add("Codice Art");
                        datatable.Columns.Add("Codice Centro");
                        datatable.Columns.Add("Codice Lav");
                        datatable.Columns.Add("Descrizione art / Centro di Lavoro");
                        datatable.Columns.Add("Quantita`");                       
                        datatable.Columns.Add("Setup Mac");
                        datatable.Columns.Add("Setup Uomo");
                        datatable.Columns.Add("Tempo Mac");
                        datatable.Columns.Add("Tempo Uomo");
                        datatable.Columns.Add("Costo Art");
                        datatable.Columns.Add("Costo Att Mac");
                        datatable.Columns.Add("Costo Att Uomo");
                        datatable.Columns.Add("Costo Mac");
                        datatable.Columns.Add("Costo Uomo");
                        datatable.Columns.Add("Totale");
                        datatable.Columns.Add("Totale + %Var");
                        datatable.Columns.Add("setup mac decimale");
                        datatable.Columns.Add("setup uomo decimale");
                        datatable.Columns.Add("tempo mac decimale");
                        datatable.Columns.Add("tempo uomo decimale");

                        datarow = datatable.NewRow();
                        datarow["rowindex"] = "";
                        datarow["CODICE_PADRE"] = "";
                        datarow["Codice Lav"] = textBoxNome.Text;
                        datarow["Codice Art"] = textBoxDescrizione.Text;
                        datarow["Descrizione art / Centro di Lavoro"] = textBoxDescrizione.Text;
                        datarow["Codice Centro"] = textBoxCostoArticolo.Text;
                        datarow["Quantita`"] = textBoxQuantitaNuovo.Text;
                        datarow["Costo Art"] = textBoxCostoLavEst.Text;
                        datarow["Setup Mac"] = textBoxSetupMac.Text;
                        datarow["Setup Uomo"] = textBoxSetupUomo.Text;
                        datarow["Tempo Mac"] = textBoxTempoMac.Text;
                        datarow["Tempo Uomo"] = textBoxTempoUomo.Text;
                        datarow["Costo Att Mac"] = textBoxCostoSetupMac.Text;
                        datarow["Costo Att Uomo"] = textBoxCostoSetupUomo.Text;
                        datarow["Costo Mac"] = textBoxCostoTempoMac.Text;
                        datarow["Costo Uomo"] = textBoxCostoTempoUomo.Text;
                        datarow["Totale"] = "";
                        datarow["Totale + %Var"] = "";
                        datarow["setup mac decimale"] = "";
                        datarow["Setup Uomo decimale"] = "";
                        datarow["Tempo mac decimale"] = "";
                        datarow["Tempo Uomo decimale"] = "";
                        f.InserisciRigo(tipologiaInserimento, comboBox.Text, datarow);                  
                        datatable.Reset();
                    }
                    this.Close();
                }              
            }
            catch(Exception ex)
            {
                MessageBox.Show("Errore durante l'inserimento del rigo.\n" + ex);
            }
        }

        /// <summary>
        /// Evento click del button degli help. Richiama l'helpxml.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonHelp_Click(object sender, EventArgs e)
        {
            Helper h = new Helper();
            String[] par = { };
            string temp = "";
            if (radioButtonAgilis.Checked == true && radioButtonArticolo.Checked == true)
            {
                temp = h.StartHelper(Setting.Istance.HelpArticolo, Setting.Istance.Ip, Setting.Istance.Port, Setting.Istance.Database, Setting.Istance.User, Setting.Istance.Password, "", "", "", par, "", Setting.Istance.Font, Setting.Istance.FontLabel);           
            }
            else if (radioButtonAgilis.Checked == true && radioButtonLavorazione.Checked == true)
            {
                temp = h.StartHelper(Setting.Istance.HelpLavorazione, Setting.Istance.Ip, Setting.Istance.Port, Setting.Istance.Database, Setting.Istance.User, Setting.Istance.Password, "", "", "", par, "", Setting.Istance.Font, Setting.Istance.FontLabel);
            }
            else if (radioButtonAgilis.Checked == true && radioButtonLavorazioneEsterna.Checked == true)
            {
                temp = h.StartHelper(Setting.Istance.HelpLavorazioneEsterna, Setting.Istance.Ip, Setting.Istance.Port, Setting.Istance.Database, Setting.Istance.User, Setting.Istance.Password, "", "", "", par, "", Setting.Istance.Font, Setting.Istance.FontLabel);
            }
            else if (radioButtonPreventivi.Checked == true)
            {
                //Setting.Istance.CambiaValoreCliente("RGG");
                temp = h.StartHelper(Setting.Istance.HelpPreventivo, Setting.Istance.Ip, Setting.Istance.Port, Setting.Istance.Database, Setting.Istance.User, Setting.Istance.Password, "", "", "", par, "", Setting.Istance.Font, Setting.Istance.FontLabel);
            }

            if (!String.IsNullOrEmpty(temp))
            {
                this.textBoxArticolo.Text = temp;
                textBoxArticolo.Focus();
                textBoxQuantita.Focus();
            }
        }

        private void buttonHelpCentri_Click(object sender, EventArgs e)
        {
            Helper h = new Helper();
            String[] par = { };
            string temp = "";
            if (radioButtonAgilis.Checked == true && radioButtonLavorazione.Checked == true)
            {
                temp = h.StartHelper(Setting.Istance.HelpCentro, Setting.Istance.Ip, Setting.Istance.Port, Setting.Istance.Database, Setting.Istance.User, Setting.Istance.Password, "", "", "", par, "", Setting.Istance.Font, Setting.Istance.FontLabel);
            }
            else if (radioButtonAgilis.Checked == true && radioButtonLavorazioneEsterna.Checked == true)
            {
                temp = h.StartHelper(Setting.Istance.HelpCentroEsterno, Setting.Istance.Ip, Setting.Istance.Port, Setting.Istance.Database, Setting.Istance.User, Setting.Istance.Password, "", "", "", par, "", Setting.Istance.Font, Setting.Istance.FontLabel);
            }
            if (!String.IsNullOrEmpty(temp))
            {
                this.textBoxCentro.Text = temp;
                textBoxCentro.Focus();
                textBoxQuantita.Focus();
            }
        }

    }
}
