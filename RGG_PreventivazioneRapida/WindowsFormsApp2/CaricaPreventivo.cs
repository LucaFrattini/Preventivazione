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
    public partial class CaricaPreventivo : Form
    {
        private Model m;
        Form1 f;
        Dictionary<int, int> id;
        public CaricaPreventivo(Form1 f, Model pippo)
        {
            InitializeComponent();
            m = pippo;
            this.f = f;
        }

        private void textBoxCliente_TextChanged(object sender, EventArgs e)
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
                    textBoxID.Enabled = true;
                    buttonHelpID.Enabled = true;
                    id = m.IDUltimoPreventivo(textBoxCliente.Text);
                    textBoxID.Text = id.Keys.First().ToString();//id.ToString();
                    //questo è da finire
                }
                else
                {
                    textBoxCliente.BackColor = (textBoxCliente.Text == "" ? Color.White : Color.OrangeRed);
                    labelCliente.Text = "<Descrizione cliente>";
                    textBoxID.Enabled = false;
                    buttonHelpID.Enabled = false;
                }
            }
            catch {
                textBoxCliente.BackColor = (textBoxCliente.Text == "" ? Color.White : Color.OrangeRed);
                labelCliente.Text = "<Descrizione cliente>";
                textBoxID.Enabled = false;
            }
        }

        private void buttonHelpClienti_Click(object sender, EventArgs e)
        {
            Helper h = new Helper();
            String[] par = { };
            //public string StartHelper(string path, string ip, string port, string database, string username, string password, string _campofocus, string _utenteWSAI, string _passwordWSAI, string[] parametri, string valoreFocus, string font, string fontLabel)
            string temp = h.StartHelper(Setting.Istance.HelpCliente, Setting.Istance.Ip, Setting.Istance.Port, Setting.Istance.Database, Setting.Istance.User, Setting.Istance.Password, "", "", "", par, "", Setting.Istance.Font, Setting.Istance.FontLabel);

            if (!String.IsNullOrEmpty(temp))
            {
                this.textBoxCliente.Text = temp;
            }
        }

        private void buttonConferma_Click(object sender, EventArgs e)
        {
            try
            {
                string idpreventivo = textBoxID.Text;
                //DataRow r = m.ds.Tables["Preventivi"].Rows.Find(idpreventivo);
                //if (r != null)
                //{

                //}
                string idEffettivo = id[Int32.Parse(idpreventivo)].ToString();
                //using (SqlCommand cmd = new SqlCommand(query, ))
                List<string> testata = m.OttieniTestata(idEffettivo);
                testata.Add(idpreventivo);
                f.TbCLiente.Text = textBoxCliente.Text;
                //ci vorrebbe anche qualcosa per distinguere l'articolo caricato dal preventivo da quello con la distinta base, per 
                //far sì che la form principale visualizzi i dati corretti 
                f.InserisciTestata(testata);
                m.CaricaPreventivoRighi(idpreventivo, textBoxCliente.Text);
                f.BindingGrid();
                this.Close();

            }
            catch
            {
                MessageBox.Show("Errore! Verificare di aver inserito un ID di preventivo corretto per il cliente selezionato.");
            }
        }

        private void textBoxID_TextChanged(object sender, EventArgs e)
        {
            if(textBoxID.Text != "")
            {
                try
                {
                    string idpreventivo = textBoxID.Text;
                    string idEffettivo = id[Int32.Parse(idpreventivo)].ToString();
                    List<string> testata = m.OttieniTestata(idEffettivo);
                    DataRow dr = m.ds.Tables["Articoli"].Rows.Find(testata[1].ToString());
                    labelNote.Text = testata[6].ToString() + "\n" + testata[1].ToString() + ": " + dr[1].ToString() + "\n" + testata[3].ToString();

                }
                catch
                {
                    MessageBox.Show("ID preventivo non trovato!");
                    labelNote.Text = "<Note preventivo>";
                    textBoxID.Text = "";
                }
            }         
        }

        private void buttonHelpID_Click(object sender, EventArgs e)
        {
            Helper h = new Helper();
            string cliente = textBoxCliente.Text;
            String[] par = { cliente };
            //public string StartHelper(string path, string ip, string port, string database, string username, string password, string _campofocus, string _utenteWSAI, string _passwordWSAI, string[] parametri, string valoreFocus, string font, string fontLabel)
            string temp = h.StartHelper(Setting.Istance.HelpPreventivo, Setting.Istance.Ip, Setting.Istance.Port, Setting.Istance.Database, Setting.Istance.User, Setting.Istance.Password, "", "", "", par, "", Setting.Istance.Font, Setting.Istance.FontLabel);

            if (!String.IsNullOrEmpty(temp))
            {
                this.textBoxID.Text = temp;
            }
        }
    }
}
