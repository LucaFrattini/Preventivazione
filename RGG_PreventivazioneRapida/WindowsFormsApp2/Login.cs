using LicenseManager;
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

namespace Preventivazione_RGG
{
    public partial class Login : Form
    {
        Form1 f;
        public Login()
        {
            InitializeComponent();
            SetFont(Setting.Istance.Font);
            /*int x = (int)((Screen.PrimaryScreen.WorkingArea.Width / 2) - (this.Width / 2));
            int y = (int)((Screen.PrimaryScreen.WorkingArea.Height / 2) - (this.Height / 2));
            this.Location = new Point(x, y);*/
        }

        private void Accedi_Click(object sender, EventArgs e)
        {
            if (LicenseController.Instance.checkLicenseAlt())
            {
                string utente = textBoxUtente.Text;
                string password = textBoxPassword.Text;
                string query = "SELECT * FROM preventiviLogin WHERE utente = '" + utente + "' AND password = '" + password + "' AND (ultimoAccesso < CURRENT_TIMESTAMP OR ultimoAccesso IS NULL)";
                SqlConnection connection = new SqlConnection(Setting.Istance.ConnStr);
                connection.Open();
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    SqlDataReader dr = cmd.ExecuteReader();
                    if (dr.HasRows)
                    {
                        f = new Form1(this, textBoxUtente.Text);
                        f.Show();
                        this.Visible = false;
                        
                        dr.Close();
                        SqlCommand cmdUpdateTime = new SqlCommand("UPDATE preventiviLogin SET ultimoAccesso = CURRENT_TIMESTAMP WHERE utente = '" + utente + "' AND password = '" + password + "'", connection);
                        dr = cmdUpdateTime.ExecuteReader();
                    }
                    else
                    {
                        MessageBox.Show("Utente e password non corrispondono!");
                    }
                }
                connection.Close();
            }
            else
            {
                MessageBox.Show("La licenza del programma è scaduta.\nSi prega di contattare Computer Sistemi per risolvere il problema.");
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
