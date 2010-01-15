using System;
using System.Collections.Generic;

namespace MonoWorks.Modeling
{
	/// <summary>
	/// Interface for objects that want to keep track of the entities in a document.
	/// </summary>
	public interface IEntityListener
	{

		void AddEntity(Entity entity);

		void RemoveEntity(Entity entity);


	}
}
