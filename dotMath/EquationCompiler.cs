using System;
using System.Collections;
using System.Collections.Generic;
using dotMath.Core;

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
		private IEnumerator tokenEnumerator;
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
		/// Reports the current variable count.  It is valid after a 'Compile()' function is executed.
		/// </summary>
		public int VariableCount { get { return _variables.Count; } }

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
			tokenEnumerator = parser.GetTokenEnumerator();

			NextToken();

			_function = Relational();
		}

		/// <summary>
		/// Calls into the runnable function set to evaluate the function and returns the result.
		/// </summary>
		/// <returns>double value evaluation of the function in its current state</returns>
		public double Calculate()
		{
			if (_function == null)
				Compile();

			return _function.GetValue();
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

			if (_currentToken.ToString() == "(")
			{
				NextToken();

				value = Relational();

				if (_currentToken.ToString() == ",")
					return value;

				if (_currentToken.ToString() != ")")
					throw new ApplicationException("Unmatched parenthesis in equation.");
			}
			else
			{
				switch (_currentToken.TokenType)
				{
					case TokenType.Number:
						value = new CNumber(_currentToken.ToString());
						break;

					case TokenType.Letter:
						if (_nextToken.ToString() == "(")
						{
							if (!_functions.ContainsKey(_currentToken.ToString()))
								throw new ApplicationException(string.Format("Function '{0}' not found.", _currentToken.ToString()));

							CFunction function = _functions[_currentToken.ToString()];
							ArrayList parameters = new ArrayList();

							do
							{
								NextToken();
								value = Paren();

								parameters.Add(value);
							}
							while (_currentToken.ToString() == ",");

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
			{
				ValidateParameters(token, function);
				function = new CSignNeg(function);
			}

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
				ValidateParameters(token, value, nextValue);
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
			CValue value = Power();

			while (_currentToken == "*" || _currentToken == "/")
			{
				Token token = _currentToken;
				NextToken();

				CValue nextValue = Power();
				ValidateParameters(token, value, nextValue);
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
				ValidateParameters(token, value, nextValue);
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
				ValidateParameters(token, value, nextValue);
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

			throw new ApplicationException(string.Format("Invalid operator {0} in equation.", operatorToken.ToString()));
		}

		#endregion

		#region Helper Functions

		/// <summary>
		/// Validates that the arguments are non-null, and raises an exception if they are.
		/// </summary>
		/// <param name="token">Currently processed Token object</param>
		/// <param name="param1">CValue argument 1</param>
		/// <param name="param2">CValue argument 2</param>
		private void ValidateParameters(Token token, CValue param1, CValue param2)
		{
			if (param1 == null || param2 == null)
				throw new ApplicationException("Argument not supplied near " + token.ToString() + " operation.");
		}

		/// <summary>
		/// Validates that the single argument is non-null, and raises an exception if it is.
		/// </summary>
		/// <param name="token">Token object</param>
		/// <param name="param1">CValue argument</param>
		private void ValidateParameters(Token token, CValue param1)
		{
			if (param1 == null)
				throw new ApplicationException("Argument not supplied for " + token.ToString() + " operation.");
		}

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
			_functions.Add("abs", new CFunction(x => Math.Abs(x)));
			_functions.Add("acos", new CFunction(x => Math.Acos(x)));
			_functions.Add("asin", new CFunction(x => Math.Asin(x)));
			_functions.Add("atan", new CFunction(x => Math.Atan(x)));
			_functions.Add("ceiling", new CFunction(x => Math.Ceiling(x)));
			_functions.Add("cos", new CFunction(x => Math.Cos(x)));
			_functions.Add("cosh", new CFunction(x => Math.Cosh(x)));
			_functions.Add("exp", new CFunction(x => Math.Exp(x)));
			_functions.Add("floor", new CFunction(x => Math.Floor(x)));
			_functions.Add("log", new CFunction(x => Math.Log(x)));
			_functions.Add("log10", new CFunction(x => Math.Log10(x)));
			_functions.Add("round", new CFunction(x => Math.Round(x)));
			_functions.Add("sign", new CFunction(x => Math.Sign(x)));
			_functions.Add("sin", new CFunction(x => Math.Sin(x)));
			_functions.Add("sinh", new CFunction(x => Math.Sinh(x)));
			_functions.Add("sqrt", new CFunction(x => Math.Sqrt(x)));
			_functions.Add("tan", new CFunction(x => Math.Tan(x)));
			_functions.Add("tanh", new CFunction(x => Math.Tanh(x)));

			// TODO
			//eq.AddFunction("max", (x, y) => Math.Max(x, y));
			//eq.AddFunction("min", (x, y) => Math.Min(x, y));
			//eq.AddFunction("if", (x, y, z) => if (x) return y; else return z;);
		}

		/// <summary>
		/// Manipulates the current Token position forward in the chain of tokens discovered by the parser.
		/// </summary>
		private void NextToken()
		{
			if (_currentToken == null)
			{
				if (!tokenEnumerator.MoveNext())
					throw new ApplicationException("Invalid equation.");

				_nextToken = (Token) tokenEnumerator.Current;
			}

			_currentToken = _nextToken;

			if (tokenEnumerator.MoveNext())
				_nextToken = (Token) tokenEnumerator.Current;
			else
				_nextToken = new Token();
		}


		/// <summary>
		/// Returns the variable associated with the provided name string.
		/// </summary>
		/// <param name="name">string variable name</param>
		/// <returns>CVariable object mapped to the passed variable name</returns>
		private CVariable GetVariableByName(string name)
		{
			if (_variables == null)
				_variables = new Dictionary<string, CVariable>();

			if (_variables.ContainsKey(name))
				return _variables[name];

			var variable = new CVariable();
			_variables.Add(name, variable);

			return variable;
		}

		#endregion
	}
}
