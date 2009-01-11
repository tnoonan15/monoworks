using System;
using System.Collections.Generic;

namespace MonoWorks.Model
{
	/// <summary>
	/// Interface for objects that want to keep track of the entities in a document.
	/// </summary>
	public interface IEntityListener
	{

		void OnEntityAdded(Entity entity);

		void OnEntityDeleted(Entity entity);


	}
}
