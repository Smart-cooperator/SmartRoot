using ProvisioningBuildTools.SelectOutput;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;
using Utilities;

namespace ProvisioningBuildTools.SelectInput
{
    public class SelectPackagesInfoInput
    {
        //private List<Package> m_Packages;
        //public List<Package> Packages => m_Packages;

        //public SelectPackagesInfoInput()
        //{
        //    m_Packages = new List<Package>();
        //}    

        public List<Package> Packages { get; set; }

        public SelectPackagesInfoInput()
        {
            Packages = new List<Package>();
        }
    }
}
