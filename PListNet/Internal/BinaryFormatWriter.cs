using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using PListNet.Nodes;

namespace PListNet.Internal
{
	// Token: 0x0200000A RID: 10
	public class BinaryFormatWriter
	{
		// Token: 0x0600002E RID: 46 RVA: 0x0000290C File Offset: 0x00000B0C
		internal BinaryFormatWriter()
		{
		}

		// Token: 0x0600002F RID: 47 RVA: 0x00002920 File Offset: 0x00000B20
		public void Write(Stream stream, PNode node)
		{
			stream.Write(BinaryFormatWriter._header, 0, BinaryFormatWriter._header.Length);
			List<int> list = new List<int>();
			int num = BinaryFormatWriter.GetNodeCount(node);
			byte b;
			if (num <= 255)
			{
				b = 1;
			}
			else if (num <= 32767)
			{
				b = 2;
			}
			else
			{
				b = 4;
			}
			int host = this.WriteInternal(stream, b, list, node);
			num = list.Count;
			int num2 = (int)stream.Position;
			byte b2;
			if (num2 <= 255)
			{
				b2 = 1;
			}
			else if (num2 <= 32767)
			{
				b2 = 2;
			}
			else
			{
				b2 = 4;
			}
			for (int i = 0; i < list.Count; i++)
			{
				byte[] array = null;
				switch (b2)
				{
				case 1:
					array = new byte[]
					{
						(byte)list[i]
					};
					break;
				case 2:
					array = BitConverter.GetBytes(EndianConverter.HostToNetworkOrder((short)list[i]));
					break;
				case 4:
					array = BitConverter.GetBytes(EndianConverter.HostToNetworkOrder(list[i]));
					break;
				}
				stream.Write(array, 0, array.Length);
			}
			byte[] array2 = new byte[32];
			array2[6] = b2;
			array2[7] = b;
			BitConverter.GetBytes(EndianConverter.HostToNetworkOrder(num)).CopyTo(array2, 12);
			BitConverter.GetBytes(EndianConverter.HostToNetworkOrder(host)).CopyTo(array2, 20);
			BitConverter.GetBytes(EndianConverter.HostToNetworkOrder(num2)).CopyTo(array2, 28);
			stream.Write(array2, 0, array2.Length);
		}

		// Token: 0x06000030 RID: 48 RVA: 0x00002A80 File Offset: 0x00000C80
		internal int WriteInternal(Stream stream, byte nodeIndexSize, List<int> offsets, PNode node)
		{
			int num = offsets.Count;
			if (node.IsBinaryUnique && node is IEquatable<PNode>)
			{
				if (!this._uniqueElements.ContainsKey(node.BinaryTag))
				{
					this._uniqueElements.Add(node.BinaryTag, new Dictionary<PNode, int>());
				}
				if (!this._uniqueElements[node.BinaryTag].ContainsKey(node))
				{
					this._uniqueElements[node.BinaryTag][node] = num;
				}
				else
				{
					if (!(node is BooleanNode))
					{
						return this._uniqueElements[node.BinaryTag][node];
					}
					num = this._uniqueElements[node.BinaryTag][node];
				}
			}
			int item = (int)stream.Position;
			offsets.Add(item);
			int binaryLength = node.BinaryLength;
			byte value = (byte)((int)node.BinaryTag << 4 | ((binaryLength < 15) ? binaryLength : 15));
			stream.WriteByte(value);
			if (binaryLength >= 15)
			{
				PNode pnode = NodeFactory.CreateLengthElement(binaryLength);
				byte value2 = (byte)((int)pnode.BinaryTag << 4 | pnode.BinaryLength);
				stream.WriteByte(value2);
				pnode.WriteBinary(stream);
			}
			ArrayNode arrayNode = node as ArrayNode;
			if (arrayNode != null)
			{
				this.WriteInternal(stream, nodeIndexSize, offsets, arrayNode);
				return num;
			}
			DictionaryNode dictionaryNode = node as DictionaryNode;
			if (dictionaryNode != null)
			{
				this.WriteInternal(stream, nodeIndexSize, offsets, dictionaryNode);
				return num;
			}
			node.WriteBinary(stream);
			return num;
		}

		// Token: 0x06000031 RID: 49 RVA: 0x00002BF0 File Offset: 0x00000DF0
		private void WriteInternal(Stream stream, byte nodeIndexSize, List<int> offsets, ArrayNode array)
		{
			byte[] array2 = new byte[(int)nodeIndexSize * array.Count];
			long position = stream.Position;
			stream.Write(array2, 0, array2.Length);
			for (int i = 0; i < array.Count; i++)
			{
				BinaryFormatWriter.FormatIdx(this.WriteInternal(stream, nodeIndexSize, offsets, array[i]), nodeIndexSize).CopyTo(array2, (int)nodeIndexSize * i);
			}
			stream.Seek(position, SeekOrigin.Begin);
			stream.Write(array2, 0, array2.Length);
			stream.Seek(0L, SeekOrigin.End);
		}

		// Token: 0x06000032 RID: 50 RVA: 0x00002C70 File Offset: 0x00000E70
		private void WriteInternal(Stream stream, byte nodeIndexSize, List<int> offsets, DictionaryNode dictionary)
		{
			byte[] array = new byte[(int)nodeIndexSize * dictionary.Count];
			byte[] array2 = new byte[(int)nodeIndexSize * dictionary.Count];
			long position = stream.Position;
			stream.Write(array, 0, array.Length);
			stream.Write(array2, 0, array2.Length);
			KeyValuePair<string, PNode>[] array3 = dictionary.ToArray<KeyValuePair<string, PNode>>();
			for (int i = 0; i < dictionary.Count; i++)
			{
				BinaryFormatWriter.FormatIdx(this.WriteInternal(stream, nodeIndexSize, offsets, NodeFactory.CreateKeyElement(array3[i].Key)), nodeIndexSize).CopyTo(array, (int)nodeIndexSize * i);
			}
			for (int j = 0; j < dictionary.Count; j++)
			{
				BinaryFormatWriter.FormatIdx(this.WriteInternal(stream, nodeIndexSize, offsets, array3[j].Value), nodeIndexSize).CopyTo(array2, (int)nodeIndexSize * j);
			}
			stream.Seek(position, SeekOrigin.Begin);
			stream.Write(array, 0, array.Length);
			stream.Write(array2, 0, array2.Length);
			stream.Seek(0L, SeekOrigin.End);
		}

		// Token: 0x06000033 RID: 51 RVA: 0x00002D68 File Offset: 0x00000F68
		private static int GetNodeCount(PNode node)
		{
			if (node == null)
			{
				throw new ArgumentNullException("node");
			}
			ArrayNode arrayNode = node as ArrayNode;
			if (arrayNode != null)
			{
				int num = 1;
				foreach (PNode node2 in arrayNode)
				{
					num += BinaryFormatWriter.GetNodeCount(node2);
				}
				return num;
			}
			DictionaryNode dictionaryNode = node as DictionaryNode;
			if (dictionaryNode != null)
			{
				int num2 = 1;
				foreach (PNode node3 in dictionaryNode.Values)
				{
					num2 += BinaryFormatWriter.GetNodeCount(node3);
				}
				num2 += dictionaryNode.Keys.Count;
				return num2;
			}
			return 1;
		}

		// Token: 0x06000034 RID: 52 RVA: 0x00002E34 File Offset: 0x00001034
		private static byte[] FormatIdx(int index, byte nodeIndexSize)
		{
			switch (nodeIndexSize)
			{
			case 1:
				return new byte[]
				{
					(byte)index
				};
			case 2:
				return BitConverter.GetBytes(EndianConverter.HostToNetworkOrder((short)index));
			case 4:
				return BitConverter.GetBytes(EndianConverter.HostToNetworkOrder(index));
			}
			throw new PListFormatException("Invalid node index size");
		}

		// Token: 0x04000007 RID: 7
		private static readonly byte[] _header = new byte[]
		{
			98,
			112,
			108,
			105,
			115,
			116,
			48,
			48
		};

		// Token: 0x04000008 RID: 8
		private readonly Dictionary<byte, Dictionary<PNode, int>> _uniqueElements = new Dictionary<byte, Dictionary<PNode, int>>();
	}
}
