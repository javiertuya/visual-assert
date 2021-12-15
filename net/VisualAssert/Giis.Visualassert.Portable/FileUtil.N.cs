using System;
using System.Collections.Generic;
using System.IO;

namespace Giis.Visualassert.Portable
{
    /// <summary>
    /// Utility methods for file management for compatibilty between Java and C#
    /// </summary>
    public static class FileUtil
    {
        public static string FileRead(string fileName, bool throwIfNotExists)
        {
            if (File.Exists(fileName))
                return File.ReadAllText(fileName);
            if (throwIfNotExists)
                throw new Exception("File does not exist " + fileName);
            else
                return null;
        }
        public static string FileRead(string fileName)
        {
            return FileRead(fileName, true);
        }

        public static void FileWrite(string fileName, string contents)
        {
            File.WriteAllText(fileName, contents);
        }

        public static IList<string> GetFileListInDirectory(String path)
        {
            //En java se tiene solo nombre, pero .net devuelve path y nombre
            IList<string> fileNames = new List<String>();
            IList<string> filesPath = Directory.GetFiles(path);
            foreach (String filePath in filesPath)
                fileNames.Add(GetFileNameOnly(filePath));
            return fileNames;
        }

        public static string GetPath(params string[] path)
        {
            return Path.Combine(path);
        }

        public static string GetFullPath(string path)
        {
            return Path.GetFullPath(path);
        }

        public static void CreateDirectory(string filePath)
        {
            Directory.CreateDirectory(filePath);
        }

        private static string GetFileNameOnly(string fileWithPath)
        {
            //puee haber mezcla de separadores / \, busca el ultimo de ellos
            int first = -1; //si no se encuentram obtendra desde fist+1, es decir, desde cero
            for (int i = 0; i < fileWithPath.Length; i++)
                if (fileWithPath[i] == '/' || fileWithPath[i] == '\\')
                    first = i;
            return fileWithPath.Substring(first + 1, fileWithPath.Length - first - 1);
        }

    }
}
