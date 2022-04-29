package giis.visualassert;

import static org.junit.Assert.assertEquals;
import static org.junit.Assert.assertTrue;
import static org.junit.Assert.fail;

import java.util.List;

import org.junit.Test;

import giis.visualassert.portable.FileUtil;
import giis.visualassert.portable.JavaCs;

public class TestVisualAssert {
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
	String defaultFolder = JavaCs.DEFAULT_REPORT_SUBDIR;
	String diffFile = "VisualAssertDiffFile.html";
	String expected = "abc def ghi\nmno pqr s tu";
	String actualNofail = "abc def ghi\nmno pqr s tu";
	String actualFail = " abc DEF ghi\nother line\nmno pqr stu";
	String expectedMessageShort = "Strings are different." + "\nThis is the additional message."
			+ "\n- Visual diffs at: " + diffFile;
	String htmlDiffs = "<ins style=\"background:#e6ffe6;\">&nbsp;</ins>"
			+ "<span>abc </span><del style=\"background:#ffe6e6;\">def</del>"
			+ "<ins style=\"background:#e6ffe6;\">DEF</ins><span> ghi&para;"
			+ "<br></span><ins style=\"background:#e6ffe6;\">other&nbsp;line&para;" 
			+ "<br></ins><span>mno pqr s</span><del style=\"background:#ffe6e6;\">&nbsp;</del><span>tu</span>";

	@Test
	public void testNoFail() {
		VisualAssert va = new VisualAssert();
		va.assertEquals(expected, actualNofail);
	}

	@Test
	public void testFailAllConditionsTrue() {
		VisualAssert va = new VisualAssert(); // all config methods by default
		FileUtil.createDirectory(defaultFolder); // ensure folder exists
		try {
			va.assertEquals(expected, actualFail, "This is the additional message", diffFile);
			fail("this should fail");
		} catch (AssertionError e) {
			//first transforms the file name in expected mesage to include the path
			expectedMessageShort=expectedMessageShort.replace(diffFile, FileUtil.getPath(JavaCs.DEFAULT_REPORT_SUBDIR, diffFile));
			assertEquals(expectedMessageShort, e.getMessage());
			assertEquals(htmlDiffs, FileUtil.fileRead(FileUtil.getPath(defaultFolder, diffFile)));
		}
	}

	@Test
	public void testFailAllConditionsFalse() {
		String tempReportPath = FileUtil.getPath(defaultFolder, "tmp-"+JavaCs.getUniqueId()); //folder does not exist
		VisualAssert va = new VisualAssert()
				.clearCurrentSequence()
				.setShowExpectedAndActual(true)
				.setUseLocalAbsolutePath(true)
				.setSoftDifferences(true)
				.setReportSubdir(tempReportPath);
		try {
			va.assertEquals(expected, actualFail);
			fail("this should fail");
		} catch (AssertionError e) {
			//get file and path of the generated diff file (only file in the folder)
			List<String> allFiles = FileUtil.getFileListInDirectory(tempReportPath);
			assertEquals(1, allFiles.size()); // only a file has been created
			String diffFileName = allFiles.get(0);
			String diffFileFullPath = "file:///" + FileUtil.getFullPath(FileUtil.getPath(tempReportPath, diffFileName));

			String expectedMessageLong = "Strings are different." 
					+ "\n- Visual diffs at: " + diffFileFullPath 
					+ "\n- Expected: <" + expected + ">." 
					+ "\n- Actual: <" + actualFail + ">.";
			assertEquals(expectedMessageLong.replace("\r", ""), e.getMessage().replace("\r", ""));
			assertEquals(htmlDiffs.replace("&nbsp;", " "), FileUtil.fileRead(FileUtil.getPath(tempReportPath, diffFileName)));
		}
	}
	
	@Test
	public void testAutogeneratedFileSequence() {
		String tempReportPath=FileUtil.getPath(defaultFolder, "tmp-"+JavaCs.getUniqueId());
		FileUtil.createDirectory(tempReportPath);
		VisualAssert va=new VisualAssert().setReportSubdir(tempReportPath);
		int initialSequence=JavaCs.getCurrentSequence();
		//Sequence number increments in succesive asserts with same va
		runFailSilently(va);
		assertTrue(FileUtil.fileRead(FileUtil.getPath(tempReportPath, "diff-" + String.valueOf(initialSequence) + ".html")).length()>0);
		runFailSilently(va);
		assertTrue(FileUtil.fileRead(FileUtil.getPath(tempReportPath, "diff-" + String.valueOf(initialSequence+1) + ".html")).length()>0);
		//a different va, sequence continues
		va=new VisualAssert().setReportSubdir(tempReportPath);
		runFailSilently(va);
		assertTrue(FileUtil.fileRead(FileUtil.getPath(tempReportPath, "diff-" + String.valueOf(initialSequence+2) + ".html")).length()>0);
	}
	private void runFailSilently(VisualAssert va) {
		try {
			va.assertEquals("abc", "def");
		} catch (AssertionError e) {
			//no action, but a file was generated
		}
	}

}
