using System;

namespace dotMath.Exceptions
{
	public class InvalidFunctionException : Exception
	{
		/// <summary>
		/// Initializes a new instance of the InvalidFunctionException class for the specified function name.
		/// </summary>
		/// <param name="name">Name of the invalid function.</param>
		public InvalidFunctionException(string name)
			: base("Invalid function: " + name) { }
	}
}
