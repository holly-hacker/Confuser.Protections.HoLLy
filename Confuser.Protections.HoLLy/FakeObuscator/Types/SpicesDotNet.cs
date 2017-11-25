using System;
using dnlib.DotNet;

namespace Confuser.Protections.HoLLy.FakeObuscator.Types
{
	internal class SpicesDotNet
	{
		public static TypeDefUser GetAttributes() => new TypeDefUser("NineRays.Obfuscator", "SoftwareWatermarkAttribute");
	}
}
