/*
 * TeamSpeak 3 server minimal sample C#
 *
 * Copyright (c) 2007-2010 TeamSpeak-Systems
 */

using System;
using System.Runtime.InteropServices;

using uint64 = System.UInt64;

internal class ServerMainLoop
{
    private static void Main(string[] args)
    {
        /* Assign the used callback function pointers */
        ServerEventCallbackMapper mapper = ServerEventHandler.getDefaultEventCallbackMapper();
        /* Initialize server lib with callbacks */
        uint error = ServerDLLFacade.initServerLib(ref mapper, LogTypes.LogType_FILE | LogTypes.LogType_CONSOLE | LogTypes.LogType_USERLOGGING, null);
        if (error != Error.ok)
        {
            Console.WriteLine("Failed to initialize serverlib: {0}.", error);
            return;
        }

        /* Query and print client lib version */
        IntPtr versionPtr = IntPtr.Zero;
        error = ServerDLLFacade.getServerLibVersion(out versionPtr);
        if (error != Error.ok)
        {
            Console.WriteLine("Failed to get clientlib version: {0}.", error);
            return;
        }
        string version = Marshal.PtrToStringAnsi(versionPtr);
        Console.WriteLine(version);
        ServerDLLFacade.freeMemory(versionPtr); /* Release dynamically allocated memory */

        string filename = string.Format("keypair_{0}.txt", 9987); // 9987 = default port
        string keyPair;
        if (ServerDLLFacade.readKeyPairFromFile(filename, out keyPair))
        {
            keyPair = "";
        }

        /* Create virtual server using default port 9987 with max 10 slots */

        /* Create the virtual server with specified port, name, keyPair and max clients */
        uint64 serverID = 0;
        Console.WriteLine("Create virtual server using keypair '{0}'", keyPair);
        IntPtr pServerID = IntPtr.Zero;
        error = ServerDLLFacade.createVirtualServer(9987, "0.0.0.0", "TeamSpeak 3 SDK Testserver", keyPair, 10, out serverID);
        if (error != Error.ok)
        {
            IntPtr errormsgPtr = IntPtr.Zero;
            ServerDLLFacade.getGlobalErrorMessage(error, out errormsgPtr);
            if (error == Error.ok)
            {
                string errormsg = Marshal.PtrToStringAnsi(errormsgPtr);
                Console.WriteLine("Error creating virtual server: {0} ({1})", errormsg, error);
                ServerDLLFacade.freeMemory(errormsgPtr);
            }
            return;
        }

        /* If we didn't load the keyPair before, query it from virtual server and save to file */
        if (keyPair == null)
        {
            IntPtr keyPairPtr = IntPtr.Zero;
            error = ServerDLLFacade.getVirtualServerKeyPair(serverID, out keyPairPtr);
            if (error != Error.ok)
            {
                IntPtr errormsgPtr = IntPtr.Zero;
                ServerDLLFacade.getGlobalErrorMessage(error, out errormsgPtr);
                if (error == Error.ok)
                {
                    string errormsg = Marshal.PtrToStringAnsi(errormsgPtr);
                    Console.WriteLine("Error querying keyPair: %s\n", errormsg);
                    ServerDLLFacade.freeMemory(errormsgPtr);
                }
                return;
            }
            keyPair = Marshal.PtrToStringAnsi(keyPairPtr);

            /* Save keyPair to file "keypair_<port>.txt"*/
            if (ServerDLLFacade.writeKeyPairToFile(filename, keyPair))
            {
                return;
            }
        }

        /* Set welcome message */
        error = ServerDLLFacade.setVirtualServerVariableAsString(serverID, VirtualServerProperties.VIRTUALSERVER_WELCOMEMESSAGE, "Hello TeamSpeak 3");
        if (error != Error.ok)
        {
            Console.WriteLine("Error setting server welcomemessage: {0}", error);
            return;
        }

        /* Set server password */
        error = ServerDLLFacade.setVirtualServerVariableAsString(serverID, VirtualServerProperties.VIRTUALSERVER_PASSWORD, "secret");
        if (error != Error.ok)
        {
            Console.WriteLine("Error setting server password: {0}", error);
            return;
        }

        /* Flush above two changes to server */
        error = ServerDLLFacade.flushVirtualServerVariable(serverID);
        if (error != Error.ok)
        {
            Console.WriteLine("Error flushing server variables: {0}", error);
            return;
        }

        /* Wait for user input */
        Console.WriteLine("\n--- Press Return to shutdown server and exit ---");
        Console.ReadLine();

        /* Stop virtual server */
        error = ServerDLLFacade.stopVirtualServer(serverID);
        if (error != Error.ok)
        {
            Console.WriteLine("Error stopping virtual server: {0}", error);
            return;
        }

        /* Shutdown server lib */
        error = ServerDLLFacade.destroyServerLib();
        if (error != Error.ok)
        {
            Console.WriteLine("Error destroying server lib: {0}", error);
            return;
        }
    }
}