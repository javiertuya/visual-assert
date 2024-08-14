/////////////////////////////////////////////////////////////////////////////////////////////
/////// THIS FILE HAS BEEN AUTOMATICALLY CONVERTED FROM THE JAVA SOURCES. DO NOT EDIT ///////
/////////////////////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using Giis.Visualassert.Portable;
using NUnit.Framework;


namespace Giis.Visualassert
{
	public class TestVisualAssert
	{
		internal static string defaultFolder = JavaCs.DefaultReportSubdir;

		internal static string diffFile = "VisualAssertDiffFile.html";

		internal static string expected = "abc def ghi\nmno pqr s tu";

		internal static string actualNofail = "abc def ghi\nmno pqr s tu";

		internal static string actualFail = "abc DEF ghi\nother line\nmno pqr stu";

		internal static string expectedMessageShort = "Strings are different. First diff at line 1 column 5." + "\nThis is the additional message." + "\n- Visual diffs at: " + diffFile;

		internal static string htmlDiffs = string.Empty + "<span>abc </span><del style=\"background:#ffe6e6;\">def</del>" + "<ins style=\"background:#e6ffe6;\">DEF</ins><span> ghi&para;" + "<br></span><ins style=\"background:#e6ffe6;\">other&nbsp;line&para;" + "<br></ins><span>mno pqr s</span><del style=\"background:#ffe6e6;\">&nbsp;</del><span>tu</span>";

		/*
		* What is being tested: no failure and failure for every feature. Each choice of:
		* conditions on features:
		* - additional message
		* - existing report subdir
		* - use default report subdir
		* - use relative path
		* - do not show expected and actual
		* - soft differences
		* - bright colors
		* - specified dif filename
		* conditions on self generated file
		* - different instances
		* - different executions same instance
		* Framework usage is tested in separate class
		*/
		[Test]
		public virtual void TestNoFail()
		{
			VisualAssert va = new VisualAssert();
			va.AssertEquals(expected, actualNofail);
			va.AssertEquals(null, null);
		}

		[Test]
		public virtual void TestFailAllConditionsFalse()
		{
			VisualAssert va = new VisualAssert();
			// all config methods by default
			DoFailAllConditionsFalse(va, "System.Exception", string.Empty, string.Empty);
		}

		//Actual execution and assertions in a method that will be reused to test with frameworks
		public static void DoFailAllConditionsFalse(VisualAssert va, string assertionException, string assertMessage, string expActMessage)
		{
			FileUtil.CreateDirectory(defaultFolder);
			// ensure folder exists
			FileUtil.FileWrite(FileUtil.GetPath(defaultFolder, diffFile), string.Empty);
			// Always use this pattern with a boolean variable to check that assert exception is raised
			// to avoid problems with exception hierarchy across platforms and frameworks.
			bool success = false;
			try
			{
				va.AssertEquals(expected, actualFail, "This is the additional message", diffFile);
			}
			catch (Exception e)
			{
				NUnit.Framework.Legacy.ClassicAssert.AreEqual(assertionException, e.GetType().FullName);
				//not a subclass of this exception
				//first transforms the file name in expected message to include the path
				string message = expectedMessageShort.Replace(diffFile, FileUtil.GetPath(JavaCs.DefaultReportSubdir, diffFile));
				message += expActMessage;
				NUnit.Framework.Legacy.ClassicAssert.AreEqual(message, e.Message);
				NUnit.Framework.Legacy.ClassicAssert.AreEqual(htmlDiffs, FileUtil.FileRead(FileUtil.GetPath(defaultFolder, diffFile)));
				success = true;
			}
			NUnit.Framework.Legacy.ClassicAssert.IsTrue(success);
		}

		[Test]
		public virtual void TestFailAllConditionsTrue()
		{
			string tempReportPath = FileUtil.GetPath(defaultFolder, "tmp-" + JavaCs.GetUniqueId());
			//folder does not exist
			VisualAssert va = new VisualAssert().ClearCurrentSequence().SetShowExpectedAndActual(true).SetUseLocalAbsolutePath(true).SetSoftDifferences(true).SetBrightColors(true).SetReportSubdir(tempReportPath);
			bool success = false;
			try
			{
				va.AssertEquals(expected, actualFail);
			}
			catch (Exception e)
			{
				//get file and path of the generated diff file (only file in the folder)
				IList<string> allFiles = FileUtil.GetFileListInDirectory(tempReportPath);
				NUnit.Framework.Legacy.ClassicAssert.AreEqual(1, allFiles.Count);
				// only a file has been created
				string diffFileName = allFiles[0];
				string fullPath = FileUtil.GetFullPath(FileUtil.GetPath(tempReportPath, diffFileName));
				//on windows, back slash must be replaced by forward slash and full path start with slash
				if (fullPath.Contains("\\"))
				{
					fullPath = "/" + fullPath.Replace("\\", "/");
				}
				string diffFileFullPath = "file://" + fullPath;
				string expectedMessageLong = "Strings are different. First diff at line 1 column 5." + "\n- Visual diffs at: " + diffFileFullPath + "\n- Expected: <" + expected + ">." + "\n- Actual: <" + actualFail + ">.";
				NUnit.Framework.Legacy.ClassicAssert.AreEqual(expectedMessageLong.Replace("\r", string.Empty), e.Message.Replace("\r", string.Empty));
				NUnit.Framework.Legacy.ClassicAssert.AreEqual(htmlDiffs.Replace("&nbsp;", " ").Replace("e6ffe6", "00ff00").Replace("ffe6e6", "ff4000"), FileUtil.FileRead(FileUtil.GetPath(tempReportPath, diffFileName)));
				success = true;
			}
			NUnit.Framework.Legacy.ClassicAssert.IsTrue(success);
		}

		[Test]
		public virtual void TestFailWithNulls()
		{
			VisualAssert va = new VisualAssert().ClearCurrentSequence();
			DoAssertNulls(va, "abc", null, string.Empty, "Strings are different. Actual was <null>.\n" + "- Visual diffs at: ../../../../reports/diff-0.html", "null-actual.html");
			DoAssertNulls(va, null, "def", "Custom message", "Strings are different. Expected was <null>.\nCustom message.\n" + "- Visual diffs at: ../../../../reports/diff-1.html", "null-expected.html");
		}

		private void DoAssertNulls(VisualAssert va, string expected, string actual, string message, string expectedMessage, string htmlFile)
		{
			bool success = false;
			try
			{
				va.AssertEquals(expected, actual, message);
			}
			catch (Exception e)
			{
				NUnit.Framework.Legacy.ClassicAssert.AreEqual(expectedMessage, e.Message.Replace("\\", "/"));
				success = true;
			}
			NUnit.Framework.Legacy.ClassicAssert.IsTrue(success);
		}

		[Test]
		public virtual void TestNormalizeLineEndings()
		{
			string linux = "line1\nline2\nline3\n";
			string windows = "line1\r\nline2\r\nline3\r\n";
			VisualAssert va = new VisualAssert();
			bool success = false;
			try
			{
				va.AssertEquals(windows, linux, "Should fail without normalize eol", "va-normalize-eol.html");
			}
			catch (Exception)
			{
				success = true;
			}
			NUnit.Framework.Legacy.ClassicAssert.IsTrue(success);
			va = new VisualAssert().SetNormalizeEol(true);
			va.AssertEquals(windows, linux, "Should not fail with normalize eol", "va-normalize-eol.html");
			va.AssertEquals(linux, windows, "Should not fail with normalize eol", "va-normalize-eol.html");
			va.AssertEquals(null, null, "Should not fail with nulls", "va-normalize-eol.html");
		}

		[Test]
		public virtual void TestAutogeneratedFileSequence()
		{
			// Check that the diff files have been actually written
			string tempReportPath = FileUtil.GetPath(defaultFolder, "tmp-" + JavaCs.GetUniqueId());
			FileUtil.CreateDirectory(tempReportPath);
			VisualAssert va = new VisualAssert().SetReportSubdir(tempReportPath);
			int initialSequence = JavaCs.GetCurrentSequence();
			//Sequence number increments in succesive asserts with same va
			RunFailSilently(va);
			NUnit.Framework.Legacy.ClassicAssert.IsTrue(FileUtil.FileRead(FileUtil.GetPath(tempReportPath, "diff-" + initialSequence.ToString() + ".html")).Length > 0);
			RunFailSilently(va);
			NUnit.Framework.Legacy.ClassicAssert.IsTrue(FileUtil.FileRead(FileUtil.GetPath(tempReportPath, "diff-" + (initialSequence + 1).ToString() + ".html")).Length > 0);
			//a different va, sequence continues
			va = new VisualAssert().SetReportSubdir(tempReportPath);
			RunFailSilently(va);
			NUnit.Framework.Legacy.ClassicAssert.IsTrue(FileUtil.FileRead(FileUtil.GetPath(tempReportPath, "diff-" + (initialSequence + 2).ToString() + ".html")).Length > 0);
		}

		private void RunFailSilently(VisualAssert va)
		{
			try
			{
				va.AssertEquals("abc", "def");
			}
			catch (Exception)
			{
			}
		}

		//no action, but a file was generated
		[Test]
		public virtual void TestQualifiedFileNames()
		{
			VisualAssert va = new VisualAssert();
			NUnit.Framework.Legacy.ClassicAssert.AreEqual("diff-" + JavaCs.GetCurrentSequence() + ".html", va.GetUniqueFileName());
			va = new VisualAssert().SetDiffFileQualifier("xxx");
			NUnit.Framework.Legacy.ClassicAssert.AreEqual("diff-xxx-" + JavaCs.GetCurrentSequence() + ".html", va.GetUniqueFileName());
			// With environment variable, gets a valid variable for linux and windows
			string variable = string.Empty;
			string value = string.Empty;
			if (!JavaCs.IsEmpty(JavaCs.GetEnvironmentVariable("USER")))
			{
				variable = "USER";
				value = JavaCs.GetEnvironmentVariable("USER");
			}
			else
			{
				if (!JavaCs.IsEmpty(JavaCs.GetEnvironmentVariable("USERNAME")))
				{
					variable = "USERNAME";
					value = JavaCs.GetEnvironmentVariable("USERNAME");
				}
			}
			va = new VisualAssert().SetDiffFileEnvQualifier(variable);
			NUnit.Framework.Legacy.ClassicAssert.AreEqual("diff-" + value + "-" + JavaCs.GetCurrentSequence() + ".html", va.GetUniqueFileName());
			// qualifiers are additive
			va = new VisualAssert().SetDiffFileQualifier("yyy").SetDiffFileEnvQualifier(variable);
			NUnit.Framework.Legacy.ClassicAssert.AreEqual("diff-yyy-" + value + "-" + JavaCs.GetCurrentSequence() + ".html", va.GetUniqueFileName());
		}
	}
}
