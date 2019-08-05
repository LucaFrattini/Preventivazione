using PreventivazioneRapida;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Preventivazione_RGG
{
    public partial class ConfermaSalvataggio : Form
    {
        string codArticoloOriginale;
        string[] valoriTestata;
        Model m;
        bool stampa;

        public ConfermaSalvataggio(Model m, String[] valoriTestata, bool stampa)
        {
            InitializeComponent();
            textBoxArticolo.Text = valoriTestata[1];
            codArticoloOriginale = valoriTestata[1];
            this.stampa = stampa;
            this.valoriTestata = valoriTestata;
            this.m = m;
        }

        private void buttonConferma_Click(object sender, EventArgs e)
        {
            if(textBoxArticolo.Text != "")
            {
                try {
                    foreach (DataRow row in m.ds.Tables["DistintaBase"].Rows)
                    {
                        if (row["CODICE_PADRE"].ToString() == codArticoloOriginale)
                        {
                            row["CODICE_PADRE"] = textBoxArticolo.Text;
                        }
                    }
                }
                catch { }
                
                valoriTestata[1] = textBoxArticolo.Text;
                this.Close();
                m.InsertPreventivo(valoriTestata);
                if (stampa)
                {
                    string fileSTAMPA = "StampaPreventivoRGG.xml";
                    m.ScriviXMLperStampa(fileSTAMPA, valoriTestata);
                }
                
            }
            else
            {
                MessageBox.Show("Indicare un codice articolo nuovo non nullo!");
            }
        }
    }
}
