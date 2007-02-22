using System;

namespace NProf.Glue.Profiler.Project
{
	/// <summary>
	/// Specifies a type of project.
	/// </summary>
	public enum ProjectType
	{
		/// <summary>
		/// The project references a file on the filesystem.
		/// </summary>
		File,
		/// <summary>
		/// The project is run from VS.NET.
		/// </summary>
		VSNet,
		/// <summary>
		/// The project attaches to ASP.NET.
		/// </summary>
		AspNet,
	}
}
