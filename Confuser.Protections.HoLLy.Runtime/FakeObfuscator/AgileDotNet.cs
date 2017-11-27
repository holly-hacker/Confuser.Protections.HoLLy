using dnlib.DotNet;

namespace Confuser.Protections.HoLLy.Runtime.FakeObfuscator
{
    internal static class AgileDotNet
    {
        public static TypeDefUser GetAttributes() => new TypeDefUser("SecureTeam.Attributes", "ObfuscatedByCliSecureAttribute");
    }
}