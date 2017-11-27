using System;
using System.Globalization;

namespace Confuser.Protections.HoLLy.Runtime.FakeObfuscator
{
	internal class CodeFort
    {
        public static Type[] GetTypes() => new[] { typeof(CodeFortStringDecrypter) };

        //may not have fields
        internal static class CodeFortStringDecrypter
        {
            //static, has double 3992.0
            private static string Decrypt(string s) => 3992.0.ToString(CultureInfo.InvariantCulture);
        }
    }
}
