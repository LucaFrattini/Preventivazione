using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PreventivazioneRapida
{
    public class Model
    {
        public DataSet ds;
        //private DataTable Articoli, Clienti;
        private static SqlConnection sqlserverConn;

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
                //Articolo articolo = new Articolo(reader.GetValue(0).ToString(), reader.GetValue(1).ToString(), reader.GetValue(2).ToString(),
                //    reader.GetValue(3).ToString(), reader.GetValue(4).ToString());
                //listaArticoli.Add(articolo);
                //Articoli = listaArticoli.ToArray();
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
            }
            catch
            {
                MessageBox.Show("Errore nella lettura dei clienti! La query deve essere all'interno del nodo" +
                    "<Configuration><AllQuery><Cliente>");
            }
        }

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
                    //ds.Tables["Articoli"].Columns[Setting.istance.PKArticolo].DataType = Type.GetType("System.String");
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
                    ds.Tables["DistintaBase"].Columns.Add("setup mac decimale");
                    ds.Tables["DistintaBase"].Columns.Add("setup uomo decimale");
                    ds.Tables["DistintaBase"].Columns.Add("tempo mac decimale");
                    ds.Tables["DistintaBase"].Columns.Add("tempo uomo decimale");
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

        public int VerificaSemilavorato(DataRow row)
        {
            SqlDataAdapter da;
            sqlserverConn.Open();
            string distintaBase = Setting.Istance.QueryDistintaBase.Replace("@CodDistBase", row["CODICE ART"].ToString());
            da = new SqlDataAdapter(distintaBase, sqlserverConn);
            int count = da.Fill(ds.Tables["DistintaBase"]);
            sqlserverConn.Close();
            /*if (count == 0)
            {
                string rowindex = row["rowindex"].ToString();
                
                string rowindexfiglio = rowindex + ",1";
                int countvirgole = FindNumberOfChar(',', rowindexfiglio);
                foreach (DataRow dr in ds.Tables["DistintaBase"].Rows)
                {
                    string prova = dr["rowindex"].ToString().Substring(0, rowindex.Length);
                    int countvirgolefiglio = FindNumberOfChar(',', dr["rowindex"].ToString());
                    if (countvirgole == countvirgolefiglio && rowindex == dr["rowindex"].ToString().Substring(0,rowindex.Length))
                    {
                        count++;
                    }
                }
            }*/
            
            return count;
        }

        public void InsertPreventivo(string []valoriTestata)
        {
            try
            {
                sqlserverConn.Open();
                int idpreventivo = 0, idpreventivoass = 0;
                string queryTestata = "INSERT INTO preventivi (cliente, articolo, quantita, variazione, variazionelav, totale, totalevar, datacreazione, note) VALUES ('" 
                    + valoriTestata[0] + "', '" + valoriTestata[1] + "', " + valoriTestata[2].Replace(',','.') + ", " + valoriTestata[3].Replace(',', '.') + ", " + valoriTestata[4].Replace(',', '.') + ", " 
                    + valoriTestata[5].Replace(',', '.') + ", " + valoriTestata[6].Replace(',', '.') + ", CURRENT_TIMESTAMP, '" + valoriTestata[7] + "')";
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

                foreach (DataRow row in ds.Tables["DistintaBase"].Rows)
                {
                    string queryRighi = "INSERT INTO preventivirighi (idpreventivo, rowindex, codicepadre, codiceart, codicecentro, codicelav, descrizione, quantita, setupmac, setupuomo, tempomac, tempouomo, costoart" +
                    ", costoattmac, costoattuomo, costomac, costouomo, totale, totalevar, setupmacdec, setupuomodec, tempomacdec, tempouomodec) VALUES (" + idpreventivo + ", '" + row["rowindex"].ToString() + "', '" + row["CODICE_PADRE"].ToString() + "', '" +
                    row["CODICE ART"].ToString() + "', '" +row["Codice centro"].ToString() + "', '" + row["Codice lav"].ToString() + "', '" + row["Descrizione art / Centro di Lavoro"].ToString() + "', '" + row["Quantita`"].ToString() +
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
            catch
            {
                MessageBox.Show("Errore durante il salvataggio del preventivo. Controllare la connessione con il server.");
                sqlserverConn.Close();
            }
            return;
        }

        public Dictionary<int,int> IDUltimoPreventivo(string cliente)
        {
            Dictionary<int, int> IDPreventivi = new Dictionary<int, int>();
            try
            {
                
                string nometabella = "Preventivi";
                string query = "SELECT (ROW_NUMBER() OVER(ORDER BY id)) as rownumber, * FROM preventivi WHERE cliente='" + cliente + "'";
                EstraiRisultatoQuery(query, nometabella);
                DataTable dt = ds.Tables["Preventivi"];
                //query = "SELECT MAX(rownumber) FROM (SELECT (ROW_NUMBER() OVER(ORDER BY id)) as rownumber, * FROM preventivi WHERE cliente='" + cliente + "') AS preventiviMAXID";

                //query per selezionare gli id dei preventivi, sia assoluto che relativi al singolo cliente; la esegue e inserisce i risultati in un dictionary
                query = "SELECT rownumber, id FROM (SELECT (ROW_NUMBER() OVER(ORDER BY id)) as rownumber, id FROM preventivi WHERE cliente='" + cliente + "' group by id) AS preventiviMAXID group by rownumber, id order by rownumber DESC";
                sqlserverConn.Open();
                using (SqlCommand cmd = new SqlCommand(query, sqlserverConn))
                {
                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        IDPreventivi.Add(Int32.Parse(dr[0].ToString()), Int32.Parse(dr[1].ToString()));
                    }
                    //MaxIDPreventivo.Add(Int32.Parse(dr[0].ToString()), Int32.Parse(dr[1].ToString()));
                    dr.Close();
                }
                sqlserverConn.Close();
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
                //MessageBox.Show("Errore durante la connessione al database.");
            }
            return IDPreventivi;//maxID;
        }

        public List<string> OttieniTestata(string idpreventivo)
        {
            List<string> testata = new List<string>();
            string query = "select cliente, articolo, quantita, note, variazione, variazionelav, datacreazione from preventivi where id = '" + idpreventivo + "'";
            sqlserverConn.Open();
            using (SqlCommand cmd = new SqlCommand(query, sqlserverConn))
            {
                SqlDataReader dr = cmd.ExecuteReader();
                dr.Read();
                testata.Add(dr[0].ToString());
                testata.Add(dr[1].ToString());
                testata.Add(dr[2].ToString());
                testata.Add(dr[3].ToString());
                testata.Add(dr[4].ToString());
                testata.Add(dr[5].ToString());
                testata.Add(dr[6].ToString());
                dr.Close();
            }
            sqlserverConn.Close();
            return (testata);
        }


        public void CaricaPreventivoRighi(string idpreventivo, string cliente)
        {
            try
            {
                SqlDataAdapter da;
                sqlserverConn.Open();
                string query = "SELECT rowindex,codicepadre as CODICE_PADRE,codiceart AS 'Codice Art', codicecentro AS 'Codice Centro', codicelav AS 'Codice Lav', descrizione AS 'Descrizione art / Centro di Lavoro'," +
                    "quantita AS 'Quantita`', setupmac AS 'Setup Mac', setupuomo AS 'Setup Uomo', tempomac AS 'Tempo Mac', tempouomo AS 'Tempo Uomo', costoart AS 'Costo Art', " +
                    "costoattmac AS 'Costo Att Mac', costoattuomo AS 'Costo Att Uomo'," +
                    "costomac AS 'Costo Mac', costouomo AS 'Costo Uomo', totale AS 'Totale', totalevar AS 'Totale + %Var', setupmacdec AS 'setup mac decimale', setupuomodec AS 'setup uomo decimale'," +
                    " tempomacdec AS 'tempo mac decimale', tempouomodec AS 'tempo uomo decimale'  FROM preventivirighi WHERE idpreventivo = (SELECT id FROM(SELECT (ROW_NUMBER() OVER(ORDER BY id)) as rowindex, id FROM preventivi" +
                    " WHERE cliente = '"+cliente+"') AS clientepreventivi WHERE rowindex = "+Int32.Parse(idpreventivo)+")";
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

        public void FromDecimalToTime()
        {          
            foreach (DataRow dr in ds.Tables["DistintaBase"].Rows)
            {
                try
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
                catch { }
            }           
        }

        public void FromTimeToDecimal()
        {
            foreach (DataRow dr in ds.Tables["DistintaBase"].Rows)
            {
                try
                {
                    double tempoDecimale = Double.Parse(dr["setup Mac"].ToString());
                    int ore = Int32.Parse(Math.Floor(tempoDecimale).ToString());
                    tempoDecimale -= ore;
                    tempoDecimale = tempoDecimale * 100;
                    double minuti = Int32.Parse(Math.Floor(tempoDecimale).ToString());
                    tempoDecimale = Math.Round((tempoDecimale - minuti) * 100, 2);
                    double secondi = Int32.Parse(Math.Floor(tempoDecimale).ToString());
                    secondi = Math.Round(secondi + (minuti * 60));
                    double tempo = secondi / 3600;
                    dr["setup mac decimale"] = (ore + tempo).ToString();

                    tempoDecimale = Double.Parse(dr["setup uomo"].ToString());
                    ore = Int32.Parse(Math.Floor(tempoDecimale).ToString());
                    tempoDecimale -= ore;
                    tempoDecimale = tempoDecimale * 100;
                    minuti = Int32.Parse(Math.Floor(tempoDecimale).ToString());
                    tempoDecimale = Math.Round((tempoDecimale - minuti) * 100, 2);
                    secondi = Int32.Parse(Math.Floor(tempoDecimale).ToString());
                    secondi = Math.Round(secondi + (minuti * 60));
                    tempo = secondi / 3600;
                    dr["setup uomo decimale"] = (ore + tempo).ToString();

                    tempoDecimale = Double.Parse(dr["tempo mac"].ToString());
                    ore = Int32.Parse(Math.Floor(tempoDecimale).ToString());
                    tempoDecimale -= ore;
                    tempoDecimale = tempoDecimale * 100;
                    minuti = Int32.Parse(Math.Floor(tempoDecimale).ToString());
                    tempoDecimale = Math.Round((tempoDecimale - minuti) * 100, 2);
                    secondi = Int32.Parse(Math.Floor(tempoDecimale).ToString());
                    secondi = Math.Round(secondi + (minuti * 60));
                    tempo = secondi / 3600;
                    dr["tempo mac decimale"] = (ore + tempo).ToString();

                    tempoDecimale = Double.Parse(dr["tempo uomo"].ToString());
                    ore = Int32.Parse(Math.Floor(tempoDecimale).ToString());
                    tempoDecimale -= ore;
                    tempoDecimale = tempoDecimale * 100;
                    minuti = Int32.Parse(Math.Floor(tempoDecimale).ToString());
                    tempoDecimale = Math.Round((tempoDecimale - minuti) * 100, 2);
                    secondi = Int32.Parse(Math.Floor(tempoDecimale).ToString());
                    secondi = Math.Round(secondi + (minuti * 60));
                    tempo = secondi / 3600;
                    dr["tempo uomo decimale"] = (ore + tempo).ToString();
                }
                catch { }               
            }
        }

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

        public void GestisciLavorazioneEsterna(DataRow rowLavorazione, string articolo)
        {
            rowLavorazione["Codice Art"] = rowLavorazione["Descrizione art / Centro di Lavoro"].ToString();
            string query = Setting.Istance.QueryLavorazioneEsterna.Replace("@LavorazioneCentro", rowLavorazione["Codice Centro"].ToString());
            query = query.Replace("@LavorazioneEsterna", articolo);
            sqlserverConn.Open();
            string prezzo;
            using (SqlCommand cmd = new SqlCommand(query, sqlserverConn))
            {
                SqlDataReader dr = cmd.ExecuteReader();
                dr.Read();
                prezzo = dr[0].ToString();
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
}
