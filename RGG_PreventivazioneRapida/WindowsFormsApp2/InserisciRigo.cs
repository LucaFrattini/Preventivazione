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

        public InserisciRigo(Form1 form1, Model model)
        {
            f = form1;
            m = model;
            InitializeComponent();
            PopolaComboBox();
            this.textBoxArticolo.Leave += new System.EventHandler(this.textBoxArticolo_Leave);
            this.textBoxCentro.Leave += new System.EventHandler(this.textBoxCentro_Leave);
        }

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
                        query = (from row in m.ds.Tables["DistintaBase"].AsEnumerable() where row["Codice Art"].ToString() != "" && row["Codice centro"].ToString() == "" select row["CODICE_PADRE"].ToString()).Distinct().ToList();
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

        private void radioButton_CheckedChanged(object sender, EventArgs e)
        {
            textBoxArticolo.Text = "";
            textBoxArticolo.BackColor = Color.White;
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
            }
        }

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
                    labelImportaDescrizione.Text = "<Descrizione Articolo>";
                    MessageBox.Show("Inserire un codice articolo corretto.");
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
                        m.EstraiRisultatoQuery(Setting.Istance.QueryLavorazione.Replace("&lt;", "&gt;="), "Lavorazioni");
                    }
                    DataRow dr = m.ds.Tables["Lavorazioni"].Rows.Find(Int32.Parse(textBoxArticolo.Text));
                    if (dr != null)
                    {
                        textBoxArticolo.BackColor = Color.LightGreen;
                        labelImportaDescrizione.Text = dr[1].ToString();
                        buttonConferma.Enabled = true;
                    }
                    else
                    {
                        textBoxArticolo.BackColor = Color.White;
                        labelImportaDescrizione.Text = "<Descrizione Lavorazione>";
                        MessageBox.Show("Inserire un codice di lavorazione corretto.");
                        buttonConferma.Enabled = false;
                    }
                }
                catch
                {
                    MessageBox.Show("Inserire un codice di lavorazione corretto.");
                    textBoxArticolo.BackColor = Color.OrangeRed;
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
                            MessageBox.Show("Inserire un id preventivo corretto.");
                            textBoxArticolo.BackColor = Color.OrangeRed;
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

        private void textBoxCentro_Leave(object sender, EventArgs e)
        {
            string query = Setting.Istance.QueryCercaCentro.Replace("@CodCent", textBoxArticolo.Text);

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
                    buttonConferma.Enabled = true;
                }
                else
                {
                    textBoxArticolo.BackColor = Color.White;
                    labelCentro.Text = "<Descrizione Centro>";
                    MessageBox.Show("Inserire un codice articolo corretto.");
                    buttonConferma.Enabled = false;
                }
            }
            connection.Close();
        }

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
                        datatable.Columns.Add("Lavorazione");
                        datatable.Columns.Add("Quantita");
                        datatable.Columns.Add("LavDesc");
                        datatable.Columns.Add("Centro");
                        datatable.Columns.Add("CenDesc");
                        datatable.Columns.Add("CostoAttMac");
                        datatable.Columns.Add("CostoAttUomo");
                        datatable.Columns.Add("CostoMac");
                        datatable.Columns.Add("CostoUomo");
                        datarow = datatable.NewRow();
                        datarow["Lavorazione"] = textBoxArticolo.Text;
                        datarow["Quantita"] = textBoxQuantita.Text;
                        datarow["Centro"] = textBoxCentro.Text;
                        query = Setting.Istance.QueryCercaPreventivo.Replace("@CodLav", textBoxArticolo.Text);
                        connection = new SqlConnection(Setting.Istance.ConnStr);
                        connection.Open();
                        using (SqlCommand cmd = new SqlCommand(query, connection))
                        {
                            SqlDataReader dr = cmd.ExecuteReader();
                            if (dr.HasRows)
                            {
                                dr.Read();
                                datarow["LavDesc"] = dr["tb_deslavo"] ;

                            }
                            dr.Close();
                        }
                        query = Setting.Istance.QueryCercaCentro.Replace("@CodCent", textBoxCentro.Text);
                        using (SqlCommand cmd = new SqlCommand(query, connection))
                        {
                            SqlDataReader dr = cmd.ExecuteReader();
                            if (dr.HasRows)
                            {
                                dr.Read();
                                datarow["CenDesc"] = dr["tb_descent"];
                                datarow["CostoAttMac"] = dr["tb_cmacoratt"];
                                datarow["CostoAttUomo"] = dr["tb_pagaoratt"];
                                datarow["CostoMac"] = dr["tb_cmacora"];
                                datarow["CostoUomo"] = dr["tb_pagaora"];
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
                        datatable.Columns.Add("Nome");
                        datatable.Columns.Add("Descrizione");
                        datatable.Columns.Add("Prezzo");
                        datatable.Columns.Add("Quantita");
                        datarow = datatable.NewRow();
                        datarow["Nome"] = textBoxNome.Text;
                        datarow["Descrizione"] = textBoxDescrizione.Text;
                        datarow["Prezzo"] = textBoxCostoArticolo.Text;
                        datarow["Quantita"] = textBoxQuantitaNuovo.Text;
                        f.InserisciRigo(tipologiaInserimento, comboBox.Text, datarow);
                        datatable.Reset();
                    }
                    else if (radioButtonNuovo.Checked == true && radioButtonLavorazione.Checked == true)
                    {
                        tipologiaInserimento = 5;
                        datatable.Columns.Add("Nome");
                        datatable.Columns.Add("Centro");
                        datatable.Columns.Add("Descrizione");
                        datatable.Columns.Add("Prezzo");
                        datatable.Columns.Add("Quantita");
                        datatable.Columns.Add("Setup mac");
                        datatable.Columns.Add("Setup uomo");
                        datatable.Columns.Add("Tempo mac");
                        datatable.Columns.Add("Tempo uomo");
                        datatable.Columns.Add("Costo setup mac");
                        datatable.Columns.Add("Costo setup uomo");
                        datatable.Columns.Add("Costo tempo mac");
                        datatable.Columns.Add("Costo tempo uomo");
                        datarow = datatable.NewRow();
                        datarow["Nome"] = textBoxNome.Text;
                        datarow["Centro"] = textBoxCostoArticolo.Text;
                        datarow["Descrizione"] = textBoxDescrizione.Text;
                        datarow["Prezzo"] = textBoxCostoArticolo.Text;
                        datarow["Quantita"] = textBoxQuantitaNuovo.Text;
                        datarow["Setup mac"] = textBoxSetupMac.Text;
                        datarow["Setup uomo"] = textBoxSetupUomo.Text;
                        datarow["Tempo mac"] = textBoxTempoMac.Text;
                        datarow["Tempo uomo"] = textBoxTempoUomo.Text;
                        datarow["Costo setup mac"] = textBoxCostoSetupMac.Text;
                        datarow["Costo setup uomo"] = textBoxCostoSetupUomo.Text;
                        datarow["Costo tempo mac"] = textBoxCostoTempoMac.Text;
                        datarow["Costo tempo uomo"] = textBoxCostoTempoUomo.Text;


                        f.InserisciRigo(tipologiaInserimento, comboBox.Text, datarow);
                        datatable.Reset();
                    }
                    else if(radioButtonNuovo.Checked == true && radioButtonLavorazioneEsterna.Checked == true)
                    {
                        tipologiaInserimento = 6;
                        datatable.Columns.Add("Nome");
                        datatable.Columns.Add("Descrizione");
                        datatable.Columns.Add("Centro");
                        datatable.Columns.Add("Quantita");
                        datatable.Columns.Add("Costo");
                        datarow = datatable.NewRow();
                        datarow["Nome"] = textBoxNome.Text;
                        datarow["Descrizione"] = textBoxDescrizione.Text;
                        datarow["Centro"] = textBoxCostoArticolo.Text;
                        datarow["Quantita"] = textBoxQuantitaNuovo.Text;
                        datarow["Costo"] = textBoxCostoLavEst.Text;
                        f.InserisciRigo(tipologiaInserimento, comboBox.Text, datarow);
                        datatable.Reset();
                    }
                }               
            }
            catch(Exception ex)
            {
                MessageBox.Show("Errore durante l'inserimento del rigo.\n" + ex);
            }
        }

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
                temp = h.StartHelper(Setting.Istance.HelpPreventivo, Setting.Istance.Ip, Setting.Istance.Port, Setting.Istance.Database, Setting.Istance.User, Setting.Istance.Password, "", "", "", par, "", Setting.Istance.Font, Setting.Istance.FontLabel);
            }

            if (!String.IsNullOrEmpty(temp))
            {
                this.textBoxArticolo.Text = temp;
                textBoxArticolo.Focus();
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
            }
        }
    }
}
