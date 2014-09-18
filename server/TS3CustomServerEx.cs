using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using teamspeak.definitions;

namespace teamspeak
{
    /* Ferry: This class created to demostrate how to implement extra events for server */
    public class TS3CustomServerEx : TS3CustomServer
    {
        /* These events are seldom use, so if you need a specific feature set for your server,
         you can reference this and the TSCustomServer to make your own. */
        #region Event Declarations
        public event VoiceDataEvent VoiceData;
        public event CustomPacketEncryptEvent CustomPacketEncrypt;
        public event CustomPacketDecryptEvent CustomPacketDecrypt;
        #endregion

        #region Event Handlers
        void onCustomPacketEncrypt(string dataToSend, ref uint sizeOfData)
        {
            if (CustomPacketEncrypt != null)
                CustomPacketEncrypt(dataToSend, ref sizeOfData);
        }

        void onCustomPacketDecrypt(string dataReceived, ref uint sizeOfData)
        {
            if (CustomPacketDecrypt != null)
                CustomPacketDecrypt(dataReceived, ref sizeOfData);
        }

        void onVoiceData(ulong serverID, ushort clientID, string voiceData, uint voiceDataSize, uint frequency)
        {
            if (VoiceData != null) 
                VoiceData(serverID, clientID, voiceData, voiceDataSize, frequency);
        }

        protected override void initMapper()
        {
            base.initMapper();
            _mapper.onCustomPacketEncrypt = onCustomPacketEncrypt;
            _mapper.onCustomPacketDecrypt = onCustomPacketDecrypt;
        }
        #endregion
    }
}
