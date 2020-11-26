using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Utilities;

namespace frmTest
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            //Command.OpenReposSln("wae");           

            //Command.CommandResult commandResult = Command.CreatePacakge("wae",false);

            //Command.CommandResult commandResult = Command.RunWitheWDK(null, @"cd c:\1234", false, waitHandle);

            AutoResetEvent waitHandle = new AutoResetEvent(false);

            Func<CommandResult>[] funcs = new Func<CommandResult>[] { () => { return Command.Init("wae", false, waitHandle, true); }, () => { return Command.UpdateExternalDrops("wae", false, waitHandle, true); } };

            for (int i = 0; i < funcs.Length; i++)
            {
                NewMethod(waitHandle, funcs[i]);
            }

           
        }

        private static void NewMethod(AutoResetEvent waitHandle, Func<CommandResult> func)
        {
            Task<CommandResult> task = new Task<CommandResult>(func);

            task.Start();

            while (!task.Wait(TimeSpan.FromSeconds(3)))
            {
                if (MessageBox.Show("请确认结果", string.Empty, MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    waitHandle.Set();
                }
            }

            CommandResult commandResult = task.Result;
        }
    }
}
