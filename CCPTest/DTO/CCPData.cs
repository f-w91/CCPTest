using System;
using System.Collections.Generic;
using System.Text;

namespace CCP.DTO
{
    //Container class for the below DTOs. Just makes life easier.
    public class CCPDataSet
    {
        public List<CCPDataRow> Rows { get; set; } = new List<CCPDataRow>();
    }

    //Each of these represents either:
    //A row of csv data
    //A JSON object entry
    //A top level XML node
    public class CCPDataRow
    {
        public List<CCPDataItem> Items { get; set; } = new List<CCPDataItem>();
    }

    //The most basic, unified representation of our data. All types can be converted into this, and this type can be converted into all types.
    public class CCPDataItem
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public int OrderIndex { get; set; }
    }
}
