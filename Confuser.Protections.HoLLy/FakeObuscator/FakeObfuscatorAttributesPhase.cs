using System;
using System.Collections.Generic;
using System.Linq;
using Confuser.Core;
using Confuser.Core.Helpers;
using Confuser.Core.Services;
using Confuser.Protections.HoLLy.Runtime.FakeObfuscator;
using Confuser.Renamer;
using dnlib.DotNet;

namespace Confuser.Protections.HoLLy.FakeObuscator
{
    public class FakeObfuscatorAttributesPhase : ProtectionPhase
    {
        public override ProtectionTargets Targets => ProtectionTargets.Modules;
        public override string Name => "Fake obfuscator attribute addition";

        public FakeObfuscatorAttributesPhase(ConfuserComponent parent) : base(parent) { }
        
        protected override void Execute(ConfuserContext context, ProtectionParameters parameters)
        {
            var marker = context.Registry.GetService<IMarkerService>();
            var name = context.Registry.GetService<INameService>();
            var allAddedTypes = new List<IDnlibDef>();

            TypeDefUser[] attributesToAdd = {
                AgileDotNet.GetAttributes(),    //+10
                BabelDotNet.GetAttributes(),    //+10
                CryptoObfuscator.GetAttributes(), //+10
                Dotfuscator.GetAttributes(),    //+10
                GoliathDotNet.GetAttributes(),  //+10
                SpicesDotNet.GetAttributes(),   //+10
                Xenocode.GetAttributes(),       //+10
                SmartAssembly.GetAttributes(),  //+10
                new TypeDefUser("YanoAttribute")//for unknown obfuscator
            };

            foreach (var m in parameters.Targets.Cast<ModuleDef>())
            {
                //inject types
                foreach (TypeDefUser idk in attributesToAdd)
                    allAddedTypes.AddRange(InjectType(m, context.Logger, idk));

                //mark types to NOT be renamed
                foreach (IDnlibDef def in allAddedTypes)
                {
                    marker.Mark(def, Parent);
                    name.SetCanRename(def, false);
                }
            }
        }

        private static IEnumerable<IDnlibDef> InjectType(ModuleDef m, Core.ILogger l, params TypeDefUser[] types)
        {
            List<IDnlibDef> ret = new List<IDnlibDef>();

            foreach (TypeDefUser type in types)
            {
                m.Types.Add(type);
                l.Debug("Added attribute " + type);

                ret.AddRange(InjectHelper.Inject(type, type, m));
            }

            return ret;
        }
    }
}
