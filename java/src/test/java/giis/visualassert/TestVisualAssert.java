package giis.visualassert;

import static org.junit.Assert.assertEquals;
import static org.junit.Assert.fail;

import java.util.List;

import org.junit.Test;

import giis.portable.FileUtil;
import giis.portable.JavaCs;
import giis.visualassert.VisualAssert;

public class TestVisualAssert {
	/*
	 * What is being tested: no failure and failure with each choice of below
	 * conditions: 
	 * - additional message
	 * - existing report subdir
	 * - use default report subdir 
	 * - use relative path 
	 * - do not show expected and actual 
	 * - specified dif filename
	 */
	String defaultFolder = "target";
	String diffFile = "VisualAssertDiffFile.html";
	String expected = "abc def ghi\nmno pqr stu";
	String actualNofail = "abc def ghi\nmno pqr stu";
	String actualFail = "abc DEF ghi\nother line\nmno pqr stu";
	String expectedMessageShort = "Strings are different." + "\nThis is the additional message."
			+ "\nVisual diffs at: <a href=\"VisualAssertDiffFile.html\">VisualAssertDiffFile.html</a>";
	String htmlDiffs = "<span>abc </span><del style=\"background:#ffe6e6;\">def</del><ins style=\"background:#e6ffe6;\">DEF</ins><span> ghi&para;"
			+ "<br></span><ins style=\"background:#e6ffe6;\">other line&para;" + "<br></ins><span>mno pqr stu</span>";

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
			assertEquals(expectedMessageShort, e.getMessage());
			assertEquals(htmlDiffs, FileUtil.fileRead(FileUtil.getPath(defaultFolder, diffFile)));
		}
	}

	@Test
	public void FailAllConditionsFalse() {
		String tempReportPath = FileUtil.getPath(defaultFolder, "tmp-"+JavaCs.getUniqueId()); //folder does not exist
		VisualAssert va = new VisualAssert()
				.setShowExpectedAndActual(true)
				.setUseLocalAbsolutePath(true)
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
					+ "\nVisual diffs at: <a href=\"" + diffFileFullPath + "\">" + diffFileName + "</a>" 
					+ "\nExpected: <" + expected + ">." 
					+ "\nActual: <" + actualFail + ">.";
			assertEquals(expectedMessageLong.replace("\r", ""), e.getMessage().replace("\r", ""));
			assertEquals(htmlDiffs, FileUtil.fileRead(FileUtil.getPath(tempReportPath, diffFileName)));
		}
	}

}
