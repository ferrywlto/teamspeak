using System;
using System.IO;
using System.Runtime.InteropServices;
using teamspeak.definition;

/* Ferry: This is the place to wire-up server event to actual handlers
 * i.e. the UI, such that all object that are interested in server event
 * can register the listener.
 */

public class CustomTS3Server
{
    //Ferry: [WARNING] _mapper must be public or AccessViolation will throw
    public ServerEventCallbackMapper _mapper;
    private ServerState _state = ServerState.STATE_NONE;
    private ulong _serverID = 0;
    private string _serverKey = string.Empty;

    #region Event Declarations
    public event VoiceDataEventHandler VoiceData;

    public event ClientTalkingEventHandler ClientStartTalking;

    public event ClientTalkingEventHandler ClientStopTalking;

    public event ClientConnectedEventHandler ClientConnected;

    public event ClientDisconnectedEventHandler ClientDisconnected;

    public event ClientMovedEventHandler ClientMoved;

    public event ChannelEventHandler ChannelCreated;

    public event ChannelEventHandler ChannelEdited;

    public event ChannelEventHandler ChannelDeleted;

    public event ServerTextMessageEventHandler ServerTextMessage;

    public event ChannelTextMessageEventHandler ChannelTextMessage;

    public event UserLoggingMessageEventHandler UserLoggingMessage;

    public event AccountingErrorEventHandler AccountingError;

    public event MessageEventHandler ErrorOccured;

    public event MessageEventHandler NotificationNeeded;
    #endregion

    public CustomTS3Server()
    {
        initMapper();
    }

    #region Event Handlers
    //Ferry: The null check can remove when need more performance, just have to ensure there must be at least one listener listening each event
    public void onClientConnected(ulong serverID, ushort clientID, ulong channelID, ref uint removeClientError)
    {
        if(ClientConnected!=null) ClientConnected(serverID, clientID, channelID, ref removeClientError);
    }

    public void onClientDisconnected(ulong serverID, ushort clientID, ulong channelID)
    {
        if (ClientDisconnected != null) ClientDisconnected(serverID, clientID, channelID);
    }

    public void onClientMoved(ulong serverID, ushort clientID, ulong oldChannelID, ulong newChannelID)
    {
        if (ClientMoved != null) ClientMoved(serverID, clientID, oldChannelID, newChannelID);
    }

    public void onChannelCreated(ulong serverID, ushort invokerClientID, ulong channelID)
    {
        if (ChannelCreated != null) ChannelCreated(serverID, invokerClientID, channelID);
    }

    public void onChannelEdited(ulong serverID, ushort invokerClientID, ulong channelID)
    {
        if (ChannelEdited != null) ChannelEdited(serverID, invokerClientID, channelID);
    }

    public void onChannelDeleted(ulong serverID, ushort invokerClientID, ulong channelID)
    {
        if (ChannelDeleted != null) ChannelDeleted(serverID, invokerClientID, channelID);
    }

    public void onServerTextMessage(ulong serverID, ushort invokerClientID, string textMessage)
    {
        if (ServerTextMessage != null) ServerTextMessage(serverID, invokerClientID, textMessage);
    }

    public void onChannelTextMessage(ulong serverID, ushort invokerClientID, ulong targetChannelID, string textMessage)
    {
        if (ChannelTextMessage != null) ChannelTextMessage(serverID, invokerClientID, targetChannelID, textMessage);
    }

    public void onUserLoggingMessage(string logMessage, int logLevel, string logChannel, ulong logID, string logTime, string completeLogString)
    {
        if (UserLoggingMessage != null) UserLoggingMessage(logMessage, logLevel, logChannel, logID, logTime, completeLogString);
    }

    public void onClientStartTalking(ulong serverID, ushort clientID)
    {
        if (ClientStartTalking != null) ClientStartTalking(serverID, clientID);
    }

    public void onClientStopTalking(ulong serverID, ushort clientID)
    {
        if (ClientStopTalking != null) ClientStopTalking(serverID, clientID);
    }

    public void onAccountingError(ulong serverID, int errorCode)
    {
        if (AccountingError != null) AccountingError(serverID, errorCode);
    }


    #endregion
    #region Utilities
    /* Read server key from file */
    private string readServerKeyFromFile(string fileName)
    {
        try
        {
            if(!File.Exists(fileName))
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
            if((result = GetVirtualServerKeyPair(_serverID, out serverKeyPtr)) != Error.ok)
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

    private void notify(string message)
    {
        if (NotificationNeeded != null) NotificationNeeded(message);
    }

    private void notifyError(string message)
    {
        if (ErrorOccured != null) ErrorOccured(message);
    }
    #endregion
    
    private void initMapper()
    {
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
        /* Ferry: actually it will not happen, private and only called once when server start */
        //if (_state != ServerState.STATE_NONE)
        //{
        //    notifyError("Server library initialized already!");
        //    return;
        //}

        /* Assign the used callback function pointers */
        /* Initialize server lib with callbacks */
        uint result = Error.ok;
        if ((result = InitServerLib(ref this._mapper, LogTypes.LogType_FILE | LogTypes.LogType_CONSOLE | LogTypes.LogType_USERLOGGING, null)) != Error.ok)
        {
            notifyError(string.Format("Failed to initialize serverlib: {0}.", result));
            return;
        }

        /* Query and print client lib version */
        IntPtr versionPtr = IntPtr.Zero;
        
        if((result = GetServerLibVersion(out versionPtr)) != Error.ok)
        {
            notifyError(string.Format("Failed to get server lib version: {0}.", result));
            return;
        }

        notify(string.Format("Server Library Initialize Success. Library Version: {0}",
            getStringFromPointer(versionPtr)));
    }

    public void start(int serverPort = 9987, string bindIP = "0.0.0.0",
        uint maxSlots = 32, string serverPassword = "secret",
        string serverName = "Testserver", string welcomeMessage = "Hello TeamSpeak 3")
    {
        if (_state == ServerState.STATE_STARTED)
        {
            notifyError("Server already started.");
            return;
        }
        else if(_state == ServerState.STATE_NONE) // Ferry: Run once only, if not initialized
        {
            initServerLibrary();
        }

        string serverKeyFile = string.Format("serverKey_{0}.txt", serverPort);
        _serverKey = readServerKeyFromFile(serverKeyFile); // 9987 = default port

        /* Create the virtual server with specified port, name, keyPair and max clients */
        notify(string.Format("Create virtual server using keypair '{0}'", _serverKey));

        uint result = Error.ok;
        if((result = CreateVirtualServer(serverPort, bindIP, serverName, _serverKey, maxSlots, out _serverID)) != Error.ok)
        {
            notifyError(string.Format("Error creating virtual server: {0}", getGlobalErrorMsg(result)));
            return;
        }

        /* If we didn't load the keyPair before, query it from virtual server and save to file */
        /* Ferry: This will only run once at the first time server created. */
        if (_serverKey == string.Empty) writeServerKeyToFile(serverKeyFile);

        /* Set welcome message */
        if ((result = SetVirtualServerVariableAsString(_serverID, VirtualServerProperties.VIRTUALSERVER_WELCOMEMESSAGE, welcomeMessage)) != Error.ok)
        {
            notifyError(string.Format("Error setting server welcomemessage: {0}", result));
            return;
        }
                
        /* Set server password */
        if ((result = SetVirtualServerVariableAsString(_serverID, VirtualServerProperties.VIRTUALSERVER_PASSWORD, serverPassword)) != Error.ok)
        {
            notifyError(string.Format("Error setting server password: {0}", result));
            return;
        }

        /* Flush above two changes to server */
        if ((result = FlushVirtualServerVariable(_serverID)) != Error.ok)
        {
            notifyError(string.Format("Error flushing server variables: {0}", result));
            return;
        }

        notify(string.Format("Server ID:{0} started successfully.", _serverID));
        _state = ServerState.STATE_STARTED;
    }

    public void close()
    {
        if (_state != ServerState.STATE_STARTED)
        {
            notifyError("Server is not running.");
            return;
        }

        uint result = Error.ok;
        /* Stop virtual server */
        if ((result = StopVirtualServer(_serverID)) != Error.ok)
        {
            notifyError(string.Format("Error stopping virtual server: {0}", result));
            return;
        }
        
        /* Shutdown server lib */
        if ((result = DestroyServerLib()) != Error.ok)
        {
            notifyError(string.Format("Error destroying server lib: {0}", result));
            return;
        }
        notify(string.Format("Server ID:{0} closed successfully.", _serverID));
        _state = ServerState.STATE_CLOSED;
    }

    #region Server DLL Facade
#if x64
    const string DLL_FILE_NAME = "ts3server_win64.dll";
#else
    const string dllFileName = "ts3server_win32.dll";
#endif
    [DllImport(DLL_FILE_NAME, EntryPoint = "ts3server_setVirtualServerVariableAsInt")]
    extern static uint SetVirtualServerVariableAsInt(ulong serverID, ChannelProperties flag, int value);

    [DllImport(DLL_FILE_NAME, EntryPoint = "ts3server_initServerLib")]
    extern static uint InitServerLib(ref ServerEventCallbackMapper functionPointers, LogTypes logTypes, string logFolderPath);

    [DllImport(DLL_FILE_NAME, EntryPoint = "ts3server_getServerLibVersion")]
    extern static uint GetServerLibVersion(out IntPtr result);

    [DllImport(DLL_FILE_NAME, EntryPoint = "ts3server_freeMemory")]
    extern static uint FreeMemory(IntPtr ptrVariableToFree);

    [DllImport(DLL_FILE_NAME, EntryPoint = "ts3server_destroyServerLib")]
    extern static uint DestroyServerLib();

    [DllImport(DLL_FILE_NAME, EntryPoint = "ts3server_createVirtualServer")]
    extern static uint CreateVirtualServer(int serverPort, string ip, string serverName, string serverKeyPair, uint serverMaxClients, out ulong generatedServerID);

    [DllImport(DLL_FILE_NAME, EntryPoint = "ts3server_getGlobalErrorMessage")]
    extern static uint GetGlobalErrorMessage(uint errorCode, out IntPtr errorMessage);

    [DllImport(DLL_FILE_NAME, EntryPoint = "ts3server_getVirtualServerKeyPair")]
    extern static uint GetVirtualServerKeyPair(ulong serverID, out IntPtr result);

    [DllImport(DLL_FILE_NAME, EntryPoint = "ts3server_flushVirtualServerVariable")]
    extern static uint FlushVirtualServerVariable(ulong serverID);

    [DllImport(DLL_FILE_NAME, EntryPoint = "ts3server_stopVirtualServer")]
    extern static uint StopVirtualServer(ulong serverID);

    #region variable getters
    //Ferry: Connection Properties have no setters
    [DllImport(DLL_FILE_NAME, EntryPoint = "ts3server_getVirtualServerConnectionVariableAsulong")]
    extern static uint GetVirtualServerConnectionVariableAsulong(ulong serverID, ConnectionProperties flag, out IntPtr result);

    [DllImport(DLL_FILE_NAME, EntryPoint = "ts3server_getVirtualServerConnectionVariableAsDouble")]
    extern static uint GetVirtualServerConnectionVariableAsDouble(ulong serverID, ConnectionProperties flag, out IntPtr result);

    [DllImport(DLL_FILE_NAME, EntryPoint = "ts3server_getVirtualServerVariableAsInt")]
    extern static uint GetVirtualServerVariableAsInt(ulong serverID, VirtualServerProperties flag, out IntPtr result);

    [DllImport(DLL_FILE_NAME, EntryPoint = "ts3server_getVirtualServerVariableAsString")]
    extern static uint GetVirtualServerVariableAsString(ulong serverID, VirtualServerProperties flag, out IntPtr result);

    [DllImport(DLL_FILE_NAME, EntryPoint = "ts3server_getChannelVariableAsInt")]
    extern static uint GetChannelVariableAsInt(ulong serverID, ChannelProperties flag, out IntPtr result);

    [DllImport(DLL_FILE_NAME, EntryPoint = "ts3server_getChannelVariableAsString")]
    extern static uint GetChannelVariableAsString(ulong serverID, ChannelProperties flag, out IntPtr result);

    [DllImport(DLL_FILE_NAME, EntryPoint = "ts3server_getClientVariableAsInt")]
    extern static uint GetClientVariableAsInt(ulong serverID, ClientProperties flag, out IntPtr result);

    [DllImport(DLL_FILE_NAME, EntryPoint = "ts3server_getClientVariableAsString")]
    extern static uint GetClientVariableAsString(ulong serverID, ClientProperties flag, out IntPtr result);
    #endregion

    #region variable setters
    [DllImport(DLL_FILE_NAME, EntryPoint = "ts3server_setVirtualServerVariableAsInt")]
    extern static uint SetVirtualServerVariableAsInt(ulong serverID, VirtualServerProperties flag, int value);

    [DllImport(DLL_FILE_NAME, EntryPoint = "ts3server_setVirtualServerVariableAsString")]
    extern static uint SetVirtualServerVariableAsString(ulong serverID, VirtualServerProperties flag, string value);

    [DllImport(DLL_FILE_NAME, EntryPoint = "ts3server_setChannelVariableAsInt")]
    extern static uint SetChannelVariableAsInt(ulong serverID, ChannelProperties flag, int value);

    [DllImport(DLL_FILE_NAME, EntryPoint = "ts3server_setChannelVariableAsString")]
    extern static uint SetChannelVariableAsString(ulong serverID, ChannelProperties flag, string value);
    
    [DllImport(DLL_FILE_NAME, EntryPoint = "ts3server_setClientVariableAsInt")]
    extern static uint SetClientVariableAsInt(ulong serverID, ClientProperties flag, int value);

    [DllImport(DLL_FILE_NAME, EntryPoint = "ts3server_setClientVariableAsString")]
    extern static uint SetClientVariableAsString(ulong serverID, ClientProperties flag, string value);
    #endregion

    [DllImport(DLL_FILE_NAME, EntryPoint = "ts3server_setClientWhisperList")]
    extern static uint SetClientWhisperList(ulong serverID, ushort flag, ulong[] channels, ushort[] clients);

    [DllImport(DLL_FILE_NAME, EntryPoint = "ts3server_getParentChannelOfChannel")]
    extern static uint GetParentChannelOfChannel(ulong serverID, ulong channelID, out IntPtr result);

    [DllImport(DLL_FILE_NAME, EntryPoint = "ts3server_getChannelOfClient")]
    extern static uint GetChannelOfClient(ulong serverID, ulong channelID, out IntPtr result);

    //Ferry: take care the IntPtr[] here are ushort Array
    [DllImport(DLL_FILE_NAME, EntryPoint = "ts3server_getChannelClientList")]
    extern static uint GetChannelClientList(ulong serverID, ulong channelID, out IntPtr[] result);

    [DllImport(DLL_FILE_NAME, EntryPoint = "ts3server_getClientList")]
    extern static uint GetClientList(ulong serverID, out IntPtr[] result);

    [DllImport(DLL_FILE_NAME, EntryPoint = "ts3server_getChannelList")]
    extern static uint GetChannelList(ulong serverID, out IntPtr[] result);

    [DllImport(DLL_FILE_NAME, EntryPoint = "ts3server_flushChannelCreation")]
    extern static uint FlushChannelCreation(ulong serverID, ulong channelID, out IntPtr[] result);

    [DllImport(DLL_FILE_NAME, EntryPoint = "ts3server_channelDelete")]
    extern static uint ChannelDelete(ulong serverID, ulong channelID, int forceDelete);

    [DllImport(DLL_FILE_NAME, EntryPoint = "ts3server_channelMove")]
    extern static uint ChannelMove(ulong serverID, ulong channelID, ulong newChannelParentID);

    [DllImport(DLL_FILE_NAME, EntryPoint = "ts3server_clientMove")]
    extern static uint ClientMove(ulong serverID, ulong newChannelID, ushort[] clients);
  
    #endregion
    //REMEMBER TO TRY OPTIMZE THE DATA TYPE WHEN HAVE TIME, USING ALL ulong and uint is waste of memory.
}
