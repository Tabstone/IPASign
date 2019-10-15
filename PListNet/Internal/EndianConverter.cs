using System;

namespace PListNet.Internal
{
	// Token: 0x0200000B RID: 11
	internal static class EndianConverter
	{
		// Token: 0x06000036 RID: 54 RVA: 0x00002EA2 File Offset: 0x000010A2
		public static long NetworkToHostOrder(long network)
		{
			if (BitConverter.IsLittleEndian)
			{
				return EndianConverter.SwapLong(network);
			}
			return network;
		}

		// Token: 0x06000037 RID: 55 RVA: 0x00002EB3 File Offset: 0x000010B3
		public static int NetworkToHostOrder(int network)
		{
			if (BitConverter.IsLittleEndian)
			{
				return EndianConverter.SwapInt(network);
			}
			return network;
		}

		// Token: 0x06000038 RID: 56 RVA: 0x00002EC4 File Offset: 0x000010C4
		public static short NetworkToHostOrder(short network)
		{
			if (BitConverter.IsLittleEndian)
			{
				return EndianConverter.SwapShort(network);
			}
			return network;
		}

		// Token: 0x06000039 RID: 57 RVA: 0x00002EC4 File Offset: 0x000010C4
		public static short HostToNetworkOrder(short host)
		{
			if (BitConverter.IsLittleEndian)
			{
				return EndianConverter.SwapShort(host);
			}
			return host;
		}

		// Token: 0x0600003A RID: 58 RVA: 0x00002EB3 File Offset: 0x000010B3
		public static int HostToNetworkOrder(int host)
		{
			if (BitConverter.IsLittleEndian)
			{
				return EndianConverter.SwapInt(host);
			}
			return host;
		}

		// Token: 0x0600003B RID: 59 RVA: 0x00002EA2 File Offset: 0x000010A2
		public static long HostToNetworkOrder(long host)
		{
			if (BitConverter.IsLittleEndian)
			{
				return EndianConverter.SwapLong(host);
			}
			return host;
		}

		// Token: 0x0600003C RID: 60 RVA: 0x00002ED5 File Offset: 0x000010D5
		private static short SwapShort(short number)
		{
			return (short)((number >> 8 & 255) | ((int)number << 8 & 65280));
		}

		// Token: 0x0600003D RID: 61 RVA: 0x00002EEB File Offset: 0x000010EB
		private static int SwapInt(int number)
		{
			return (number >> 24 & 255) | (number >> 8 & 65280) | (number << 8 & 16711680) | number << 24;
		}

		// Token: 0x0600003E RID: 62 RVA: 0x00002F10 File Offset: 0x00001110
		private static long SwapLong(long number)
		{
			return (number >> 56 & 255L) | (number >> 40 & 65280L) | (number >> 24 & 16711680L) | (number >> 8 & (long)((ulong)4278190080L)) | (number << 8 & 1095216660480L) | (number << 24 & 280375465082880L) | (number << 40 & 71776119061217280L) | number << 56;
		}
	}
}
