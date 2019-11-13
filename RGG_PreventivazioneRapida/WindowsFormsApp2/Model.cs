using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using Microsoft.Reporting.WinForms;

namespace PreventivazioneRapida
{
    public class Model
    {
        
        private Microsoft.Reporting.WinForms.ReportViewer reportViewer1;
        private IList<FileStream> m_streams;
        public DataSet ds;
        //private DataTable Articoli, Clienti;
        private static SqlConnection sqlserverConn;
        //lock
        private readonly object connectionLock = new object();

        /// <summary>
        /// Funzione costruttore per inizializzare il dataset.
        /// </summary>
        public Model()
        {
            try
            {
                ds = new DataSet();
                sqlserverConn = new SqlConnection(Setting.Istance.ConnStr);
            }
            catch
            {
                MessageBox.Show("Errore nel tentativo di connessione al database!");
            }
            try
            {
                string query = Setting.Istance.QueryArticolo;
                EstraiRisultatoQuery(query, "Articoli");
            }
            catch
            {
                MessageBox.Show("Errore nella lettura degli articoli! La query deve essere all'interno del nodo" +
                    "<Configuration><AllQuery><Articolo>");
            }
            try
            {
                string query = Setting.Istance.QueryCliente;
                EstraiRisultatoQuery(query, "Clienti");
                /*int count = reader.FieldCount;
                while (reader.Read())
                {
                    Cliente cliente = new Cliente(reader.GetValue(0).ToString(), reader.GetValue(1).ToString(), reader.GetValue(2).ToString(),
                        reader.GetValue(3).ToString(), reader.GetValue(4).ToString(), reader.GetValue(5).ToString(), reader.GetValue(6).ToString());
                    listaClienti.Add(cliente);
                }
                Clienti = listaClienti.ToArray();*/
                //sqlserverConn = new SqlConnection(Setting.Istance.ConnStr);
                sqlserverConn.Open();
            }
            catch
            {
                MessageBox.Show("Errore nella lettura dei clienti! La query deve essere all'interno del nodo" +
                    "<Configuration><AllQuery><Cliente>");
            }
        }

        /// <summary>
        /// Funzione che permette di calcolare i numeri del carattere passato come primo parametro all'interno della stringa passata come secondo paramentro.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="searched"></param>
        /// <returns></returns>
        private int FindNumberOfChar(Char target, String searched)
        {
            Console.Write(
                target);

            int startIndex = -1;
            int hitCount = 0;

            // Search for all occurrences of the target.
            while (true)
            {
                startIndex = searched.IndexOf(
                    target, startIndex + 1,
                    searched.Length - startIndex - 1);

                // Exit the loop if the target is not found.
                if (startIndex < 0)
                    break;

                hitCount++;
            }
            return hitCount;

        }

        //Funzione per estrarre dati correttamete dal database ed inserirli in una Datatable
        //Viene usata questa funzione perchè prende la query dal file XML cosi che questa ultima può
        // essere modificata e la DataTable che si va a creare si adatta alle colonee che sono state estratte dalla query
        public void EstraiRisultatoQuery(string query, string nomeTabella)
        {
            try
            {
                SqlDataAdapter da;
                sqlserverConn = new SqlConnection(Setting.Istance.ConnStr);
                sqlserverConn.Open();
                da = new SqlDataAdapter(query, sqlserverConn);
                if (ds.Tables.IndexOf(nomeTabella) > 0)
                {
                    ds.Tables[nomeTabella].Reset();
                    da.Fill(ds.Tables[nomeTabella]);
                }
                else
                {
                    da.Fill(ds);
                    int count = ds.Tables.Count;
                    ds.Tables[count - 1].TableName = nomeTabella;                   
                }
                DataColumn[] key = new DataColumn[1];
                if (nomeTabella == "Articoli")
                {
                    key[0] = ds.Tables["Articoli"].Columns[Setting.Istance.PKArticolo];
                    ds.Tables["Articoli"].PrimaryKey = key; 
                }
                else if (nomeTabella == "Clienti")
                {
                    key[0] = ds.Tables["Clienti"].Columns[Setting.istance.PKCliente];
                    ds.Tables["Clienti"].PrimaryKey = key;
                }
                else if(nomeTabella == "Preventivi")
                {
                    key[0] = ds.Tables["Preventivi"].Columns[0];
                    ds.Tables["Preventivi"].PrimaryKey = key;
                }
                else if(nomeTabella == "DistintaBase")
                {
                    /*ds.Tables["DistintaBase"].Columns.Add("setup mac decimale");
                    ds.Tables["DistintaBase"].Columns.Add("setup uomo decimale");
                    ds.Tables["DistintaBase"].Columns.Add("tempo mac decimale");
                    ds.Tables["DistintaBase"].Columns.Add("tempo uomo decimale");*/
                }else if(nomeTabella == "Lavorazioni")
                {
                    key[0] = ds.Tables["Lavorazioni"].Columns[Setting.Istance.PKLavorazione];
                    ds.Tables["Lavorazioni"].PrimaryKey = key;
                }
                sqlserverConn.Close();               
            }
            catch
            {
                MessageBox.Show("ERRORE nell'esecuzione della query '" + query + "'");
            }                      
        }

        /// <summary>
        /// Funzione che consente di verificare se la datarow passata come parametro sia un lavorato, cioè controlla nel database se ci sono
        /// figli di questo articolo. Ritorna il numero dei figli trovati.
        /// </summary>
        /// <param name="row"></param>
        /// <returns></returns>
        public int VerificaSemilavorato(DataRow row)
        {
            SqlDataAdapter da;
            try
            {
                int count;
                lock (connectionLock)
                {
                    sqlserverConn.Open();
                    string distintaBase = Setting.Istance.QueryDistintaBase.Replace("@CodDistBase", row["CODICE ART"].ToString());
                    da = new SqlDataAdapter(distintaBase, sqlserverConn);
                    if (ds.Tables["DistintaBase"].PrimaryKey.Length > 0)
                    {
                        ds.Tables["DistintaBase"].PrimaryKey = null;
                    }
                    count = da.Fill(ds.Tables["DistintaBase"]);
                    sqlserverConn.Close();
                }               
                return count;
            }
            catch(Exception e)
            {
                sqlserverConn.Close();
                MessageBox.Show("Errore Verifica Semilavorato!\n" + e.Message);
                return 0;
            }
            
                      
        }

        /// <summary>
        /// Funzione che viene utilizzata per salvare il preventivo nel database (nel database un preventivo viene salvato su due tabelle distinta, una che riguarda l'interstazione
        /// e un'altra che riguarda le righe). Quindi viene passato come parametro in ingresso un array che contiene i dati di intestazione.
        /// </summary>
        /// <param name="valoriTestata"></param>
        public void InsertPreventivo(string []valoriTestata)
        {
            try
            {
                sqlserverConn.Open();
                int idpreventivo = 0, idpreventivoass = 0;
                //Prima query utilizzata per il salvataggio dei dati della testata del preventivo.
                string queryTestata = "INSERT INTO preventivi (utente, cliente, desccliente, articolo, descarticolo, quantita, variazione, variazionelav, totale, totalevar, datacreazione, note, QImateriaprima,QIcostomac, QIcostouomo, QIcostosingolo, QIricavosingolo, quantita1, costoMacchina1, costoUomo1, costoMateriali1, costoSingolo1, ricavoSingolo1, costoTotale1, ricavoTotale1, quantita2, costoMacchina2, costoUomo2, costoMateriali2, costoSingolo2, ricavoSingolo2, costoTotale2, ricavoTotale2, quantita3, costoMacchina3, costoUomo3, costoMateriali3, costoSingolo3, ricavoSingolo3, costoTotale3, ricavoTotale3) VALUES ('"
                    + valoriTestata[15] + "', '" + valoriTestata[0] + "', '" + valoriTestata[13] + "', '" + valoriTestata[1] + "', '" + valoriTestata[14] + "', " + valoriTestata[2].Replace(',','.') + ", " + valoriTestata[3].Replace(',', '.') + ", " + valoriTestata[4].Replace(',', '.') + ", " 
                    + valoriTestata[5].Replace(',', '.') + ", " + valoriTestata[6].Replace(',', '.') + ", CURRENT_TIMESTAMP, '" + valoriTestata[7] + "', " + valoriTestata[8].Replace(',', '.') + "," + valoriTestata[9].Replace(',', '.') + ", " + valoriTestata[10].Replace(',', '.') + ", " 
                    + valoriTestata[11].Replace(',', '.') + ", " + valoriTestata[12].Replace(',', '.') + ", "
                    + valoriTestata[16].Replace(',', '.') + ", " + valoriTestata[17].Replace(',', '.') + ", " + valoriTestata[18].Replace(',', '.') + ", " + valoriTestata[19].Replace(',', '.') + ", " + valoriTestata[20].Replace(',', '.') + ", " + valoriTestata[21].Replace(',', '.') + ", " + valoriTestata[22].Replace(',', '.') + ", " + valoriTestata[23].Replace(',', '.') + ", "
                    + valoriTestata[24].Replace(',', '.') + ", " + valoriTestata[25].Replace(',', '.') + ", " + valoriTestata[26].Replace(',', '.') + ", " + valoriTestata[27].Replace(',', '.') + ", " + valoriTestata[28].Replace(',', '.') + ", " + valoriTestata[29].Replace(',', '.') + ", " + valoriTestata[30].Replace(',', '.') + ", " + valoriTestata[31].Replace(',', '.') + ", "
                    + valoriTestata[32].Replace(',', '.') + ", " + valoriTestata[33].Replace(',', '.') + ", " + valoriTestata[34].Replace(',', '.') + ", " + valoriTestata[35].Replace(',', '.') + ", " + valoriTestata[36].Replace(',', '.') + ", " + valoriTestata[37].Replace(',', '.') + ", " + valoriTestata[38].Replace(',', '.') + ", " + valoriTestata[39].Replace(',', '.') 
                    + ")";
                SqlCommand command = new SqlCommand(queryTestata, sqlserverConn);
                SqlDataReader reader = command.ExecuteReader();
                reader.Close();
                             
                string queryIDpreventivo = "SELECT MAX(id) FROM preventivi";
                using(SqlCommand cmd = new SqlCommand(queryIDpreventivo, sqlserverConn))
                {
                    SqlDataReader dr = cmd.ExecuteReader();
                    dr.Read();
                    idpreventivo = (int)dr[0];//Int32.Parse(dr[0].ToString());
                    dr.Close();
                }

                //Quindi scorro il dataset e salvo nel database (tabella dei righi) tutte le righe che sono visualizzate nella datagrid della form, e quindi presenti nel dataset.
                foreach (DataRow row in ds.Tables["DistintaBase"].Rows)
                {
                    string queryRighi = "INSERT INTO preventivirighi (idpreventivo, rowindex, codicepadre, codiceart, codicecentro, codicelav, descrizione, um1, quantita1, um2, quantita2, um3, quantita3, setupmac, setupuomo, tempomac, tempouomo, costoart" +
                    ", costoattmac, costoattuomo, costomac, costouomo, totale, totalevar, setupmacdec, setupuomodec, tempomacdec, tempouomodec) VALUES (" + idpreventivo + ", '" + row["Rigo"].ToString() + "', '" + row["Codice Padre"].ToString() + "', '" +
                    row["CODICE ART"].ToString() + "', '" +row["Codice centro"].ToString() + "', '" + row["Codice lav"].ToString() + "', '" + row["Descrizione art / Centro di Lavoro"].ToString() + 
                    "', '" + row["UM 1"].ToString() +"', '" + row["Quantita` 1"].ToString() + "', '" + row["UM 2"].ToString() + "', '" + row["Qta 2"].ToString() + "', '" + row["UM 3"].ToString() + "', '" + row["Qta 3"].ToString() +
                    "', '" + row["Setup Mac"].ToString() + "', '" + row["Setup Uomo"].ToString() + "', '" + row["Tempo Mac"].ToString() + "', '" + row["Tempo Uomo"].ToString() +
                    "', '" + row["Costo Art"].ToString() + "', '" + row["Costo Att Mac"].ToString() + "', '" + row["Costo Att Uomo"].ToString() + "', '" + row["Costo Mac"].ToString() +
                    "', '" + row["Costo Uomo"].ToString() + "', '" + row["Totale"].ToString() + "', '" + row["Totale + %Var"].ToString() +
                    "', '" + row["Setup Mac decimale"].ToString() +"', '"+ row["Setup Uomo decimale"].ToString() + "', '"+ row["Tempo Mac decimale"].ToString() + "', '"+ row["Tempo Uomo decimale"].ToString() + "')";
                    command = new SqlCommand(queryRighi, sqlserverConn);
                    command.ExecuteNonQuery();
                }
                string queryIDpreventivocliente = "SELECT MAX(rowindex) FROM (SELECT (ROW_NUMBER() OVER(ORDER BY id)) as rowindex, * FROM preventivi WHERE cliente ='"+ valoriTestata[0] + "') AS clientepreventivi";
                using (SqlCommand cmd = new SqlCommand(queryIDpreventivocliente, sqlserverConn))
                {
                    SqlDataReader dr = cmd.ExecuteReader();
                    dr.Read();
                    idpreventivoass = Int32.Parse(dr[0].ToString());
                    dr.Close();
                }
                MessageBox.Show("Il preventivo è stato salvato!\nID preventivo assoluto: " + idpreventivo + ".\nID preventivo del cliente " + valoriTestata[0] + ": " + idpreventivoass);
                sqlserverConn.Close();
            }
            catch(Exception e)
            {
                MessageBox.Show("Errore durante il salvataggio del preventivo. Controllare la connessione con il server.\n"+e.Message);
                sqlserverConn.Close();
            }
            return;
        }

        /// <summary>
        /// Funzione che viene utilizzata per poter selezionare e far visualizzare l'ID dell'ultimo preventivo salvato per il cliente selezionato dall'utente.
        /// </summary>
        /// <param name="cliente"></param>
        /// <returns></returns>
        public Dictionary<int,int> IDUltimoPreventivo(string cliente)
        {
            Dictionary<int, int> IDPreventivi = new Dictionary<int, int>();
            try
            {
                
                string nometabella = "Preventivi";
                string query = "SELECT (ROW_NUMBER() OVER(ORDER BY id)) as rownumber, * FROM preventivi WHERE cliente='" + cliente + "'";
                EstraiRisultatoQuery(query, nometabella);
                DataTable dt = ds.Tables["Preventivi"];

                query = "SELECT rownumber, id FROM (SELECT (ROW_NUMBER() OVER(ORDER BY id)) as rownumber, id FROM preventivi WHERE cliente='" + cliente + "' group by id) AS preventiviMAXID group by rownumber, id order by rownumber DESC";
                sqlserverConn.Open();
                using (SqlCommand cmd = new SqlCommand(query, sqlserverConn))
                {
                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        IDPreventivi.Add(Int32.Parse(dr[0].ToString()), Int32.Parse(dr[1].ToString()));
                    }
                    dr.Close();
                }
                sqlserverConn.Close();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            return IDPreventivi;//maxID;
        }

        /// <summary>
        /// Funzione che viene utilizzata per estrarre i dati di testata dal database
        /// </summary>
        /// <param name="idpreventivo"></param>
        /// <returns></returns>
        public List<string> OttieniTestata(string idpreventivo)
        {
            List<string> testata = new List<string>();
            string query = "select cliente, articolo, quantita, note, variazione, variazionelav, datacreazione, desccliente, descarticolo from preventivi where id = '" + idpreventivo + "'";
            sqlserverConn.Open();
            using (SqlCommand cmd = new SqlCommand(query, sqlserverConn))
            {
                SqlDataReader dr = cmd.ExecuteReader();
                dr.Read();
                if (dr.HasRows)
                {
                    testata.Add(dr[0].ToString());
                    testata.Add(dr[1].ToString());
                    testata.Add(dr[2].ToString());
                    testata.Add(dr[3].ToString());
                    testata.Add(dr[4].ToString());
                    testata.Add(dr[5].ToString());
                    testata.Add(dr[6].ToString());
                    testata.Add(dr[7].ToString());
                    testata.Add(dr[8].ToString());

                }
                dr.Close();
            }
            sqlserverConn.Close();
            return (testata);
        }

        /// <summary>
        /// Funzione che cerca le righe del preventivo che si vuole caricaricare, le estrae e le salva nel dataset
        /// </summary>
        /// <param name="idpreventivo"></param>
        /// <param name="cliente"></param>
        public void CaricaPreventivoRighi(string idpreventivo, string cliente)
        {
            try
            {
                SqlDataAdapter da;
                sqlserverConn.Open();
                string query = "SELECT rowindex as 'Rigo',codicepadre as 'Codice Padre',codiceart AS 'Codice Art', codicecentro AS 'Codice Centro', codicelav AS 'Codice Lav', descrizione AS 'Descrizione art / Centro di Lavoro'," +
                    "um1 AS 'UM 1', quantita1 AS 'Quantita` 1', um2 AS 'UM 2', quantita2 AS 'Qta 2', um3 AS 'UM 3', quantita3 AS 'Qta 3', setupmac AS 'Setup Mac', setupuomo AS 'Setup Uomo', tempomac AS 'Tempo Mac', tempouomo AS 'Tempo Uomo', costoart AS 'Costo Art', " +
                    "costoattmac AS 'Costo Att Mac', costoattuomo AS 'Costo Att Uomo'," +
                    "costomac AS 'Costo Mac', costouomo AS 'Costo Uomo', totale AS 'Totale', totalevar AS 'Totale + %Var', setupmacdec AS 'setup mac decimale', setupuomodec AS 'setup uomo decimale'," +
                    " tempomacdec AS 'tempo mac decimale', tempouomodec AS 'tempo uomo decimale'  FROM preventivirighi WHERE idpreventivo = "+Int32.Parse(idpreventivo);
                da = new SqlDataAdapter(query, sqlserverConn);
                if (ds.Tables.IndexOf("DistintaBase") > 0)
                {
                    //ds.Tables["DistintaBase"].Reset();
                    ds.Tables.Remove("DistintaBase");
                    da.Fill(ds);
                    int count = ds.Tables.Count;
                    ds.Tables[count - 1].TableName = "DistintaBase";
                }
                else
                {
                    da.Fill(ds);
                    int count = ds.Tables.Count;
                    ds.Tables[count - 1].TableName = "DistintaBase";
                }
                sqlserverConn.Close();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
        }

        /// <summary>
        /// Funzione di conversione dei tempi. Si passa dal tempo salvato in decimale a tempo in sessantesimi. Questa funzione viene utilizzata su tutte le righe del dataset.
        /// </summary>
        public void FromDecimalToTime()
        {          
            foreach (DataRow dr in ds.Tables["DistintaBase"].Rows)
            {
                try
                {
                    if(dr["codice art"].ToString() == "")
                    {
                        double tempoDecimale = Double.Parse(dr["setup mac decimale"].ToString());
                        int ore = (int)tempoDecimale;
                        tempoDecimale -= ore;
                        double tempo = Math.Round((tempoDecimale * 60), 2);
                        double minuti = (int)tempo;
                        tempo = tempo - minuti;
                        double secondi = tempo * 60;
                        tempo = ore + (minuti / 100) + (secondi / 10000);
                        string s = String.Format("{0:N4}", tempo);
                        dr["Setup Mac"] = s;

                        tempoDecimale = Double.Parse(dr["setup uomo decimale"].ToString());
                        ore = (int)tempoDecimale;
                        tempoDecimale -= ore;
                        tempo = Math.Round((tempoDecimale * 60), 2);
                        minuti = (int)tempo;
                        tempo = tempo - minuti;
                        secondi = tempo * 60;
                        tempo = ore + (minuti / 100) + (secondi / 10000);
                        s = String.Format("{0:N4}", tempo);
                        dr["Setup uomo"] = s;

                        tempoDecimale = Double.Parse(dr["tempo mac decimale"].ToString());
                        ore = (int)tempoDecimale;
                        tempoDecimale -= ore;
                        tempo = Math.Round((tempoDecimale * 60), 2);
                        minuti = (int)tempo;
                        tempo = tempo - minuti;
                        secondi = tempo * 60;
                        tempo = ore + (minuti / 100) + (secondi / 10000);
                        s = String.Format("{0:N4}", tempo);
                        dr["tempo mac"] = s;

                        tempoDecimale = Double.Parse(dr["tempo uomo decimale"].ToString());
                        ore = (int)tempoDecimale;
                        tempoDecimale -= ore;
                        tempo = Math.Round((tempoDecimale * 60), 2);
                        minuti = (int)tempo;
                        tempo = tempo - minuti;
                        secondi = tempo * 60;
                        tempo = ore + (minuti / 100) + (secondi / 10000);
                        s = String.Format("{0:N4}", tempo);
                        dr["tempo uomo"] = s;
                    }                   
                }
                catch { }
            }           
        }

        /// <summary>
        /// Funzione di conversione dei tempi di una riga specifica. Si passa dal tempo salvato in decimale a tempo in sessantesimi. 
        /// </summary>
        public void FromDecimalToTime(DataRow dr)
        {
            try
            {
                if(dr["codice art"].ToString() == "")
                {
                    double tempoDecimale = Double.Parse(dr["setup mac decimale"].ToString());
                    int ore = (int)tempoDecimale;
                    tempoDecimale -= ore;
                    double tempo = Math.Round((tempoDecimale * 60), 2);
                    double minuti = (int)tempo;
                    tempo = tempo - minuti;
                    double secondi = tempo * 60;
                    tempo = ore + (minuti / 100) + (secondi / 10000);
                    string s = String.Format("{0:N4}", tempo);
                    dr["Setup Mac"] = s;

                    tempoDecimale = Double.Parse(dr["setup uomo decimale"].ToString());
                    ore = (int)tempoDecimale;
                    tempoDecimale -= ore;
                    tempo = Math.Round((tempoDecimale * 60), 2);
                    minuti = (int)tempo;
                    tempo = tempo - minuti;
                    secondi = tempo * 60;
                    tempo = ore + (minuti / 100) + (secondi / 10000);
                    s = String.Format("{0:N4}", tempo);
                    dr["Setup uomo"] = s;

                    tempoDecimale = Double.Parse(dr["tempo mac decimale"].ToString());
                    ore = (int)tempoDecimale;
                    tempoDecimale -= ore;
                    tempo = Math.Round((tempoDecimale * 60), 2);
                    minuti = (int)tempo;
                    tempo = tempo - minuti;
                    secondi = tempo * 60;
                    tempo = ore + (minuti / 100) + (secondi / 10000);
                    s = String.Format("{0:N4}", tempo);
                    dr["tempo mac"] = s;

                    tempoDecimale = Double.Parse(dr["tempo uomo decimale"].ToString());
                    ore = (int)tempoDecimale;
                    tempoDecimale -= ore;
                    tempo = Math.Round((tempoDecimale * 60), 2);
                    minuti = (int)tempo;
                    tempo = tempo - minuti;
                    secondi = tempo * 60;
                    tempo = ore + (minuti / 100) + (secondi / 10000);
                    s = String.Format("{0:N4}", tempo);
                    dr["tempo uomo"] = s;
                }               
            }
            catch { }
            
        }

        /// <summary>
        /// Funzione di conversione dei tempi. Si passa dal tempo salvato in sessantesimi a tempo in decimali. Questa funzione viene utilizzata su tutte le righe del dataset.
        /// </summary>
        public void FromTimeToDecimal()
        {
            foreach (DataRow dr in ds.Tables["DistintaBase"].Rows)
            {
                try
                {
                    if(dr["codice art"].ToString() == "")
                    {
                        double tempoDecimale = Double.Parse(dr["setup Mac"].ToString());
                        int ore = Int32.Parse(Math.Floor(tempoDecimale).ToString());
                        tempoDecimale -= ore;
                        tempoDecimale = tempoDecimale * 100;
                        double minuti = Int32.Parse(Math.Floor(Math.Round(tempoDecimale)).ToString());
                        tempoDecimale = Math.Round((tempoDecimale - minuti) * 100, 2);
                        double secondi = Int32.Parse(Math.Floor(Math.Round(tempoDecimale)).ToString());
                        secondi = Math.Round(secondi + (minuti * 60));
                        double tempo = secondi / 3600;
                        dr["setup mac decimale"] = (ore + tempo).ToString();

                        tempoDecimale = Double.Parse(dr["setup uomo"].ToString());
                        ore = Int32.Parse(Math.Floor(tempoDecimale).ToString());
                        tempoDecimale -= ore;
                        tempoDecimale = tempoDecimale * 100;
                        minuti = Int32.Parse(Math.Floor(Math.Round(tempoDecimale)).ToString());
                        tempoDecimale = Math.Round((tempoDecimale - minuti) * 100, 2);
                        secondi = Int32.Parse(Math.Floor(Math.Round(tempoDecimale)).ToString());
                        secondi = Math.Round(secondi + (minuti * 60));
                        tempo = secondi / 3600;
                        dr["setup uomo decimale"] = (ore + tempo).ToString();

                        tempoDecimale = Double.Parse(dr["tempo mac"].ToString());
                        ore = Int32.Parse(Math.Floor(tempoDecimale).ToString());
                        tempoDecimale -= ore;
                        tempoDecimale = tempoDecimale * 100;
                        minuti = Int32.Parse(Math.Floor(Math.Round(tempoDecimale)).ToString());
                        tempoDecimale = Math.Round((tempoDecimale - minuti) * 100, 2);
                        secondi = Int32.Parse(Math.Floor(Math.Round(tempoDecimale)).ToString());
                        secondi = Math.Round(secondi + (minuti * 60));
                        tempo = secondi / 3600;
                        dr["tempo mac decimale"] = (ore + tempo).ToString();

                        tempoDecimale = Double.Parse(dr["tempo uomo"].ToString());
                        ore = Int32.Parse(Math.Floor(tempoDecimale).ToString());
                        tempoDecimale -= ore;
                        tempoDecimale = tempoDecimale * 100;
                        minuti = Int32.Parse(Math.Floor(Math.Round(tempoDecimale)).ToString());
                        tempoDecimale = Math.Round((tempoDecimale - minuti) * 100, 2);
                        secondi = Int32.Parse(Math.Floor(Math.Round(tempoDecimale)).ToString());
                        secondi = Math.Round(secondi + (minuti * 60));
                        tempo = secondi / 3600;
                        dr["tempo uomo decimale"] = (ore + tempo).ToString();
                    }                  
                }
                catch { }               
            }
        }

        /// <summary>
        /// Funzione di conversione dei tempi di una singola riga. Si passa dal tempo salvato in sessantesimi a tempo in decimale. 
        /// </summary>
        public void FromTimeToDecimal(DataRow dr)
        {
            try
            {
                if(dr["codice art"].ToString() == "")
                {
                    double tempoDecimale = Double.Parse(dr["setup Mac"].ToString());
                    int ore = Int32.Parse(Math.Floor(tempoDecimale).ToString());
                    tempoDecimale -= ore;
                    tempoDecimale = tempoDecimale * 100;
                    double minuti = Int32.Parse(Math.Floor(Math.Round(tempoDecimale)).ToString());
                    tempoDecimale = Math.Round((tempoDecimale - minuti) * 100, 2);
                    double secondi = Int32.Parse(Math.Floor(Math.Round(tempoDecimale)).ToString());
                    secondi = Math.Round(secondi + (minuti * 60));
                    double tempo = secondi / 3600;
                    dr["setup mac decimale"] = (ore + tempo).ToString();

                    tempoDecimale = Double.Parse(dr["setup uomo"].ToString());
                    ore = Int32.Parse(Math.Floor(tempoDecimale).ToString());
                    tempoDecimale -= ore;
                    tempoDecimale = tempoDecimale * 100;
                    minuti = Int32.Parse(Math.Floor(Math.Round(tempoDecimale)).ToString());
                    tempoDecimale = Math.Round((tempoDecimale - minuti) * 100, 2);
                    secondi = Int32.Parse(Math.Floor(Math.Round(tempoDecimale)).ToString());
                    secondi = Math.Round(secondi + (minuti * 60));
                    tempo = secondi / 3600;
                    dr["setup uomo decimale"] = (ore + tempo).ToString();

                    tempoDecimale = Double.Parse(dr["tempo mac"].ToString());
                    ore = Int32.Parse(Math.Floor(tempoDecimale).ToString());
                    tempoDecimale -= ore;
                    tempoDecimale = tempoDecimale * 100;
                    minuti = Int32.Parse(Math.Floor(Math.Round(tempoDecimale)).ToString());
                    tempoDecimale = Math.Round((tempoDecimale - minuti) * 100, 2);
                    secondi = Int32.Parse(Math.Floor(Math.Round(tempoDecimale)).ToString());
                    secondi = Math.Round(secondi + (minuti * 60));
                    tempo = secondi / 3600;
                    dr["tempo mac decimale"] = (ore + tempo).ToString();

                    tempoDecimale = Double.Parse(dr["tempo uomo"].ToString());
                    ore = Int32.Parse(Math.Floor(tempoDecimale).ToString());
                    tempoDecimale -= ore;
                    tempoDecimale = tempoDecimale * 100;
                    minuti = Int32.Parse(Math.Floor(Math.Round(tempoDecimale)).ToString());
                    tempoDecimale = Math.Round((tempoDecimale - minuti) * 100, 2);
                    secondi = Int32.Parse(Math.Floor(Math.Round(tempoDecimale)).ToString());
                    secondi = Math.Round(secondi + (minuti * 60));
                    tempo = secondi / 3600;
                    dr["tempo uomo decimale"] = (ore + tempo).ToString();
                }                
            }
            catch { }
        }

        /// <summary>
        /// Funzione utilizzata per inizializzare le colonne del tempo in decimale uguale a quello in sessantesimi, poi verrà convertito.
        /// </summary>
        public void InizializzaColonneTempo()
        {
            foreach (DataRow datarow in ds.Tables["DistintaBase"].Rows)
            {
                datarow["setup mac decimale"] = datarow["Setup Mac"];
                datarow["setup uomo decimale"] = datarow["Setup Uomo"];
                datarow["tempo mac decimale"] = datarow["Tempo Mac"];
                datarow["tempo uomo decimale"] = datarow["Tempo Uomo"];
            }
        }

        /// <summary>
        /// Questa funzione controlla se la datarow passata come parametro faccia parte di una lavorazione esterna, quindi verrà trattata in una maniera particolare.
        /// Viene gestita come se fosse un articolo, quindi inserendo in "codice art" un valore, per poterla differenziare da tutte le altre righe di lavorazione.
        /// </summary>
        /// <param name="rowLavorazione"></param>
        /// <param name="articolo"></param>
        public void GestisciLavorazioneEsterna(DataRow rowLavorazione, string articolo)
        {
            try
            {
                lock (connectionLock)
                {
                    rowLavorazione["Codice Art"] = rowLavorazione["Descrizione art / Centro di Lavoro"].ToString();
                    string query = Setting.Istance.QueryLavorazioneEsterna.Replace("@LavorazioneCentro", rowLavorazione["Codice Centro"].ToString());
                    query = query.Replace("@LavorazioneEsterna", articolo);
                    sqlserverConn.Open();
                    string prezzo = "0";
                    using (SqlCommand cmd = new SqlCommand(query, sqlserverConn))
                    {
                        SqlDataReader dr = cmd.ExecuteReader();
                        dr.Read();
                        if (dr.HasRows)
                        {
                            prezzo = dr[0].ToString();
                        }
                        dr.Close();
                    }
                    sqlserverConn.Close();
                    rowLavorazione["Costo art"] = prezzo;
                    rowLavorazione["Setup mac"] = "";
                    rowLavorazione["Setup uomo"] = "";
                    rowLavorazione["Tempo mac"] = "";
                    rowLavorazione["Tempo uomo"] = "";
                    rowLavorazione["Costo att mac"] = "";
                    rowLavorazione["Costo att uomo"] = "";
                    rowLavorazione["Costo mac"] = "";
                    rowLavorazione["Costo uomo"] = "";
                }
            }
            catch { MessageBox.Show("Errore gestione lavorazione esterna."); }
            
        }

        public void ScriviXMLperStampa(string fileSTAMPA, string[] valoriTestata)
        {
            XmlDocument xmlDoc = new XmlDocument();
            //File.AppendAllText(fileSTAMPA, "<?xml version=\"1.0\" ?>");
            
            using (StreamWriter outputFile = new StreamWriter(fileSTAMPA))
            {
                outputFile.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
                outputFile.WriteLine("<Rows>");
                foreach(DataRow row in ds.Tables["DistintaBase"].Rows)
                {
                    outputFile.WriteLine("  <Row>");
                    outputFile.WriteLine("      <Data>" + DateTime.Now.ToString() + "</Data>");
                    outputFile.WriteLine("      <Cliente>"+valoriTestata[0]+"</Cliente>");
                    outputFile.WriteLine("      <DescrizioneCliente>" + (valoriTestata[13].ToString() == "<Descrizione cliente>" ? "" : valoriTestata[13].ToString().Replace("&", "&amp;").Replace("'", "&apos;").Replace("\"", "&quot;")) + "</DescrizioneCliente>");
                    outputFile.WriteLine("      <DistintaBase>" + valoriTestata[1] + "</DistintaBase>");
                    outputFile.WriteLine("      <DescrizioneDistintaBase>" + (valoriTestata[14].ToString() == "<Descrizione articolo>" ? "" : valoriTestata[14].ToString().Replace("&", "&amp;").Replace("'", "&apos;").Replace("\"", "&quot;")) + "</DescrizioneDistintaBase>");
                    outputFile.WriteLine("      <Quantita>" + valoriTestata[2] + "</Quantita>");
                    outputFile.WriteLine("      <PercentualeMateriaPrima>" + valoriTestata[3] + "</PercentualeMateriaPrima>");
                    outputFile.WriteLine("      <PercentualeLavorazioni>" + valoriTestata[4] + "</PercentualeLavorazioni>");
                    outputFile.WriteLine("      <Note>" + valoriTestata[7].ToString().Replace("&", "&amp;").Replace("'", "&apos;").Replace("\"", "&quot;") + "</Note>");
                    outputFile.WriteLine("      <CostoMateriaPrima>" + valoriTestata[8] + "</CostoMateriaPrima>");
                    outputFile.WriteLine("      <CostoMacchina>" + valoriTestata[9] + "</CostoMacchina>");
                    outputFile.WriteLine("      <CostoUomo>" + valoriTestata[10] + "</CostoUomo>");
                    outputFile.WriteLine("      <CostoSingolo>" + valoriTestata[11] + "</CostoSingolo>");
                    outputFile.WriteLine("      <RicavoSingolo>" + valoriTestata[12] + "</RicavoSingolo>");
                    outputFile.WriteLine("      <CostoTotale>" + valoriTestata[5] + "</CostoTotale>");
                    outputFile.WriteLine("      <RicavoTotale>" + valoriTestata[6] + "</RicavoTotale>");
                    outputFile.WriteLine("      <Livello>" + row["Rigo"].ToString() + "</Livello>");
                    outputFile.WriteLine("      <Padre>" + row["Codice Padre"].ToString() + "</Padre>");
                    outputFile.WriteLine("      <Articolo>" + row["Codice Art"].ToString() + "</Articolo>");
                    outputFile.WriteLine("      <Centro>" + row["Codice Centro"].ToString() + "</Centro>");
                    outputFile.WriteLine("      <Lavorazione>" + row["Codice Lav"].ToString() + "</Lavorazione>");
                    outputFile.WriteLine("      <Descrizione>" + row["Descrizione art / Centro di Lavoro"].ToString().Replace("&", "&amp;").Replace("'", "&apos;").Replace("\"", "&quot;") + "</Descrizione>");
                    outputFile.WriteLine("      <UM1>" + row["UM 1"].ToString() + "</UM1>");
                    outputFile.WriteLine("      <QuantitaRigo1>" + row["Quantita` 1"].ToString() + "</QuantitaRigo1>");
                    outputFile.WriteLine("      <UM2>" + row["UM 2"].ToString() + "</UM2>");
                    outputFile.WriteLine("      <QuantitaRigo2>" + row["Qta 2"].ToString() + "</QuantitaRigo2>");
                    outputFile.WriteLine("      <UM3>" + row["UM 3"].ToString() + "</UM3>");
                    outputFile.WriteLine("      <QuantitaRigo3>" + row["Qta 3"].ToString() + "</QuantitaRigo3>");
                    int lavorazioneOmateriale = row["Setup Mac"].ToString().Length;
                    
                    if(lavorazioneOmateriale > 0)
                    {
                        int indiceVirgola = row["Setup Mac"].ToString().IndexOf(',');
                        if(indiceVirgola < 0)
                        {
                            indiceVirgola = 0;
                        }
                        outputFile.WriteLine("      <SetupMacchina>" + row["Setup Mac"].ToString().Substring(0, indiceVirgola) + ":" + row["Setup Mac"].ToString().Substring(indiceVirgola + 1, 2) + ":" + row["Setup Mac"].ToString().Substring(indiceVirgola + 3, 2) + "</SetupMacchina>");
                        indiceVirgola = row["Setup Uomo"].ToString().IndexOf(',');
                        if (indiceVirgola < 0)
                        {
                            indiceVirgola = 0;
                        }
                        outputFile.WriteLine("      <SetupUomo>" + row["Setup Uomo"].ToString().Substring(0, indiceVirgola) + ":" + row["Setup Uomo"].ToString().Substring(indiceVirgola + 1, 2) + ":" + row["Setup Uomo"].ToString().Substring(indiceVirgola + 3, 2) + "</SetupUomo>");
                        indiceVirgola = row["Tempo Mac"].ToString().IndexOf(',');
                        if (indiceVirgola < 0)
                        {
                            indiceVirgola = 0;
                        }
                        outputFile.WriteLine("      <TempoMacchina>" + row["Tempo Mac"].ToString().Substring(0, indiceVirgola) + ":" + row["Tempo Mac"].ToString().Substring(indiceVirgola + 1, 2) + ":" + row["Tempo Mac"].ToString().Substring(indiceVirgola + 3, 2) + "</TempoMacchina>");
                        indiceVirgola = row["Tempo Mac"].ToString().IndexOf(',');
                        if (indiceVirgola < 0)
                        {
                            indiceVirgola = 0;
                        }
                        outputFile.WriteLine("      <TempoUomo>" + row["Tempo Uomo"].ToString().Substring(0, indiceVirgola) + ":" + row["Tempo Uomo"].ToString().Substring(indiceVirgola + 1, 2) + ":" + row["Tempo Uomo"].ToString().Substring(indiceVirgola + 3, 2) + "</TempoUomo>");
                    }
                    else
                    {
                        outputFile.WriteLine("      <SetupMacchina></SetupMacchina>");
                        outputFile.WriteLine("      <SetupUomo></SetupUomo>");
                        outputFile.WriteLine("      <TempoMacchina></TempoMacchina>");
                        outputFile.WriteLine("      <TempoUomo></TempoUomo>");
                    }
                    outputFile.WriteLine("      <CostoArticolo>" + row["Costo Art"].ToString() + "</CostoArticolo>");
                    outputFile.WriteLine("      <CostoAttrezzaggioMacchina>" + (row["Costo Att Mac"].ToString() != "" ? row["Costo Att Mac"].ToString() : "0") + "</CostoAttrezzaggioMacchina>");
                    outputFile.WriteLine("      <CostoAttrezzaggioUomo>" + (row["Costo Att Uomo"].ToString() != "" ? row["Costo Att Uomo"].ToString() : "0") + "</CostoAttrezzaggioUomo>");
                    outputFile.WriteLine("      <CostoOrarioMacchina>" + (row["Costo Mac"].ToString() != "" ? row["Costo Mac"].ToString() : "0") + "</CostoOrarioMacchina>");
                    outputFile.WriteLine("      <CostoOrarioUomo>" + (row["Costo Uomo"].ToString() != "" ? row["Costo Uomo"].ToString() : "0") + "</CostoOrarioUomo>");
                    outputFile.WriteLine("      <CostoTotaleRigo>" + row["Totale"].ToString() + "</CostoTotaleRigo>");
                    outputFile.WriteLine("      <RicavoTotaleRigo>" + row["Totale + %Var"].ToString() + "</RicavoTotaleRigo>");
                    outputFile.WriteLine("      <TempoSetupMacchinaDecimale>" + (row["setup mac decimale"].ToString() != "" ? row["setup mac decimale"].ToString() : "0") + "</TempoSetupMacchinaDecimale>");
                    outputFile.WriteLine("      <TempoSetupUomoDecimale>" + (row["setup uomo decimale"].ToString() != "" ? row["setup uomo decimale"].ToString() : "0") + "</TempoSetupUomoDecimale>");
                    outputFile.WriteLine("      <TempoMacchinaDecimale>" + (row["tempo mac decimale"].ToString() != "" ? row["tempo mac decimale"].ToString() : "0") + "</TempoMacchinaDecimale>");
                    outputFile.WriteLine("      <TempoUomoDecimale>" + (row["tempo uomo decimale"].ToString() != "" ? row["tempo uomo decimale"].ToString() : "0") + "</TempoUomoDecimale>");

                    outputFile.WriteLine("  </Row>");
                }
                outputFile.WriteLine("</Rows>");
            }
            reportViewer1 = new ReportViewer();
            reportViewer1.ProcessingMode = ProcessingMode.Local;
            LocalReport localReport = reportViewer1.LocalReport;
            reportViewer1.LocalReport.EnableExternalImages = true;
            string rdlName = "StampaPreventivazioneRapida.rdl";
            localReport.ReportPath = rdlName;


            if (!File.Exists(localReport.ReportPath))
            {
                MessageBox.Show("Il file " + rdlName + " specificato non è presente nella cartella report");
                return;
            }
            DataSet dataSet = new DataSet();
            dataSet.ReadXml("StampaPreventivoRGG.xml");
            localReport.DataSources.Add(new ReportDataSource("DataSet1", dataSet.Tables[0]));
            Export(localReport);


        }

        private Stream CreateStream(string name, string fileNameExtension, Encoding encoding, string mimeType, bool willSeek)
        {
            String filePdf = "";
            int i = 1;
            do
            {
                if (File.Exists("StampaPreventivazioneRapida" + i + ".pdf"))
                {
                    i++;
                    try
                    {
                        FileStream fs = File.Open("StampaPreventivazioneRapida" + (i - 1) + ".pdf", FileMode.Open, FileAccess.Read, FileShare.None);
                        fs.Close();
                        File.Delete("StampaPreventivazioneRapida" + (i - 1) + ".pdf");
                    }
                    catch
                    {

                    }
                }
                else
                {
                    filePdf = "StampaPreventivazioneRapida"+i+".pdf";
                    i = 0;
                }
            } while (i != 0);

            FileStream stream = new FileStream(filePdf, FileMode.Create);


            m_streams.Add(stream);
            return stream;
        }

        private void Export(LocalReport report)
        {


            string deviceInfo =
              "<DeviceInfo>" +
              "  <OutputFormat>PDF</OutputFormat>" +
              "</DeviceInfo>";
            Warning[] warnings;
            m_streams = new List<FileStream>();
            report.Render("PDF", deviceInfo, CreateStream, out warnings);

            foreach (Stream stream in m_streams)
            {
                stream.Position = 0;
            }

            m_streams[0].Close();

            string nomePdf = m_streams[0].Name;


            try
            {
                string nomeFoxit = @"Foxit Reader.exe";
                if (!File.Exists(nomeFoxit))
                    nomeFoxit = @"FoxitReader.exe";

                //SE IL PARAMETRO DELLA STAMPANTE NN VIENE PASSATO O è VUOTO STAMPERà SULLA PREDEFINITA
                string sArgs = "\"" + nomePdf + "\"";
                System.Diagnostics.ProcessStartInfo startInfo = new ProcessStartInfo();
                startInfo.FileName = nomeFoxit;
                startInfo.Arguments = sArgs;
                startInfo.CreateNoWindow = true;
                startInfo.WindowStyle = ProcessWindowStyle.Hidden;
                System.Diagnostics.Process proc;
                if (Setting.Istance.FoxitReader == "TRUE")
                {
                    proc = Process.Start(startInfo);
                }
                else
                {
                    proc = Process.Start(nomePdf);
                }

                //p.WaitForExit();

                //ATTENDO FINO A QUANDO IL FILE NON SARà PIU BLOCCATO
                FileInfo fileInfo = new FileInfo(nomePdf);


                FileStream stream = null;
                bool isLocked = true;
                bool wasLocked = false;

                while (isLocked == true)
                {
                    try
                    {
                        stream = fileInfo.Open(FileMode.Open, FileAccess.Read, FileShare.None);
                        stream.Close();

                        //PRIMA DI STABILIRE SE IL FILE è SBLOCCATO,  CONTROLLO ANCHE CHE PRIMA SIA MAI STATO BLOCCATO (ALTIRMENTI SIGNIFICA CHE è SBLOCCATO MA SOLO PERCHE ANCORA NON è STATO DEL TUTTO APERTO E QUINDI LOCKATO)
                        if (wasLocked == true)
                        {
                            //IL FILE NON é PIU BLOCCATO
                            isLocked = false;
                        }
                    }
                    catch (IOException)
                    {
                        isLocked = true;
                        wasLocked = true;
                    }

                    //ATTENDI .. SECONDI PER NON CICLARE TROPPE VOLTE INUTILMENTE
                    Thread.Sleep(1500);
                }
                proc.Close();
                proc.Dispose();
            }

            catch(Exception e)
            {
                MessageBox.Show(e.Message);
                Environment.Exit(1);
            }


            File.Delete(nomePdf);
        }
    }
}
