using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

namespace PreventivazioneRapida
{
    class Setting
    {
        /// Classe singleton che maniene un'unica istanza nell'intero programma con le configurazioni del programma

        public static Setting istance;

        public static Setting Istance
        {
            get
            {
                if (istance == null)
                {
                    istance = new Setting();
                }
                return istance;
            }
        }

        /// apre il file RilTempConfiguration.xml e legge tutte le chiavi presenti, sono presenti tutta la configurazione esternamente permettendo massima configurabilità
        private Setting()
        {
            string fileConfig = "PreventivazioneRapidaConfig.xml";
            XmlDocument xmlDoc;
            try
            {
                xmlDoc = new XmlDocument();
                xmlDoc.Load(fileConfig);
                this.ip = xmlDoc.SelectSingleNode("configuration/ip").InnerText;
                this.port = xmlDoc.SelectSingleNode("configuration/port").InnerText;
                this.database = xmlDoc.SelectSingleNode("configuration/database").InnerText;
                this.user = xmlDoc.SelectSingleNode("configuration/user").InnerText;
                this.password = xmlDoc.SelectSingleNode("configuration/password").InnerText;

                this.q1 = xmlDoc.SelectSingleNode("configuration/Quantita/q1").InnerText;
                this.q2 = xmlDoc.SelectSingleNode("configuration/Quantita/q2").InnerText;
                this.q3 = xmlDoc.SelectSingleNode("configuration/Quantita/q3").InnerText;

                this.queryArticolo = xmlDoc.SelectSingleNode("configuration/AllQuery/Articolo/Query").InnerText;
                this.pkArticolo = xmlDoc.SelectSingleNode("configuration/AllQuery/Articolo/PK").InnerText;
                this.queryCliente = xmlDoc.SelectSingleNode("configuration/AllQuery/Cliente/Query").InnerText;
                this.pkCliente = xmlDoc.SelectSingleNode("configuration/AllQuery/Cliente/PK").InnerText;
                this.queryLavorazione = xmlDoc.SelectSingleNode("configuration/AllQuery/Lavorazione/Query").InnerText;
                this.pkLavorazione = xmlDoc.SelectSingleNode("configuration/AllQuery/Lavorazione/PK").InnerText;
                this.queryCodDistBase = xmlDoc.SelectSingleNode("configuration/AllQuery/CodDistintaBase").InnerText;
                this.queryDistintaBase = xmlDoc.SelectSingleNode("configuration/AllQuery/DistintaBase").InnerText;
                this.queryLavorazioneEsterna = xmlDoc.SelectSingleNode("configuration/AllQuery/LavorazioneEsterna").InnerText;
                this.queryCercaArticolo = xmlDoc.SelectSingleNode("configuration/AllQuery/CercaArticolo").InnerText;
                this.queryCercaLavorazione = xmlDoc.SelectSingleNode("configuration/AllQuery/CercaLavorazione").InnerText;
                this.queryCercaCentro = xmlDoc.SelectSingleNode("configuration/AllQuery/CercaCentro").InnerText;
                this.queryCercaPreventivo = xmlDoc.SelectSingleNode("configuration/AllQuery/CercaPreventivo").InnerText;

                this.campiModificabili = xmlDoc.SelectSingleNode("configuration/CampiModificabili");

                this.helpCliente = xmlDoc.SelectSingleNode("configuration/helpCliente").InnerText;
                this.helpArticolo = xmlDoc.SelectSingleNode("configuration/helpArticolo").InnerText;
                this.helpLavorazione = xmlDoc.SelectSingleNode("configuration/helpLavorazione").InnerText;
                this.helpLavorazioneEsterna = xmlDoc.SelectSingleNode("configuration/helpLavorazioneEsterna").InnerText;
                this.helpPreventivo = xmlDoc.SelectSingleNode("configuration/helpPreventivo").InnerText;
                this.helpCentro = xmlDoc.SelectSingleNode("configuration/helpCentro").InnerText;
                this.helpCentroEsterno = xmlDoc.SelectSingleNode("configuration/helpCentroEsterno").InnerText;



                if (xmlDoc.SelectSingleNode("configuration/FL") != null)
                {
                    this.fontlabel = xmlDoc.SelectSingleNode("configuration/FL").InnerText;
                }


                if (xmlDoc.SelectSingleNode("configuration/FO") != null)
                {
                    this.font = xmlDoc.SelectSingleNode("configuration/FO").InnerText;
                }
            }
            catch
            {
                MessageBox.Show("Impossibile leggere correttamente i dati dal file di configurazione \"PreventivazioneRapidaConfig.xml\"!");
            }     
        }



        public string ConnStr
        {
            get
            {
                //< ConnectionSqlserver > Server = LUCA; Database = PCM_NEW; User Id = sa; Password = X$agilis;</ ConnectionSqlserver >
                return "Server=" + this.ip + /*";Port=" + this.port + */";Database=" + this.database + ";User Id=" + this.user + ";Password=" + this.password + ";";// CommandTimeout=0;";
            }
        }


        private string ip;
        public string Ip { get { return this.ip; } }

        private string port;
        public string Port { get { return this.port; } }

        private string user;
        public string User { get { return this.user; } }

        private string database;
        public string Database { get { return this.database; } }

        private string password;
        public string Password { get { return this.password; } }

        private string q1;
        public string Q1 { get { return this.q1; } }

        private string q2;
        public string Q2 { get { return this.q2; } }

        private string q3;
        public string Q3 { get { return this.q3; } }

        private string queryArticolo;
        public string QueryArticolo { get { return this.queryArticolo; } }

        private string pkArticolo;
        public string PKArticolo { get { return this.pkArticolo; } }

        private string queryCliente;
        public string QueryCliente { get { return this.queryCliente; } }

        private string pkCliente;
        public string PKCliente { get { return this.pkCliente; } }

        private string queryLavorazione;
        public string QueryLavorazione { get { return this.queryLavorazione; } }

        private string pkLavorazione;
        public string PKLavorazione { get { return this.pkLavorazione; } }

        private string queryCodDistBase;
        public string QueryCodDistBase { get { return this.queryCodDistBase; } }

        private string queryLavorazioneEsterna;
        public string QueryLavorazioneEsterna { get { return this.queryLavorazioneEsterna; } }

        private string queryCercaArticolo;
        public string QueryCercaArticolo { get { return this.queryCercaArticolo; } }

        private string queryCercaLavorazione;
        public string QueryCercaLavorazione { get { return this.queryCercaLavorazione; } }

        private string queryCercaCentro;
        public string QueryCercaCentro { get { return this.queryCercaCentro; } }

        private string queryCercaPreventivo;
        public string QueryCercaPreventivo { get { return this.queryCercaPreventivo; } }

        private XmlNode campiModificabili;
        public XmlNode CampiModificabili { get { return this.campiModificabili; } }

        private string queryDistintaBase;
        public string QueryDistintaBase { get { return this.queryDistintaBase; } }

        private string helpCliente;
        public string HelpCliente { get { return this.helpCliente; } }

        private string helpArticolo;
        public string HelpArticolo { get { return this.helpArticolo; } }

        private string helpLavorazione;
        public string HelpLavorazione { get { return this.helpLavorazione; } }

        private string helpLavorazioneEsterna;
        public string HelpLavorazioneEsterna { get { return this.helpLavorazioneEsterna; } }

        private string helpPreventivo;
        public string HelpPreventivo { get { return this.helpPreventivo; } }

        private string helpCentro;
        public string HelpCentro { get { return this.helpCentro; } }

        private string helpCentroEsterno;
        public string HelpCentroEsterno { get { return this.helpCentroEsterno; } }

        private string fontlabel = "";
        public string FontLabel { get { return this.fontlabel; } }

        private string font = "";
        public string Font { get { return this.font; } }
    }
}
