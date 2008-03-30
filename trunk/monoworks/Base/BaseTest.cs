//    /home/andy/csharp/MonoWorks/base/BaseTest.cs - MonoWorks Project
//
//    Copyright Andy Selvig 2008
//
//    This program is free software: you can redistribute it and/or modify
//    it under the terms of the GNU Lesser General Public License as published 
//    by the Free Software Foundation, either version 3 of the License, or
//    (at your option) any later version.
//
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU Lesser General Public License for more details.
//
//    You should have received a copy of the GNU Lesser General Public 
//    License along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using NUnit.Framework;
using MonoWorks.Base;

namespace MonoWorks
{
	
	
	/// <summary>
	/// All tests for the Base assembly.
	/// </summary>
	[TestFixture()]
	public class BaseTest
	{
		
		/// <summary>
		/// Test for dimensional arithmatic.
		/// </summary>
		[Test()]
		public void Dimensionals()
		{
			Length a = new Length(1);
			Length b = new Length();
			b["cm"] = 50;
			Length c = a+b;
			Assert.AreEqual(c["mm"], 1500);
			c = a * 1.2;
			Assert.AreEqual(c["m"], 1.2);
		}
		
		/// <summary>
		/// Tests that dimensionals properly throw a UnitException when an
		/// invalid unit is accessed.
		/// </summary>
		[Test()]
		[ExpectedException(typeof(UnitException))]
		public void ExpectUnitException()
		{
			Length a = new Length(1);
			a["invalid"] = 42;
		}
		
		
		/// <summary>
		/// Test for angle trigonometry.
		/// </summary>
		[Test()]
		public void Trig()
		{
			Angle a = new Angle(Angle.PI/6.0);
			Assert.AreEqual(0.5,a.Sin(),  0.0001);
			Assert.AreEqual(30, a["deg"], 0.0001);
		}
		
		
		/// <summary>
		/// Test for the Point class.
		/// </summary>
		[Test()]
		public void Points()
		{
			Vector p = new Vector(1.0, 2.0, 3.0);
			Assert.AreEqual(p[1], 2.0);
		}
		
	}
}
