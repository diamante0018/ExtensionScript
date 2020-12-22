// ==================== ExtensionScript ===================
// Admin Manager via Rcon. It is recommended you
// use this script with IW4M 
// Project: https://github.com/diamante0018/ExtensionScript
// Author: Diavolo (https://github.com/diamante0018)
// License: GNU GPL v3.0
// ========================================================
using System.Text;

namespace ExtensionScript
{
    public static class ExtUtil
    {
        public static unsafe string GetDSRName()
        {
            int address = 0x01B3ECB3;

            StringBuilder result = new StringBuilder();

            for (; address < address + 8 && *(byte*)address != 0; address++)
                result.Append(Encoding.ASCII.GetString(new byte[] { *(byte*)address }));

            return result.ToString();
        }
    }
}