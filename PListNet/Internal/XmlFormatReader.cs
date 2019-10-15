using System;
using System.IO;
using System.Xml;

namespace PListNet.Internal
{
	// Token: 0x02000008 RID: 8
	public static class XmlFormatReader
	{
		// Token: 0x06000025 RID: 37 RVA: 0x00002540 File Offset: 0x00000740
		public static PNode Read(Stream stream)
		{
			XmlReaderSettings settings = new XmlReaderSettings();
			PNode result;
			using (XmlReader xmlReader = XmlReader.Create(stream, settings))
			{
				xmlReader.ReadStartElement("plist");
				PNode pnode = NodeFactory.Create(xmlReader.LocalName);
				pnode.ReadXml(xmlReader);
				xmlReader.ReadEndElement();
				result = pnode;
			}
			return result;
		}
	}
}
