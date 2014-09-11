﻿using System;
using teamspeak.enumeration.server;
using anyID = System.UInt16;
using uint64 = System.UInt64;

namespace ts3_server_minimal_sample
{
    /* Ferry: This is the actual place for server-client interaction logic, 
     * the architecture is: 
     *  Program: Server main thread (init>start>listen/maintain connections), 
     *  callback: server-client interaction logic, this class is what we should subclass/modify  
     */
    public static class callback_x
    {
        public static server_callback_structx getDefaultMapper()
        {
            server_callback_structx cbs = new server_callback_structx();
            cbs.onClientConnected = onClientConnected;
            cbs.onClientDisconnected_delegate = onClientDisconnected;
            cbs.onClientMoved_delegate = onClientMoved;
            cbs.onChannelCreated_delegate = onChannelCreated;
            cbs.onChannelEdited_delegate = onChannelEdited;
            cbs.onChannelDeleted_delegate = onChannelDeleted;
            cbs.onServerTextMessageEvent_delegate = onServerTextMessageEvent;
            cbs.onChannelTextMessageEvent_delegate = onChannelTextMessageEvent;
            cbs.onUserLoggingMessageEvent_delegate = onUserLoggingMessageEvent;
            cbs.onClientStartTalkingEvent_delegate = onClientStartTalkingEvent;
            cbs.onClientStopTalkingEvent_delegate = onClientStopTalkingEvent;
            cbs.onAccountingErrorEvent_delegate = onAccountingErrorEvent;
            return cbs;
        }
        /*
         * 
         * Callback when client has connected.
         *
         * Parameter:
         *   serverID  - Virtual server ID
         *   clientID  - ID of connected client
         *   channelID - ID of channel the client joined
         */

        public static void onClientConnected(uint64 serverID, anyID clientID, uint64 channelID, ref uint removeClientError)
        {
            Console.WriteLine("Client {0} joined channel {1} on virtual server {2}", clientID, channelID, serverID);
        }

        /*
         * Callback when client has disconnected.
         *
         * Parameter:
         *   serverID  - Virtual server ID
         *   clientID  - ID of disconnected client
         *   channelID - ID of channel the client left
         */

        public static void onClientDisconnected(uint64 serverID, anyID clientID, uint64 channelID)
        {
            Console.WriteLine("Client {0} left channel {1} on virtual server {2}", clientID, channelID, serverID);
        }

        /*
         * Callback when client has moved.
         *
         * Parameter:
         *   serverID     - Virtual server ID
         *   clientID     - ID of moved client
         *   oldChannelID - ID of old channel the client left
         *   newChannelID - ID of new channel the client joined
         */

        public static void onClientMoved(uint64 serverID, anyID clientID, uint64 oldChannelID, uint64 newChannelID)
        {
            Console.WriteLine("Client {0} moved from channel {1} to channel {2} on virtual server {3}", clientID, oldChannelID, newChannelID, serverID);
        }

        /*
         * Callback when channel has been created.
         *
         * Parameter:
         *   serverID        - Virtual server ID
         *   invokerClientID - ID of the client who created the channel
         *   channelID       - ID of the created channel
         */

        public static void onChannelCreated(uint64 serverID, anyID invokerClientID, uint64 channelID)
        {
            Console.WriteLine("Channel {0} created by {1} on virtual server {2}", channelID, invokerClientID, serverID);
        }

        /*
         * Callback when channel has been edited.
         *
         * Parameter:
         *   serverID        - Virtual server ID
         *   invokerClientID - ID of the client who edited the channel
         *   channelID       - ID of the edited channel
         */

        public static void onChannelEdited(uint64 serverID, anyID invokerClientID, uint64 channelID)
        {
            Console.WriteLine("Channel {0} edited by {1} on virtual server {2}", channelID, invokerClientID, serverID);
        }

        /*
         * Callback when channel has been deleted.
         *
         * Parameter:
         *   serverID        - Virtual server ID
         *   invokerClientID - ID of the client who deleted the channel
         *   channelID       - ID of the deleted channel
         */

        public static void onChannelDeleted(uint64 serverID, anyID invokerClientID, uint64 channelID)
        {
            Console.WriteLine("Channel {0} deleted by {1} on virtual server {2}", channelID, invokerClientID, serverID);
        }

        /*
         * Callback when a server text message has been received.
         * Note that only server and channel chats are received, private client messages are not caught due to privacy reasons.
         *
         * Parameter:
         *   serverID        - Virtual server ID
         *   invokerClientID - ID of the client who sent the text message
         *   textMessage     - Message text
         */

        public static void onServerTextMessageEvent(uint64 serverID, anyID invokerClientID, string textMessage)
        {
            Console.WriteLine("Text message in server chat by {0}: {1}", invokerClientID, textMessage);
        }

        /*
         * Callback when a channel text message has been received.
         * Note that only server and channel chats are received, private client messages are not caught due to privacy reasons.
         *
         * Parameter:
         *   serverID        - Virtual server ID
         *   invokerClientID - ID of the client who sent the text message
         *   targetChannelID - ID of the channel in which the chat was sent
         *   textMessage     - Message text
         */

        public static void onChannelTextMessageEvent(uint64 serverID, anyID invokerClientID, uint64 targetChannelID, string textMessage)
        {
            Console.WriteLine("Text message in channel ({0})chat by {1}: {2}", targetChannelID, invokerClientID, textMessage);
        }

        /*
         * Callback for user-defined logging.
         *
         * Parameter:
         *   logMessage        - Log message text
         *   logLevel          - Severity of log message
         *   logChannel        - Custom text to categorize the message channel
         *   logID             - Virtual server ID giving the virtual server source of the log event
         *   logTime           - String with the date and time the log entry occured
         *   completeLogString - Verbose log message including all previous parameters for convinience
         */

        public static void onUserLoggingMessageEvent(string logMessage, int logLevel, string logChannel, uint64 logID, string logTime, string completeLogString)
        {
            /* Your custom error display here... */
            /* Console.WriteLine("LOG: {0}", completeLogString); */
            if (logLevel == (int)LogLevel.LogLevel_CRITICAL)
            {
                System.Environment.Exit(1);  /* Your custom handling of critical errors */
            }
        }

        /*
         * Callback triggered when the specified client starts talking.
         *
         * Parameters:
         *   serverID - ID of the server sending the callback
         *   clientID - ID of the client which started talking
         */

        public static void onClientStartTalkingEvent(uint64 serverID, anyID clientID)
        {
            Console.WriteLine("onClientStartTalkingEvent serverID={0}, clientID={1}", serverID, clientID);
        }

        /*
         * Callback triggered when the specified client stops talking.
         *
         * Parameters:
         *   serverID - ID of the server sending the callback
         *   clientID - ID of the client which stopped talking
         */

        public static void onClientStopTalkingEvent(uint64 serverID, anyID clientID)
        {
            Console.WriteLine("onClientStopTalkingEvent serverID={0}, clientID={1}", serverID, clientID);
        }

        /*
         * Callback triggered when a license error occurs.
         *
         * Parameters:
         *   serverID  - ID of the virtual server on which the license error occured. This virtual server will be automatically
         *               shutdown. If the parameter is zero, all virtual servers are affected and have been shutdown.
         *   errorCode - Code of the occured error. Use ts3server_getGlobalErrorMessage() to convert to a message string
         */

        public static void onAccountingErrorEvent(uint64 serverID, int errorCode)
        {
            Console.WriteLine("onAccountingErrorEvent serverID={0}, errorCode={1}", serverID, errorCode);
            System.Environment.Exit(1);
        }
    }
}