//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using Newtonsoft.Json.Linq;
//using Newtonsoft.Json;
//using System.IO;
//using System.Security.Cryptography;
//using MsgPack;

//namespace CodeMercury.MsgPack
//{
//    /// <summary>
//    /// A self-describing MsgPack-serialized object.
//    /// </summary>
//    public class MpObject : IPackable, IUnpackable
//    {
//        public Type Type { get; set; }
//        public byte[] Value { get; set; }

//        public static MpObject FromValue<T>(T value)
//        {
//            return new MpObject
//            {
//                Type = typeof(T),
//                Value = MpHelper.ToMpObject(value)
//            };
//        }

//        public object ToObject()
//        {

//            return JsonHelper.ToObject(Value, Type);
//        }

//        /// <summary>
//        /// If this message's value is of type T, then apply the action.
//        /// </summary>
//        public void When<T>(Action<T> action)
//        {
//            if (typeof(T).IsAssignableFrom(Type))
//                action(JsonHelper.ToObject<T>(Value));
//        }

//        public static byte[] Hash(MpObject message)
//        {
//            return HashHelper.ComputeHash(Serialize(message));
//        }

//        public static string HashString(MpObject message)
//        {
//            return Convert.ToBase64String(Hash(message));
//        }

//        public static BoolString HashBoolString(MpObject message)
//        {
//            return new BoolString(Hash(message).Take(2));
//        }

//        public static byte[] Serialize(MpObject message)
//        {
//            return Encoding.ASCII.GetBytes(JToken.FromObject(message).ToString(Formatting.None));
//        }

//        public static MpObject Deserialize(byte[] message)
//        {
//            return MpHelper.Parse(Encoding.ASCII.GetString(message)).ToObject<MpObject>();
//        }

//        public void PackToMessage(Packer packer, PackingOptions options)
//        {
//            packer.PackType(Type);
//            packer.PackRaw(Value);
//        }

//        public void UnpackFromMessage(Unpacker unpacker)
//        {
//            Type = unpacker.UnpackType();
//            Value = unpacker.Unpack<byte[]>();
//        }
//    }
//}
