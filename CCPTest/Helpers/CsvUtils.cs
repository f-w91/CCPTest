using CCP.DTO;
using Microsoft.VisualBasic.FileIO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CCP.Helpers
{
    public static class CsvUtils
    {
        public static CCPDataSet Read(string importFilePath)
        {
            CCPDataSet set = new CCPDataSet();
            if (File.Exists(importFilePath))
            {

                //List<List<CCPDataItem>> allItems = new List<List<CCPDataItem>>();
                using (TextFieldParser parser = new TextFieldParser(importFilePath))
                {
                    parser.SetDelimiters(new string[] { "," });

                    string[] headers = parser.ReadFields();

                    while (!parser.EndOfData)
                    {
                        //List<CCPDataItem> items = new List<CCPDataItem>();
                        var row = new CCPDataRow();
                        string[] properties = parser.ReadFields();
                        int index = 0;
                        foreach (var p in properties)
                        {
                            var headerName = headers[index];


                            var header = new CCPDataItem();
                            header.Name = headers[index];
                            header.Value = p;
                            header.OrderIndex = index;
                            row.Items.Add(header);
                            index++;
                        }
                        set.Rows.Add(row);
                    }
                }

            }

            return set;
        }

        public static void Convert(CCPDataSet set, string exportFilePath)
        {
            var headers = set.Rows.First().Items.Select(x => x.Name).ToList();

            StringBuilder sb = new StringBuilder();
            sb.AppendLine(string.Join(',', headers));

            foreach (var row in set.Rows)
            {
                sb.AppendLine(string.Join(',', row.Items.Select(x => x.Value)));
            }

            var csvString = sb.ToString();

            using FileStream fs = File.OpenWrite(exportFilePath);

            byte[] bytes = Encoding.UTF8.GetBytes(csvString);

            fs.Write(bytes, 0, bytes.Length);
        }

    }
}
