// 
//  DrawingTests.cs - MonoWorks Project
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

using MonoWorks.Modeling;

namespace MonoWorks.Modeling.Tests
{
	/// <summary>
	/// Test fixture for drawings.
	/// </summary>
	[TestFixture]
	public class DrawingTests
	{
		public DrawingTests()
		{
		}
		
		/// <summary>
		/// Tests reading and writing a part to a mwp file.
		/// </summary>
		[Test]
		public void PartIo()
		{
			var part = new TestPart();
			part.SaveAs("test.mwp");
			
			var newPart = Drawing.FromFile("test.mwp") as Part;
//			Assert.AreEqual(typeof(Sketching.Sketch), newPart.Gets)
		}
		
	}
}

