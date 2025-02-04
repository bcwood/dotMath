﻿using System;
using System.Collections.Generic;
using System.Globalization;
using dotMath.Exceptions;

namespace dotMath.Core
{
	/// <summary>
	/// This is the basic building block of all operations, variables, functions, etc.  
	/// Any object may call to a CValue object asking for it to resolve itself and return it's processed value.
	/// </summary>
	internal abstract class CValue
	{
		public abstract double GetValue();
	}

	/// <summary>
	/// Implements a named expression variable, holding the name and associated value.
	///	This object is accessed when an expression is evaluated or when a user sets a variable value.
	/// </summary>
	internal class CVariable : CValue
	{
		private double _value;

		public void SetValue(double value)
		{
			_value = value;
		}

		public override double GetValue()
		{
			return _value;
		}
	}

	/// <summary>
	/// Implements a static numeric value in an expression.
	/// </summary>
	internal class CNumber : CValue
	{
		private double _value;

		public CNumber(string value, CultureInfo cultureInfo)
		{
			_value = Convert.ToDouble(value, cultureInfo);
		}

		public override double GetValue()
		{
			return _value;
		}
	}

	/// <summary>
	/// Provides negative number functionality within an equation. 
	/// </summary>
	internal class CSignNeg : CValue
	{
		CValue _value;

		public CSignNeg(CValue value)
		{
			_value = value;
		}

		public override double GetValue()
		{
			return _value.GetValue() * -1;
		}
	}

	/// <summary>
	/// Responsible for identifying and implementing operations during the parsing and evaluation processes.
	/// </summary>
	internal class COperator : CValue
	{
		private Func<double, double, double> _function;
		private CValue _param1;
		private CValue _param2;

		public COperator(Func<double, double, double> function)
		{
			_function = function;
		}

		public COperator SetParameters(CValue param1, CValue param2)
		{
            var clone = new COperator(_function);

            clone._param1 = param1;
            clone._param2 = param2;

            return clone;
		}

		public override double GetValue()
		{
			return _function(_param1.GetValue(), _param2.GetValue());
		}
	}

	/// <summary>
	/// Provides the base for all functions implemented in the compiler. Also allows for external clients 
	/// to create and register functions with the compiler, thereby extending the compiler's syntax and functionality.
	/// </summary>
	internal class CFunction : CValue
	{
		private object _function;
		private List<CValue> _parameters;
		private int _expectedArgCount = -1;

		public CFunction(Func<double, double> function)
		{
			_function = function;
			_expectedArgCount = 1;
		}

		public CFunction(Func<double, double, double> function)
		{
			_function = function;
			_expectedArgCount = 2;
		}

		public CFunction(Func<bool, double, double, double> function)
		{
			_function = function;
			_expectedArgCount = 3;
		}

		public CFunction(Func<double, double, double, double> function)
		{
			_function = function;
			_expectedArgCount = 3;
		}

        private CFunction(object function, int expectedArgCount)
        {
            _function = function;
            _expectedArgCount = expectedArgCount;
        }

		public CFunction SetParameters(List<CValue> values)
		{
            // validate argument count
			if (_expectedArgCount != values.Count)
				throw new ArgumentCountException(values.Count);

			// validate arguments aren't null
			for (int i = 0; i < values.Count; i++)
			{
				if (values[i] == null)
					throw new ArgumentCountException(values.Count);
			}

            var clone = new CFunction(_function, _expectedArgCount);

            clone._parameters = values;

            return clone;
		}

		public override double GetValue()
		{
			switch (_parameters.Count)
			{
				case 1:
					return (_function as Func<double, double>)(((CValue) _parameters[0]).GetValue());
				case 2:
					return (_function as Func<double, double, double>)(((CValue) _parameters[0]).GetValue(), ((CValue) _parameters[1]).GetValue());
				case 3:
					var boolFunction = _function as Func<bool, double, double, double>;
					if (boolFunction != null) 
					{
						return boolFunction(Convert.ToBoolean(((CValue) _parameters[0]).GetValue()), ((CValue) _parameters[1]).GetValue(), ((CValue) _parameters[2]).GetValue());
					}
					var doubleFunction = _function as Func<double, double, double, double>;
					if (doubleFunction != null) 
					{
						return doubleFunction(((CValue) _parameters[0]).GetValue(), ((CValue) _parameters[1]).GetValue(), ((CValue) _parameters[2]).GetValue());
					}
					throw new InvalidOperationException("The Custom Function Type is not supported");
				default:
					throw new ArgumentCountException(_parameters.Count);
			}
		}
	}
}
