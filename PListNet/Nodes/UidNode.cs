using System;

namespace PListNet.Nodes
{
	// Token: 0x0200000C RID: 12
	public class UidNode : IntegerNode
	{
		// Token: 0x17000007 RID: 7
		// (get) Token: 0x0600003F RID: 63 RVA: 0x00002F7C File Offset: 0x0000117C
		internal override string XmlTag
		{
			get
			{
				return "uid";
			}
		}

		// Token: 0x17000008 RID: 8
		// (get) Token: 0x06000040 RID: 64 RVA: 0x00002F83 File Offset: 0x00001183
		internal override byte BinaryTag
		{
			get
			{
				return 8;
			}
		}

		// Token: 0x06000041 RID: 65 RVA: 0x00002F86 File Offset: 0x00001186
		internal override void Parse(string data)
		{
			throw new NotImplementedException();
		}

		// Token: 0x06000042 RID: 66 RVA: 0x00002F86 File Offset: 0x00001186
		internal override string ToXmlString()
		{
			throw new NotImplementedException();
		}
	}
}
