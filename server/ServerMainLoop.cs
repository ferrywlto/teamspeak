/*
 * TeamSpeak 3 server minimal sample C#
 *
 * Copyright (c) 2007-2010 TeamSpeak-Systems
 */

using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using teamspeak;
using teamspeak.definition;
using uint64 = System.UInt64;

internal class ServerMainLoop
{
    public ServerState _state;
    private uint _errorCode = Error.ok;
    private string _KeyPair = string.Empty;
    private ServerForm _UI;
    private int defaultPort = 9987;
    private string defaultServerIP = "0.0.0.0"; // 0.0.0.0 = bind to all interface(IP)
    private string defaultServerPassword = "secret";

    private uint defaultServerSlot = 10;
    private uint64 firstServerID = 0;
    private Task serverTask;

    public ServerMainLoop(ServerForm ui)
    {
        _UI = ui;
        init();
    }

    public void closeServer()
    {
        if (_state == ServerState.STATE_CLOSED) return;

        uint errorCode;
        /* Stop virtual server */
        errorCode = ServerDLLFacade.stopVirtualServer(firstServerID);
        checkError("Error stopping virtual server: {0}", errorCode);

        /* Shutdown server lib */
        errorCode = ServerDLLFacade.destroyServerLib();
        checkError("Error destroying server lib: {0}", errorCode);

        _state = ServerState.STATE_CLOSED;
    }

    public string getStringFromPointer(IntPtr pointer)
    {
        string temp = string.Empty;
        temp = Marshal.PtrToStringAnsi(pointer);
        ServerDLLFacade.freeMemory(pointer); /* Release dynamically allocated memory */
        return temp;
    }

    public void start()
    {
        if (serverTask == null)
            serverTask = new Task(() => run());
        if (serverTask.Status != TaskStatus.Running)
        {
        }
    }

    public void startServer()
    {
        if (_state == ServerState.STATE_STARTED) return;

        string keyPairFileName = string.Format("keypair_{0}.txt", defaultPort);
        ServerDLLFacade.readKeyPairFromFile(keyPairFileName, out _KeyPair); // 9987 = default port
        uint errorCode;
        /* Create virtual server using default port 9987 with max 10 slots */

        /* Create the virtual server with specified port, name, keyPair and max clients */
        msg(string.Format("Create virtual server using keypair '{0}'", _KeyPair));
        //IntPtr pServerID = IntPtr.Zero;
        errorCode = ServerDLLFacade.createVirtualServer(defaultPort, defaultServerIP, "TeamSpeak 3 SDK Testserver", _KeyPair, defaultServerSlot, out firstServerID);
        checkError(string.Format("Error creating virtual server: {0}", getGlobalErrorMsg(errorCode)), errorCode);

        /* If we didn't load the keyPair before, query it from virtual server and save to file */
        if (_KeyPair == string.Empty)
        {
            IntPtr keyPairPtr = IntPtr.Zero;
            errorCode = ServerDLLFacade.getVirtualServerKeyPair(firstServerID, out keyPairPtr);
            checkError(string.Format("Error querying keyPair: %s\n", getGlobalErrorMsg(errorCode)), errorCode);

            _KeyPair = getStringFromPointer(keyPairPtr);

            /* Save keyPair to file "keypair_<port>.txt"*/
            if (!ServerDLLFacade.writeKeyPairToFile(keyPairFileName, _KeyPair))
                throw new Exception("Error write key pair to file" + keyPairFileName);
        }
        /* Set welcome message */
        checkError("Error setting server welcomemessage: {0}",
            ServerDLLFacade.setVirtualServerVariableAsString(firstServerID, VirtualServerProperties.VIRTUALSERVER_WELCOMEMESSAGE, "Hello TeamSpeak 3"));

        /* Set server password */
        checkError("Error setting server password: {0}",
            ServerDLLFacade.setVirtualServerVariableAsString(firstServerID, VirtualServerProperties.VIRTUALSERVER_PASSWORD, defaultServerPassword));

        /* Flush above two changes to server */
        checkError("Error flushing server variables: {0}",
            ServerDLLFacade.flushVirtualServerVariable(firstServerID));

        _state = ServerState.STATE_STARTED;
    }

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

    private void checkError(string errorMsg, uint errorCode)
    {
        if (errorCode != Error.ok) throw new Exception(string.Format(errorMsg, _errorCode));
    }

    private void errorMsg(string msg, uint errorCode = Error.ok)
    {
        errorMsg(string.Format(msg, errorCode));
    }

    private string getGlobalErrorMsg(uint inputErrorCode)
    {
        string tmpMsg = string.Empty;
        IntPtr tmpPtr = IntPtr.Zero;
        uint tmpErrCode = ServerDLLFacade.getGlobalErrorMessage(inputErrorCode, out tmpPtr);
        if (tmpErrCode == Error.ok)
            tmpMsg = getStringFromPointer(tmpPtr);
        return tmpMsg;
    }

    private void init()
    {
        /* Assign the used callback function pointers */
        ServerEventCallbackMapper mapper = ServerEventHandler.getDefaultEventCallbackMapper();
        /* Initialize server lib with callbacks */
        checkError("Failed to initialize serverlib: {0}.",
            ServerDLLFacade.initServerLib(ref mapper, LogTypes.LogType_FILE | LogTypes.LogType_CONSOLE | LogTypes.LogType_USERLOGGING, null));

        /* Query and print client lib version */
        IntPtr versionPtr = IntPtr.Zero;
        checkError("Failed to get server lib version: {0}.",
            ServerDLLFacade.getServerLibVersion(out versionPtr));

        msg(getStringFromPointer(versionPtr));
        msg("Server Library Initialize Success.");
    }

    private void msg(string msg)
    {
        if (_UI != null)
            _UI.writeLine(msg);
        else
            Console.WriteLine(msg);
    }

    private void run()
    {
    }
}