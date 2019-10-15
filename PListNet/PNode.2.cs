using System;
using System.Xml;

namespace PListNet
{
	// Token: 0x02000005 RID: 5
	public abstract class PNode<T> : PNode, IEquatable<PNode>
	{
		// Token: 0x17000005 RID: 5
		// (get) Token: 0x0600000F RID: 15 RVA: 0x000021F4 File Offset: 0x000003F4
		// (set) Token: 0x06000010 RID: 16 RVA: 0x000021FC File Offset: 0x000003FC
		public virtual T Value { get; set; }

		// Token: 0x17000006 RID: 6
		// (get) Token: 0x06000011 RID: 17 RVA: 0x00002205 File Offset: 0x00000405
		internal override bool IsBinaryUnique
		{
			get
			{
				return true;
			}
		}

		// Token: 0x06000012 RID: 18 RVA: 0x00002208 File Offset: 0x00000408
		internal override void ReadXml(XmlReader reader)
		{
			reader.ReadStartElement();
			this.Parse(reader.ReadContentAsString());
			reader.ReadEndElement();
		}

		// Token: 0x06000013 RID: 19 RVA: 0x00002222 File Offset: 0x00000422
		internal override void WriteXml(XmlWriter writer)
		{
			writer.WriteStartElement(this.XmlTag);
			writer.WriteValue(this.ToXmlString());
			writer.WriteEndElement();
		}

		// Token: 0x06000014 RID: 20
		internal abstract void Parse(string data);

		// Token: 0x06000015 RID: 21
		internal abstract string ToXmlString();

		// Token: 0x06000016 RID: 22 RVA: 0x00002244 File Offset: 0x00000444
		public bool Equals(PNode other)
		{
			if (other is PNode<T>)
			{
				T value = this.Value;
				return value.Equals(((PNode<T>)other).Value);
			}
			return false;
		}

		// Token: 0x06000017 RID: 23 RVA: 0x00002280 File Offset: 0x00000480
		public override bool Equals(object obj)
		{
			PNode pnode = obj as PNode;
			return pnode != null && this.Equals(pnode);
		}

		// Token: 0x06000018 RID: 24 RVA: 0x000022A0 File Offset: 0x000004A0
		public override int GetHashCode()
		{
			T value = this.Value;
			return value.GetHashCode();
		}

		// Token: 0x06000019 RID: 25 RVA: 0x000022C1 File Offset: 0x000004C1
		public override string ToString()
		{
			return string.Format("{0}: {1}", this.XmlTag, this.Value);
		}
	}
}
