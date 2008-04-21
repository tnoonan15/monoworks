//   Material.cs - MonoWorks Project
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

using gl = Tao.OpenGl.Gl;

namespace MonoWorks.Model
{
	
	/// <summary>
	/// The Material class represents a material for a feature.
	/// It stores color and texture information.
	/// </summary>
	public class Material
	{
		/// <summary>
		/// Default constructor.
		/// </summary>
		public Material()
		{
		}
		
		
#region Attributes

		protected Color ambientColor;
		/// <value>
		/// The ambient color.
		/// </value>
		public Color AmbientColor
		{
			get {return ambientColor;}
			set {ambientColor = value;}
		}

		protected Color diffuseColor;
		/// <value>
		/// The ambient diffuse color.
		/// </value>
		public Color DiffuseColor
		{
			get {return diffuseColor;}
			set {diffuseColor = value;}
		}
				
#endregion
		
		
#region OpenGL Setup
		
		/// <summary>
		/// Sets the material properties in the current OpenGL context.
		/// </summary>
		public virtual void Setup()
		{
			
		}
		
#endregion
		
	}
}
