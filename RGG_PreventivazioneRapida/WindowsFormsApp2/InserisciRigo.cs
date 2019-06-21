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
        }

        private void PopolaComboBox()
        {
            try
            {
                List<String> query = new List<string>();
                query = (from row in m.ds.Tables["DistintaBase"].AsEnumerable() where row["Codice Art"].ToString() != "" && row["Codice centro"].ToString() == "" select row["Codice Art"].ToString()).ToList();
                int count = query.Count();
                comboBox.Items.Add(f.Articolo);
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

        private void radioButton_CheckedChanged(object sender, EventArgs e)
        {
            if(radioButtonAgilis.Checked == true && radioButtonArticolo.Checked == true)
            {
                this.Height = 510;
                groupBox2.Enabled = true;
                groupBoxImporta.Visible = true;
                groupBoxNuovo.Visible = false;
                groupBoxImporta.Text = "Dichiara articolo da importare";
                labelImportaDescrizione.Text = "<Descrizione articolo>";
                buttonHelp.Text = "Help articoli";
            }
            else if(radioButtonAgilis.Checked == true && radioButtonLavorazione.Checked == true)
            {
                this.Height = 510;
                groupBox2.Enabled = true;
                groupBoxImporta.Visible = true;
                groupBoxNuovo.Visible = false;
                groupBoxImporta.Text = "Dichiara lavorazione da importare";
                labelImportaDescrizione.Text = "<Descrizione lavorazione>";
                buttonHelp.Text = "Help lavorazione";
            }
            else if(radioButtonPreventivi.Checked == true)
            {
                this.Height = 510;
                groupBox2.Enabled = false;
                groupBoxImporta.Visible = true;
                groupBoxNuovo.Visible = false;
                groupBoxImporta.Text = "Dichiara preventivo da importare";
                labelImportaDescrizione.Text = "<Note preventivo>";
                buttonHelp.Text = "Help preventivi";
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
            }
            else if(radioButtonNuovo.Checked == true && radioButtonLavorazione.Checked == true)
            {
                this.Height = 680;
                groupBox2.Enabled = true;
                groupBoxImporta.Visible = false;
                groupBoxNuovo.Visible = true;
                textBoxCostoArticolo.Enabled = false;
                textBoxSetupMac.Enabled = true;
                textBoxSetupUomo.Enabled = true;
                textBoxTempoMac.Enabled = true;
                textBoxTempoUomo.Enabled = true;
                textBoxCostoSetupMac.Enabled = true;
                textBoxCostoSetupUomo.Enabled = true;
                textBoxCostoTempoMac.Enabled = true;
                textBoxCostoTempoUomo.Enabled = true;
            }
        }

        private void textBoxArticolo_Leave(object sender, EventArgs e)
        {
            if (radioButtonAgilis.Checked == true && radioButtonArticolo.Checked == true)
            {
                string query = Setting.Istance.QueryCercaArticolo.Replace("@CodArticolo", textBoxArticolo.Text);
                DataRow dr = m.ds.Tables["Articoli"].Rows.Find(textBoxArticolo.Text);
                if (dr != null)
                {
                    textBoxArticolo.BackColor = Color.LightGreen;
                    labelImportaDescrizione.Text = dr[1].ToString();
                    buttonConferma.Enabled = true;
                }
                else
                {
                    textBoxArticolo.BackColor = Color.LightGreen;
                    labelImportaDescrizione.Text = dr[1].ToString();
                    buttonConferma.Enabled = false;
                }
            }
            else if (radioButtonAgilis.Checked == true && radioButtonLavorazione.Checked == true)
            {

            }
            else if (radioButtonPreventivi.Checked == true)
            {

            }
        }
    }
}
