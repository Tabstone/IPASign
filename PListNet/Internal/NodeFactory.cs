using System;
using System.Collections.Generic;
using PListNet.Nodes;

namespace PListNet.Internal
{
	// Token: 0x02000007 RID: 7
	internal static class NodeFactory
	{
		// Token: 0x0600001E RID: 30 RVA: 0x00002304 File Offset: 0x00000504
		static NodeFactory()
		{
			NodeFactory.Register<DictionaryNode>(new DictionaryNode());
			NodeFactory.Register<IntegerNode>(new IntegerNode());
			NodeFactory.Register<RealNode>(new RealNode());
			NodeFactory.Register<StringNode>(new StringNode());
			NodeFactory.Register<ArrayNode>(new ArrayNode());
			NodeFactory.Register<DataNode>(new DataNode());
			NodeFactory.Register<DateNode>(new DateNode());
			NodeFactory.Register<UidNode>(new UidNode());
			NodeFactory.Register<StringNode>("string", 5, new StringNode());
			NodeFactory.Register<StringNode>("ustring", 6, new StringNode());
			NodeFactory.Register<BooleanNode>("true", 0, new BooleanNode());
			NodeFactory.Register<BooleanNode>("false", 0, new BooleanNode());
		}

		// Token: 0x0600001F RID: 31 RVA: 0x000023B8 File Offset: 0x000005B8
		private static void Register<T>(T node) where T : PNode, new()
		{
			if (!NodeFactory._xmlTags.ContainsKey(node.XmlTag))
			{
				NodeFactory._xmlTags.Add(node.XmlTag, node.GetType());
			}
			if (!NodeFactory._binaryTags.ContainsKey(node.BinaryTag))
			{
				NodeFactory._binaryTags.Add(node.BinaryTag, node.GetType());
			}
		}

		// Token: 0x06000020 RID: 32 RVA: 0x00002434 File Offset: 0x00000634
		private static void Register<T>(string xmlTag, byte binaryTag, T node) where T : PNode, new()
		{
			if (!NodeFactory._xmlTags.ContainsKey(xmlTag))
			{
				NodeFactory._xmlTags.Add(xmlTag, node.GetType());
			}
			if (!NodeFactory._binaryTags.ContainsKey(binaryTag))
			{
				NodeFactory._binaryTags.Add(binaryTag, node.GetType());
			}
		}

		// Token: 0x06000021 RID: 33 RVA: 0x00002488 File Offset: 0x00000688
		public static PNode Create(byte binaryTag, int length)
		{
			if (binaryTag == 0 && length == 0)
			{
				return new NullNode();
			}
			if (binaryTag == 0 && length == 15)
			{
				return new FillNode();
			}
			if (binaryTag == 6)
			{
				return new StringNode
				{
					IsUtf16 = true
				};
			}
			if (NodeFactory._binaryTags.ContainsKey(binaryTag))
			{
				return (PNode)Activator.CreateInstance(NodeFactory._binaryTags[binaryTag]);
			}
			throw new PListFormatException(string.Format("Unknown node - binary tag {0}", binaryTag));
		}

		// Token: 0x06000022 RID: 34 RVA: 0x000024F8 File Offset: 0x000006F8
		public static PNode Create(string tag)
		{
			if (NodeFactory._xmlTags.ContainsKey(tag))
			{
				return (PNode)Activator.CreateInstance(NodeFactory._xmlTags[tag]);
			}
			throw new PListFormatException(string.Format("Unknown node - XML tag \"{0}\"", tag));
		}

		// Token: 0x06000023 RID: 35 RVA: 0x0000252D File Offset: 0x0000072D
		public static PNode CreateLengthElement(int length)
		{
			return new IntegerNode((long)length);
		}

		// Token: 0x06000024 RID: 36 RVA: 0x00002536 File Offset: 0x00000736
		public static PNode CreateKeyElement(string key)
		{
			return new StringNode(key);
		}

		// Token: 0x04000005 RID: 5
		private static readonly Dictionary<string, Type> _xmlTags = new Dictionary<string, Type>();

		// Token: 0x04000006 RID: 6
		private static readonly Dictionary<byte, Type> _binaryTags = new Dictionary<byte, Type>();
	}
}
