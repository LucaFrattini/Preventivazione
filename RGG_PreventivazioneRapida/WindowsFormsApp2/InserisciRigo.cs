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
        DataRow drAgilis;

        //Funzione costruttore
        public InserisciRigo(Form1 form1, Model model)
        {
            f = form1;
            m = model;
            InitializeComponent();
            PopolaComboBox();
            this.textBoxArticolo.Leave += new System.EventHandler(this.textBoxArticolo_Leave);
            this.textBoxCentro.Leave += new System.EventHandler(this.textBoxCentro_Leave);
            SetFont(Setting.Istance.Font);
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
                    comboBox.Text = f.Articolo;
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
                        query = (from row in m.ds.Tables["DistintaBase"].AsEnumerable() /*where row["Codice Art"].ToString() != "" && row["Codice centro"].ToString() == ""*/ select row["Codice Padre"].ToString()).Distinct().ToList();
                    }
                    catch { }                   
                }
                int count = query.Count();
                foreach (String padre in query)
                {
                    comboBox.Items.Add(padre);
                }
                if (count > 0)
                {
                    comboBox.Text = comboBox.Items[0].ToString();
                }
            }
            catch
            {
                MessageBox.Show("Errore nel popolamento della combobox.");
            }
        }

        private void SetFont(String font)
        {

            font = font.Replace('#', ' ');

            String fontFamil = font.Split('-')[0];
            float fontSize;
            float.TryParse(font.Split('-')[1], out fontSize);

            Font f = new Font(fontFamil, fontSize);
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
                foreach (Control c in groupBox1.Controls)
                {
                    c.Font = f;
                }
                foreach (Control c in groupBox2.Controls)
                {
                    c.Font = f;
                }
                foreach (Control c in groupBoxImporta.Controls)
                {
                    c.Font = f;
                }
                foreach (Control c in groupBoxNuovo.Controls)
                {
                    c.Font = f;
                }
                //this.dataGridView.Font = f;
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
                    if(t.Text != "")
                    {
                        if(t.Name == "textBoxQuantita")
                        {
                            if(drAgilis["ar_conver"].ToString() != "" && drAgilis["ar_conver"].ToString() != "0")
                            {
                                textBoxQuantita2.Text = (Double.Parse(t.Text) * Double.Parse(drAgilis["ar_conver"].ToString())).ToString();
                            }
                            if (drAgilis["ar_qtacon2"].ToString() != "" && drAgilis["ar_qtacon2"].ToString() != "0")
                            {
                                textBoxQuantita3.Text = (Double.Parse(t.Text) / Double.Parse(drAgilis["ar_qtacon2"].ToString())).ToString();
                            }
                        }
                        else if(t.Name == "textBoxQuantita2")
                        {
                            if (drAgilis["ar_conver"].ToString() != "" && drAgilis["ar_conver"].ToString() != "0")
                            {
                                textBoxQuantita.Text = (Double.Parse(t.Text) / Double.Parse(drAgilis["ar_conver"].ToString())).ToString();
                            }
                            if (drAgilis["ar_qtacon2"].ToString() != "" && drAgilis["ar_qtacon2"].ToString() != "0")
                            {
                                textBoxQuantita3.Text = (Double.Parse(textBoxQuantita.Text) / Double.Parse(drAgilis["ar_qtacon2"].ToString())).ToString();
                            }
                        }
                        else if(t.Name == "textBoxQuantita3")
                        {
                            if (drAgilis["ar_qtacon2"].ToString() != "" && drAgilis["ar_qtacon2"].ToString() != "0")
                            {
                                textBoxQuantita.Text = (Double.Parse(t.Text) * Double.Parse(drAgilis["ar_qtacon2"].ToString())).ToString();
                            }
                            if (drAgilis["ar_conver"].ToString() != "" && drAgilis["ar_conver"].ToString() != "0")
                            {
                                textBoxQuantita2.Text = (Double.Parse(textBoxQuantita.Text) * Double.Parse(drAgilis["ar_conver"].ToString())).ToString();
                            }
                        }
                        Double prova = double.Parse(t.Text);
                    }
                    if (radioButtonAgilis.Checked == true && radioButtonArticolo.Checked == true)
                    {                       
                        if(textBoxQuantita.Text != "" && textBoxUM.Text != "" && textBoxArticolo.BackColor == Color.LightGreen)
                        {
                            buttonConferma.Enabled = true;
                        }
                        else
                        {
                            buttonConferma.Enabled = false;
                        }
                    }
                    else if (radioButtonAgilis.Checked == true && (radioButtonLavorazione.Checked == true || radioButtonLavorazioneEsterna.Checked == true))
                    {                      
                        if (textBoxQuantita.Text != "" && textBoxUM.Text != "" && textBoxArticolo.BackColor == Color.LightGreen && textBoxCentro.BackColor == Color.LightGreen)
                        {
                            buttonConferma.Enabled = true;
                        }
                        else
                        {
                            buttonConferma.Enabled = false;
                        }
                    }
                    else if (radioButtonPreventivi.Checked == true)
                    {
                        if(textBoxArticolo.BackColor == Color.LightGreen)
                        {
                            buttonConferma.Enabled = true;
                        }
                        else
                        {
                            buttonConferma.Enabled = false;
                        }
                    }
                    else if (radioButtonNuovo.Checked == true && radioButtonArticolo.Checked == true)
                    {
                        if(textBoxNome.Text != "" && textBoxQuantitaNuovo.Text != "" && textBoxCostoArticolo.Text != "")
                        {
                            buttonConferma.Enabled = true;
                        }
                        else
                        {
                            buttonConferma.Enabled = false;
                        }
                    }
                    else if (radioButtonNuovo.Checked == true && radioButtonLavorazione.Checked == true)
                    {
                        if(textBoxNome.Text != "" && textBoxQuantitaNuovo.Text != "" && textBoxSetupMac.Text != "" && textBoxSetupUomo.Text != "" && textBoxTempoMac.Text !=""
                            && textBoxTempoUomo.Text != "" && textBoxCostoSetupMac.Text != "" && textBoxCostoSetupUomo.Text != "" && textBoxCostoTempoMac.Text != "" &&
                            textBoxCostoTempoUomo.Text != "" && textBoxCostoArticolo.Text != "")
                        {
                            buttonConferma.Enabled = true;
                        }
                        else
                        {
                            buttonConferma.Enabled = false;
                        }
                    }
                    else if (radioButtonNuovo.Checked == true && radioButtonLavorazioneEsterna.Checked == true)
                    {
                        if(textBoxNome.Text != "" && textBoxQuantita.Text != "" && textBoxCostoLavEst.Text != "" && textBoxCostoArticolo.Text != "")
                        {
                            buttonConferma.Enabled = true;
                        }
                        else
                        {
                            buttonConferma.Enabled = false;
                        }
                    }
                }
            }
            catch
            {
                MessageBox.Show("Errore nella compilazione del campo. Inserire solo valori numerici.");
                buttonConferma.Enabled = false;
            }
        }

        /// <summary>
        /// Evento checkchanged. Ad ogni selezione diversa la form farà visualizzare dettagli specifici.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void radioButton_CheckedChanged(object sender, EventArgs e)
        {
            textBoxQuantita.Text = "1";
            textBoxQuantita2.Text = "";
            textBoxQuantita3.Text = "";
            textBoxUM.Text = "";
            textBoxUM2.Text = "";
            textBoxUM3.Text = "";
            textBoxQuantita2Nuovo.Text = "";
            textBoxQuantita3Nuovo.Text = "";
            textBoxUM2Nuovo.Text = "";
            textBoxUM3Nuovo.Text = "";
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
            labelUM2.Visible = false;
            labelUM3.Visible = false;
            labelQuantita2.Visible = false;
            labelQuantita3.Visible = false;
            textBoxQuantita.Visible = true;
            textBoxQuantita2.Visible = false;
            textBoxQuantita3.Visible = false;
            textBoxUM.Visible = true;
            textBoxUM2.Visible = false;
            textBoxUM3.Visible = false;
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
                labelUM2.Visible = true;
                labelUM3.Visible = true;
                labelQuantita2.Visible = true;
                labelQuantita3.Visible = true;
                textBoxQuantita.Enabled = true;
                textBoxQuantita2.Visible = true;
                textBoxQuantita3.Visible = true;
                textBoxUM2.Visible = true;
                textBoxUM3.Visible = true;
                textBoxUM.Enabled = true;
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
                textBoxUM.Enabled = false;
            }
            else if(radioButtonNuovo.Checked == true && radioButtonArticolo.Checked == true)
            {
                this.Height = this.groupBoxNuovo.Location.Y + this.groupBoxNuovo.Size.Height + 100;
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
                textBoxQuantita2Nuovo.Enabled = true;
                textBoxQuantita3Nuovo.Enabled = true;
                textBoxUM2Nuovo.Enabled = true;
                textBoxUM3Nuovo.Enabled = true;
                labelPrezzo.Text = "Prezzo:";
            }
            else if(radioButtonNuovo.Checked == true && radioButtonLavorazione.Checked == true)
            {
                this.Height = this.groupBoxNuovo.Location.Y + this.groupBoxNuovo.Size.Height + 100; 
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
                textBoxQuantita2Nuovo.Enabled = false;
                textBoxQuantita3Nuovo.Enabled = false;
                textBoxUM2Nuovo.Enabled = false;
                textBoxUM3Nuovo.Enabled = false;
                labelPrezzo.Text = "Centro:";
            }
            else if (radioButtonNuovo.Checked == true && radioButtonLavorazioneEsterna.Checked == true)
            {
                this.Height = this.groupBoxNuovo.Location.Y + this.groupBoxNuovo.Size.Height + 100; 
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
                textBoxQuantita2Nuovo.Enabled = false;
                textBoxQuantita3Nuovo.Enabled = false;
                textBoxUM2Nuovo.Enabled = false;
                textBoxUM3Nuovo.Enabled = false;
                labelCostoLavEst.Visible = true;
                labelPrezzo.Text = "Centro:";
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
                drAgilis = m.ds.Tables["Articoli"].Rows.Find(textBoxArticolo.Text);
                if (drAgilis != null)
                {
                    textBoxArticolo.BackColor = Color.LightGreen;
                    labelImportaDescrizione.Text = drAgilis[1].ToString();
                    buttonConferma.Enabled = true;
                    textBoxUM.Text = drAgilis["ar_unmis"].ToString();                    
                    if (drAgilis["ar_conver"].ToString() == "" || drAgilis["ar_conver"].ToString() == "0" || drAgilis["ar_unmis2"].ToString() == "")
                    {
                        drAgilis["ar_conver"] = 0;
                        textBoxUM2.Enabled = false;
                        textBoxQuantita2.Enabled = false;
                    }
                    else
                    {
                        textBoxUM2.Text = drAgilis["ar_unmis2"].ToString();
                        textBoxQuantita2.Text = (Double.Parse(textBoxQuantita.Text) * Double.Parse(drAgilis["ar_conver"].ToString())).ToString();
                        textBoxUM2.Enabled = true;
                        textBoxQuantita2.Enabled = true;
                    }
                    if (drAgilis["ar_qtacon2"].ToString() == "" || drAgilis["ar_qtacon2"].ToString() == "0" || drAgilis["ar_confez2"].ToString() == "")
                    {
                        drAgilis["ar_qtacon2"] = 0;
                        textBoxUM3.Enabled = false;
                        textBoxQuantita3.Enabled = false;
                    }
                    else
                    {
                        textBoxUM3.Text = drAgilis["ar_confez2"].ToString();
                        textBoxQuantita3.Text = (Double.Parse(textBoxQuantita.Text) / Double.Parse(drAgilis["ar_qtacon2"].ToString())).ToString();
                        textBoxUM3.Enabled = true;
                        textBoxQuantita3.Enabled = true;
                    }
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
                        datarow["UM 1"] = textBoxUM.Text;
                        datarow["Quantita` 1"] = Double.Parse(textBoxQuantita.Text);
                        datarow["UM 2"] = textBoxUM2.Text;
                        datarow["Qta 2"] = textBoxQuantita2.Text;
                        datarow["UM 3"] = textBoxUM3.Text;
                        datarow["Qta 3"] = (textBoxQuantita3.Text == "" ? "" : textBoxQuantita3.Text);
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
                        datarow["UM 1"] = textBoxUM.Text;
                        datarow["Quantita` 1"] = Double.Parse(textBoxQuantita.Text); 
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
                        datatable.Columns.Add("Rigo");
                        datatable.Columns.Add("Codice Padre");
                        datatable.Columns.Add("Codice Art");
                        datatable.Columns.Add("Codice Centro");
                        datatable.Columns.Add("Codice Lav");
                        datatable.Columns.Add("Descrizione art / Centro di Lavoro");
                        datatable.Columns.Add("UM 1");
                        datatable.Columns.Add("Quantita` 1");
                        datatable.Columns.Add("UM 2");
                        datatable.Columns.Add("Qta 2");
                        datatable.Columns.Add("UM 3");
                        datatable.Columns.Add("Qta 3");
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
                        datarow["Rigo"] = "";
                        datarow["Codice Padre"] = "";
                        datarow["Codice Art"] = textBoxNome.Text;
                        datarow["Descrizione art / Centro di Lavoro"] = textBoxDescrizione.Text;
                        datarow["Costo Art"] = Double.Parse(textBoxCostoArticolo.Text);
                        datarow["UM 1"] = textBoxUMNuovo.Text;
                        datarow["Quantita` 1"] = Double.Parse(textBoxQuantitaNuovo.Text);
                        datarow["UM 2"] = textBoxUM2Nuovo.Text;
                        datarow["Qta 2"] = textBoxQuantita2Nuovo.Text;
                        datarow["UM 3"] = textBoxUM3Nuovo.Text;
                        datarow["Qta 3"] = textBoxQuantita3Nuovo.Text;
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
                        datatable.Columns.Add("Rigo");
                        datatable.Columns.Add("Codice Padre");
                        datatable.Columns.Add("Codice Art");
                        datatable.Columns.Add("Codice Centro");
                        datatable.Columns.Add("Codice Lav");
                        datatable.Columns.Add("Descrizione art / Centro di Lavoro");
                        datatable.Columns.Add("UM 1");
                        datatable.Columns.Add("Quantita` 1");
                        datatable.Columns.Add("UM 2");
                        datatable.Columns.Add("Qta 2");
                        datatable.Columns.Add("UM 3");
                        datatable.Columns.Add("Qta 3");
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
                        datarow["Rigo"] = "";
                        datarow["Codice Padre"] = "";
                        datarow["Codice Art"] = "";
                        datarow["Codice Lav"] = textBoxNome.Text;
                        datarow["Codice Centro"] = Double.Parse(textBoxCostoArticolo.Text);
                        datarow["Descrizione art / Centro di Lavoro"] = textBoxDescrizione.Text;
                        datarow["Costo Art"] = "";
                        datarow["UM 1"] = textBoxUMNuovo.Text;
                        datarow["Quantita` 1"] = Double.Parse(textBoxQuantitaNuovo.Text);
                        datarow["UM 2"] = "";
                        datarow["Qta 2"] = "";
                        datarow["UM 3"] = "";
                        datarow["Qta 3"] = "";
                        datarow["Setup Mac"] = Double.Parse(textBoxSetupMac.Text);
                        datarow["Setup Uomo"] = Double.Parse(textBoxSetupUomo.Text);
                        datarow["Tempo Mac"] = Double.Parse(textBoxTempoMac.Text);
                        datarow["Tempo Uomo"] = Double.Parse(textBoxTempoUomo.Text);
                        datarow["Costo Att Mac"] = Double.Parse(textBoxCostoSetupMac.Text);
                        datarow["Costo Att Uomo"] = Double.Parse(textBoxCostoSetupUomo.Text);
                        datarow["Costo Mac"] = Double.Parse(textBoxCostoTempoMac.Text);
                        datarow["Costo Uomo"] = Double.Parse(textBoxCostoTempoUomo.Text);
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
                        datatable.Columns.Add("Rigo");
                        datatable.Columns.Add("Codice Padre");
                        datatable.Columns.Add("Codice Art");
                        datatable.Columns.Add("Codice Centro");
                        datatable.Columns.Add("Codice Lav");
                        datatable.Columns.Add("Descrizione art / Centro di Lavoro");
                        datatable.Columns.Add("UM 1");
                        datatable.Columns.Add("Quantita` 1");
                        datatable.Columns.Add("UM 2");
                        datatable.Columns.Add("Qta 2");
                        datatable.Columns.Add("UM 3");
                        datatable.Columns.Add("Qta 3");
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
                        datarow["Rigo"] = "";
                        datarow["Codice Padre"] = "";
                        datarow["Codice Lav"] = textBoxNome.Text;
                        datarow["Codice Art"] = textBoxDescrizione.Text;
                        datarow["Descrizione art / Centro di Lavoro"] = textBoxDescrizione.Text;
                        datarow["Codice Centro"] = textBoxCostoArticolo.Text;
                        datarow["UM 1"] = textBoxUMNuovo.Text;
                        datarow["Quantita` 1"] = textBoxQuantitaNuovo.Text;
                        datarow["UM 2"] = "";
                        datarow["Qta 2"] = "";
                        datarow["UM 3"] = "";
                        datarow["Qta 3"] = "";
                        datarow["Costo Art"] = Double.Parse(textBoxCostoLavEst.Text);
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
                MessageBox.Show("Errore durante l'inserimento del rigo.\nControllare di aver inserito correttamente i campi richiesti!\n\n\n" + ex);
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
