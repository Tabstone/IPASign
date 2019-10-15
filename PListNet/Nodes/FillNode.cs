using System;
using System.IO;
using System.Xml;
using System.Xml.Schema;

namespace PListNet.Nodes
{
	// Token: 0x02000012 RID: 18
	public class FillNode : PNode
	{
		// Token: 0x17000023 RID: 35
		// (get) Token: 0x06000093 RID: 147 RVA: 0x0000358C File Offset: 0x0000178C
		internal override string XmlTag
		{
			get
			{
				return "fill";
			}
		}

		// Token: 0x17000024 RID: 36
		// (get) Token: 0x06000094 RID: 148 RVA: 0x00002FAD File Offset: 0x000011AD
		internal override byte BinaryTag
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x17000025 RID: 37
		// (get) Token: 0x06000095 RID: 149 RVA: 0x00003593 File Offset: 0x00001793
		internal override int BinaryLength
		{
			get
			{
				return 15;
			}
		}

		// Token: 0x17000026 RID: 38
		// (get) Token: 0x06000096 RID: 150 RVA: 0x00002FAD File Offset: 0x000011AD
		internal override bool IsBinaryUnique
		{
			get
			{
				return false;
			}
		}

		// Token: 0x06000097 RID: 151 RVA: 0x00003597 File Offset: 0x00001797
		internal override void ReadBinary(Stream stream, int nodeLength)
		{
			if (nodeLength != 15)
			{
				throw new PListFormatException();
			}
		}

		// Token: 0x06000098 RID: 152 RVA: 0x000031AB File Offset: 0x000013AB
		internal override void WriteBinary(Stream stream)
		{
		}

		// Token: 0x06000099 RID: 153 RVA: 0x000035A4 File Offset: 0x000017A4
		public XmlSchema GetSchema()
		{
			return null;
		}

		// Token: 0x0600009A RID: 154 RVA: 0x000035A7 File Offset: 0x000017A7
		internal override void ReadXml(XmlReader reader)
		{
			reader.ReadStartElement(this.XmlTag);
		}

		// Token: 0x0600009B RID: 155 RVA: 0x000035B5 File Offset: 0x000017B5
		internal override void WriteXml(XmlWriter writer)
		{
			writer.WriteStartElement(this.XmlTag);
			writer.WriteEndElement();
		}
	}
}
