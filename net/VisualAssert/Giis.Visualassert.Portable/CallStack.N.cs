using System;
using System.IO;
using System.Text.RegularExpressions;

namespace Giis.Visualassert.Portable
{
    /// <summary>
    /// Determines the call stack trace from the position of the method where this instance has been created
    /// </summary>
    public class CallStack
    {
        private readonly string[] stack;

        public CallStack()
        {
            stack = Environment.StackTrace.ToString().Split('\n');
        }

        public int Size()
        {
            return stack.Length;
        }
        //obtiene un elemento de la linea del stack entre dos textos dados
        private string GetStackBetween(int position, string from, string to)
        {
            int start = stack[position].IndexOf(from);
            if (start < 0) //no encontrado inicio (p.e. metodos del sistema al buscar linea, pues no se muestra
                return "";
            start = start + from.Length;
            int end = to==null ? stack[position].Length : stack[position].IndexOf(to, start);
            return stack[position].Substring(start, end - start).Trim();
        }
        public string GetClassName(int position)
        {
            //La clase esta unida al metodo, quito el metodo
            string[] items = GetStackBetween(position, " at ", "(").Split('.'); //despues vienen los parametros
            string clas = "";
            for (int i = 0; i < items.Length - 1; i++)
                clas += (i == 0 ? "" : ".") + items[i];
            return clas;
        }
        public string GetMethodName(int position)
        {
            //La clase esta unida al metodo, busco todo y luego saco el ultimo componente
            string[] items = GetStackBetween(position, " at ", "(").Split('.'); //despues vienen los parametros
            string method = items[items.Length - 1];
            return method;
        }
        public int GetLineNumber(int position)
        {
            string posStr = GetStackBetween(position, ":line", null);
            if (string.IsNullOrEmpty(posStr))
                return 0;
            return Int32.Parse(posStr);
        }
        public string GetFileName(int position)
        {
            string fullName=GetStackBetween(position, " in ", ":line").Replace("\\","/");
            string[] components = fullName.Split('/');
            if (components.Length > 0)
                return components[components.Length - 1];
            return "";
        }
        public string GetString()
        {
            StringWriter sw = new StringWriter();
            for (int i = 0; i < stack.Length; i++)
                sw.Write("\n        " + GetClassName(i) + " " + GetMethodName(i) + " " + GetLineNumber(i) + " " + GetFileName(i));
            return sw.ToString();
        }
        /// <summary>
        /// Normalizes a string representing a call stack by replacing line numbers by a hashtag,
        /// removing the language extension (java and cs) and replacing windows path separators
        /// </summary>
        public static string Normalize(string stack)
        {
            return Regex.Replace(stack, "\\:\\d+\\)", ":##)")
                .Replace(".java:", ":").Replace(".cs:", ":").Replace("\\", "/");
        }

    }
}
