/*
 * TeamSpeak 3 server minimal sample C#
 *
 * Copyright (c) 2007-2010 TeamSpeak-Systems
 */

using System;
using System.Runtime.InteropServices;
using System.IO;

using anyID = System.UInt16;
using uint64 = System.UInt64;
using teamspeak.constant.error;
using teamspeak.enumeration.server;

namespace ts3_server_minimal_sample
{
//    class ts3server {
//        /*
//         * Read server key from file
//        */
//        public static int readKeyPairFromFile(string fileName, out string keyPair) {
//            try {
//                keyPair = System.IO.File.ReadAllText(fileName);
//            } catch (System.Exception /*ex*/) {
//                Console.WriteLine("Could not open file '{0}' for reading keypair", fileName);
//                keyPair = "";
//                return -1;
//            }
//            Console.WriteLine("Read keypair '{0}' from file '{1}'.", keyPair, fileName);
//            return 0;
//        }

//        /*
//         * Write server key to file
//        */
//        public static int writeKeyPairToFile(string fileName, string keyPair) {
//            try {
//                File.WriteAllText(fileName, keyPair);
//            } catch (System.Exception /*ex*/) {
//                return -1;
//            }
//            return 0;
//        }

//        #if x64
//            [DllImport("ts3server_win64.dll", EntryPoint = "ts3server_initServerLib")]
//            public static extern uint ts3server_initServerLib(ref server_callback_struct arg0, LogTypes arg1, string arg2);

//            [DllImport("ts3server_win64.dll", EntryPoint = "ts3server_getServerLibVersion")]
//            public static extern uint ts3server_getServerLibVersion(out IntPtr arg0);

//            [DllImport("ts3server_win64.dll", EntryPoint = "ts3server_freeMemory")]
//            public static extern uint ts3server_freeMemory(IntPtr arg0);

//            [DllImport("ts3server_win64.dll", EntryPoint = "ts3server_destroyServerLib")]
//            public static extern uint ts3server_destroyServerLib();

//            [DllImport("ts3server_win64.dll", EntryPoint = "ts3server_createVirtualServer")]
//            public static extern uint ts3server_createVirtualServer(int serverPort, string ip, string serverName, string serverKeyPair, uint serverMaxClients, out uint64 serverID);

//            [DllImport("ts3server_win64.dll", EntryPoint = "ts3server_getGlobalErrorMessage")]
//            public static extern uint ts3server_getGlobalErrorMessage(uint errorcode, out IntPtr errormessage);

//            [DllImport("ts3server_win64.dll", EntryPoint = "ts3server_getVirtualServerKeyPair")]
//            public static extern uint ts3server_getVirtualServerKeyPair(uint64 serverID, out IntPtr result);

//            [DllImport("ts3server_win64.dll", EntryPoint = "ts3server_setVirtualServerVariableAsString")]
//            public static extern uint ts3server_setVirtualServerVariableAsString(uint64 serverID, VirtualServerProperties flag, string result);

//            [DllImport("ts3server_win64.dll", EntryPoint = "ts3server_flushVirtualServerVariable")]
//            public static extern uint ts3server_flushVirtualServerVariable(uint64 serverID);

//            [DllImport("ts3server_win64.dll", EntryPoint = "ts3server_stopVirtualServer")]
//            public static extern uint ts3server_stopVirtualServer(uint64 serverID);
//        #else
//            [DllImport("ts3server_win32.dll", EntryPoint = "ts3server_initServerLib")]
//            public static extern uint ts3server_initServerLib(ref server_callback_struct arg0, LogTypes arg1, string arg2);

//            [DllImport("ts3server_win32.dll", EntryPoint = "ts3server_getServerLibVersion")]
//            public static extern uint ts3server_getServerLibVersion(out IntPtr arg0);

//            [DllImport("ts3server_win32.dll", EntryPoint = "ts3server_freeMemory")]
//            public static extern uint ts3server_freeMemory(IntPtr arg0);

//            [DllImport("ts3server_win32.dll", EntryPoint = "ts3server_destroyServerLib")]
//            public static extern uint ts3server_destroyServerLib();

//            [DllImport("ts3server_win32.dll", EntryPoint = "ts3server_createVirtualServer")]
//            public static extern uint ts3server_createVirtualServer(int serverPort, string ip, string serverName, string serverKeyPair, uint serverMaxClients, out uint64 serverID);

//            [DllImport("ts3server_win32.dll", EntryPoint = "ts3server_getGlobalErrorMessage")]
//            public static extern uint ts3server_getGlobalErrorMessage(uint errorcode, out IntPtr errormessage);

//            [DllImport("ts3server_win32.dll", EntryPoint = "ts3server_getVirtualServerKeyPair")]
//            public static extern uint ts3server_getVirtualServerKeyPair(uint64 serverID, out IntPtr result);

//            [DllImport("ts3server_win32.dll", EntryPoint = "ts3server_setVirtualServerVariableAsString")]
//            public static extern uint ts3server_setVirtualServerVariableAsString(uint64 serverID, VirtualServerProperties flag, string result);

//            [DllImport("ts3server_win32.dll", EntryPoint = "ts3server_flushVirtualServerVariable")]
//            public static extern uint ts3server_flushVirtualServerVariable(uint64 serverID);

//            [DllImport("ts3server_win32.dll", EntryPoint = "ts3server_stopVirtualServer")]
//            public static extern uint ts3server_stopVirtualServer(uint64 serverID);
//#endif
//    }

	class Program {
		static void Main(string[] args) {
			/* Assign the used callback function pointers */
            //server_callback_struct cbs = new server_callback_struct();
            //cbs.onClientConnected_delegate = callback_x.onClientConnected;
            //cbs.onClientDisconnected_delegate = callback_x.onClientDisconnected;
            //cbs.onClientMoved_delegate = callback_x.onClientMoved;
            //cbs.onChannelCreated_delegate = callback_x.onChannelCreated;
            //cbs.onChannelEdited_delegate = callback_x.onChannelEdited;
            //cbs.onChannelDeleted_delegate = callback_x.onChannelDeleted;
            //cbs.onServerTextMessageEvent_delegate = callback_x.onServerTextMessageEvent;
            //cbs.onChannelTextMessageEvent_delegate = callback_x.onChannelTextMessageEvent;
            //cbs.onUserLoggingMessageEvent_delegate = callback_x.onUserLoggingMessageEvent;
            //cbs.onClientStartTalkingEvent_delegate = callback_x.onClientStartTalkingEvent;
            //cbs.onClientStopTalkingEvent_delegate = callback_x.onClientStopTalkingEvent;
            //cbs.onAccountingErrorEvent_delegate = callback_x.onAccountingErrorEvent;

            //ServerEventHandlerMapper cbs2 = callback_x.getDefaultMapper();
            server_callback_structx cbs2 = callback_x.getDefaultMapper();
			/* Initialize server lib with callbacks */
            uint error = TS3ServerDLLFacade.initServerLib(ref cbs2, LogTypes.LogType_FILE | LogTypes.LogType_CONSOLE | LogTypes.LogType_USERLOGGING, null);
            if (error != Error.ok) {
                Console.WriteLine("Failed to initialize serverlib: {0}.", error);
                return;
            }

			/* Query and print client lib version */
			IntPtr versionPtr = IntPtr.Zero;
			error = TS3ServerDLLFacade.getServerLibVersion(out versionPtr);
            if (error != Error.ok)
            {
				Console.WriteLine("Failed to get clientlib version: {0}.", error);
				return;
			}
			string version = Marshal.PtrToStringAnsi(versionPtr);
			Console.WriteLine(version);
			TS3ServerDLLFacade.freeMemory(versionPtr); /* Release dynamically allocated memory */

			string filename = string.Format("keypair_{0}.txt", 9987); // 9987 = default port
			string keyPair;
            if (TS3ServerDLLFacade.readKeyPairFromFile(filename, out keyPair))
            {
				keyPair = "";
			}

			/* Create virtual server using default port 9987 with max 10 slots */

			/* Create the virtual server with specified port, name, keyPair and max clients */
            uint64 serverID = 0;
			Console.WriteLine("Create virtual server using keypair '{0}'", keyPair);
			IntPtr pServerID = IntPtr.Zero;
			error = TS3ServerDLLFacade.createVirtualServer(9987, "0.0.0.0", "TeamSpeak 3 SDK Testserver", keyPair, 10, out serverID);
            if (error != Error.ok)
            {
				IntPtr errormsgPtr = IntPtr.Zero;
				TS3ServerDLLFacade.getGlobalErrorMessage(error, out errormsgPtr);
                if (error == Error.ok)
                {
					string errormsg = Marshal.PtrToStringAnsi(errormsgPtr);
					Console.WriteLine("Error creating virtual server: {0} ({1})", errormsg, error);
					TS3ServerDLLFacade.freeMemory(errormsgPtr);
				}
				return;
			}
			
			/* If we didn't load the keyPair before, query it from virtual server and save to file */
			if (keyPair == null) {
				IntPtr keyPairPtr = IntPtr.Zero;
				error = TS3ServerDLLFacade.getVirtualServerKeyPair(serverID, out keyPairPtr);
                if (error != Error.ok)
                {
					IntPtr errormsgPtr = IntPtr.Zero;
					TS3ServerDLLFacade.getGlobalErrorMessage(error, out errormsgPtr);
                    if (error == Error.ok)
                    {
						string errormsg = Marshal.PtrToStringAnsi(errormsgPtr);
						Console.WriteLine("Error querying keyPair: %s\n", errormsg);
						TS3ServerDLLFacade.freeMemory(errormsgPtr);
					}
					return;
				}
				keyPair = Marshal.PtrToStringAnsi(keyPairPtr);

				/* Save keyPair to file "keypair_<port>.txt"*/
                if (TS3ServerDLLFacade.writeKeyPairToFile(filename, keyPair))
                {
					return;
				}
			}

			/* Set welcome message */
			error = TS3ServerDLLFacade.setVirtualServerVariableAsString(serverID, VirtualServerProperties.VIRTUALSERVER_WELCOMEMESSAGE, "Hello TeamSpeak 3");
            if (error != Error.ok)
            {
				Console.WriteLine("Error setting server welcomemessage: {0}", error);
				return;
			}

			/* Set server password */
			error = TS3ServerDLLFacade.setVirtualServerVariableAsString(serverID, VirtualServerProperties.VIRTUALSERVER_PASSWORD, "secret");
            if (error != Error.ok)
            {
				Console.WriteLine("Error setting server password: {0}", error);
				return;
			}

			/* Flush above two changes to server */
			error = TS3ServerDLLFacade.flushVirtualServerVariable(serverID);
            if (error != Error.ok)
            {
				Console.WriteLine("Error flushing server variables: {0}", error);
				return;
			}

			/* Wait for user input */
			Console.WriteLine("\n--- Press Return to shutdown server and exit ---");
			Console.ReadLine();

			/* Stop virtual server */
			error = TS3ServerDLLFacade.stopVirtualServer(serverID);
            if (error != Error.ok)
            {
				Console.WriteLine("Error stopping virtual server: {0}", error);
				return;
			}

			/* Shutdown server lib */
			error = TS3ServerDLLFacade.destroyServerLib();
            if (error != Error.ok)
            {
				Console.WriteLine("Error destroying server lib: {0}", error);
				return;
			}

		}
	}
}
