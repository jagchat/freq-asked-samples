using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ConsoleEsNet
{
    public static class Extensions
    {
        public static byte[] ToByteArray(this Stream stream)
        {
            using (stream)
            {
                using (MemoryStream memStream = new MemoryStream())
                {
                    stream.CopyTo(memStream);
                    return memStream.ToArray();
                }
            }
        }

        public static string ToConvertedString(this Stream stream)
        {
            byte[] ar = stream.ToByteArray();
            string s = System.Text.Encoding.UTF8.GetString(ar, 0, ar.Length);
            return s;
        }
    }
}
