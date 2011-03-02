using System;
using System.IO;
using System.Reflection;

namespace UnitTests
{
	public static class AssemblyExtensions
	{
		public static string Directory(this Assembly assembly)
		{
			return Path.GetDirectoryName(Uri.UnescapeDataString(new UriBuilder(assembly.CodeBase).Path));
		}
	}
}