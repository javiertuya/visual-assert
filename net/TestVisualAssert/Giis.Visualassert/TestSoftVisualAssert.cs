/////////////////////////////////////////////////////////////////////////////////////////////
/////// THIS FILE HAS BEEN AUTOMATICALLY CONVERTED FROM THE JAVA SOURCES. DO NOT EDIT ///////
/////////////////////////////////////////////////////////////////////////////////////////////
using System;
using Giis.Visualassert.Portable;
using NUnit.Framework;


namespace Giis.Visualassert
{
	public class TestSoftVisualAssert
	{
		[Test]
		public virtual void TestSoftNoFail()
		{
			SoftVisualAssert va = new SoftVisualAssert();
			va.AssertEquals("ab cd", "ab cd");
			va.AssertEquals("xy vw", "xy vw");
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
		public virtual void TestSoftFail()
		{
			//uses a different path for files to avoid filename collisions with other tests
			string tempReportPath = FileUtil.GetPath(JavaCs.DefaultReportSubdir, "tmp-" + JavaCs.GetUniqueId());
			SoftVisualAssert va = new SoftVisualAssert().SetReportSubdir(tempReportPath).ClearCurrentSequence();
			DoFailSoftAssert(va, ExpectedMessage(false), tempReportPath, string.Empty);
		}

		//Actual execution and assertions in a method that will be reused to test with frameworks
		public static void DoFailSoftAssert(SoftVisualAssert va, string expected, string reportPath, string aggregateFile)
		{
			va.AssertEquals("ab zz cd", "ab cd", string.Empty, "f1.html");
			va.AssertEquals("ab cd", "ab cd", "msg2");
			va.AssertEquals("mn op", "mn op", "msg3");
			va.Fail("msg4");
			va.AssertEquals(null, "this is notnull", "msgen", "fen.html");
			va.AssertEquals("this is notnull", null, "msgan", "fan.html");
			va.AssertEquals("xy vw", "xy zz vw", "msg5");
			NUnit.Framework.Legacy.ClassicAssert.AreEqual(5, va.GetFailureCount());
			bool success = false;
			try
			{
				if (string.Empty.Equals(aggregateFile))
				{
					va.AssertAll();
				}
				else
				{
					va.AssertAll(aggregateFile);
				}
			}
			catch (Exception e)
			{
				//first transforms the file name in expected mesage to include the path
				expected = expected.Replace("f1.html", FileUtil.GetPath(reportPath, "f1.html"));
				expected = expected.Replace("fen.html", FileUtil.GetPath(reportPath, "fen.html"));
				expected = expected.Replace("fan.html", FileUtil.GetPath(reportPath, "fan.html"));
				expected = expected.Replace("diff-0.html", FileUtil.GetPath(reportPath, "diff-0.html"));
				expected = expected.Replace("Aggregate.html", FileUtil.GetPath(reportPath, "Aggregate.html"));
				NUnit.Framework.Legacy.ClassicAssert.AreEqual(CallStack.Normalize(expected), CallStack.Normalize(e.Message));
				success = true;
			}
			NUnit.Framework.Legacy.ClassicAssert.IsTrue(success);
			//assertAll resets the list
			NUnit.Framework.Legacy.ClassicAssert.AreEqual(0, va.GetFailureCount());
			va.AssertAll();
			//assertClear resets the list
			va.AssertEquals("ab zz cd", "ab cd");
			NUnit.Framework.Legacy.ClassicAssert.AreEqual(1, va.GetFailureCount());
			va.AssertClear();
			NUnit.Framework.Legacy.ClassicAssert.AreEqual(0, va.GetFailureCount());
			va.AssertAll();
		}

		public static string ExpectedMessage(bool aggregateDiffs)
		{
			return "There are 5 failed assertion(s)\n" + (aggregateDiffs ? "Aggregated visual diffs at: Aggregate.html\n" : string.Empty) + "Failure 1: Strings are different. First diff at line 1 column 4.\n" + "- Visual diffs at: f1.html\n" + "- Call Stack:\n" + "    at Giis.Visualassert.TestSoftVisualAssert.DoFailSoftAssert(TestSoftVisualAssert.java:15)\n"
				 + "Failure 2: Fail assertion raised.\n" + "msg4\n" + "- Call Stack:\n" + "    at Giis.Visualassert.TestSoftVisualAssert.DoFailSoftAssert(TestSoftVisualAssert.java:18)\n" + "Failure 3: Strings are different. Expected was <null>.\n" + "msgen.\n" + "- Visual diffs at: fen.html\n" +
				 "- Call Stack:\n" + "    at Giis.Visualassert.TestSoftVisualAssert.DoFailSoftAssert(TestSoftVisualAssert.java:19)\n" + "Failure 4: Strings are different. Actual was <null>.\n" + "msgan.\n" + "- Visual diffs at: fan.html\n" + "- Call Stack:\n" + "    at Giis.Visualassert.TestSoftVisualAssert.DoFailSoftAssert(TestSoftVisualAssert.java:20)\n"
				 + "Failure 5: Strings are different. First diff at line 1 column 4.\n" + "msg5.\n" + "- Visual diffs at: diff-0.html\n" + "- Call Stack:\n" + "    at Giis.Visualassert.TestSoftVisualAssert.DoFailSoftAssert(TestSoftVisualAssert.java:21)";
		}

		[Test]
		public virtual void TestNormalizeLineEndings()
		{
			string linux = "line1\nline2\nline3\n";
			string windows = "line1\r\nline2\r\nline3\r\n";
			SoftVisualAssert sva = new SoftVisualAssert().SetNormalizeEol(true);
			sva.AssertEquals(windows, linux, "Should not fail with normalize eol");
			sva.AssertEquals(linux, windows, "Should not fail with normalize eol");
			sva.AssertEquals(null, null, "Should not fail with nulls");
			sva.AssertAll("sva-normalize-eol.html");
		}
	}
}
