using dnlib.DotNet;

namespace Confuser.Protections.HoLLy.Runtime.FakeObfuscator
{
    internal class SmartAssembly
    {
        public static TypeDefUser GetAttributes() => new TypeDefUser("SmartAssembly.Attributes", "PoweredByAttribute");
    }
}
