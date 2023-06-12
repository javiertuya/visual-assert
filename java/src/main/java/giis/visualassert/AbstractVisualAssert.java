package giis.visualassert;

import java.util.LinkedList;

import org.bitbucket.cowwoc.diffmatchpatch.DiffMatchPatch;

import giis.visualassert.portable.FileUtil;
import giis.visualassert.portable.JavaCs;

/**
 * Base class with common configuration and methods applicables to
 * `VisualAssert` and `SoftVisualAssert`
 */
public abstract class AbstractVisualAssert<T extends AbstractVisualAssert<T>> {
	private boolean useLocalAbsolutePath = false;
	private boolean showExpectedAndActual = false;
	private boolean softDifferences = false;
	private boolean brightColors = false;
	private String reportSubdir = JavaCs.DEFAULT_REPORT_SUBDIR;
	protected FrameworkAssert platformAssert = new FrameworkAssert(null); // no platform by default

	/**
	 * Sets the the test framework that will raise the assertion failures 
	 * (useful if  you want also see the diffs from your development environment)
	 * @param framework One of JUnit 3, 4 and 5
	 * @return this object to allow fluent style
	 */
	@SuppressWarnings("unchecked")
	public T setFramework(Framework framework) {
		this.platformAssert = new FrameworkAssert(framework);
		return (T) this;
	}

	/**
	 * Sets the folder where the generated files with the differences are stored; if
	 * not set, files are stored by default in folder named 'target'
	 * @param reportSubdir the folder to store differences, relative to the current directory
	 * @return this object to allow fluent style
	 */
	@SuppressWarnings("unchecked")
	public T setReportSubdir(String reportSubdir) {
		this.reportSubdir = reportSubdir;
		return (T) this;
	}

	/**
	 * By default (hard), differences in whitespaces are rendered as whitespace html
	 * entities and therefore, always visible in the html ouput; if set to true
	 * (soft), some whitespace differences may be hidden from the html output
	 * @param useSoftDifferences sets soft differences
	 * @return this object to allow fluent style
	 */
	@SuppressWarnings("unchecked")
	public T setSoftDifferences(boolean useSoftDifferences) {
		this.softDifferences = useSoftDifferences;
		return (T) this;
	}

	/**
	 * By default differences are highlighted with pale red and green colors, if set
	 * to true the colors are brighter to easily locate small differences.
	 * @param useBrightColors sets brighter colors in diff files
	 * @return this object to allow fluent style
	 */
	@SuppressWarnings("unchecked")
	public T setBrightColors(boolean useBrightColors) {
		this.brightColors = useBrightColors;
		return (T) this;
	}

	/**
	 * If set to true, the link with the differences file will include a file url
	 * with the absolute path to the file; 
	 * useful when running tests from a development environment that allows links 
	 * in the messages (eg MS Visual Studio)
	 * @param useLocalAbsolutePath activates or deactivates this option
	 * @return this object to allow fluent style
	 */
	@SuppressWarnings("unchecked")
	public T setUseLocalAbsolutePath(boolean useLocalAbsolutePath) {
		this.useLocalAbsolutePath = useLocalAbsolutePath;
		return (T) this;
	}

	/**
	 * If set to true, the assert message will include the whole content of the
	 * expected and actual strings that are compared
	 * @param showExpectedAndActual activates or deactivates this option
	 * @return this object to allow fluent style
	 */
	@SuppressWarnings("unchecked")
	public T setShowExpectedAndActual(boolean showExpectedAndActual) {
		this.showExpectedAndActual = showExpectedAndActual;
		return (T) this;
	}

	/**
	 * Resets the sequence number used to identify diff files when name is not
	 * specified in the assert
	 */
	@SuppressWarnings("unchecked")
	T clearCurrentSequence() {
		JavaCs.clearCurrentSequence();
		return (T) this;
	}

	/**
	 * Asserts that two large strings are equal; if they are not, generates an html
	 * file highlighting the additions and deletions and includes a link to the html
	 * file in the assert message.
	 * @param expected the expected string
	 * @param actual   the value to compare against expected
	 */
	public void assertEquals(String expected, String actual) {
		assertEquals(expected, actual, "", "");
	}

	/**
	 * Asserts that two large strings are equal; if they are not, generates an html
	 * file highlighting the additions and deletions and includes a link to the html
	 * file in the assert message.
	 * @param expected the expected string
	 * @param actual   the value to compare against expected
	 */
	public void assertEquals(String expected, String actual, String message) {
		assertEquals(expected, actual, message, "");
	}

	/**
	 * Asserts that two large strings are equal; if they are not, generates an html
	 * file with the 'fileName' provided highlighting the additions and deletions
	 * and includes the specified 'message' and a link to the html file in the
	 * assert message.
	 * @param expected the expected string
	 * @param actual   the value to compare against expected
	 * @param message  additional message to be included
	 * @param fileName the name of the file with the differences, if empty, an
	 * autogenerated unique file name is used
	 */
	public abstract void assertEquals(String expected, String actual, String message, String fileName);

	/**
	 * Gets the assertion message to be included in the assertEquals statement
	 * (same parameters than AssertEquals)
	 */
	protected abstract String getAssertionMessage(String expected, String actual, String message, String fileName);

	// Common methods to obtain messages and diffs
	 
	protected String getAssertionMessage(String expected, String actual, String message, String fileName,
			String messagePrefix) {
		// Determina las diferencias en html usando diff match patch
		String htmlDiffs = getHtmlDiffs(expected, actual);

		// Asegura que existe la carpeta y guarda las diferencias en un html con nombre unico
		FileUtil.createDirectory(reportSubdir);
		String uniqueFileName = fileName;
		if (JavaCs.isEmpty(fileName))
			uniqueFileName = "diff-" + JavaCs.getSequenceAndIncrement() + ".html";
		String uniqueFile = FileUtil.getPath(reportSubdir, uniqueFileName);
		FileUtil.fileWrite(uniqueFile, htmlDiffs);

		// Compone el mensaje html
		String fullMessage = messagePrefix;
		if (!JavaCs.isEmpty(message))
			fullMessage += "\n" + message + ".";
		fullMessage += "\n- Visual diffs at: " + getFileUrl(uniqueFileName);
		// if using a framework, the expected/actual message is already always supplied by the platform
		if (platformAssert.getFramework() != Framework.NONE) {
			fullMessage += "\n";
		} else if (showExpectedAndActual) {
			fullMessage += "\n- Expected: <" + expected + ">.";
			fullMessage += "\n- Actual: <" + actual + ">.";
		}
		return fullMessage;
	}

	public String getHtmlDiffs(String expected, String actual) {
		DiffMatchPatch dmp = new DiffMatchPatch();
		LinkedList<DiffMatchPatch.Diff> diff = dmp.diffMain(expected, actual);
		dmp.diffCleanupSemantic((LinkedList<DiffMatchPatch.Diff>) diff);
		String diffs = this.softDifferences ? dmp.diffPrettyHtml(diff) : diffPrettyHtmlHard(diff);
		if (brightColors)
			diffs = diffs.replace("background:#e6ffe6;", "background:#00ff00;")
				.replace("background:#ffe6e6;", "background:#ff4000;");
		return diffs;
	}

	// customized method to display spaces as whtiespace entities
	protected String diffPrettyHtmlHard(LinkedList<DiffMatchPatch.Diff> diffs) {
		StringBuilder html = new StringBuilder();
		for (DiffMatchPatch.Diff aDiff : diffs) {
			String text = aDiff.text.replace("&", "&amp;")
					.replace("<", "&lt;").replace(">", "&gt;")
					.replace("\n", "&para;<br>");
			switch (aDiff.operation) {
			case INSERT:
				html.append("<ins style=\"background:#e6ffe6;\">").append(text.replace(" ", "&nbsp;")).append("</ins>");
				break;
			case DELETE:
				html.append("<del style=\"background:#ffe6e6;\">").append(text.replace(" ", "&nbsp;")).append("</del>");
				break;
			case EQUAL:
				html.append("<span>").append(text).append("</span>");
				break;
			}
		}
		return html.toString();
	}

	protected String getFileUrl(String uniqueFileName) {
		String fileUrl = FileUtil.getPath(reportSubdir, uniqueFileName);
		if (useLocalAbsolutePath) {
			String absPath = FileUtil.getFullPath(fileUrl).replace("\\", "/");
			fileUrl = "file://" + (absPath.startsWith("/") ? "" : "/") + absPath;
		}
		return fileUrl;
	}

}
