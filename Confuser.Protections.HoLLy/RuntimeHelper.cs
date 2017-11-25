using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using dnlib.DotNet;

namespace Confuser.Protections.HoLLy
{
    internal static class RuntimeHelper
    {
        private static readonly Lazy<ModuleDefMD> CurrentModule = new Lazy<ModuleDefMD>(() => ModuleDefMD.Load(typeof(RuntimeHelper).Module));

        public static TypeDef GetType(Type t)
        {
            return CurrentModule.Value.Find(t.FullName, true);
        }
    }
}
