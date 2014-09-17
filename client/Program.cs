/*
 * TeamSpeak 3 client minimal sample C#
 *
 * Copyright (c) 2007-2010 TeamSpeak-Systems
 */

using System;
using System.Text;
using System.Runtime.InteropServices;
using System.Threading;
using teamspeak.definitions;

namespace teamspeak {
	public class CustomTS3Client
    {
        public CustomTS3Client() { }

        #region Event Handlers

        public static void onConnectStatusChangeEvent(ulong serverConnectionHandlerID, int newStatus, uint errorNumber)
        {
            Console.WriteLine("Connect status changed: {0} {1} {2}", serverConnectionHandlerID, newStatus, errorNumber);
            /* Failed to connect ? */
            if (newStatus == (int)ConnectStatus.STATUS_DISCONNECTED && errorNumber == Error.failed_connection_initialisation)
            {
                Console.WriteLine("Looks like there is no server running, terminate!");
                Console.ReadLine();
                System.Environment.Exit(-1);
            }
        }

        public static void onServerProtocolVersionEvent(ulong serverConnectionHandlerID, int protocolVersion)
        {
            Console.WriteLine("Server protocol version: {0} {1}", serverConnectionHandlerID, protocolVersion);
        }

        public static void onNewChannelEvent(ulong serverConnectionHandlerID, ulong channelID, ulong channelParentID)
        {
            string name;
            string errormsg;
            uint error;
            IntPtr namePtr = IntPtr.Zero;
            Console.WriteLine("onNewChannelEvent: {0} {1} {2}", serverConnectionHandlerID, channelID, channelParentID);
            error = ts3client_getChannelVariableAsString(serverConnectionHandlerID, channelID, ChannelProperties.CHANNEL_NAME, out namePtr);
            if (error == Error.ok)
            {
                name = Marshal.PtrToStringAnsi(namePtr);
                Console.WriteLine("New channel: {0} {1}", channelID, name);
                ts3client_freeMemory(namePtr);  /* Release dynamically allocated memory only if function succeeded */
            }
            else
            {
                IntPtr errormsgPtr = IntPtr.Zero;
                if (ts3client_getErrorMessage(error, errormsgPtr) == Error.ok)
                {
                    errormsg = Marshal.PtrToStringAnsi(errormsgPtr);
                    Console.WriteLine("Error getting channel name in onNewChannelEvent: {0}", errormsg);
                    ts3client_freeMemory(errormsgPtr);
                }
            }
        }

        public static void onNewChannelCreatedEvent(ulong serverConnectionHandlerID, ulong channelID, ulong channelParentID, ushort invokerID, string invokerName, string invokerUniqueIdentifier)
        {
            string name;
            IntPtr namePtr = IntPtr.Zero;
            /* Query channel name from channel ID */
            uint error = ts3client_getChannelVariableAsString(serverConnectionHandlerID, channelID, ChannelProperties.CHANNEL_NAME, out namePtr);
            if (error != Error.ok)
            {
                return;
            }
            name = Marshal.PtrToStringAnsi(namePtr);
            Console.WriteLine("New channel created: {0}", name);
            ts3client_freeMemory(namePtr);  /* Release dynamically allocated memory only if function succeeded */
        }

        public static void onDelChannelEvent(ulong serverConnectionHandlerID, ulong channelID, ushort invokerID, string invokerName, string invokerUniqueIdentifier)
        {
            Console.WriteLine("Channel ID {0} deleted by {1} ({2})", channelID, invokerName, invokerID);
        }

        public static void onClientMoveEvent(ulong serverConnectionHandlerID, ushort clientID, ulong oldChannelID, ulong newChannelID, int visibility, string moveMessage)
        {
            Console.WriteLine("ClientID {0} moves from channel {1} to {2} with message {3}", clientID, oldChannelID, newChannelID, moveMessage);
        }

        public static void onClientMoveSubscriptionEvent(ulong serverConnectionHandlerID, ushort clientID, ulong oldChannelID, ulong newChannelID, int visibility)
        {
            string name;
            IntPtr namePtr = IntPtr.Zero;
            /* Query client nickname from ID */
            uint error = ts3client_getClientVariableAsString(serverConnectionHandlerID, clientID, ClientProperties.CLIENT_NICKNAME, out namePtr);
            if (error != Error.ok)
            {
                return;
            }
            name = Marshal.PtrToStringAnsi(namePtr);
            Console.WriteLine("New client: {0}", name);
            ts3client_freeMemory(namePtr);  /* Release dynamically allocated memory only if function succeeded */
        }

        public static void onClientMoveTimeoutEvent(ulong serverConnectionHandlerID, ushort clientID, ulong oldChannelID, ulong newChannelID, int visibility, string timeoutMessage)
        {
            Console.WriteLine("ClientID {0} timeouts with message {1}", clientID, timeoutMessage);
        }

        public static void onTalkStatusChangeEvent(ulong serverConnectionHandlerID, int status, int isReceivedWhisper, ushort clientID)
        {
            string name;
            IntPtr namePtr = IntPtr.Zero;
            /* Query client nickname from ID */
            uint error = ts3client_getClientVariableAsString(serverConnectionHandlerID, clientID, ClientProperties.CLIENT_NICKNAME, out namePtr);
            if (error != Error.ok)
            {
                return;
            }
            name = Marshal.PtrToStringAnsi(namePtr);
            if (status == (int)TalkStatus.STATUS_TALKING)
            {
                Console.WriteLine("Client \"{0}\" starts talking.", name);
            }
            else
            {
                Console.WriteLine("Client \"{0}\" stops talking.", name);
            }
            ts3client_freeMemory(namePtr);  /* Release dynamically allocated memory only if function succeeded */
        }

        public static void onIgnoredWhisperEvent(ulong serverConnectionHandlerID, ushort clientID)
        {
            Console.WriteLine("Ignored whisper: {0} {1}", serverConnectionHandlerID, clientID);
        }

        public static void onServerErrorEvent(ulong serverConnectionHandlerID, string errorMessage, uint error, string returnCode, string extraMessage)
        {
            Console.WriteLine("Error for server {0}: {1}", serverConnectionHandlerID, errorMessage);
        }

        public static void onServerStopEvent(ulong serverConnectionHandlerID, string shutdownMessage)
        {
            Console.WriteLine("Server {0} stopping: {1}", serverConnectionHandlerID, shutdownMessage);
        }
        #endregion
        #region DLL Import
#if x64
        const string DLL_FILE_NAME = "ts3client_win64.dll";
#else
        string DLL_FILE_NAME = "ts3client_win64.dll";
#endif
        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_initClientLib")]
         static extern uint ts3client_initClientLib(ref ClientEventCallbackMapper arg0, ref client_callbackrare_struct arg1, LogTypes arg2, string arg3, string arg4);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_getClientLibVersion")]
         static extern uint ts3client_getClientLibVersion(out IntPtr arg0);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_freeMemory")]
         static extern uint ts3client_freeMemory(IntPtr arg0);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_spawnNewServerConnectionHandler")]
         static extern uint ts3client_spawnNewServerConnectionHandler(int port, out ulong arg0);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_openCaptureDevice")]
         static extern uint ts3client_openCaptureDevice(ulong arg0, string arg1, string arg2);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_openPlaybackDevice")]
         static extern uint ts3client_openPlaybackDevice(ulong arg0, string arg1, string arg2);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_createIdentity")]
         static extern uint ts3client_createIdentity(out IntPtr arg0);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_startConnection", CharSet = CharSet.Ansi)]
         static extern uint ts3client_startConnection(ulong arg0, string identity, string ip, uint port, string nick, ref string defaultchannelarray, string defaultchannelpassword, string serverpassword);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_stopConnection")]
         static extern uint ts3client_stopConnection(ulong arg0, string arg1);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_destroyServerConnectionHandler")]
         static extern uint ts3client_destroyServerConnectionHandler(ulong arg0);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_destroyClientLib")]
         static extern uint ts3client_destroyClientLib();

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_getChannelVariableAsString")]
         static extern uint ts3client_getChannelVariableAsString(ulong arg0, ulong arg1, ChannelProperties arg2, out IntPtr arg3);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_getErrorMessage")]
         static extern uint ts3client_getErrorMessage(uint arg0, IntPtr arg1);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_getClientVariableAsString")]
         static extern uint ts3client_getClientVariableAsString(ulong arg0, ushort arg1, ClientProperties arg2, out IntPtr arg3);

        /*
unsigned int ts3client_getParentChannelOfChannel(	serverConnectionHandlerID,	 
 	channelID,	 
 	result);	 
uint64 serverConnectionHandlerID;
uint64 channelID;
uint64* result;
         * unsigned int ts3client_getChannelOfClient(	serverConnectionHandlerID,	 
 	clientID,	 
 	result);	 
uint64 serverConnectionHandlerID;
anyID clientID;
uint64* result;
unsigned int ts3client_getChannelClientList(	serverConnectionHandlerID,	 
 	channelID,	 
 	result);	 
uint64 serverConnectionHandlerID;
uint64 channelID;
anyID** result;
unsigned intts3client_getClientList(	serverConnectionHandlerID,	 
 	result);	 
uint64 serverConnectionHandlerID;
anyID** result;
        unsigned int ts3client_getChannelList(	serverConnectionHandlerID,	 
 	result);	 
uint64 serverConnectionHandlerID;
uint64** result;
         * * unsigned int ts3client_getServerConnectionHandlerList(	result);	 
uint64** result;

unsigned int ts3client_getClientSelfVariableAsInt(	serverConnectionHandlerID,	 
 	flag,	 
 	result);	 
uint64 serverConnectionHandlerID;
ClientProperties flag;
int* result;
 
unsigned int ts3client_getClientSelfVariableAsString(	serverConnectionHandlerID,	 
 	flag,	 
 	result);	 
uint64 serverConnectionHandlerID;
ClientProperties flag;
char** result;         
unsigned int ts3client_setClientSelfVariableAsInt(	serverConnectionHandlerID,	 
 	flag,	 
 	value);	 
uint64 serverConnectionHandlerID;
ClientProperties flag;
int value;
 
unsigned int ts3client_setClientSelfVariableAsString(	serverConnectionHandlerID,	 
 	flag,	 
 	value);	 
uint64 serverConnectionHandlerID;
ClientProperties flag;
const char* value;
 
        unsigned int ts3client_setChannelVariableAsInt(	serverConnectionHandlerID,	 
 	channelID,	 
 	flag,	 
 	value);	 
uint64 serverConnectionHandlerID;
uint64 channelID;
ChannelProperties flag;
int value;
 

unsigned int ts3client_setChannelVariableAsUInt64(	serverConnectionHandlerID,	 
 	channelID,	 
 	flag,	 
 	value);	 
uint64 serverConnectionHandlerID;
uint64 channelID;
ChannelProperties flag;
uint64 value;
 

unsigned int ts3client_setChannelVariableAsString(	serverConnectionHandlerID,	 
 	channelID,	 
 	flag,	 
 	value);	 
uint64 serverConnectionHandlerID;
uint64 channelID;
ChannelProperties flag;
const char* value;
unsigned int ts3client_getChannelVariableAsInt(	serverConnectionHandlerID,	 
 	channelID,	 
 	flag,	 
 	result);	 
uint64 serverConnectionHandlerID;
uint64 channelID;
ChannelProperties flag;
int* result;
 
unsigned int ts3client_getChannelVariableAsUInt64(	serverConnectionHandlerID,	 
 	channelID,	 
 	flag,	 
 	result);	 
uint64 serverConnectionHandlerID;
uint64 channelID;
ChannelProperties flag;
uint64* result;
 
unsigned int ts3client_getChannelVariableAsString(	serverConnectionHandlerID,	 
 	channelID,	 
 	flag,	 
 	result);	 
uint64 serverConnectionHandlerID;
uint64 channelID;
ChannelProperties flag;
char* result;
 
unsigned int ts3client_getServerVariableAsInt(	serverConnectionHandlerID,	 
 	flag,	 
 	result);	 
uint64 serverConnectionHandlerID;
VirtualServerProperties flag;
int* result;
 
unsigned int ts3client_getServerVariableAsUInt64(	serverConnectionHandlerID,	 
 	flag,	 
 	result);	 
uint64 serverConnectionHandlerID;
VirtualServerProperties flag;
uint64* result;
 

unsigned int ts3client_getServerVariableAsString(	serverConnectionHandlerID,	 
 	flag,	 
 	result);	 
uint64 serverConnectionHandlerID;
VirtualServerProperties flag;
char** result;
 unsigned int ts3client_getServerVariableAsInt(	serverConnectionHandlerID,	 
 	flag,	 
 	result);	 
uint64 serverConnectionHandlerID;
VirtualServerProperties flag;
int* result;
 
unsigned int ts3client_getServerVariableAsUInt64(	serverConnectionHandlerID,	 
 	flag,	 
 	result);	 
uint64 serverConnectionHandlerID;
VirtualServerProperties flag;
uint64* result;
 

unsigned int ts3client_getServerVariableAsString(	serverConnectionHandlerID,	 
 	flag,	 
 	result);	 
uint64 serverConnectionHandlerID;
VirtualServerProperties flag;
char** result;
unsigned int ts3client_requestClientMove(	serverConnectionHandlerID,	 
 	clientID,	 
 	newChannelID,	 
 	password,	 
 	returnCode);	 
uint64 serverConnectionHandlerID;
anyID clientID;
uint64 newChannelID;
const char* password;
const char* returnCode;
unsigned int ts3client_flushChannelCreation(	serverConnectionHandlerID,	 
 	channelParentID);	 
uint64 serverConnectionHandlerID;
uint64 channelParentID;
unsigned int ts3client_requestChannelDelete(	serverConnectionHandlerID,	 
 	channelID,	 
 	force,	 
 	returnCode);	 
uint64 serverConnectionHandlerID;
uint64 channelID;
int force;
const char* returnCode;
unsigned int ts3client_requestSendServerTextMsg(	serverConnectionHandlerID,	 
 	message,	 
 	returnCode);	 
uint64 serverConnectionHandlerID;
const char* message;
const char* returnCode;
unsigned int ts3client_requestSendServerTextMsg(	serverConnectionHandlerID,	 
 	message,	 
 	returnCode);	 
uint64 serverConnectionHandlerID;
const char* message;
const char* returnCode;
unsigned int ts3client_requestSendChannelTextMsg(	serverConnectionHandlerID,	 
 	message,	 
 	targetChannelID,	 
 	returnCode);	 
uint64 serverConnectionHandlerID;
const char* message;
anyID targetChannelID;
const char* returnCode;
unsigned int ts3client_requestSendPrivateTextMsg(	serverConnectionHandlerID,	 
 	message,	 
 	targetClientID,	 
 	returnCode);	 
uint64 serverConnectionHandlerID;
const char* message;
anyID targetClientID;
const char* returnCode;
unsigned int ts3client_requestClientKickFromChannel(	serverConnectionHandlerID,	 
 	clientID,	 
 	kickReason,	 
 	returnCode);	 
uint64 serverConnectionHandlerID;
anyID clientID;
const char* kickReason;
const char* returnCode;
 
unsigned int ts3client_requestClientKickFromServer(	serverConnectionHandlerID,	 
 	clientID,	 
 	kickReason,	 
 	returnCode);	 
uint64 serverConnectionHandlerID;
anyID clientID;
const char* kickReason;
const char* returnCode;
 
unsigned int ts3client_requestChannelSubscribe(	serverConnectionHandlerID,	 
 	channelIDArray,	 
 	returnCode);	 
uint64 serverConnectionHandlerID;
const uint64* channelIDArray;
const char* returnCode;
 
To unsubscribe from a list of channels (zero-terminated array of channel IDs) call:

unsigned int ts3client_requestChannelUnsubscribe(	serverConnectionHandlerID,	 
 	channelIDArray,	 
 	returnCode);	 
uint64 serverConnectionHandlerID;
const uint64* channelIDArray;
const char* returnCode;
 
To subscribe to all channels on the server call:

unsigned int ts3client_requestChannelSubscribeAll(	serverConnectionHandlerID,	 
 	returnCode);	 
uint64 serverConnectionHandlerID;
const char* returnCode;
 
To unsubscribe from all channels on the server call:

unsigned int ts3client_requestChannelUnsubscribeAll(	serverConnectionHandlerID,	 
 	returnCode);	 
uint64 serverConnectionHandlerID;
const char* returnCode;
 
To check if a channel is currently subscribed, check the channel property CHANNEL_FLAG_ARE_SUBSCRIBED with ts3client_getChannelVariableAsInt:

int isSubscribed;

if(ts3client_getChannelVariableAsInt(scHandlerID, channelID, CHANNEL_FLAG_ARE_SUBSCRIBED, &isSubscribed)
   != ERROR_ok) {
    /* Handle error /
}
The following event will be sent for each successfully subscribed channel:

void onChannelSubscribeEvent(	serverConnectionHandlerID,	 
 	channelID);	 
uint64 serverConnectionHandlerID;
uint64 channelID;
 
Provided for convinience, to mark the end of mulitple calls to onChannelSubscribeEvent when subscribing to several channels, this event is called:

void onChannelSubscribeFinishedEvent(	serverConnectionHandlerID);	 
uint64 serverConnectionHandlerID;
 
The following event will be sent for each successfully unsubscribed channel:

void onChannelUnsubscribeEvent(	serverConnectionHandlerID,	 
 	channelID);	 
uint64 serverConnectionHandlerID;
uint64 channelID;
 
Similar like subscribing, this event is a convinience callback to mark the end of multiple calls to onChannelUnsubscribeEvent:

void onChannelUnsubscribeFinishedEvent(	serverConnectionHandlerID);	 
uint64 serverConnectionHandlerID;
 
Once a channel has been subscribed or unsubscribed, the event onClientMoveSubscriptionEvent is sent for each client in the subscribed channel. The event is not to be confused with onClientMoveEvent, which is called for clients actively switching channels.

void onClientMoveSubscriptionEvent(	serverConnectionHandlerID,	 
 	clientID,	 
 	oldChannelID,	 
 	newChannelID,	 
 	visibility);	 
uint64 serverConnectionHandlerID;
anyID clientID;
uint64 oldChannelID;
uint64 newChannelID;
int visibility;

unsigned int ts3client_requestMuteClients(	serverConnectionHandlerID,	 
    clientIDArray,	 
    returnCode);	 
uint64 serverConnectionHandlerID;
const anyID* clientIDArray;
const char* returnCode;
 
To unmute one or more clients:

unsigned int ts3client_requestUnmuteClients(	serverConnectionHandlerID,	 
    clientIDArray,	 
    returnCode);	 
uint64 serverConnectionHandlerID;
const anyID* clientIDArray;
const char* returnCode;
         */
        #endregion
    }

	class Program {
		static void Main(string[] args) {
			/* Create struct for callback function pointers */
			ClientEventCallbackMapper cbs = new ClientEventCallbackMapper();
			client_callbackrare_struct cbs_rare = new client_callbackrare_struct(); // dummy

			cbs.onConnectStatusChangeEvent_delegate = new onConnectStatusChangeEvent_type(callback.onConnectStatusChangeEvent);
            cbs.onServerProtocolVersionEvent_delegate = new onServerProtocolVersionEvent_type(callback.onServerProtocolVersionEvent);
			cbs.onNewChannelEvent_delegate = new onNewChannelEvent_type(callback.onNewChannelEvent);
			cbs.onNewChannelCreatedEvent_delegate = new onNewChannelCreatedEvent_type(callback.onNewChannelCreatedEvent);
			cbs.onDelChannelEvent_delegate = new onDelChannelEvent_type(callback.onDelChannelEvent);
			cbs.onClientMoveEvent_delegate = new onClientMoveEvent_type(callback.onClientMoveEvent);
			cbs.onClientMoveSubscriptionEvent_delegate = new onClientMoveSubscriptionEvent_type(callback.onClientMoveSubscriptionEvent);
			cbs.onClientMoveTimeoutEvent_delegate = new onClientMoveTimeoutEvent_type(callback.onClientMoveTimeoutEvent);
			cbs.onTalkStatusChangeEvent_delegate = new onTalkStatusChangeEvent_type(callback.onTalkStatusChangeEvent);
            cbs.onIgnoredWhisperEvent_delegate = new onIgnoredWhisperEvent_type(callback.onIgnoredWhisperEvent);
			cbs.onServerErrorEvent_delegate = new onServerErrorEvent_type(callback.onServerErrorEvent);

            /* Initialize client lib with callbacks */
            /* Resource path points to the SDK\bin directory to locate the soundbackends folder when running from Visual Studio. */
            /* If you want to run directly from the SDK\bin directory, use an empty string instead to locate the soundbackends folder in the current directory. */
            uint error = CustomTS3Client.ts3client_initClientLib(ref cbs, ref cbs_rare, LogTypes.LogType_FILE | LogTypes.LogType_CONSOLE, null, "");
			if (error != Error.ok) {
				Console.WriteLine("Failed to init clientlib: {0}.", error);
				return;
			}

            ulong scHandlerID = 0;
			/* Spawn a new server connection handler using the default port and store the server ID */
			error = CustomTS3Client.ts3client_spawnNewServerConnectionHandler(0, out scHandlerID);
            if (error != Error.ok)
            {
				Console.WriteLine("Error spawning server connection handler: {0}", error);
				return;
			}

            /* Open default capture device */
            /* Passing empty string for mode and null or empty string for device will open the default device */
			error = CustomTS3Client.ts3client_openCaptureDevice(scHandlerID, "", null);
            if (error != Error.ok)
            {
				Console.WriteLine("Error opening capture device: {0}", error);
			}

            /* Open default playback device */
            /* Passing empty string for mode and NULL or empty string for device will open the default device */
            error = CustomTS3Client.ts3client_openPlaybackDevice(scHandlerID, "", null);
            if (error != Error.ok)
            {
				Console.WriteLine("Error opening playback device: {0}", error);
			}

			/* Create a new client identity */
			/* In your real application you should do this only once, store the assigned identity locally and then reuse it. */
			IntPtr identityPtr = IntPtr.Zero;
			error = CustomTS3Client.ts3client_createIdentity(out identityPtr);
            if (error != Error.ok)
            {
				Console.WriteLine("Error creating identity: {0}", error);
				return;
			}
			string identity = Marshal.PtrToStringAnsi(identityPtr);
			
			string defaultarray = "";
			/* Connect to server on localhost:9987 with nickname "client", no default channel, no default channel password and server password "secret" */
			//error = ts3client.ts3client_startConnection(scHandlerID, identity, "54.68.20.34", 9987, "win7", ref defaultarray, "", "secret");
            error = CustomTS3Client.ts3client_startConnection(scHandlerID, identity, "localhost", 9987, "win08r2", ref defaultarray, "", "secret");
            if (error != Error.ok)
            {
				Console.WriteLine("Error connecting to server: 0x{0:X4}", error);
				Console.ReadLine();
				return;
			}
			CustomTS3Client.ts3client_freeMemory(identityPtr);  /* Release dynamically allocated memory */

			Console.WriteLine("Client lib initialized and running");

			/* Query and print client lib version */
			IntPtr versionPtr = IntPtr.Zero;
			error = CustomTS3Client.ts3client_getClientLibVersion(out versionPtr);
            if (error != Error.ok)
            {
				Console.WriteLine("Failed to get clientlib version: {0}.", error);
				return;
			}
			string version = Marshal.PtrToStringAnsi(versionPtr);
			Console.WriteLine(version);
			CustomTS3Client.ts3client_freeMemory(versionPtr); /* Release dynamically allocated memory */

			Thread.Sleep(500);

			/* Wait for user input */
			Console.WriteLine("\n--- Press Return to disconnect from server and exit ---");
			Console.ReadLine();

			/* Disconnect from server */
			error = CustomTS3Client.ts3client_stopConnection(scHandlerID, "leaving");
            if (error != Error.ok)
            {
				Console.WriteLine("Error stopping connection: {0}", error);
				return;
			}

			Thread.Sleep(200);

			/* Destroy server connection handler */
			error = CustomTS3Client.ts3client_destroyServerConnectionHandler(scHandlerID);
            if (error != Error.ok)
            {
				Console.WriteLine("Error destroying clientlib: {0}", error);
				return;
			}

			/* Shutdown client lib */
			error = CustomTS3Client.ts3client_destroyClientLib();
            if (error != Error.ok)
            {
				Console.WriteLine("Failed to destroy clientlib: {0}", error);
				return;
			}

			Console.ReadLine();
		}
	}
}
