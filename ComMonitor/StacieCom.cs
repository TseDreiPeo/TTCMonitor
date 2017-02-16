using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComMonitor {
    public class StacieCom : MessageFactory {
        public StacieCom(SerialPort sp) : base(sp) {
            
        }

        public void SendAdusrtRTC(Int32 deltaSeconds) {
            Byte[] buffer = new Byte[49];
            buffer[0] = Package.C_PCKCMD_RECEIVE;
            buffer[1] = Package.C_PCKTYP_EXEC;
            buffer[2] = 0x13;        // PID
            buffer[3] = 43;          // Len/Pckcnt
            buffer[4] = 0xFF;        // Pck Nr

            int i = 5;
            for(; i < 37; i++) {
                buffer[i] = (byte)i;  // dummy adcs Data;
            }
            buffer[i++] = (byte)(deltaSeconds & 0x000000FF);
            buffer[i++] = (byte)((deltaSeconds & 0x0000FF00) >> 8);
            buffer[i++] = (byte)((deltaSeconds & 0x00FF0000) >> 16);
            buffer[i++] = (byte)((deltaSeconds & 0xFF000000) >> 24);
            for(; i < 48; i++) {
                buffer[i] = 0x55;  // dummy filler;
            }
            var crc = new CRC8Calc(CRC8_POLY.CRC8_CCITT);
            buffer[48] = crc.Checksum(2, 46, buffer);
            Port.Write(buffer, 0, 49);
        }

        public void SendTransmitAck() {
            Byte[] buffer = new Byte[49];
            buffer[0] = Package.C_PCKCMD_TRANSMIT;
            buffer[1] = Package.C_PCKTYP_ACK;
            Port.Write(buffer, 0, 2);
        }
    }
}
