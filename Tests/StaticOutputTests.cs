using System;
using NUnit.Framework;

namespace dotMath.Tests
{
	[TestFixture]
	public class StaticOutputTests
	{
		private double m_a;
		private double m_b;
		private double m_c;
		private double m_d;

		private EquationCompiler GetCompilerSetup(string sFunction)
		{
			m_a = -4;
			m_b = 4;
			m_c = -8;
			m_d = -8;

			EquationCompiler oComp = new EquationCompiler(sFunction);
			oComp.SetVariable("a", m_a);
			oComp.SetVariable("b", m_b);
			oComp.SetVariable("c", m_c);
			oComp.SetVariable("d", m_d);

			return oComp;
		}

		[Test]
		public void ConstantExpression()
		{
			EquationCompiler oComp = GetCompilerSetup("a");

			Assert.AreEqual(m_a, oComp.Calculate());
		}


		[Test]
		public void SignNeg()
		{
			EquationCompiler oComp = new EquationCompiler("-4.2");

			Assert.AreEqual(-4.2, oComp.Calculate());
		}

		[Test]
		public void DivideByZero()
		{
			EquationCompiler oComp = new EquationCompiler("4/0");

			double dValue1 = 4;
			double dValue2 = 0;
			double dValue = dValue1 / dValue2;
			Assert.AreEqual(dValue, oComp.Calculate());
		}

		[Test]
		public void Addition()
		{
			EquationCompiler oComp = new EquationCompiler("2+2");

			Assert.AreEqual(2 + 2, oComp.Calculate());
		}

		[Test]
		public void Multiplication()
		{
			EquationCompiler oComp = new EquationCompiler("4*4");

			Assert.AreEqual(4 * 4, oComp.Calculate());
		}

		[Test]
		public void Division()
		{
			EquationCompiler oComp = new EquationCompiler("16/2");

			Assert.AreEqual(16 / 2, oComp.Calculate());
		}

		[Test]
		public void Subtraction()
		{
			EquationCompiler oComp = new EquationCompiler("10-2");

			Assert.AreEqual(10 - 2, oComp.Calculate());
		}

		[Test]
		public void Exponent()
		{
			EquationCompiler oComp = new EquationCompiler("8^2");

			Assert.AreEqual(Math.Pow(8, 2), oComp.Calculate());
		}

		[Test]
		public void MultipleFunctionsPerObject()
		{
			EquationCompiler oComp = new EquationCompiler("abs(-4)");
			Assert.AreEqual(Math.Abs(-4), oComp.Calculate());

			oComp.SetFunction("acos(10)");
			Assert.AreEqual(Math.Acos(10), oComp.Calculate());
		}

		[Test]
		public void NestedFunctions()
		{
			EquationCompiler oComp = new EquationCompiler("sin(cos(tan(4.2)))");

			Assert.AreEqual(Math.Sin(Math.Cos(Math.Tan(4.2))), oComp.Calculate());
		}

		[Test]
		public void Abs()
		{
			EquationCompiler oComp = new EquationCompiler("abs(-5)");

			Assert.AreEqual(Math.Abs(-5), oComp.Calculate());
		}

		[Test]
		public void Acos()
		{
			EquationCompiler oComp = new EquationCompiler("acos(4)");

			Assert.AreEqual(Math.Acos(4), oComp.Calculate());
		}

		[Test]
		public void Asin()
		{
			EquationCompiler oComp = new EquationCompiler("asin(4)");

			Assert.AreEqual(Math.Asin(4), oComp.Calculate());
		}

		[Test]
		public void Atan()
		{
			EquationCompiler oComp = new EquationCompiler("atan(4)");

			Assert.AreEqual(Math.Atan(4), oComp.Calculate());
		}

		[Test]
		public void Ceiling()
		{
			EquationCompiler oComp = new EquationCompiler("ceiling(-3.2)");

			Assert.AreEqual(Math.Ceiling(-3.2), oComp.Calculate());
		}

		[Test]
		public void Cos()
		{
			EquationCompiler oComp = new EquationCompiler("cos(4)");

			Assert.AreEqual(Math.Cos(4), oComp.Calculate());
		}

		[Test]
		public void Cosh()
		{
			EquationCompiler oComp = new EquationCompiler("cosh(4)");

			Assert.AreEqual(Math.Cosh(4), oComp.Calculate());
		}

		[Test]
		public void Exp()
		{
			EquationCompiler oComp = new EquationCompiler("exp(4.2)");

			Assert.AreEqual(Math.Exp(4.2), oComp.Calculate());
		}

		[Test]
		public void Floor()
		{
			EquationCompiler oComp = new EquationCompiler("floor(-4.2)");

			Assert.AreEqual(Math.Floor(-4.2), oComp.Calculate());
		}

		[Test]
		public void Log()
		{
			EquationCompiler oComp = new EquationCompiler("log(4.2)");

			Assert.AreEqual(Math.Log(4.2), oComp.Calculate());
		}

		[Test]
		public void Log10()
		{
			EquationCompiler oComp = new EquationCompiler("log10(4.2)");

			Assert.AreEqual(Math.Log10(4.2), oComp.Calculate());
		}

		[Test]
		public void Round()
		{
			EquationCompiler oComp = new EquationCompiler("round(4.2)");

			Assert.AreEqual(Math.Round(4.2), oComp.Calculate());
		}

		[Test]
		public void Sign()
		{
			EquationCompiler oComp = new EquationCompiler("sign(-4.2)");

			Assert.AreEqual(Math.Sign(-4.2), oComp.Calculate());
		}

		[Test]
		public void Sin()
		{
			EquationCompiler oComp = new EquationCompiler("sin(4.2)");

			Assert.AreEqual(Math.Sin(4.2), oComp.Calculate());
		}

		[Test]
		public void Sinh()
		{
			EquationCompiler oComp = new EquationCompiler("sinh(4.2)");

			Assert.AreEqual(Math.Sinh(4.2), oComp.Calculate());
		}

		[Test]
		public void Sqrt()
		{
			EquationCompiler oComp = new EquationCompiler("sqrt(4.2)");

			Assert.AreEqual(Math.Sqrt(4.2), oComp.Calculate());
		}

		[Test]
		public void Tan()
		{
			EquationCompiler oComp = new EquationCompiler("tan(4.2)");

			Assert.AreEqual(Math.Tan(4.2), oComp.Calculate());
		}

		[Test]
		public void Tanh()
		{
			EquationCompiler oComp = new EquationCompiler("tanh(4.2)");

			Assert.AreEqual(Math.Tanh(4.2), oComp.Calculate());
		}

		//[Test]
		//public void Max_ExtremeTest()
		//{
		//	StringBuilder sMax = new StringBuilder("max( 3,2,5,4", 300000);  //actual needed space is 288903

		//	for (int i = 0; i < 50000; i++)
		//		sMax.AppendFormat(",{0}", i);

		//	sMax.Append(")");

		//	EqCompiler oComp = new EqCompiler(sMax.ToString());

		//	Assert.AreEqual(49999, oComp.Calculate());
		//}

		//[Test]
		//public void Max()
		//{
		//	EqCompiler oComp = new EqCompiler("max(3,2,5,4)");

		//	Assert.AreEqual(5, oComp.Calculate());
		//}

		//[Test]
		//public void Min_ExtremeTest()
		//{
		//	StringBuilder sMin = new StringBuilder("min(3,2,5,4", 300000); //actual needed space is 288903

		//	for (int i = 0; i < 50000; i++)
		//		sMin.Append(string.Format(",{0}", i));

		//	sMin.Append(")");

		//	EqCompiler oComp = new EqCompiler(sMin.ToString());

		//	Assert.AreEqual(0, oComp.Calculate());
		//}

		//[Test]
		//public void Min()
		//{
		//	EqCompiler oComp = new EqCompiler("min(3,2,5,4)");

		//	Assert.AreEqual(2, oComp.Calculate());
		//}

		//[Test]
		//public void IfThen()
		//{
		//	EqCompiler oComp = new EqCompiler("if(1<2,3,4)");

		//	Assert.AreEqual(3, oComp.Calculate());
		//}

		//[Test]
		//public void IfElse()
		//{
		//	EqCompiler oComp = new EqCompiler("if(2<1,3,4)");

		//	Assert.AreEqual(4, oComp.Calculate());
		//}

		[Test]
		public void BinaryOperators()
		{
			EquationCompiler oComp = new EquationCompiler();

			oComp.SetFunction("1<2");
			Assert.AreEqual(1, oComp.Calculate());

			oComp.SetFunction("1<=2");
			Assert.AreEqual(1, oComp.Calculate());

			oComp.SetFunction("2>1");
			Assert.AreEqual(1, oComp.Calculate());

			oComp.SetFunction("2>=1");
			Assert.AreEqual(1, oComp.Calculate());

			oComp.SetFunction("2==2");
			Assert.AreEqual(1, oComp.Calculate());

			oComp.SetFunction("2<1");
			Assert.AreEqual(0, oComp.Calculate());

			oComp.SetFunction("2<=1");
			Assert.AreEqual(0, oComp.Calculate());

			oComp.SetFunction("1>2");
			Assert.AreEqual(0, oComp.Calculate());

			oComp.SetFunction("1>=2");
			Assert.AreEqual(0, oComp.Calculate());

			oComp.SetFunction("2==1");
			Assert.AreEqual(0, oComp.Calculate());

			oComp.SetFunction("2>=2");
			Assert.AreEqual(1, oComp.Calculate());

			oComp.SetFunction("2<=2");
			Assert.AreEqual(1, oComp.Calculate());
		}
	}
}
