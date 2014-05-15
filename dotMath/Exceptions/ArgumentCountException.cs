using System;

namespace dotMath.Exceptions
{
	public class ArgumentCountException : Exception
	{
		/// <summary>
		/// Initializes a new instance of the ArgumentCountException class. An exception of this type means that
		/// the incorrect number of arguments were passed to a function. 
		/// </summary>
		/// <param name="count">The number of arguments passed to the function.</param>
		public ArgumentCountException(int count)
			: base("Unexpected number of function arguments: " + count) { }
	}
}
