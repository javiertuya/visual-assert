using System;
using System.Collections.Generic;
using System.Text;
using DiffMatchPatch;
using Giis.Visualassert.Portable;

namespace Giis.Visualassert
{
	/// <summary>
	/// Assertion methods that generate an html file with the differences highlighting the additions and deletions;
	/// useful for comparing large strings.
	/// </summary>
	public class VisualAssert
	{
		private bool useLocalAbsolutePath = false;
		private bool showExpectedAndActual = false;
		private bool softDifferences = false;
		private bool brightColors = false;
		private string reportSubdir = JavaCs.DefaultReportSubdir;

		/// <summary>Sets the folder where the generated files with the differences are stored; if not set, files are stored by default in folder named 'target'</summary>
		/// <param name="reportSubdir">the folder to store differences, relative to the current directory</param>
		/// <returns>this object to allow fluent style</returns>
		public virtual VisualAssert SetReportSubdir(string reportSubdir)
		{
			this.reportSubdir = reportSubdir;
			return this;
		}

		/// <summary>
		/// By default (hard), differences in whitespaces are rendered as whitespace html entities and therefore, always visible in the html ouput;
		/// if set to true (soft), some whitespace differences may be hidden from the html output
		/// </summary>
		/// <param name="useSoftDifferences">sets soft differences</param>
		/// <returns>this object to allow fluent style</returns>
		public VisualAssert SetSoftDifferences(bool useSoftDifferences)
		{
			this.softDifferences = useSoftDifferences;
			return this;
		}

		/// <summary>
		/// By default differences are highlighted with pale red and green colors,
		/// if set to true the colors are brighter to easily locate small differences.
		/// </summary>
		/// <param name="useBrightColors">sets brighter colors in diff files</param>
		/// <returns>this object to allow fluent style</returns>
		public VisualAssert SetBrightColors(bool useBrightColors)
		{
			this.brightColors = useBrightColors;
			return this;
		}

		/// <summary>
		/// If set to true, the link with the differences file will include an file url with the absolute path to the file;
		/// useful when running tests from a development environment that allows links in the messages (eg MS Visual Studio)
		/// </summary>
		/// <param name="useLocalAbsolutePath">activates or deactivates this option</param>
		/// <returns>this object to allow fluent style</returns>
		public virtual VisualAssert SetUseLocalAbsolutePath(bool useLocalAbsolutePath)
		{
			this.useLocalAbsolutePath = useLocalAbsolutePath;
			return this;
		}

		/// <summary>If set to true, the assert message will include the whole content of the exepcted and actual strings that are compared</summary>
		/// <param name="showExpectedAndActual">activates or deacctivates this option</param>
		/// <returns>this object to allow fluent style</returns>
		public virtual VisualAssert SetShowExpectedAndActual(bool showExpectedAndActual)
		{
			this.showExpectedAndActual = showExpectedAndActual;
			return this;
		}

		/// <summary>Resets the sequence number used to identify diff files when name is not specified in the assert</summary>
		public VisualAssert ClearCurrentSequence()
		{
			JavaCs.ClearCurrentSequence();
			return this;
		}

		/// <summary>
		/// Asserts that two large strings are equal;
		/// if they are not, generates an html file highlighting the additions and deletions
		/// and includes a link to the html file in the assert message.
		/// </summary>
		/// <param name="expected">the expected string</param>
		/// <param name="actual">the value to compare against expected</param>
		public virtual void AssertEquals(string expected, string actual)
		{
			AssertEquals(expected, actual, string.Empty, string.Empty);
		}

		/// <summary>
		/// Asserts that two large strings are equal;
		/// if they are not, generates an html file highlighting the additions and deletions
		/// and includes a link to the html file in the assert message.
		/// </summary>
		/// <param name="expected">the expected string</param>
		/// <param name="actual">the value to compare against expected</param>
		/// <param name="message">additional message to be included</param>
		public virtual void AssertEquals(string expected, string actual, string message)
		{
			AssertEquals(expected, actual, message, string.Empty);
		}

		/// <summary>
		/// Asserts that two large strings are equal;
		/// if they are not, generates an html file with the 'fileName' provided highlighting the additions and deletions
		/// and includes the specified 'message' and a link to the html file in the assert message.
		/// </summary>
		/// <param name="expected">the expected string</param>
		/// <param name="actual">the value to compare against expected</param>
		/// <param name="message">additional message to be included</param>
		/// <param name="fileName">the name of the file with the differences, if empty, an autogenerated unique file name is used</param>
		public virtual void AssertEquals(string expected, string actual, string message, string fileName)
		{
			if (!expected.Equals(actual))
			{
				ThrowAssertionError(GetAssertionMessage(expected, actual, message, fileName));
			}
		}
		protected virtual void ThrowAssertionError(string assertionMessage)
		{
			throw new Exception(assertionMessage);
		}

		protected virtual string GetAssertionMessage(string expected,string actual, string message, string fileName)
		{
			return GetAssertionMessage(expected, actual, message, fileName, "Strings are different.");
		}
		protected virtual string GetAssertionMessage(string expected, string actual, string message, string fileName, string messagePrefix)
		{
			//Determina las diferencias en html usando diff match patch
			string htmlDiffs = GetHtmlDiffs(expected, actual);
			//Asegura que existe la carpeta y guarda las diferencias en un html con nombre unico
			FileUtil.CreateDirectory(reportSubdir);
			String uniqueFileName = fileName;
			if (JavaCs.IsEmpty(fileName))
				uniqueFileName = "diff-" + JavaCs.GetSequenceAndIncrement() + ".html";
			string uniqueFile = FileUtil.GetPath(reportSubdir, uniqueFileName);
			FileUtil.FileWrite(uniqueFile, htmlDiffs);
			//Compone el mensaje html
			string fullMessage = messagePrefix;
			if (!JavaCs.IsEmpty(message))
			{
				fullMessage += "\n" + message + ".";
			}
			fullMessage += "\n- Visual diffs at: " + GetFileUrl(uniqueFileName);
			if (showExpectedAndActual)
			{
				fullMessage += "\n- Expected: <" + expected + ">.";
				fullMessage += "\n- Actual: <" + actual + ">.";
			}
			return fullMessage;
		}

		//Uses the original source code (C#) of Google diff match patch
		//https://github.com/google/diff-match-patch
		//Last commit on Jul 25, 2019: 62f2e68
		public string GetHtmlDiffs(string expected, string actual)
		{
			DiffMatchPatch.diff_match_patch dmp = new DiffMatchPatch.diff_match_patch();
			List<Diff> diff = dmp.diff_main(expected, actual);
			dmp.diff_cleanupSemantic(diff);
			string diffs = this.softDifferences ? dmp.diff_prettyHtml(diff) : DiffPrettyHtmlHard(diff);
			if (brightColors)
				diffs = diffs.Replace("background:#e6ffe6;", "background:#00ff00;")
						.Replace("background:#ffe6e6;", "background:#ff4000;");
			return diffs;
		}

		//customized method to display spaces as whtiespace entities
		protected string DiffPrettyHtmlHard(List<DiffMatchPatch.Diff> diffs)
		{
			StringBuilder html = new StringBuilder();
			foreach (DiffMatchPatch.Diff aDiff in diffs)
			{
				string text = aDiff.text.Replace("&", "&amp;").Replace("<", "&lt;")
						.Replace(">", "&gt;").Replace("\n", "&para;<br>");
				switch (aDiff.operation)
				{
					case Operation.INSERT:
						html.Append("<ins style=\"background:#e6ffe6;\">").Append(text.Replace(" ", "&nbsp;"))
						.Append("</ins>");
						break;
					case Operation.DELETE:
						html.Append("<del style=\"background:#ffe6e6;\">").Append(text.Replace(" ", "&nbsp;"))
						.Append("</del>");
						break;
					case Operation.EQUAL:
						html.Append("<span>").Append(text).Append("</span>");
						break;
				}
			}
			return html.ToString();
		}

		protected string GetFileUrl(string uniqueFileName)
		{
			string fileUrl = FileUtil.GetPath(reportSubdir, uniqueFileName);
			if (useLocalAbsolutePath)
			{
                string absPath = FileUtil.GetFullPath(fileUrl).Replace("\\", "/");
                fileUrl = "file://" + (absPath.StartsWith("/") ? "" : "/") + absPath;
            }
            return fileUrl;
		}
	}
}
