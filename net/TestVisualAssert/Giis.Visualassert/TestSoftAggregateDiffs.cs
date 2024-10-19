/////////////////////////////////////////////////////////////////////////////////////////////
/////// THIS FILE HAS BEEN AUTOMATICALLY CONVERTED FROM THE JAVA SOURCES. DO NOT EDIT ///////
/////////////////////////////////////////////////////////////////////////////////////////////
using Giis.Visualassert.Portable;
using NUnit.Framework;


namespace Giis.Visualassert
{
	/// <summary>SoftVisualAssert allows generating an aggregate file with all differences.</summary>
	/// <remarks>
	/// SoftVisualAssert allows generating an aggregate file with all differences.
	/// Testing combinations of using framework (produces a message)
	/// and specifying an aggregate file (produces a file)
	/// </remarks>
	public class TestSoftAggregateDiffs
	{
		private const string SrcTestResources = "../../../../../java/src/test/resources";

		private const string AggregateHtml = "Aggregate.html";

		//Soft assert tests used junit4 as base, here using unit5
		//Each test uses a different path for files to avoid filename collisions with other tests
		//Reusing the general procedure to execute soft asserts at TestSoftVisualAssert
		[Test]
		[Ignore("")] public virtual void TestJunit5FailureWithFramework()
		{
			string tempReportPath = FileUtil.GetPath(JavaCs.DefaultReportSubdir, "tmp-" + JavaCs.GetUniqueId());
			SoftVisualAssert va = new SoftVisualAssert().SetReportSubdir(tempReportPath).ClearCurrentSequence().SetFramework(Giis.Visualassert.Framework.Junit5);
			TestSoftVisualAssert.DoFailSoftAssert(va, TestSoftVisualAssert.ExpectedMessage(false) + GetExpectedDiffsJUnit5(va), tempReportPath, string.Empty);
			//Check aggregate no generated
			string htmlDiffs = FileUtil.FileRead(FileUtil.GetPath(tempReportPath, AggregateHtml), false);
			//null if does not exist
			NUnit.Framework.Legacy.ClassicAssert.IsNull("Aggregate file " + tempReportPath + "/" + AggregateHtml + " should not exist", htmlDiffs);
		}

		[Test]
		public virtual void TestJunit5FailureWithFile()
		{
			string tempReportPath = FileUtil.GetPath(JavaCs.DefaultReportSubdir, "tmp-" + JavaCs.GetUniqueId());
			SoftVisualAssert va = new SoftVisualAssert().SetReportSubdir(tempReportPath).ClearCurrentSequence();
			TestSoftVisualAssert.DoFailSoftAssert(va, TestSoftVisualAssert.ExpectedMessage(true), tempReportPath, AggregateHtml);
			AssertAggregateFile(tempReportPath);
		}

		[Test]
		[Ignore("")] public virtual void TestJunit5FailureWithFileAndFramework()
		{
			string tempReportPath = FileUtil.GetPath(JavaCs.DefaultReportSubdir, "tmp-" + JavaCs.GetUniqueId());
			SoftVisualAssert va = new SoftVisualAssert().SetReportSubdir(tempReportPath).ClearCurrentSequence().SetFramework(Giis.Visualassert.Framework.Junit5);
			TestSoftVisualAssert.DoFailSoftAssert(va, TestSoftVisualAssert.ExpectedMessage(true) + GetExpectedDiffsJUnit5(va), tempReportPath, AggregateHtml);
			AssertAggregateFile(tempReportPath);
		}

		private void AssertAggregateFile(string tempReportPath)
		{
			NUnit.Framework.Legacy.ClassicAssert.AreEqual(FileUtil.FileRead(FileUtil.GetPath(SrcTestResources, AggregateHtml)).Replace("\r", string.Empty), FileUtil.FileRead(FileUtil.GetPath(tempReportPath, AggregateHtml)));
		}

		private string GetExpectedDiffsJUnit5(SoftVisualAssert va)
		{
			return " ==> expected: <Aggregated failures:" + "\n" + va.GetAggregateFailureHeader(0, string.Empty) + "\nab zz cd" + "\n" + va.GetAggregateFailureHeader(1, "msg4") + "\n" + "\n" + va.GetAggregateFailureHeader(2, "msgen") + "\n" + "\n" + va.GetAggregateFailureHeader(3, "msgan") + 
				"\nthis is notnull" + "\n" + va.GetAggregateFailureHeader(4, "msg5") + "\nxy vw> but was: <Aggregated failures:" + "\n" + va.GetAggregateFailureHeader(0, string.Empty) + "\nab cd" + "\n" + va.GetAggregateFailureHeader(1, "msg4") + "\nFail assertion raised.\nmsg4" + "\n" + va.GetAggregateFailureHeader
				(2, "msgen") + "\nthis is notnull" + "\n" + va.GetAggregateFailureHeader(3, "msgan") + "\n" + "\n" + va.GetAggregateFailureHeader(4, "msg5") + "\nxy zz vw>";
		}

		//Full test with other frameworks (3 and 4), expected diffs are slightly different
		[Test]
		[Ignore("")] public virtual void TestJunit4FailureWithFileAndFramework()
		{
			string tempReportPath = FileUtil.GetPath(JavaCs.DefaultReportSubdir, "tmp-" + JavaCs.GetUniqueId());
			SoftVisualAssert va = new SoftVisualAssert().SetReportSubdir(tempReportPath).ClearCurrentSequence().SetFramework(Giis.Visualassert.Framework.Junit4);
			TestSoftVisualAssert.DoFailSoftAssert(va, TestSoftVisualAssert.ExpectedMessage(true) + GetExpectedDiffsJUnit34(va), tempReportPath, AggregateHtml);
			AssertAggregateFile(tempReportPath);
		}

		[Test]
		[Ignore("")] public virtual void TestJunit3FailureWithFileAndFramework()
		{
			string tempReportPath = FileUtil.GetPath(JavaCs.DefaultReportSubdir, "tmp-" + JavaCs.GetUniqueId());
			SoftVisualAssert va = new SoftVisualAssert().SetReportSubdir(tempReportPath).ClearCurrentSequence().SetFramework(Giis.Visualassert.Framework.Junit3);
			TestSoftVisualAssert.DoFailSoftAssert(va, TestSoftVisualAssert.ExpectedMessage(true) + GetExpectedDiffsJUnit34(va), tempReportPath, AggregateHtml);
			AssertAggregateFile(tempReportPath);
		}

		private string GetExpectedDiffsJUnit34(SoftVisualAssert va)
		{
			return " expected:<...----------------" + "\nab [zz cd" + "\n" + va.GetAggregateFailureHeader(1, "msg4") + "\n" + "\n" + va.GetAggregateFailureHeader(2, "msgen") + "\n" + "\n" + va.GetAggregateFailureHeader(3, "msgan") + "\nthis is notnull" + "\n" + va.GetAggregateFailureHeader(4, 
				"msg5") + "\nxy] vw> but was:<...----------------" + "\nab [cd" + "\n" + va.GetAggregateFailureHeader(1, "msg4") + "\nFail assertion raised.\nmsg4" + "\n" + va.GetAggregateFailureHeader(2, "msgen") + "\nthis is notnull" + "\n" + va.GetAggregateFailureHeader(3, "msgan") + "\n" + "\n"
				 + va.GetAggregateFailureHeader(4, "msg5") + "\nxy zz] vw>";
		}
	}
}
