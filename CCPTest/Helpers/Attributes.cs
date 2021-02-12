using System;
using System.Collections.Generic;
using System.Text;

namespace CCP.Helpers
{
    public class ExtensionAttribute: Attribute
    {
        public string Name { get; private set; }

        public ExtensionAttribute(string name)
        {
            this.Name = name;
        }
    }
}
