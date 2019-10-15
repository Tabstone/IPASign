using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PListNet.Nodes
{
	// Token: 0x02000016 RID: 22
	public class StringNode : PNode<string>
	{
		// Token: 0x17000032 RID: 50
		// (get) Token: 0x060000BA RID: 186 RVA: 0x0000391A File Offset: 0x00001B1A
		internal override string XmlTag
		{
			get
			{
				return "string";
			}
		}

		// Token: 0x17000033 RID: 51
		// (get) Token: 0x060000BB RID: 187 RVA: 0x00003921 File Offset: 0x00001B21
		internal override byte BinaryTag
		{
			get
			{
				return this.IsUtf16 ? (byte)6 : (byte)5;
			}
		}

		// Token: 0x17000034 RID: 52
		// (get) Token: 0x060000BC RID: 188 RVA: 0x00003930 File Offset: 0x00001B30
		internal override int BinaryLength
		{
			get
			{
				return this.Value.Length;
			}
		}

		// Token: 0x17000035 RID: 53
		// (get) Token: 0x060000BD RID: 189 RVA: 0x0000393D File Offset: 0x00001B3D
		// (set) Token: 0x060000BE RID: 190 RVA: 0x00003945 File Offset: 0x00001B45
		internal bool IsUtf16 { get; set; }

		// Token: 0x060000BF RID: 191 RVA: 0x0000394E File Offset: 0x00001B4E
		public StringNode()
		{
		}

		// Token: 0x060000C0 RID: 192 RVA: 0x00003956 File Offset: 0x00001B56
		public StringNode(string value)
		{
			this.Value = value;
		}

		// Token: 0x17000036 RID: 54
		// (get) Token: 0x060000C1 RID: 193 RVA: 0x00003965 File Offset: 0x00001B65
		// (set) Token: 0x060000C2 RID: 194 RVA: 0x00003970 File Offset: 0x00001B70
		public sealed override string Value
		{
			get
			{
				return this._value;
			}
			set
			{
				this._value = value;
				for (int i = 0; i < value.Length; i++)
				{
					char item = value[i];
					if (!StringNode._utf8Chars.Contains(item))
					{
						this.IsUtf16 = true;
						return;
					}
				}
				this.IsUtf16 = false;
			}
		}

		// Token: 0x060000C3 RID: 195 RVA: 0x000039BB File Offset: 0x00001BBB
		internal override void Parse(string data)
		{
			this.Value = data;
		}

		// Token: 0x060000C4 RID: 196 RVA: 0x000039C4 File Offset: 0x00001BC4
		internal override string ToXmlString()
		{
			return this.Value;
		}

		// Token: 0x060000C5 RID: 197 RVA: 0x000039CC File Offset: 0x00001BCC
		internal override void ReadBinary(Stream stream, int nodeLength)
		{
			byte[] array = new byte[nodeLength * ((this.BinaryTag == 5) ? 1 : 2)];
			if (stream.Read(array, 0, array.Length) != array.Length)
			{
				throw new PListFormatException();
			}
			Encoding encoding = (this.BinaryTag == 5) ? Encoding.UTF8 : Encoding.BigEndianUnicode;
			this.Value = encoding.GetString(array, 0, array.Length);
		}

		// Token: 0x060000C6 RID: 198 RVA: 0x00003A2C File Offset: 0x00001C2C
		internal override void WriteBinary(Stream stream)
		{
			byte[] bytes = (this.IsUtf16 ? Encoding.BigEndianUnicode : Encoding.UTF8).GetBytes(this.Value);
			stream.Write(bytes, 0, bytes.Length);
		}

		// Token: 0x0400000C RID: 12
		private static readonly byte[] _utf8Bytes = new byte[]
		{
			0,
			1,
			2,
			3,
			4,
			5,
			6,
			7,
			8,
			9,
			10,
			11,
			12,
			13,
			14,
			15,
			16,
			17,
			18,
			19,
			20,
			21,
			22,
			23,
			24,
			25,
			26,
			27,
			28,
			29,
			30,
			31,
			32,
			33,
			34,
			35,
			36,
			37,
			38,
			39,
			40,
			41,
			42,
			43,
			44,
			45,
			46,
			47,
			48,
			49,
			50,
			51,
			52,
			53,
			54,
			55,
			56,
			57,
			58,
			59,
			60,
			61,
			62,
			63,
			64,
			65,
			66,
			67,
			68,
			69,
			70,
			71,
			72,
			73,
			74,
			75,
			76,
			77,
			78,
			79,
			80,
			81,
			82,
			83,
			84,
			85,
			86,
			87,
			88,
			89,
			90,
			91,
			92,
			93,
			94,
			95,
			96,
			97,
			98,
			99,
			100,
			101,
			102,
			103,
			104,
			105,
			106,
			107,
			108,
			109,
			110,
			111,
			112,
			113,
			114,
			115,
			116,
			117,
			118,
			119,
			120,
			121,
			122,
			123,
			124,
			125,
			126,
			127,
			128,
			129,
			130,
			131,
			132,
			133,
			134,
			135,
			136,
			137,
			138,
			139,
			140,
			141,
			142,
			143,
			144,
			145,
			146,
			147,
			148,
			149,
			150,
			151,
			152,
			153,
			154,
			155,
			156,
			157,
			158,
			159,
			160,
			161,
			162,
			163,
			164,
			165,
			166,
			167,
			168,
			169,
			170,
			171,
			172,
			173,
			174,
			175,
			176,
			177,
			178,
			179,
			180,
			181,
			182,
			183,
			184,
			185,
			186,
			187,
			188,
			189,
			190,
			191,
			192,
			193,
			194,
			195,
			196,
			197,
			198,
			199,
			200,
			201,
			202,
			203,
			204,
			205,
			206,
			207,
			208,
			209,
			210,
			211,
			212,
			213,
			214,
			215,
			216,
			217,
			218,
			219,
			220,
			221,
			222,
			223,
			224,
			225,
			226,
			227,
			228,
			229,
			230,
			231,
			232,
			233,
			234,
			235,
			236,
			237,
			238,
			239,
			240,
			241,
			242,
			243,
			244,
			245,
			246,
			247,
			248,
			249,
			250,
			251,
			252,
			253,
			254,
			byte.MaxValue
		};

		// Token: 0x0400000D RID: 13
		private static readonly HashSet<char> _utf8Chars = new HashSet<char>(Encoding.UTF8.GetChars(StringNode._utf8Bytes));

		// Token: 0x0400000E RID: 14
		private string _value;
	}
}
