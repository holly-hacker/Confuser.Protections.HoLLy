using System;
using System.Reflection;
#pragma warning disable 169
#pragma warning disable 219

namespace Confuser.Protections.HoLLy.Runtime.FakeObfuscator
{
	internal class EazfuscatorDotNet
	{
		public static Type[] GetTypes() => new[] {typeof(EazfuscatorStringDecrypter)};

		internal class EazfuscatorStringDecrypter
		{
			//required fields
			private byte[] _b;
			private short _s;

			public static string Decrypter(int i)
			{
				//required locals
				var l1 = default(bool);
				var l2 = default(byte[]);
				var l3 = default(char[]);
				var l4 = default(short);
				var l5 = default(int);
				var l6 = default(Assembly);
				var l7 = default(string);

				//required instruction
				l6.GetManifestResourceStream(l7);

				return l7;
			}

			//nested enum, for CheckType
			public enum Enum { }
		}
	}
}
