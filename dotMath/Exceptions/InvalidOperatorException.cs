using System;

namespace dotMath.Exceptions
{
	public class InvalidOperatorException : Exception
	{
		/// <summary>
		/// Initializes a new instance of the InvalidOperatorException class for the specified operator name.
		/// </summary>
		/// <param name="name">Name of the invalid operator.</param>
		public InvalidOperatorException(string name)
			: base("Invalid operator: " + name) { }
	}
}
