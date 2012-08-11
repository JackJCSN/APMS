using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DataProvider
{
    /// <summary>
    /// 调试器
    /// </summary>
    public static class Debuger
    {
        public static String PrintExcetionW(Exception ex, int a)
        {
            StringBuilder s = new StringBuilder();
            int l = ex.Message.Length;
            if (l <= a)
            {
                return ex.Message;
            }
            else
            {
                l = l / a;
                for (int i = 1; i <= l; i++)
                {
                    s.Append(ex.Message.Substring((i - 1) * a, a) + "\r\n");
                }
                if (l * a < ex.Message.Length)
                {
                    s.Append(ex.Message.Substring((l) * a, ex.Message.Length - l * a));
                }
            }
            return s.ToString();
        }

        /// <summary>
        /// 向调试器输出异常信息
        /// <para>注意：仅在程序进行DEBUG编译时会输出这些异常，
        /// 否则程序将不输出任何有关此异常的调试信息</para>
        /// </summary>
        /// <param name="ex">要输出的异常</param>
        public static void PrintException(Exception ex)
        {
#if DEBUG
            Trace.WriteLineIf(ex != null, ex.ToString());
            Trace.WriteLineIf(ex != null, ex.StackTrace);
#endif
#if CONSOLEDEBUG
            Console.WriteLine(ex.ToString());
            Console.WriteLine(ex.StackTrace);
#endif
        }
    }
}
