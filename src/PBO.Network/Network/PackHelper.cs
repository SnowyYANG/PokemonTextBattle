using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Runtime.Serialization.Json;
using System.IO.Compression;

namespace PokemonBattleOnline.Network
{
    public static class PackHelper
    {
        public static readonly byte[] EMPTYPACK;

        static PackHelper()
        {
            EMPTYPACK = new byte[0];
        }

        public static void SendEmpty(this TcpPackSender network)
        {
            network.Send(EMPTYPACK);
        }
        public static bool IsEmpty(this byte[] pack)
        {
            return pack.Length == 0;
        }

        public static byte[] ToPack(this byte pack)
        {
            return new byte[] { pack };
        }
        public static byte? ToByte(this byte[] pack)
        {
            return pack.Length == 1 ? (byte?)pack[0] : null;
        }

        public static byte[] ToPack(this UInt16 pack)
        {
            return new byte[] { (byte)(pack >> 8), (byte)pack };
        }
        public static UInt16? ToUInt16(this byte[] pack)
        {
            return pack.Length == 2 ? (UInt16?)((pack[0] << 8) | pack[1]) : null;
        }
        public static UInt16? ToUInt16(this byte[] pack, int offset)
        {
            return pack.Length > offset + 1 ? (UInt16?)((pack[offset] << 8) | pack[offset + 1]) : null;
        }

        public static byte[] ToPack(this string s)
        {
            var pack = new byte[Encoding.Unicode.GetByteCount(s)];
            Encoding.Unicode.GetBytes(s, 0, s.Length, pack, 0);
            return pack;
        }
        public static string ToUnicodeString(this byte[] pack)
        {
            return Encoding.Unicode.GetString(pack);
        }

        public static byte[] ToPack<T>(this T obj)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                using (DeflateStream ds = new DeflateStream(ms, CompressionMode.Compress, true))
                {
                    var s = new DataContractJsonSerializer(typeof(T));
                    s.WriteObject(ds, obj);
                }
                return ms.ToArray();
            }
        }
        public static T ToObject<T>(this byte[] pack)
        {
            using (MemoryStream ms = new MemoryStream(pack))
            using (DeflateStream ds = new DeflateStream(ms, CompressionMode.Decompress))
            {
                var d = new DataContractJsonSerializer(typeof(T));
                return (T)d.ReadObject(ds);
            }
        }
    }
}
