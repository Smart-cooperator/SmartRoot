using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Utilities;

namespace ProvisioningBuildTools
{
    public class BackGroundCommand
    {
        private readonly object isBusyLock = new object();
        private readonly object cancellationTokenSourceLock = new object();

        private bool m_IsBusy;
        public bool IsBusy
        {
            get
            {
                lock (isBusyLock)
                {
                    return m_IsBusy;
                }
            }
        }

        private CancellationTokenSource m_cancellationTokenSource;

        public void Abort()
        {
            lock (isBusyLock)
            {
                if (m_IsBusy)
                {
                    lock (cancellationTokenSourceLock)
                    {
                        if (m_cancellationTokenSource != null && m_cancellationTokenSource.IsCancellationRequested == false)
                        {
                            m_cancellationTokenSource.Cancel();
                        }
                    }
                }
                else
                {
                    throw new Exception("there is no command running!!!");
                }
            }
        }

        public IAsyncResult AsyncRun(Action runAct, Action startInvoke, Action endInvoke, CancellationTokenSource cancellationTokenSource, ILogNotify logNotify)
        {
            lock (isBusyLock)
            {
                if (!m_IsBusy)
                {
                    lock (cancellationTokenSourceLock)
                    {
                        if (m_cancellationTokenSource != null)
                        {
                            m_cancellationTokenSource.Dispose();
                        }

                        m_cancellationTokenSource = cancellationTokenSource;
                    }

                    m_IsBusy = true;

                    Action internalRun = new Action(() =>
                    {
                        try
                        {
                            startInvoke?.Invoke();
                            runAct.Invoke();
                        }
                        catch (Exception ex)
                        {
                            logNotify?.WriteLog(ex);
                        }
                        finally
                        {
                            lock (isBusyLock)
                            {
                                m_IsBusy = false;
                            }
                        }
                    });

                    return internalRun.BeginInvoke(new AsyncCallback(iAsyncResult => { endInvoke?.Invoke(); }), null);
                }
                else
                {
                    throw new Exception("there is a command running,can not duplicate run!!!");
                }
            }
        }
    }
}
