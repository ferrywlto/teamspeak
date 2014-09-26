using System;
using System.IO;
using System.Runtime.InteropServices;
using teamspeak.definitions;

/* Ferry: This is the place to wire-up server event to actual handlers
 * i.e. the UI, such that all object that are interested in server event
 * can register the listener.
 */

public class TS3CustomServer : TS3SDKConsumerBase
{
    //Ferry: Below are our own Delegates
    //public delegate void CustomServerMessageEvent(string message);

    //Ferry: [WARNING] _mapper must be public or AccessViolation will throw
    public ServerEventCallbackMapper _mapper;

    private ServerState _state = ServerState.STATE_NONE;
    private ulong _serverID = 0;
    private string _serverKey = string.Empty;

    #region Event Declarations

    public event ClientStartTalkingEvent ClientStartTalking;

    public event ClientStopTalkingEvent ClientStopTalking;

    public event ClientConnectedEvent ClientConnected;

    public event ClientDisconnectedEvent ClientDisconnected;

    public event ClientMovedEvent ClientMoved;

    public event ChannelCreatedEvent ChannelCreated;

    public event ChannelEditedEvent ChannelEdited;

    public event ChannelDeletedEvent ChannelDeleted;

    public event ServerTextMessageEvent ServerTextMessage;

    public event ChannelTextMessageEvent ChannelTextMessage;

    public event UserLoggingMessageEvent UserLoggingMessage;

    public event AccountingErrorEvent AccountingError;

    #endregion Event Declarations

    #region Event Handlers

    //Ferry: The null check can remove when need more performance, just have to ensure there must be at least one listener listening each event
    private void onClientConnected(ulong serverID, ushort clientID, ulong channelID, ref uint removeClientError)
    {
        if (ClientConnected != null) ClientConnected(serverID, clientID, channelID, ref removeClientError);
    }

    private void onClientDisconnected(ulong serverID, ushort clientID, ulong channelID)
    {
        if (ClientDisconnected != null) ClientDisconnected(serverID, clientID, channelID);
    }

    private void onClientMoved(ulong serverID, ushort clientID, ulong oldChannelID, ulong newChannelID)
    {
        if (ClientMoved != null) ClientMoved(serverID, clientID, oldChannelID, newChannelID);
    }

    private void onChannelCreated(ulong serverID, ushort invokerClientID, ulong channelID)
    {
        if (ChannelCreated != null) ChannelCreated(serverID, invokerClientID, channelID);
    }

    private void onChannelEdited(ulong serverID, ushort invokerClientID, ulong channelID)
    {
        if (ChannelEdited != null) ChannelEdited(serverID, invokerClientID, channelID);
    }

    private void onChannelDeleted(ulong serverID, ushort invokerClientID, ulong channelID)
    {
        if (ChannelDeleted != null) ChannelDeleted(serverID, invokerClientID, channelID);
    }

    private void onServerTextMessage(ulong serverID, ushort invokerClientID, string textMessage)
    {
        if (ServerTextMessage != null) ServerTextMessage(serverID, invokerClientID, textMessage);
    }

    private void onChannelTextMessage(ulong serverID, ushort invokerClientID, ulong targetChannelID, string textMessage)
    {
        if (ChannelTextMessage != null) ChannelTextMessage(serverID, invokerClientID, targetChannelID, textMessage);
    }

    private void onUserLoggingMessage(string logMessage, int logLevel, string logChannel, ulong logID, string logTime, string completeLogString)
    {
        if (UserLoggingMessage != null) UserLoggingMessage(logMessage, logLevel, logChannel, logID, logTime, completeLogString);
    }

    private void onClientStartTalking(ulong serverID, ushort clientID)
    {
        if (ClientStartTalking != null) ClientStartTalking(serverID, clientID);
    }

    private void onClientStopTalking(ulong serverID, ushort clientID)
    { if (ClientStopTalking != null) ClientStopTalking(serverID, clientID);
    }

    private void onAccountingError(ulong serverID, int errorCode)
    {
        if (AccountingError != null) AccountingError(serverID, errorCode);
    }

    #endregion Event Handlers

    #region Utilities

    /* Read server key from file */

    private string readServerKeyFromFile(string fileName)
    {
        try
        {
            if (!File.Exists(fileName))
            {
                File.CreateText(fileName).Close();
                notify(string.Format("{0} not found, new key file created.", fileName));
                return string.Empty;
            }
            else
            {
                string key = File.ReadAllText(fileName);
                notify(string.Format("Server key: '{0}' read from file '{1}'.", key, fileName));
                return key;
            }
        }
        catch
        {
            notifyError(string.Format("Could not open file '{0}' for reading keypair", fileName));
            return string.Empty;
        }
    }

    /* Write server key to file */

    private void writeServerKeyToFile(string fileName)
    {
        try
        {
            IntPtr serverKeyPtr = IntPtr.Zero;
            uint result = Error.ok;
            if ((result = GetVirtualServerKeyPair(_serverID, out serverKeyPtr)) != Error.ok)
            {
                notifyError(string.Format("Error querying keyPair: %s\n", getGlobalErrorMsg(result)));
                return;
            }
            else
            {
                string key = getStringFromPointer(serverKeyPtr);
                File.WriteAllText(fileName, key);
                notify(string.Format("Keypair '{0}' written to file '{1}'.", key, fileName));
            }
        }
        catch (FileNotFoundException)
        {
            notifyError(string.Format("File '{0}' not found", fileName));
        }
        catch
        {
            notifyError(string.Format("Could not open file '{0}' for writing keypair", fileName));
        }
    }

    private string getStringFromPointer(IntPtr pointer)
    {
        string temp = string.Empty;
        temp = Marshal.PtrToStringAnsi(pointer);
        FreeMemory(pointer); /* Release dynamically allocated memory */
        return temp;
    }

    private string getGlobalErrorMsg(uint inputErrorCode)
    {
        string tmpMsg = string.Empty;
        IntPtr tmpPtr = IntPtr.Zero;
        uint tmpErrCode = GetGlobalErrorMessage(inputErrorCode, out tmpPtr);
        if (tmpErrCode == Error.ok)
            tmpMsg = getStringFromPointer(tmpPtr);
        return tmpMsg;
    }

    #endregion Utilities

    /* Assign the used callback function pointers */

    protected override void initMapper()
    {
        if (mapperInitialized) return;

        _mapper = new ServerEventCallbackMapper();
        _mapper.onClientConnected = onClientConnected;
        _mapper.onClientDisconnected = onClientDisconnected;
        _mapper.onClientMoved = onClientMoved;
        _mapper.onChannelCreated = onChannelCreated;
        _mapper.onChannelEdited = onChannelEdited;
        _mapper.onChannelDeleted = onChannelDeleted;
        _mapper.onServerTextMessage = onServerTextMessage;
        _mapper.onChannelTextMessage = onChannelTextMessage;
        _mapper.onUserLoggingMessage = onUserLoggingMessage;
        _mapper.onClientStartTalking = onClientStartTalking;
        _mapper.onClientStopTalking = onClientStopTalking;
        _mapper.onAccountingError = onAccountingError;
    }

    private void initServerLibrary()
    {
        /* Initialize server lib with callbacks */
        uint result = Error.ok;
        if ((result = InitServerLib(ref this._mapper, LogTypes.LogType_FILE | LogTypes.LogType_CONSOLE | LogTypes.LogType_USERLOGGING, null)) != Error.ok)
        {
            notifyError(string.Format("Failed to initialize serverlib: {0}.", result));
            return;
        }

        /* Query and print client lib version */
        IntPtr versionPtr = IntPtr.Zero;
        if ((result = GetServerLibVersion(out versionPtr)) != Error.ok)
        {
            notifyError(string.Format("Failed to get server lib version: {0}.", result));
            return;
        }
        notify(string.Format("Server Library Initialize Success. Library Version: {0}",
        getStringFromPointer(versionPtr)));
    }

    public bool start(int serverPort = 9987, string bindIP = "0.0.0.0",
        uint maxSlots = 32, string serverPassword = "secret",
        string serverName = "Testserver", string welcomeMessage = "Hello TeamSpeak 3")
    {
        if (_state == ServerState.STATE_STARTED)
        {
            notifyError("Server already started.");
            return false;
        }
        else if (_state == ServerState.STATE_NONE) // Ferry: Run once only, if not initialized
        {
            initServerLibrary();
        }

        string serverKeyFile = string.Format("serverKey_{0}.txt", serverPort);
        _serverKey = readServerKeyFromFile(serverKeyFile); // 9987 = default port

        /* Create the virtual server with specified port, name, keyPair and max clients */
        notify(string.Format("Create virtual server using keypair '{0}'", _serverKey));

        uint result = Error.ok;
        if ((result = CreateVirtualServer(serverPort, bindIP, serverName, _serverKey, maxSlots, out _serverID)) != Error.ok)
        {
            notifyError(string.Format("Error creating virtual server: {0}", getGlobalErrorMsg(result)));
            return false;
        }

        /* If we didn't load the keyPair before, query it from virtual server and save to file */
        /* Ferry: This will only run once at the first time server created. */
        if (_serverKey == string.Empty) writeServerKeyToFile(serverKeyFile);

        /* Set welcome message */
        if ((result = SetVirtualServerVariableAsString(_serverID, VirtualServerProperties.VIRTUALSERVER_WELCOMEMESSAGE, welcomeMessage)) != Error.ok)
        {
            notifyError(string.Format("Error setting server welcomemessage: {0}", result));
            return false;
        }

        /* Set server password */
        if ((result = SetVirtualServerVariableAsString(_serverID, VirtualServerProperties.VIRTUALSERVER_PASSWORD, serverPassword)) != Error.ok)
        {
            notifyError(string.Format("Error setting server password: {0}", result));
            return false;
        }

        /* Flush above two changes to server */
        if ((result = FlushVirtualServerVariable(_serverID)) != Error.ok)
        {
            notifyError(string.Format("Error flushing server variables: {0}", result));
            return false;
        }

        notify(string.Format("Server ID:{0} started successfully.", _serverID));
        _state = ServerState.STATE_STARTED;
        return true;
    }

    public bool close()
    {
        if (_state != ServerState.STATE_STARTED)
        {
            notifyError("Server is not running.");
            return false;
        }

        uint result = Error.ok;
        /* Stop virtual server */
        if ((result = StopVirtualServer(_serverID)) != Error.ok)
        {
            notifyError(string.Format("Error stopping virtual server: {0}", result));
            return false;
        }

        /* Shutdown server lib */
        if ((result = DestroyServerLib()) != Error.ok)
        {
            notifyError(string.Format("Error destroying server lib: {0}", result));
            return false;
        }
        notify(string.Format("Server ID:{0} closed successfully.", _serverID));
        _state = ServerState.STATE_CLOSED;
        return true;
    }

    /*
    public bool setStringVariable(ulong serverID, ulong channelID, ChannelProperties property, string value)
    {
        if (SetChannelVariableAsString(serverID, channelID, property, value) != Error.ok)
        {
            notifyError(string.Format("Error setting variable: {0}", property.ToString()));
            return false;
        }
        return true;
    }
    public bool setStringVariable(ulong serverID, ulong channelID, ClientProperties property, string value)
    {
        if (SetClientSelfVariableAsString(serverID, property, value) != Error.ok)
        {
            notifyError(string.Format("Error setting variable: {0}", property.ToString()));
            return false;
        }
        return true;
    }
    public bool setIntVariable(ulong serverID, ulong channelID, ChannelProperties property, int value)
    {
        if (SetChannelVariableAsInt(serverID, channelID, property, value) != Error.ok)
        {
            notifyError(string.Format("Error setting variable: {0}", property.ToString()));
            return false;
        }
        return true;
    }
    public bool setIntVariable(ulong serverID, ulong channelID, ClientProperties property, int value)
    {
        if (SetClientSelfVariableAsInt(serverID, property, value) != Error.ok)
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
        if ((result = GetServerVariableAsString(serverID, property, out valuePtr)) != Error.ok)
        {
            notifyError(string.Format("Error getting variable: {0}", property.ToString()));
            return string.Empty;
        }
        return getStringFromPointer(valuePtr);
    }
     */
    public string getStringVariable(ulong channelID, ChannelProperties property)
    {
        IntPtr valuePtr = IntPtr.Zero;
        uint result = Error.ok;
        if ((result = GetChannelVariableAsString(_serverID, channelID, property, out valuePtr)) != Error.ok)
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
        if ((result = GetClientVariableAsString(_serverID, clientID, property, out valuePtr)) != Error.ok)
        {
            notifyError(string.Format("Error getting variable: {0}", property.ToString()));
            return string.Empty;
        }
        return getStringFromPointer(valuePtr);
    }
    /*
    public string getStringVariable(ClientProperties property)
    {
        IntPtr valuePtr = IntPtr.Zero;
        uint result = Error.ok;
        if ((result = GetClientSelfVariableAsString(serverID, property, out valuePtr)) != Error.ok)
        {
            notifyError(string.Format("Error getting variable: {0}", property.ToString()));
            return string.Empty;
        }
        return getStringFromPointer(valuePtr);
    }
    public int getIntVariable(ulong serverID, VirtualServerProperties property)
    {
        int value = 0;
        if (GetServerVariableAsInt(serverID, property, out value) != Error.ok)
        {
            notifyError(string.Format("Error getting variable: {0}", property.ToString()));
            return 0;
        }
        return value;
    }
    public int getIntVariable(ulong channelID, ChannelProperties property)
    {
        int value = 0;
        if (GetChannelVariableAsInt(serverID, channelID, property, out value) != Error.ok)
        {
            notifyError(string.Format("Error getting variable: {0}", property.ToString()));
            return 0;
        }
        return value;
    }
    public int getIntVariable(ushort clientID, ClientProperties property)
    {
        int value = 0;
        if (GetClientVariableAsInt(serverID, clientID, property, out value) != Error.ok)
        {
            notifyError(string.Format("Error getting variable: {0}", property.ToString()));
            return 0;
        }
        return value;
    }
    public int getIntVariable(ClientProperties property)
    {
        int value = 0;
        if (GetClientSelfVariableAsInt(serverID, property, out value) != Error.ok)
        {
            notifyError(string.Format("Error getting variable: {0}", property.ToString()));
            return 0;
        }
        return value;
    }
     * */
    #region Server DLL Facade

#if x64
    private const string DLL_FILE_NAME = "ts3server_win64.dll";
#else
    const string DLL_FILE_NAME = "ts3server_win32.dll";
#endif

    #region Misc

    [DllImport(DLL_FILE_NAME, EntryPoint = "ts3server_freeMemory")]
    private static extern uint FreeMemory(IntPtr ptrVariableToFree);

    [DllImport(DLL_FILE_NAME, EntryPoint = "ts3server_setLogVerbosity")]
    private static extern uint SetLogVerbosity(LogLevel logVerbosity);

    [DllImport(DLL_FILE_NAME, EntryPoint = "ts3server_getGlobalErrorMessage")]
    private static extern uint GetGlobalErrorMessage(uint errorCode, out IntPtr errorMessage);

    #endregion Misc

    #region Server Library

    [DllImport(DLL_FILE_NAME, EntryPoint = "ts3server_initServerLib")]
    private static extern uint InitServerLib(ref ServerEventCallbackMapper functionPointers, LogTypes logTypes, string logFolderPath);

    [DllImport(DLL_FILE_NAME, EntryPoint = "ts3server_getServerLibVersionNumber")]
    private static extern uint GetServerLibVersionNumber(out ulong result);

    [DllImport(DLL_FILE_NAME, EntryPoint = "ts3server_getServerLibVersion")]
    private static extern uint GetServerLibVersion(out IntPtr result);

    [DllImport(DLL_FILE_NAME, EntryPoint = "ts3server_destroyServerLib")]
    private static extern uint DestroyServerLib();

    #endregion Server Library

    #region Server

    [DllImport(DLL_FILE_NAME, EntryPoint = "ts3server_getVirtualServerList")]
    private static extern uint GetVirtualServerList(out ulong[] result);

    [DllImport(DLL_FILE_NAME, EntryPoint = "ts3server_createVirtualServer")]
    private static extern uint CreateVirtualServer(int serverPort, string ip, string serverName, string serverKeyPair, uint serverMaxClients, out ulong generatedServerID);

    [DllImport(DLL_FILE_NAME, EntryPoint = "ts3server_stopVirtualServer")]
    private static extern uint StopVirtualServer(ulong serverID);

    [DllImport(DLL_FILE_NAME, EntryPoint = "ts3server_getVirtualServerKeyPair")]
    private static extern uint GetVirtualServerKeyPair(ulong serverID, out IntPtr result);

    //Ferry: Connection Properties have no setters
    [DllImport(DLL_FILE_NAME, EntryPoint = "ts3server_getVirtualServerConnectionVariableAsUInt64")]
    private static extern uint GetVirtualServerConnectionVariableAsulong(ulong serverID, ConnectionProperties flag, out IntPtr result);

    [DllImport(DLL_FILE_NAME, EntryPoint = "ts3server_getVirtualServerConnectionVariableAsDouble")]
    private static extern uint GetVirtualServerConnectionVariableAsDouble(ulong serverID, ConnectionProperties flag, out IntPtr result);

    [DllImport(DLL_FILE_NAME, EntryPoint = "ts3server_getVirtualServerVariableAsInt")]
    private static extern uint GetVirtualServerVariableAsInt(ulong serverID, VirtualServerProperties flag, out IntPtr result);

    [DllImport(DLL_FILE_NAME, EntryPoint = "ts3server_getVirtualServerVariableAsString")]
    private static extern uint GetVirtualServerVariableAsString(ulong serverID, VirtualServerProperties flag, out IntPtr result);

    [DllImport(DLL_FILE_NAME, EntryPoint = "ts3server_setVirtualServerVariableAsInt")]
    private static extern uint SetVirtualServerVariableAsInt(ulong serverID, VirtualServerProperties flag, int value);

    [DllImport(DLL_FILE_NAME, EntryPoint = "ts3server_setVirtualServerVariableAsString")]
    private static extern uint SetVirtualServerVariableAsString(ulong serverID, VirtualServerProperties flag, string value);

    [DllImport(DLL_FILE_NAME, EntryPoint = "ts3server_flushVirtualServerVariable")]
    private static extern uint FlushVirtualServerVariable(ulong serverID);

    #endregion Server

    #region Channel

    [DllImport(DLL_FILE_NAME, EntryPoint = "ts3server_getChannelList")]
    private static extern uint GetChannelList(ulong serverID, out ulong[] result);

    //Ferry: take care the IntPtr[] here are ushort Array
    [DllImport(DLL_FILE_NAME, EntryPoint = "ts3server_getChannelClientList")]
    private static extern uint GetChannelClientList(ulong serverID, ulong channelID, out IntPtr[] result);

    [DllImport(DLL_FILE_NAME, EntryPoint = "ts3server_getChannelOfClient")]
    private static extern uint GetChannelOfClient(ulong serverID, ushort clientID, out ulong result);

    [DllImport(DLL_FILE_NAME, EntryPoint = "ts3server_getParentChannelOfChannel")]
    private static extern uint GetParentChannelOfChannel(ulong serverID, ulong channelID, out IntPtr result);

    [DllImport(DLL_FILE_NAME, EntryPoint = "ts3server_channelDelete")]
    private static extern uint ChannelDelete(ulong serverID, ulong channelID, int forceDelete);

    [DllImport(DLL_FILE_NAME, EntryPoint = "ts3server_channelMove")]
    private static extern uint ChannelMove(ulong serverID, ulong channelID, ulong newChannelParentID);

    [DllImport(DLL_FILE_NAME, EntryPoint = "ts3server_flushChannelCreation")]
    private static extern uint FlushChannelCreation(ulong serverID, ulong channelID, out IntPtr[] result);

    [DllImport(DLL_FILE_NAME, EntryPoint = "ts3server_getChannelVariableAsInt")]
    private static extern uint GetChannelVariableAsInt(ulong serverID, ChannelProperties flag, out IntPtr result);

    [DllImport(DLL_FILE_NAME, EntryPoint = "ts3server_getChannelVariableAsString")]
    private static extern uint GetChannelVariableAsString(ulong serverID, ulong channelID, ChannelProperties flag, out IntPtr result);

    [DllImport(DLL_FILE_NAME, EntryPoint = "ts3server_setChannelVariableAsInt")]
    private static extern uint SetChannelVariableAsInt(ulong serverID, ChannelProperties flag, int value);

    [DllImport(DLL_FILE_NAME, EntryPoint = "ts3server_setChannelVariableAsString")]
    private static extern uint SetChannelVariableAsString(ulong serverID, ChannelProperties flag, string value);

    [DllImport(DLL_FILE_NAME, EntryPoint = "ts3server_flushChannelVariable")]
    private static extern uint FlushChannelVariable(ulong serverID, ulong channelID);

    #endregion Channel

    #region Client

    [DllImport(DLL_FILE_NAME, EntryPoint = "ts3server_getClientList")]
    private static extern uint GetClientList(ulong serverID, out IntPtr[] result);

    [DllImport(DLL_FILE_NAME, EntryPoint = "ts3server_clientMove")]
    private static extern uint ClientMove(ulong serverID, ulong newChannelID, ushort[] clients);

    [DllImport(DLL_FILE_NAME, EntryPoint = "ts3server_getClientVariableAsInt")]
    private static extern uint GetClientVariableAsInt(ulong serverID, ushort clientID, ClientProperties flag, out IntPtr result);

    [DllImport(DLL_FILE_NAME, EntryPoint = "ts3server_getClientVariableAsString")]
    private static extern uint GetClientVariableAsString(ulong serverID, ushort clientID, ClientProperties flag, out IntPtr result);

    [DllImport(DLL_FILE_NAME, EntryPoint = "ts3server_setClientVariableAsInt")]
    private static extern uint SetClientVariableAsInt(ulong serverID, ClientProperties flag, int value);

    [DllImport(DLL_FILE_NAME, EntryPoint = "ts3server_setClientVariableAsString")]
    private static extern uint SetClientVariableAsString(ulong serverID, ClientProperties flag, string value);

    [DllImport(DLL_FILE_NAME, EntryPoint = "ts3server_setClientWhisperList")]
    private static extern uint SetClientWhisperList(ulong serverID, ushort flag, ulong[] channels, ushort[] clients);

    [DllImport(DLL_FILE_NAME, EntryPoint = "ts3server_flushClientVariable")]
    private static extern uint FlushClientVariable(ulong serverID, ushort clientID);

    #endregion Client

    #region Original Defitition in C

    /*
uint ts3server_freeMemory(void* pointer);

uint ts3server_initServerLib(const struct ServerLibFunctions* functionPointers, int usedLogTypes, const char* logFileFolder);
uint ts3server_destroyServerLib();
uint ts3server_getServerLibVersion(char** result);
uint ts3server_getServerLibVersionNumber(uint64* result);

uint ts3server_setLogVerbosity(enum LogLevel logVerbosity);

uint ts3server_getGlobalErrorMessage(unsigned int globalErrorCode, char** result);

uint ts3server_getClientVariableAsInt(uint64 serverID, anyID clientID, enum ClientProperties flag, int* result);
uint ts3server_getClientVariableAsString(uint64 serverID, anyID clientID, enum ClientProperties flag, char** result);
uint ts3server_setClientVariableAsInt(uint64 serverID, anyID clientID, enum ClientProperties flag, int value);
uint ts3server_setClientVariableAsString(uint64 serverID, anyID clientID, enum ClientProperties flag, const char* value);
uint ts3server_flushClientVariable(uint64 serverID, anyID clientID);

uint ts3server_setClientWhisperList(uint64 serverID, anyID clID, const uint64* channelID, const anyID* clientID);

uint ts3server_getClientList(uint64 serverID, anyID** result);
uint ts3server_getChannelOfClient(uint64 serverID, anyID clientID, uint64* result);
uint ts3server_clientMove(uint64 serverID, uint64 newChannelID, const anyID* clientIDArray); #

uint ts3server_getChannelVariableAsInt(uint64 serverID, uint64 channelID, enum ChannelProperties flag, int* result);
uint ts3server_getChannelVariableAsString(uint64 serverID, uint64 channelID, enum ChannelProperties flag, char** result);
uint ts3server_setChannelVariableAsInt(uint64 serverID, uint64 channelID, enum ChannelProperties flag, int value);
uint ts3server_setChannelVariableAsString(uint64 serverID, uint64 channelID, enum ChannelProperties flag, const char* value);
uint ts3server_flushChannelVariable(uint64 serverID, uint64 channelID);
uint ts3server_flushChannelCreation(uint64 serverID, uint64 channelParentID, uint64* result);

uint ts3server_getChannelList(uint64 serverID, uint64** result);
uint ts3server_getChannelClientList(uint64 serverID, uint64 channelID, anyID** result);
uint ts3server_getParentChannelOfChannel(uint64 serverID, uint64 channelID, uint64* result);

uint ts3server_channelDelete(uint64 serverID, uint64 channelID, int force);
uint ts3server_channelMove(uint64 serverID, uint64 channelID, uint64 newChannelParentID, uint64 newOrder);

uint ts3server_getVirtualServerVariableAsInt(uint64 serverID, enum VirtualServerProperties flag, int* result);
uint ts3server_getVirtualServerVariableAsString(uint64 serverID, enum VirtualServerProperties flag, char** result);
uint ts3server_setVirtualServerVariableAsInt(uint64 serverID, enum VirtualServerProperties flag, int value);
uint ts3server_setVirtualServerVariableAsString(uint64 serverID, enum VirtualServerProperties flag, const char* value);
uint ts3server_flushVirtualServerVariable(uint64 serverID);

uint ts3server_getVirtualServerConnectionVariableAsUInt64(uint64 serverID, enum ConnectionProperties flag, uint64* result);
uint ts3server_getVirtualServerConnectionVariableAsDouble(uint64 serverID, enum ConnectionProperties flag, double* result);

uint ts3server_getVirtualServerList(uint64** result);
uint ts3server_stopVirtualServer(uint64 serverID);
uint ts3server_createVirtualServer(unsigned int serverPort, const char* serverIp, const char* serverName, const char* serverKeyPair, unsigned int serverMaxClients, uint64* result);
uint ts3server_getVirtualServerKeyPair(uint64 serverID, char** result);
 */

    #endregion Original Defitition in C

    #endregion Server DLL Facade

    //REMEMBER TO TRY OPTIMZE THE DATA TYPE WHEN HAVE TIME, USING ALL ulong and uint is waste of memory.
}