using System;
using System.Collections;
using System.Collections.Generic;

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
	/// EqCompiler is the class that takes the parsed tokens and turns them
	/// into a network of pre-compiled objects that perform the designated
	/// functions.
	/// </summary>
	public class EqCompiler
	{
		private string m_sEquation;
		private CValue m_Function;
		private Parser.Token m_currentToken;
		private Parser.Token m_nextToken;
		private IEnumerator m_enumTokens;
		private SortedList m_slVariables = new SortedList();
		private Dictionary<string, COperator> _operators = new Dictionary<string, COperator>();
		private Dictionary<string, CFunction> _functions = new Dictionary<string, CFunction>();
		
		/// <summary>
		/// EqCompiler(string) constructor: creates the compiler object and sets the current function to the string passed
		/// </summary>
		/// <param name="sEquation"></param>
		/// <param name="bIncludeStandardFunctions"></param>
		public EqCompiler(string sEquation = null, bool bIncludeStandardFunctions = true)
		{
			SetFunction(sEquation);
			InitOperators();

			if (bIncludeStandardFunctions)
				InitFunctions();
		}

		/// <summary>
		/// VariableCount property: This property reports the current
		///		variable count.  It is valid after a 'Compile()' function is executed.
		/// </summary>
		public int VariableCount { get { return m_slVariables.Count; } }

		/// <summary>
		/// SetVariable( string, double):  Sets the object mapped to the string variable name
		///		to the double value passed.
		/// </summary>
		/// <param name="sVarName">Variable Name</param>
		/// <param name="dValue">New Value for variable</param>
		public void SetVariable(string sVarName, double dValue)
		{
			CVariable oVar = GetVariableByName(sVarName);
			oVar.SetValue(dValue);
		}

		/// <summary>
		/// GetVariableList(): returns a string array containing all the variables that
		///		have been found by the compiler.
		/// </summary>
		/// <returns>string array of current variable names</returns>
		public string[] GetVariableList()
		{
			if (m_slVariables.Count == 0)
				return null;

			string[] asVars = new string[m_slVariables.Count];

			IEnumerator enu = m_slVariables.GetKeyList().GetEnumerator();

			string sValue = "";
			int iPos = 0;

			while (enu.MoveNext())
			{
				sValue = (string) enu.Current;

				asVars[iPos] = sValue;
				iPos++;
			}

			return asVars;
		}

		/// <summary>
		/// SetFunction(string): Sets the current function to a passed string.
		/// </summary>
		/// <param name="sEquation">string representing the function being used</param>
		public void SetFunction(string sEquation)
		{
			m_currentToken = null;
			m_nextToken = null;

			m_sEquation = sEquation;
			m_Function = null;
		}

		/// <summary>
		/// Compile():  This function kicks off the process to tokenize the function
		///		and compile the resulting token set into a runnable form.
		/// </summary>
		public void Compile()
		{
			Parser oParser = new Parser(m_sEquation);
			m_enumTokens = oParser.GetTokenEnumerator();

			PositionNextToken();

			m_Function = Relational();
		}

		/// <summary>
		/// Calculate():  Calls into the runnable function set to evaluate the function and returns the result.
		/// </summary>
		/// <returns>double value evaluation of the function in its current state</returns>
		public double Calculate()
		{
			if (m_Function == null)
				Compile();

			return m_Function.GetValue();
		}

		#region Operations and Compiling Functions

		/// <summary>
		/// Paren() : This method evaluates Parenthesis in the equation and
		///		insures they are handled properly according to the Order of Operations. Because this is last in the chain,
		///		it also evaluates Variable and Function names.
		/// </summary>
		/// <returns>CValue object that holds an operation.</returns>
		private CValue Paren()
		{
			bool bFunc = false;
			CValue oValue = null;

			if (m_currentToken.ToString() == "(")
			{
				PositionNextToken();

				oValue = Relational();

				if (m_currentToken.ToString() == ",")
					return oValue;

				if (m_currentToken.ToString() != ")")
					throw new ApplicationException("Unmatched parenthesis in equation.");
			}
			else
			{
				switch (m_currentToken.TokenType)
				{
					case Parser.CharType.CT_NUMBER:
						oValue = new CNumber(m_currentToken.ToString());
						break;

					case Parser.CharType.CT_LETTER:
						if (m_nextToken.ToString() == "(")
						{
							if (!_functions.ContainsKey(m_currentToken.ToString()))
								throw new ApplicationException("Function not found - " + m_currentToken.ToString());

							CFunction oFunc = _functions[m_currentToken.ToString()];

							ArrayList alValues = new ArrayList();

							do
							{
								PositionNextToken();
								oValue = Paren();

								alValues.Add(oValue);
							}
							while (m_currentToken.ToString() == ",");

							bFunc = true;

							oFunc.SetParameters(alValues);
							oValue = oFunc;
						}
						else
							oValue = GetVariableByName(m_currentToken.ToString());

						break;
				}
			}

			if (!bFunc)
				PositionNextToken();

			return oValue;
		}

		/// <summary>
		/// Sign():  This method detects the existence of sign operators before
		///		a number or variable.  
		/// </summary>
		/// <returns>CValue object representing an operation.</returns>
		private CValue Sign()
		{
			bool bNeg = false;
			Parser.Token oToken = null;

			if (m_currentToken == "+" || m_currentToken == "-")
			{
				oToken = m_currentToken;
				bNeg = (m_currentToken == "-");
				PositionNextToken();
			}

			//CValue oFunc = Function();
			// sdh: should be function when ready.
			CValue oFunc = Paren();
			
			if (bNeg)
			{
				CheckParms(oToken, oFunc);
				oFunc = new CSignNeg(oFunc);
			}

			return oFunc;
		}

		/// <summary>
		/// Power():  Detects the operation to raise one number to the power
		///		of another (a^2).
		/// </summary>
		/// <returns>CValue object representing an operation.</returns>
		private CValue Power()
		{
			CValue oValue = Sign();

			while (m_currentToken == "^")
			{
				Parser.Token oOp = m_currentToken;

				PositionNextToken();

				CValue oNextVal = Sign();

				CheckParms(oOp, oValue, oNextVal);
				oValue = OpFactory(oOp, oValue, oNextVal);
			}

			return oValue;
		}

		/// <summary>
		/// MultDiv(): Detects the operation to perform multiplication or division.
		/// </summary>
		/// <returns>CValue object representing an operation.</returns>
		private CValue MultDiv()
		{
			CValue oValue = Power();

			while (m_currentToken == "*" || m_currentToken == "/")
			{
				Parser.Token oOp = m_currentToken;

				PositionNextToken();

				CValue oNextVal = Power();

				CheckParms(oOp, oValue, oNextVal);
				oValue = OpFactory(oOp, oValue, oNextVal);
			}

			return oValue;
		}

		/// <summary>
		/// AddSub(): Detects the operation to perform addition or substraction.
		/// </summary>
		/// <returns>CValue object representing an operation.</returns>
		private CValue AddSub()
		{
			CValue oValue = MultDiv();

			while (m_currentToken == "+" || m_currentToken == "-")
			{
				Parser.Token oOp = m_currentToken;
				PositionNextToken();

				CValue oNextVal = MultDiv();

				CheckParms(oOp, oValue, oNextVal);
				oValue = OpFactory(oOp, oValue, oNextVal);
			}

			return oValue;
		}

		/// <summary>
		/// Relational():  Detects the operation to perform a relational operator (>, <, <=, etc.).
		/// </summary>
		/// <returns>CValue object representing an operation.</returns>
		private CValue Relational()
		{
			CValue oValue = AddSub();

			while (m_currentToken == "&&" ||
				   m_currentToken == "||" ||
				   m_currentToken == "==" ||
				   m_currentToken == "<"  ||
				   m_currentToken == ">"  ||
				   m_currentToken == "<=" ||
				   m_currentToken == ">=" ||
				   m_currentToken == "!=" ||
				   m_currentToken == "<>")
			{
				Parser.Token oOp = m_currentToken;
				PositionNextToken();
				CValue oNextVal = Relational();

				CheckParms(oOp, oValue, oNextVal);
				oValue = OpFactory(oOp, oValue, oNextVal);
			}

			return oValue;
		}

		/// <summary>
		/// OpFactor(...): Reads the passed operator, identifies the associated implementation object
		///		and requests an operation object to be used in evaluating the equation.
		/// </summary>
		/// <param name="oSourceOp">Parser.Token object representing the operator in question.</param>
		/// <param name="oValue1">The first value object to be operated on.</param>
		/// <param name="oValue2">The second value object to be operated on.</param>
		/// <returns>CValue object representing an operation.</returns>
		private CValue OpFactory(Parser.Token oSourceOp, CValue oValue1, CValue oValue2)
		{
			if (_operators.ContainsKey(oSourceOp.ToString()))
			{
				COperator oOp = _operators[oSourceOp.ToString()];
				oOp.SetParameters(oValue1, oValue2);
				return oOp;
			}

			throw new ApplicationException(string.Format("Invalid operator {0} in equation.", oSourceOp.ToString()));
		}

		#endregion

		#region Core Base Classes

		/// <summary>
		/// CValue class:  This base is the basic building block of
		///		all operations, variables, functions, etc..  Any object
		///		may call to a CValue object asking for it to resolve itself
		///		and return it's processed value.
		/// </summary>
		public abstract class CValue
		{
			public abstract double GetValue();
		}

		/// <summary>
		/// CVariable class : Derived from CValue, CVariable implements
		///		a named expression variable, holding the name and associated value.
		///		This object is accessed when an expression is evaluated or when
		///		a user sets a variable value.
		/// </summary>
		public class CVariable : CValue
		{
			private string m_sVarName;
			private double m_dValue;

			/// <summary>
			/// CVariable(string) constructor: Creates the object and holds onto the 
			///		compile-time assigned variable name.
			/// </summary>
			/// <param name="sVarName">Name of the variable within the expression.</param>
			public CVariable(string sVarName)
			{
				m_sVarName = sVarName;
			}

			/// <summary>
			/// CVariable(string,double) constructor: Creates the objects and holds onto the
			///		compile-time assigned variable name and value.
			/// </summary>
			/// <param name="sVarName">string containing the variable name</param>
			/// <param name="dValue">double containing the assigned variable value</param>
			public CVariable(string sVarName, double dValue)
			{
				m_sVarName = sVarName;
			}

			/// <summary>
			/// SetValue(): Allows the setting of the variables value at runtime.
			/// </summary>
			/// <param name="dValue"></param>
			public void SetValue(double dValue)
			{
				m_dValue = dValue;
			}

			/// <summary>
			/// GetValue(): Returns the value of the variable to the calling client.
			/// </summary>
			/// <returns>double</returns>
			public override double GetValue()
			{
				return m_dValue;
			}
		}

		/// <summary>
		/// CNumber Class:  A CValue-derived class that implements a static
		///    numeric value in an expression.
		/// </summary>
		public class CNumber : CValue
		{
			private double m_dValue;

			/// <summary>
			/// CNumber(string) constructor:  take a string representation of a static number
			///		and stores it for future retrieval.
			/// </summary>
			/// <param name="sValue">string/text representation of a number</param>
			public CNumber(string sValue)
			{
				m_dValue = Convert.ToDouble(sValue);
			}

			/// <summary>
			/// CNumber(double) constructor: takes a double represenation of a static number
			/// and stores it for future retrieval.
			/// </summary>
			/// <param name="dValue"></param>
			public CNumber(double dValue)
			{
				m_dValue = dValue;
			}

			/// <summary>
			/// GetValue(): returns the static value when called upon.
			/// </summary>
			/// <returns>double</returns>
			public override double GetValue()
			{
				return m_dValue;
			}
		}

		/// <summary>
		/// CSignNeg provides negative number functionality
		/// within an equation. 
		/// </summary>
		private class CSignNeg : CValue
		{
			CValue m_oValue;

			/// <summary>
			/// CSignNeg constructor:  Grabs onto the assigned
			///		CValue object and retains it for processing
			///		requested operations.
			/// </summary>
			/// <param name="oValue">Child operation this object operates upon.</param>
			public CSignNeg(CValue oValue)
			{
				m_oValue = oValue;
			}

			/// <summary>
			/// GetValue():  Performs the negative operation on the child operation and returns the value.
			/// </summary>
			/// <returns>A double value evaluated and returned with the opposite sign.</returns>
			public override double GetValue()
			{
				return m_oValue.GetValue() * -1;
			}
		}

		/// <summary>
		/// COperator class: A CValue derived class responsible for identifying
		///		and implementing operations during the parsing and evaluation processes.
		/// </summary>
		private class COperator : CValue
		{
			private Func<double, double, double> _function;
			private CValue _param1;
			private CValue _param2;

			public COperator(Func<double, double, double> function)
			{
				_function = function;
			}

			public void SetParameters(CValue param1, CValue param2)
			{
				_param1 = param1;
				_param2 = param2;
			}

			public override double GetValue()
			{
				return _function(_param1.GetValue(), _param2.GetValue());
			}
		}

		/// <summary>
		/// CFunction class: A CValue derived class that provdes the base for all functions
		///		implemented in the compiler.  This class also allows for external clients to
		///		create and register functions with the compiler - thereby extending the compilers
		///		syntax and functionality.
		/// </summary>
		public class CFunction : CValue
		{
			private Func<double, double> _function;
			private ArrayList _parameters;
			
			public CFunction(Func<double, double> function)
			{
				_function = function;
			}

			public void SetParameters(ArrayList values)
			{
				_parameters = values;
			}

			public override double GetValue()
			{
				return _function(((CValue) _parameters[0]).GetValue());
			}
		}

		#endregion

		#region Helper Functions

		/// <summary>
		/// CheckParms( Parser.Token, CValue, CValue) - This method makes certain the arguments are non-null
		/// </summary>
		/// <param name="oToken">Currently processed Parser.Token object</param>
		/// <param name="arg1">CValue argument 1</param>
		/// <param name="arg2">CValue argument 2</param>
		private void CheckParms(Parser.Token oToken, CValue arg1, CValue arg2)
		{
			if (arg1 == null || arg2 == null)
				throw new ApplicationException("Argument not supplied near " + oToken.ToString() + " operation.");
		}

		/// <summary>
		/// CheckParms( Parser.Token, CValue) - This method makes certain the single argument is non-null.
		///		Raises an exception if it is.
		/// </summary>
		/// <param name="oToken">Parser.Token object</param>
		/// <param name="arg1">CValue argument</param>
		private void CheckParms(Parser.Token oToken, CValue arg1)
		{
			if (arg1 == null)
				throw new ApplicationException("Argument not supplied near " + oToken.ToString() + " operation.");
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
		/// PositionNextToken():  Manipulates the current Token position forward in the chain of tokens
		///		discovered by the parser.
		/// </summary>
		private void PositionNextToken()
		{
			if (m_currentToken == null)
			{
				if (!m_enumTokens.MoveNext())
					throw new ApplicationException("Invalid equation.");

				m_nextToken = (Parser.Token) m_enumTokens.Current;
			}

			m_currentToken = m_nextToken;

			if (!m_enumTokens.MoveNext())
				m_nextToken = new Parser.Token();
			else
				m_nextToken = (Parser.Token) m_enumTokens.Current;
		}


		/// <summary>
		/// GetVariableByName(string) : This method returns the variable associated with the
		///		provided name string.
		/// </summary>
		/// <param name="sVarName">string variable name</param>
		/// <returns>CVariable object mapped to the passed variable name</returns>
		private CVariable GetVariableByName(string sVarName)
		{
			if (m_slVariables == null)
				m_slVariables = new SortedList();

			int iIdx = m_slVariables.IndexOfKey(sVarName);

			if (iIdx > -1)
				return (CVariable) m_slVariables.GetByIndex(iIdx);

			CVariable oVar = new CVariable(sVarName);
			m_slVariables.Add(sVarName, oVar);

			return oVar;
		}

		#endregion
	}
}
