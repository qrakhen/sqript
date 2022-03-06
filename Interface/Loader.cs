using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;

namespace Qrakhen.Sqript
{
	internal static class Loader
	{
		public static Interface[] loadLibrary(string file) {
			var r = new List<Interface>();
			var assembly = AssemblyLoadContext.Default.LoadFromAssemblyPath(file);
			var types = assembly.GetExportedTypes();
			foreach (var type in types) {
				foreach (var member in type.GetMembers()) {
					if (member.Name == "load") {
						r.Add((Interface) Activator.CreateInstance(type));
						break;
					}
				}
			}
			return r.ToArray();
		}

		public static Interface[] loadDirectory(string path, string filter = "*.dll") {
			if (!Directory.Exists(path)) {
				Log.warn("default library folder lib/ not found - could not load external libraries.");
				return new Interface[0];
			}
			var r = new List<Interface>();
			var files = Directory.GetFiles(path, "*.dll", SearchOption.AllDirectories);
			foreach (var file in files) {
				try {
					r.AddRange(loadLibrary(file));
				} catch(Exception e) {
					Log.error("error while trying to load external library '" + file + "':\n" + e);
				}
			}
			return r.ToArray();
		}
	}
}
