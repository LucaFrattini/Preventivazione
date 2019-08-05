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
                string query = "SELECT * FROM preventiviLogin WHERE utente = '" + utente + "' AND password = '" + password + "'";
                SqlConnection connection = new SqlConnection(Setting.Istance.ConnStr);
                connection.Open();
                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    SqlDataReader dr = cmd.ExecuteReader();
                    if (dr.HasRows)
                    {
                        f = new Form1(this);
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
            }
            else
            {
                MessageBox.Show("La licenza del programma è scaduta.\nSi prega di contattare Computer Sistemi per risolvere il problema.");
            }
        }
    }
}
