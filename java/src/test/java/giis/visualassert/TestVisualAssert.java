package giis.visualassert;

import static org.junit.Assert.assertEquals;
import static org.junit.Assert.assertThrows;
import static org.junit.Assert.assertTrue;

import java.util.List;

import org.junit.Test;

import giis.visualassert.portable.FileUtil;
import giis.visualassert.portable.JavaCs;

public class TestVisualAssert {
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
	static String defaultFolder = JavaCs.DEFAULT_REPORT_SUBDIR;
	static String diffFile = "VisualAssertDiffFile.html";
	static String expected = "abc def ghi\nmno pqr s tu";
	static String actualNofail = "abc def ghi\nmno pqr s tu";
	static String actualFail = "abc DEF ghi\nother line\nmno pqr stu";
	static String expectedMessageShort = "Strings are different. First diff at line 1 column 5." + "\nThis is the additional message."
			+ "\n- Visual diffs at: " + diffFile;
	static String htmlDiffs = ""
			+ "<pre>\n"
			+ "<span>abc </span><del style=\"background:#ffe6e6;\">def</del><ins style=\"background:#e6ffe6;\">DEF</ins><span> ghi&para;\n"
			+ "</span><ins style=\"background:#e6ffe6;\">other line&para;\n"
			+ "</ins><span>mno pqr s</span><del style=\"background:#ffe6e6;\"> </del><span>tu</span>\n"
			+ "</pre>";

	@Test
	public void testNoFail() {
		VisualAssert va = new VisualAssert();
		va.assertEquals(expected, actualNofail);
		va.assertEquals(null, null);
	}

	@Test
	public void testFailAllConditionsFalse() {
		VisualAssert va = new VisualAssert(); // all config methods by default
		doFailAllConditionsFalse(va, "java.lang.AssertionError", "", "");
	}
	//Actual execution and assertions in a method that will be reused to test with frameworks
	public static void doFailAllConditionsFalse(VisualAssert va, String assertionException, String assertMessage, String expActMessage) {
		FileUtil.createDirectory(defaultFolder); // ensure folder exists
		FileUtil.fileWrite(FileUtil.getPath(defaultFolder, diffFile), "");

		AssertionError e = assertThrows(AssertionError.class, () -> {
			va.assertEquals(expected, actualFail, "This is the additional message", diffFile);
		});
		assertEquals(assertionException, e.getClass().getName()); //not a subclass of this exception
		//first transforms the file name in expected message to include the path
		String message=expectedMessageShort.replace(diffFile, FileUtil.getPath(JavaCs.DEFAULT_REPORT_SUBDIR, diffFile));
		message+=expActMessage;
		assertEquals(message, e.getMessage());
		assertEquals(htmlDiffs, FileUtil.fileRead(FileUtil.getPath(defaultFolder, diffFile)));
	}

	@Test
	public void testFailAllConditionsTrue() {
		String tempReportPath = FileUtil.getPath(defaultFolder, "tmp-"+JavaCs.getUniqueId()); //folder does not exist
		VisualAssert va = new VisualAssert()
				.clearCurrentSequence()
				.setShowExpectedAndActual(true)
				.setUseLocalAbsolutePath(true)
				.setSoftDifferences(true)
				.setBrightColors(true)
				.setReportSubdir(tempReportPath);
		AssertionError e = assertThrows(AssertionError.class, () -> {
			va.assertEquals(expected, actualFail);
		});
		//get file and path of the generated diff file (only file in the folder)
		List<String> allFiles = FileUtil.getFileListInDirectory(tempReportPath);
		assertEquals(1, allFiles.size()); // only a file has been created
		String diffFileName = allFiles.get(0);
			
		String fullPath=FileUtil.getFullPath(FileUtil.getPath(tempReportPath, diffFileName));
		//on windows, back slash must be replaced by forward slash and full path start with slash
		if (fullPath.contains("\\"))
			fullPath="/" + fullPath.replace("\\", "/");
		String diffFileFullPath = "file://" + fullPath;
		
		String expectedMessageLong = "Strings are different. First diff at line 1 column 5." 
				+ "\n- Visual diffs at: " + diffFileFullPath 
				+ "\n- Expected: <" + expected + ">." 
				+ "\n- Actual: <" + actualFail + ">.";
		assertEquals(expectedMessageLong.replace("\r", ""), e.getMessage().replace("\r", ""));
		assertEquals(htmlDiffs
				.replace("<pre>\n", "").replace("\n</pre>", "")
				.replace("\n", "<br>")
				.replace("e6ffe6", "00ff00").replace("ffe6e6", "ff4000"),
				FileUtil.fileRead(FileUtil.getPath(tempReportPath, diffFileName)));
	}
	
	@Test
	public void testFailWithNulls() {
		VisualAssert va = new VisualAssert().clearCurrentSequence();
		doAssertNulls(va, "abc", null, "", "Strings are different. Actual was <null>.\n"
				+ "- Visual diffs at: target/diff-0.html", "null-actual.html");
		doAssertNulls(va, null, "def", "Custom message", "Strings are different. Expected was <null>.\nCustom message.\n"
				+ "- Visual diffs at: target/diff-1.html", "null-expected.html");
	}
	private void doAssertNulls(VisualAssert va, String expected, String actual, String message, String expectedMessage, String htmlFile) {
		AssertionError e = assertThrows(AssertionError.class, () -> {
			va.assertEquals(expected, actual, message);
		});
		assertEquals(expectedMessage, e.getMessage().replace("\\", "/"));
	}
	
	@Test
	public void testNormalizeLineEndings() {
		String linux = "line1\nline2\nline3\n";
		String windows = "line1\r\nline2\r\nline3\r\n";
		assertThrows(AssertionError.class, () -> {
			VisualAssert va = new VisualAssert();
			va.assertEquals(windows, linux, "Should fail without normalize eol", "va-normalize-eol.html");
		});
		
		VisualAssert va = new VisualAssert();
		va = new VisualAssert().setNormalizeEol(true);
		va.assertEquals(windows, linux, "Should not fail with normalize eol", "va-normalize-eol.html");
		va.assertEquals(linux, windows, "Should not fail with normalize eol", "va-normalize-eol.html");
		va.assertEquals(null, null, "Should not fail with nulls", "va-normalize-eol.html");
	}

	@Test
	public void testAutogeneratedFileSequence() {
		// Check that the diff files have been actually written
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
		assertThrows(AssertionError.class, () -> {
			va.assertEquals("abc", "def");
		});
		//no action, but a file was generated
	}

	@Test
	public void testQualifiedFileNames() {
		VisualAssert va = new VisualAssert();
		assertEquals("diff-" + JavaCs.getCurrentSequence() + ".html", va.getUniqueFileName());
		va = new VisualAssert().setDiffFileQualifier("xxx");
		assertEquals("diff-xxx-" + JavaCs.getCurrentSequence() + ".html", va.getUniqueFileName());
		
		// With environment variable, gets a valid variable for linux and windows
		String variable = "";
		String value = "";
		if (!JavaCs.isEmpty(JavaCs.getEnvironmentVariable("USER"))) {
			variable = "USER";
			value = JavaCs.getEnvironmentVariable("USER");
		} else if (!JavaCs.isEmpty(JavaCs.getEnvironmentVariable("USERNAME"))) {
			variable = "USERNAME";
			value = JavaCs.getEnvironmentVariable("USERNAME");
		}
		va = new VisualAssert().setDiffFileEnvQualifier(variable);
		assertEquals("diff-" + value + "-" + JavaCs.getCurrentSequence() + ".html", va.getUniqueFileName());
		
		// qualifiers are additive
		va = new VisualAssert().setDiffFileQualifier("yyy").setDiffFileEnvQualifier(variable);
		assertEquals("diff-yyy-" + value + "-" + JavaCs.getCurrentSequence() + ".html", va.getUniqueFileName());
	}
	
}
