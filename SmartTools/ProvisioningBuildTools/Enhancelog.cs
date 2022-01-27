using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProvisioningBuildTools
{
    public class Enhancelog
    {
        private RichTextBox richTextBox;
        private BlockingCollection<LogItem> logItemCollection;
        private int lastSelectionStart;
        private IEnhancelogNotify notify;

        public Enhancelog(RichTextBox richTextBox, IEnhancelogNotify notify)
        {
            this.richTextBox = richTextBox;
            this.notify = notify;
            richTextBox.SelectionChanged += richTextBox_SelectionChanged;
            richTextBox.KeyDown += richTextBox_KeyDown;
            richTextBox.FindForm().Activated += richTextBox_Activated;
            logItemCollection = new BlockingCollection<LogItem>(new ConcurrentQueue<LogItem>());
        }

        private void richTextBox_Activated(object sender, EventArgs e)
        {
            richTextBox.Refresh();
        }

        private void richTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyData == (Keys.V | Keys.Control))
            {
                e.Handled = true;

                if (logItemCollection.Count == 0)
                {
                    string[] lines = Clipboard.GetText().Split(new string[] { Environment.NewLine, @"\n" }, StringSplitOptions.None);

                    if (richTextBox.SelectionStart < lastSelectionStart)
                    {
                        richTextBox.AppendText(lines.FirstOrDefault() ?? string.Empty);
                    }
                    else
                    {
                        richTextBox.SelectedText = $"{lines.FirstOrDefault() ?? string.Empty}";
                    }

                    if (lines != null && lines.Length > 1)
                    {
                        SendKeys.Send("{Enter}");
                    }
                }
            }
            else
            {
                if (e.KeyCode == Keys.Up || e.KeyCode == Keys.Down)
                {
                    e.Handled = true;

                    if (logItemCollection.Count == 0)
                    {
                        string history = notify.GetHistory(e.KeyCode == Keys.Up);

                        if (!string.IsNullOrEmpty(history))
                        {
                            richTextBox.SelectionStart = lastSelectionStart;
                            richTextBox.SelectionLength = richTextBox.TextLength - lastSelectionStart;
                            richTextBox.SelectedText = history;
                            richTextBox.SelectionStart = richTextBox.TextLength;
                        }
                    }
                }

                if (e.KeyCode == Keys.Back)
                {
                    if (richTextBox.SelectionStart == lastSelectionStart && richTextBox.SelectionLength == 0)
                    {
                        e.Handled = true;
                    }
                }
                else if (e.KeyCode == Keys.Left)
                {
                    if (richTextBox.SelectionStart <= lastSelectionStart)
                    {
                        e.Handled = true;
                    }
                }
                else if (e.KeyCode == Keys.Enter)
                {
                    e.Handled = true;

                    if (richTextBox.SelectionStart >= lastSelectionStart)
                    {
                        if (logItemCollection.Count == 0)
                        {
                            int start = lastSelectionStart;
                            int end = richTextBox.TextLength;
                            int length = end - start;
                            string input = string.Empty;

                            if (length > 0)
                            {
                                richTextBox.SelectionStart = start;
                                richTextBox.SelectionLength = length;
                                input = richTextBox.SelectedText;
                            }                         

                            richTextBox.AppendText(Environment.NewLine);

                            lastSelectionStart = richTextBox.TextLength;
                            richTextBox.SelectionStart = richTextBox.TextLength;

                            notify.HandleInput(input);
                        }
                    }
                    else
                    {
                        richTextBox.SelectionStart = richTextBox.TextLength;
                    }
                }
            }
        }

        private void richTextBox_SelectionChanged(object sender, EventArgs e)
        {
            if (richTextBox.SelectionStart < lastSelectionStart)
            {
                richTextBox.ReadOnly = true;
            }
            else
            {
                richTextBox.ForeColor = Color.FromArgb(204, 204, 204);
                richTextBox.SelectionColor = Color.FromArgb(204, 204, 204);
                richTextBox.ReadOnly = false;
            }
        }

        public void AppendLine(string line, bool error = false)
        {
            Append($"{line}{Environment.NewLine}", error);
        }

        public void AppendLine(string line, Color color)
        {
            Append($"{line}{Environment.NewLine}", color);
        }

        public void Append(string msg, bool error = false)
        {
            Append(msg, error ? Color.Red : Color.FromArgb(204, 204, 204));
        }

        public void Append(string msg, Color color)
        {
            if (logItemCollection != null && !logItemCollection.IsAddingCompleted)
            {
                logItemCollection.Add(new LogItem(msg, color));
            }
        }

        public void Clear()
        {
            if (logItemCollection != null && !logItemCollection.IsAddingCompleted)
            {
                logItemCollection.Add(new LogItem(null, default(Color), true));
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
            //Color lastColor = Color.White;
            Color lastColor = Color.FromArgb(204, 204, 204);
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
                    bool clear = item.Clear;

                    if (logItemCollection.Count == 0)
                    {
                        tempFinished = true;
                    }

                    if (clear || logBufferLines + logCurrentLines > 10000)
                    {
                        logNeedClear = true;
                    }

                    if (!logNeedClear && (logBuffer.Length == 0 || lastColor == color))
                    {
                        logBuffer.Append(line);
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

                            lastSelectionStart = richTextBox.TextLength;

                            richTextBox.ScrollToCaret();
                        }
                    };

                    if (tempFinished || logBufferFinished || logNeedClear)
                    {
                        richTextBox.Invoke(action);

                        if (logNeedClear)
                        {
                            logNeedClear = false;
                        }

                        if (tempFinished && logItemCollection.Count != 0)
                        {
                            tempFinished = false;
                        }

                        if (logBufferFinished)
                        {
                            if (line != null)
                            {
                                logBuffer.Append(line);
                                logBufferLines++;
                                lastColor = color;
                            }

                            if (tempFinished && logBuffer.Length > 0)
                            {
                                richTextBox.Invoke(action);
                            }

                            logBufferFinished = false;
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

        public bool Clear { get; set; }

        public LogItem(string context, Color color, bool clear = false)
        {
            Context = context;
            Color = color;
            Clear = clear;
        }
    }

    public interface IEnhancelogNotify
    {
        string GetHistory(bool upOrDown);

        void HandleInput(string command);
    }
}
