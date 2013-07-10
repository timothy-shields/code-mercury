//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using MsgPack;
//using System.IO;

//namespace CodeMercury.MsgPack
//{
//    public static class MpHelper
//    {
//        public static void PackType(this Packer packer, Type type)
//        {
//            packer.PackString(type.AssemblyQualifiedName);
//        }

//        public static Type UnpackType(this Unpacker unpacker)
//        {
//            return Type.GetType(unpacker.Unpack<string>());
//        }

//        public static byte[] SerializeMpObject(MpObject mpObject)
//        {
//            using (var stream = new MemoryStream())
//            {
//                using (var packer = Packer.Create(stream, false))
//                    packer.Pack(mpObject);
//                return stream.ToArray();
//            }
//        }

//        public static MpObject DeserializeMpObject(byte[] bytes)
//        {
//            using (var stream = new MemoryStream(bytes))
//            {
//                MpObject mpObject;
//                using (var unpacker = Unpacker.Create(stream, false))
//                    mpObject = unpacker.Unpack<MpObject>();
//                return mpObject;
//            }
//        }
//    }
//}
