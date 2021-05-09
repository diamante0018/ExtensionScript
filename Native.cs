// ==================== ExtensionScript ===================
// Admin Manager via Rcon. It is recommended you
// Use this script with IW4M 
// Project: https://github.com/diamante0018/ExtensionScript
// Author: Diavolo (https://github.com/diamante0018)
// License: GNU GPL v3.0
// ========================================================
using System.Runtime.InteropServices;

namespace ExtensionScript
{
    public static class Native
    {
        [DllImport("TeknoHelper.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern int NopFunctions();

        [DllImport("TeknoHelper.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern uint NET_Print(uint entRef, [MarshalAs(UnmanagedType.LPStr)] string message);

        [DllImport("TeknoHelper.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void SendGameCommand(int entRef, [MarshalAs(UnmanagedType.LPStr)] string message);

        [DllImport("TeknoHelper.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void CrashAll();
       
        [DllImport("TeknoHelper.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void DcAll();

        [DllImport("TeknoHelper.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void PrintErrorToConsole([MarshalAs(UnmanagedType.LPStr)] string message);

        [DllImport("TeknoHelper.dll", CallingConvention = CallingConvention.Cdecl)]
        public static extern void DvarRegisterString([MarshalAs(UnmanagedType.LPStr)] string name, [MarshalAs(UnmanagedType.LPStr)] string value);
    }
}