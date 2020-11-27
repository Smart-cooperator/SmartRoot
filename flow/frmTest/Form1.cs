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
    public partial class Form1 : Form, ICommandNotify, ILogNotify
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Command.OpenReposSln("wae_study");           

            //Command.CommandResult commandResult = Command.CreatePacakge("wae",false);

            //Command.CommandResult commandResult = Command.RunWitheWDK(null, @"cd c:\1234", false, waitHandle);

            //AutoResetEvent waitHandle = new AutoResetEvent(false);

            //Func<CommandResult>[] funcs = new Func<CommandResult>[] { () => { return Command.Init("wae", false, waitHandle, true); }, () => { return Command.UpdateExternalDrops("wae", false, waitHandle, true); } };

            //Func<CommandResult>[] funcs = new Func<CommandResult>[] { () => { return Command.GitTest("wae", false, waitHandle, true); }};

            //for (int i = 0; i < funcs.Length; i++)
            //{
            //    NewMethod(waitHandle, funcs[i]);
            //}

            //CommandResult commandResult = Command.GetLatestBranch("calgary");
            //int i = Thread.CurrentThread.ManagedThreadId;

            //CommandResult commandResult = Command.GitLog("wae", "origin/product/wae/main",true,null,false,OutPutEnum.Single,this);
            //CommandResult commandResult = Command.GitLog("wae", "origin/product/wae/main");
            //CommandResult commandResult = Command.CheckOutLatestBranch("calgary",this, this);

            //NewMethod1 (new Func<CommandResult>(()=> Command.CheckOutLatestBranch("wae", this, this,"study")));
          
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

        private static void NewMethod1(Func<CommandResult> func)
        {
            Task<CommandResult> task = new Task<CommandResult>(func);

            task.Start();

            //task.GetAwaiter().OnCompleted();

           // CheckOutedLatestBranch commandResult = task.Result;
        }

        public void WriteOutPut(int processId, string outputLine)
        {
            if (!this.InvokeRequired)
            {
                richTextBox1.AppendText($"ProcessId {processId}:{outputLine}{Environment.NewLine}");
            }
            else
            {
                this.BeginInvoke(new Action<int, string>(WriteOutPut), processId, outputLine);
            }
        }

        public void WriteError(int processId, string errorLine)
        {
            if (!this.InvokeRequired)
            {
                richTextBox1.AppendText($"ProcessId {processId}:{errorLine}{Environment.NewLine}");
            }
            else
            {
                this.BeginInvoke(new Action<int, string>(WriteError), processId,  errorLine);
            }
        }

        public void Exit(int processId, int exitCode)
        {
            if (!this.InvokeRequired)
            {
                richTextBox1.AppendText($"ProcessId {processId}:ExitCode {exitCode}{Environment.NewLine}");
            }
            else
            {
                this.BeginInvoke(new Action<int, int>(Exit), processId,  exitCode);
            }
        }

        public void WriteLog(string logLine)
        {
            if (!this.InvokeRequired)
            {
                richTextBox1.AppendText($"{logLine}{Environment.NewLine}");
            }
            else
            {
                this.BeginInvoke(new Action<string>(WriteLog), logLine);
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            richTextBox1.WordWrap = true;
        }
    }
}
