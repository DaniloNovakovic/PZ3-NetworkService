using System;
using System.IO;
using System.Windows;
using System.Xml;
using System.Xml.Serialization;

namespace PZ3_NetworkService
{
    public class DataIO
    {
        public bool SerializeObject<T>(T serializableObject, string fileName)
        {
            if (serializableObject == null) { return false; }

            try
            {
                var xmlDocument = new XmlDocument();
                var serializer = new XmlSerializer(serializableObject.GetType());
                using (var stream = new MemoryStream())
                {
                    serializer.Serialize(stream, serializableObject);
                    stream.Position = 0;
                    xmlDocument.Load(stream);
                    xmlDocument.Save(fileName);
                    stream.Close();
                }
                return true;
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message);
            }
            return false;
        }

        public T DeSerializeObject<T>(string fileName)
        {
            //if (string.IsNullOrEmpty(fileName)) { return default(T); }

            var objectOut = default(T);

            string attributeXml = string.Empty;

            var xmlDocument = new XmlDocument();
            xmlDocument.Load(fileName);
            string xmlString = xmlDocument.OuterXml;

            using (var read = new StringReader(xmlString))
            {
                var outType = typeof(T);

                var serializer = new XmlSerializer(outType);
                using (XmlReader reader = new XmlTextReader(read))
                {
                    objectOut = (T)serializer.Deserialize(reader);
                    reader.Close();
                }

                read.Close();
            }

            return objectOut;
        }
    }
}