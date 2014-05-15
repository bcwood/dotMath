using System;

namespace dotMath.Exceptions
{
	public class UnmatchedParenthesesException : Exception
	{
		/// <summary>
		/// Initializes a new instance of the UnmatchedParenthesesException class.
		/// </summary>
		public UnmatchedParenthesesException()
			: base("Unmatched parentheses in equation.") { }
	}
}
