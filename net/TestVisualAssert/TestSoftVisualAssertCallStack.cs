using NUnit.Framework;
using System;

namespace Giis.Visualassert
{
	/// <summary>
	/// Checks the stack trace items shown in messages.
	/// WARNING: Expected messages (at the end of this class) are sensitive to 
	/// the line numbers where the asserts are thrown.
	/// Adjust the numbers if new lines are added to the code
	/// </summary>
	public class TestSoftVisualAssertCallStack
	{

		//Deep stack assert invocations
		public class OtherClass
		{
			public void DoAssert(SoftVisualAssert va)
			{
				va.AssertEquals("xy vw", "xy zz vw", "", "fstack12.html");
			}
		}

		private void CallDoAssert(SoftVisualAssert va)
		{
			OtherClass cls = new OtherClass();
			cls.DoAssert(va);
		}

		/*
		 * Conditions:
		 * -default(1)/more traces (0 already tested)
		 * -single/multiple fault
		 * -assert in the same test case/deeper in call stack (test class/other class)
		 */
		[Test]
		public void TestSingleStackItem()
		{
			SoftVisualAssert va = new SoftVisualAssert(); //default stack 1
			va.AssertEquals("ab zz cd", "ab cd", "", "fstack11.html");
			CallDoAssert(va);
			try
			{
				va.AssertAll();
				Assert.Fail("this should fail");
			}
			catch (Exception e)
			{
				/*System.Diagnostics.Debug.WriteLine(e.Message);*/ Assert.AreEqual(GetExpectedSingleStackItem(), e.Message.Replace("\\", "/"));
			}
		}
		[Test]
		public void TestMultipleStackItem()
		{
			SoftVisualAssert va = (SoftVisualAssert)new SoftVisualAssert().SetCallStackLength(3);
			CallDoAssert(va);
			try
			{
				va.AssertAll();
				Assert.Fail("this should fail");
			}
			catch (Exception e)
			{
				/*System.Diagnostics.Debug.WriteLine(e.Message);*/ Assert.AreEqual(GetExpectedMultipleStackItem(), e.Message.Replace("\\", "/"));
			}
		}

		private string GetExpectedSingleStackItem()
		{
			return "There are 2 failed assertion(s)\n"
					+ "Failure 1: Strings are different.\n"
					+ "- Visual diffs at: ../../../../reports/fstack11.html\n"
					+ "- Call Stack:\n"
					+ "    at Giis.Visualassert.TestSoftVisualAssertCallStack.TestSingleStackItem(TestSoftVisualAssertCallStack.cs:40)\n"
					+ "Failure 2: Strings are different.\n"
					+ "- Visual diffs at: ../../../../reports/fstack12.html\n"
					+ "- Call Stack:\n"
					+ "    at Giis.Visualassert.TestSoftVisualAssertCallStack.OtherClass.DoAssert(TestSoftVisualAssertCallStack.cs:20)";
		}

		private string GetExpectedMultipleStackItem()
		{
			return "There are 1 failed assertion(s)\n"
					+ "Failure 1: Strings are different.\n"
					+ "- Visual diffs at: ../../../../reports/fstack12.html\n"
					+ "- Call Stack:\n"
					+ "    at Giis.Visualassert.TestSoftVisualAssertCallStack.OtherClass.DoAssert(TestSoftVisualAssertCallStack.cs:20)\n"
					+ "    at Giis.Visualassert.TestSoftVisualAssertCallStack.CallDoAssert(TestSoftVisualAssertCallStack.cs:27)\n"
					+ "    at Giis.Visualassert.TestSoftVisualAssertCallStack.TestMultipleStackItem(TestSoftVisualAssertCallStack.cs:56)";
		}

	}
}
