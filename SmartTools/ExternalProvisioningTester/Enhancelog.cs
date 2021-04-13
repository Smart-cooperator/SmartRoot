using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ExternalProvisioningTester
{
    public class Enhancelog
    {

        private BlockingCollection<LogItem> logItemCollection;

        private ManualResetEvent exitEvent = new ManualResetEvent(false);

        public Enhancelog()
        {
            logItemCollection = new BlockingCollection<LogItem>(new ConcurrentQueue<LogItem>());
        }

        public void AppendLine(string line, bool error = false)
        {
            Append($"{line}{Environment.NewLine}", error);
        }

        public void Append(string msg, bool error = false)
        {
            if (logItemCollection != null && !logItemCollection.IsAddingCompleted)
            {
                logItemCollection.Add(new LogItem(msg, error));
            }
        }

        public void AppendException(Exception ex)
        {
            StringBuilder msgSB = new StringBuilder();
            msgSB.AppendLine(string.Empty);
            msgSB.AppendLine("Tester exited with an error...");
            msgSB.AppendLine(ex.Message);
            msgSB.AppendLine("Stack Trace:");
            msgSB.AppendLine(ex.StackTrace);
            Append(msgSB.ToString(), true);
        }

        public void Start()
        {
            Action action = new Action(InternalStart);
            action.BeginInvoke(null, null);
        }

        private void InternalStart()
        {
            foreach (var item in logItemCollection.GetConsumingEnumerable())
            {
                try
                {
                    if (item.IsError)
                    {
                        Console.Error.Write(item.Context);
                    }
                    else
                    {
                        Console.Write(item.Context);
                    }
                }
                catch (Exception)
                {

                }
                finally
                {

                }
            }

           exitEvent.Set();
        }

        public void Close()
        {
            logItemCollection.CompleteAdding();
        }

        public void Exit()
        {
            Close();
            exitEvent.WaitOne();
        }
    }

    public class LogItem
    {
        public string Context { get; set; }

        public bool IsError { get; set; }

        public LogItem(string context, bool isError)
        {
            Context = context;
            IsError = isError;
        }
    }
}
