package giis.visualassert;

import static org.junit.Assert.assertEquals;
import static org.junit.Assert.assertNull;

import org.junit.Test;

import giis.visualassert.portable.FileUtil;
import giis.visualassert.portable.JavaCs;

/**
 * SoftVisualAssert allows generating an aggregate file with all differences.
 * Testing combinations of using framework (produces a message) 
 * and specifying an aggregate file (produces a file)
 */
public class TestSoftAggregateDiffs {
	
	private static final String SRC_TEST_RESOURCES = "src/test/resources";
	private static final String AGGREGATE_HTML = "Aggregate.html";

	//Soft assert tests used junit4 as base, here using unit5
	//Each test uses a different path for files to avoid filename collisions with other tests
	//Reusing the general procedure to execute soft asserts at TestSoftVisualAssert
	@Test
	public void testJunit5FailureWithFramework() {
		String tempReportPath=FileUtil.getPath(JavaCs.DEFAULT_REPORT_SUBDIR, "tmp-"+JavaCs.getUniqueId());
		SoftVisualAssert va = new SoftVisualAssert()
				.setReportSubdir(tempReportPath).clearCurrentSequence()
				.setFramework(Framework.JUNIT5);
		TestSoftVisualAssert.doFailSoftAssert(va, TestSoftVisualAssert.expectedMessage(false) + getExpectedDiffsJUnit5(va),
				tempReportPath, "");
		//Check aggregate no generated
		String htmlDiffs = FileUtil.fileRead(FileUtil.getPath(tempReportPath, AGGREGATE_HTML), false); //null if does not exist
		assertNull("Aggregate file " + tempReportPath + "/" + AGGREGATE_HTML + " should not exist", htmlDiffs);
	}
	
	@Test
	public void testJunit5FailureWithFile() {
		String tempReportPath=FileUtil.getPath(JavaCs.DEFAULT_REPORT_SUBDIR, "tmp-"+JavaCs.getUniqueId());
		SoftVisualAssert va = new SoftVisualAssert()
				.setReportSubdir(tempReportPath).clearCurrentSequence();
		TestSoftVisualAssert.doFailSoftAssert(va, TestSoftVisualAssert.expectedMessage(true),
				tempReportPath, AGGREGATE_HTML);
		assertAggregateFile(tempReportPath);
	}
	
	@Test
	public void testJunit5FailureWithFileAndFramework() {
		String tempReportPath=FileUtil.getPath(JavaCs.DEFAULT_REPORT_SUBDIR, "tmp-"+JavaCs.getUniqueId());
		SoftVisualAssert va = new SoftVisualAssert()
				.setReportSubdir(tempReportPath).clearCurrentSequence()
				.setFramework(Framework.JUNIT5);
		TestSoftVisualAssert.doFailSoftAssert(va, TestSoftVisualAssert.expectedMessage(true) + getExpectedDiffsJUnit5(va),
				tempReportPath, AGGREGATE_HTML);
		assertAggregateFile(tempReportPath);
	}
	private void assertAggregateFile(String tempReportPath) {
		assertEquals(FileUtil.fileRead(FileUtil.getPath(SRC_TEST_RESOURCES, AGGREGATE_HTML)), 
				FileUtil.fileRead(FileUtil.getPath(tempReportPath, AGGREGATE_HTML)));
	}
	
	private String getExpectedDiffsJUnit5(SoftVisualAssert va) {
		return " ==> expected: <Aggregated failures:"
			+ "\n" + va.getAggregateFailureHeader(0)
			+ "\nab zz cd"
			+ "\n" + va.getAggregateFailureHeader(1)
			+ "\n"
			+ "\n" + va.getAggregateFailureHeader(2)
			+ "\n"
			+ "\n" + va.getAggregateFailureHeader(3)
			+ "\nthis is notnull"
			+ "\n" + va.getAggregateFailureHeader(4)
			+ "\nxy vw> but was: <Aggregated failures:"
			+ "\n" + va.getAggregateFailureHeader(0)
			+ "\nab cd"
			+ "\n" + va.getAggregateFailureHeader(1)
			+ "\n"
			+ "\n" + va.getAggregateFailureHeader(2)
			+ "\nthis is notnull"
			+ "\n" + va.getAggregateFailureHeader(3)
			+ "\n"
			+ "\n" + va.getAggregateFailureHeader(4)
			+ "\nxy zz vw>";
	}

	//Full test with other frameworks (3 and 4), expected diffs are slightly different
	
	@Test
	public void testJunit4FailureWithFileAndFramework() {
		String tempReportPath=FileUtil.getPath(JavaCs.DEFAULT_REPORT_SUBDIR, "tmp-"+JavaCs.getUniqueId());
		SoftVisualAssert va = new SoftVisualAssert()
				.setReportSubdir(tempReportPath).clearCurrentSequence()
				.setFramework(Framework.JUNIT4);
		TestSoftVisualAssert.doFailSoftAssert(va, TestSoftVisualAssert.expectedMessage(true) + getExpectedDiffsJUnit34(va),
				tempReportPath, AGGREGATE_HTML);
		assertAggregateFile(tempReportPath);
	}
	@Test
	public void testJunit3FailureWithFileAndFramework() {
		String tempReportPath=FileUtil.getPath(JavaCs.DEFAULT_REPORT_SUBDIR, "tmp-"+JavaCs.getUniqueId());
		SoftVisualAssert va = new SoftVisualAssert()
				.setReportSubdir(tempReportPath).clearCurrentSequence()
				.setFramework(Framework.JUNIT3);
		TestSoftVisualAssert.doFailSoftAssert(va, TestSoftVisualAssert.expectedMessage(true) + getExpectedDiffsJUnit34(va),
				tempReportPath, AGGREGATE_HTML);
		assertAggregateFile(tempReportPath);
	}
	private String getExpectedDiffsJUnit34(SoftVisualAssert va) {
		return " expected:<...----------------"
			+ "\nab [zz cd"
			+ "\n" + va.getAggregateFailureHeader(1)
			+ "\n"
			+ "\n" + va.getAggregateFailureHeader(2)
			+ "\n"
			+ "\n" + va.getAggregateFailureHeader(3)
			+ "\nthis is notnull"
			+ "\n" + va.getAggregateFailureHeader(4)
			+ "\nxy] vw> but was:<...----------------"
			+ "\nab [cd"
			+ "\n" + va.getAggregateFailureHeader(1)
			+ "\n"
			+ "\n" + va.getAggregateFailureHeader(2)
			+ "\nthis is notnull"
			+ "\n" + va.getAggregateFailureHeader(3)
			+ "\n"
			+ "\n" + va.getAggregateFailureHeader(4)
			+ "\nxy zz] vw>";
	}
	
}
