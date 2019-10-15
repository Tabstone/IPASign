using System;
using System.Globalization;
using System.IO;
using System.Linq;

namespace PListNet.Nodes
{
	// Token: 0x02000015 RID: 21
	public sealed class RealNode : PNode<double>
	{
		// Token: 0x1700002F RID: 47
		// (get) Token: 0x060000B1 RID: 177 RVA: 0x000037FE File Offset: 0x000019FE
		internal override string XmlTag
		{
			get
			{
				return "real";
			}
		}

		// Token: 0x17000030 RID: 48
		// (get) Token: 0x060000B2 RID: 178 RVA: 0x00003805 File Offset: 0x00001A05
		internal override byte BinaryTag
		{
			get
			{
				return 2;
			}
		}

		// Token: 0x17000031 RID: 49
		// (get) Token: 0x060000B3 RID: 179 RVA: 0x00003244 File Offset: 0x00001444
		internal override int BinaryLength
		{
			get
			{
				return 3;
			}
		}

		// Token: 0x060000B4 RID: 180 RVA: 0x00003808 File Offset: 0x00001A08
		public RealNode()
		{
		}

		// Token: 0x060000B5 RID: 181 RVA: 0x00003810 File Offset: 0x00001A10
		public RealNode(double value)
		{
			this.Value = value;
		}

		// Token: 0x060000B6 RID: 182 RVA: 0x0000381F File Offset: 0x00001A1F
		internal override void Parse(string data)
		{
			this.Value = double.Parse(data, CultureInfo.InvariantCulture);
		}

		// Token: 0x060000B7 RID: 183 RVA: 0x00003834 File Offset: 0x00001A34
		internal override string ToXmlString()
		{
			return this.Value.ToString(CultureInfo.InvariantCulture);
		}

		// Token: 0x060000B8 RID: 184 RVA: 0x00003854 File Offset: 0x00001A54
		internal override void ReadBinary(Stream stream, int nodeLength)
		{
			byte[] array = new byte[1 << nodeLength];
			if (stream.Read(array, 0, array.Length) != array.Length)
			{
				throw new PListFormatException();
			}
			switch (nodeLength)
			{
			case 0:
				throw new PListFormatException("Real < 32Bit");
			case 1:
				throw new PListFormatException("Real < 32Bit");
			case 2:
				this.Value = (double)BitConverter.ToSingle(array.Reverse<byte>().ToArray<byte>(), 0);
				return;
			case 3:
				this.Value = BitConverter.ToDouble(array.Reverse<byte>().ToArray<byte>(), 0);
				return;
			default:
				throw new PListFormatException("Real > 64Bit");
			}
		}

		// Token: 0x060000B9 RID: 185 RVA: 0x000038EC File Offset: 0x00001AEC
		internal override void WriteBinary(Stream stream)
		{
			byte[] array = BitConverter.GetBytes(this.Value).Reverse<byte>().ToArray<byte>();
			stream.Write(array, 0, array.Length);
		}
	}
}
