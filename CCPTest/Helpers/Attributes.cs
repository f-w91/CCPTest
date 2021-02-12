using System;
using System.Collections.Generic;
using System.Text;

namespace CCP.Helpers
{
    //Dont want to rely on text, have one extension defined everywhere
    public class ExtensionAttribute: Attribute
    {
        public string Name { get; private set; }

        public ExtensionAttribute(string name)
        {
            this.Name = name;
        }
    }
}
