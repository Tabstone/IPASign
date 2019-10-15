using System;
using System.IO;
using System.Xml;

namespace PListNet
{
	// Token: 0x02000004 RID: 4
	public abstract class PNode
	{
		// Token: 0x17000001 RID: 1
		// (get) Token: 0x06000006 RID: 6
		internal abstract string XmlTag { get; }

		// Token: 0x17000002 RID: 2
		// (get) Token: 0x06000007 RID: 7
		internal abstract byte BinaryTag { get; }

		// Token: 0x17000003 RID: 3
		// (get) Token: 0x06000008 RID: 8
		internal abstract int BinaryLength { get; }

		// Token: 0x17000004 RID: 4
		// (get) Token: 0x06000009 RID: 9
		internal abstract bool IsBinaryUnique { get; }

		// Token: 0x0600000A RID: 10
		internal abstract void ReadXml(XmlReader reader);

		// Token: 0x0600000B RID: 11
		internal abstract void WriteXml(XmlWriter writer);

		// Token: 0x0600000C RID: 12
		internal abstract void ReadBinary(Stream stream, int nodeLength);

		// Token: 0x0600000D RID: 13
		internal abstract void WriteBinary(Stream stream);
	}
}
