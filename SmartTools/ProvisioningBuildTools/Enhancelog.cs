using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProvisioningBuildTools
{
    public class Enhancelog
    {
        private RichTextBox richTextBox;
        private BlockingCollection<LogItem> logItemCollection;

        public Enhancelog(RichTextBox richTextBox)
        {
            this.richTextBox = richTextBox;

            logItemCollection = new BlockingCollection<LogItem>(new ConcurrentQueue<LogItem>());
        }

        public void AppenLine(string line, bool error = false)
        {
            AppenLine(line, error ? Color.Red : Color.White);
        }

        public void AppenLine(string line, Color color)
        {
            if (logItemCollection != null && !logItemCollection.IsAddingCompleted)
            {
                logItemCollection.Add(new LogItem(line, color));
            }
        }

        public void Start()
        {
            Action action = new Action(InternalStart);
            action.BeginInvoke(null, null);
        }

        private void InternalStart()
        {
            bool tempFinished = false;
            DateTime tempDateTime = DateTime.Now;            

            foreach (var item in logItemCollection.GetConsumingEnumerable())
            {

                try
                {
                    if (tempFinished)
                    {
                        tempDateTime = DateTime.Now;
                        tempFinished = false;
                    }

                    Action action = () =>
                    {

                        string line = item.Context;
                        Color color = item.Color;

                        line = $"{line}{Environment.NewLine}";                        

                        if (richTextBox.Lines.Count() > 10000)
                        {
                            richTextBox.Clear();
                        }

                        int start = richTextBox.TextLength - 1;

                        richTextBox.AppendText(line);

                        int end = richTextBox.TextLength - 1;

                        richTextBox.SelectionStart = start + 1;
                        richTextBox.SelectionLength = end - start;
                        richTextBox.SelectionColor = color;

                        richTextBox.SelectionStart = richTextBox.TextLength;

                        richTextBox.ScrollToCaret();

                    };

                    IAsyncResult asyncResult = richTextBox.BeginInvoke(action);

                    asyncResult.AsyncWaitHandle.WaitOne();

                    if (logItemCollection.Count == 0)
                    {
                        tempFinished = true;
                    }

                    if (DateTime.Now.Subtract(tempDateTime).TotalSeconds >= 3)
                    {
                        Thread.Sleep(50);
                        tempDateTime = DateTime.Now;
                    }
                }
                catch (Exception)
                {
                    richTextBox.Clear();
                }
                finally
                {
                    //Application.DoEvents();
                }
            }
        }

        public void Close()
        {
            logItemCollection.CompleteAdding();
        }
    }

    public class LogItem
    {
        public string Context { get; set; }
        public Color Color { get; set; }

        public LogItem(string context, Color color)
        {
            Context = context;
            Color = color;
        }
    }
}
