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
            Command.Run(@"C:\Users\v-fengzhou\source\repos\paloma", @"CreatePackage.cmd Debug", out int exitCode, out string standOutput);

            Console.WriteLine($"exitCode: {exitCode}");
            Console.WriteLine($"standOutput: {standOutput}");

            Console.ReadKey();
        }
    }
}
