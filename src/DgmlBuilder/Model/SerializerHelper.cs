using System.Collections;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;

namespace OpenSoftware.DgmlTools.Model
{
    public static class SerializerHelper
    {
        private static void ToXmlGeneric(object obj, XmlWriter writer)
        {
            var properties = obj.GetType().GetProperties();
            var xmlAttributes =
                properties.Where(x => x.GetCustomAttributes(typeof(XmlAttributeAttribute), true).Any());

            foreach (var xmlAttribute in xmlAttributes)
            {
                var value = xmlAttribute.GetValue(obj);
                if (xmlAttribute.PropertyType.IsValueType || value != null)
                {
                    writer.WriteAttributeString(xmlAttribute.Name, value.ToString());
                }
            }
            var xmlElements = properties.Where(x => x.GetCustomAttributes(typeof(XmlElementAttribute), true).Any());
            foreach (var xmlElement in xmlElements)
            {
                var value = xmlElement.GetValue(obj);
                if (!xmlElement.PropertyType.IsValueType && value == null) continue;
                var attribute = (XmlElementAttribute)xmlElement.GetCustomAttributes(typeof(XmlElementAttribute), true).Single();
                if (value is IEnumerable list)
                {
                    foreach (var element in list)
                    {
                        var elementName = attribute.ElementName ?? element.GetType().Name;
                        writer.WriteStartElement(elementName);
                        ToXmlGeneric(element, writer);
                        writer.WriteEndElement();
                    }
                }
                else
                {
                    var elementName = attribute.ElementName ?? xmlElement.Name;
                    writer.WriteElementString(elementName, value.ToString());
                }
            }
        }
        internal static void ToXml<T>(this T obj, XmlWriter writer) where T : ICustomPropertiesProvider
        {
            ToXmlGeneric(obj, writer);

            foreach (var dataElement in obj.Properties)
            {
                writer.WriteAttributeString(dataElement.Key, dataElement.Value.ToString());
            }

        }
    }
}