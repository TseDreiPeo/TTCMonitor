using System;

namespace ComMonitor {
    public abstract class Package {
        public const int C_MAX_PACKAGE_SIZE = 50;

        private const byte C_PCKCMD_TRANSMIT = 0x07;
        private const byte C_PCKCMD_RECEIVE = 0x1E;
        private const byte C_PCKCMD_GETTELEMETRY = 0x2D;
        private const byte C_PCKCMD_OPMODE = 0x2A;

        private const byte C_PCKTYP_EXEC = 0x4b;
        private const byte C_PCKTYP_ACK = 0x00;
        private const byte C_PCKTYP_NAK = 0xFF;

        public DateTime Time { get; internal set; }
        private byte[] RawData = new byte[C_MAX_PACKAGE_SIZE];
        public byte crc;
        public bool crcOk = true;

        public int ExpectedLength { get { return GetExpectedLength(); } }

        public Package(DateTime creationTime) {
            Time = creationTime;
        }

        protected abstract int GetExpectedLength();

        public virtual void FillData(byte[] currentPackageBytes) {
            Array.Copy(currentPackageBytes, RawData, ExpectedLength);

            if(ExpectedLength > 2) { 
                // Check the received Checksum....
                crc = RawData[ExpectedLength - 1];
                byte[] dataOnly = new byte[ExpectedLength-3];
                Array.Copy(RawData, 2, dataOnly, 0, ExpectedLength - 3);
                
                byte calculated = crcCalculator.Checksum(dataOnly);
                if (crc == calculated) {
                    crcOk = true;
                } else {
                    crcOk = false;
                }

            }
        }

            
        public string GetDebugText() {
            String retVal = String.Empty;
            retVal += this.ToString() + Environment.NewLine;
            
            for (int i = 1; i<=GetExpectedLength(); i++) { 
                retVal += RawData[i-1].ToString("X2") + " ";
                if (i%8 == 0) {
                    retVal += Environment.NewLine;
                }
            }
            if (!crcOk) {
                retVal += Environment.NewLine + "*** CRC ERROR ***";
            }

            return retVal;
        }

        private static CRC8Calc crcCalculator = new CRC8Calc(CRC8_POLY.CRC8_CCITT);

        // Factory Methods - static
        public static Package CreateStacieObcPackage(DateTime creationTime, byte pckCmd, byte pckTyp, bool DirIn) {
            if(pckCmd == C_PCKCMD_TRANSMIT) {
                if(pckTyp == C_PCKTYP_EXEC) {
                    return new TransmitExec(creationTime);
                } else if(pckTyp == C_PCKTYP_ACK) {
                    return new TransmitAck(creationTime);
                } else if(pckTyp == C_PCKTYP_NAK) {
                    return new TransmitNak(creationTime);
                }
            } else if(pckCmd == C_PCKCMD_GETTELEMETRY) {
                if(pckTyp == C_PCKTYP_EXEC) {
                    return new GetTelemetryExec(creationTime);
                } else if(pckTyp == C_PCKTYP_ACK) {
                    return new GetTelemetryAck(creationTime);
                } else if(pckTyp == C_PCKTYP_NAK) {
                    return new GetTelemetryNak(creationTime);
                }
            } else if(pckCmd == C_PCKCMD_OPMODE) {
                if(pckTyp == C_PCKTYP_EXEC) {
                    if(DirIn) {
                        return new OpmodeExecIn(creationTime);
                    } else {
                        return new OpmodeExecOut(creationTime);
                    }
                } else if(pckTyp == C_PCKTYP_ACK) {
                    return new OpmodeAck(creationTime);
                } else if(pckTyp == C_PCKTYP_NAK) {
                    return new OpmodeNak(creationTime);
                }
            } else if(pckCmd == C_PCKCMD_RECEIVE) {
                if(pckTyp == C_PCKTYP_EXEC) {
                    return new ReceiveExec(creationTime);
                } else if(pckTyp == C_PCKTYP_ACK) {
                    return new ReceiveAck(creationTime);
                } else if(pckTyp == C_PCKTYP_NAK) {
                    return new ReceiveNak(creationTime);
                }
            }
            return new UnknownPackage(creationTime);
        }

    }
}