using CCP.DTO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;

namespace CCP.Helpers
{
    public static class XmlUtils
    {

        public static CCPDataSet Read(string importFilePath)
        {
            CCPDataSet set = new CCPDataSet();

            if (File.Exists(importFilePath))
            {
                //Load xml into memory
                XmlDocument doc = new XmlDocument();
                doc.Load(importFilePath);

                foreach (XmlNode n in doc.DocumentElement)
                {
                    CCPDataRow row = new CCPDataRow();
                    foreach (XmlNode r in n.ChildNodes)
                    {
                        //If we have child nodes, and those child nodes are actually xml elements, that means we want to dig deeper
                        if (r.HasChildNodes && r.ChildNodes.OfType<XmlElement>().Any())
                        {
                            foreach (XmlNode cc in r.ChildNodes)
                            {
                                CCPDataItem item = new CCPDataItem();
                                item.Name = $"{r.Name}_{cc.Name}";
                                item.Value = cc.InnerText;
                                row.Items.Add(item);
                            }
                        }
                        else
                        {
                            //Otherwise, we're as deep as we want to be
                            CCPDataItem item = new CCPDataItem();
                            item.Name = r.Name;
                            item.Value = r.InnerText;
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
            //Set up some settings to make our XML nicer
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.OmitXmlDeclaration = true;
            settings.IndentChars = "\t";
            settings.Indent = true;
            settings.CloseOutput = true;

            string xmlString;

            using (StringWriter sw = new StringWriter())
            {
                using (XmlWriter writer = XmlWriter.Create(sw))
                {
                    //Container element
                    writer.WriteStartElement("row");

                    foreach (var row in set.Rows)
                    {
                        //Another container element
                        writer.WriteStartElement("item");

                        bool openElement = false;
                        for (int i = 0; i < row.Items.Count; i++)
                        {
                            var subItem = row.Items[i];

                            //If the name has a hyphen, we want to have a nested item
                            if (subItem.Name.Contains('_'))
                            {
                                //Split out the name into the parent/child relationship
                                var split = subItem.Name.Split('_');
                                var group = split[0];
                                var groupedItem = split[1];

                                //If we're just starting this nest, open up an appropriate element
                                if (!openElement)
                                {
                                    writer.WriteStartElement(group);
                                    openElement = true;
                                }

                                //Slam the info in
                                writer.WriteElementString(groupedItem, subItem.Value);

                                //If we have more items to go, check the next one
                                if (i < row.Items.Count - 1)
                                {
                                    var nextItem = row.Items[i + 1];

                                    if (nextItem.Name.Contains('_') && nextItem.Name.Split('_')[0] == group)
                                    {
                                        //The next item is also nested and belongs to this
                                    }
                                    else if (openElement)
                                    {
                                        //We're dealing with a different element or nest, so we can close this element, if its open
                                        writer.WriteEndElement();
                                        openElement = false;
                                    }
                                }
                                else if (openElement)
                                {
                                    //We're at the end of a row and need to end the element if it's open
                                    writer.WriteEndElement();
                                    openElement = false;
                                }

                            }
                            else
                            {
                                //In this case, just a plain element
                                writer.WriteElementString(subItem.Name, subItem.Value);
                            }
                        }

                        writer.WriteEndElement();
                    }

                    writer.WriteEndElement();
                }

                xmlString = sw.ToString();
            }

            using FileStream fs = File.OpenWrite(exportFilePath);
            byte[] bytes = Encoding.UTF8.GetBytes(xmlString);
            fs.Write(bytes, 0, bytes.Length);
        }

    }
}
