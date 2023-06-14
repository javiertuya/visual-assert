package giis.visualassert;

import static org.junit.Assert.*;

import org.junit.Test;

public class TestDiffLocation {

	@Test
	public void testEqualSizes() {
		VisualAssert va = new VisualAssert();
		assertEquals("No diffs", va.getDiffLocation("a b\nc d", "a b\nc d"));
		assertEquals("First diff at line 1 column 1", va.getDiffLocation("X b\nc d", "a b\nc d"));
		assertEquals("First diff at line 1 column 3", va.getDiffLocation("a X\nc d", "a b\nc d"));
		assertEquals("First diff at line 2 column 1", va.getDiffLocation("a b\nX d", "a b\nc d"));
		assertEquals("First diff at line 2 column 3", va.getDiffLocation("a b\nc X", "a b\nc d"));
		// More than one diff (report first one)
		assertEquals("First diff at line 1 column 3", va.getDiffLocation("a X\nX d", "a b\nc d"));
	}

	@Test
	public void testDifferentSizesNoFail() {
		VisualAssert va = new VisualAssert();
		assertEquals("Expected is contained in actual", va.getDiffLocation("a b\nc d", "a b\nc d+"));
		assertEquals("Expected is contained in actual", va.getDiffLocation("a b\nc d", "a b\nc d\n"));
		assertEquals("Actual is contained in expected", va.getDiffLocation("a b\nc d+", "a b\nc d"));
		assertEquals("Actual is contained in expected", va.getDiffLocation("a b\nc d\n", "a b\nc d"));
	}

	@Test
	public void testDifferentSizesWithFail() {
		VisualAssert va = new VisualAssert();
		assertEquals("First diff at line 2 column 3", va.getDiffLocation("a b\nc X", "a b\nc d+"));
		assertEquals("First diff at line 2 column 3", va.getDiffLocation("a b\nc d+", "a b\nc X"));
	}
	
	@Test
	public void testEmptyFiles() {
		VisualAssert va = new VisualAssert();
		assertEquals("Expected is empty", va.getDiffLocation("", "a b\nc d"));
		assertEquals("Actual is empty", va.getDiffLocation("a b\nc d", ""));
		assertEquals("No diffs", va.getDiffLocation("", ""));
	}
	
	
}
