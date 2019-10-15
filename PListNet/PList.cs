using System;
using System.IO;
using System.Text;
using System.Xml;
using PListNet.Internal;

namespace PListNet
{
	// Token: 0x02000003 RID: 3
	public static class PList
	{
		// Token: 0x06000001 RID: 1 RVA: 0x00002050 File Offset: 0x00000250
		public static PNode Load(Stream stream)
		{
			if (!PList.IsFormatBinary(stream))
			{
				return PList.LoadAsXml(stream);
			}
			return PList.LoadAsBinary(stream);
		}

		// Token: 0x06000002 RID: 2 RVA: 0x00002068 File Offset: 0x00000268
		private static bool IsFormatBinary(Stream stream)
		{
			byte[] array = new byte[8];
			stream.Read(array, 0, array.Length);
			stream.Seek(0L, SeekOrigin.Begin);
			return Encoding.UTF8.GetString(array, 0, array.Length) == "bplist00";
		}

		// Token: 0x06000003 RID: 3 RVA: 0x000020AB File Offset: 0x000002AB
		private static PNode LoadAsBinary(Stream stream)
		{
			return new BinaryFormatReader().Read(stream);
		}

		// Token: 0x06000004 RID: 4 RVA: 0x000020B8 File Offset: 0x000002B8
		private static PNode LoadAsXml(Stream stream)
		{
			XmlReaderSettings settings = new XmlReaderSettings
			{
				ProhibitDtd = false,
				XmlResolver = null
			};
			PNode result;
			using (XmlReader xmlReader = XmlReader.Create(stream, settings))
			{
				xmlReader.MoveToContent();
				xmlReader.ReadStartElement("plist");
				xmlReader.MoveToContent();
				PNode pnode = NodeFactory.Create(xmlReader.LocalName);
				pnode.ReadXml(xmlReader);
				xmlReader.ReadEndElement();
				result = pnode;
			}
			return result;
		}

		// Token: 0x06000005 RID: 5 RVA: 0x00002130 File Offset: 0x00000330
		public static void Save(PNode rootNode, Stream stream, PListFormat format)
		{
			if (format == PListFormat.Xml)
			{
				XmlWriterSettings settings = new XmlWriterSettings
				{
					Encoding = Encoding.UTF8,
					Indent = true,
					IndentChars = "\t",
					NewLineChars = "\n"
				};
				using (XmlWriter xmlWriter = XmlWriter.Create(stream, settings))
				{
					xmlWriter.WriteStartDocument();
					xmlWriter.WriteDocType("plist", "-//Apple//DTD PLIST 1.0//EN", "http://www.apple.com/DTDs/PropertyList-1.0.dtd", null);
					xmlWriter.WriteStartElement("plist");
					xmlWriter.WriteAttributeString("version", "1.0");
					rootNode.WriteXml(xmlWriter);
					xmlWriter.WriteEndElement();
					xmlWriter.Flush();
					return;
				}
			}
			new BinaryFormatWriter().Write(stream, rootNode);
		}
	}
}
