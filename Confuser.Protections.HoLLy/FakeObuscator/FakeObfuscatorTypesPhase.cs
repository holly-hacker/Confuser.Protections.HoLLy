using System;
using System.Linq;
using Confuser.Core;
using Confuser.Core.Helpers;
using Confuser.Protections.HoLLy.Runtime.FakeObfuscator;
using dnlib.DotNet;

namespace Confuser.Protections.HoLLy.FakeObuscator
{
    public class FakeObfuscatorTypesPhase : ProtectionPhase
    {
        private const string DefaultNamespace = "Confuser.Protections.HoLLy.FakeObuscator";

        public override ProtectionTargets Targets => ProtectionTargets.Modules;
        public override string Name => "Fake obfuscator type addition";

        public FakeObfuscatorTypesPhase(ConfuserComponent parent) : base(parent) { }

        protected override void Execute(ConfuserContext context, ProtectionParameters parameters)
        {
			//inject types
            foreach (ModuleDef m in parameters.Targets.OfType<ModuleDef>()) {
                InjectType(m, context.Logger, BabelDotNet.GetTypes());		//+110
                InjectType(m, context.Logger, CodeFort.GetTypes());         //+100
                InjectType(m, context.Logger, CodeWall.GetTypes());         //+100
                InjectType(m, context.Logger, CryptoObfuscator.GetTypes()); //+120
	            InjectType(m, context.Logger, Dotfuscator.GetTypes());      //+100
	            InjectType(m, context.Logger, EazfuscatorDotNet.GetTypes()); //+100
                InjectType(m, context.Logger, GoliathDotNet.GetTypes());    //+100
	            InjectType(m, context.Logger, Xenocode.GetTypes());         //+100
            }

			//TODO: obfuscate names in DefaultNamespace
		}

		private static void InjectType(ModuleDef m, Core.ILogger l, params Type[] types)
	    {
		    foreach (TypeDef type in types.Select(RuntimeHelper.GetType)) {
			    var newType = new TypeDefUser(DefaultNamespace, type.Name);
			    m.Types.Add(newType);
			    l.Debug("Added type " + newType);

			    InjectHelper.Inject(type, newType, m);
		    }
		}
    }
}
