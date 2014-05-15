using System;

namespace dotMath.Exceptions
{
	public class InvalidEquationException : Exception
	{
		/// <summary>
		/// Initializes a new instance of the InvalidEquationException class with a specified error message. 
		/// </summary>
		/// <param name="message">The message that describes the error.</param>
		public InvalidEquationException(string message)
			: base(message) { }
	}
}
