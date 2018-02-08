using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Xml;
using System.Xml.Serialization;

namespace OpenSoftware.DgmlTools.Model
{
    public static class SerializerHelper
    {
        private static void WriteXmlAttributes(XmlWriter writer, object obj)
        {
            var xmlAttributes = GetXmlAttributes(obj);
            foreach (var xmlAttribute in xmlAttributes)
            {
                var value = xmlAttribute.GetValue(obj);
                if (xmlAttribute.PropertyType.IsValueType || value != null)
                {
                    writer.WriteAttributeString(xmlAttribute.Name, value.ToString());
                }
            }
        }
        private static void WriteXmlElements(XmlWriter writer, object obj)
        {
            var xmlElements = GetXmlElements(obj);
            foreach (var xmlElement in xmlElements)
            {
                var value = xmlElement.GetValue(obj);
                if (!xmlElement.PropertyType.IsValueType && value == null) continue;
                var attribute = (XmlElementAttribute)xmlElement.GetCustomAttributes(typeof(XmlElementAttribute), true).SingleOrDefault();
                if (attribute == null)
                    continue;
                if (value is IEnumerable list)
                {
                    foreach (var element in list)
                    {
                        var elementName = attribute.ElementName ?? element.GetType().Name;
                        writer.WriteStartElement(elementName);
                        WriteXmlAttributes(writer, element);
                        WriteXmlElements(writer, element);
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

        private static IEnumerable<PropertyInfo> GetXmlAttributes(object obj)
        {
            var properties = obj.GetType().GetProperties();
            return 
                properties.Where(x => x.GetCustomAttributes(typeof(XmlAttributeAttribute), true).Any()).ToArray();
        }

        private static IEnumerable<PropertyInfo> GetXmlElements(object obj)
        {
            var properties = obj.GetType().GetProperties();
            return
                properties
                .Where(x => x.GetCustomAttributes(typeof(XmlAttributeAttribute), true).Any() == false)
                .Where(x => x.GetCustomAttributes(typeof(XmlIgnoreAttribute), true).Any() == false)
                .ToArray();
        }

        internal static void ToXml<T>(this T obj, XmlWriter writer) where T : ICustomPropertiesProvider
        {
            WriteXmlAttributes(writer, obj);
            foreach (var dataElement in obj.Properties)
            {
                writer.WriteAttributeString(dataElement.Key, dataElement.Value.ToString());
            }
            WriteXmlElements(writer, obj);
        }
    }
}