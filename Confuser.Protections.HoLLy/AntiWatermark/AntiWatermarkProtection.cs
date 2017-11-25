using System;
using Confuser.Core;

namespace Confuser.Protections.HoLLy.AntiWatermark
{
	public class AntiWatermarkProtection : Protection
	{
		public override string Name => "Anti Watermark";
		public override string Description => "Removes the ConfusedBy watermark to prevent ConfuserEx detection.";
		public override string Id => "anti watermark";
		public override string FullId => "HoLLy.AntiWatermark";
		public override ProtectionPreset Preset => ProtectionPreset.Minimum;

		protected override void Initialize(ConfuserContext context) { }

		protected override void PopulatePipeline(ProtectionPipeline pipeline)
		{
			//watermark is added in the inspection stage, this executes right after
			pipeline.InsertPostStage(PipelineStage.Inspection, new AntiWatermarkPhase(this));
		}
	}
}
