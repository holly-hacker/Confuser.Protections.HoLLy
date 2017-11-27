using System;
using System.Linq;
using Confuser.Core;
using Confuser.Core.Helpers;
using Confuser.Protections.HoLLy.AntiMemoryEditing.Types;
using dnlib.DotNet;
using dnlib.DotNet.Emit;

namespace Confuser.Protections.HoLLy.AntiMemoryEditing
{
    public class MemoryEditApplyPhase : ProtectionPhase
    {
        public override ProtectionTargets Targets => ProtectionTargets.Properties;
        public override string Name => "Apply memory protection";

        public MemoryEditApplyPhase(ConfuserComponent parent) : base(parent) { }

        protected override void Execute(ConfuserContext context, ProtectionParameters parameters)
        {
            //TODO: split type injection into other phase?

            var m = context.CurrentModule;

            //import type
            var obfType = RuntimeHelper.GetType(typeof(ObfuscatedValue<>));
            var newType = new TypeDefUser("ConfuserEx.Protections.HoLLy.AntiMemoryEditing.Types", obfType.Name, new Importer(m).Import(typeof(object)));
            newType.GenericParameters.Add(new GenericParamUser(0, GenericParamAttributes.NonVariant, "T"));
            m.Types.Add(newType);
            InjectHelper.Inject(obfType, newType, m);

            //find read/write methods
            var methods = newType.FindMethods("op_Implicit").ToArray();
            var methodRead = methods[0];
            var methodWrite = methods[1];

            var service = context.Registry.GetService<IMemoryEditService>();

            //wrap all fields in this type
            foreach (FieldDef field in service.GetFields()) {
                field.FieldType = new GenericInstSig((ClassOrValueTypeSig)newType.ToTypeSig(), field.FieldType.ToTypeDefOrRefSig());
            }

            //run through all methods, patching r/w
            context.Logger.Debug("Looping through all types to patch access to obfuscated values");
            foreach (MethodDef t in context.CurrentModule.GetTypes().SelectMany(a => a.Methods).WithProgress(context.Logger)) {
                if (!t.HasBody || !t.Body.HasInstructions) continue;

                for (int i = 0; i < t.Body.Instructions.Count; i++) {
                    Instruction instr = t.Body.Instructions[i];

                    if (instr.Operand == null) continue;
                    if (!(instr.Operand is FieldDef fd) || !(fd.FieldType is GenericInstSig sig)) continue;
                    if (!new SigComparer().Equals(sig.GenericType.TypeDef, newType)) continue;

                    //TODO: newobj may be required for structs, even though it is not supported
                    switch (instr.OpCode.Code) {
                        case Code.Ldfld:
                        case Code.Ldsfld: {
                            //loading field
                            //add a call after it
                            t.Body.Instructions.Insert(i + 1, new Instruction(OpCodes.Call, FindReadMethod(m, sig, methodRead)));
                            i++;
                            break;
                        }
                        case Code.Stfld:
                        case Code.Stsfld: {
                            //storing field
                            //add a call before it
                            t.Body.Instructions.Insert(i, new Instruction(OpCodes.Call, FindReadMethod(m, sig, methodWrite)));
                            i++;
                        }
                        break;
                    }
                }
            }
            
            //TODO: apply to locals (if applied to methods)
        }

        private static MemberRefUser FindReadMethod(ModuleDef m, GenericInstSig sig, IMethod method)
        {
            return new MemberRefUser(m, method.Name, method.MethodSig) { Class = new TypeSpecUser(sig) };
        }
    }
}
