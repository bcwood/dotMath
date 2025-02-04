using System;
using NUnit.Framework;
using dotMath.Exceptions;

namespace dotMath.Tests
{
	[TestFixture]
	public class FunctionTests
	{
		[TestCase(-5)]
		public void Abs(double a)
		{
			var compiler = new EquationCompiler("abs(a)");
			compiler.SetVariable("a", a);

			Assert.AreEqual(Math.Abs(a), compiler.Calculate());
		}

		[TestCase(4.2)]
		public void Acos(double a)
		{
			var compiler = new EquationCompiler("acos(a)");
			compiler.SetVariable("a", a);

			Assert.AreEqual(Math.Acos(a), compiler.Calculate());
		}

		[TestCase(4.2)]
		public void Asin(double a)
		{
			var compiler = new EquationCompiler("asin(a)");
			compiler.SetVariable("a", a);

			Assert.AreEqual(Math.Asin(a), compiler.Calculate());
		}

		[TestCase(4.2)]
		public void Atan(double a)
		{
			var compiler = new EquationCompiler("atan(a)");
			compiler.SetVariable("a", a);

			Assert.AreEqual(Math.Atan(a), compiler.Calculate());
		}

		[TestCase(-4.2)]
		public void Ceiling(double a)
		{
			var compiler = new EquationCompiler("ceiling(a)");
			compiler.SetVariable("a", a);

			Assert.AreEqual(Math.Ceiling(a), compiler.Calculate());
		}

		[TestCase(4.2)]
		public void Cos(double a)
		{
			var compiler = new EquationCompiler("cos(a)");
			compiler.SetVariable("a", a);

			Assert.AreEqual(Math.Cos(a), compiler.Calculate());
		}

		[TestCase(4.2)]
		public void Cosh(double a)
		{
			var compiler = new EquationCompiler("cosh(a)");
			compiler.SetVariable("a", a);

			Assert.AreEqual(Math.Cosh(a), compiler.Calculate());
		}

		[TestCase(4.2)]
		public void Exp(double a)
		{
			var compiler = new EquationCompiler("exp(a)");
			compiler.SetVariable("a", a);

			Assert.AreEqual(Math.Exp(a), compiler.Calculate());
		}

		[TestCase(-4.2)]
		public void Floor(double a)
		{
			var compiler = new EquationCompiler("floor(a)");
			compiler.SetVariable("a", a);

			Assert.AreEqual(Math.Floor(a), compiler.Calculate());
		}

		[TestCase(4.2)]
		public void Log(double a)
		{
			var compiler = new EquationCompiler("log(a)");
			compiler.SetVariable("a", a);

			Assert.AreEqual(Math.Log(a), compiler.Calculate());
		}

		[TestCase(4.2)]
		public void Log10(double a)
		{
			var compiler = new EquationCompiler("log10(a)");
			compiler.SetVariable("a", a);

			Assert.AreEqual(Math.Log10(a), compiler.Calculate());
		}

		[TestCase(27, 3)]
		[TestCase(4.2, 2.5)]
		public void Root(double a, double b)
		{
			var compiler = new EquationCompiler("root(a;b)");
			compiler.SetVariable("a", a);
			compiler.SetVariable("b", b);

			Assert.AreEqual(Math.Pow(a, 1 / b), compiler.Calculate());
		}

		[TestCase(4.2)]
		public void Round(double a)
		{
			var compiler = new EquationCompiler("round(a)");
			compiler.SetVariable("a", a);

			Assert.AreEqual(Math.Round(a), compiler.Calculate());
		}

		[TestCase(-4.2)]
		public void Sign(double a)
		{
			var compiler = new EquationCompiler("sign(a)");
			compiler.SetVariable("a", a);

			Assert.AreEqual(Math.Sign(a), compiler.Calculate());
		}

		[TestCase(4.2)]
		public void Sin(double a)
		{
			var compiler = new EquationCompiler("sin(a)");
			compiler.SetVariable("a", a);

			Assert.AreEqual(Math.Sin(a), compiler.Calculate());
		}

		[TestCase(4.2)]
		public void Sinh(double a)
		{
			var compiler = new EquationCompiler("sinh(a)");
			compiler.SetVariable("a", a);

			Assert.AreEqual(Math.Sinh(a), compiler.Calculate());
		}

		[TestCase(4.2)]
		public void Sqrt(double a)
		{
			var compiler = new EquationCompiler("sqrt(a)");
			compiler.SetVariable("a", a);

			Assert.AreEqual(Math.Sqrt(a), compiler.Calculate());
		}

		[TestCase(4.2)]
		public void Tan(double a)
		{
			var compiler = new EquationCompiler("tan(a)");
			compiler.SetVariable("a", a);

			Assert.AreEqual(Math.Tan(a), compiler.Calculate());
		}

		[TestCase(4.2)]
		public void Tanh(double a)
		{
			var compiler = new EquationCompiler("tanh(a)");
			compiler.SetVariable("a", a);

			Assert.AreEqual(Math.Tanh(a), compiler.Calculate());
		}

		[TestCase(4.2)]
		public void NestedFunctions(double a)
		{
			var compiler = new EquationCompiler("sin(cos(tan(a)))");
			compiler.SetVariable("a", a);

			Assert.AreEqual(Math.Sin(Math.Cos(Math.Tan(a))), compiler.Calculate());
		}

        [Test]
        public void NestedFunctionsWithMultipleParameters()
        {
            var compiler = new EquationCompiler("max(1;min(2;3))");
			Assert.AreEqual(2, compiler.Calculate());
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

		[TestCase(1.5, 2.25, 3.3)]
		[TestCase(-1.5, -2.25, -3.3)]
		public void MultipleFunctions(double a, double b, double c)
		{
			var compiler = new EquationCompiler("sin(a) + cos(b) - tan(c)");
			compiler.SetVariable("a", a);
			compiler.SetVariable("b", b);
			compiler.SetVariable("c", c);

			Assert.AreEqual(Math.Sin(a) + Math.Cos(b) - Math.Tan(c), compiler.Calculate());
		}

        [TestCase(1.5, 2.25, 3.3)]
        [TestCase(-1.5, -2.25, -3.3)]
        public void MultipleOfSameFunction(double a, double b, double c)
        {
            var compiler = new EquationCompiler("sin(a) + sin(b) - sin(c)");
            compiler.SetVariable("a", a);
            compiler.SetVariable("b", b);
            compiler.SetVariable("c", c);

            Assert.AreEqual(Math.Sin(a) + Math.Sin(b) - Math.Sin(c), compiler.Calculate());
        }

        [TestCase(4, 2)]
		[TestCase(2, 4)]
		[TestCase(4, -2)]
		public void Min(double a, double b)
		{
			var compiler = new EquationCompiler("min(a;b)");
			compiler.SetVariable("a", a);
			compiler.SetVariable("b", b);

			Assert.AreEqual((a < b ? a : b), compiler.Calculate());
		}

		[TestCase(4, 2)]
		[TestCase(2, 4)]
		[TestCase(4, -2)]
		public void Max(double a, double b)
		{
			var compiler = new EquationCompiler("max(a;b)");
			compiler.SetVariable("a", a);
			compiler.SetVariable("b", b);

			Assert.AreEqual((a > b ? a : b), compiler.Calculate());
		}

		[TestCase("1>2", false, 3, 4)]
		[TestCase("2>1", true, 5, 6)]
		public void If(string condition, bool result, double a, double b)
		{
			var compiler = new EquationCompiler(string.Format("if({0};a;b)", condition));
			compiler.SetVariable("a", a);
			compiler.SetVariable("b", b);

			Assert.AreEqual((result ? a : b), compiler.Calculate());
		}

        [TestCase("1>2", false, 3, 4, 1)]
        [TestCase("2>1", true, 5, 6, 2)]
        public void IfWithEquation(string condition, bool result, double a, double b, double c)
        {
            var compiler = new EquationCompiler(string.Format("if({0};(a+b);c)", condition));
            compiler.SetVariable("a", a);
            compiler.SetVariable("b", b);
            compiler.SetVariable("c", c);
            Assert.AreEqual((result ? a + b : c), compiler.Calculate());

            compiler.SetFunction(string.Format("if({0};a;(b+c))", condition));
            Assert.AreEqual((result ? a : b + c), compiler.Calculate());
        }

        [Test]
        public void NestedIf()
        {
			var compiler = new EquationCompiler("if(1; if(1; 2; 3); 4)");
			Assert.AreEqual(2, compiler.Calculate());
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

		[TestCase("abs(-5")]
		[TestCase("abs(-5))")]
		public void UnmatchedParen_ThrowsUnmatchedParenthesesException(string equation)
		{
			var compiler = new EquationCompiler(equation);
			Assert.Throws<UnmatchedParenthesesException>(() => compiler.Calculate());
		}

		[Test]
		public void InvalidFunction_ThrowsInvalidFunctionException()
		{
			var compiler = new EquationCompiler("foo(5)");
			Assert.Throws<InvalidFunctionException>(() => compiler.Calculate());
		}

		[TestCase("abs()")]
		[TestCase("abs(1;2)")]
		[TestCase("min(1)")]
		[TestCase("min(1;)")]
		[TestCase("min(;2)")]
		[TestCase("min(1;2;3)")]
		[TestCase("if(1>2;;3)")]
		[TestCase("if(1>2;3;)")]
        [TestCase("if(1>2;3;4;5)")]
        public void InvalidArgumentCount_ThrowsArgumentCountException(string equation)
		{
			var compiler = new EquationCompiler(equation);
			Assert.Throws<ArgumentCountException>(() => compiler.Calculate());
		}

        [TestCase(1, 2, 3, 6)]
		[TestCase(5, 10, 15, 30)]
        public void SupportFunc4Double_Succeeds(double a, double b, double c, double d)
        {
			var compiler = new EquationCompiler($"sum({a};{b};{c})");
			compiler.AddFunction("sum", (double x, double y, double z) => x + y + z);
            Assert.AreEqual(d, compiler.Calculate());
        }

		[TestCase(1, 1.0, 2.0, 1.0)]
		[TestCase(0, 1.0, 2.0, 2.0)]
		public void SupportFuncBool3Double_Succeeds(double a, double b, double c, double d) 
		{
			var compiler = new EquationCompiler($"test({a};{b};{c})");
			compiler.AddFunction("test", (bool x, double y, double z) => 
			{
				if (x) return y;
				else return z;
			});
		 	Assert.AreEqual(d, compiler.Calculate());
		}
	}
}
