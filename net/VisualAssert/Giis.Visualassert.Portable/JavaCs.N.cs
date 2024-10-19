using System;
using System.Text.RegularExpressions;

namespace Giis.Visualassert.Portable
{
    /// <summary>
    ///  Utility methods for compatibility between Java and C#
    /// </summary>
	public static class JavaCs
	{
        //Default report folder, relative to the current solution directory
        //(provided that the project folder is under the solution)
        public const string DEFAULT_REPORT_SUBDIR = "../../../../reports";
        //to generate sequential identifiers
        public static int currentSequenceId = 0;
        public static object Lock = new object();

        public static bool IsJava()
        {
            return false;
        }

        public static bool IsEmpty(string str)
        {
            return string.IsNullOrWhiteSpace(str);
        }

        public static int GetSequenceAndIncrement()
        {
            lock (Lock)
            {
                return currentSequenceId++;
            }
        }
        public static int GetCurrentSequence()
        {
            lock(Lock)
            {
                return currentSequenceId;
            }
        }
        public static void ClearCurrentSequence()
        {
            lock (Lock)
            {
                currentSequenceId = 0;
            }
        }
        public static string GetEnvironmentVariable(string name)
        {
            return Environment.GetEnvironmentVariable(name);
        }
        public static string GetUniqueId()
        {
            return Guid.NewGuid().ToString();
        }
    }
}
