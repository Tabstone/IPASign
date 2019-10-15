using System;
using System.IO;
using System.Xml;

namespace PListNet.Nodes
{
	// Token: 0x02000014 RID: 20
	public class NullNode : PNode
	{
		// Token: 0x1700002B RID: 43
		// (get) Token: 0x060000A8 RID: 168 RVA: 0x000037EC File Offset: 0x000019EC
		internal override string XmlTag
		{
			get
			{
				return "null";
			}
		}

		// Token: 0x1700002C RID: 44
		// (get) Token: 0x060000A9 RID: 169 RVA: 0x00002FAD File Offset: 0x000011AD
		internal override byte BinaryTag
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x1700002D RID: 45
		// (get) Token: 0x060000AA RID: 170 RVA: 0x00002FAD File Offset: 0x000011AD
		internal override int BinaryLength
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x1700002E RID: 46
		// (get) Token: 0x060000AB RID: 171 RVA: 0x00002FAD File Offset: 0x000011AD
		internal override bool IsBinaryUnique
		{
			get
			{
				return false;
			}
		}

		// Token: 0x060000AC RID: 172 RVA: 0x000037F3 File Offset: 0x000019F3
		internal override void ReadBinary(Stream stream, int nodeLength)
		{
			if (nodeLength != 0)
			{
				throw new PListFormatException();
			}
		}

		// Token: 0x060000AD RID: 173 RVA: 0x000031AB File Offset: 0x000013AB
		internal override void WriteBinary(Stream stream)
		{
		}

		// Token: 0x060000AE RID: 174 RVA: 0x000035A7 File Offset: 0x000017A7
		internal override void ReadXml(XmlReader reader)
		{
			reader.ReadStartElement(this.XmlTag);
		}

		// Token: 0x060000AF RID: 175 RVA: 0x000035B5 File Offset: 0x000017B5
		internal override void WriteXml(XmlWriter writer)
		{
			writer.WriteStartElement(this.XmlTag);
			writer.WriteEndElement();
		}
	}
}
