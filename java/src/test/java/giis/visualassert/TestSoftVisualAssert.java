package giis.visualassert;

import static org.junit.Assert.assertEquals;
import static org.junit.Assert.fail;

import org.junit.Test;

import giis.visualassert.portable.FileUtil;
import giis.visualassert.portable.JavaCs;

public class TestSoftVisualAssert {
	String expectedMessage="There are 3 failed assertion(s)\n"
			+ "Failure 1: Strings are different.\n"
			+ "- Visual diffs at: f1.html\n"
			+ "Failure 2: Fail assertion raised.\n"
			+ "msg4\n"
			+ "Failure 3: Strings are different.\n"
			+ "msg5.\n"
			+ "- Visual diffs at: diff-0.html";

	@Test
	public void testNoFail() {
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
	public void testFail() {
		//uses a different path for files to avoid filename collisions with other tests
		String tempReportPath=FileUtil.getPath(JavaCs.DEFAULT_REPORT_SUBDIR, "tmp-"+JavaCs.getUniqueId());
		SoftVisualAssert va = (SoftVisualAssert) new SoftVisualAssert().setCallStackLength(0)
				.setReportSubdir(tempReportPath).clearCurrentSequence();

		va.assertEquals("ab zz cd", "ab cd", "", "f1.html");
		va.assertEquals("ab cd", "ab cd", "msg2");
		va.assertEquals("mn op", "mn op", "msg3");
		va.fail("msg4");
		va.assertEquals("xy vw", "xy zz vw", "msg5");
		assertEquals(3, va.getFailureCount());
		try {
			va.assertAll();
			fail("this should fail");
		} catch (AssertionError e) {
			//first transforms the file name in expected mesage to include the path
			expectedMessage=expectedMessage.replace("f1.html", FileUtil.getPath(tempReportPath, "f1.html"));
			expectedMessage=expectedMessage.replace("diff-0.html", FileUtil.getPath(tempReportPath, "diff-0.html"));
			assertEquals(expectedMessage, e.getMessage());
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

}
