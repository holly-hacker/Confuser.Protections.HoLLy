using System;
using System.Linq;
using Confuser.Core;
using Confuser.Core.Helpers;
using Confuser.Protections.HoLLy.Runtime.FakeObfuscator;
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
            //inject types
            foreach (ModuleDef m in parameters.Targets.OfType<ModuleDef>())
            {
                InjectType(m, context.Logger, AgileDotNet.GetAttributes());     //+10
                InjectType(m, context.Logger, BabelDotNet.GetAttributes());     //+10
                InjectType(m, context.Logger, CryptoObfuscator.GetAttributes()); //+10
                InjectType(m, context.Logger, Dotfuscator.GetAttributes());     //+10
                InjectType(m, context.Logger, GoliathDotNet.GetAttributes());   //+10
                InjectType(m, context.Logger, SpicesDotNet.GetAttributes());    //+10
                InjectType(m, context.Logger, Xenocode.GetAttributes());        //+10
                InjectType(m, context.Logger, SmartAssembly.GetAttributes());   //+10
                
                //in case unknown obfuscator is forced
                InjectType(m, context.Logger, new TypeDefUser("YanoAttribute"));
            }
        }

        private static void InjectType(ModuleDef m, Core.ILogger l, params TypeDefUser[] types)
        {
            foreach (TypeDefUser type in types)
            {
                m.Types.Add(type);
                l.Debug("Added attribute " + type);

                InjectHelper.Inject(type, type, m);
            }
        }
    }
}
