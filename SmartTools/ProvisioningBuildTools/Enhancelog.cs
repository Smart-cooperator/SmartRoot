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
            bool logBufferFinished = false;
            bool logNeedClear = false;
            DateTime tempDateTime = DateTime.Now;
            StringBuilder logBuffer = new StringBuilder();
            Color lastColor = Color.White;
            int logBufferLines = 0;
            int logCurrentLines = 0;

            foreach (var item in logItemCollection.GetConsumingEnumerable())
            {

                try
                {
                    if (tempFinished)
                    {
                        tempDateTime = DateTime.Now;
                        tempFinished = false;
                    }

                    string line = item.Context;
                    Color color = item.Color;

                    //line = $"{line}{Environment.NewLine}";

                    if (logItemCollection.Count == 0)
                    {
                        tempFinished = true;
                    }

                    if (logBufferLines + logCurrentLines > 10000)
                    {
                        logNeedClear = true;
                    }

                    if (!logNeedClear && (logBuffer.Length == 0 || lastColor == color))
                    {
                        logBuffer.AppendLine(line);
                        logBufferLines++;
                        lastColor = color;
                    }
                    else
                    {
                        logBufferFinished = true;
                    }

                    Action action = () =>
                    {
                        if (logNeedClear)
                        {
                            richTextBox.Clear();
                            logBuffer.Clear();
                            logBufferLines = 0;
                            logCurrentLines = 0;
                        }
                        else
                        {
                            int start = richTextBox.TextLength - 1;

                            richTextBox.AppendText(logBuffer.ToString());
                            logCurrentLines = richTextBox.Lines.Count();
                            logBuffer.Clear();
                            logBufferLines = 0;

                            int end = richTextBox.TextLength - 1;

                            richTextBox.SelectionStart = start + 1;
                            richTextBox.SelectionLength = end - start;
                            richTextBox.SelectionColor = lastColor;

                            richTextBox.SelectionStart = richTextBox.TextLength;

                            richTextBox.ScrollToCaret();
                        }
                    };

                    if (tempFinished || logBufferFinished || logNeedClear)
                    {
                        richTextBox.Invoke(action);

                        if (tempFinished && logItemCollection.Count != 0)
                        {
                            tempFinished = false;
                        }

                        if (logBufferFinished)
                        {
                            logBuffer.AppendLine(line);
                            logBufferLines++;
                            lastColor = color;

                            if (tempFinished)
                            {
                                richTextBox.Invoke(action);
                            }

                            logBufferFinished = false;
                        }

                        if (logNeedClear)
                        {
                            logNeedClear = false;
                        }
                    }

                    if (DateTime.Now.Subtract(tempDateTime).TotalSeconds >= 3)
                    {
                        Thread.Sleep(1);
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
