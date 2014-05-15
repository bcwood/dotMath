using System;
using System.Collections;
using System.Collections.Generic;
using dotMath.Core;
using dotMath.Exceptions;

namespace dotMath
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
	/// EquationCompiler is the class that takes the parsed tokens and turns them
	/// into a network of pre-compiled objects that perform the designated functions.
	/// </summary>
	public class EquationCompiler
	{
		private string _equation;
		private CValue _function;
		private Token _currentToken;
		private Token _nextToken;
		private IEnumerator _tokenEnumerator;
		private Dictionary<string, CVariable> _variables = new Dictionary<string, CVariable>();
		private Dictionary<string, COperator> _operators = new Dictionary<string, COperator>();
		private Dictionary<string, CFunction> _functions = new Dictionary<string, CFunction>();
		
		/// <summary>
		/// Creates the compiler object and sets the current function to the string passed
		/// </summary>
		/// <param name="equation"></param>
		public EquationCompiler(string equation = null)
		{
			SetFunction(equation);
			InitOperators();
			InitFunctions();
		}

		/// <summary>
		/// Sets the object mapped to the string variable name to the double value passed.
		/// </summary>
		/// <param name="name">Variable Name</param>
		/// <param name="value">New Value for variable</param>
		public void SetVariable(string name, double value)
		{
			CVariable variable = GetVariableByName(name);
			variable.SetValue(value);
		}

		/// <summary>
		/// Sets the current function to a passed string.
		/// </summary>
		/// <param name="function">string representing the function being used</param>
		public void SetFunction(string function)
		{
			_currentToken = null;
			_nextToken = null;

			_equation = function;
			_function = null;
		}

		/// <summary>
		/// Kicks off the process to tokenize the function and compile the resulting token set into a runnable form.
		/// </summary>
		public void Compile()
		{
			var parser = new Parser(_equation);
			_tokenEnumerator = parser.GetTokenEnumerator();

			NextToken();

			_function = Relational();
		}

		/// <summary>
		/// Evaluates the function and returns the result.
		/// </summary>
		/// <returns>double value evaluation of the function in its current state</returns>
		public double Calculate()
		{
			if (_function == null)
				Compile();

			if (_nextToken != null)
				throw new InvalidEquationException("Invalid token found in equation: " + _currentToken.ToString());

			return _function.GetValue();
		}

		/// <summary>
		/// Adds a custom function with the given name.
		/// </summary>
		/// <param name="name">Name of function.</param>
		/// <param name="function">Function delegate.</param>
		public void AddFunction(string name, Func<double, double> function)
		{
			_functions.Add(name, new CFunction(function));
		}

		/// <summary>
		/// Adds a custom function with the given name.
		/// </summary>
		/// <param name="name">Name of function.</param>
		/// <param name="function">Function delegate.</param>
		public void AddFunction(string name, Func<double, double, double> function)
		{
			_functions.Add(name, new CFunction(function));
		}

		/// <summary>
		/// Adds a custom function with the given name.
		/// </summary>
		/// <param name="name">Name of function.</param>
		/// <param name="function">Function delegate.</param>
		public void AddFunction(string name, Func<bool, double, double, double> function)
		{
			_functions.Add(name, new CFunction(function));
		}

		#region Operations and Compiling Functions

		/// <summary>
		/// Evaluates parenthesis in the equation and insures they are handled properly according to the Order of Operations. 
		/// Because this is last in the chain, it also evaluates Variable and Function names.
		/// </summary>
		/// <returns>CValue object that holds an operation.</returns>
		private CValue Paren()
		{
			bool isFunction = false;
			CValue value = null;

			if (_currentToken == null)
				throw new InvalidEquationException("Unexpected end of equation (are you missing an operator argument?).");

			if (_currentToken.ToString() == "(")
			{
				NextToken();

				value = Relational();

				if (string.Equals(_currentToken, ","))
					return value;
			}
			else
			{
				switch (_currentToken.TokenType)
				{
					case TokenType.Number:
						value = new CNumber(_currentToken.ToString());
						break;

					case TokenType.Letter:
						if (string.Equals(_nextToken, "("))
						{
							if (!_functions.ContainsKey(_currentToken.ToString()))
								throw new InvalidFunctionException(_currentToken.ToString());

							CFunction function = _functions[_currentToken.ToString()];
							ArrayList parameters = new ArrayList();

							do
							{
								NextToken();

								try
								{
									value = Paren();
								}
								catch (InvalidEquationException)
								{
									throw new ArgumentCountException(parameters.Count);
								}

								parameters.Add(value);
							}
							while (string.Equals(_currentToken, ","));

							isFunction = true;

							function.SetParameters(parameters);
							value = function;
						}
						else
							value = GetVariableByName(_currentToken.ToString());

						break;
				}
			}

			if (!isFunction)
				NextToken();

			return value;
		}

		/// <summary>
		/// Detects the existence of sign operators before a number or variable.  
		/// </summary>
		/// <returns>CValue object representing an operation.</returns>
		private CValue Sign()
		{
			bool isNegative = false;
			Token token = null;

			if (_currentToken == "+" || _currentToken == "-")
			{
				token = _currentToken;
				isNegative = (_currentToken == "-");
				NextToken();
			}

			CValue function = Paren();
			
			if (isNegative)
				function = new CSignNeg(function);

			return function;
		}

		/// <summary>
		/// Detects the operation to raise one number to the power of another (a^2).
		/// </summary>
		/// <returns>CValue object representing an operation.</returns>
		private CValue Power()
		{
			CValue value = Sign();

			while (_currentToken == "^")
			{
				Token token = _currentToken;
				NextToken();

				CValue nextValue = Sign();
				value = GetOperator(token, value, nextValue);
			}

			return value;
		}

		/// <summary>
		/// Detects the modulo operator (%)
		/// </summary>
		/// <returns>CValue object representing an operation.</returns>
		private CValue Modulo()
		{
			CValue value = Power();

			while (_currentToken == "%")
			{
				Token token = _currentToken;
				NextToken();

				CValue nextValue = Power();
				value = GetOperator(token, value, nextValue);
			}

			return value;
		}

		/// <summary>
		/// Detects the operation to perform multiplication or division.
		/// </summary>
		/// <returns>CValue object representing an operation.</returns>
		private CValue MultDiv()
		{
			CValue value = Modulo();

			while (_currentToken == "*" || _currentToken == "/")
			{
				Token token = _currentToken;
				NextToken();

				CValue nextValue = Modulo();
				value = GetOperator(token, value, nextValue);
			}

			return value;
		}

		/// <summary>
		/// Detects the operation to perform addition or substraction.
		/// </summary>
		/// <returns>CValue object representing an operation.</returns>
		private CValue AddSub()
		{
			CValue value = MultDiv();

			while (_currentToken == "+" || _currentToken == "-")
			{
				Token token = _currentToken;
				NextToken();

				CValue nextValue = MultDiv();
				value = GetOperator(token, value, nextValue);
			}

			return value;
		}

		/// <summary>
		/// Detects the operation to perform a relational operator (>, <, <=, etc.).
		/// </summary>
		/// <returns>CValue object representing an operation.</returns>
		private CValue Relational()
		{
			CValue value = AddSub();

			while (_currentToken == "&&" ||
				   _currentToken == "||" ||
				   _currentToken == "==" ||
				   _currentToken == "<"  ||
				   _currentToken == ">"  ||
				   _currentToken == "<=" ||
				   _currentToken == ">=" ||
				   _currentToken == "!=" ||
				   _currentToken == "<>")
			{
				Token token = _currentToken;
				NextToken();

				CValue nextValue = Relational();
				value = GetOperator(token, value, nextValue);
			}

			return value;
		}

		/// <summary>
		/// Reads the passed operator, identifies the associated implementation object
		///	and requests an operation object to be used in evaluating the equation.
		/// </summary>
		/// <param name="operatorToken">Token object representing the operator in question.</param>
		/// <param name="value1">The first value object to be operated on.</param>
		/// <param name="value2">The second value object to be operated on.</param>
		/// <returns>CValue object representing an operation.</returns>
		private CValue GetOperator(Token operatorToken, CValue value1, CValue value2)
		{
			if (_operators.ContainsKey(operatorToken.ToString()))
			{
				COperator op = _operators[operatorToken.ToString()];
				op.SetParameters(value1, value2);
				return op;
			}

			throw new InvalidEquationException("Invalid operator found in equation: " + operatorToken.ToString());
		}

		#endregion

		#region Helper Functions

		/// <summary>
		/// Creates all operation functions recognized by the compiler.
		/// </summary>
		private void InitOperators()
		{
			_operators.Add("+", new COperator((x, y) => x + y));
			_operators.Add("-", new COperator((x, y) => x - y));
			_operators.Add("*", new COperator((x, y) => x * y));
			_operators.Add("/", new COperator((x, y) => x / y));
			_operators.Add("%", new COperator((x, y) => x % y));
			_operators.Add("^", new COperator((x, y) => Math.Pow(x, y)));
			_operators.Add(">", new COperator((x, y) => { if (x > y) return 1; else return 0; }));
			_operators.Add(">=", new COperator((x, y) => { if (x >= y) return 1; else return 0; }));
			_operators.Add("<", new COperator((x, y) => { if (x < y) return 1; else return 0; }));
			_operators.Add("<=", new COperator((x, y) => { if (x <= y) return 1; else return 0; }));
			_operators.Add("=", new COperator((x, y) => { if (x == y) return 1; else return 0; }));
			_operators.Add("==", new COperator((x, y) => { if (x == y) return 1; else return 0; }));
			_operators.Add("<>", new COperator((x, y) => { if (x != y) return 1; else return 0; }));
			_operators.Add("!=", new COperator((x, y) => { if (x != y) return 1; else return 0; }));
		}

		/// <summary>
		/// Creates all functions recognized by the compiler.
		/// </summary>
		private void InitFunctions()
		{
			this.AddFunction("abs", x => Math.Abs(x));
			this.AddFunction("acos", x => Math.Acos(x));
			this.AddFunction("asin", x => Math.Asin(x));
			this.AddFunction("atan", x => Math.Atan(x));
			this.AddFunction("ceiling", x => Math.Ceiling(x));
			this.AddFunction("cos", x => Math.Cos(x));
			this.AddFunction("cosh", x => Math.Cosh(x));
			this.AddFunction("exp", x => Math.Exp(x));
			this.AddFunction("floor", x => Math.Floor(x));
			this.AddFunction("log", x => Math.Log(x));
			this.AddFunction("log10", x => Math.Log10(x));
			this.AddFunction("round", x => Math.Round(x));
			this.AddFunction("sign", x => Math.Sign(x));
			this.AddFunction("sin", x => Math.Sin(x));
			this.AddFunction("sinh", x => Math.Sinh(x));
			this.AddFunction("sqrt", x => Math.Sqrt(x));
			this.AddFunction("tan", x => Math.Tan(x));
			this.AddFunction("tanh", x => Math.Tanh(x));

			this.AddFunction("max", (x, y) => Math.Max(x, y));
			this.AddFunction("min", (x, y) => Math.Min(x, y));
			this.AddFunction("if", (x, y, z) =>
				                       {
					                       if (x) return y;
					                       else return z;
				                       });
		}

		/// <summary>
		/// Manipulates the current Token position forward in the chain of tokens discovered by the parser.
		/// </summary>
		private void NextToken()
		{
			if (_currentToken == null)
			{
				if (!_tokenEnumerator.MoveNext())
					throw new InvalidEquationException("Unexpected end of equation.");

				_nextToken = (Token) _tokenEnumerator.Current;
			}

			_currentToken = _nextToken;

			if (_tokenEnumerator.MoveNext())
				_nextToken = (Token) _tokenEnumerator.Current;
			else
				_nextToken = null;
		}

		/// <summary>
		/// Returns the variable associated with the provided name string.
		/// </summary>
		/// <param name="name">string variable name</param>
		/// <returns>CVariable object mapped to the passed variable name</returns>
		private CVariable GetVariableByName(string name)
		{
			if (_variables.ContainsKey(name))
				return _variables[name];

			var variable = new CVariable();
			_variables.Add(name, variable);

			return variable;
		}

		#endregion
	}
}
