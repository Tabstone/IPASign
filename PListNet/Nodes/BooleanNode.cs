using System;
using System.IO;
using System.Xml;

namespace PListNet.Nodes
{
	// Token: 0x0200000E RID: 14
	public sealed class BooleanNode : PNode<bool>
	{
		// Token: 0x17000010 RID: 16
		// (get) Token: 0x0600005B RID: 91 RVA: 0x00003113 File Offset: 0x00001313
		internal override string XmlTag
		{
			get
			{
				return "boolean";
			}
		}

		// Token: 0x17000011 RID: 17
		// (get) Token: 0x0600005C RID: 92 RVA: 0x00002FAD File Offset: 0x000011AD
		internal override byte BinaryTag
		{
			get
			{
				return 0;
			}
		}

		// Token: 0x17000012 RID: 18
		// (get) Token: 0x0600005D RID: 93 RVA: 0x0000311A File Offset: 0x0000131A
		internal override int BinaryLength
		{
			get
			{
				if (!this.Value)
				{
					return 8;
				}
				return 9;
			}
		}

		// Token: 0x17000013 RID: 19
		// (get) Token: 0x0600005E RID: 94 RVA: 0x00002205 File Offset: 0x00000405
		internal override bool IsBinaryUnique
		{
			get
			{
				return true;
			}
		}

		// Token: 0x0600005F RID: 95 RVA: 0x00003128 File Offset: 0x00001328
		public BooleanNode()
		{
		}

		// Token: 0x06000060 RID: 96 RVA: 0x00003130 File Offset: 0x00001330
		public BooleanNode(bool value)
		{
			this.Value = value;
		}

		// Token: 0x06000061 RID: 97 RVA: 0x0000313F File Offset: 0x0000133F
		internal override void ReadXml(XmlReader reader)
		{
			this.Parse(reader.LocalName);
			reader.ReadStartElement();
		}

		// Token: 0x06000062 RID: 98 RVA: 0x00003153 File Offset: 0x00001353
		internal override void WriteXml(XmlWriter writer)
		{
			writer.WriteStartElement(this.ToXmlString());
			writer.WriteEndElement();
		}

		// Token: 0x06000063 RID: 99 RVA: 0x00003167 File Offset: 0x00001367
		internal override void Parse(string data)
		{
			this.Value = (data == "true");
		}

		// Token: 0x06000064 RID: 100 RVA: 0x0000317A File Offset: 0x0000137A
		internal override string ToXmlString()
		{
			if (!this.Value)
			{
				return "false";
			}
			return "true";
		}

		// Token: 0x06000065 RID: 101 RVA: 0x0000318F File Offset: 0x0000138F
		internal override void ReadBinary(Stream stream, int nodeLength)
		{
			if (nodeLength != 8 && nodeLength != 9)
			{
				throw new PListFormatException();
			}
			this.Value = (nodeLength == 9);
		}

		// Token: 0x06000066 RID: 102 RVA: 0x000031AB File Offset: 0x000013AB
		internal override void WriteBinary(Stream stream)
		{
		}
	}
}
