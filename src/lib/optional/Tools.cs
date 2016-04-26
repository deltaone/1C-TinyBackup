using System;
using System.Text;
using System.IO;
using System.Security.Cryptography;
using System.Xml;
using System.Xml.Serialization;
using System.Web.Script.Serialization;
using System.Text.RegularExpressions;
using Microsoft.Win32;

namespace Core
{
    public static class Tools
    {
        public static string DumpObjectToJSON(object data, string path = null)
        {   // add a reference to System.Web.Extensions / using System.Web.Script.Serialization;
            string dump;
            if (data.GetType() == typeof(string)) dump = (string)data;
            //using Newtonsoft.Json;
            //else dump = (string)(JsonConvert.SerializeObject(data, Newtonsoft.Json.Formatting.Indented, 
            //            new Newtonsoft.Json.JsonSerializerSettings { 
            //                   ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore,
            //                   PreserveReferencesHandling = Newtonsoft.Json.PreserveReferencesHandling.Objects
            //            }));
            else dump = FormatJSON((new JavaScriptSerializer()).Serialize(data));

            if (!(String.IsNullOrEmpty(path) || path.Trim().Length == 0)) File.WriteAllText(path, dump);            
            return (dump);
        }

        public static string DumpObjectToXML(object data)
        {
            if (data == null) throw new ArgumentNullException("data");

            var xs = new XmlSerializer(data.GetType());

            using (var memoryStream = new MemoryStream())
            using (var xmlTextWriter = new XmlTextWriter(memoryStream, new UTF8Encoding()))
            {
                xs.Serialize(xmlTextWriter, data);
                return Encoding.UTF8.GetString(memoryStream.ToArray());
            }
        }

        public static string FormatJSON(string jsonString)
        {
            var stringBuilder = new StringBuilder();

            bool escaping = false;
            bool inQuotes = false;
            int indentation = 0;

            foreach (char character in jsonString)
            {
                if (escaping)
                {
                    escaping = false;
                    stringBuilder.Append(character);
                }
                else
                {
                    if (character == '\\')
                    {
                        escaping = true;
                        stringBuilder.Append(character);
                    }
                    else if (character == '\"')
                    {
                        inQuotes = !inQuotes;
                        stringBuilder.Append(character);
                    }
                    else if (!inQuotes)
                    {
                        if (character == ',')
                        {
                            stringBuilder.Append(character);
                            stringBuilder.Append("\r\n");
                            stringBuilder.Append('\t', indentation);
                        }
                        else if (character == '[' || character == '{')
                        {
                            stringBuilder.Append(character);
                            stringBuilder.Append("\r\n");
                            stringBuilder.Append('\t', ++indentation);
                        }
                        else if (character == ']' || character == '}')
                        {
                            stringBuilder.Append("\r\n");
                            stringBuilder.Append('\t', --indentation);
                            stringBuilder.Append(character);
                        }
                        else if (character == ':')
                        {
                            stringBuilder.Append(character);
                            stringBuilder.Append('\t');
                        }
                        else
                        {
                            stringBuilder.Append(character);
                        }
                    }
                    else
                    {
                        stringBuilder.Append(character);
                    }
                }
            }
            return stringBuilder.ToString();
        }


        public static string GetMD5(string source)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] checkSum = md5.ComputeHash(Encoding.UTF8.GetBytes(source));
            return (BitConverter.ToString(checkSum).Replace("-", String.Empty));
        }

        public static string GetMD5FromFile(string path)
        {
            using (FileStream fs = System.IO.File.OpenRead(path))
            {
                MD5 md5 = new MD5CryptoServiceProvider();
                byte[] fileData = new byte[fs.Length];
                fs.Read(fileData, 0, (int)fs.Length);
                byte[] checkSum = md5.ComputeHash(fileData);
                return (BitConverter.ToString(checkSum).Replace("-", String.Empty));
            }
        }

        public static string XMLEscape(string unescaped)
        {
            XmlDocument doc = new XmlDocument();
            XmlNode node = doc.CreateElement("root");
            node.InnerText = unescaped;
            return (node.InnerXml);
        }

        public static string XMLUnescape(string escaped)
        {
            XmlDocument doc = new XmlDocument();
            XmlNode node = doc.CreateElement("root");
            node.InnerXml = escaped;
            return (node.InnerText);
        }

        public static string HTMLStripTags(string source)
        { // http://haacked.com/archive/2005/04/22/Matching_HTML_With_Regex.aspx/   
            return Regex.Replace(source, @"</?\w+((\s+\w+(\s*=\s*(?:"".*?""|'.*?'|[^'"">\s]+))?)+\s*|\s*)/?>",
                        string.Empty, RegexOptions.Singleline);
        }

        public static string RegistryGetValueString(string keyName, string keySection, string defaultValue = "")
        {
            RegistryKey registryKey = Registry.LocalMachine.OpenSubKey("Software\\" + keySection + "\\");
            if (registryKey == null) return (defaultValue);
            object value = registryKey.GetValue(keyName);
            if (value == null) return (defaultValue);
            return (value.ToString());
        }
    }
}
