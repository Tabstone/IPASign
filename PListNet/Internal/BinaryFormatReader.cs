using System;
using System.Collections.Generic;
using System.IO;
using PListNet.Nodes;

namespace PListNet.Internal
{
	// Token: 0x02000009 RID: 9
	internal class BinaryFormatReader
	{
		// Token: 0x06000026 RID: 38 RVA: 0x0000259C File Offset: 0x0000079C
		public PNode Read(Stream stream)
		{
			byte[] array = BinaryFormatReader.ReadHeader(stream);
			int[] nodeOffsets = BinaryFormatReader.ReadNodeOffsets(stream, array);
			byte indexSize = array[7];
			BinaryFormatReader.ReaderState readerState = new BinaryFormatReader.ReaderState(stream, nodeOffsets, (int)indexSize);
			int elemIdx = EndianConverter.NetworkToHostOrder(BitConverter.ToInt32(array, 20));
			return this.ReadInternal(readerState, elemIdx);
		}

		// Token: 0x06000027 RID: 39 RVA: 0x000025E0 File Offset: 0x000007E0
		private static byte[] ReadHeader(Stream stream)
		{
			byte[] array = new byte[32];
			stream.Seek(-32L, SeekOrigin.End);
			if (stream.Read(array, 0, array.Length) != array.Length)
			{
				throw new PListFormatException("Invalid Header Size");
			}
			return array;
		}

		// Token: 0x06000028 RID: 40 RVA: 0x0000261C File Offset: 0x0000081C
		private static int[] ReadNodeOffsets(Stream stream, byte[] header)
		{
			byte b = header[6];
			int num = EndianConverter.NetworkToHostOrder(BitConverter.ToInt32(header, 12));
			int num2 = EndianConverter.NetworkToHostOrder(BitConverter.ToInt32(header, 28));
			byte[] array = new byte[num * (int)b];
			stream.Seek((long)num2, SeekOrigin.Begin);
			if (stream.Read(array, 0, array.Length) != array.Length)
			{
				throw new PListFormatException("Invalid offsetTable Size");
			}
			int[] array2 = new int[num];
			for (int i = 0; i < num; i++)
			{
				byte[] array3 = new byte[4];
				for (int j = 0; j < (int)b; j++)
				{
					array3[(int)(b - 1) - j] = array[i * (int)b + j];
				}
				array2[i] = BitConverter.ToInt32(array3, 0);
			}
			return array2;
		}

		// Token: 0x06000029 RID: 41 RVA: 0x000026C8 File Offset: 0x000008C8
		private PNode ReadInternal(BinaryFormatReader.ReaderState readerState, int elemIdx)
		{
			readerState.Stream.Seek((long)readerState.NodeOffsets[elemIdx], SeekOrigin.Begin);
			return this.ReadInternal(readerState);
		}

		// Token: 0x0600002A RID: 42 RVA: 0x000026E8 File Offset: 0x000008E8
		private PNode ReadInternal(BinaryFormatReader.ReaderState readerState)
		{
			byte[] array = new byte[1];
			if (readerState.Stream.Read(array, 0, array.Length) != 1)
			{
				throw new PListFormatException("Couldn't read type Byte");
			}
			int num = (int)(array[0] & 15);
			byte b = (byte)(array[0] >> 4 & 15);
			if (b != 0 && num == 15)
			{
				PNode pnode = this.ReadInternal(readerState);
				if (!(pnode is IntegerNode))
				{
					throw new PListFormatException("Length is not an integer.");
				}
				num = (int)((IntegerNode)pnode).Value;
				if (num <= 0)
				{
					throw new PListFormatException(string.Format("Object length may not be less than 1 (parsed value was {0}). ", num) + "This error could be caused by a malformed PList file or an issue with this parser library. Try converting the file from binary to XML and parsing that. If XML parsing succeeds, please file a bug in our GitHub repo and include a pull request with a failing test.");
				}
			}
			PNode pnode2 = NodeFactory.Create(b, num);
			ArrayNode arrayNode = pnode2 as ArrayNode;
			if (arrayNode != null)
			{
				this.ReadInArray(arrayNode, num, readerState);
				return pnode2;
			}
			DictionaryNode dictionaryNode = pnode2 as DictionaryNode;
			if (dictionaryNode != null)
			{
				this.ReadInDictionary(dictionaryNode, num, readerState);
				return pnode2;
			}
			pnode2.ReadBinary(readerState.Stream, num);
			return pnode2;
		}

		// Token: 0x0600002B RID: 43 RVA: 0x000027C0 File Offset: 0x000009C0
		private void ReadInArray(ICollection<PNode> node, int nodeLength, BinaryFormatReader.ReaderState readerState)
		{
			byte[] array = new byte[nodeLength * readerState.IndexSize];
			if (readerState.Stream.Read(array, 0, array.Length) != array.Length)
			{
				throw new PListFormatException();
			}
			for (int i = 0; i < nodeLength; i++)
			{
				short elemIdx = (readerState.IndexSize == 1) ? ((short)array[i]) : EndianConverter.NetworkToHostOrder(BitConverter.ToInt16(array, 2 * i));
				node.Add(this.ReadInternal(readerState, (int)elemIdx));
			}
		}

		// Token: 0x0600002C RID: 44 RVA: 0x00002830 File Offset: 0x00000A30
		private void ReadInDictionary(IDictionary<string, PNode> node, int nodeLength, BinaryFormatReader.ReaderState readerState)
		{
			byte[] array = new byte[nodeLength * readerState.IndexSize];
			byte[] array2 = new byte[nodeLength * readerState.IndexSize];
			if (readerState.Stream.Read(array, 0, array.Length) != array.Length)
			{
				throw new PListFormatException();
			}
			if (readerState.Stream.Read(array2, 0, array2.Length) != array2.Length)
			{
				throw new PListFormatException();
			}
			for (int i = 0; i < nodeLength; i++)
			{
				short elemIdx = (readerState.IndexSize == 1) ? ((short)array[i]) : EndianConverter.NetworkToHostOrder(BitConverter.ToInt16(array, 2 * i));
				StringNode stringNode = this.ReadInternal(readerState, (int)elemIdx) as StringNode;
				if (stringNode == null)
				{
					throw new PListFormatException("Key is not a string");
				}
				elemIdx = ((readerState.IndexSize == 1) ? ((short)array2[i]) : EndianConverter.NetworkToHostOrder(BitConverter.ToInt16(array2, 2 * i)));
				PNode value = this.ReadInternal(readerState, (int)elemIdx);
				node.Add(stringNode.Value, value);
			}
		}

		// Token: 0x02000018 RID: 24
		private class ReaderState
		{
			// Token: 0x17000037 RID: 55
			// (get) Token: 0x060000C8 RID: 200 RVA: 0x00003A99 File Offset: 0x00001C99
			// (set) Token: 0x060000C9 RID: 201 RVA: 0x00003AA1 File Offset: 0x00001CA1
			public Stream Stream { get; private set; }

			// Token: 0x17000038 RID: 56
			// (get) Token: 0x060000CA RID: 202 RVA: 0x00003AAA File Offset: 0x00001CAA
			// (set) Token: 0x060000CB RID: 203 RVA: 0x00003AB2 File Offset: 0x00001CB2
			public int[] NodeOffsets { get; private set; }

			// Token: 0x17000039 RID: 57
			// (get) Token: 0x060000CC RID: 204 RVA: 0x00003ABB File Offset: 0x00001CBB
			// (set) Token: 0x060000CD RID: 205 RVA: 0x00003AC3 File Offset: 0x00001CC3
			public int IndexSize { get; private set; }

			// Token: 0x060000CE RID: 206 RVA: 0x00003ACC File Offset: 0x00001CCC
			public ReaderState(Stream stream, int[] nodeOffsets, int indexSize)
			{
				this.Stream = stream;
				this.NodeOffsets = nodeOffsets;
				this.IndexSize = indexSize;
			}
		}
	}
}
