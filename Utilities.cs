// ==================== ExtensionScript ===================
// Admin Manager via Rcon. It is recommended you
// Use this script with IW4M 
// Project: https://github.com/diamante0018/ExtensionScript
// Author: Diavolo (https://github.com/diamante0018)
// License: GNU GPL v3.0
// ========================================================
using System.Text;

namespace ExtensionScript
{
    public static class Mem
    {
        public static unsafe string ReadString(int address, int maxlen = 0)
        {
            string ret = "";
            maxlen = (maxlen == 0) ? int.MaxValue : maxlen;

            for (; address < address + maxlen && *(byte*)address != 0; address++)
            {
                ret += Encoding.ASCII.GetString(new byte[] { *(byte*)address });
            }

            return ret;
        }

        public static unsafe void WriteString(int address, string str)
        {
            byte[] strarr = Encoding.ASCII.GetBytes(str);

            foreach (byte ch in strarr)
            {
                *(byte*)address = ch;
                address++;
            }

            *(byte*)address = 0;
        }
    }
}