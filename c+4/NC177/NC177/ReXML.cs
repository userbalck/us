using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace NC177
{
    public class ReXML
    {
		private XmlElement _root;
		public XmlElement Root { get { return (_root); } set { this._root = value; } }
		public string XML { get { return (this.Doc.InnerXml); } set { this._doc.LoadXml(value); } }
		private FileInfo _xmlfile;
		public FileInfo XMLFile { get { return (this._xmlfile); } }
		private XmlDocument _doc;
		public XmlDocument Doc { get { return (this._doc); } }
		public RMXML(string xml, bool isfile)
		{
			this._doc = new XmlDocument();
			if (isfile) _doc.Load(xml);
			else _doc.LoadXml(xml);
			this.Root = _doc.DocumentElement;
		}
	}
}
