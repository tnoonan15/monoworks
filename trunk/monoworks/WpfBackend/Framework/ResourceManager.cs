using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.IO;
using System.Reflection;

namespace MonoWorks.WpfBackend.Framework
{
	/// <summary>
	/// Resource manager for WPF applications.
	/// </summary>
	public class ResourceManager : MonoWorks.Framework.ResourceManagerBase
	{

		protected ResourceManager()
			: base()
		{

		}

		#region Singleton

		/// <summary>
		/// The singleton instance.
		/// </summary>
		protected static ResourceManager Instance;

		/// <summary>
		/// Safely initializes the global resource manager.
		/// </summary>
		public static void Initialize()
		{
			if (Instance == null)
				Instance = new ResourceManager();
		}

		/// <summary>
		/// Loads the given directory.
		/// </summary>
		/// <remarks>The manager will be automatically initialized.</remarks>
		public static void LoadDirectory(string dirName)
		{
			Initialize();
			Instance.LoadDir(dirName);
		}

		/// <summary>
		/// Loads an assembly.
		/// </summary>
		/// <param name="asmName">The name of the assembly.</param>
		/// <remarks>The assembly should have generally the same layout 
		/// as a resource directory should.</remarks>
		public static new void LoadAssembly(string asmName)
		{
			Initialize();
			Instance.LoadAsm(asmName);
		}

		#endregion



		#region Icons

		/// <summary>
		/// The icons.
		/// </summary>
		protected Dictionary<string, Icon> icons = new Dictionary<string, Icon>();

		protected override void LoadIcon(FileInfo fileInfo, int size)
		{
			string name = fileInfo.Name.Split('.')[0];
			Icon icon;
			if (icons.ContainsKey(name))
				icon = icons[name];
			else
			{
				icon = new Icon();
				icon.Name = name;
				icons[name] = icon;
			}
			icon.AddFile(fileInfo.FullName);
		}

		protected override void LoadIconStream(Stream stream, string name)
		{
			Icon icon;
			if (icons.ContainsKey(name))
				icon = icons[name];
			else
			{
				icon = new Icon();
				icon.Name = name;
				icons[name] = icon;
			}
			icon.AddStream(stream);
		}

		/// <summary>
		/// Get an icon by name.
		/// </summary>
		/// <param name="name"> The name of the icon.</param>
		/// <returns> The icon.</returns>
		protected Icon GetIcon(string name)
		{
			if (!icons.ContainsKey(name))
				throw new Exception(String.Format("The resource manager doesn't contain an icon called {0}", name));
			return icons[name];
		}

		/// <summary>
		/// Renders an icon to an image of the given size.
		/// </summary>
		/// <param name="name"> The name of the icon.</param>
		/// <param name="size"> The icon size.</param>
		/// <returns></returns>
		public static Image RenderIcon(string name, int size)
		{
			EnsureInitialized();
			return Instance.GetIcon(name).Render(size);
		}


		public override void WriteLoadedIconToFile(string fileName, string iconName, int size)
		{
			GetIcon(iconName).RenderToFile(fileName, size);
		}

		#endregion


    }
}
