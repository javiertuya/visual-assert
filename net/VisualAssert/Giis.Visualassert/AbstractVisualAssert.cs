/////////////////////////////////////////////////////////////////////////////////////////////
/////// THIS FILE HAS BEEN AUTOMATICALLY CONVERTED FROM THE JAVA SOURCES. DO NOT EDIT ///////
/////////////////////////////////////////////////////////////////////////////////////////////
using System.Collections.Generic;
using System.Text;
using Giis.Visualassert.Portable;



namespace Giis.Visualassert
{
	/// <summary>
	/// Base class with common configuration and methods applicables to
	/// `VisualAssert` and `SoftVisualAssert`
	/// </summary>
	public abstract class AbstractVisualAssert<T>
		where T : AbstractVisualAssert<T>
	{
		private bool useLocalAbsolutePath = false;

		private bool showExpectedAndActual = false;

		private bool softDifferences = false;

		private bool brightColors = false;

		private string reportSubdir = JavaCs.DefaultReportSubdir;

		protected internal FrameworkAssert platformAssert = new FrameworkAssert(Framework.None);

		// no platform by default
		/// <summary>
		/// Sets the the test framework that will raise the assertion failures
		/// (useful if  you want also see the diffs from your development environment)
		/// </summary>
		/// <param name="framework">One of JUnit 3, 4 and 5</param>
		/// <returns>this object to allow fluent style</returns>
		public virtual T SetFramework(Framework framework)
		{
			this.platformAssert = new FrameworkAssert(framework);
			return (T)this;
		}

		/// <summary>
		/// Sets the folder where the generated files with the differences are stored; if
		/// not set, files are stored by default in folder named 'target'
		/// </summary>
		/// <param name="reportSubdir">the folder to store differences, relative to the current directory</param>
		/// <returns>this object to allow fluent style</returns>
		public virtual T SetReportSubdir(string reportSubdir)
		{
			this.reportSubdir = reportSubdir;
			return (T)this;
		}

		/// <summary>
		/// By default (hard), differences in whitespaces are rendered as whitespace html
		/// entities and therefore, always visible in the html ouput; if set to true
		/// (soft), some whitespace differences may be hidden from the html output
		/// </summary>
		/// <param name="useSoftDifferences">sets soft differences</param>
		/// <returns>this object to allow fluent style</returns>
		public virtual T SetSoftDifferences(bool useSoftDifferences)
		{
			this.softDifferences = useSoftDifferences;
			return (T)this;
		}

		/// <summary>
		/// By default differences are highlighted with pale red and green colors, if set
		/// to true the colors are brighter to easily locate small differences.
		/// </summary>
		/// <param name="useBrightColors">sets brighter colors in diff files</param>
		/// <returns>this object to allow fluent style</returns>
		public virtual T SetBrightColors(bool useBrightColors)
		{
			this.brightColors = useBrightColors;
			return (T)this;
		}

		/// <summary>
		/// If set to true, the link with the differences file will include a file url
		/// with the absolute path to the file;
		/// useful when running tests from a development environment that allows links
		/// in the messages (eg MS Visual Studio)
		/// </summary>
		/// <param name="useLocalAbsolutePath">activates or deactivates this option</param>
		/// <returns>this object to allow fluent style</returns>
		public virtual T SetUseLocalAbsolutePath(bool useLocalAbsolutePath)
		{
			this.useLocalAbsolutePath = useLocalAbsolutePath;
			return (T)this;
		}

		/// <summary>
		/// If set to true, the assert message will include the whole content of the
		/// expected and actual strings that are compared
		/// </summary>
		/// <param name="showExpectedAndActual">activates or deactivates this option</param>
		/// <returns>this object to allow fluent style</returns>
		public virtual T SetShowExpectedAndActual(bool showExpectedAndActual)
		{
			this.showExpectedAndActual = showExpectedAndActual;
			return (T)this;
		}

		/// <summary>
		/// Resets the sequence number used to identify diff files when name is not
		/// specified in the assert
		/// </summary>
		public virtual T ClearCurrentSequence()
		{
			JavaCs.ClearCurrentSequence();
			return (T)this;
		}

		/// <summary>
		/// Asserts that two large strings are equal; if they are not, generates an html
		/// file highlighting the additions and deletions and includes a link to the html
		/// file in the assert message.
		/// </summary>
		/// <param name="expected">the expected string</param>
		/// <param name="actual">the value to compare against expected</param>
		public virtual void AssertEquals(string expected, string actual)
		{
			AssertEquals(expected, actual, string.Empty, string.Empty);
		}

		/// <summary>
		/// Asserts that two large strings are equal; if they are not, generates an html
		/// file highlighting the additions and deletions and includes a link to the html
		/// file in the assert message.
		/// </summary>
		/// <param name="expected">the expected string</param>
		/// <param name="actual">the value to compare against expected</param>
		public virtual void AssertEquals(string expected, string actual, string message)
		{
			AssertEquals(expected, actual, message, string.Empty);
		}

		/// <summary>
		/// Asserts that two large strings are equal; if they are not, generates an html
		/// file with the 'fileName' provided highlighting the additions and deletions
		/// and includes the specified 'message' and a link to the html file in the
		/// assert message.
		/// </summary>
		/// <param name="expected">the expected string</param>
		/// <param name="actual">the value to compare against expected</param>
		/// <param name="message">additional message to be included</param>
		/// <param name="fileName">
		/// the name of the file with the differences, if empty, an
		/// autogenerated unique file name is used
		/// </param>
		public abstract void AssertEquals(string expected, string actual, string message, string fileName);

		/// <summary>
		/// Gets the assertion message to be included in the assertEquals statement
		/// (same parameters than AssertEquals)
		/// </summary>
		protected internal abstract string GetAssertionMessage(string expected, string actual, string message, string fileName);

		// Common methods to obtain messages and diffs
		// Always use this to check for equality with null handling
		protected internal virtual bool StringsAreEqual(string expected, string actual)
		{
			if (expected == null && actual == null)
			{
				return true;
			}
			return expected == null ? actual.Equals(expected) : expected.Equals(actual);
		}

		protected internal virtual string GetAssertionMessage(string expected, string actual, string message, string fileName, string messagePrefix)
		{
			// Determina las diferencias en html usando diff match patch
			string htmlDiffs = GetHtmlDiffs(expected, actual);
			string uniqueFileName = WriteDiffFile(htmlDiffs, fileName);
			// Compone el mensaje html
			string fullMessage = messagePrefix;
			fullMessage += actual == null ? " Actual was <null>." : string.Empty;
			fullMessage += expected == null ? " Expected was <null>." : string.Empty;
			if (!JavaCs.IsEmpty(message))
			{
				fullMessage += "\n" + message + ".";
			}
			fullMessage += "\n- Visual diffs at: " + GetFileUrl(uniqueFileName);
			// if using a framework, the expected/actual message is already always supplied by the platform
			if (platformAssert.GetFramework() != Framework.None)
			{
				fullMessage += "\n";
			}
			else
			{
				if (showExpectedAndActual)
				{
					fullMessage += "\n- Expected: <" + expected + ">.";
					fullMessage += "\n- Actual: <" + actual + ">.";
				}
			}
			return fullMessage;
		}

		protected internal virtual string WriteDiffFile(string htmlDiffs, string fileName)
		{
			FileUtil.CreateDirectory(reportSubdir);
			string uniqueFileName = fileName;
			if (JavaCs.IsEmpty(fileName))
			{
				uniqueFileName = "diff-" + JavaCs.GetSequenceAndIncrement() + ".html";
			}
			string uniqueFile = FileUtil.GetPath(reportSubdir, uniqueFileName);
			FileUtil.FileWrite(uniqueFile, htmlDiffs);
			return uniqueFileName;
		}

		public virtual string GetHtmlDiffs(string expected, string actual)
		{
			DiffMatchPatch.diff_match_patch dmp = new DiffMatchPatch.diff_match_patch();
			expected = expected == null ? string.Empty : expected;
			actual = actual == null ? string.Empty : actual;
			List<DiffMatchPatch.Diff> diff = dmp.diff_main(expected, actual);
			dmp.diff_cleanupSemantic((List<DiffMatchPatch.Diff>)diff);
			string diffs = this.softDifferences ? dmp.diff_prettyHtml(diff) : DiffPrettyHtmlHard(diff);
			if (brightColors)
			{
				diffs = diffs.Replace("background:#e6ffe6;", "background:#00ff00;").Replace("background:#ffe6e6;", "background:#ff4000;");
			}
			return diffs;
		}

		// customized method to display spaces as whtiespace entities
		protected internal virtual string DiffPrettyHtmlHard(List<DiffMatchPatch.Diff> diffs)
		{
			StringBuilder html = new StringBuilder();
			foreach (DiffMatchPatch.Diff aDiff in diffs)
			{
				string text = aDiff.text.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("\n", "&para;<br>");
				switch (aDiff.operation)
				{
					case DiffMatchPatch.Operation.INSERT:
					{
						html.Append("<ins style=\"background:#e6ffe6;\">").Append(text.Replace(" ", "&nbsp;")).Append("</ins>");
						break;
					}

					case DiffMatchPatch.Operation.DELETE:
					{
						html.Append("<del style=\"background:#ffe6e6;\">").Append(text.Replace(" ", "&nbsp;")).Append("</del>");
						break;
					}

					case DiffMatchPatch.Operation.EQUAL:
					{
						html.Append("<span>").Append(text).Append("</span>");
						break;
					}
				}
			}
			return html.ToString();
		}

		protected internal virtual string GetFileUrl(string uniqueFileName)
		{
			string fileUrl = FileUtil.GetPath(reportSubdir, uniqueFileName);
			if (useLocalAbsolutePath)
			{
				string absPath = FileUtil.GetFullPath(fileUrl).Replace("\\", "/");
				fileUrl = "file://" + (absPath.StartsWith("/") ? string.Empty : "/") + absPath;
			}
			return fileUrl;
		}
	}
}
