using System;
using System.Collections.Generic;
using dnlib.DotNet;

namespace Confuser.Protections.HoLLy.AntiMemoryEditing
{
    //This is probably not needed, but it looks nice :)
    internal interface IMemoryEditService
    {
        void AddToList(FieldDef d);

        IEnumerable<FieldDef> GetFields();
    }

    internal class MemoryEditService : IMemoryEditService
    {
        private List<FieldDef> _fields = new List<FieldDef>();

        public void AddToList(FieldDef d) => _fields.Add(d);
        public IEnumerable<FieldDef> GetFields() => _fields;
    }
}
