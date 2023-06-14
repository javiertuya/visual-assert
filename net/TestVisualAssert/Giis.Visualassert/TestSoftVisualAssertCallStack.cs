/////////////////////////////////////////////////////////////////////////////////////////////
/////// THIS FILE HAS BEEN AUTOMATICALLY CONVERTED FROM THE JAVA SOURCES. DO NOT EDIT ///////
/////////////////////////////////////////////////////////////////////////////////////////////
using System;
using Giis.Visualassert.Portable;
using NUnit.Framework;


namespace Giis.Visualassert
{
	/// <summary>Checks the stack trace items shown in messages.</summary>
	/// <remarks>
	/// Checks the stack trace items shown in messages.
	/// WARNING: Expected messages (at the end of this class) are sensitive to
	/// the line numbers where the asserts are thrown.
	/// Adjust the numbers if new lines are added to the code
	/// </remarks>
	public class TestSoftVisualAssertCallStack
	{
		public class OtherClass
		{
			//Deep stack assert invocations
			public virtual void DoAssert(SoftVisualAssert va)
			{
				va.AssertEquals("xy vw", "xy zz vw", string.Empty, "fstack12.html");
			}

			internal OtherClass(TestSoftVisualAssertCallStack _enclosing)
			{
				this._enclosing = _enclosing;
			}

			private readonly TestSoftVisualAssertCallStack _enclosing;
		}

		private void CallDoAssert(SoftVisualAssert va)
		{
			TestSoftVisualAssertCallStack.OtherClass cls = new TestSoftVisualAssertCallStack.OtherClass(this);
			cls.DoAssert(va);
		}

		/*
		* Conditions:
		* -default(1)/more traces (0 already tested)
		* -single/multiple fault
		* -assert in the same test case/deeper in call stack (test class/other class)
		*/
		[Test]
		public virtual void TestSingleStackItem()
		{
			SoftVisualAssert va = new SoftVisualAssert();
			//default stack 1
			va.AssertEquals("ab zz cd", "ab cd", string.Empty, "fstack11.html");
			CallDoAssert(va);
			try
			{
				va.AssertAll();
				NUnit.Framework.Assert.Fail("this should fail");
			}
			catch (Exception e)
			{
				NUnit.Framework.Assert.AreEqual(CallStack.Normalize(GetExpectedSingleStackItem()), CallStack.Normalize(e.Message));
			}
		}

		[Test]
		public virtual void TestMultipleStackItem()
		{
			SoftVisualAssert va = new SoftVisualAssert().SetCallStackLength(3);
			CallDoAssert(va);
			try
			{
				va.AssertAll();
				NUnit.Framework.Assert.Fail("this should fail");
			}
			catch (Exception e)
			{
				NUnit.Framework.Assert.AreEqual(CallStack.Normalize(GetExpectedMultipleStackItem()), CallStack.Normalize(e.Message));
			}
		}

		[Test]
		public virtual void TestZeroStackItem()
		{
			SoftVisualAssert va = new SoftVisualAssert().SetCallStackLength(0);
			va.AssertEquals("ab zz cd", "ab cd", string.Empty, "fstack11.html");
			CallDoAssert(va);
			try
			{
				va.AssertAll();
				NUnit.Framework.Assert.Fail("this should fail");
			}
			catch (Exception e)
			{
				NUnit.Framework.Assert.AreEqual(CallStack.Normalize(GetExpectedZeroStackItem()), CallStack.Normalize(e.Message));
			}
		}

		private string GetExpectedSingleStackItem()
		{
			return "There are 2 failed assertion(s)\n" + "Failure 1: Strings are different. First diff at line 1 column 4.\n" + "- Visual diffs at: ../../../../reports/fstack11.html\n" + "- Call Stack:\n" + "    at Giis.Visualassert.TestSoftVisualAssertCallStack.TestSingleStackItem(TestSoftVisualAssertCallStack.java:37)\n"
				 + "Failure 2: Strings are different. First diff at line 1 column 4.\n" + "- Visual diffs at: ../../../../reports/fstack12.html\n" + "- Call Stack:\n" + "    at Giis.Visualassert.TestSoftVisualAssertCallStack.OtherClass.DoAssert(TestSoftVisualAssertCallStack.java:19)";
		}

		private string GetExpectedMultipleStackItem()
		{
			return "There are 1 failed assertion(s)\n" + "Failure 1: Strings are different. First diff at line 1 column 4.\n" + "- Visual diffs at: ../../../../reports/fstack12.html\n" + "- Call Stack:\n" + "    at Giis.Visualassert.TestSoftVisualAssertCallStack.OtherClass.DoAssert(TestSoftVisualAssertCallStack.java:19)\n"
				 + "    at Giis.Visualassert.TestSoftVisualAssertCallStack.CallDoAssert(TestSoftVisualAssertCallStack.java:25)\n" + "    at Giis.Visualassert.TestSoftVisualAssertCallStack.TestMultipleStackItem(TestSoftVisualAssertCallStack.java:49)";
		}

		private string GetExpectedZeroStackItem()
		{
			return "There are 2 failed assertion(s)\n" + "Failure 1: Strings are different. First diff at line 1 column 4.\n" + "- Visual diffs at: ../../../../reports/fstack11.html\n" + "Failure 2: Strings are different. First diff at line 1 column 4.\n" + "- Visual diffs at: ../../../../reports/fstack12.html";
		}
	}
}
