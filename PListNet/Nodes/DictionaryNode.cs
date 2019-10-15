using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using PListNet.Internal;

namespace PListNet.Nodes
{
	// Token: 0x02000011 RID: 17
	public class DictionaryNode : PNode, IDictionary<string, PNode>, ICollection<KeyValuePair<string, PNode>>, IEnumerable<KeyValuePair<string, PNode>>, IEnumerable
	{
		// Token: 0x1700001A RID: 26
		// (get) Token: 0x06000079 RID: 121 RVA: 0x0000339A File Offset: 0x0000159A
		internal override string XmlTag
		{
			get
			{
				return "dict";
			}
		}

		// Token: 0x1700001B RID: 27
		// (get) Token: 0x0600007A RID: 122 RVA: 0x000033A1 File Offset: 0x000015A1
		internal override byte BinaryTag
		{
			get
			{
				return 13;
			}
		}

		// Token: 0x1700001C RID: 28
		// (get) Token: 0x0600007B RID: 123 RVA: 0x000033A5 File Offset: 0x000015A5
		internal override int BinaryLength
		{
			get
			{
				return this.Count;
			}
		}

		// Token: 0x1700001D RID: 29
		// (get) Token: 0x0600007C RID: 124 RVA: 0x00002FAD File Offset: 0x000011AD
		internal override bool IsBinaryUnique
		{
			get
			{
				return false;
			}
		}

		// Token: 0x0600007D RID: 125 RVA: 0x00002FB0 File Offset: 0x000011B0
		internal override void ReadBinary(Stream stream, int nodeLength)
		{
			throw new NotImplementedException("This type of node does not do it's own reading, refer to the binary reader.");
		}

		// Token: 0x0600007E RID: 126 RVA: 0x00002FBC File Offset: 0x000011BC
		internal override void WriteBinary(Stream stream)
		{
			throw new NotImplementedException("This type of node does not do it's own writing, refer to the binary writer.");
		}

		// Token: 0x0600007F RID: 127 RVA: 0x000033B0 File Offset: 0x000015B0
		internal override void ReadXml(XmlReader reader)
		{
			bool isEmptyElement = reader.IsEmptyElement;
			reader.Read();
			if (isEmptyElement)
			{
				return;
			}
			reader.MoveToContent();
			while (reader.NodeType != XmlNodeType.EndElement)
			{
				reader.ReadStartElement("key");
				string key = reader.ReadContentAsString();
				reader.ReadEndElement();
				reader.MoveToContent();
				PNode pnode = NodeFactory.Create(reader.LocalName);
				pnode.ReadXml(reader);
				this.Add(key, pnode);
				reader.MoveToContent();
			}
			reader.ReadEndElement();
		}

		// Token: 0x06000080 RID: 128 RVA: 0x00003428 File Offset: 0x00001628
		internal override void WriteXml(XmlWriter writer)
		{
			writer.WriteStartElement(this.XmlTag);
			foreach (string text in this.Keys)
			{
				writer.WriteStartElement("key");
				writer.WriteValue(text);
				writer.WriteEndElement();
				this[text].WriteXml(writer);
			}
			writer.WriteEndElement();
		}

		// Token: 0x06000081 RID: 129 RVA: 0x000034A8 File Offset: 0x000016A8
		public bool ContainsKey(string key)
		{
			return this._dictionary.ContainsKey(key);
		}

		// Token: 0x06000082 RID: 130 RVA: 0x000034B6 File Offset: 0x000016B6
		public void Add(string key, PNode value)
		{
			this._dictionary.Add(key, value);
		}

		// Token: 0x06000083 RID: 131 RVA: 0x000034C5 File Offset: 0x000016C5
		public bool Remove(string key)
		{
			return this._dictionary.Remove(key);
		}

		// Token: 0x06000084 RID: 132 RVA: 0x000034D3 File Offset: 0x000016D3
		public bool TryGetValue(string key, out PNode value)
		{
			return this._dictionary.TryGetValue(key, out value);
		}

		// Token: 0x1700001E RID: 30
		public PNode this[string index]
		{
			get
			{
				return this._dictionary[index];
			}
			set
			{
				this._dictionary[index] = value;
			}
		}

		// Token: 0x1700001F RID: 31
		// (get) Token: 0x06000087 RID: 135 RVA: 0x000034FF File Offset: 0x000016FF
		public ICollection<string> Keys
		{
			get
			{
				return this._dictionary.Keys;
			}
		}

		// Token: 0x17000020 RID: 32
		// (get) Token: 0x06000088 RID: 136 RVA: 0x0000350C File Offset: 0x0000170C
		public ICollection<PNode> Values
		{
			get
			{
				return this._dictionary.Values;
			}
		}

		// Token: 0x06000089 RID: 137 RVA: 0x00003519 File Offset: 0x00001719
		public void Add(KeyValuePair<string, PNode> item)
		{
			this._dictionary.Add(item);
		}

		// Token: 0x0600008A RID: 138 RVA: 0x00003527 File Offset: 0x00001727
		public void Clear()
		{
			this._dictionary.Clear();
		}

		// Token: 0x0600008B RID: 139 RVA: 0x00003534 File Offset: 0x00001734
		public bool Contains(KeyValuePair<string, PNode> item)
		{
			return this._dictionary.Contains(item);
		}

		// Token: 0x0600008C RID: 140 RVA: 0x00003542 File Offset: 0x00001742
		public void CopyTo(KeyValuePair<string, PNode>[] array, int arrayIndex)
		{
			this._dictionary.CopyTo(array, arrayIndex);
		}

		// Token: 0x0600008D RID: 141 RVA: 0x00003551 File Offset: 0x00001751
		public bool Remove(KeyValuePair<string, PNode> item)
		{
			return this._dictionary.Remove(item);
		}

		// Token: 0x17000021 RID: 33
		// (get) Token: 0x0600008E RID: 142 RVA: 0x0000355F File Offset: 0x0000175F
		public int Count
		{
			get
			{
				return this._dictionary.Count;
			}
		}

		// Token: 0x17000022 RID: 34
		// (get) Token: 0x0600008F RID: 143 RVA: 0x00002FAD File Offset: 0x000011AD
		public bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		// Token: 0x06000090 RID: 144 RVA: 0x0000356C File Offset: 0x0000176C
		public IEnumerator<KeyValuePair<string, PNode>> GetEnumerator()
		{
			return this._dictionary.GetEnumerator();
		}

		// Token: 0x06000091 RID: 145 RVA: 0x0000356C File Offset: 0x0000176C
		IEnumerator IEnumerable.GetEnumerator()
		{
			return this._dictionary.GetEnumerator();
		}

		// Token: 0x0400000A RID: 10
		private readonly IDictionary<string, PNode> _dictionary = new Dictionary<string, PNode>();
	}
}
