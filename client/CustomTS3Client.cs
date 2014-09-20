/*
 * TeamSpeak 3 client minimal sample C#
 *
 * Copyright (c) 2007-2010 TeamSpeak-Systems
 */

using System;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using teamspeak.definitions;

namespace teamspeak
{
    public class CustomTS3Client : TS3SDKConsumerBase
    {
        //Ferry: [WARNING] _mapper must be public or AccessViolation will throw
        public ClientEventCallbackMapper _mapper;
        //unless we want to connect more than one server simutaneouly, otherwise only one connection ID need to spawn,
        //the same connection ID can be reused to connect and disconnect different servers
        ulong _connectionID = 0;
        ulong _connectedServerID = 0;
        string _identity = string.Empty;
        ClientState _state = ClientState.STATE_NONE;

        #region Event Declarations
        public event ConnectStatusChangeEvent ConnectStatusChange;
        public event ServerProtocolVersionEvent ServerProtocolVersion;
        public event NewChannelEvent NewChannel;
        public event NewChannelCreatedEvent NewChannelCreated;
        public event DelChannelEvent DelChannel;
        public event ChannelMoveEvent ChannelMove;
        public event UpdateChannelEvent UpdateChannel;
        public event UpdateChannelEditedEvent UpdateChannelEdited;
        public event UpdateClientEvent UpdateClient;
        public event ClientMoveEvent ClientMove;
        public event ClientMoveSubscriptionEvent ClientMoveSubscription;
        public event ClientMoveTimeoutEvent ClientMoveTimeout;
        public event ClientMovedByOtherEvent ClientMovedByOther;
        public event ClientKickFromChannelEvent ClientKickFromChannel;
        public event ClientKickFromServerEvent ClientKickFromServer;
        public event ClientIDsEvent ClientIDs;
        public event ClientIDsFinishedEvent ClientIDsFinished;
        public event ServerEditedEvent ServerEdited;
        public event ServerUpdatedEvent ServerUpdated;
        public event ServerErrorEvent ServerError;
        public event ServerStopEvent ServerStop;
        public event TextMessageEvent TextMessage;
        public event TalkStatusChangeEvent TalkStatusChange;
        public event IgnoredWhisperEvent IgnoredWhisper;
        public event ConnectionInfoEvent ConnectionInfo;
        public event ServerConnectionInfoEvent ServerConnectionInfo;
        public event ChannelSubscribeEvent ChannelSubscribe;
        public event ChannelSubscribeFinishedEvent ChannelSubscribeFinished;
        public event ChannelUnsubscribeEvent ChannelUnsubscribe;
        public event ChannelUnsubscribeFinishedEvent ChannelUnsubscribeFinished;
        public event ChannelDescriptionUpdateEvent ChannelDescriptionUpdate;
        public event ChannelPasswordChangedEvent ChannelPasswordChanged;
        public event PlaybackShutdownCompleteEvent PlaybackShutdownComplete;
        public event SoundDeviceListChangedEvent SoundDeviceListChanged;
        public event EditPlaybackVoiceDataEvent EditPlaybackVoiceData;
        public event EditPostProcessVoiceDataEvent EditPostProcessVoiceData;
        public event EditMixedPlaybackVoiceDataEvent EditMixedPlaybackVoiceData;
        public event EditCapturedVoiceDataEvent EditCapturedVoiceData;
        public event Custom3dRolloffCalculationClientEvent Custom3dRolloffCalculationClient;
        public event Custom3dRolloffCalculationWaveEvent Custom3dRolloffCalculationWave;
        public event UserLoggingMessageEvent UserLoggingMessage;
        public event CustomPacketDecryptEvent CustomPacketDecrypt;
        public event CustomPacketEncryptEvent CustomPacketEncrypt;
        public event ProvisioningSlotRequestResultEvent ProvisioningSlotRequestResult;
        #endregion Event Declarations

        public ClientState CurrentState { get { return _state; } }

        protected override void initMapper()
        {
            _mapper = new ClientEventCallbackMapper();
            _mapper.onTextMessage = onTextMessage;
            _mapper.onConnectStatusChange = onConnectStatusChange;
            _mapper.onServerProtocolVersion = onServerProtocolVersion;
            _mapper.onNewChannel = onGettingExistingChannelHirachry;
            _mapper.onNewChannelCreated = onNewChannelCreated;
            _mapper.onDelChannel = onDelChannel;
            _mapper.onClientMove = onClientMove;
            _mapper.onClientMoveSubscription = onClientMoveSubscription;
            _mapper.onClientMoveTimeout = onClientMoveTimeout;
            _mapper.onTalkStatusChange = onTalkStatusChange;
            _mapper.onIgnoredWhisper = onIgnoredWhisper;
            _mapper.onServerError = onServerError;
            _mapper.onServerStop = onServerStop;
        }

        private void onTextMessage(ulong serverID, TextMessageTargetMode targetMode, ushort toID, ushort fromID, string fromName, string fromUniqueIdentifier, string message)
        {
            if (TextMessage != null)
                TextMessage(serverID, targetMode, toID, fromID, fromName, fromUniqueIdentifier, message);
        }

        #region Event Handlers

        void onConnectStatusChange(ulong serverID, ConnectStatus newStatus, uint errorNumber)
        {
            if (newStatus == ConnectStatus.STATUS_CONNECTED)
            {
                _state = ClientState.STATE_CONNECTED;
                _connectedServerID = serverID;
            }

            notify(string.Format("Connect status changed: {0} {1} {2}", serverID, newStatus.ToString(), errorNumber));
            /* Failed to connect ? */
            if (newStatus == ConnectStatus.STATUS_DISCONNECTED && errorNumber == Error.failed_connection_initialisation)
            {
                notifyError(string.Format("Looks like there is no server running, terminate!"));
                return;
            }
            if (ConnectStatusChange != null)
                ConnectStatusChange(serverID, newStatus, errorNumber);
        }

        void onServerProtocolVersion(ulong serverID, int protocolVersion)
        {
            notify(string.Format("Server protocol version: {0} {1}", serverID, protocolVersion));
            if (ServerProtocolVersion != null)
                ServerProtocolVersion(serverID, protocolVersion);
        }
        //void onNewChannelEvent <- confusing name
        // this will call several times when client connect to get the whole existing channel hierarchy
        // in parent-child style pair by pair
        void onGettingExistingChannelHirachry(ulong serverID, ulong channelID, ulong channelParentID)
        {
            notify(string.Format("onNewChannelEvent: {0} {1} {2}", serverID, channelID, channelParentID));

            //uint result;
            //IntPtr namePtr = IntPtr.Zero;
            //if ((result = GetChannelVariableAsString(serverID, channelID, ChannelProperties.CHANNEL_NAME, out namePtr)) == Error.ok)
            //    notify(string.Format("New channel: {0} {1}", channelID, getStringFromPointer(namePtr)));
            string value = getStringVariable(channelID, ChannelProperties.CHANNEL_NAME);
            notify(string.Format("New channel: {0} {1}", channelID, value));
            //else
            //{
            //    IntPtr errorMsgPtr = IntPtr.Zero;
            //    if (GetErrorMessage(result, errorMsgPtr) == Error.ok)
            //        notifyError(string.Format("Error getting channel name in onNewChannelEvent: {0}", getStringFromPointer(errorMsgPtr)));
            //}
            if (NewChannel != null)
                NewChannel(serverID, channelID, channelParentID);
        }
        
        void onNewChannelCreated(ulong serverID, ulong channelID, ulong channelParentID, ushort invokerID, string invokerName, string invokerUniqueIdentifier)
        {
            //IntPtr namePtr = IntPtr.Zero;
            ///* Query channel name from channel ID */
            //uint result = Error.ok;
            //if ((result = GetChannelVariableAsString(serverID, channelID, ChannelProperties.CHANNEL_NAME, out namePtr)) != Error.ok)
            //{
            //    notifyError(string.Format("Error reading client variable {0}", ChannelProperties.CHANNEL_NAME.ToString()));
            //    return;
            //}
            string value = getStringVariable(channelID, ChannelProperties.CHANNEL_NAME);
            //notify(string.Format("New channel created: {0}", getStringFromPointer(namePtr)));
            notify(string.Format("New channel created: {0}", value));
            if (NewChannelCreated != null)
                NewChannelCreated(serverID, channelID, channelParentID, invokerID, invokerName, invokerUniqueIdentifier);
        }

        void onDelChannel(ulong serverID, ulong channelID, ushort invokerID, string invokerName, string invokerUniqueIdentifier)
        {
            notify(string.Format("Channel ID {0} deleted by {1} ({2})", channelID, invokerName, invokerID));
            if (DelChannel != null)
                DelChannel(serverID, channelID, invokerID, invokerName, invokerUniqueIdentifier);
        }

        void onClientMove(ulong serverID, ushort clientID, ulong oldChannelID, ulong newChannelID, int visibility, string moveMessage)
        {
            notify(string.Format("ClientID {0} moves from channel {1} to {2} with message {3}", clientID, oldChannelID, newChannelID, moveMessage));
            if (ClientMove != null)
                ClientMove(serverID, clientID, oldChannelID, newChannelID, visibility, moveMessage);
        }

        void onClientMoveSubscription(ulong serverID, ushort clientID, ulong oldChannelID, ulong newChannelID, int visibility)
        {
            IntPtr namePtr = IntPtr.Zero;
            /* Query client nickname from ID */
            uint result = Error.ok;
            if ((result = GetClientVariableAsString(serverID, clientID, ClientProperties.CLIENT_NICKNAME, out namePtr)) != Error.ok)
            {
                notifyError(string.Format("Error reading client variable {0}", ClientProperties.CLIENT_NICKNAME.ToString()));
                return;
            }
            notify(string.Format("New client: {0}", getStringFromPointer(namePtr)));

            if (ClientMoveSubscription != null)
                ClientMoveSubscription(serverID, clientID, oldChannelID, newChannelID, visibility);
        }

        void onClientMoveTimeout(ulong serverID, ushort clientID, ulong oldChannelID, ulong newChannelID, int visibility, string timeoutMessage)
        {
            notify(string.Format("ClientID {0} timeouts with message {1}", clientID, timeoutMessage));

            if (ClientMoveTimeout != null)
                ClientMoveTimeout(serverID, clientID, oldChannelID, newChannelID, visibility, timeoutMessage);
        }

        void onTalkStatusChange(ulong serverID, int status, int isReceivedWhisper, ushort clientID)
        {
            IntPtr namePtr = IntPtr.Zero;
            /* Query client nickname from ID */
            uint result = Error.ok;
            if((result = GetClientVariableAsString(serverID, clientID, ClientProperties.CLIENT_NICKNAME, out namePtr)) != Error.ok)
            {
                notifyError(string.Format("Error reading client variable {0}", ClientProperties.CLIENT_NICKNAME.ToString()));
                return;
            }
            notify(string.Format("Client \"{0}\" {1} talking.", getStringFromPointer(namePtr), status == (int)TalkStatus.STATUS_TALKING ? "start" : "stop"));

            if (TalkStatusChange != null)
                TalkStatusChange(serverID, status, isReceivedWhisper, clientID);
        }

        void onIgnoredWhisper(ulong serverID, ushort clientID)
        {
            notify(string.Format("Ignored whisper: {0} {1}", serverID, clientID));
            if (IgnoredWhisper != null)
                IgnoredWhisper(serverID, clientID);
        }

        void onServerError(ulong serverID, string errorMessage, uint error, string returnCode, string extraMessage)
        {
            notify(string.Format("Error for server {0}: {1}", serverID, errorMessage));
            if (ServerError != null)
                ServerError(serverID, errorMessage, error, returnCode, extraMessage);
        }

        void onServerStop(ulong serverID, string shutdownMessage)
        {
            notify(string.Format("Server {0} stopping: {1}", serverID, shutdownMessage));
            if (ServerStop != null)
                ServerStop(serverID, shutdownMessage);
        }
        #endregion Event Handlers

        public void connect(string nickName = "win7", string defaultChannel = "", string channelPassword = "",  
            string serverIP = "localhost", string serverPassword = "secret", uint serverPort = 9987)
        {
            if (_state == ClientState.STATE_NONE)
                initClientLib();
            else if (_state == ClientState.STATE_CONNECTED)
            {
                notifyError("Already connected to a server.");
                return;
            }
            /* Connect to server on localhost:9987 with nickname "client", no default channel, no default channel password and server password "secret" */
            //error = ts3client.ts3client_startConnection(scHandlerID, identity, "54.68.20.34", 9987, "win7", ref defaultarray, "", "secret");
            uint result = Error.ok;
            string[] channels = {defaultChannel, string.Empty};
            if ((result = StartConnection(_connectionID, _identity, serverIP, serverPort, nickName, channels, channelPassword, serverPassword)) != Error.ok)
            {
                notifyError(string.Format("Error connecting to server: 0x{0:X4}", result));
                return;
            }

        }
        void initClientLib()
        {
            /* Initialize client lib with callbacks */
            /* Resource path points to the SDK\bin directory to locate the soundbackends folder when running from Visual Studio. */
            /* If you want to run directly from the SDK\bin directory, use an empty string instead to locate the soundbackends folder in the current directory. */
            uint result = Error.ok;
            if ((result = InitClientLib(ref this._mapper, null, LogTypes.LogType_FILE | LogTypes.LogType_CONSOLE, null, "")) != Error.ok)
            {
                notifyError(string.Format("Failed to init clientlib: {0}.", result));
                return;
            }
            /* Spawn a new server connection handler using the default port and store the server ID */
            if ((result = SpawnNewServerConnectionHandler(0, out _connectionID)) != Error.ok)
            {
                notifyError(string.Format("Error spawning server connection handler: {0}", result));
                return;
            }
            /* Open default capture device */
            /* Passing empty string for mode and null or empty string for device will open the default device */
            if ((result = OpenCaptureDevice(_connectionID, "", null)) != Error.ok)
            {
                notifyError(string.Format("Error opening capture device: {0}", result));
                //return;
            }
            /* Open default playback device */
            /* Passing empty string for mode and NULL or empty string for device will open the default device */
            if ((result = OpenPlaybackDevice(_connectionID, "", null)) != Error.ok)
            {
                notifyError(string.Format("Error opening playback device: {0}", result));
                //return;
            }

            /* Create a new client identity */
            /* In your real application you should do this only once, store the assigned identity locally and then reuse it. */
            IntPtr identityPtr = IntPtr.Zero;
            if ((result = CreateIdentity(out identityPtr)) != Error.ok)
            {
                notifyError(string.Format("Error creating identity: {0}", result));
                return;
            }
            _identity = getStringFromPointer(identityPtr);
            notify("identity:" + _identity);

            /* Query and print client lib version */
            IntPtr versionPtr = IntPtr.Zero;
            if ((result = GetClientLibVersion(out versionPtr)) != Error.ok)
            {
                notifyError(string.Format("Failed to get clientlib version: {0}.", result));
                return;
            }
            string version = getStringFromPointer(versionPtr);
            notify(version);
            notify("Client lib initialized and running");
        }
        public void disconnect()
        {
            if(_state != ClientState.STATE_CONNECTED)
            {
                notifyError("No server connected.");
                return;
            }
            /* Disconnect from server */
            uint result = Error.ok;
            if ((result = StopConnection(_connectionID, "leaving")) != Error.ok)
            {
                notifyError(string.Format("Error stopping connection: {0}", result));
                return;
            }
            _state = ClientState.STATE_DISCONNECTED;
        }
        public void kill()
        {
            if (_state == ClientState.STATE_CONNECTED)
            {
                notifyError("Please disconnect first.");
                return;
            }
            else if(_state == ClientState.STATE_NONE)
            {
                notifyError("Nothing to kill.");
                return;
            }

            /* Destroy server connection handler */
            uint result = Error.ok;
            if ((result = DestroyServerConnectionHandler(_connectionID)) != Error.ok)
            {
                notifyError(string.Format("Error destroying clientlib: {0}", result));
                return;
            }

            /* Shutdown client lib */
            if ((result = DestroyClientLib()) != Error.ok)
            {
                notifyError(string.Format("Failed to destroy clientlib: {0}", result));
                return;
            }
            _state = ClientState.STATE_NONE;
        }
        public bool tell(string message, TextMessageTargetMode mode, ulong target = 0)
        {
            if(_state != ClientState.STATE_CONNECTED)
            {
                notifyError("Not connected.");
                return false;
            }
            uint result = Error.ok;
            switch(mode)
            {
                case TextMessageTargetMode.TextMessageTarget_SERVER:
                    result = RequestSendServerTextMsg(_connectedServerID, message, null);
                    break;
                case TextMessageTargetMode.TextMessageTarget_CHANNEL:
                    result = RequestSendChannelTextMsg(_connectedServerID, message, target, null);
                    break;
                case TextMessageTargetMode.TextMessageTarget_CLIENT:
                    result = RequestSendPrivateTextMsg(_connectedServerID, message, (ushort)target, null);
                    break;
                default: break;
            }
            if (result != Error.ok)
            {
                notifyError("Error senting message.");
                return false;
            }
            else
                notify("Message sent.");
            return true;
        }
        public bool setStringVariable(ulong serverID, ulong channelID, ChannelProperties property, string value)
        {
            if (SetChannelVariableAsString(_connectedServerID, channelID, property, value) != Error.ok)
            {
                notifyError(string.Format("Error setting variable: {0}", property.ToString()));
                return false;
            }
            return true;
        }
        public bool setStringVariable(ulong serverID, ulong channelID, ClientProperties property, string value)
        {
            if (SetClientSelfVariableAsString(_connectedServerID, property, value) != Error.ok)
            {
                notifyError(string.Format("Error setting variable: {0}", property.ToString()));
                return false;
            }
            return true;
        }
        public bool setIntVariable(ulong serverID, ulong channelID, ChannelProperties property, int value)
        {
            if (SetChannelVariableAsInt(_connectedServerID, channelID, property, value) != Error.ok)
            {
                notifyError(string.Format("Error setting variable: {0}", property.ToString()));
                return false;
            }
            return true;
        }
        public bool setIntVariable(ulong serverID, ulong channelID, ClientProperties property, int value)
        {
            if (SetClientSelfVariableAsInt(_connectedServerID, property, value) != Error.ok)
            {
                notifyError(string.Format("Error setting variable: {0}", property.ToString()));
                return false;
            }
            return true;
        }
        public string getStringVariable(ulong serverID, VirtualServerProperties property)
        {
            IntPtr valuePtr = IntPtr.Zero;
            uint result = Error.ok;
            if ((result = GetServerVariableAsString(_connectedServerID, property, out valuePtr)) != Error.ok)
            {
                notifyError(string.Format("Error getting variable: {0}", property.ToString()));
                return string.Empty;
            }
            return getStringFromPointer(valuePtr);
        }
        public string getStringVariable(ulong channelID, ChannelProperties property)
        {
            IntPtr valuePtr = IntPtr.Zero;
            uint result = Error.ok;
            if ((result = GetChannelVariableAsString(_connectedServerID, channelID, property, out valuePtr)) != Error.ok)
            {
                notifyError(string.Format("Error getting variable: {0}", property.ToString()));
                return string.Empty;
            }
            return getStringFromPointer(valuePtr);
        }
        public string getStringVariable(ushort clientID, ClientProperties property)
        {
            IntPtr valuePtr = IntPtr.Zero;
            uint result = Error.ok;
            if((result = GetClientVariableAsString(_connectedServerID, clientID, property, out valuePtr)) != Error.ok)
            {
                notifyError(string.Format("Error getting variable: {0}", property.ToString()));
                return string.Empty;
            }
            return getStringFromPointer(valuePtr);
        }
        public string getStringVariable(ClientProperties property)
        {
            IntPtr valuePtr = IntPtr.Zero;
            uint result = Error.ok;
            if ((result = GetClientSelfVariableAsString(_connectedServerID, property, out valuePtr)) != Error.ok)
            {
                notifyError(string.Format("Error getting variable: {0}", property.ToString()));
                return string.Empty;
            }
            return getStringFromPointer(valuePtr);
        }
        public int getIntVariable(ulong serverID, VirtualServerProperties property)
        {
            int value = 0;
            if (GetServerVariableAsInt(_connectedServerID, property, out value) != Error.ok)
            {
                notifyError(string.Format("Error getting variable: {0}", property.ToString()));
                return 0;
            }
            return value;
        }
        public int getIntVariable(ulong channelID, ChannelProperties property)
        {
            int value = 0;
            if (GetChannelVariableAsInt(_connectedServerID, channelID, property, out value) != Error.ok)
            {
                notifyError(string.Format("Error getting variable: {0}", property.ToString()));
                return 0;
            }
            return value;
        }
        public int getIntVariable(ushort clientID, ClientProperties property)
        {
            int value = 0;
            if (GetClientVariableAsInt(_connectedServerID, clientID, property, out value) != Error.ok)
            {
                notifyError(string.Format("Error getting variable: {0}", property.ToString()));
                return 0;
            }
            return value;
        }
        public int getIntVariable(ClientProperties property)
        {
            int value = 0;
            if (GetClientSelfVariableAsInt(_connectedServerID, property, out value) != Error.ok)
            {
                notifyError(string.Format("Error getting variable: {0}", property.ToString()));
                return 0;
            }
            return value;
        }
        #region Utilities
        private string getStringFromPointer(IntPtr pointer)
        {
            string temp = string.Empty;
            temp = Marshal.PtrToStringAnsi(pointer);
            FreeMemory(pointer); /* Release dynamically allocated memory */
            return temp;
        }
        #endregion Utilities

        #region DLL Import

#if x64
        private const string DLL_FILE_NAME = "ts3client_win64.dll";
#else
        string DLL_FILE_NAME = "ts3client_win64.dll";
#endif
        //All copied from clientlib.h so no need to paste source here
        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_freeMemory")]
        static extern uint FreeMemory(IntPtr pointer);

        /*Construction and Destruction*/

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_initClientLib")]
        static extern uint InitClientLib(ref ClientEventCallbackMapper functionPointers, object functionRarePointers, LogTypes usedLogTypes, string logFileFolder, string resourcesFolder);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_destroyClientLib")]
        static extern uint DestroyClientLib();

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_getClientLibVersion")]
        static extern uint GetClientLibVersion(out IntPtr result);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_getClientLibVersionNumber")]
        static extern uint GetClientLibVersionNumber(out ulong result);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_spawnNewServerConnectionHandler")]
        static extern uint SpawnNewServerConnectionHandler(int port, out ulong result);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_destroyServerConnectionHandler")]
        static extern uint DestroyServerConnectionHandler(ulong serverID);

        /*Identity management*/

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_createIdentity")]
        static extern uint CreateIdentity(out IntPtr result);

        /*sound*/

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_getPlaybackDeviceList")]
        //static extern uint GetPlaybackDeviceList(string modeID, out string[][] result);
        static extern uint GetPlaybackDeviceList(string modeID, out IntPtr result);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_getCaptureDeviceList")]
        //static extern uint GetCaptureDeviceList(string modeID, out string[][] result);
        static extern uint GetCaptureDeviceList(string modeID, out IntPtr result);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_getPlaybackModeList")]
        //static extern uint GetPlaybackModeList(out string[] result);
        static extern uint GetPlaybackModeList(out IntPtr result);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_getCaptureModeList")]
        //static extern uint GetCaptureModeList(out string[] result);
        static extern uint GetCaptureModeList(out IntPtr result);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_getDefaultPlaybackDevice")]
        //static extern uint GetDefaultPlaybackDevice(string modeID, out string[] result);
        static extern uint GetDefaultPlaybackDevice(string modeID, out IntPtr result);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_getDefaultCaptureDevice")]
        //static extern uint GetDefaultCaptureDevice(string modeID, out string[] result);
        static extern uint GetDefaultCaptureDevice(string modeID, out IntPtr result);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_getDefaultPlayBackMode")]
        //static extern uint GetDefaultPlayBackMode(out string result);
        static extern uint GetDefaultPlayBackMode(out IntPtr result);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_getDefaultCaptureMode")]
        //static extern uint GetDefaultCaptureMode(out string result);
        static extern uint GetDefaultCaptureMode(out IntPtr result);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_openPlaybackDevice")]
        static extern uint OpenPlaybackDevice(ulong serverID, string modeID, string playbackDevice);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_openCaptureDevice")]
        static extern uint OpenCaptureDevice(ulong serverID, string modeID, string captureDevice);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_getCurrentPlaybackDeviceName")]
        //static extern uint GetCurrentPlaybackDeviceName(ulong serverID, out string result, out int isDefault);
        static extern uint GetCurrentPlaybackDeviceName(ulong serverID, out IntPtr result, out int isDefault);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_getCurrentPlayBackMode")]
        //static extern uint GetCurrentPlayBackMode(ulong serverID, out string result);
        static extern uint GetCurrentPlayBackMode(ulong serverID, out IntPtr result);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_getCurrentCaptureDeviceName")]
        //static extern uint GetCurrentCaptureDeviceName(ulong serverID, out string result, out int isDefault);
        static extern uint GetCurrentCaptureDeviceName(ulong serverID, out IntPtr result, out int isDefault);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_getCurrentCaptureMode")]
        //static extern uint GetCurrentCaptureMode(ulong serverID, out string result);
        static extern uint GetCurrentCaptureMode(ulong serverID, out IntPtr result);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_initiateGracefulPlaybackShutdown")]
        static extern uint InitiateGracefulPlaybackShutdown(ulong serverID);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_closePlaybackDevice")]
        static extern uint ClosePlaybackDevice(ulong serverID);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_closeCaptureDevice")]
        static extern uint CloseCaptureDevice(ulong serverID);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_activateCaptureDevice")]
        static extern uint ActivateCaptureDevice(ulong serverID);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_playWaveFile")]
        static extern uint PlayWaveFile(ulong serverID, string path);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_playWaveFileHandle")]
        static extern uint PlayWaveFileHandle(ulong serverID, string path, int loop, out ulong waveHandle);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_pauseWaveFileHandle")]
        static extern uint PauseWaveFileHandle(ulong serverID, ulong waveHandle, int pause);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_closeWaveFileHandle")]
        static extern uint CloseWaveFileHandle(ulong serverID, ulong waveHandle);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_registerCustomDevice")]
        static extern uint RegisterCustomDevice(string deviceID, string deviceDisplayName, int capFrequency, int capChannels, int playFrequency, int playChannels);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_unregisterCustomDevice")]
        static extern uint UnregisterCustomDevice(string deviceID);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_processCustomCaptureData")]
        static extern uint ProcessCustomCaptureData(string deviceName, short[] buffer, int samples);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_acquireCustomPlaybackData")]
        static extern uint AcquireCustomPlaybackData(string deviceName, short[] buffer, int samples);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_setLocalTestMode")]
        static extern uint SetLocalTestMode(ulong serverID, int status);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_startVoiceRecording")]
        static extern uint StartVoiceRecording(ulong serverID);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_stopVoiceRecording")]
        static extern uint StopVoiceRecording(ulong serverID);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_allowWhispersFrom")]
        static extern uint AllowWhispersFrom(ulong serverID, ushort clID);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_removeFromAllowedWhispersFrom")]
        static extern uint RemoveFromAllowedWhispersFrom(ulong serverID, ushort clID);

        /* 3d sound positioning */

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_systemset3DListenerAttributes")]
        static extern uint Systemset3DListenerAttributes(ulong serverID, TS3_VECTOR position, TS3_VECTOR forward, TS3_VECTOR up);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_set3DWaveAttributes")]
        static extern uint Set3DWaveAttributes(ulong serverID, ulong waveHandle, TS3_VECTOR position);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_systemset3DSettings")]
        static extern uint Systemset3DSettings(ulong serverID, float distanceFactor, float rolloffScale);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_channelset3DAttributes")]
        static extern uint Channelset3DAttributes(ulong serverID, ushort clientID, TS3_VECTOR position);

        /*preprocessor*/

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_getPreProcessorInfoValueFloat")]
        static extern uint GetPreProcessorInfoValueFloat(ulong serverID, string ident, out float result);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_getPreProcessorConfigValue")]
        //static extern uint GetPreProcessorConfigValue(ulong serverID, string ident, out string result);
        static extern uint GetPreProcessorConfigValue(ulong serverID, string ident, out IntPtr result);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_setPreProcessorConfigValue")]
        static extern uint SetPreProcessorConfigValue(ulong serverID, string ident, string value);

        /*encoder*/

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_getEncodeConfigValue")]
        //static extern uint GetEncodeConfigValue(ulong serverID, string ident, out string result);
        static extern uint GetEncodeConfigValue(ulong serverID, string ident, out IntPtr result);

        /*playback*/

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_getPlaybackConfigValueAsFloat")]
        static extern uint GetPlaybackConfigValueAsFloat(ulong serverID, string ident, out float result);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_setPlaybackConfigValue")]
        static extern uint SetPlaybackConfigValue(ulong serverID, string ident, string value);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_setClientVolumeModifier")]
        static extern uint SetClientVolumeModifier(ulong serverID, ushort clientID, float value);

        /*logging*/

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_logMessage")]
        static extern uint LogMessage(string logMessage, LogLevel severity, string channel, ulong logID);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_setLogVerbosity")]
        static extern uint SetLogVerbosity(LogLevel logVerbosity);

        /*error handling*/

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_getErrorMessage")]
        static extern uint GetErrorMessage(uint errorCode, IntPtr error);

        /*Interacting with the server*/

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_startConnection", CharSet = CharSet.Ansi)]
        static extern uint StartConnection(ulong serverID, string identity, string ip, uint port, string nickname, string[] defaultChannelArray, string defaultChannelPassword, string serverPassword);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_stopConnection")]
        static extern uint StopConnection(ulong serverID, string quitMessage);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_requestClientMove")]
        static extern uint RequestClientMove(ulong serverID, ushort clientID, ulong newChannelID, string password, string returnCode);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_requestClientVariables")]
        static extern uint RequestClientVariables(ulong serverID, ushort clientID, string returnCode);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_requestClientKickFromChannel")]
        static extern uint RequestClientKickFromChannel(ulong serverID, ushort clientID, string kickReason, string returnCode);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_requestClientKickFromServer")]
        static extern uint RequestClientKickFromServer(ulong serverID, ushort clientID, string kickReason, string returnCode);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_requestChannelDelete")]
        static extern uint RequestChannelDelete(ulong serverID, ulong channelID, int force, string returnCode);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_requestChannelMove")]
        static extern uint RequestChannelMove(ulong serverID, ulong channelID, ulong newChannelParentID, ulong newChannelOrder, string returnCode);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_requestSendPrivateTextMsg")]
        static extern uint RequestSendPrivateTextMsg(ulong serverID, string message, ushort targetClientID, string returnCode);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_requestSendChannelTextMsg")]
        static extern uint RequestSendChannelTextMsg(ulong serverID, string message, ulong targetChannelID, string returnCode);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_requestSendServerTextMsg")]
        static extern uint RequestSendServerTextMsg(ulong serverID, string message, string returnCode);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_requestConnectionInfo")]
        static extern uint RequestConnectionInfo(ulong serverID, ushort clientID, string returnCode);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_requestClientSetWhisperList")]
        static extern uint RequestClientSetWhisperList(ulong serverID, ushort clientID, ulong[] targetChannelIDArray, ushort[] targetClientIDArray, string returnCode);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_requestChannelSubscribe")]
        static extern uint RequestChannelSubscribe(ulong serverID, ulong[] channelIDArray, string returnCode);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_requestChannelSubscribeAll")]
        static extern uint RequestChannelSubscribeAll(ulong serverID, string returnCode);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_requestChannelUnsubscribe")]
        static extern uint RequestChannelUnsubscribe(ulong serverID, ulong[] channelIDArray, string returnCode);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_requestChannelUnsubscribeAll")]
        static extern uint RequestChannelUnsubscribeAll(ulong serverID, string returnCode);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_requestChannelDescription")]
        static extern uint RequestChannelDescription(ulong serverID, ulong channelID, string returnCode);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_requestMuteClients")]
        static extern uint RequestMuteClients(ulong serverID, ushort[] clientIDArray, string returnCode);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_requestUnmuteClients")]
        static extern uint RequestUnmuteClients(ulong serverID, ushort[] clientIDArray, string returnCode);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_requestClientIDs")]
        static extern uint RequestClientIDs(ulong serverID, string clientUniqueIdentifier, string returnCode);

        /*provisioning server calls*/

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_requestSlotsFromProvisioningServer")]
        static extern uint RequestSlotsFromProvisioningServer(string ip, ushort port, string serverPassword, ushort slots, string identity, string region, out ulong requestHandle);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_cancelRequestSlotsFromProvisioningServer")]
        static extern uint CancelRequestSlotsFromProvisioningServer(ulong requestHandle);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_startConnectionWithProvisioningKey")]
        static extern uint StartConnectionWithProvisioningKey(ulong serverID, string identity, string nickname, string connectionKey);

        /*retrieve information ClientLib has stored*/

        /*general info*/

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_getClientID")]
        static extern uint GetClientID(ulong serverID, out ushort result);

        /*client connection info*/

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_getConnectionStatus")]
        static extern uint GetConnectionStatus(ulong serverID, out int result);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_getConnectionVariableAsUInt64")]
        static extern uint GetConnectionVariableAsulong(ulong serverID, ushort clientID, ConnectionProperties flag, out ulong result);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_getConnectionVariableAsDouble")]
        static extern uint GetConnectionVariableAsDouble(ulong serverID, ushort clientID, ConnectionProperties flag, out double result);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_getConnectionVariableAsString")]
        //static extern uint GetConnectionVariableAsString(ulong serverID, ushort clientID, ConnectionProperties flag, out string result);
        static extern uint GetConnectionVariableAsString(ulong serverID, ushort clientID, ConnectionProperties flag, out IntPtr result);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_cleanUpConnectionInfo")]
        static extern uint CleanUpConnectionInfo(ulong serverID, ushort clientID);

        /*server connection info*/

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_requestServerConnectionInfo")]
        static extern uint RequestServerConnectionInfo(ulong serverID, string returnCode);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_getServerConnectionVariableAsUInt64")]
        static extern uint GetServerConnectionVariableAsulong(ulong serverID, ConnectionProperties flag, out ulong result);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_getServerConnectionVariableAsFloat")]
        static extern uint GetServerConnectionVariableAsFloat(ulong serverID, ConnectionProperties flag, out float result);

        /*client info*/

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_getClientSelfVariableAsInt")]
        static extern uint GetClientSelfVariableAsInt(ulong serverID, ClientProperties flag, out int result);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_getClientSelfVariableAsString")]
        static extern uint GetClientSelfVariableAsString(ulong serverID, ClientProperties flag, out IntPtr result);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_setClientSelfVariableAsInt")]
        static extern uint SetClientSelfVariableAsInt(ulong serverID, ClientProperties flag, int value);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_setClientSelfVariableAsString")]
        static extern uint SetClientSelfVariableAsString(ulong serverID, ClientProperties flag, string value);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_flushClientSelfUpdates")]
        static extern uint FlushClientSelfUpdates(ulong serverID, string returnCode);

        //Ferry: ClientVariables => other clients, ClientSelfVariables => self client, so no setter and flush for other clients
        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_getClientVariableAsInt")]
        static extern uint GetClientVariableAsInt(ulong serverID, ushort clientID, ClientProperties flag, out int result);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_getClientVariableAsUInt64")]
        static extern uint GetClientVariableAsulong(ulong serverID, ushort clientID, ClientProperties flag, out ulong result);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_getClientVariableAsString")]
        static extern uint GetClientVariableAsString(ulong serverID, ushort clientID, ClientProperties flag, out IntPtr result);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_getClientList")]
        static extern uint GetClientList(ulong serverID, out ushort[] result);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_getChannelOfClient")]
        static extern uint GetChannelOfClient(ulong serverID, ushort clientID, out ulong result);

        /*channel info*/

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_getChannelVariableAsInt")]
        static extern uint GetChannelVariableAsInt(ulong serverID, ulong channelID, ChannelProperties flag, out int result);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_getChannelVariableAsUInt64")]
        static extern uint GetChannelVariableAsulong(ulong serverID, ulong channelID, ChannelProperties flag, out ulong result);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_getChannelVariableAsString")]
        //static extern uint GetChannelVariableAsString(ulong serverID, ulong channelID, ChannelProperties flag, out IntPtr result);
        static extern uint GetChannelVariableAsString(ulong serverID, ulong channelID, ChannelProperties flag, out IntPtr result);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_getChannelIDFromChannelNames")]
        static extern uint GetChannelIDFromChannelNames(ulong serverID, out IntPtr channelNameArray, out ulong result);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_setChannelVariableAsInt")]
        static extern uint SetChannelVariableAsInt(ulong serverID, ulong channelID, ChannelProperties flag, int value);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_setChannelVariableAsUInt64")]
        static extern uint SetChannelVariableAsulong(ulong serverID, ulong channelID, ChannelProperties flag, ulong value);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_setChannelVariableAsString")]
        static extern uint SetChannelVariableAsString(ulong serverID, ulong channelID, ChannelProperties flag, string value);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_flushChannelUpdates")]
        static extern uint FlushChannelUpdates(ulong serverID, ulong channelID);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_flushChannelCreation")]
        static extern uint FlushChannelCreation(ulong serverID, ulong channelParentID);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_getChannelList")]
        static extern uint GetChannelList(ulong serverID, out ulong[] result);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_getChannelClientList")]
        static extern uint GetChannelClientList(ulong serverID, ulong channelID, out ushort[] result);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_getParentChannelOfChannel")]
        static extern uint GetParentChannelOfChannel(ulong serverID, ulong channelID, out ulong result);

        /*server info*/

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_getServerConnectionHandlerList")]
        static extern uint GetServerConnectionHandlerList(out ulong[] result);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_getServerVariableAsInt")]
        static extern uint GetServerVariableAsInt(ulong serverID, VirtualServerProperties flag, out int result);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_getServerVariableAsUInt64")]
        static extern uint GetServerVariableAsulong(ulong serverID, VirtualServerProperties flag, out ulong result);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_getServerVariableAsString")]
        static extern uint GetServerVariableAsString(ulong serverID, VirtualServerProperties flag, out IntPtr result);

        [DllImport(DLL_FILE_NAME, EntryPoint = "ts3client_requestServerVariables")]
        static extern uint RequestServerVariables(ulong serverID);

        #endregion DLL Import
    }
}