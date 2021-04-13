using ProvisioningBuildTools.SelectInput;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProvisioningBuildTools.SelectOutput
{
    public class SelectLocalProjectOutput
    {
        private LocalProjectInfo m_SelectedLocalProjectInfo;
        public LocalProjectInfo SelectedLocalProjectInfo => m_SelectedLocalProjectInfo;


        private string[] m_SelectedPackagesList;
        public string[] SelectedPackagesList => m_SelectedPackagesList;

        private bool m_Append;
        public bool Append => m_Append;

        private string m_AppendList;
        public string AppendList => m_AppendList;

        public SelectLocalProjectOutput(LocalProjectInfo selectedLocalProjectInfo)
        {
            m_SelectedLocalProjectInfo = selectedLocalProjectInfo;
        }

        public SelectLocalProjectOutput(LocalProjectInfo selectedLocalProjectInfo, bool append, string appendList)
        {
            m_SelectedLocalProjectInfo = selectedLocalProjectInfo;
            m_Append = append;
            m_AppendList = appendList.Replace("Append:",string.Empty).Trim();
        }

        public SelectLocalProjectOutput(LocalProjectInfo selectedLocalProjectInfo, string[] selectedPackagesList, bool append, string appendList)
        {
            m_SelectedLocalProjectInfo = selectedLocalProjectInfo;
            m_SelectedPackagesList = selectedPackagesList;
            m_Append = append;
            m_AppendList = appendList.Replace("Append:", string.Empty).Trim();
        }




        public SelectLocalProjectOutput()
        {

        }
    }
}
