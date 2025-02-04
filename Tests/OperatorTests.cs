using System;
using System.Globalization;
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

		[Test]
		public void ConstantWithGroupSeparator()
		{
			var compiler = new EquationCompiler("4,200");

			Assert.AreEqual(4200.0, compiler.Calculate());
		}
        
        [Test]
        public void ConstantWithCulture_DE()
        {
            var compiler = new EquationCompiler("4,2", CultureInfo.GetCultureInfo("de-DE"));
            Assert.AreEqual(4.2, compiler.Calculate());
        }

        [Test]
        public void ConstantWithCulture_EN()
        {
            var compiler = new EquationCompiler("4.2", CultureInfo.GetCultureInfo("en-US"));
            Assert.AreEqual(4.2, compiler.Calculate());
        }

        [Test]
        public void ConstantWithCulture_FR()
        {
            var compiler = new EquationCompiler("4,2", CultureInfo.GetCultureInfo("fr-FR"));
            Assert.AreEqual(4.2, compiler.Calculate());
        }

        [Test]
        public void ConstantWithCulture_CH()
        {
            var compiler = new EquationCompiler("4\u202f200,50", CultureInfo.GetCultureInfo("fr-CH"));
            Assert.AreEqual(4200.50, compiler.Calculate());
        }

        [Test]
        public void ConstantWithCulture_CH_ApostropheEdgeCase()
        {
            var compiler = new EquationCompiler("4'200,50", CultureInfo.GetCultureInfo("fr-CH"));
            Assert.AreEqual(4200.50, compiler.Calculate());
        }

        [Test]
        public void ConstantWithCulture_RU()
        {
            var compiler = new EquationCompiler("4\u00a0200,50", CultureInfo.GetCultureInfo("ru-RU"));
            Assert.AreEqual(4200.50, compiler.Calculate());
        }

        [Test]
        public void ConstantWithCulture_ES()
        {
            var compiler = new EquationCompiler("4.200,50", CultureInfo.GetCultureInfo("es-ES"));
            Assert.AreEqual(4200.50, compiler.Calculate());
        }

        [Test]
        public void ConstantWithCulture_SE()
        {
            var compiler = new EquationCompiler("4\u00a0200,50", CultureInfo.GetCultureInfo("sv-SE"));
            Assert.AreEqual(4200.50, compiler.Calculate());
        }

        [Test]
        public void LeadingAndTrailingSpaces_EN()
        {
            var compiler = new EquationCompiler(" 4.2 ", CultureInfo.GetCultureInfo("en-US"));
            Assert.AreEqual(4.2, compiler.Calculate());
        }

        [Test]
        public void LeadingAndTrailingSpaces_DE()
        {
            var compiler = new EquationCompiler(" 4,2 ", CultureInfo.GetCultureInfo("de-DE"));
            Assert.AreEqual(4.2, compiler.Calculate());
        }

        [Test]
        public void DifferentGroupSeparators_FR()
        {
            var compiler = new EquationCompiler("4\u202f200,50", CultureInfo.GetCultureInfo("fr-FR")); // French narrow no-break space
            Assert.AreEqual(4200.50, compiler.Calculate());
        }

        [Test]
        public void NegativeNumber_EN()
        {
            var compiler = new EquationCompiler("-4.2", CultureInfo.GetCultureInfo("en-US"));
            Assert.AreEqual(-4.2, compiler.Calculate());
        }

        [Test]
        public void NegativeNumber_DE()
        {
            var compiler = new EquationCompiler("-4,2", CultureInfo.GetCultureInfo("de-DE"));
            Assert.AreEqual(-4.2, compiler.Calculate());
        }

        [Test]
        public void NoDecimalPlaces_EN()
        {
            var compiler = new EquationCompiler("1000", CultureInfo.GetCultureInfo("en-US"));
            Assert.AreEqual(1000, compiler.Calculate());
        }

        [Test]
        public void NoDecimalPlaces_RU()
        {
            var compiler = new EquationCompiler("1\u00a0000", CultureInfo.GetCultureInfo("ru-RU")); // Space as thousands separator
            Assert.AreEqual(1000, compiler.Calculate());
        }

        [Test]
        public void ScientificNotation_EN()
        {
            var compiler = new EquationCompiler("1.2E3", CultureInfo.GetCultureInfo("en-US"));
            Assert.AreEqual(1200, compiler.Calculate());
        }

        [Test]
        public void ScientificNotation_FR()
        {
            var compiler = new EquationCompiler("1,2E3", CultureInfo.GetCultureInfo("fr-FR"));
            Assert.AreEqual(1200, compiler.Calculate());
        }

        [Test]
        public void InvalidFormat_DoubleComma_DE()
        {
            Assert.Throws<FormatException>(() =>
            {
                var compiler = new EquationCompiler("4,,2", CultureInfo.GetCultureInfo("de-DE"));
                compiler.Calculate();
            });
        }

        [Test]
        public void InvalidFormat_DoubleDot_EN()
        {
            Assert.Throws<FormatException>(() =>
            {
                var compiler = new EquationCompiler("4..2", CultureInfo.GetCultureInfo("en-US"));
                compiler.Calculate();
            });
        }

        [Test]
        public void InvalidFormat_MixedSeparators()
        {
            Assert.Throws<FormatException>(() =>
            {
                var compiler = new EquationCompiler("4.2,3", CultureInfo.GetCultureInfo("en-US"));
                compiler.Calculate();
            });
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

            compiler.SetFunction("a<>b");
            Assert.AreEqual(a != b, Convert.ToBoolean(compiler.Calculate()));
        }

        [TestCase(1, 0)]
        [TestCase(1, 1)]
        [TestCase(0, 1)]
        [TestCase(0, 0)]
        public void And(double a, double b)
	    {
            var compiler = new EquationCompiler("a&&b");
            compiler.SetVariable("a", a);
            compiler.SetVariable("b", b);
            Assert.AreEqual(Convert.ToBoolean(a) && Convert.ToBoolean(b), Convert.ToBoolean(compiler.Calculate()));
        }

        [TestCase(1, 0)]
        [TestCase(1, 1)]
        [TestCase(0, 1)]
        [TestCase(0, 0)]
        public void Or(double a, double b)
        {
            var compiler = new EquationCompiler("a||b");
            compiler.SetVariable("a", a);
            compiler.SetVariable("b", b);
            Assert.AreEqual(Convert.ToBoolean(a) || Convert.ToBoolean(b), Convert.ToBoolean(compiler.Calculate()));
        }

        [TestCase("3 + 4 / 2", ExpectedResult = 3 + 4 / 2.0)]
		[TestCase("(3 + 4) / 2", ExpectedResult = (3 + 4) / 2.0)]
		[TestCase("1 != 2 && 2 != 3", ExpectedResult = 1)]
		[TestCase("1 && 0 || 1 && 1", ExpectedResult = 1)]
        public double OrderOfOperations(string equation)
		{
			var compiler = new EquationCompiler(equation);
			return compiler.Calculate();
		}

        [TestCase("1 + 2 + 3", ExpectedResult = 1 + 2 + 3)]
        [TestCase("1 - 2 - 3", ExpectedResult = 1 - 2 - 3)]
        [TestCase("1 * 2 * 3", ExpectedResult = 1 * 2 * 3)]
        [TestCase("1 / 2 / 3", ExpectedResult = 1.0 / 2.0 / 3.0)]
        public double MultipleOfSameOperator(string equation)
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

        [Test]
        public void GetVariableNames()
        {
            var compiler = new EquationCompiler("a+b+c");

            CollectionAssert.AreEquivalent(new string[] { "a", "b", "c" }, compiler.GetVariableNames());
        }

        [Test]
        public void GetVariableNames_CollectionIsReadOnly()
        {
            var compiler = new EquationCompiler("a+b+c");

            Assert.IsTrue(compiler.GetVariableNames().IsReadOnly);
        }
    }
}
