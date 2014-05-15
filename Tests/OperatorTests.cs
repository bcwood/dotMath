using System;
using NUnit.Framework;
using dotMath.Exceptions;

namespace dotMath.Tests
{
	[TestFixture]
	public class OperatorTests
	{
		[Test]
		public void Constant()
		{
			var compiler = new EquationCompiler("4.2");

			Assert.AreEqual(4.2, compiler.Calculate());
		}

		[TestCase(-4.2)]
		[TestCase(4.2)]
		public void Variable(double a)
		{
			var compiler = new EquationCompiler("a");
			compiler.SetVariable("a", a);

			Assert.AreEqual(a, compiler.Calculate());
		}

		[TestCase(4, 2)]
		[TestCase(4, -2)]
		public void Addition(double a, double b)
		{
			var compiler = new EquationCompiler("a+b");
			compiler.SetVariable("a", a);
			compiler.SetVariable("b", b);

			Assert.AreEqual(a + b, compiler.Calculate());
		}

		[TestCase(4, 2)]
		[TestCase(2, 4)]
		[TestCase(2, -4)]
		public void Subtraction(double a, double b)
		{
			var compiler = new EquationCompiler("a-b");
			compiler.SetVariable("a", a);
			compiler.SetVariable("b", b);

			Assert.AreEqual(a - b, compiler.Calculate());
		}

		[Test]
		public void Sign()
		{
			var compiler = new EquationCompiler("4 + -3");
			Assert.AreEqual(4 + -3, compiler.Calculate());

			compiler.SetFunction("4+-3");
			Assert.AreEqual(4 + -3, compiler.Calculate());

			compiler.SetFunction("4 - -3");
			Assert.AreEqual(4 - -3, compiler.Calculate());

			compiler.SetFunction("4--3");
			Assert.AreEqual(4 - -3, compiler.Calculate());
		}

		[TestCase(4, 2)]
		[TestCase(4, 0.5)]
		public void Multiplication(double a, double b)
		{
			var compiler = new EquationCompiler("a*b");
			compiler.SetVariable("a", a);
			compiler.SetVariable("b", b);

			Assert.AreEqual(a*b, compiler.Calculate());
		}

		[TestCase(4, 2)]
		[TestCase(2, 4)]
		[TestCase(2, 0.5)]
		[TestCase(4, 0, TestName = "Divide by zero")]
		public void Division(double a, double b)
		{
			var compiler = new EquationCompiler("a/b");
			compiler.SetVariable("a", a);
			compiler.SetVariable("b", b);

			Assert.AreEqual(a/b, compiler.Calculate());
		}

		[TestCase(8, 2)]
		[TestCase(8, -2)]
		[TestCase(8, 0.5)]
		public void Power(double a, double b)
		{
			var compiler = new EquationCompiler("a^b");
			compiler.SetVariable("a", a);
			compiler.SetVariable("b", b);

			Assert.AreEqual(Math.Pow(a, b), compiler.Calculate());
		}

		[TestCase(8, 2)]
		[TestCase(8, 3)]
		[TestCase(3, 8)]
		public void Modulo(double a, double b)
		{
			var compiler = new EquationCompiler("a%b");
			compiler.SetVariable("a", a);
			compiler.SetVariable("b", b);

			Assert.AreEqual(a % b, compiler.Calculate());
		}

		[TestCase(4, 2)]
		[TestCase(4, 4)]
		[TestCase(2, 4)]
		public void LessThan(double a, double b)
		{
			var compiler = new EquationCompiler("a<b");
			compiler.SetVariable("a", a);
			compiler.SetVariable("b", b);

			Assert.AreEqual(a < b, Convert.ToBoolean(compiler.Calculate()));
		}

		[TestCase(4, 2)]
		[TestCase(4, 4)]
		[TestCase(2, 4)]
		public void LessThanOrEqual(double a, double b)
		{
			var compiler = new EquationCompiler("a<=b");
			compiler.SetVariable("a", a);
			compiler.SetVariable("b", b);

			Assert.AreEqual(a <= b, Convert.ToBoolean(compiler.Calculate()));
		}

		[TestCase(4, 2)]
		[TestCase(4, 4)]
		[TestCase(2, 4)]
		public void GreaterThan(double a, double b)
		{
			var compiler = new EquationCompiler("a>b");
			compiler.SetVariable("a", a);
			compiler.SetVariable("b", b);

			Assert.AreEqual(a > b, Convert.ToBoolean(compiler.Calculate()));
		}

		[TestCase(4, 2)]
		[TestCase(4, 4)]
		[TestCase(2, 4)]
		public void GreaterThanOrEqual(double a, double b)
		{
			var compiler = new EquationCompiler("a>=b");
			compiler.SetVariable("a", a);
			compiler.SetVariable("b", b);

			Assert.AreEqual(a >= b, Convert.ToBoolean(compiler.Calculate()));
		}

		[TestCase(4, 2)]
		[TestCase(4, 4)]
		[TestCase(2, 4)]
		public void Equal(double a, double b)
		{
			var compiler = new EquationCompiler("a==b");
			compiler.SetVariable("a", a);
			compiler.SetVariable("b", b);

			Assert.AreEqual(a == b, Convert.ToBoolean(compiler.Calculate()));
		}

		[TestCase(4, 2)]
		[TestCase(4, 4)]
		[TestCase(2, 4)]
		public void NotEqual(double a, double b)
		{
			var compiler = new EquationCompiler("a!=b");
			compiler.SetVariable("a", a);
			compiler.SetVariable("b", b);

			Assert.AreEqual(a != b, Convert.ToBoolean(compiler.Calculate()));
		}

		[TestCase("3 + 4 / 2", ExpectedResult = 3 + 4 / 2.0)]
		[TestCase("(3 + 4) / 2", ExpectedResult = (3 + 4) / 2.0)]
		public double OrderOfOperations(string equation)
		{
			var compiler = new EquationCompiler(equation);
			return compiler.Calculate();
		}

		[TestCase("4!2")]
		[TestCase("4~2")]
		[TestCase("4$2")]
		[TestCase("4\"2")]
		[TestCase("4'2")]
		public void InvalidToken_ThrowsInvalidEquationException(string equation)
		{
			var compiler = new EquationCompiler(equation);
			Assert.Throws<InvalidEquationException>(() => compiler.Calculate());
		}

		[Test]
		public void NullArgument_ThrowsInvalidEquationException()
		{
			var compiler = new EquationCompiler("4+");
			Assert.Throws<InvalidEquationException>(() => compiler.Calculate());
		}
	}
}
