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
using System.Xml;

namespace Preventivazione_RGG
{
    public partial class ModificaQuantita : Form
    {
        Form1 f;
        public ModificaQuantita(Form1 f)
        {
            InitializeComponent();
            numericUpDown1.Value = Int32.Parse(Setting.Istance.Q1);
            numericUpDown2.Value = Int32.Parse(Setting.Istance.Q2);
            numericUpDown3.Value = Int32.Parse(Setting.Istance.Q3);
            this.f = f;
            SetFont(Setting.Istance.Font);
        }

        private void Accedi_Click(object sender, EventArgs e)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load("PreventivazioneRapidaConfig.xml");
            XmlNode XMLquantita = doc.SelectSingleNode("configuration/Quantita/q1");
            XMLquantita.InnerText = numericUpDown1.Value.ToString();
            Setting.Istance.Q1 = numericUpDown1.Value.ToString();
            XMLquantita = doc.SelectSingleNode("configuration/Quantita/q2");
            XMLquantita.InnerText = numericUpDown2.Value.ToString();
            Setting.Istance.Q2 = numericUpDown2.Value.ToString();
            XMLquantita = doc.SelectSingleNode("configuration/Quantita/q3");
            XMLquantita.InnerText = numericUpDown3.Value.ToString();
            Setting.Istance.Q3 = numericUpDown3.Value.ToString();
            doc.Save("PreventivazioneRapidaConfig.xml");
            f.CaricaQuantita();
            this.Close();
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
