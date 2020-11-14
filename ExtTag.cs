using InfinityScript;
using System.Text;

namespace ExtensionScript
{
    public static class ExtTag
    {
        public static unsafe void SetClanTag(this Entity player, string tag)
        {
            if (player == null || !player.IsPlayer || tag.Length > 8)
                return;

            int address = 0x38A4 * player.EntRef + 0x01AC5564;

            for (int i = 0; i < tag.Length; i++)
                *(byte*)(address + i) = (byte)tag[i];

            *(byte*)(address + tag.Length) = 0;
        }

        public static unsafe string GetClanTag(this Entity player)
        {
            if (player == null || !player.IsPlayer)
                return null;

            int address = 0x38A4 * player.EntRef + 0x01AC5564;

            string result = "";
            for (; address < address + 8 && *(byte*)address != 0; address++)
                result += Encoding.ASCII.GetString(new byte[] { *(byte*)address });

            return result;
        }

        public static unsafe string SetName(this Entity player, string name)
        {
            if (player == null || !player.IsPlayer || name.Length > 15)
                return null;

            for (int i = 0; i < name.Length; i++)
            {
                *(byte*)((0x38A4 * player.EntRef + 0x1AC5490) + i) = (byte)name[i];
                *(byte*)((0x38A4 * player.EntRef + 0x1AC5508) + i) = (byte)name[i];
            }

            return name;
        }

        public static void MyGiveMaxAmmo(this Entity player)
        {
            string gun = player.GetCurrentWeapon();
            player.GiveStartAmmo(gun);
            player.GiveMaxAmmo(gun);
        }
    }
}
