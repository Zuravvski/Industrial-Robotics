using System.Xml;

namespace Driver
{
    public interface IXMLObject
    {
        XmlElement ToXML();
    }
}
