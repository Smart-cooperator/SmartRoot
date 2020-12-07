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
        private readonly object cancellationTokenSourceLockForKill = new object();

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

        private CancellationTokenSource m_cancellationTokenSourceForKill;

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

        public void Kill()
        {
            lock (isBusyLock)
            {
                if (m_IsBusy)
                {
                    lock (cancellationTokenSourceLockForKill)
                    {
                        if (m_cancellationTokenSourceForKill != null && m_cancellationTokenSourceForKill.IsCancellationRequested == false)
                        {
                            m_cancellationTokenSourceForKill.Cancel();
                        }
                    }
                }
                else
                {
                    throw new Exception("there is no command running!!!");
                }
            }
        }

        public IAsyncResult AsyncRun(Action actRun, Action startInvoke, Action endInvoke, CancellationTokenSource cancellationTokenSource, ILogNotify logNotify, CancellationTokenSource cancellationTokenSourceForKill = null)
        {
            return AsyncRun(new Func<CommandResult>[] { new Func<CommandResult>(() => { actRun.Invoke(); return default(CommandResult); }) }, startInvoke, endInvoke, cancellationTokenSource, logNotify, cancellationTokenSourceForKill);
        }

        public IAsyncResult AsyncRun(Func<CommandResult> func, Action startInvoke, Action endInvoke, CancellationTokenSource cancellationTokenSource, ILogNotify logNotify, CancellationTokenSource cancellationTokenSourceForKill = null)
        {
            return AsyncRun(new Func<CommandResult>[] { func }, startInvoke, endInvoke, cancellationTokenSource, logNotify, cancellationTokenSourceForKill);
        }

        public IAsyncResult AsyncRun(Func<CommandResult>[] funcs, Action startInvoke, Action endInvoke, CancellationTokenSource cancellationTokenSource, ILogNotify logNotify, CancellationTokenSource cancellationTokenSourceForKill = null)
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

                    lock (cancellationTokenSourceLockForKill)
                    {
                        if (m_cancellationTokenSourceForKill != null)
                        {
                            m_cancellationTokenSourceForKill.Dispose();
                        }

                        m_cancellationTokenSourceForKill = cancellationTokenSourceForKill;
                    }

                    m_IsBusy = true;

                    Action internalRun = new Action(() =>
                    {
                        try
                        {
                            startInvoke?.Invoke();

                            CommandResult commandResult;

                            foreach (var func in funcs)
                            {
                                commandResult = func.Invoke();

                                if (!string.IsNullOrEmpty(commandResult.CommandName))
                                {
                                    if (commandResult.ExitCode == 0)
                                    {
                                        logNotify?.WriteLog($"Exec command {commandResult.CommandName} successfully!!!");
                                    }
                                    else
                                    {
                                        throw new Exception($"Exec command {commandResult.CommandName} failed!!!");
                                    }
                                }
                            }
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

                    return internalRun.BeginInvoke(
                        new AsyncCallback(iAsyncResult =>
                    {
                        try
                        {
                            endInvoke?.Invoke();
                        }
                        catch (Exception ex)
                        {
                            logNotify?.WriteLog(ex);
                        }
                    }), null);
                }
                else
                {
                    throw new Exception("there is a command running,can not duplicate run!!!");
                }
            }
        }
    }
}
