using System;

namespace PListNet
{
	// Token: 0x02000006 RID: 6
	public class PListFormatException : Exception
	{
		// Token: 0x0600001B RID: 27 RVA: 0x000022E6 File Offset: 0x000004E6
		public PListFormatException()
		{
		}

		// Token: 0x0600001C RID: 28 RVA: 0x000022EE File Offset: 0x000004EE
		public PListFormatException(string message) : base(message)
		{
		}

		// Token: 0x0600001D RID: 29 RVA: 0x000022F7 File Offset: 0x000004F7
		public PListFormatException(string message, Exception inner) : base(message, inner)
		{
		}
	}
}
