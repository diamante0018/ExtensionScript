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
