using System;
using dnlib.DotNet;

namespace Confuser.Protections.HoLLy.FakeObuscator.Types
{
    internal class SmartAssembly
    {
        public static TypeDefUser GetAttributes() => new TypeDefUser("SmartAssembly.Attributes", "PoweredByAttribute");
    }
}
