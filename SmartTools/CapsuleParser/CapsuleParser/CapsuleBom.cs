//-------------------------------------------------------------------------------
//Copyright (c) Microsoft Corporation.  All rights reserved.
//-------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using Excel = Microsoft.Office.Interop.Excel;

namespace CapsuleParser
{
    public class CapsuleBom
    {
        private string[] _columns =
        {
            "Type",
            "Subsystem",
            "Name",
            "Ver #"
        };

        private Excel.Application _xlsApp = null;
        private Excel.Workbook _xlsWorkbook = null;
        private Excel.Worksheet _xlsWorksheet = null;

        public CapsuleBom(string xlsFile)
        {
            _xlsApp = new Excel.Application();
            _xlsWorkbook = _xlsApp.Workbooks.Open(xlsFile);
            _xlsWorksheet = _xlsWorkbook.Sheets[1];         // may need to iterate and select proper sheet
        }

        public Dictionary<string, string> GetCapsuleVersions()
        {
            // Excel row,col are 1 based
            int typeColumnNum = 0;
            int subsystemColumnNum = 0;
            int nameColumnNum = 0;
            int verColumnNum = 0;

            Dictionary<string, string> capsuleVersions = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            // Find the column numbers
            if (_xlsWorksheet != null)
            {
                string cellContent = null;
                int currentColumn = 1;

                do
                {
                    cellContent = _xlsWorksheet.Cells[1, currentColumn].Text;

                    switch (cellContent)
                    {
                        case "Type":
                            typeColumnNum = currentColumn++;
                            break;

                        case "Subsystem":
                            subsystemColumnNum = currentColumn++;
                            break;

                        case "Name of inf or bin file (anticipated)":
                            nameColumnNum = currentColumn++;
                            break;

                        case "Ver #":
                            verColumnNum = currentColumn++;
                            break;

                        default:
                            currentColumn++;
                            break;
                    }
                } while (!string.IsNullOrWhiteSpace(cellContent));

                int currentRow = 2; // first row after header
                string type = string.Empty;

                do
                {
                    type = _xlsWorksheet.Cells[currentRow, typeColumnNum].Text;

                    if (type == "Capsule")
                    {
                        string name = _xlsWorksheet.Cells[currentRow, nameColumnNum].Text;
                        string ver = _xlsWorksheet.Cells[currentRow, verColumnNum].Text;
                        capsuleVersions.Add(name, ver);
                    }
                    currentRow++;

                } while (!string.IsNullOrWhiteSpace(type));
            }
            return capsuleVersions;
        }
    }
}
