package giis.visualassert.portable;

import java.util.UUID;

/**
 * Utility methods for compatibility between Java and C#
 */
public class JavaCs {
	//Default report folder, relative to the current directory
	public static final String DEFAULT_REPORT_SUBDIR="target";
	//to generate sequential identifiers
	public static int currentSequenceId=0;
	
	private JavaCs() {
	    throw new IllegalAccessError("Utility class");
	}
	public static boolean isEmpty(String str) {
		return str==null || "".equals(str.trim());
	}
	public static synchronized int getSequenceAndIncrement() {
		return currentSequenceId++;
	}
	public static synchronized int getCurrentSequence() {
		return currentSequenceId;
	}
	public static synchronized void clearCurrentSequence() {
		currentSequenceId=0;
	}
	public static String getEnvironmentVariable(String name) {
		return System.getenv(name); // NOSONAR
	}
	public static String getUniqueId() {
		return UUID.randomUUID().toString();
	}
}
