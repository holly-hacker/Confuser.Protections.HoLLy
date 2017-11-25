using System;
using dnlib.DotNet;

namespace Confuser.Protections.HoLLy.FakeObuscator.Types
{
	internal class SpicesDotNet
	{
		public static TypeDef GetTypeDefs() => new TypeDefUser("NineRays.Obfuscator", "SoftwareWatermarkAttribute");
	}
}
