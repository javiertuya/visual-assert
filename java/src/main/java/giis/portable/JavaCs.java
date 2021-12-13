package giis.portable;

import java.util.UUID;

/**
 * Utility methods for compatibility between Java and C#
 */
public class JavaCs {
	//Default report folder, relative to the current directory
	public static final String DEFAULT_REPORT_SUBDIR="target";
	
	private JavaCs() {
	    throw new IllegalAccessError("Utility class");
	}
	public static boolean isEmpty(String str) {
		return str==null || "".equals(str.trim());
	}
	public static String getUniqueId() {
		return UUID.randomUUID().toString();
	}
}
