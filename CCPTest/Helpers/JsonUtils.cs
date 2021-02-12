using CCP.DTO;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Text;

namespace CCP.Helpers
{
    public static class JsonUtils
    {
        public static CCPDataSet Read(string importFilePath)
        {
            CCPDataSet set = new CCPDataSet();

            if (File.Exists(importFilePath))
            {
                //Read out our json into a new set for interrogation
                var entries = JsonConvert.DeserializeObject<List<Dictionary<string, object>>>(File.ReadAllText(importFilePath));

                foreach (var entry in entries)
                {
                    CCPDataRow row = new CCPDataRow();

                    foreach (var field in entry)
                    {
                        if (field.Value.ToString().StartsWith("{") && field.Value.ToString().EndsWith("}"))
                        {
                            //We're looking at a collection here
                            var nest = JsonConvert.DeserializeObject<Dictionary<string, string>>(field.Value.ToString());

                            foreach (var n in nest)
                            {
                                CCPDataItem item = new CCPDataItem();
                                item.Name = $"{field.Key}_{n.Key}";
                                item.Value = n.Value.ToString();
                                row.Items.Add(item);
                            }
                        }
                        else
                        {
                            //Basic item, nothing fancy
                            CCPDataItem item = new CCPDataItem();
                            item.Name = field.Key;
                            item.Value = field.Value.ToString();
                            row.Items.Add(item);
                        }
                    }

                    set.Rows.Add(row);
                }

            }

            return set;
        }


        public static void Convert(CCPDataSet set, string exportFilePath)
        {
            List<IDictionary<string, object>> outputSet = new List<IDictionary<string, object>>();

            //Each row
            foreach (var row in set.Rows)
            {
                IDictionary<string, object> parent = new ExpandoObject();
                //Each property in the row
                foreach (var subItem in row.Items)
                {
                    if (subItem.Name.Contains('_'))
                    {
                        var splitted = subItem.Name.Split('_');
                        var group = splitted[0];
                        var groupedItem = splitted[1];

                        //We're using ExpandoObjects since we can dynamically add properties as indexed entries, wrapping them as IDictionary types
                        IDictionary<string, object> child = new ExpandoObject();

                        try
                        {
                            child = ((IDictionary<string, object>)parent[group]);
                            child.Add(groupedItem, subItem.Value);
                        }
                        catch (Exception)
                        {
                            child[groupedItem] = subItem.Value;
                        }

                        parent[group] = child;
                    }
                    else
                    {
                        parent[subItem.Name] = subItem.Value;
                    }

                }

                outputSet.Add(parent);

            }

            string jsonString = JsonConvert.SerializeObject(outputSet);

            using FileStream fs = File.OpenWrite(exportFilePath);

            byte[] bytes = Encoding.UTF8.GetBytes(jsonString);

            fs.Write(bytes, 0, bytes.Length);
        }

    }
}
