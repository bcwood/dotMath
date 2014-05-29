using System;
using System.Collections;
using dotMath.Exceptions;

namespace dotMath.Core
{
	/// <summary>
	/// Responsible for traversing a given function and creating an enumerable set of tokens.
	/// </summary>
	internal class Parser
	{
		private string _function;
		private ArrayList _tokens;
		private Stack _parentheses;

		/// <summary>
		/// Takes an expression and launches the parsing process.
		/// </summary>
		/// <param name="function">The expression string to be parsed.</param>
		public Parser(string function)
		{
			_function = function;

			this.Parse();
		}

		/// <summary>
		/// Gets an enumerator associated with the token collection.
		/// </summary>
		/// <returns>IEnumerator object of the Token ArrayList</returns>
		public IEnumerator GetTokenEnumerator()
		{
			return _tokens.GetEnumerator();
		}

		/// <summary>
		/// Kicks off the parsing process of the provided function.
		/// </summary>
		private void Parse()
		{
			_tokens = new ArrayList();
			_parentheses = new Stack();
			string token = "";
			TokenType tokenType = TokenType.Undefined;
			
			foreach (char currentChar in _function)
			{
				switch (Token.GetTypeByChar(currentChar))
				{
					case TokenType.Whitespace:
						if (!string.IsNullOrEmpty(token))
							_tokens.Add(new Token(token, tokenType));

						token = "";
						break;

					case TokenType.Delimeter:
						if (!string.IsNullOrEmpty(token))
							_tokens.Add(new Token(token, tokenType));

						_tokens.Add(new Token(currentChar.ToString(), TokenType.Delimeter));

						token = "";
						tokenType = TokenType.Undefined;
						break;

					case TokenType.Number:
						if (string.IsNullOrEmpty(token))
							tokenType = TokenType.Number;

						token += currentChar;
						break;

					case TokenType.Letter:
						if (string.IsNullOrEmpty(token))
							tokenType = TokenType.Letter;
						
						token += currentChar;
						break;
				}

				// use a stack to keep track of parentheses depth/matching
				if (currentChar == '(')
					_parentheses.Push(currentChar);
				else if (currentChar == ')')
				{
					try
					{
						_parentheses.Pop();
					}
					catch (InvalidOperationException)
					{
						throw new UnmatchedParenthesesException();
					}
				}
			}

			if (token.Length > 0)
				_tokens.Add(new Token(token, tokenType));

			// check for unmatched parentheses
			if (_parentheses.Count > 0)
				throw new UnmatchedParenthesesException();

			CheckMultiCharOps();
		}

		/// <summary>
		/// Checks for multi character operations that together take on a different meaning than simply by themselves.  
		/// This can reorganize the entire ArrayList by the time it is complete.
		/// </summary>
		private void CheckMultiCharOps()
		{
			ArrayList tokens = new ArrayList();
			IEnumerator tokenEnumerator = GetTokenEnumerator();
			Token token1 = null;
			Token token2 = null;

			if (tokenEnumerator.MoveNext())
				token1 = (Token) tokenEnumerator.Current;

			if (tokenEnumerator.MoveNext())
				token2 = (Token) tokenEnumerator.Current;

			while (token1 != null)
			{
				if (token1.TokenType == TokenType.Delimeter)
				{
					if (token2 != null && token2.TokenType == TokenType.Delimeter)
					{
						string s1 = token1.ToString() + token2.ToString();

						if (s1 == "&&" ||
							s1 == "||" ||
							s1 == "<=" ||
							s1 == ">=" ||
							s1 == "!=" ||
							s1 == "<>" ||
							s1 == "==")
						{
							token1 = new Token(s1, TokenType.Delimeter);
							
							if (tokenEnumerator.MoveNext())
								token2 = (Token) tokenEnumerator.Current;
						}
					}
				}

				tokens.Add(token1);

				token1 = token2;

				if (token2 != null)
				{
					if (tokenEnumerator.MoveNext())
						token2 = (Token) tokenEnumerator.Current;
					else
						token2 = null;
				}
				else
					token1 = null;
			}

			_tokens = tokens;
		}
	}
}
