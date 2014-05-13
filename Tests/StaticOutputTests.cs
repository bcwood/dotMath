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

		private EqCompiler GetCompilerSetup(string sFunction)
		{
			m_a = -4;
			m_b = 4;
			m_c = -8;
			m_d = -8;

			EqCompiler oComp = new EqCompiler(sFunction);
			oComp.SetVariable("a", m_a);
			oComp.SetVariable("b", m_b);
			oComp.SetVariable("c", m_c);
			oComp.SetVariable("d", m_d);

			return oComp;
		}

		[Test]
		public void ConstantExpression()
		{
			EqCompiler oComp = GetCompilerSetup("a");

			Assert.AreEqual(m_a, oComp.Calculate());
		}


		[Test]
		public void SignNeg()
		{
			EqCompiler oComp = new EqCompiler("-4.2", false);

			Assert.AreEqual(-4.2, oComp.Calculate());
		}

		[Test]
		public void DivideByZero()
		{
			EqCompiler oComp = new EqCompiler("4/0", false);

			double dValue1 = 4;
			double dValue2 = 0;
			double dValue = dValue1 / dValue2;
			Assert.AreEqual(dValue, oComp.Calculate());
		}

		[Test]
		public void Addition()
		{
			EqCompiler oComp = new EqCompiler("2+2", false);

			Assert.AreEqual(2 + 2, oComp.Calculate());
		}

		[Test]
		public void Multiplication()
		{
			EqCompiler oComp = new EqCompiler("4*4", false);

			Assert.AreEqual(4 * 4, oComp.Calculate());
		}

		[Test]
		public void Division()
		{
			EqCompiler oComp = new EqCompiler("16/2", false);

			Assert.AreEqual(16 / 2, oComp.Calculate());
		}

		[Test]
		public void Subtraction()
		{
			EqCompiler oComp = new EqCompiler("10-2", false);

			Assert.AreEqual(10 - 2, oComp.Calculate());
		}

		[Test]
		public void Exponent()
		{
			EqCompiler oComp = new EqCompiler("8^2", false);

			Assert.AreEqual(Math.Pow(8, 2), oComp.Calculate());
		}

		[Test]
		public void MultipleFunctionsPerObject()
		{
			EqCompiler oComp = new EqCompiler("abs(-4)");
			Assert.AreEqual(Math.Abs(-4), oComp.Calculate());

			oComp.SetFunction("acos(10)");
			Assert.AreEqual(Math.Acos(10), oComp.Calculate());
		}

		[Test]
		public void NestedFunctions()
		{
			EqCompiler oComp = new EqCompiler("sin(cos(tan(4.2)))");

			Assert.AreEqual(Math.Sin(Math.Cos(Math.Tan(4.2))), oComp.Calculate());
		}

		[Test]
		public void Abs()
		{
			EqCompiler oComp = new EqCompiler("abs(-5)");

			Assert.AreEqual(Math.Abs(-5), oComp.Calculate());
		}

		[Test]
		public void Acos()
		{
			EqCompiler oComp = new EqCompiler("acos(4)");

			Assert.AreEqual(Math.Acos(4), oComp.Calculate());
		}

		[Test]
		public void Asin()
		{
			EqCompiler oComp = new EqCompiler("asin(4)");

			Assert.AreEqual(Math.Asin(4), oComp.Calculate());
		}

		[Test]
		public void Atan()
		{
			EqCompiler oComp = new EqCompiler("atan(4)");

			Assert.AreEqual(Math.Atan(4), oComp.Calculate());
		}

		[Test]
		public void Ceiling()
		{
			EqCompiler oComp = new EqCompiler("ceiling(-3.2)");

			Assert.AreEqual(Math.Ceiling(-3.2), oComp.Calculate());
		}

		[Test]
		public void Cos()
		{
			EqCompiler oComp = new EqCompiler("cos(4)");

			Assert.AreEqual(Math.Cos(4), oComp.Calculate());
		}

		[Test]
		public void Cosh()
		{
			EqCompiler oComp = new EqCompiler("cosh(4)");

			Assert.AreEqual(Math.Cosh(4), oComp.Calculate());
		}

		[Test]
		public void Exp()
		{
			EqCompiler oComp = new EqCompiler("exp(4.2)");

			Assert.AreEqual(Math.Exp(4.2), oComp.Calculate());
		}

		[Test]
		public void Floor()
		{
			EqCompiler oComp = new EqCompiler("floor(-4.2)");

			Assert.AreEqual(Math.Floor(-4.2), oComp.Calculate());
		}

		[Test]
		public void Log()
		{
			EqCompiler oComp = new EqCompiler("log(4.2)");

			Assert.AreEqual(Math.Log(4.2), oComp.Calculate());
		}

		[Test]
		public void Log10()
		{
			EqCompiler oComp = new EqCompiler("log10(4.2)");

			Assert.AreEqual(Math.Log10(4.2), oComp.Calculate());
		}

		[Test]
		public void Round()
		{
			EqCompiler oComp = new EqCompiler("round(4.2)");

			Assert.AreEqual(Math.Round(4.2), oComp.Calculate());
		}

		[Test]
		public void Sign()
		{
			EqCompiler oComp = new EqCompiler("sign(-4.2)");

			Assert.AreEqual(Math.Sign(-4.2), oComp.Calculate());
		}

		[Test]
		public void Sin()
		{
			EqCompiler oComp = new EqCompiler("sin(4.2)");

			Assert.AreEqual(Math.Sin(4.2), oComp.Calculate());
		}

		[Test]
		public void Sinh()
		{
			EqCompiler oComp = new EqCompiler("sinh(4.2)");

			Assert.AreEqual(Math.Sinh(4.2), oComp.Calculate());
		}

		[Test]
		public void Sqrt()
		{
			EqCompiler oComp = new EqCompiler("sqrt(4.2)");

			Assert.AreEqual(Math.Sqrt(4.2), oComp.Calculate());
		}

		[Test]
		public void Tan()
		{
			EqCompiler oComp = new EqCompiler("tan(4.2)");

			Assert.AreEqual(Math.Tan(4.2), oComp.Calculate());
		}

		[Test]
		public void Tanh()
		{
			EqCompiler oComp = new EqCompiler("tanh(4.2)");

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
			EqCompiler oComp = new EqCompiler();

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
