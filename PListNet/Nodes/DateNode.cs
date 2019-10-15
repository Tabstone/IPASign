using System;
using System.Globalization;
using System.IO;
using System.Linq;

namespace PListNet.Nodes
{
	// Token: 0x02000010 RID: 16
	public sealed class DateNode : PNode<DateTime>
	{
		// Token: 0x17000017 RID: 23
		// (get) Token: 0x06000070 RID: 112 RVA: 0x0000323D File Offset: 0x0000143D
		internal override string XmlTag
		{
			get
			{
				return "date";
			}
		}

		// Token: 0x17000018 RID: 24
		// (get) Token: 0x06000071 RID: 113 RVA: 0x00003244 File Offset: 0x00001444
		internal override byte BinaryTag
		{
			get
			{
				return 3;
			}
		}

		// Token: 0x17000019 RID: 25
		// (get) Token: 0x06000072 RID: 114 RVA: 0x00003244 File Offset: 0x00001444
		internal override int BinaryLength
		{
			get
			{
				return 3;
			}
		}

		// Token: 0x06000073 RID: 115 RVA: 0x00003247 File Offset: 0x00001447
		public DateNode()
		{
		}

		// Token: 0x06000074 RID: 116 RVA: 0x0000324F File Offset: 0x0000144F
		public DateNode(DateTime value)
		{
			this.Value = value;
		}

		// Token: 0x06000075 RID: 117 RVA: 0x0000325E File Offset: 0x0000145E
		internal override void Parse(string data)
		{
			this.Value = DateTime.Parse(data, CultureInfo.InvariantCulture);
		}

		// Token: 0x06000076 RID: 118 RVA: 0x00003274 File Offset: 0x00001474
		internal override string ToXmlString()
		{
			return this.Value.ToUniversalTime().ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss.ffffffZ");
		}

		// Token: 0x06000077 RID: 119 RVA: 0x0000329C File Offset: 0x0000149C
		internal override void ReadBinary(Stream stream, int nodeLength)
		{
			byte[] array = new byte[1 << nodeLength];
			if (stream.Read(array, 0, array.Length) != array.Length)
			{
				throw new PListFormatException();
			}
			double value;
			switch (nodeLength)
			{
			case 0:
				throw new PListFormatException("Date < 32Bit");
			case 1:
				throw new PListFormatException("Date < 32Bit");
			case 2:
				value = (double)BitConverter.ToSingle(array.Reverse<byte>().ToArray<byte>(), 0);
				break;
			case 3:
				value = BitConverter.ToDouble(array.Reverse<byte>().ToArray<byte>(), 0);
				break;
			default:
				throw new PListFormatException("Date > 64Bit");
			}
			this.Value = new DateTime(2001, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(value);
		}

		// Token: 0x06000078 RID: 120 RVA: 0x0000334C File Offset: 0x0000154C
		internal override void WriteBinary(Stream stream)
		{
			DateTime d = new DateTime(2001, 1, 1, 0, 0, 0, DateTimeKind.Utc);
			byte[] array = BitConverter.GetBytes((this.Value - d).TotalSeconds).Reverse<byte>().ToArray<byte>();
			stream.Write(array, 0, array.Length);
		}
	}
}
