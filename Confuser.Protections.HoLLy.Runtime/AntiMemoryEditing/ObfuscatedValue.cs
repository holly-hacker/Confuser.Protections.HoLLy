using System;
using System.Text;

namespace Confuser.Protections.HoLLy.Runtime.AntiMemoryEditing
{
    internal class ObfuscatedValue<T>
    {
        private static readonly Random Rand = new Random();
        private readonly int _salt;
        private T _val;

        public T Value
        {
            get => Obfuscate(_val, true);
            set => _val = Obfuscate(value, false);
        }

        public ObfuscatedValue(T val)
        {
            _salt = Rand.Next(int.MinValue, int.MaxValue);
            Value = val;
        }
        
        //allow normal read/write
        public static implicit operator T(ObfuscatedValue<T> value) => value.Value;
        public static implicit operator ObfuscatedValue<T>(T value) => new ObfuscatedValue<T>(value);

        public override string ToString() => Value.ToString();

        private T Obfuscate(T val, bool reverse = false) => Obfuscate(val, _salt, reverse);

        private static T Obfuscate(T currentvalue, int salt, bool reverse)
        {
            Type type = currentvalue.GetType();

            switch (currentvalue)
            {
                case string s:
                    return (T)(object)XorString(s, salt);
                case int i:
                    return (T)(object)(i ^ salt);
                case double d:
                    return (T)(object)(d * (reverse ? 1.0 / salt : salt));
                case float f:
                    return (T)(object)(f * (reverse ? 1f / salt : salt));
                default:
                    if (type.BaseType == typeof(Enum))
                        return (T)(object)((int)Convert.ChangeType(currentvalue, typeof(int)) ^ salt);
                    else
                        throw new NotSupportedException();
            }
        }

        private static string XorString(string str, int salt)
        {
            var sb = new StringBuilder(str.Length);
            foreach (char c in str) {
                sb.Append((char) (c ^ salt));
            }
            return sb.ToString();
        }
    }
}
