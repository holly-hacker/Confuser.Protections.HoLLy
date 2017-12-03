using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
#pragma warning disable 169
#pragma warning disable 219

namespace Confuser.Protections.HoLLy.Runtime.FakeObfuscator
{
    internal class CodeWall
    {
        public static Type[] GetTypes() => new[] { typeof(CodeWallStringDecrypter) };

        //requires 2 methods: 1 decrypter and any other (eg. cctor)
        internal static class CodeWallStringDecrypter
        {
            //required fields
            private static object _o;
            private static Dictionary<int, string> _d = new Dictionary<int, string>();

            private static string Decrypt(int x, int y, int z)
            {
                //required locals
                var l1 = default(int);
	            var l2 = default(byte[]);
	            var l3 = default(Assembly);
	            var l4 = default(Stream);
	            var l5 = default(Random);
	            var l6 = default(string);
	            var l7 = default(object);

                return l6;
            }
        }
    }
}
