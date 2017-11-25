using System;
using dnlib.DotNet;

namespace Confuser.Protections.HoLLy.FakeObuscator.Types
{
    internal static class AgileDotNet
    {
        public static TypeDefUser GetAttributes() => new TypeDefUser("SecureTeam.Attributes", "ObfuscatedByCliSecureAttribute");
    }
}