﻿using System;
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
                sqlserverConn.Close();               
            }
            catch
            {
                MessageBox.Show("ERRORE nell'esecuzione della query '" + query + "'");
            }                      
        }

        public int VerificaSemilavorato(string articolo)
        {
            SqlDataAdapter da;
            sqlserverConn.Open();
            string distintaBase = Setting.Istance.QueryDistintaBase.Replace("@CodDistBase", articolo);
            da = new SqlDataAdapter(distintaBase, sqlserverConn);
            int count = da.Fill(ds.Tables["DistintaBase"]);
            sqlserverConn.Close();

            return count;
        }

        public void InsertPreventivo(string []valoriTestata)
        {
            try
            {
                sqlserverConn.Open();
                int idpreventivo = 0, idpreventivoass = 0;
                string queryTestata = "INSERT INTO preventivi (cliente, articolo, quantita, variazione, totale, totalevar, datacreazione, note) VALUES ('" 
                    + valoriTestata[0] + "', '" + valoriTestata[1] + "', " + valoriTestata[2].Replace(',','.') + ", " + valoriTestata[3].Replace(',', '.') + ", " + valoriTestata[4].Replace(',', '.') + ", " 
                    + valoriTestata[5].Replace(',', '.') + ", CURRENT_TIMESTAMP, '" + valoriTestata[6] + "')";
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
                    string queryRighi = "INSERT INTO preventivirighi (idpreventivo, rowindex, codicepadre, codiceart, codicelav, descrizione, quantita, setupmac, setupuomo, tempomac, tempouomo, costoart" +
                    ", costoattmac, costoattuomo, costomac, costouomo, totale, totalevar) VALUES (" + idpreventivo + ", '" + row["rowindex"].ToString() + "', '" + row["CODICE_PADRE"].ToString() + "', '" +
                    row["CODICE ART"].ToString() + "', '" + row["Codice lav"].ToString() + "', '" + row["Descrizione art / Centro di Lavoro"].ToString() + "', " + row["Quantita`"].ToString().Replace(',', '.') +
                    ", " + row["Setup Mac"].ToString().Replace(',', '.') + ", " + row["Setup Uomo"].ToString().Replace(',', '.') + ", " + row["Tempo Mac"].ToString().Replace(',', '.') + ", " + row["Tempo Uomo"].ToString().Replace(',', '.') +
                    ", " + row["Costo Art"].ToString().Replace(',', '.') + ", " + row["Costo Att Mac"].ToString().Replace(',', '.') + ", " + row["Costo Att Uomo"].ToString().Replace(',', '.') + ", " + row["Costo Mac"].ToString().Replace(',', '.') +
                    ", " + row["Costo Uomo"].ToString().Replace(',', '.') + ", " + row["Totale"].ToString().Replace(',', '.') + ", " + row["Totale + %Var"].ToString().Replace(',', '.') + ")";
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
            string query = "select cliente, articolo, quantita, note, variazione, datacreazione from preventivi where id = '" + idpreventivo + "'";
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
                string query = "SELECT rowindex,codicepadre as CODICE_PADRE,codiceart AS 'Codice Art', codicelav AS 'Codice Lav', descrizione AS 'Descrizione art / Centro di Lavoro'," +
                    "quantita AS 'Quantita`', setupmac AS 'Setup Mac', setupuomo AS 'Setup Uomo', tempomac AS 'Tempo Mac', tempouomo AS 'Tempo Uomo', costoart AS 'Costo Art', " +
                    "costoattmac AS 'Costo Att Mac', costoattuomo AS 'Costo Att Uomo'," +
                    "costomac AS 'Costo Mac', costouomo AS 'Costo Uomo', totale AS 'Totale', totalevar AS 'Totale + %Var' FROM preventivirighi WHERE idpreventivo = (SELECT id FROM(SELECT (ROW_NUMBER() OVER(ORDER BY id)) as rowindex, id FROM preventivi" +
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
    }
}