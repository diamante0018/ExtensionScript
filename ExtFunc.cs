using InfinityScript;
using System.Collections.Generic;
using System.Text;

namespace ExtensionScript
{
    public static class ExtFunc
    {
        private static Dictionary<string, Dictionary<string, int>> fields = new Dictionary<string, Dictionary<string, int>>();

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

            StringBuilder result = new StringBuilder();

            for (; address < address + 8 && *(byte*)address != 0; address++)
                result.Append(Encoding.ASCII.GetString(new byte[] { *(byte*)address }));

            return result.ToString();
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

        public static unsafe void NoClip(this Entity player)
        {
            byte set = 0x01;
            if (player.HasNoClip())
                set = 0x00;

            int address = 0x38A4 * player.EntRef + 0x01AC56C0;
            *(byte*)address = set;
        }

        public static unsafe bool HasNoClip(this Entity player)
        {
            int address = 0x38A4 * player.EntRef + 0x01AC56C0;
            return *(byte*)address == 1;
        }

        public static void MyGiveMaxAmmo(this Entity player, bool feedback = true)
        {
            string gun = player.GetCurrentWeapon();
            player.GiveStartAmmo(gun);
            player.GiveMaxAmmo(gun);
            if (feedback)
            {
                player.PlayLocalSound("mp_suitcase_pickup");
                player.IPrintLnBold("^1Wow^0! ^3You have ^7received ^1Ammunition");
            }
        }

        public static bool MyHasField(this Entity player, string field)
        {
            if (!player.IsPlayer)
                return false;
            if (fields.ContainsKey(player.HWID))
                return fields[player.HWID].ContainsKey(field);
            return false;
        }

        public static void MySetField(this Entity player, string field, int value)
        {
            if (!player.IsPlayer)
                return;
            if (!fields.ContainsKey(player.HWID))
                fields.Add(player.HWID, new Dictionary<string, int>());

            if (!MyHasField(player, field))
                fields[player.HWID].Add(field, value);
            else
                fields[player.HWID][field] = value;
        }

        public static int MyGetField(this Entity player, string field)
        {
            if (!player.IsPlayer)
                return int.MinValue;
            if (!MyHasField(player, field))
                return int.MinValue;
            return fields[player.HWID][field];
        }

        public static void MyRemoveField(this Entity player)
        {
            fields.Remove(player.HWID);
        }
    }
}
