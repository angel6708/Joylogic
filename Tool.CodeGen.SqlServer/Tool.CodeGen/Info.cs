using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq; 
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace Tool.CodeGen
{
    public class Info
    {
        private XmlDocument xml;
        private string filename;

        public XmlDocument doc { get { return xml; } }
        public String FileName { get { return filename; } set { filename = value; } }

        public Info() { this.xml = new XmlDocument(); }
        public Info(XmlDocument xml) { this.xml = xml; }
        public Info(string filename)
        {
            this.filename = filename;
            xml = new XmlDocument();
            try
            {
                xml.Load(filename);
            }
            catch (Exception)
            {
                RemoveAll();
            }
        }
        public bool HasValues
        {//is file loaded?? 
            get { return xml.DocumentElement.ChildNodes.Count > 0; }
        }
        public XmlNode AppendChild(XmlNode nd, string name)
        {
            return nd.AppendChild(xml.CreateElement(name, nd.OwnerDocument.DocumentElement.NamespaceURI));
        }
        public XmlNode AppendChild(string name)
        {
            return xml.DocumentElement.AppendChild(xml.CreateElement(name, xml.DocumentElement.NamespaceURI));
        }
        public void RemoveAll() { xml.RemoveAll(); xml.LoadXml("<p/>"); }
        public ArrayList ArrayList(string name)
        {
            ArrayList lst = new ArrayList();
            XmlNodeList nds = xml.DocumentElement.SelectNodes(name);
            if (nds != null) foreach (XmlNode nd in nds) lst.Add(nd.FirstChild.Value);
            return lst;
        }
        public void Save()
        {
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            XmlWriter xw;
            xw = XmlWriter.Create(filename, settings);

            xml.WriteTo(xw);
            xw.Close();
        }
        public string Value(string name)
        {
            return Value(xml.DocumentElement, name);
        }
        public bool Bool(string name)
        {
            return Bool(name, false);
        }
        public bool Bool(string name, bool def)
        {
            string v = Value(name);
            return (v == String.Empty) ? def : bool.Parse(v);
        }
        public string Value(XmlNode nd, string name)
        {
            XmlNode nv;
            if ((nv = nd.SelectSingleNode(name)) != null && nv.FirstChild != null)
                return nv.FirstChild.Value;
            return String.Empty;
        }

        public XmlNode SetValue(String name) { return SetValue(xml.DocumentElement, name, null); }
        public XmlNode SetValue(String name, object o) { return SetValue(xml.DocumentElement, name, o); }

        public XmlNode SetValue(XmlNode nd, String name, object o)
        {
            String val = o == null ? String.Empty : o.ToString();
            XmlNode elem = nd.SelectSingleNode(name);
            if (elem == null)
                elem = nd.OwnerDocument.CreateElement(name, nd.OwnerDocument.DocumentElement.NamespaceURI);
            elem.RemoveAll();
            if (val != String.Empty)
            {

                if (val.IndexOfAny(new char[] { '&', '>', '<', '"' }) < 0)
                    elem.AppendChild(nd.OwnerDocument.CreateTextNode(val));
                else
                    elem.AppendChild(nd.OwnerDocument.CreateCDataSection(val));
            }
            nd.AppendChild(elem);
            return elem;

        }

        public XmlNode AddValue(String name)
        { return AddValue(xml.DocumentElement, name, null); }

        public XmlNode AddValue(XmlNode nd, String name)
        { return AddValue(nd, name, null); }

        public XmlNode AddValue(XmlNode nd, String name, object o)
        {
            XmlNode elem = nd.OwnerDocument.CreateElement(name, nd.OwnerDocument.DocumentElement.NamespaceURI);
            if (o != null && (o is XmlNode))
            {
                nd.InsertAfter(elem, (XmlNode)o);
            }
            else
            {
                String val = o == null ? String.Empty : o.ToString();
                if (val != String.Empty)
                {
                    if (val.IndexOfAny(new char[] { '&', '>', '<', '"' }) < 0)
                        elem.AppendChild(nd.OwnerDocument.CreateTextNode(val));
                    else
                        elem.AppendChild(nd.OwnerDocument.CreateCDataSection(val));
                }
                nd.AppendChild(elem);
            }
            return elem;
        }

        public XmlAttribute AddAtt(string name, string val)
        {
            return AddAtt(null, name, val);
        }

        public XmlAttribute AddAtt(XmlNode nd, string name, string val)
        {
            if (val == String.Empty) return null;
            if (nd == null) nd = xml.DocumentElement;
            XmlAttribute att = nd.OwnerDocument.CreateAttribute(name);
            att.Value = val;
            nd.Attributes.Append(att);
            return att;
        }


        public static string RemoveSeparatorAndCapNext(string input)
        {
            string dashUnderscore = "-_";
            string workingString = input;
            char[] chars = workingString.ToCharArray();
            int under = workingString.IndexOfAny(dashUnderscore.ToCharArray());
            while (under > -1)
            {
                chars[under + 1] = Char.ToUpper(chars[under + 1], CultureInfo.InvariantCulture);
                workingString = new String(chars);
                under = workingString.IndexOfAny(dashUnderscore.ToCharArray(), under + 1);
            }
            chars[0] = Char.ToUpper(chars[0], CultureInfo.InvariantCulture);
            workingString = new string(chars);
            return Regex.Replace(workingString, "[-_]", "");

        }
        public static int IndexOfCol(Column c)
        {
          return 0;
        }

    }
}
