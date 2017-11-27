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
            //TODO: rewrite this entire thing because it is shit and doesn't work

            var m = context.CurrentModule;

            //import type
            var obfType = RuntimeHelper.GetType(typeof(ObfuscatedValue<>));
            var newType = new TypeDefUser("ConfuserEx.Protections.HoLLy.AntiMemoryEditing.Types", obfType.Name, new Importer(m).Import(typeof(object)));
            newType.GenericParameters.Add(new GenericParamUser(0, GenericParamAttributes.NonVariant, "T"));
            m.Types.Add(newType);
            InjectHelper.Inject(obfType, newType, m);
            

            var service = context.Registry.GetService<IMemoryEditService>();

            //wrap all fields in this type
            foreach (FieldDef field in service.GetFields()) {
                field.FieldType = new GenericInstSig((ClassOrValueTypeSig)newType.ToTypeSig(), field.FieldType.ToTypeDefOrRefSig());
            }

            //run through all methods, patching r/w
            context.Logger.Info("Looping through all types to patch access to obfuscated values");
            foreach (MethodDef t in context.CurrentModule.GetTypes().SelectMany(a => a.Methods).WithProgress(context.Logger)) {
                if (!t.HasBody || !t.Body.HasInstructions) continue;

                for (int i = 0; i < t.Body.Instructions.Count; i++) {
                    Instruction instr = t.Body.Instructions[i];
                    if (instr.Operand == null) continue;
                    switch (instr.OpCode.Code) {
                        case Code.Ldfld:
                        case Code.Ldsfld: {
                            //loading field
                            //add a call after it
                            if (!(instr.Operand is FieldDef fd) || !(fd.FieldType is GenericInstSig gis)) continue;

                            var td = gis.GenericType.TypeDef;
                            if (td.FullName != newType.FullName) continue;

                            context.Logger.Debug(gis.ToString());
                                
                            context.Logger.InfoFormat("Type (L): {0}", td);
                            var method = td.FindMethods("op_Implicit").ToArray()[0];
                            context.Logger.InfoFormat("Method: {0}", method);

                            var mref = new MemberRefUser(m, "op_Implicit", method.MethodSig);
                            context.Logger.InfoFormat("MemberRef: {0}", mref);

                            mref.Class = new TypeSpecUser(gis);
                            context.Logger.InfoFormat("Class: {0}", mref.Class);

                            t.Body.Instructions.Insert(i + 1, new Instruction(OpCodes.Call, mref));
                            i++;
                            break;
                        }
                        case Code.Stfld:
                        case Code.Stsfld: {
                            //loading field
                            //add a call after it
                            if (!(instr.Operand is FieldDef fd) || !(fd.FieldType is GenericInstSig gis)) continue;

                            var td = gis.GenericType.TypeDef;
                            if (td.FullName != newType.FullName) continue;

                            var method = td.FindMethods("op_Implicit").ToArray()[1];
                            var mref = new MemberRefUser(m, "op_Implicit", method.MethodSig);
                            mref.Class = new TypeSpecUser(gis);

                            t.Body.Instructions.Insert(i, new Instruction(OpCodes.Call, mref));
                            i++;
                        }
                        break;
                    }
                }
            }


            //TODO: apply to locals
        }
    }
}
