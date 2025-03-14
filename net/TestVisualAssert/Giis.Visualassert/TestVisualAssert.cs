using NUnit.Framework;
using Giis.Visualassert.Portable;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
/////// THIS FILE HAS BEEN AUTOMATICALLY CONVERTED FROM THE JAVA SOURCES. DO NOT EDIT ///////

namespace Giis.Visualassert
{
    public class TestVisualAssert
    {
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
        static string defaultFolder = JavaCs.DEFAULT_REPORT_SUBDIR;
        static string diffFile = "VisualAssertDiffFile.html";
        static string expected = "abc def ghi\nmno pqr s tu";
        static string actualNofail = "abc def ghi\nmno pqr s tu";
        static string actualFail = "abc DEF ghi\nother line\nmno pqr stu";
        static string expectedMessageShort = "Strings are different. First diff at line 1 column 5." + "\nThis is the additional message." + "\n- Visual diffs at: " + diffFile;
        static string htmlDiffs = "" + "<pre>\n" + "<span>abc </span><del style=\"background:#ffe6e6;\">def</del><ins style=\"background:#e6ffe6;\">DEF</ins><span> ghi&para;\n" + "</span><ins style=\"background:#e6ffe6;\">other line&para;\n" + "</ins><span>mno pqr s</span><del style=\"background:#ffe6e6;\"> </del><span>tu</span>\n" + "</pre>";
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
            VisualAssert va = new VisualAssert(); // all config methods by default
            DoFailAllConditionsFalse(va, "System.Exception", "", "");
        }

        //Actual execution and assertions in a method that will be reused to test with frameworks
        public static void DoFailAllConditionsFalse(VisualAssert va, string assertionException, string assertMessage, string expActMessage)
        {
            FileUtil.CreateDirectory(defaultFolder); // ensure folder exists
            FileUtil.FileWrite(FileUtil.GetPath(defaultFolder, diffFile), "");
            Exception e = NUnit.Framework.Assert.Throws(typeof(Exception), () =>
            {
                va.AssertEquals(expected, actualFail, "This is the additional message", diffFile);
            });
            NUnit.Framework.Legacy.ClassicAssert.AreEqual(assertionException, e.GetType().FullName); //not a subclass of this exception

            //first transforms the file name in expected message to include the path
            string message = expectedMessageShort.Replace(diffFile, FileUtil.GetPath(JavaCs.DEFAULT_REPORT_SUBDIR, diffFile));
            message += expActMessage;
            NUnit.Framework.Legacy.ClassicAssert.AreEqual(message, e.Message);
            NUnit.Framework.Legacy.ClassicAssert.AreEqual(htmlDiffs, FileUtil.FileRead(FileUtil.GetPath(defaultFolder, diffFile)));
        }

        [Test]
        public virtual void TestFailAllConditionsTrue()
        {
            string tempReportPath = FileUtil.GetPath(defaultFolder, "tmp-" + JavaCs.GetUniqueId()); //folder does not exist
            VisualAssert va = new VisualAssert().ClearCurrentSequence().SetShowExpectedAndActual(true).SetUseLocalAbsolutePath(true).SetSoftDifferences(true).SetBrightColors(true).SetReportSubdir(tempReportPath);
            Exception e = NUnit.Framework.Assert.Throws(typeof(Exception), () =>
            {
                va.AssertEquals(expected, actualFail);
            });

            //get file and path of the generated diff file (only file in the folder)
            IList<string> allFiles = FileUtil.GetFileListInDirectory(tempReportPath);
            NUnit.Framework.Legacy.ClassicAssert.AreEqual(1, allFiles.Count); // only a file has been created
            string diffFileName = allFiles[0];
            string fullPath = FileUtil.GetFullPath(FileUtil.GetPath(tempReportPath, diffFileName));

            //on windows, back slash must be replaced by forward slash and full path start with slash
            if (fullPath.Contains("\\"))
                fullPath = "/" + fullPath.Replace("\\", "/");
            string diffFileFullPath = "file://" + fullPath;
            string expectedMessageLong = "Strings are different. First diff at line 1 column 5." + "\n- Visual diffs at: " + diffFileFullPath + "\n- Expected: <" + expected + ">." + "\n- Actual: <" + actualFail + ">.";
            NUnit.Framework.Legacy.ClassicAssert.AreEqual(expectedMessageLong.Replace("\r", ""), e.Message.Replace("\r", ""));
            NUnit.Framework.Legacy.ClassicAssert.AreEqual(htmlDiffs.Replace("<pre>\n", "").Replace("\n</pre>", "").Replace("\n", "<br>").Replace("e6ffe6", "00ff00").Replace("ffe6e6", "ff4000"), FileUtil.FileRead(FileUtil.GetPath(tempReportPath, diffFileName)));
        }

        [Test]
        public virtual void TestFailWithNulls()
        {
            VisualAssert va = new VisualAssert().ClearCurrentSequence();
            DoAssertNulls(va, "abc", null, "", "Strings are different. Actual was <null>.\n" + "- Visual diffs at: ../../../../reports/diff-0.html", "null-actual.html");
            DoAssertNulls(va, null, "def", "Custom message", "Strings are different. Expected was <null>.\nCustom message.\n" + "- Visual diffs at: ../../../../reports/diff-1.html", "null-expected.html");
        }

        private void DoAssertNulls(VisualAssert va, string expected, string actual, string message, string expectedMessage, string htmlFile)
        {
            Exception e = NUnit.Framework.Assert.Throws(typeof(Exception), () =>
            {
                va.AssertEquals(expected, actual, message);
            });
            NUnit.Framework.Legacy.ClassicAssert.AreEqual(expectedMessage, e.Message.Replace("\\", "/"));
        }

        [Test]
        public virtual void TestNormalizeLineEndings()
        {
            string linux = "line1\nline2\nline3\n";
            string windows = "line1\r\nline2\r\nline3\r\n";
            NUnit.Framework.Assert.Throws(typeof(Exception), () =>
            {
                VisualAssert va = new VisualAssert();
                va.AssertEquals(windows, linux, "Should fail without normalize eol", "va-normalize-eol.html");
            });
            VisualAssert va = new VisualAssert();
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
            NUnit.Framework.Legacy.ClassicAssert.IsTrue(FileUtil.FileRead(FileUtil.GetPath(tempReportPath, "diff-" + (initialSequence) + ".html")).Length > 0);
            RunFailSilently(va);
            NUnit.Framework.Legacy.ClassicAssert.IsTrue(FileUtil.FileRead(FileUtil.GetPath(tempReportPath, "diff-" + (initialSequence + 1) + ".html")).Length > 0);

            //a different va, sequence continues
            va = new VisualAssert().SetReportSubdir(tempReportPath);
            RunFailSilently(va);
            NUnit.Framework.Legacy.ClassicAssert.IsTrue(FileUtil.FileRead(FileUtil.GetPath(tempReportPath, "diff-" + (initialSequence + 2) + ".html")).Length > 0);
        }

        private void RunFailSilently(VisualAssert va)
        {
            NUnit.Framework.Assert.Throws(typeof(Exception), () =>
            {
                va.AssertEquals("abc", "def");
            }); //no action, but a file was generated
        }

        [Test]
        public virtual void TestQualifiedFileNames()
        {
            VisualAssert va = new VisualAssert();
            NUnit.Framework.Legacy.ClassicAssert.AreEqual("diff-" + JavaCs.GetCurrentSequence() + ".html", va.GetUniqueFileName());
            va = new VisualAssert().SetDiffFileQualifier("xxx");
            NUnit.Framework.Legacy.ClassicAssert.AreEqual("diff-xxx-" + JavaCs.GetCurrentSequence() + ".html", va.GetUniqueFileName());

            // With environment variable, gets a valid variable for linux and windows
            string variable = "";
            string value = "";
            if (!JavaCs.IsEmpty(JavaCs.GetEnvironmentVariable("USER")))
            {
                variable = "USER";
                value = JavaCs.GetEnvironmentVariable("USER");
            }
            else if (!JavaCs.IsEmpty(JavaCs.GetEnvironmentVariable("USERNAME")))
            {
                variable = "USERNAME";
                value = JavaCs.GetEnvironmentVariable("USERNAME");
            }

            va = new VisualAssert().SetDiffFileEnvQualifier(variable);
            NUnit.Framework.Legacy.ClassicAssert.AreEqual("diff-" + value + "-" + JavaCs.GetCurrentSequence() + ".html", va.GetUniqueFileName());

            // qualifiers are additive
            va = new VisualAssert().SetDiffFileQualifier("yyy").SetDiffFileEnvQualifier(variable);
            NUnit.Framework.Legacy.ClassicAssert.AreEqual("diff-yyy-" + value + "-" + JavaCs.GetCurrentSequence() + ".html", va.GetUniqueFileName());
        }
    }
}