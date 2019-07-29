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

       
        /// <summary>
        /// Gestione dell'evento click del button conferma. Raccoglie i dati relativi al preventivo da caricare e le passa come parametro alle funzioni della form principale
        /// per poterle inserire nella form principale e visualizzarle a schermo.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonConferma_Click(object sender, EventArgs e)
        {
            try
            {
                string idpreventivo = textBoxID.Text;

                List<string> testata = m.OttieniTestata(idpreventivo);
                testata.Add(idpreventivo);
                f.TbCLiente.Text = testata[0];
                //ci vorrebbe anche qualcosa per distinguere l'articolo caricato dal preventivo da quello con la distinta base, per 
                //far sì che la form principale visualizzi i dati corretti 
                f.InserisciTestata(testata);
                m.CaricaPreventivoRighi(idpreventivo, testata[0]);
                f.BindingGrid();
                f.Show();
                this.Close();

            }
            catch
            {
                MessageBox.Show("Errore! Verificare di aver inserito un ID di preventivo corretto per il cliente selezionato.");
            }
        }


        /// <summary>
        /// Verifica che l'ID del preventivo inserito sia corretto.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBoxID_TextChanged(object sender, EventArgs e)
        {
            if(textBoxID.Text != "")
            {
                try
                {
                    string idpreventivo = textBoxID.Text;
                    List<string> testata = m.OttieniTestata(idpreventivo);
                    labelNote.Text = "\n\nDATA: " + testata[6].ToString() + "\n\nCLIENTE: " + testata[0].ToString() + "\n\nCODICE ARTICOLO: " + testata[1].ToString() + "\n\nDESCRIZIONE: " + testata[3].ToString();
                    buttonConferma.Enabled = true;
                }
                catch
                {
                    MessageBox.Show("ID preventivo non trovato!");
                    labelNote.Text = "<Note preventivo>";
                    buttonConferma.Enabled = false;
                }
            }
            else
            {
                labelNote.Text = "<Note preventivo>";
                buttonConferma.Enabled = false;
            }         
        }

        /// <summary>
        /// Funzione che gestisce l'evento click per il button degli help degli ID dei preventivi.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonHelpID_Click(object sender, EventArgs e)
        {
            Helper h = new Helper();
            String[] par = {  };
            //Setting.Istance.CambiaValoreCliente(cliente);
            //public string StartHelper(string path, string ip, string port, string database, string username, string password, string _campofocus, string _utenteWSAI, string _passwordWSAI, string[] parametri, string valoreFocus, string font, string fontLabel)
            string temp = h.StartHelper(Setting.Istance.HelpPreventivo, Setting.Istance.Ip, Setting.Istance.Port, Setting.Istance.Database, Setting.Istance.User, Setting.Istance.Password, "", "", "", par, "", Setting.Istance.Font, Setting.Istance.FontLabel);

            if (!String.IsNullOrEmpty(temp))
            {
                this.textBoxID.Text = temp;
                textBoxID.Focus();
                buttonHelpID.Focus();
                buttonConferma.Focus();
            }
        }

    }
}
