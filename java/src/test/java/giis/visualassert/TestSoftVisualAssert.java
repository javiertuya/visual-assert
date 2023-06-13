package giis.visualassert;

import static org.junit.Assert.assertEquals;
import static org.junit.Assert.fail;

import org.junit.Test;

import giis.visualassert.portable.FileUtil;
import giis.visualassert.portable.JavaCs;

public class TestSoftVisualAssert {
	//Actual execution and assertions in a method that will be reused to test with frameworks
	public static void doFailSoftAssert(SoftVisualAssert va, String expected, String reportPath,
			String aggregateFile) {
		va.assertEquals("ab zz cd", "ab cd", "", "f1.html");
		va.assertEquals("ab cd", "ab cd", "msg2");
		va.assertEquals("mn op", "mn op", "msg3");
		va.fail("msg4");
		va.assertEquals(null, "this is notnull", "msgen", "fen.html");
		va.assertEquals("this is notnull", null, "msgan", "fan.html");
		va.assertEquals("xy vw", "xy zz vw", "msg5");
		assertEquals(5, va.getFailureCount());
		try {
			if ("".equals(aggregateFile))
				va.assertAll();
			else
				va.assertAll(aggregateFile);
			fail("this should fail");
		} catch (AssertionError e) {
			//first transforms the file name in expected mesage to include the path
			expected=expected.replace("f1.html", FileUtil.getPath(reportPath, "f1.html"));
			expected=expected.replace("fen.html", FileUtil.getPath(reportPath, "fen.html"));
			expected=expected.replace("fan.html", FileUtil.getPath(reportPath, "fan.html"));
			expected=expected.replace("diff-0.html", FileUtil.getPath(reportPath, "diff-0.html"));
			expected=expected.replace("Aggregate.html", FileUtil.getPath(reportPath, "Aggregate.html"));
			assertEquals(expected, e.getMessage());
		}
		
		//assertAll resets the list
		assertEquals(0, va.getFailureCount());
		va.assertAll();
		//assertClear resets the list
		va.assertEquals("ab zz cd", "ab cd");
		assertEquals(1, va.getFailureCount());
		va.assertClear();
		assertEquals(0, va.getFailureCount());
		va.assertAll();
	}

	public static String expectedMessage(boolean aggregateDiffs) {
		return "There are 5 failed assertion(s)\n"
			+ (aggregateDiffs ?  "Aggregated visual diffs at: Aggregate.html\n" : "")
			+ "Failure 1: Strings are different.\n"
			+ "- Visual diffs at: f1.html\n"
			+ "- Call Stack:\n"
			+ "    at giis.visualassert.TestSoftVisualAssert.doFailSoftAssert(TestSoftVisualAssert.java:15)\n"
			+ "Failure 2: Fail assertion raised.\n"
			+ "msg4\n"
			+ "- Call Stack:\n"
			+ "    at giis.visualassert.TestSoftVisualAssert.doFailSoftAssert(TestSoftVisualAssert.java:18)\n"
			+ "Failure 3: Strings are different. Expected was <null>.\n"
			+ "msgen.\n"
			+ "- Visual diffs at: fen.html\n"
			+ "- Call Stack:\n"
			+ "    at giis.visualassert.TestSoftVisualAssert.doFailSoftAssert(TestSoftVisualAssert.java:19)\n"
			+ "Failure 4: Strings are different. Actual was <null>.\n"
			+ "msgan.\n"
			+ "- Visual diffs at: fan.html\n"
			+ "- Call Stack:\n"
			+ "    at giis.visualassert.TestSoftVisualAssert.doFailSoftAssert(TestSoftVisualAssert.java:20)\n"
			+ "Failure 5: Strings are different.\n"
			+ "msg5.\n"
			+ "- Visual diffs at: diff-0.html\n"
			+ "- Call Stack:\n"
			+ "    at giis.visualassert.TestSoftVisualAssert.doFailSoftAssert(TestSoftVisualAssert.java:21)";
	}
	
	@Test
	public void testSoftNoFail() {
		SoftVisualAssert va = new SoftVisualAssert();
		va.assertEquals("ab cd", "ab cd");
		va.assertEquals("xy vw", "xy vw");
		va.assertAll();
	}

	/*
	 * Conditions:
	 * -with/without failures
	 * -use fail
	 * -set/no set out file name
	 * -with/without additional message
	 * -reset because assertAll/assertClear
	 */
	@Test
	public void testSoftFail() {
		//uses a different path for files to avoid filename collisions with other tests
		String tempReportPath=FileUtil.getPath(JavaCs.DEFAULT_REPORT_SUBDIR, "tmp-"+JavaCs.getUniqueId());
		SoftVisualAssert va = new SoftVisualAssert()
				.setReportSubdir(tempReportPath).clearCurrentSequence();
		doFailSoftAssert(va, expectedMessage(false), tempReportPath, "");
	}
}
