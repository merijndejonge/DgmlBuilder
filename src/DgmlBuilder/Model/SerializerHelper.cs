using System.Linq;
using System.Xml;
using System.Xml.Serialization;

namespace OpenSoftware.DgmlTools.Model
{
    public static class SerializerHelper
    {
        internal static void ToXml<T>(this T obj, XmlWriter writer) where T : ICustomPropertiesProvider
        {
            var properties = typeof(T).GetProperties();
            var serializeableProperties =
                properties.Where(x => x.GetCustomAttributes(typeof(XmlAttributeAttribute), true).Any());

            foreach (var serializeableProperty in serializeableProperties)
            {
                var value = serializeableProperty.GetValue(obj);
                if (serializeableProperty.PropertyType.IsValueType || value != null)
                {
                    writer.WriteAttributeString(serializeableProperty.Name, value.ToString());
                }
            }

            foreach (var dataElement in obj.Properties)
            {
                writer.WriteAttributeString(dataElement.Key, dataElement.Value.ToString());
            }
        }
    }
}