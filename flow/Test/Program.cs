using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            Command.Run(@"C:\Users\v-fengzhou\source\repos\wae", @"CreatePackage.cmd Debug", out int exitCode, out string standOutput);

            Console.WriteLine($"exitCode:{Environment.NewLine}{exitCode}");
            Console.WriteLine($"standOutput:{Environment.NewLine}{standOutput}");

            //Command.RunOneWDK("","",out int x,out string y);

            Console.ReadKey();
        }
    }
}
