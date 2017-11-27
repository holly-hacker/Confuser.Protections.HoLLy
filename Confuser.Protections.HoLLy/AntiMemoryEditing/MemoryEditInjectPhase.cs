using System;
using System.Linq;
using Confuser.Core;
using Confuser.Core.Helpers;
using Confuser.Protections.HoLLy.AntiMemoryEditing.Types;
using dnlib.DotNet;

namespace Confuser.Protections.HoLLy.AntiMemoryEditing
{
    class MemoryEditInjectPhase : ProtectionPhase
    {
        public override ProtectionTargets Targets => ProtectionTargets.Modules;
        public override string Name => "Memory obfuscation type injection";

        public MemoryEditInjectPhase(ConfuserComponent parent) : base(parent) { }

        protected override void Execute(ConfuserContext context, ProtectionParameters parameters)
        {
            var m = context.CurrentModule;

            var service = context.Registry.GetService<IMemoryEditService>();

            //import type
            var obfType = RuntimeHelper.GetType(typeof(ObfuscatedValue<>));
            var newType = new TypeDefUser("ConfuserEx.Protections.HoLLy.AntiMemoryEditing.Types", obfType.Name, new Importer(m).Import(typeof(object)));
            newType.GenericParameters.Add(new GenericParamUser(0, GenericParamAttributes.NonVariant, "T"));
            m.Types.Add(newType);
            InjectHelper.Inject(obfType, newType, m);
            service.SetWrapperType(m, newType);

            //find read/write methods
            var methods = newType.FindMethods("op_Implicit").ToArray();
            service.SetReadMethod(m, methods[0]);
            service.SetWriteMethod(m, methods[1]);

            //TODO: mark type for renaming
        }
    }
}
