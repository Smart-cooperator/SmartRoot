using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace CapsuleParser
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("******************************************");
            Console.WriteLine("Copy right Microsoft Surface Provisioning Team.");
            Console.WriteLine("Example: CapsuleParser.exe -Type TextParser -ConfigFile CapsuleInfoConfiguration.xml -CapsuleFolder C:\\Documents\\Capsules");
            Console.WriteLine("Example: CapsuleParser.exe -Type XmlParser -ConfigFile CapsuleInfoConfiguration.xml -InfFile C:\\Documents\\Capsules\\SurfaceSAM.inf -BinFile C:\\Documents\\Capsules\\SurfaceSAM_1.8.139.bin");
            Console.WriteLine("******************************************");

            try
            {
                Dictionary<string, string> argTable = Utility.Parse(args);

                if (argTable.ContainsKey("Type"))
                {
                    string parserType = argTable["Type"];

                    if(parserType.Contains("TextParser") == true)
                    {
                        TextParser.ParseCapsules(argTable);
                    }
                    else
                    {
                        XmlParser.ParseCapsules(argTable);
                    }
                }
                             
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error with messsage：{0}", ex.Message);
            }
        }
    }
}
