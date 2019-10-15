using System;
using System.IO;

namespace PListNet.Nodes
{
	// Token: 0x0200000F RID: 15
	public sealed class DataNode : PNode<byte[]>
	{
		// Token: 0x17000014 RID: 20
		// (get) Token: 0x06000067 RID: 103 RVA: 0x000031AD File Offset: 0x000013AD
		internal override string XmlTag
		{
			get
			{
				return "data";
			}
		}

		// Token: 0x17000015 RID: 21
		// (get) Token: 0x06000068 RID: 104 RVA: 0x000031B4 File Offset: 0x000013B4
		internal override byte BinaryTag
		{
			get
			{
				return 4;
			}
		}

		// Token: 0x17000016 RID: 22
		// (get) Token: 0x06000069 RID: 105 RVA: 0x000031B7 File Offset: 0x000013B7
		internal override int BinaryLength
		{
			get
			{
				return this.Value.Length;
			}
		}

		// Token: 0x0600006A RID: 106 RVA: 0x000031C1 File Offset: 0x000013C1
		public DataNode()
		{
		}

		// Token: 0x0600006B RID: 107 RVA: 0x000031C9 File Offset: 0x000013C9
		public DataNode(byte[] value)
		{
			this.Value = value;
		}

		// Token: 0x0600006C RID: 108 RVA: 0x000031D8 File Offset: 0x000013D8
		internal override void Parse(string data)
		{
			this.Value = Convert.FromBase64String(data);
		}

		// Token: 0x0600006D RID: 109 RVA: 0x000031E6 File Offset: 0x000013E6
		internal override string ToXmlString()
		{
			return Convert.ToBase64String(this.Value);
		}

		// Token: 0x0600006E RID: 110 RVA: 0x000031F3 File Offset: 0x000013F3
		internal override void ReadBinary(Stream stream, int nodeLength)
		{
			this.Value = new byte[nodeLength];
			if (stream.Read(this.Value, 0, this.Value.Length) != this.Value.Length)
			{
				throw new PListFormatException();
			}
		}

		// Token: 0x0600006F RID: 111 RVA: 0x00003226 File Offset: 0x00001426
		internal override void WriteBinary(Stream stream)
		{
			stream.Write(this.Value, 0, this.Value.Length);
		}
	}
}
