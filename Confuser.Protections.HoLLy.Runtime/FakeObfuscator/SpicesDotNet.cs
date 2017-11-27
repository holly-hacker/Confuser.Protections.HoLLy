using dnlib.DotNet;

namespace Confuser.Protections.HoLLy.Runtime.FakeObfuscator
{
	internal class SpicesDotNet
	{
		public static TypeDefUser GetAttributes() => new TypeDefUser("NineRays.Obfuscator", "SoftwareWatermarkAttribute");
	}
}
