using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProvisioningBuildTools;
using ProvisioningBuildTools.CLI;

namespace ProvisioningBuildTools
{
    public interface ISelect<T> : ISelect,ICLISupprot where T : class, new()
    {
        T SelectResult { get; }
    }

    public interface ISelect
    {
        ILogNotify LogNotify { get; set; }
        ICommandNotify CommandNotify { get; set; }

        string CommandLine { get; }
    }

    public interface ICLISupprot
    {
        AbCLIExecInstance CLIInstance { get; set; }

        void CLIExec();
    }
}
