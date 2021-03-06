﻿using PreventivazioneRapida;
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
            SetFont(Setting.Istance.Font);

        }

        private void buttonConferma_Click(object sender, EventArgs e)
        {
            if(textBoxArticolo.Text != "")
            {
                try {
                    foreach (DataRow row in m.ds.Tables["DistintaBase"].Rows)
                    {
                        if(row["Codice Padre"].ToString() == codArticoloOriginale)
                        {
                            row["Codice Padre"] = textBoxArticolo.Text;
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
                foreach (Control c in this.Controls)
                {
                    c.Font = f;
                }
                //this.dataGridView.Font = f;
            }


        }
    }


}
