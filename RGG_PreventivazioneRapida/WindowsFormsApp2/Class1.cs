using PreventivazioneRapida;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Preventivazione_RGG
{
    public partial class ThreadWithState
    {
        // State information used in the task.
        private string boilerplate;
        private int numberValue;
        private Form1 f;
        private DataRow padre;
        private readonly object connectionLock = new object();
        private Model m;
        private double quantitaNuova, precedenteQuantita, variazione, variazionelav;

        // The constructor obtains the state information.
        public ThreadWithState(string padre, int livello, Form1 form)
        {
            boilerplate = padre;
            numberValue = livello;
            f = form;
        }

        public ThreadWithState(DataRow datarow, Form1 form, Model model, double quantitaNuova, double precedenteQuantita, double variazione, double variazionelav)
        {
            padre = datarow;
            f = form;
            m = model;
            this.quantitaNuova = quantitaNuova;
            this.precedenteQuantita = precedenteQuantita;
            this.variazione = variazione;
            this.variazione = variazionelav;
        }

        // The thread procedure performs the task, such as formatting
        // and printing a document.
        public void ThreadProc()
        {
            f.EsplodiDistintaBase(boilerplate, numberValue);            
        }

        public void ThreadCambiaQuantita()
        {
            f.EsplodiDistintaBaseThread(padre);
        }
       
        public void AssociaImmagine()
        {
            f.EsplodiDistintaBaseThread(padre);
        }

    }
}
