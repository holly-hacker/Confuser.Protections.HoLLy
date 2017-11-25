using System;
using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using dnlib.DotNet;

namespace Confuser.Protections.HoLLy.FakeObuscator.Types
{
	internal class CryptoObfuscator
    {
        public static TypeDef GetTypeDefs() => new TypeDefUser("CryptoObfuscator", "ProtectedWithCryptoObfuscatorAttribute");
        public static Type[] GetTypes() => new[] {
			typeof(CryptoObfuscatorMethodDecrypter),
			typeof(CryptoObfuscatorStringDecrypter),
			typeof(CryptoObfuscatorConstantsDecrypter) 
        };

        //1 nested type, 3 fields, has cctor
        internal static class CryptoObfuscatorMethodDecrypter
        {
            //required fields
            private static byte[] _b;
            private static Dictionary<int, int> _d = new Dictionary<int, int>();    //creates .cctor
            private static ModuleHandle _m;

            //has to be static
            public static void Decrypt(int x, int y, int z)
            {
	            var l1 = default(Delegate);
	            var l2 = default(ModuleHandle);
	            var l3 = default(DynamicILInfo);
	            var l4 = default(DynamicMethod);
	            var l5 = default(FieldInfo);
	            var l6 = default(FieldInfo[]);
	            var l7 = default(MethodBase);
	            var l8 = default(MethodBody);
	            var l9 = default(Type);
	            var l10 = default(Type[]);
            }

            private class Nested { }
        }

		//cannot be public, 1 static field (byte[]), 2 or 3 methods, no nested types
	    internal static class CryptoObfuscatorStringDecrypter
	    {
		    private static byte[] _b;

		    private static string Decrypt(int i) => "";

			private static void ExtraMethod() { }
	    }

        //7 methods, 1 or 2 fields, has a byte[] field
        internal static class CryptoObfuscatorConstantsDecrypter
        {
	        private static byte[] _b = new byte[0];	//creates .cctor

			//required methods
	        private static int RequiredMethod1(int a) => 0;
	        private static long RequiredMethod2(int a) => (long)0;
	        private static float RequiredMethod3(int a) => 0f;
	        private static double RequiredMethod4(int a) => 0.0;
	        private static void RequiredMethod5(Array arr, int a) { }

			//getting to 7 total methods
			private static void Extra() { }
        }
    }
}
