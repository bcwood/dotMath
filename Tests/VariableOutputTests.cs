using System;
using NUnit.Framework;

namespace dotMath.Tests
{
	[TestFixture]
	public class VariableOutputTests
	{
		private double m_a = -4;
		private double m_b = 4;
		private double m_c = -8;
		private double m_d = 8;

		private void InitVarRun()
		{
			m_a = -100;
			m_b = 100;
			m_c = -100;
			m_d = 100;
		}

		private void StrobeValues()
		{
			m_a += 1;
			m_b -= 1;
			m_c += .125;
			m_d -= .125;
		}

		[Test]
		public void RangeTest()
		{
			InitVarRun();

			for (int i = 0; i < 200; i++)
			{
				Abs();
				Acos();
				Addition();
				Asin();
				Atan();
				Ceiling();
				ConstantExpression();
				Cos();
				Cosh();
				Division();
				Exp();
				Exponent();
				Floor();
				IfElse();
				IfThen();
				Log();
				Log10();
				Max();
				Min();
				MultipleFunctionsPerObject();
				Multiplication();
				NestedFunctions();
				Round();
				Sign();
				SignNeg();
				Sin();
				Sinh();
				Subtraction();
				Tan();
				Tanh();

				StrobeValues();
			}
		}

		private EqCompiler GetCompilerSetup(string sFunction)
		{
			EqCompiler oComp = new EqCompiler(sFunction, true);
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
			EqCompiler oComp = GetCompilerSetup("-a");

			Assert.AreEqual(-m_a, oComp.Calculate());
		}

		[Test]
		public void Addition()
		{
			EqCompiler oComp = GetCompilerSetup("a+b");

			Assert.AreEqual(m_a + m_b, oComp.Calculate());
		}

		[Test]
		public void Multiplication()
		{
			EqCompiler oComp = GetCompilerSetup("a*b");

			Assert.AreEqual(m_a * m_b, oComp.Calculate());
		}

		[Test]
		public void Division()
		{
			EqCompiler oComp = GetCompilerSetup("a/b");

			Assert.AreEqual(m_a / m_b, oComp.Calculate());
		}

		[Test]
		public void Subtraction()
		{
			EqCompiler oComp = GetCompilerSetup("a-b");

			Assert.AreEqual(m_a - m_b, oComp.Calculate());
		}

		[Test]
		public void Exponent()
		{
			EqCompiler oComp = GetCompilerSetup("a^b");

			Assert.AreEqual(Math.Pow(m_a, m_b), oComp.Calculate());
		}

		[Test]
		public void MultipleFunctionsPerObject()
		{
			EqCompiler oComp = GetCompilerSetup("abs(b)");
			Assert.AreEqual(Math.Abs(m_b), oComp.Calculate());

			oComp.SetFunction("acos(a)");
			Assert.AreEqual(Math.Acos(m_a), oComp.Calculate());
		}

		[Test]
		public void NestedFunctions()
		{
			EqCompiler oComp = GetCompilerSetup("sin(cos(tan(a)))");

			Assert.AreEqual(Math.Sin(Math.Cos(Math.Tan(m_a))), oComp.Calculate());
		}

		[Test]
		public void Abs()
		{
			EqCompiler oComp = GetCompilerSetup("abs(-a)");

			Assert.AreEqual(Math.Abs(-m_a), oComp.Calculate());
		}

		[Test]
		public void Acos()
		{
			EqCompiler oComp = GetCompilerSetup("acos(a)");

			Assert.AreEqual(Math.Acos(m_a), oComp.Calculate());
		}

		[Test]
		public void Asin()
		{
			EqCompiler oComp = GetCompilerSetup("asin(a)");

			Assert.AreEqual(Math.Asin(m_a), oComp.Calculate());
		}

		[Test]
		public void Atan()
		{
			EqCompiler oComp = GetCompilerSetup("atan(a)");

			Assert.AreEqual(Math.Atan(m_a), oComp.Calculate());
		}

		[Test]
		public void Ceiling()
		{
			EqCompiler oComp = GetCompilerSetup("ceiling(b)");

			Assert.AreEqual(Math.Ceiling(m_b), oComp.Calculate());
		}

		[Test]
		public void Cos()
		{
			EqCompiler oComp = GetCompilerSetup("cos(b)");

			Assert.AreEqual(Math.Cos(m_b), oComp.Calculate());
		}

		[Test]
		public void Cosh()
		{
			EqCompiler oComp = GetCompilerSetup("cosh(a)");

			Assert.AreEqual(Math.Cosh(m_a), oComp.Calculate());
		}

		[Test]
		public void Exp()
		{
			EqCompiler oComp = GetCompilerSetup("exp(a)");

			Assert.AreEqual(Math.Exp(m_a), oComp.Calculate());
		}

		[Test]
		public void Floor()
		{
			EqCompiler oComp = GetCompilerSetup("floor(-a)");

			Assert.AreEqual(Math.Floor(-m_a), oComp.Calculate());
		}

		[Test]
		public void Log()
		{
			EqCompiler oComp = GetCompilerSetup("log(a)");

			Assert.AreEqual(Math.Log(m_a), oComp.Calculate());
		}

		[Test]
		public void Log10()
		{
			EqCompiler oComp = GetCompilerSetup("log10(a)");

			Assert.AreEqual(Math.Log10(m_a), oComp.Calculate());
		}

		[Test]
		public void Round()
		{
			EqCompiler oComp = GetCompilerSetup("round(a)");

			Assert.AreEqual(Math.Round(m_a), oComp.Calculate());
		}

		[Test]
		public void Sign()
		{
			EqCompiler oComp = GetCompilerSetup("sign(-a)");

			Assert.AreEqual(Math.Sign(-m_a), oComp.Calculate());
		}

		[Test]
		public void Sin()
		{
			EqCompiler oComp = GetCompilerSetup("sin(a)");

			Assert.AreEqual(Math.Sin(m_a), oComp.Calculate());
		}

		[Test]
		public void Sinh()
		{
			EqCompiler oComp = GetCompilerSetup("sinh(a)");

			Assert.AreEqual(Math.Sinh(m_a), oComp.Calculate());
		}

		[Test]
		public void Sqrt()
		{
			EqCompiler oComp = GetCompilerSetup("sqrt(a)");

			Assert.AreEqual(Math.Sqrt(m_a), oComp.Calculate());
		}

		[Test]
		public void Tan()
		{
			EqCompiler oComp = GetCompilerSetup("tan(a)");

			Assert.AreEqual(Math.Tan(m_a), oComp.Calculate());
		}

		[Test]
		public void Tanh()
		{
			EqCompiler oComp = GetCompilerSetup("tanh(a)");

			Assert.AreEqual(Math.Tanh(m_a), oComp.Calculate());
		}

		[Test]
		public void Max()
		{
			EqCompiler oComp = GetCompilerSetup("max(a,b,c,d)");
			double dMax = m_a;

			if (dMax < m_b)
				dMax = m_b;
			if (dMax < m_c)
				dMax = m_c;
			if (dMax < m_d)
				dMax = m_d;

			Assert.AreEqual(dMax, oComp.Calculate());
		}

		[Test]
		public void Min()
		{
			EqCompiler oComp = GetCompilerSetup("min(a,b,c,d)");

			double dMin = m_a;
			if (dMin > m_b)
				dMin = m_b;
			if (dMin > m_c)
				dMin = m_c;
			if (dMin > m_d)
				dMin = m_d;

			Assert.AreEqual(dMin, oComp.Calculate());
		}

		[Test]
		public void IfThen()
		{
			string sTest;
			if (m_a < m_b)
				sTest = "if(a<b,c,d)";
			else
			{
				if (m_a == m_b)
					sTest = "if(a==b,c,d)";
				else
					sTest = "if(b<a,c,d)";
			}


			EqCompiler oComp = GetCompilerSetup(sTest);

			Assert.AreEqual(m_c, oComp.Calculate());
		}

		[Test]
		public void IfElse()
		{
			string sTest;
			if (m_a < m_b)
				sTest = "if(a>b,c,d)";
			else
				sTest = "if(a<b,c,d)";

			EqCompiler oComp = GetCompilerSetup(sTest);

			Assert.AreEqual(m_d, oComp.Calculate());
		}
	}
}
