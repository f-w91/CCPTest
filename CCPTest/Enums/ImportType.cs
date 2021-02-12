using CCP.Helpers;

namespace CCP.Enums
{
    //Don't rely on strings and magic numbers, standardise our expected file types
    public enum ImportType
    {
        None = 0,
        [Extension(".csv")]
        Csv,
        [Extension(".xml")]
        Xml,
        [Extension(".json")]
        Json
    }
}
