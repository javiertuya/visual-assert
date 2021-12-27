using System;
using System.Collections.Generic;
using Giis.Visualassert.Portable;
using NUnit.Framework;

namespace Giis.Visualassert
{
	public class TestVisualAssert
	{
		internal string defaultFolder = JavaCs.DefaultReportSubdir;
		internal string diffFile = "VisualAssertDiffFile.html";
		internal string expected = "abc def ghi\nmno pqr s tu";
		internal string actualNofail = "abc def ghi\nmno pqr s tu";
		internal string actualFail = " abc DEF ghi\nother line\nmno pqr stu";
		internal string expectedMessageShort = "Strings are different." + "\nThis is the additional message." + "\nVisual diffs at: VisualAssertDiffFile.html";
		internal string htmlDiffs = "<ins style=\"background:#e6ffe6;\">&nbsp;</ins>"
				+ "<span>abc </span><del style=\"background:#ffe6e6;\">def</del>"
				+ "<ins style=\"background:#e6ffe6;\">DEF</ins><span> ghi&para;"
				+ "<br></span><ins style=\"background:#e6ffe6;\">other&nbsp;line&para;"
				+ "<br></ins><span>mno pqr s</span><del style=\"background:#ffe6e6;\">&nbsp;</del><span>tu</span>";

		/*
		* What is being tested: no failure and failure with each choice of below
		* conditions on features:
		* - additional message
		* - existing report subdir
		* - use default report subdir
		* - use relative path
		* - do not show expected and actual
	    * - soft differences
		* - specified dif filename
	    * conditions on self generated file
	    * - different instances
	    * - different executions same instance
		*/
		[Test]
		public virtual void TestNoFail()
		{
			VisualAssert va = new VisualAssert();
			va.AssertEquals(expected, actualNofail);
		}

		[Test]
		public virtual void TestFailAllConditionsTrue()
		{
			VisualAssert va = new VisualAssert(); // all config methods by default
			FileUtil.CreateDirectory(defaultFolder); // ensure folder exists
			try
			{
				va.AssertEquals(expected, actualFail, "This is the additional message", diffFile);
				Assert.Fail("this should fail");
			}
			catch (Exception e)
			{
				Assert.AreEqual(expectedMessageShort, e.Message);
				Assert.AreEqual(htmlDiffs, FileUtil.FileRead(FileUtil.GetPath(defaultFolder, diffFile)));
			}
		}

		[Test]
		public virtual void TestFailAllConditionsFalse()
		{
			string tempReportPath = FileUtil.GetPath(defaultFolder, "tmp-" + JavaCs.GetUniqueId());
			//folder does not exist
			VisualAssert va = new VisualAssert()
				.SetShowExpectedAndActual(true)
				.SetUseLocalAbsolutePath(true)
				.SetSoftDifferences(true)
				.SetReportSubdir(tempReportPath);
			try
			{
				va.AssertEquals(expected, actualFail);
				Assert.Fail("this should fail");
			}
			catch (Exception e)
			{
				//get file and path of the generated diff file (only file in the folder)
				IList<string> allFiles = FileUtil.GetFileListInDirectory(tempReportPath);
				Assert.AreEqual(1, allFiles.Count); // only a file has been created
				string diffFileName = allFiles[0];
				string diffFileFullPath = "file:///" + FileUtil.GetFullPath(FileUtil.GetPath(tempReportPath, diffFileName));
				string expectedMessageLong = "Strings are different." + "\nVisual diffs at: " + diffFileFullPath + "\nExpected: <" + expected + ">." + "\nActual: <" + actualFail + ">.";
				Assert.AreEqual(expectedMessageLong.Replace("\r", string.Empty), e.Message.Replace("\r", string.Empty));
				Assert.AreEqual(htmlDiffs.Replace("&nbsp;", " "), FileUtil.FileRead(FileUtil.GetPath(tempReportPath, diffFileName)));
			}
		}

		[Test]
		public virtual void TestAutogeneratedFileSequence()
		{
			string tempReportPath = FileUtil.GetPath(defaultFolder, "tmp-" + JavaCs.GetUniqueId());
			FileUtil.CreateDirectory(tempReportPath);
			VisualAssert va = new VisualAssert().SetReportSubdir(tempReportPath);
			int initialSequence = JavaCs.GetCurrentSequence();
			//Sequence number increments in succesive asserts with same va
			RunFailSilently(va);
			Assert.IsTrue(FileUtil.FileRead(FileUtil.GetPath(tempReportPath, "diff-" + (initialSequence) + ".html")).Length > 0);
			RunFailSilently(va);
			Assert.IsTrue(FileUtil.FileRead(FileUtil.GetPath(tempReportPath, "diff-" + (initialSequence + 1) + ".html")).Length > 0);
			//a different va, sequence continues
			va = new VisualAssert().SetReportSubdir(tempReportPath);
			RunFailSilently(va);
			Assert.IsTrue(FileUtil.FileRead(FileUtil.GetPath(tempReportPath, "diff-" + (initialSequence + 2) + ".html")).Length > 0);
		}
		private void RunFailSilently(VisualAssert va)
		{
			try
			{
				va.AssertEquals("abc", "def");
			}
			catch (Exception)
			{
				//no action, but a file was generated
			}
		}
	}
}
