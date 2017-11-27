using System;
using Confuser.Core;

namespace Confuser.Protections.HoLLy.FakeObuscator
{
    public class FakeObfuscatorProtection : Protection
    {
        public override string Name => "Fake Obfuscator Protection";
        public override string Description => "Confuses obfuscators like de4dot by adding types typical to other obfuscators.";
        public override string Id => "fake obfuscator";
        public override string FullId => "HoLLy.FakeObfuscator";
        public override ProtectionPreset Preset => ProtectionPreset.Normal;

        protected override void Initialize(ConfuserContext context) { }

        protected override void PopulatePipeline(ProtectionPipeline pipeline)
        {
            pipeline.InsertPostStage(PipelineStage.ProcessModule, new FakeObfuscatorTypesPhase(this));
            pipeline.InsertPostStage(PipelineStage.ProcessModule, new FakeObfuscatorAttributesPhase(this));
        }
    }
}
