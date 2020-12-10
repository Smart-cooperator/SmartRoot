using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Utilities;

namespace Test
{
    class Program
    {
        static void Main(string[] args)
        {
            //Command.Run(@"C:\Users\v-fengzhou\source\repos\wae", @"CreatePackage.cmd Debug", out int exitCode, out string standOutput, out string errorOutput);

            //Console.WriteLine($"exitCode:{Environment.NewLine}{exitCode}");
            //Console.WriteLine($"standOutput:{Environment.NewLine}{standOutput}");

            //Command.RunOneWDK("","",out int x,out string y);

            string s1 = @"<ProductKeyID>sssdsadasd</ProductKeyID>";
            string s2 = @"<ProductKeyID>(?<OA3Key>\S+)</ProductKeyID>";
            Match m = Regex.Match(s1, s2);

            Console.ReadKey();
        }
    }
}
