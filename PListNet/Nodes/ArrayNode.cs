using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using PListNet.Internal;

namespace PListNet.Nodes
{
	// Token: 0x0200000D RID: 13
	public class ArrayNode : PNode, IList<PNode>, ICollection<PNode>, IEnumerable<PNode>, IEnumerable
	{
		// Token: 0x17000009 RID: 9
		// (get) Token: 0x06000044 RID: 68 RVA: 0x00002F95 File Offset: 0x00001195
		internal override string XmlTag
		{
			get
			{
				return "array";
			}
		}

		// Token: 0x1700000A RID: 10
		// (get) Token: 0x06000045 RID: 69 RVA: 0x00002F9C File Offset: 0x0000119C
		internal override byte BinaryTag
		{
			get
			{
				return 10;
			}
		}

		// Token: 0x1700000B RID: 11
		// (get) Token: 0x06000046 RID: 70 RVA: 0x00002FA0 File Offset: 0x000011A0
		internal override int BinaryLength
		{
			get
			{
				return this._list.Count;
			}
		}

		// Token: 0x1700000C RID: 12
		// (get) Token: 0x06000047 RID: 71 RVA: 0x00002FAD File Offset: 0x000011AD
		internal override bool IsBinaryUnique
		{
			get
			{
				return false;
			}
		}

		// Token: 0x06000048 RID: 72 RVA: 0x00002FB0 File Offset: 0x000011B0
		internal override void ReadBinary(Stream stream, int nodeLength)
		{
			throw new NotImplementedException("This type of node does not do it's own reading, refer to the binary reader.");
		}

		// Token: 0x06000049 RID: 73 RVA: 0x00002FBC File Offset: 0x000011BC
		internal override void WriteBinary(Stream stream)
		{
			throw new NotImplementedException("This type of node does not do it's own writing, refer to the binary writer.");
		}

		// Token: 0x0600004A RID: 74 RVA: 0x00002FC8 File Offset: 0x000011C8
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
				reader.MoveToContent();
				PNode pnode = NodeFactory.Create(reader.LocalName);
				pnode.ReadXml(reader);
				this.Add(pnode);
				reader.MoveToContent();
			}
			reader.ReadEndElement();
		}

		// Token: 0x0600004B RID: 75 RVA: 0x00003028 File Offset: 0x00001228
		internal override void WriteXml(XmlWriter writer)
		{
			writer.WriteStartElement(this.XmlTag);
			for (int i = 0; i < this.Count; i++)
			{
				this[i].WriteXml(writer);
			}
			writer.WriteEndElement();
		}

		// Token: 0x0600004C RID: 76 RVA: 0x00003065 File Offset: 0x00001265
		public int IndexOf(PNode item)
		{
			return this._list.IndexOf(item);
		}

		// Token: 0x0600004D RID: 77 RVA: 0x00003073 File Offset: 0x00001273
		public void Insert(int index, PNode item)
		{
			this._list.Insert(index, item);
		}

		// Token: 0x0600004E RID: 78 RVA: 0x00003082 File Offset: 0x00001282
		public void RemoveAt(int index)
		{
			this._list.RemoveAt(index);
		}

		// Token: 0x1700000D RID: 13
		public PNode this[int index]
		{
			get
			{
				return this._list[index];
			}
			set
			{
				this._list[index] = value;
			}
		}

		// Token: 0x06000051 RID: 81 RVA: 0x000030AD File Offset: 0x000012AD
		public void Add(PNode item)
		{
			this._list.Add(item);
		}

		// Token: 0x06000052 RID: 82 RVA: 0x000030BB File Offset: 0x000012BB
		public void Clear()
		{
			this._list.Clear();
		}

		// Token: 0x06000053 RID: 83 RVA: 0x000030C8 File Offset: 0x000012C8
		public bool Contains(PNode item)
		{
			return this._list.Contains(item);
		}

		// Token: 0x06000054 RID: 84 RVA: 0x000030D6 File Offset: 0x000012D6
		public void CopyTo(PNode[] array, int arrayIndex)
		{
			this._list.CopyTo(array, arrayIndex);
		}

		// Token: 0x06000055 RID: 85 RVA: 0x000030E5 File Offset: 0x000012E5
		public bool Remove(PNode item)
		{
			return this._list.Remove(item);
		}

		// Token: 0x1700000E RID: 14
		// (get) Token: 0x06000056 RID: 86 RVA: 0x00002FA0 File Offset: 0x000011A0
		public int Count
		{
			get
			{
				return this._list.Count;
			}
		}

		// Token: 0x1700000F RID: 15
		// (get) Token: 0x06000057 RID: 87 RVA: 0x00002FAD File Offset: 0x000011AD
		public bool IsReadOnly
		{
			get
			{
				return false;
			}
		}

		// Token: 0x06000058 RID: 88 RVA: 0x000030F3 File Offset: 0x000012F3
		public IEnumerator<PNode> GetEnumerator()
		{
			return this._list.GetEnumerator();
		}

		// Token: 0x06000059 RID: 89 RVA: 0x000030F3 File Offset: 0x000012F3
		IEnumerator IEnumerable.GetEnumerator()
		{
			return this._list.GetEnumerator();
		}

		// Token: 0x04000009 RID: 9
		private readonly IList<PNode> _list = new List<PNode>();
	}
}
