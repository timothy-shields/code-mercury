using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace CodeMercury
{
    /// <summary>
    /// Represents an infinite binary string, where only a
    /// finite number of bits are set to true and the rest
    /// are set to false.
    /// </summary>
    [JsonConverter(typeof(BoolString.JsonConverter))]
    public class BoolString : IComparable<BoolString>, IEquatable<BoolString>
    {
        private List<bool> boundedBits;

        public int BoundedCount
        {
            get { return boundedBits.Count; }
        }

        public IEnumerable<bool> BoundedBits
        {
            get { return boundedBits; }
        }

        public IEnumerable<bool> Bits
        {
            get { return boundedBits.Concat(EnumerableEx.Repeat(false)); }
        }

        public BoolString(IEnumerable<bool> boundedBits)
        {
            this.boundedBits = boundedBits.ToList();
            var i = this.boundedBits.LastIndexOf(true) + 1;
            this.boundedBits.RemoveRange(i, this.boundedBits.Count - i);
        }

        public BoolString(IEnumerable<byte> boundedBytes)
            : this(boundedBytes.SelectMany(b => Enumerable.Range(0, 8).Select(x => (byte)(1 << x)).Select(x => (b & x) != 0)))
        {
        }

        public BoolString(string boundedBits)
            : this(boundedBits.Select(x => x != '0'))
        {
        }

        public override string ToString()
        {
            return new string(boundedBits.Select(x => x ? '1' : '0').ToArray());
        }
        
        private static IEnumerable<TResult> Combine<TResult>(BoolString u, BoolString v, Func<bool, bool, TResult> operation)
        {
            return Enumerable
                .Zip(u.Bits, v.Bits, operation)
                .Take(Math.Max(u.BoundedCount, v.BoundedCount));
        }

        public static int HammingDistance(BoolString u, BoolString v)
        {
            return Combine(u, v, (a, b) => a != b).Count(x => x);
        }

        public static int Compare(BoolString u, BoolString v)
        {
            return Combine(u, v, (a, b) => a.CompareTo(b)).FirstOrDefault(x => x != 0);
        }

        public static bool Equals(BoolString u, BoolString v)
        {
            return u.BoundedBits.SequenceEqual(v.BoundedBits);
        }

        public int CompareTo(BoolString other)
        {
            return Compare(this, other);
        }

        public bool Equals(BoolString other)
        {
            return Equals(this, other);
        }

        public override bool Equals(object obj)
        {
            if (this == obj)
                return true;
            else if (obj == null)
                return false;
            else
                return Equals(obj as BoolString);
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }

        public static BoolString First()
        {
            return new BoolString(Enumerable.Empty<bool>());
        }

        public BoolString Next()
        {
            var next = new BoolString(boundedBits);
            for (int i = 0; i < BoundedCount; i++)
            {
                if (next.boundedBits[i])
                {
                    next.boundedBits[i] = false;
                }
                else
                {
                    next.boundedBits[i] = true;
                    return next;
                }
            }
            next.boundedBits.Add(true);
            return next;
        }

        public class JsonConverter : Newtonsoft.Json.JsonConverter
        {
            public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
            {
                writer.WriteValue(value.As<BoolString>().ToString());
            }

            public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
            {
                return new BoolString(reader.Value.As<string>());
            }

            public override bool CanConvert(Type objectType)
            {
                return true;
            }
        }
    }
}
