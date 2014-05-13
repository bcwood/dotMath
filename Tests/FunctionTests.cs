using System;
using NUnit.Framework;

namespace dotMath.Tests
{
	[TestFixture]
	public class FunctionTests
	{
		[TestCase(-5)]
		public void Abs(double value)
		{
			var compiler = new EquationCompiler("abs(a)");
			compiler.SetVariable("a", value);

			Assert.AreEqual(Math.Abs(value), compiler.Calculate());
		}

		[TestCase(4.2)]
		public void Acos(double value)
		{
			var compiler = new EquationCompiler("acos(a)");
			compiler.SetVariable("a", value);

			Assert.AreEqual(Math.Acos(value), compiler.Calculate());
		}

		[TestCase(4.2)]
		public void Asin(double value)
		{
			var compiler = new EquationCompiler("asin(a)");
			compiler.SetVariable("a", value);

			Assert.AreEqual(Math.Asin(value), compiler.Calculate());
		}

		[TestCase(4.2)]
		public void Atan(double value)
		{
			var compiler = new EquationCompiler("atan(a)");
			compiler.SetVariable("a", value);

			Assert.AreEqual(Math.Atan(value), compiler.Calculate());
		}

		[TestCase(-4.2)]
		public void Ceiling(double value)
		{
			var compiler = new EquationCompiler("ceiling(a)");
			compiler.SetVariable("a", value);

			Assert.AreEqual(Math.Ceiling(value), compiler.Calculate());
		}

		[TestCase(4.2)]
		public void Cos(double value)
		{
			var compiler = new EquationCompiler("cos(a)");
			compiler.SetVariable("a", value);

			Assert.AreEqual(Math.Cos(value), compiler.Calculate());
		}

		[TestCase(4.2)]
		public void Cosh(double value)
		{
			var compiler = new EquationCompiler("cosh(a)");
			compiler.SetVariable("a", value);

			Assert.AreEqual(Math.Cosh(value), compiler.Calculate());
		}

		[TestCase(4.2)]
		public void Exp(double value)
		{
			var compiler = new EquationCompiler("exp(a)");
			compiler.SetVariable("a", value);

			Assert.AreEqual(Math.Exp(value), compiler.Calculate());
		}

		[TestCase(-4.2)]
		public void Floor(double value)
		{
			var compiler = new EquationCompiler("floor(a)");
			compiler.SetVariable("a", value);

			Assert.AreEqual(Math.Floor(value), compiler.Calculate());
		}

		[TestCase(4.2)]
		public void Log(double value)
		{
			var compiler = new EquationCompiler("log(a)");
			compiler.SetVariable("a", value);

			Assert.AreEqual(Math.Log(value), compiler.Calculate());
		}

		[TestCase(4.2)]
		public void Log10(double value)
		{
			var compiler = new EquationCompiler("log10(a)");
			compiler.SetVariable("a", value);

			Assert.AreEqual(Math.Log10(value), compiler.Calculate());
		}

		[TestCase(4.2)]
		public void Round(double value)
		{
			var compiler = new EquationCompiler("round(a)");
			compiler.SetVariable("a", value);

			Assert.AreEqual(Math.Round(value), compiler.Calculate());
		}

		[TestCase(-4.2)]
		public void Sign(double value)
		{
			var compiler = new EquationCompiler("sign(a)");
			compiler.SetVariable("a", value);

			Assert.AreEqual(Math.Sign(value), compiler.Calculate());
		}

		[TestCase(4.2)]
		public void Sin(double value)
		{
			var compiler = new EquationCompiler("sin(a)");
			compiler.SetVariable("a", value);

			Assert.AreEqual(Math.Sin(value), compiler.Calculate());
		}

		[TestCase(4.2)]
		public void Sinh(double value)
		{
			var compiler = new EquationCompiler("sinh(a)");
			compiler.SetVariable("a", value);

			Assert.AreEqual(Math.Sinh(value), compiler.Calculate());
		}

		[TestCase(4.2)]
		public void Sqrt(double value)
		{
			var compiler = new EquationCompiler("sqrt(a)");
			compiler.SetVariable("a", value);

			Assert.AreEqual(Math.Sqrt(value), compiler.Calculate());
		}

		[TestCase(4.2)]
		public void Tan(double value)
		{
			var compiler = new EquationCompiler("tan(a)");
			compiler.SetVariable("a", value);

			Assert.AreEqual(Math.Tan(value), compiler.Calculate());
		}

		[TestCase(4.2)]
		public void Tanh(double value)
		{
			var compiler = new EquationCompiler("tanh(a)");
			compiler.SetVariable("a", value);

			Assert.AreEqual(Math.Tanh(value), compiler.Calculate());
		}

		[TestCase(4.2)]
		public void NestedFunctions(double value)
		{
			var compiler = new EquationCompiler("sin(cos(tan(a)))");
			compiler.SetVariable("a", value);

			Assert.AreEqual(Math.Sin(Math.Cos(Math.Tan(value))), compiler.Calculate());
		}

		[Test]
		public void RangeTest()
		{
			for (double x = -100; x <= 100; x += 0.5)
			{
				Abs(x);
				Acos(x);
				Asin(x);
				Atan(x);
				Ceiling(x);
				Cos(x);
				Cosh(x);
				Exp(x);
				Floor(x);
				Log(x);
				Log10(x);
				Round(x);
				Sign(x);
				Sin(x);
				Sinh(x);
				Tan(x);
				Tanh(x);
				NestedFunctions(x);
			}
		}

		[TestCase(4, 2)]
		[TestCase(2, 4)]
		[TestCase(4, -2)]
		public void Min(double a, double b)
		{
			var compiler = new EquationCompiler("min(a,b)");
			compiler.SetVariable("a", a);
			compiler.SetVariable("b", b);

			Assert.AreEqual((a < b ? a : b), compiler.Calculate());
		}

		[TestCase(4, 2)]
		[TestCase(2, 4)]
		[TestCase(4, -2)]
		public void Max(double a, double b)
		{
			var compiler = new EquationCompiler("max(a,b)");
			compiler.SetVariable("a", a);
			compiler.SetVariable("b", b);

			Assert.AreEqual((a > b ? a : b), compiler.Calculate());
		}

		[TestCase("1>2", false, 3, 4)]
		[TestCase("2>1", true, 5, 6)]
		public void If(string condition, bool result, double a, double b)
		{
			var compiler = new EquationCompiler(string.Format("if({0},a,b)", condition));
			compiler.SetVariable("a", a);
			compiler.SetVariable("b", b);

			Assert.AreEqual((result ? a : b), compiler.Calculate());
		}

		[Test]
		public void MultipleFunctionsPerObject()
		{
			var compiler = new EquationCompiler("abs(-4)");
			Assert.AreEqual(Math.Abs(-4), compiler.Calculate());

			compiler.SetFunction("acos(10)");
			Assert.AreEqual(Math.Acos(10), compiler.Calculate());
		}

		[Test]
		public void CustomFunction()
		{
			var compiler = new EquationCompiler("factorial(5)");
			compiler.AddFunction("factorial", x =>
				                               {
					                               int factorial = 1;
												   for (int i = 1; i <= x; i++)
												   {
													   factorial *= i;
												   }

					                               return factorial;
				                               });
			
			Assert.AreEqual(5 * 4 * 3 * 2 * 1, compiler.Calculate());
		}
	}
}
