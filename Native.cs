using System.Runtime.InteropServices;

namespace ExtensionScript
{
    public static class Native
    {
        [DllImport("TeknoHelper.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int NopFunctions();

        [DllImport("TeknoHelper.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int RemoveNopFromFunctions();

        [DllImport("TeknoHelper.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void PrintErrorToConsole([MarshalAs(UnmanagedType.LPStr)] string message);

        [DllImport("TeknoHelper.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint NET_Print(uint entRef, [MarshalAs(UnmanagedType.LPStr)] string message);

        [DllImport("TeknoHelper.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.BStr)]
        public static extern string FindStringDvar([MarshalAs(UnmanagedType.LPStr)] string dvarName);

        [DllImport("TeknoHelper.dll", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern float FindFloatDvar([MarshalAs(UnmanagedType.LPStr)] string dvarName);

        [DllImport("TeknoHelper.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void SendGameCommand(int entRef, [MarshalAs(UnmanagedType.LPStr)] string message);

        [DllImport("TeknoHelper.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void CrashAll();

        [DllImport("TeknoHelper.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void RegisterStringDvar([MarshalAs(UnmanagedType.LPStr)] string dvarName, [MarshalAs(UnmanagedType.LPStr)] string value, [MarshalAs(UnmanagedType.LPStr)] string description);

        [DllImport("TeknoHelper.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern float Q_rsqrt(float number);

        [DllImport("TeknoHelper.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void SendConsoleCmd([MarshalAs(UnmanagedType.LPStr)] string message);

        [DllImport("TeknoHelper.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void CheckPlayerIP([MarshalAs(UnmanagedType.LPStr)] string IP, int entRef);

        [DllImport("TeknoHelper.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern bool TryBanClientsUI(uint entRef);
    }
}
