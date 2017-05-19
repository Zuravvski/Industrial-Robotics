using System.Xml;

namespace IDE.Common.Utilities
{
    /// <summary>
    /// IXMLObject class
    /// </summary>
    public interface IXMLObject
    {
        /// <summary>
        /// To the XML.
        /// </summary>
        /// <returns></returns>
        XmlElement ToXML();
    }
}
