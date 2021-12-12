package giis.visualassert;

import java.util.LinkedList;

import org.bitbucket.cowwoc.diffmatchpatch.DiffMatchPatch;

import giis.portable.FileUtil;
import giis.portable.JavaCs;

/**
 * A set of assertion methods useful for comparing large strings 
 * that generates an html file with the differences highlighting the additions and deletions.
 */
public class VisualAssert {
	private boolean useLocalAbsolutePath=false;
	private boolean showExpectedAndActual=false;
	private String reportSubdir="target";

	/**
	 * Sets the folder where generated files with the differences are stored. If not set, files are stored by default in folder named 'target'
	 */
	public VisualAssert setReportSubdir(String reportSubdir) {
	    this.reportSubdir=reportSubdir;
	    return this;
	}
	/**
	 * If set to true, the link with the differences file will include an file url with the absolute path to the file.
	 * Useful when running tests from a development environment that allows links in the messages (e.g. MS Visual Studio)
	 */
	public VisualAssert setUseLocalAbsolutePath(boolean useLocalAbsolutePath) {
	    this.useLocalAbsolutePath=useLocalAbsolutePath;
	    return this;
	}
	/**
	 * If set to true, the assert message will include the whole content of the exepcted and actual strings that are compared
	 */
	public VisualAssert setShowExpectedAndActual(boolean showExpectedAndActual) {
	    this.showExpectedAndActual=showExpectedAndActual;
	    return this;
	}
	
	/**
	 * Asserts that two longs are equal. 
	 * If they are not, generates an html file highlighting the additions and deletions
	 * and includes a link to the html file in the assert message.
	 */
	public void assertEquals(String expected, String actual) {
		assertEquals(expected, actual, "", "");
	}
	/**
	 * Asserts that two longs are equal. 
	 * If they are not, generates an html file with the 'fileName' provided highlighting the additions and deletions
	 * and includes the specified 'message' and a link to the html file in the assert message.
	 */
	public void assertEquals(String expected, String actual, String message, String fileName) {
		if (!expected.equals(actual))
			throw new AssertionError(getAssertionMessage(expected, actual, message, fileName));
	}
	

	/**
	 * Obtiene el mensaje de comparacion de los dos strings que debe aparecer en un assert cuando falla,
	 * incluyendo un enlace a las diferencias de forma visual en caso de diferencias.
	 */
	private String getAssertionMessage(String expected, String actual, String message, String fileName) {
		//Determina las diferencias en html usando diff match patch
		String htmlDiffs = getHtmlDiffs(expected, actual);
		
		//Asegura que existe la carpeta y guarda las diferencias en un html con nombre unico
		FileUtil.createDirectory(reportSubdir);
		String uniqueFileName =JavaCs.isEmpty(fileName) ?  "diff-" + JavaCs.getUniqueId() + ".html" : fileName;
		String uniqueFile = FileUtil.getPath(reportSubdir, uniqueFileName);
		FileUtil.fileWrite(uniqueFile, htmlDiffs);

		//Compone el mensaje html
		String fullMessage = "Strings are different.";
		if (!JavaCs.isEmpty(message))
			fullMessage += "\n" + message + ".";
		fullMessage += "\nVisual diffs at: " + getHtmlMarkup(uniqueFileName);
		if (showExpectedAndActual) {
			fullMessage += "\nExpected: <" + expected + ">.";
			fullMessage += "\nActual: <" + actual + ">.";
		} 
		return fullMessage;
	}
	private String getHtmlDiffs(String expected, String actual) {
		DiffMatchPatch dmp = new DiffMatchPatch();
		LinkedList<DiffMatchPatch.Diff> diff = dmp.diffMain(expected, actual);
		dmp.diffCleanupSemantic((LinkedList<DiffMatchPatch.Diff>)diff);
		return dmp.diffPrettyHtml(diff);
	}
	
    /**
    * Obtiene markup html para insertar un link al archivo indicado.
    * En local es una direccion absoluta, en remoto es relativa (a donde esta el log)
    */
    private String  getHtmlMarkup(String uniqueFileName) {
    	String path=uniqueFileName;
    	if (useLocalAbsolutePath) {
    		path = FileUtil.getPath(reportSubdir, uniqueFileName);
    		path = "file:///" + FileUtil.getFullPath(path);
    	}
    	return "<a href=\"" + path + "\">" + uniqueFileName + "</a>";
    }

}
