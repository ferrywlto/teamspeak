namespace teamspeak.constant.error
{
    public class Error
    {
        //The idea here is: the values are 2 bytes wide, the first byte identifies the group, the second the count within that group

        //general
        public const uint ok = 0x0000;

        public const uint undefined = 0x0001;
        public const uint not_implemented = 0x0002;
        public const uint ok_no_Update = 0x0003;
        public const uint dont_Notify = 0x0004;

        //dunno
        public const uint command_not_found = 0x0100;

        public const uint unable_to_bind_network_port = 0x0101;
        public const uint no_network_port_available = 0x0102;

        //client
        public const uint client_invalid_id = 0x0200;

        public const uint client_nickname_inuse = 0x0201;
        public const uint client_protocol_limit_reached = 0x0203;
        public const uint client_invalid_type = 0x0204;
        public const uint client_already_subscribed = 0x0205;
        public const uint client_not_logged_in = 0x0206;
        public const uint client_could_not_validate_identity = 0x0207;
        public const uint client_version_outdated = 0x020a;

        //channel
        public const uint channel_invalid_id = 0x0300;

        public const uint channel_protocol_limit_reached = 0x0301;
        public const uint channel_already_in = 0x0302;
        public const uint channel_name_inuse = 0x0303;
        public const uint channel_not_empty = 0x0304;
        public const uint channel_can_not_delete_default = 0x0305;
        public const uint channel_default_require_permanent = 0x0306;
        public const uint channel_invalid_flags = 0x0307;
        public const uint channel_parent_not_permanent = 0x0308;
        public const uint channel_maxclients_reached = 0x0309;
        public const uint channel_maxfamily_reached = 0x030a;
        public const uint channel_invalid_order = 0x030b;
        public const uint channel_no_filetransfer_supported = 0x030c;
        public const uint channel_invalid_password = 0x030d;

        //server
        public const uint server_invalid_id = 0x0400;

        public const uint server_running = 0x0401;
        public const uint server_is_shutting_down = 0x0402;
        public const uint server_maxclients_reached = 0x0403;
        public const uint server_invalid_password = 0x0404;
        public const uint server_is_virtual = 0x0407;
        public const uint server_is_not_running = 0x0409;
        public const uint server_is_booting = 0x040a;
        public const uint server_status_invalid = 0x040b;

        //parameter
        public const uint parameter_quote = 0x0600;

        public const uint parameter_invalid_count = 0x0601;
        public const uint parameter_invalid = 0x0602;
        public const uint parameter_not_found = 0x0603;
        public const uint parameter_convert = 0x0604;
        public const uint parameter_invalid_size = 0x0605;
        public const uint parameter_missing = 0x0606;

        //unsorted, need further investigation
        public const uint vs_critical = 0x0700;

        public const uint connection_lost = 0x0701;
        public const uint not_connected = 0x0702;
        public const uint no_cached_connection_info = 0x0703;
        public const uint currently_not_possible = 0x0704;
        public const uint failed_connection_initialisation = 0x0705;
        public const uint could_not_resolve_hostname = 0x0706;
        public const uint invalid_server_connection_handler_id = 0x0707;
        public const uint could_not_initialise_input_manager = 0x0708;
        public const uint clientlibrary_not_initialised = 0x0709;
        public const uint serverlibrary_not_initialised = 0x070a;
        public const uint whisper_too_many_targets = 0x070b;
        public const uint whisper_no_targets = 0x070c;

        //sound
        public const uint sound_preprocessor_disabled = 0x0900;

        public const uint sound_internal_preprocessor = 0x0901;
        public const uint sound_internal_encoder = 0x0902;
        public const uint sound_internal_playback = 0x0903;
        public const uint sound_no_capture_device_available = 0x0904;
        public const uint sound_no_playback_device_available = 0x0905;
        public const uint sound_could_not_open_capture_device = 0x0906;
        public const uint sound_could_not_open_playback_device = 0x0907;
        public const uint sound_handler_has_device = 0x0908;
        public const uint sound_invalid_capture_device = 0x0909;
        public const uint sound_invalid_playback_device = 0x090a;
        public const uint sound_invalid_wave = 0x090b;
        public const uint sound_unsupported_wave = 0x090c;
        public const uint sound_open_wave = 0x090d;
        public const uint sound_internal_capture = 0x090e;
        public const uint sound_device_in_use = 0x090f;
        public const uint sound_device_already_registerred = 0x0910;
        public const uint sound_unknown_device = 0x0911;
        public const uint sound_unsupported_frequency = 0x0912;
        public const uint sound_invalid_channel_count = 0x0913;
        public const uint sound_read_wave = 0x0914;
        public const uint sound_need_more_data = 0x0915; //for internal purposes only
        public const uint sound_device_busy = 0x0916; //for internal purposes only
        public const uint sound_no_data = 0x0917; //for internal purposes only

        //accounting
        public const uint accounting_virtualserver_limit_reached = 0x0b00;

        public const uint accounting_slot_limit_reached = 0x0b01;
        public const uint accounting_license_file_not_found = 0x0b02;
        public const uint accounting_license_date_not_ok = 0x0b03;
        public const uint accounting_unable_to_connect_to_server = 0x0b04;
        public const uint accounting_unknown_error = 0x0b05;
        public const uint accounting_server_error = 0x0b06;
        public const uint accounting_instance_limit_reached = 0x0b07;
        public const uint accounting_instance_check_error = 0x0b08;
        public const uint accounting_license_file_invalid = 0x0b09;
        public const uint accounting_running_elsewhere = 0x0b0a;
        public const uint accounting_instance_duplicated = 0x0b0b;
        public const uint accounting_already_started = 0x0b0c;
        public const uint accounting_not_started = 0x0b0d;
        public const uint accounting_to_many_starts = 0x0b0e;
    }
}

namespace teamspeak.enumeration.client
{
    public enum Visibility
    {
        ENTER_VISIBILITY = 0,
        RETAIN_VISIBILITY,
        LEAVE_VISIBILITY
    };

    public enum ConnectStatus
    {
        STATUS_DISCONNECTED = 0,       //There is no activity to the server, this is the default value
        STATUS_CONNECTING,             //We are trying to connect, we haven't got a clientID yet, we haven't been accepted by the server
        STATUS_CONNECTED,              //The server has accepted us, we can talk and hear and we got a clientID, but we don't have the channels and clients yet, we can get server infos (welcome msg etc.)
        STATUS_CONNECTION_ESTABLISHING,//we are CONNECTED and we are visible
        STATUS_CONNECTION_ESTABLISHED, //we are CONNECTED and we have the client and channels available
    };
}

namespace teamspeak.enumeration.server
{
    public enum TalkStatus
    {
        STATUS_NOT_TALKING = 0,
        STATUS_TALKING = 1,
        STATUS_TALKING_WHILE_DISABLED = 2,
    };

    public enum SpeexCodec
    {
        CODEC_SPEEX_NARROWBAND = 0,   //mono, 16bit,  8kHz, bitrate dependant on the quality setting
        CODEC_SPEEX_WIDEBAND,         //mono, 16bit, 16kHz, bitrate dependant on the quality setting
        CODEC_SPEEX_ULTRAWIDEBAND,    //mono, 16bit, 32kHz, bitrate dependant on the quality setting
        CODEC_CELT_MONO,              //mono, 16bit, 48kHz, bitrate dependant on the quality setting
        CODEC_DUMMY_MONO,             //mono, 16bit, 48kHz, no compression (=> bitrate == 93.75 KiB/s!)
    };

    public enum TextMessageTargetMode
    {
        TextMessageTarget_CLIENT = 1,
        TextMessageTarget_CHANNEL,
        TextMessageTarget_SERVER,
        TextMessageTarget_MAX
    };

    public enum MuteInputStatus
    {
        MUTEINPUT_NONE = 0,
        MUTEINPUT_MUTED,
    };

    public enum MuteOutputStatus
    {
        MUTEOUTPUT_NONE = 0,
        MUTEOUTPUT_MUTED,
    };

    public enum HardwareInputStatus
    {
        HARDWAREINPUT_DISABLED = 0,
        HARDWAREINPUT_ENABLED,
    };

    public enum HardwareOutputStatus
    {
        HARDWAREOUTPUT_DISABLED = 0,
        HARDWAREOUTPUT_ENABLED,
    };

    public enum InputDeactivationStatus
    {
        INPUT_ACTIVE = 0,
        INPUT_DEACTIVATED = 1,
    };

    public enum ReasonIdentifier
    {
        REASON_NONE = 0,  //no reason data
        REASON_MOVED = 1,  //{SectionInvoker}
        REASON_SUBSCRIPTION = 2,  //no reason data
        REASON_LOST_CONNECTION = 3,  //reasonmsg=reason
        REASON_KICK_CHANNEL = 4,  //{SectionInvoker} reasonmsg=reason               //{SectionInvoker} is only added server->client
        REASON_KICK_SERVER = 5,  //{SectionInvoker} reasonmsg=reason               //{SectionInvoker} is only added server->client
        REASON_KICK_SERVER_BAN = 6,  //{SectionInvoker} reasonmsg=reason bantime=time  //{SectionInvoker} is only added server->client
        REASON_SERVERSTOP = 7,  //reasonmsg=reason
        REASON_CLIENTDISCONNECT = 8,  //reasonmsg=reason
        REASON_CHANNELUPDATE = 9,  //no reason data
        REASON_CHANNELEDIT = 10, //{SectionInvoker}
        REASON_CLIENTDISCONNECT_SERVER_SHUTDOWN = 11,  //reasonmsg=reason
    };

    public enum ChannelProperties
    {
        CHANNEL_NAME = 0,                       //Available for all channels that are "in view", always up-to-date
        CHANNEL_TOPIC,                          //Available for all channels that are "in view", always up-to-date
        CHANNEL_DESCRIPTION,                    //Must be requested (=> requestChannelDescription)
        CHANNEL_PASSWORD,                       //not available client side
        CHANNEL_CODEC,                          //Available for all channels that are "in view", always up-to-date
        CHANNEL_CODEC_QUALITY,                  //Available for all channels that are "in view", always up-to-date
        CHANNEL_MAXCLIENTS,                     //Available for all channels that are "in view", always up-to-date
        CHANNEL_MAXFAMILYCLIENTS,               //Available for all channels that are "in view", always up-to-date
        CHANNEL_ORDER,                          //Available for all channels that are "in view", always up-to-date
        CHANNEL_FLAG_PERMANENT,                 //Available for all channels that are "in view", always up-to-date
        CHANNEL_FLAG_SEMI_PERMANENT,            //Available for all channels that are "in view", always up-to-date
        CHANNEL_FLAG_DEFAULT,                   //Available for all channels that are "in view", always up-to-date
        CHANNEL_FLAG_PASSWORD,                  //Available for all channels that are "in view", always up-to-date
        CHANNEL_CODEC_LATENCY_FACTOR,           //Available for all channels that are "in view", always up-to-date
        CHANNEL_CODEC_IS_UNENCRYPTED,           //Available for all channels that are "in view", always up-to-date
        CHANNEL_ENDMARKER,
    };

    public enum ClientProperties
    {
        CLIENT_UNIQUE_IDENTIFIER = 0,           //automatically up-to-date for any client "in view", can be used to identify this particular client installation
        CLIENT_NICKNAME,                        //automatically up-to-date for any client "in view"
        CLIENT_VERSION,                         //for other clients than ourself, this needs to be requested (=> requestClientVariables)
        CLIENT_PLATFORM,                        //for other clients than ourself, this needs to be requested (=> requestClientVariables)
        CLIENT_FLAG_TALKING,                    //automatically up-to-date for any client that can be heard (in room / whisper)
        CLIENT_INPUT_MUTED,                     //automatically up-to-date for any client "in view", this clients microphone mute status
        CLIENT_OUTPUT_MUTED,                    //automatically up-to-date for any client "in view", this clients headphones/speakers mute status
        CLIENT_OUTPUTONLY_MUTED,                //automatically up-to-date for any client "in view", this clients headphones/speakers only mute status
        CLIENT_INPUT_HARDWARE,                  //automatically up-to-date for any client "in view", this clients microphone hardware status (is the capture device opened?)
        CLIENT_OUTPUT_HARDWARE,                 //automatically up-to-date for any client "in view", this clients headphone/speakers hardware status (is the playback device opened?)
        CLIENT_INPUT_DEACTIVATED,               //only usable for ourself, not propagated to the network
        CLIENT_IDLE_TIME,                       //internal use
        CLIENT_DEFAULT_CHANNEL,                 //only usable for ourself, the default channel we used to connect on our last connection attempt
        CLIENT_DEFAULT_CHANNEL_PASSWORD,        //internal use
        CLIENT_SERVER_PASSWORD,                 //internal use
        CLIENT_META_DATA,                       //automatically up-to-date for any client "in view", not used by TeamSpeak, free storage for sdk users
        CLIENT_IS_MUTED,                        //only make sense on the client side locally, "1" if this client is currently muted by us, "0" if he is not
        CLIENT_IS_RECORDING,                    //automatically up-to-date for any client "in view"
        CLIENT_VOLUME_MODIFICATOR,				//internal use
        CLIENT_ENDMARKER,
    };

    public enum VirtualServerProperties
    {
        VIRTUALSERVER_UNIQUE_IDENTIFIER = 0,    //available when connected, can be used to identify this particular server installation
        VIRTUALSERVER_NAME,                     //available and always up-to-date when connected
        VIRTUALSERVER_WELCOMEMESSAGE,           //available when connected, not updated while connected
        VIRTUALSERVER_PLATFORM,                 //available when connected
        VIRTUALSERVER_VERSION,                  //available when connected
        VIRTUALSERVER_MAXCLIENTS,               //only available on request (=> requestServerVariables), stores the maximum number of clients that may currently join the server
        VIRTUALSERVER_PASSWORD,                 //not available to clients, the server password
        VIRTUALSERVER_CLIENTS_ONLINE,           //only available on request (=> requestServerVariables),
        VIRTUALSERVER_CHANNELS_ONLINE,          //only available on request (=> requestServerVariables),
        VIRTUALSERVER_CREATED,                  //available when connected, stores the time when the server was created
        VIRTUALSERVER_UPTIME,                   //only available on request (=> requestServerVariables), the time since the server was started
        VIRTUALSERVER_CODEC_ENCRYPTION_MODE,    //available and always up-to-date when connected
        VIRTUALSERVER_ENDMARKER,
    };

    public enum LogTypes
    {
        LogType_NONE = 0x0000,
        LogType_FILE = 0x0001,
        LogType_CONSOLE = 0x0002,
        LogType_USERLOGGING = 0x0004,
        LogType_NO_NETLOGGING = 0x0008,
        LogType_DATABASE = 0x0010,
    };

    public enum LogLevel
    {
        LogLevel_CRITICAL = 0, //these messages stop the program
        LogLevel_ERROR,        //everything that is really bad, but not so bad we need to shut down
        LogLevel_WARNING,      //everything that *might* be bad
        LogLevel_DEBUG,        //output that might help find a problem
        LogLevel_INFO,         //informational output, like "starting database version x.y.z"
        LogLevel_DEVEL         //developer only output (will not be displayed in release mode)
    };
}