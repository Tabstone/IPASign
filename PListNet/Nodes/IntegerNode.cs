using System;
using System.Globalization;
using System.IO;
using PListNet.Internal;

namespace PListNet.Nodes
{
	// Token: 0x02000013 RID: 19
	public class IntegerNode : PNode<long>
	{
		// Token: 0x17000027 RID: 39
		// (get) Token: 0x0600009D RID: 157 RVA: 0x000035C9 File Offset: 0x000017C9
		internal override string XmlTag
		{
			get
			{
				return "integer";
			}
		}

		// Token: 0x17000028 RID: 40
		// (get) Token: 0x0600009E RID: 158 RVA: 0x00002205 File Offset: 0x00000405
		internal override byte BinaryTag
		{
			get
			{
				return 1;
			}
		}

		// Token: 0x17000029 RID: 41
		// (get) Token: 0x0600009F RID: 159 RVA: 0x000035D0 File Offset: 0x000017D0
		internal override int BinaryLength
		{
			get
			{
				if (this.Value >= 0L && this.Value <= 255L)
				{
					return 0;
				}
				if (this.Value >= -32768L && this.Value <= 32767L)
				{
					return 1;
				}
				if (this.Value >= -2147483648L && this.Value <= 2147483647L)
				{
					return 2;
				}
				if (this.Value >= -9223372036854775808L && this.Value <= 9223372036854775807L)
				{
					return 3;
				}
				return -1;
			}
		}

		// Token: 0x1700002A RID: 42
		// (get) Token: 0x060000A0 RID: 160 RVA: 0x00003658 File Offset: 0x00001858
		// (set) Token: 0x060000A1 RID: 161 RVA: 0x00003660 File Offset: 0x00001860
		public override long Value { get; set; }

		// Token: 0x060000A2 RID: 162 RVA: 0x00003669 File Offset: 0x00001869
		public IntegerNode()
		{
		}

		// Token: 0x060000A3 RID: 163 RVA: 0x00003671 File Offset: 0x00001871
		public IntegerNode(long value)
		{
			this.Value = value;
		}

		// Token: 0x060000A4 RID: 164 RVA: 0x00003680 File Offset: 0x00001880
		internal override void Parse(string data)
		{
			this.Value = long.Parse(data, CultureInfo.InvariantCulture);
		}

		// Token: 0x060000A5 RID: 165 RVA: 0x00003694 File Offset: 0x00001894
		internal override string ToXmlString()
		{
			return this.Value.ToString(CultureInfo.InvariantCulture);
		}

		// Token: 0x060000A6 RID: 166 RVA: 0x000036B4 File Offset: 0x000018B4
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
				this.Value = (long)((ulong)array[0]);
				return;
			case 1:
				this.Value = (long)EndianConverter.NetworkToHostOrder(BitConverter.ToInt16(array, 0));
				return;
			case 2:
				this.Value = (long)EndianConverter.NetworkToHostOrder(BitConverter.ToInt32(array, 0));
				return;
			case 3:
				this.Value = EndianConverter.NetworkToHostOrder(BitConverter.ToInt64(array, 0));
				return;
			default:
				throw new PListFormatException("Int > 64Bit");
			}
		}

		// Token: 0x060000A7 RID: 167 RVA: 0x0000374C File Offset: 0x0000194C
		internal override void WriteBinary(Stream stream)
		{
			byte[] array;
			switch (this.BinaryLength)
			{
			case 0:
				array = new byte[]
				{
					(byte)this.Value
				};
				break;
			case 1:
				array = BitConverter.GetBytes(EndianConverter.HostToNetworkOrder((short)this.Value));
				break;
			case 2:
				array = BitConverter.GetBytes(EndianConverter.HostToNetworkOrder((int)this.Value));
				break;
			case 3:
				array = BitConverter.GetBytes(EndianConverter.HostToNetworkOrder(this.Value));
				break;
			default:
				throw new Exception(string.Format("Unexpected length: {0}.", this.BinaryLength));
			}
			stream.Write(array, 0, array.Length);
		}
	}
}
