using System;
using System.Linq;
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
		    foreach (var m in parameters.Targets.Cast<ModuleDef>())
		    {
		        //look for watermark and remove it
		        var attr = m.CustomAttributes.Find("ConfusedByAttribute");
		        if (attr != null) {
		            m.CustomAttributes.Remove(attr);
		            m.Types.Remove((TypeDef)attr.AttributeType);
		        }
            }
	    }
	}
}
