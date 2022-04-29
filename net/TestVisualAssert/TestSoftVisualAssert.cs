using System;
using Giis.Visualassert.Portable;
using NUnit.Framework;

namespace Giis.Visualassert
{
	public class TestSoftVisualAssert
	{
		string expectedMessage = "There are 3 failed assertion(s)\n"
				+ "Failure 1: Strings are different.\n"
				+ "- Visual diffs at: f1.html\n"
				+ "Failure 2: Fail assertion raised.\n"
				+ "msg4\n"
				+ "Failure 3: Strings are different.\n"
				+ "msg5.\n"
				+ "- Visual diffs at: diff-0.html";

		[Test]
		public void TestNoFail()
		{
			SoftVisualAssert va = new SoftVisualAssert();
			va.AssertEquals("ab cd", "ab cd");
			va.AssertEquals("xy vw", "xy vw");
			va.AssertAll();
		}

		/*
		 * Conditions:
		 * -with/without failures
		 * -use fail
		 * -set/no set out file name
		 * -with/without additional message
		 * -reset because assertAll/assertClear
		 */
		[Test]
		public void TestFail()
		{
			//uses a different path for files to avoid filename collisions with other tests
			String tempReportPath = FileUtil.GetPath(JavaCs.DefaultReportSubdir, "tmp-" + JavaCs.GetUniqueId());
			SoftVisualAssert va = (SoftVisualAssert)new SoftVisualAssert()
					.SetReportSubdir(tempReportPath).ClearCurrentSequence();

			va.AssertEquals("ab zz cd", "ab cd", "", "f1.html");
			va.AssertEquals("ab cd", "ab cd", "msg2");
			va.AssertEquals("mn op", "mn op", "msg3");
			va.Fail("msg4");
			va.AssertEquals("xy vw", "xy zz vw", "msg5");
			Assert.AreEqual(3, va.GetFailureCount());
			try
			{
				va.AssertAll();
				Assert.Fail("this should fail");
			}
			catch (Exception e)
			{
				//first transforms the file name in expected mesage to include the path
				expectedMessage = expectedMessage.Replace("f1.html", FileUtil.GetPath(tempReportPath, "f1.html"));
				expectedMessage = expectedMessage.Replace("diff-0.html", FileUtil.GetPath(tempReportPath, "diff-0.html"));
				Assert.AreEqual(expectedMessage, e.Message);
			}

			//assertAll resets the list
			Assert.AreEqual(0, va.GetFailureCount());
			va.AssertAll();
			//assertClear resets the list
			va.AssertEquals("ab zz cd", "ab cd");
			Assert.AreEqual(1, va.GetFailureCount());
			va.AssertClear();
			Assert.AreEqual(0, va.GetFailureCount());
			va.AssertAll();
		}
	}
}
