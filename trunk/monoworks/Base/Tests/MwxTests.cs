// 
//  MwxTests.cs - MonoWorks Project
//  
//  Author:
//       Andy Selvig <ajselvig@gmail.com>
// 
//  Copyright (c) 2010 Andy Selvig
// 
//  This library is free software; you can redistribute it and/or modify
//  it under the terms of the GNU Lesser General Public License as
//  published by the Free Software Foundation; either version 2.1 of the
//  License, or (at your option) any later version.
// 
//  This library is distributed in the hope that it will be useful, but
//  WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU
//  Lesser General Public License for more details.
// 
//  You should have received a copy of the GNU Lesser General Public
//  License along with this library; if not, write to the Free Software
//  Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA 02111-1307 USA

using System;
using NUnit.Framework;

using MonoWorks.Base;

namespace MonoWorks.Base.Tests
{
	/// <summary>
	/// Tests for Mwx object handling.
	/// </summary>
	[TestFixture()]
	public class MwxTests
	{
		[Test()]
		public void TestDeepCopy()
		{
			var obj = new MwxTestObject() {
				Name = "Test Object"
			};
			obj.AddChild(new MwxTestObject() {
				Name = "Child 1"
			});
			
			var copier = new MwxDeepCopier();
			var newObj = copier.DeepCopy<MwxTestObject>(obj);
			
			Assert.AreEqual("Test Object", newObj.Name);
			obj.Name = "Modified Name";
			Assert.AreEqual("Test Object", newObj.Name);
			
			Assert.AreEqual(1, newObj.GetMwxChildren().Count);
			obj.GetMwxChildren()[0].Name = "New child 1";
			Assert.AreEqual("Child 1", newObj.GetMwxChildren()[0].Name);
		}
	}
}

