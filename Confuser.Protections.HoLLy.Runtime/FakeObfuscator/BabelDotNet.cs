using System;
using System.Collections;
using System.IO;
using System.Reflection;
using dnlib.DotNet;

namespace Confuser.Protections.HoLLy.Runtime.FakeObfuscator
{
	internal class BabelDotNet
	{
		public static TypeDefUser GetAttributes() => new TypeDefUser("BabelAttribute");
		public static Type[] GetTypes() => new[] {typeof(BabelAssemblyResolver), typeof(BabelStringDecrypter) };

		//no events
		internal class BabelAssemblyResolver
		{
			//required fields
			private object _o;
			private int _i;
			private Hashtable _h;

			//static, 1 exception handler
			private static void Register()
			{
				try {
				    //ldftn OnAssemblyResolve
				    AppDomain.CurrentDomain.AssemblyResolve += OnAssemblyResolve;
                } catch (Exception) { }
			}

            //has to be in same type as register, 1 exception handler
			private static Assembly OnAssemblyResolve(object o, ResolveEventArgs e)
			{
			    try {
			        return null;
                } catch (Exception) {
			        return null;
			    }
			}

			private static void Decrypt(Stream str) { }
		}

        //no events, 2 or less nested types, 1 or less fields
        //going for babel.net 3.0-3.5
	    internal class BabelStringDecrypter
		{
			//static decrypter method
			private static string Decrypt(int i) => "";

			//no properties, no events
			private class NestedType
			{
				//2 properties to detect as 3.0-3.5
				private Hashtable _o1;
				private NestedType _o2;

				//requires a .ctor
				public NestedType() { }

				//non-static decrypter method
				private string Decrypt(int i) => "";
			}
	    }
	}
}
