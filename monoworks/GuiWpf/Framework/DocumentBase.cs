using System;
using System.Collections.Generic;

using AvalonDock;

using MonoWorks.Framework;

namespace MonoWorks.WpfBackend.Framework
{
    /// <summary>
    /// Base class for document types.
    /// </summary>
    public class DocumentBase : DocumentContent, IDocument
    {

		public bool IsCurrent
		{
			get	{return IsActiveDocument;}
		}

    }
}
