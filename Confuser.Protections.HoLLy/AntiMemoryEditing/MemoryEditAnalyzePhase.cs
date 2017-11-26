using System;
using Confuser.Core;
using dnlib.DotNet;

namespace Confuser.Protections.HoLLy.AntiMemoryEditing
{
    public class MemoryEditAnalyzePhase : ProtectionPhase
    {
        public override ProtectionTargets Targets => ProtectionTargets.Fields;
        public override string Name => "Memory Protection analysis";

        public MemoryEditAnalyzePhase(ConfuserComponent parent) : base(parent) { }

        protected override void Execute(ConfuserContext context, ProtectionParameters parameters)
        {
            var service = context.Registry.GetService<IMemoryEditService>();

            foreach (IDnlibDef def in parameters.Targets.WithProgress(context.Logger)) {
                if (def is FieldDef d) {
                    service.AddToList(d);
                    context.Logger.DebugFormat("Added {0} to list", d.Name);
                }
                else {
                    context.Logger.WarnFormat("{0} (a {1}) was marked for memory protection, but this is not (yet) supported!", def, def.GetType());
                }
            }
        }
    }
}
