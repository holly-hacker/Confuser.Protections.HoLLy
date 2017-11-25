using System;
using dnlib.DotNet;

namespace Confuser.Protections.HoLLy.FakeObuscator.Types
{
    internal class SmartAssembly
    {
        public static TypeDef GetTypeDefs() => new TypeDefUser("SmartAssembly.Attributes", "PoweredByAttribute");
    }
}
