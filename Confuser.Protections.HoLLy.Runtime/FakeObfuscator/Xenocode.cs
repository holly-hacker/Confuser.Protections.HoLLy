using System;
using dnlib.DotNet;

namespace Confuser.Protections.HoLLy.Runtime.FakeObfuscator
{
	internal class Xenocode
	{
		public static TypeDefUser GetAttributes() => new TypeDefUser("Xenocode.Client.Attributes.AssemblyAttributes", "ProcessedByXenocode");
	    public static Type[] GetTypes() => new[] {typeof(XenocodeStringDecrypter)};

        //no fields, 1, 2 or 3 methods, no properties, no events
	    internal class XenocodeStringDecrypter
	    {
            //has to contain int 1789
	        public string Decrypt(string x, int y) => 1789.ToString();
	    }
    }
}
