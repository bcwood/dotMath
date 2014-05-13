using System;
using System.Collections;

namespace dotMath.Core
{
	/// <remarks>
	/// Copyright (c) 2001-2004, Stephen Hebert
	/// Copyright (c) 2014, Brandon Wood
	/// All rights reserved.
	/// 
	/// 
	/// Redistribution and use in source and binary forms, with or without modification, 
	/// are permitted provided that the following conditions are met:
	/// 
	/// Redistributions of source code must retain the above copyright notice, 
	/// this list of conditions and the following disclaimer. 
	/// 
	/// Redistributions in binary form must reproduce the above 
	/// copyright notice, this list of conditions and the following disclaimer 
	/// in the documentation and/or other materials provided with the distribution. 
	/// 
	/// Neither the name of the .Math, nor the names of its contributors 
	/// may be used to endorse or promote products derived from this software without 
	/// specific prior written permission. 
	/// 
	/// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS 
	/// "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED 
	/// TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
	/// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS 
	/// BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
	/// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF 
	/// SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; 
	/// OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, 
	/// WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR 
	/// OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, 
	/// EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
	/// 
	/// </remarks>
	/// 
	/// <summary>
	/// Responsible for traversing a given function and creating an enumerable set of tokens.
	/// </summary>
	internal class Parser
	{
		private string _function;
		private ArrayList _tokens;

		//private string m_sWhitespace = " \t";
		//private string m_sDelimiters = "+-*/^()<>=&|!,";
		//private string m_sNumbers = ".0123456789";
		//private string m_sLetters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz%_";
		//private string m_sBrackets = "[]";
		
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
			TokenType tokenType = TokenType.Undefined;
			string token = "";

			foreach (char current in _function)
			{
				switch (Token.GetTypeByChar(current))
				{
					case TokenType.Whitespace:
						if (token.Length > 0)
							_tokens.Add(new Token(token, tokenType));

						token = "";
						break;

					case TokenType.Delimeter:
						if (token.Length > 0)
							_tokens.Add(new Token(token, tokenType));

						_tokens.Add(new Token(current.ToString(), TokenType.Delimeter));

						token = "";
						tokenType = TokenType.Undefined;
						break;

					case TokenType.Brackets:
						if (token.Length > 0)
							_tokens.Add(new Token(token, tokenType));

						_tokens.Add(new Token(current.ToString(), TokenType.Brackets));

						token = "";
						tokenType = TokenType.Undefined;
						break;

					case TokenType.Number:
						if (token.Length == 0)
							tokenType = TokenType.Number;

						token += current;
						break;

					case TokenType.Letter:
						if (token.Length == 0)
							tokenType = TokenType.Letter;
						else if (tokenType != TokenType.Letter)
							tokenType = TokenType.Undefined;

						token += current;
						break;

					case TokenType.Undefined:
						if (token.Length > 0)
							_tokens.Add(new Token(token, tokenType));

						token = "";
						tokenType = TokenType.Undefined;
						break;
				}
			}

			if (token.Length > 0)
				_tokens.Add(new Token(token, tokenType));

			CheckMultiCharOps();
		}

		/// <summary>
		/// Checks for multi character operations that together take on a different meaning than simply by themselves.  
		/// This can reorganize the entire ArrayList by the time it is complete.
		/// </summary>
		private void CheckMultiCharOps()
		{
			ArrayList alTokens = new ArrayList();
			IEnumerator iEnum = GetTokenEnumerator();
			Token nextToken1 = null;
			Token nextToken2 = null;

			if (iEnum.MoveNext())
				nextToken1 = (Token) iEnum.Current;

			if (iEnum.MoveNext())
				nextToken2 = (Token) iEnum.Current;

			while (nextToken1 != null)
			{
				if (nextToken1.TokenType == TokenType.Delimeter)
				{
					if (nextToken2 != null && nextToken2.TokenType == TokenType.Delimeter)
					{
						string s1 = nextToken1.ToString() + nextToken2.ToString();

						if (s1 == "&&" ||
							s1 == "||" ||
							s1 == "<=" ||
							s1 == ">=" ||
							s1 == "!=" ||
							s1 == "<>" ||
							s1 == "==")
						{
							nextToken1 = new Token(s1, TokenType.Delimeter);
							
							if (iEnum.MoveNext())
								nextToken2 = (Token) iEnum.Current;
						}
					}
				}

				alTokens.Add(nextToken1);

				nextToken1 = nextToken2;

				if (nextToken2 != null)
				{
					if (iEnum.MoveNext())
						nextToken2 = (Token) iEnum.Current;
					else
						nextToken2 = null;
				}
				else
					nextToken1 = null;
			}

			_tokens = alTokens;
		}
	}
}
