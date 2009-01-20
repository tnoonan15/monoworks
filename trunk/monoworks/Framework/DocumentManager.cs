using System;
using System.Collections.Generic;

namespace MonoWorks.Framework
{
	/// <summary>
	/// Manages documents in an application.
	/// </summary>
	public class DocumentManager<D> where D : IDocument
	{

		protected List<D> docs = new List<D>();

		/// <summary>
		/// Adds a document to be managed.
		/// </summary>
		/// <param name="doc"></param>
		public void Add(D doc)
		{
			docs.Add(doc);
		}

		/// <summary>
		/// Removes a doc from management.
		/// </summary>
		/// <param name="doc"></param>
		public void Remove(D doc)
		{
			docs.Remove(doc);
		}

		/// <summary>
		/// The number of documents being managed.
		/// </summary>
		public int Count
		{
			get { return docs.Count; }
		}

		/// <summary>
		/// The current document.
		/// </summary>
		public D Current
		{
			get
			{
				foreach (D doc in docs)
				{
					if (doc.IsCurrent)
						return doc;
				}
				return default(D);
			}
		}

	}
}
