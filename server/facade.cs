using System;
using System.IO;
using System.Runtime.InteropServices;
using teamspeak.enumeration.server;
using uint64 = System.UInt64;

namespace ts3_server_minimal_sample
{
    /* Ferry: This class is just a facade to link the ts3server_win32/64.dll at runtime
     * and import those needed UNMANAGED C++ functions so we can use it in .NET style.
     * I rewrite this class to be more easy to use and portable. 
     * (Shit that the DLL defined all methods are static... how come!)
     * The SDK sample code also included the read/write key pair method
     */
    internal class TS3ServerDLLFacade
    {
        /* Read server key from file */
        public static bool readKeyPairFromFile(string fileName, out string keyPair)
        {
            try
            {
                keyPair = System.IO.File.ReadAllText(fileName);
            }
            catch (Exception)
            {
                Console.WriteLine("Could not open file '{0}' for reading keypair", fileName);
                keyPair = string.Empty;
                return false;
            }
            Console.WriteLine("Read keypair '{0}' from file '{1}'.", keyPair, fileName);
            return true;
        }

        /* Write server key to file */
        public static bool writeKeyPairToFile(string fileName, string keyPair)
        {
            try
            {
                File.WriteAllText(fileName, keyPair);
            }
            catch (Exception)
            {
                Console.WriteLine("Could not open file '{0}' for writing keypair", fileName);
                return false;
            }
            Console.WriteLine("Keypair '{0}' written to file '{1}'.", keyPair, fileName);
            return true;
        }
        #region External Methods "Borrowed" (Imported) From ts3server_win32/64.dll
#if x64
        [DllImport("ts3server_win64.dll", EntryPoint = "ts3server_initServerLib")]
        public extern static uint initServerLib(ref server_callback_struct arg0, LogTypes arg1, string arg2);

        [DllImport("ts3server_win64.dll", EntryPoint = "ts3server_getServerLibVersion")]
        public extern static uint getServerLibVersion(out IntPtr arg0);

        [DllImport("ts3server_win64.dll", EntryPoint = "ts3server_freeMemory")]
        public extern static uint freeMemory(IntPtr arg0);

        [DllImport("ts3server_win64.dll", EntryPoint = "ts3server_destroyServerLib")]
        public extern static uint destroyServerLib();

        [DllImport("ts3server_win64.dll", EntryPoint = "ts3server_createVirtualServer")]
        public extern static uint createVirtualServer(int serverPort, string ip, string serverName, string serverKeyPair, uint serverMaxClients, out uint64 serverID);

        [DllImport("ts3server_win64.dll", EntryPoint = "ts3server_getGlobalErrorMessage")]
        public extern static uint getGlobalErrorMessage(uint errorcode, out IntPtr errormessage);

        [DllImport("ts3server_win64.dll", EntryPoint = "ts3server_getVirtualServerKeyPair")]
        public extern static uint getVirtualServerKeyPair(uint64 serverID, out IntPtr result);

        [DllImport("ts3server_win64.dll", EntryPoint = "ts3server_setVirtualServerVariableAsString")]
        public extern static uint setVirtualServerVariableAsString(uint64 serverID, VirtualServerProperties flag, string result);

        [DllImport("ts3server_win64.dll", EntryPoint = "ts3server_flushVirtualServerVariable")]
        public extern static uint flushVirtualServerVariable(uint64 serverID);

        [DllImport("ts3server_win64.dll", EntryPoint = "ts3server_stopVirtualServer")]
        public extern static uint stopVirtualServer(uint64 serverID);
#else
    [DllImport("ts3server_win32.dll", EntryPoint = "ts3server_initServerLib")]
    public extern static uint initServerLib(ref server_callback_st arg0, LogTypes arg1, string arg2);

    [DllImport("ts3server_win32.dll", EntryPoint = "ts3server_getServerLibVersion")]
    public extern static uint getServerLibVersion(out IntPtr arg0);

    [DllImport("ts3server_win32.dll", EntryPoint = "ts3server_freeMemory")]
    public extern static uint freeMemory(IntPtr arg0);

    [DllImport("ts3server_win32.dll", EntryPoint = "ts3server_destroyServerLib")]
    public extern static uint destroyServerLib();

    [DllImport("ts3server_win32.dll", EntryPoint = "ts3server_createVirtualServer")]
    public extern static uint createVirtualServer(int serverPort, string ip, string serverName, string serverKeyPair, uint serverMaxClients, out uint64 serverID);

    [DllImport("ts3server_win32.dll", EntryPoint = "ts3server_getGlobalErrorMessage")]
    public extern static uint getGlobalErrorMessage(uint errorcode, out IntPtr errormessage);

    [DllImport("ts3server_win32.dll", EntryPoint = "ts3server_getVirtualServerKeyPair")]
    public extern static uint getVirtualServerKeyPair(uint64 serverID, out IntPtr result);

    [DllImport("ts3server_win32.dll", EntryPoint = "ts3server_setVirtualServerVariableAsString")]
    public extern static uint setVirtualServerVariableAsString(uint64 serverID, VirtualServerProperties flag, string result);

    [DllImport("ts3server_win32.dll", EntryPoint = "ts3server_flushVirtualServerVariable")]
    public extern static uint flushVirtualServerVariable(uint64 serverID);

    [DllImport("ts3server_win32.dll", EntryPoint = "ts3server_stopVirtualServer")]
    public extern static uint stopVirtualServer(uint64 serverID);

#endif
        #endregion
    }
}