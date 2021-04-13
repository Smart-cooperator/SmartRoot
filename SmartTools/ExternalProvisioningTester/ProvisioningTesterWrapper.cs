using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ExternalProvisioningTester
{
    public class ProvisioningTesterWrapper
    {
        private IProvisioningTesterHandler provisioningTesterHandler;
        private Enhancelog log;

        public ProvisioningTesterWrapper(IProvisioningTesterHandler provisioningTesterHandler, Enhancelog log)
        {
            this.provisioningTesterHandler = provisioningTesterHandler;
            this.log = log;
        }

        public int Start(string path)
        {

            int exitCode = int.MinValue;

            ManualResetEvent exitPutWaitHandle = new ManualResetEvent(false);
            ManualResetEvent standOutputWaitHandle = new ManualResetEvent(false);
            ManualResetEvent errorOutputWaitHandle = new ManualResetEvent(false);

            BlockingCollection<char> stringBuffer = new BlockingCollection<char>(new ConcurrentQueue<char>());
            BlockingCollection<char> stringBufferError = new BlockingCollection<char>(new ConcurrentQueue<char>());

            using (Process p = new Process())
            {
                p.StartInfo.FileName = path;

                p.StartInfo.UseShellExecute = false;
                p.StartInfo.RedirectStandardInput = true;

                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.RedirectStandardError = true;
                p.StartInfo.CreateNoWindow = true;

                p.StartInfo.Verb = "RunAs";
                p.StartInfo.WorkingDirectory = Path.GetDirectoryName(path);

                p.EnableRaisingEvents = true;

                //p.OutputDataReceived += new DataReceivedEventHandler(
                //    (object sender, DataReceivedEventArgs e) =>
                //    {
                //        if (e.Data == null)
                //        {
                //            standOutputWaitHandle.Set();
                //            return;
                //        }

                //        provisioningTesterHandler.HandelStandOutput(e.Data);

                //    });

                //p.ErrorDataReceived += new DataReceivedEventHandler(
                //    (object sender, DataReceivedEventArgs e) =>
                //    {
                //        if (e.Data == null)
                //        {
                //            errorOutputWaitHandle.Set();
                //            return;
                //        }

                //        provisioningTesterHandler.HandelErrorOutput(e.Data);
                //    });

                p.Exited += new EventHandler(
                    (object sender, EventArgs e) =>
                    {
                        exitPutWaitHandle.Set();
                    });


                p.Start();//启动程序               

                provisioningTesterHandler.ProvisioningTesterProcess = p;

                //p.BeginOutputReadLine();
                //p.BeginErrorReadLine();

                ReadOutPut(p, standOutputWaitHandle, errorOutputWaitHandle, stringBuffer, stringBufferError);
                provisioningTesterHandler.StartReadLine();

                exitPutWaitHandle.WaitOne();
                standOutputWaitHandle.WaitOne(100);
                errorOutputWaitHandle.WaitOne(100);
                stringBuffer.CompleteAdding();
                stringBufferError.CompleteAdding();

                provisioningTesterHandler.ProvisioningTesterProcess = null;

                //p.CancelErrorRead();
                //p.CancelOutputRead();

                exitCode = p.ExitCode;

                p.Close();
            }

            log.Exit();

            return exitCode;
        }

        private void ReadOutPut(Process p, EventWaitHandle standardOutputWaitHandle, EventWaitHandle errorOutputWaitHandle, BlockingCollection<char> stringBuffer, BlockingCollection<char> stringBufferError)
        {
            Action readStandardOutput = () =>
            {
                try
                {
                    while (true)
                    {
                        char[] buffer = new char[1];

                        int count;
                        count = p.StandardOutput.ReadBlock(buffer, 0, 1);

                        if (count <= 0)
                        {
                            stringBuffer.CompleteAdding();
                            break;
                        }
                        else
                        {
                            stringBuffer.Add(buffer[0]);
                        }
                    }
                }
                catch (Exception)
                {
                    stringBuffer.CompleteAdding();
                }
            };


            Action handleStringBuffer = () =>
              {
                  StringBuilder buffer = new StringBuilder();

                  foreach (var item in stringBuffer.GetConsumingEnumerable())
                  {
                      buffer.Append(item);
                      string temp = buffer.ToString();

                      if (temp.EndsWith(Environment.NewLine) || temp.EndsWith("\n"))
                      {
                          provisioningTesterHandler.HandelStandOutput(temp);
                          buffer.Clear();
                      }
                      else
                      {
                          if (stringBuffer.Count == 0)
                          {
                              provisioningTesterHandler.HandelStandOutput(temp);
                              buffer.Clear();
                          }
                      }
                  }

                  standardOutputWaitHandle.Set();
              };

            Action readStandardError = () =>
            {
                try
                {
                    while (true)
                    {
                        char[] buffer = new char[1];

                        int count;
                        count = p.StandardError.ReadBlock(buffer, 0, 1);

                        if (count <= 0)
                        {
                            stringBufferError.CompleteAdding();
                            break;
                        }
                        else
                        {
                            stringBufferError.Add(buffer[0]);
                        }

                    }
                }
                catch (Exception)
                {
                    stringBufferError.CompleteAdding();
                }
            };

            Action handleStringBufferError = () =>
            {
                StringBuilder buffer = new StringBuilder();

                foreach (var item in stringBufferError.GetConsumingEnumerable())
                {
                    buffer.Append(item);
                    string temp = buffer.ToString();

                    if (temp.EndsWith(Environment.NewLine) || temp.EndsWith("\n") || stringBufferError.Count == 0 )
                    {
                        provisioningTesterHandler.HandelErrorOutput(temp);
                        buffer.Clear();
                    }
                }

                errorOutputWaitHandle.Set();
            };

            readStandardOutput.BeginInvoke(null, null);
            readStandardError.BeginInvoke(null, null);
            handleStringBuffer.BeginInvoke(null, null);
            handleStringBufferError.BeginInvoke(null, null);
        }
    }

    public interface IProvisioningTesterHandler
    {

        Process ProvisioningTesterProcess { get; set; }

        void HandelStandOutput(string data);

        void HandelErrorOutput(string data);
        void StartReadLine();
    }
}
