using dotMath.Exceptions;

namespace dotMath.Core
{
	/// <summary>
	/// Represents the various types of character types known by the parser.
	/// </summary>
	internal enum TokenType
	{
		Delimeter,
		Whitespace,
		Number,
		Letter,
		Undefined
	}

	/// <summary>
	/// Represents a collection of 1 or more characters that form a given set of singly definable characters.
	/// </summary>
	internal class Token
	{
		private string _value;
		public TokenType TokenType { get; private set; }

		/// <summary>
		/// Creates a new token object with the passed token string and character type information.
		/// </summary>
		/// <param name="value">string representing the token value</param>
		/// <param name="tokenType">Character enumeration describing the type of data in the token.</param>
		public Token(string value = null, TokenType tokenType = TokenType.Undefined)
		{
			_value = value;
			TokenType = tokenType;
		}

		/// <summary>
		/// Tests for equality with another token.  Also handles the null case.
		/// </summary>
		/// <param name="sValue">string value representing the token being evaluated against.</param>
		/// <returns>bool where true means both objects are equal.</returns>
		public override bool Equals(object obj)
		{
			if (obj == null)
				return _value == null;

			if ((string) obj == _value)
				return true;
			else
				return false;
		}

		/// <summary>
		/// This overloaded operator checks for equality between a token and a string.
		/// </summary>
		/// <param name="token">Token - actual token object</param>
		/// <param name="value">string - token value to test against token object value</param>
		/// <returns></returns>
		public static bool operator ==(Token token, string value)
		{
			// the following tests are order dependent.  If both are null, eq is true.  
			// if only one is null the eq is false.

			if ((object) token == null && value == null)
				return true;

			if ((object) token == null || value == null)
				return false;

			if (token._value == value)
				return true;
			else
				return false;
		}

		/// <summary>
		/// Overloaded operator that checks for inequality between the token value and the passed string.
		/// </summary>
		/// <param name="token">Token object being test</param>
		/// <param name="value">string that is being compared with the Token object's string value</param>
		/// <returns>bool indicating true for inequality</returns>
		public static bool operator !=(Token token, string value)
		{
			if (token == null || value == null)
				return !(token == null && value == null);

			if (token._value != value)
				return true;
			else
				return false;
		}

		/// <summary>
		/// Returns the string name of the token.
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return _value;
		}

		public static TokenType GetTypeByChar(char c)
		{
			const string WHITESPACE = " \t";
			const string DELIMITERS = "+-*/^%()<>=&|!,";
			const string NUMBERS = ".0123456789";
			const string LETTERS = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz_";
			
			if (WHITESPACE.IndexOf(c) >= 0)
				return TokenType.Whitespace;
			if (DELIMITERS.IndexOf(c) >= 0)
				return TokenType.Delimeter;
			if (NUMBERS.IndexOf(c) >= 0)
				return TokenType.Number;
			if (LETTERS.IndexOf(c) >= 0)
				return TokenType.Letter;

			throw new InvalidEquationException("Invalid token found in equation: " + c);
		}
	}
}
