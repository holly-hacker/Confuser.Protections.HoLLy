using System;
using Confuser.Core;
using dnlib.DotNet;

namespace Confuser.Protections.HoLLy.AntiWatermark
{
	public class AntiWatermarkPhase : ProtectionPhase
	{
		public override ProtectionTargets Targets => ProtectionTargets.Modules;
		public override string Name => "ConfusedBy attribute removal";

		public AntiWatermarkPhase(ConfuserComponent parent) : base(parent) { }
		protected override void Execute(ConfuserContext context, ProtectionParameters parameters)
		{
			//look for watermark and remove it
		    foreach (var md in context.Modules) {
		        var attr = md.CustomAttributes.Find("ConfusedByAttribute");
		        if (attr != null) {
		            md.CustomAttributes.Remove(attr);
		            md.Types.Remove((TypeDef)attr.AttributeType);
		        }
		    }
		}
	}
}
