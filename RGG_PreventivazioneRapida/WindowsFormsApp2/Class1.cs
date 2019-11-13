using PreventivazioneRapida;
using System;
using System.Collections.Generic;
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
        // The constructor obtains the state information.
        public ThreadWithState(string padre, int livello, Form1 form)
        {
            boilerplate = padre;
            numberValue = livello;
            f = form;
        }

        // The thread procedure performs the task, such as formatting
        // and printing a document.
        public void ThreadProc()
        {
            f.EsplodiDistintaBase(boilerplate, numberValue);            
        }
    }
}
