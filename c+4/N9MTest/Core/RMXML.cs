using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml;

namespace RM
{
    public class RMXML
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
        public void SaveToXMLFile()
        {
            this.Doc.Save(this._xmlfile.FullName);
        }
        public void SaveAsToXMLFile(FileInfo fi)
        {
            this.Doc.Save(fi.FullName); this._xmlfile = fi;
        }
        /// <summary>
        /// 基于当前节点，往下寻找
        /// </summary>
        /// <param name="curnode"></param>
        /// <param name="path">不包含当前节点,例：case/IPs，假设当前节点为root</param>
        /// <returns></returns>
        public XmlNode gNode(XmlNode curnode, string path)
        {
            //path:  //ns:abc, /ns:a/ns:b, ns:abc
            //ns:TriggerList/ns:InputTrigger[contains(ns:InputChannel,'Input')
            XmlNamespaceManager mgr = new XmlNamespaceManager(curnode.OwnerDocument.NameTable);
            mgr.AddNamespace("ns", curnode.NamespaceURI);
            return (curnode.SelectSingleNode(path, mgr));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="path">不包含root节点,例：case/IPs，假设根节点为root</param>
        /// <returns></returns>
        public XmlNode gNode(string path)
        {
            //path:  //ns:abc, /ns:a/ns:b, ns:abc
            return (this.gNode(this.Root, path));
        }
        /// <summary>
        /// 基于当前节点，往下寻找
        /// </summary>
        /// <param name="curnode"></param>
        /// <param name="path">不包含当前节点,例：case/IPs，假设当前节点为root</param>
        /// <returns></returns>
        public XmlNodeList gNodes(XmlNode curnode, string path)
        {
            XmlNamespaceManager mgr = new XmlNamespaceManager(curnode.OwnerDocument.NameTable);
            mgr.AddNamespace("ns", curnode.NamespaceURI);
            return (curnode.SelectNodes(path, mgr));
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="path">不包含root节点,例：case/IPs，假设根节点为root</param>
        /// <returns></returns>
        public XmlNodeList gNodes(string path)
        {
            return (this.gNodes(this.Root, path));
        }
        /// <summary>
        /// xml配置存储IPs节点规则为：192.168.10.1,3,5-10,12...
        /// </summary>
        /// <param name="path">基于根节点路径，不包括要节点，如：Case/IPs</param>
        /// <returns></returns>
        public string[] getIPs(string path)
        {
            if (this.gNode(path) == null)
            {
                return null;
            }

            Match m = Regex.Match(this.gNode(path).InnerText, @"(\d{1,3}\.\d{1,3}\.\d{1,3}\.)(.*)");
            // m.Groups 返回：{ [192.168.10.1,3,5-10,12], [192.168.10.], [1,3,5-10,12] }
            string[] ips = m.Groups[2].Value.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            List<string> result = new List<string>();
            for (int i = 0; i < ips.Length; i++)
            {
                string[] tmp = ips[i].Split(new string[] { "-" }, StringSplitOptions.RemoveEmptyEntries);
                if (tmp.Length == 1)
                {
                    string ip = $"{m.Groups[1].Value}{tmp[0]}";
                    if (!result.Contains(ip)) result.Add(ip);
                }
                else
                {
                    for (int j = Convert.ToInt32(tmp[0]); j <= Convert.ToInt32(tmp[1]); j++)
                    {
                        string ip = $"{m.Groups[1].Value}{j}";
                        if (!result.Contains(ip)) result.Add(ip);
                    }
                }
            }
            return (result.ToArray());
        }
    }
}
