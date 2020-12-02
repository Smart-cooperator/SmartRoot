using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace ProvisioningBuildTools
{
    public interface ISelect<T> where T : class, new()
    {
        ILogNotify LogNotify { get; set; }
        ICommandNotify CommandNotify { get; set; }
        T SelectResult { get; }   
    }
}
