﻿using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.Devices.I2c;

namespace MirrorSUPINFO.Components.ComponentModel.Hardware
{
    public class SparkFun_APDS9960
    {
        #region Enumerations

        /* Direction definitions */
        public enum DirectionDefinitions
        {
            DIR_NONE,
            DIR_LEFT,
            DIR_RIGHT,
            DIR_UP,
            DIR_DOWN,
            DIR_NEAR,
            DIR_FAR,
            DIR_ALL
        };

        /* State definitions */
        private enum StateDefinitions
        {
            NA_STATE,
            NEAR_STATE,
            FAR_STATE,
            ALL_STATE
        };

        #endregion

        #region Structures

        /* Container for gesture data */
        private struct GestureDataType
        {
            internal byte[] UData;
            internal byte[] DData;
            internal byte[] LData;
            internal byte[] RData;
            internal byte Index;
            internal byte TotalGestures;
            internal byte InThreshold;
            internal byte OutThreshold;
        };

        #endregion

        #region Consts

        /* APDS-9960 I2C address */
        internal const byte APDS9960_I2C_ADDR = 0x39;

        /* Gesture parameters */
        private const byte GESTURE_THRESHOLD_OUT = 10;
        private const byte GESTURE_SENSITIVITY_1 = 50;
        private const byte GESTURE_SENSITIVITY_2 = 20;

        /* Error code for returned values */
        private const byte ERROR = 0xFF;

        /* Acceptable device IDs */
        private const byte APDS9960_ID_1 = 0xAB;
        private const byte APDS9960_ID_2 = 0x9C;

        /* Misc parameters */
        private const byte FIFO_PAUSE_TIME = 30; // Wait period (ms) between FIFO reads

        /* APDS-9960 register addresses */
        private const byte APDS9960_ENABLE = 0x80;
        private const byte APDS9960_ATIME = 0x81;
        private const byte APDS9960_WTIME = 0x83;
        private const byte APDS9960_AILTL = 0x84;
        private const byte APDS9960_AILTH = 0x85;
        private const byte APDS9960_AIHTL = 0x86;
        private const byte APDS9960_AIHTH = 0x87;
        private const byte APDS9960_PILT = 0x89;
        private const byte APDS9960_PIHT = 0x8B;
        private const byte APDS9960_PERS = 0x8C;
        private const byte APDS9960_CONFIG1 = 0x8D;
        private const byte APDS9960_PPULSE = 0x8E;
        private const byte APDS9960_CONTROL = 0x8F;
        private const byte APDS9960_CONFIG2 = 0x90;
        private const byte APDS9960_ID = 0x92;
        private const byte APDS9960_STATUS = 0x93;
        private const byte APDS9960_CDATAL = 0x94;
        private const byte APDS9960_CDATAH = 0x95;
        private const byte APDS9960_RDATAL = 0x96;
        private const byte APDS9960_RDATAH = 0x97;
        private const byte APDS9960_GDATAL = 0x98;
        private const byte APDS9960_GDATAH = 0x99;
        private const byte APDS9960_BDATAL = 0x9A;
        private const byte APDS9960_BDATAH = 0x9B;
        private const byte APDS9960_PDATA = 0x9C;
        private const byte APDS9960_POFFSET_UR = 0x9D;
        private const byte APDS9960_POFFSET_DL = 0x9E;
        private const byte APDS9960_CONFIG3 = 0x9F;
        private const byte APDS9960_GPENTH = 0xA0;
        private const byte APDS9960_GEXTH = 0xA1;
        private const byte APDS9960_GCONF1 = 0xA2;
        private const byte APDS9960_GCONF2 = 0xA3;
        private const byte APDS9960_GOFFSET_U = 0xA4;
        private const byte APDS9960_GOFFSET_D = 0xA5;
        private const byte APDS9960_GOFFSET_L = 0xA7;
        private const byte APDS9960_GOFFSET_R = 0xA9;
        private const byte APDS9960_GPULSE = 0xA6;
        private const byte APDS9960_GCONF3 = 0xAA;
        private const byte APDS9960_GCONF4 = 0xAB;
        private const byte APDS9960_GFLVL = 0xAE;
        private const byte APDS9960_GSTATUS = 0xAF;
        private const byte APDS9960_IFORCE = 0xE4;
        private const byte APDS9960_PICLEAR = 0xE5;
        private const byte APDS9960_CICLEAR = 0xE6;
        private const byte APDS9960_AICLEAR = 0xE7;
        private const byte APDS9960_GFIFO_U = 0xFC;
        private const byte APDS9960_GFIFO_D = 0xFD;
        private const byte APDS9960_GFIFO_L = 0xFE;
        private const byte APDS9960_GFIFO_R = 0xFF;

        /* Bit fields */
        private const byte APDS9960_PON = 1;
        private const byte APDS9960_AEN = 2;
        private const byte APDS9960_PEN = 4;
        private const byte APDS9960_WEN = 8;
        private const byte APSD9960_AIEN = 16;
        private const byte APDS9960_PIEN = 32;
        private const byte APDS9960_GEN = 64;
        private const byte APDS9960_GVALID = 1;

        /* On/Off definitions */
        private const byte OFF = 0;
        private const byte ON = 1;

        /* Acceptable parameters for setMode */
        private const byte POWER = 0;
        private const byte AMBIENT_LIGHT = 1;
        private const byte PROXIMITY = 2;
        private const byte WAIT = 3;
        private const byte AMBIENT_LIGHT_INT = 4;
        private const byte PROXIMITY_INT = 5;
        private const byte GESTURE = 6;
        private const byte ALL = 7;

        /* LED Drive values */
        private const byte LED_DRIVE_100MA = 0;
        private const byte LED_DRIVE_50MA = 1;
        private const byte LED_DRIVE_25MA = 2;
        private const byte LED_DRIVE_12_5MA = 3;

        /* Proximity Gain (PGAIN) values */
        private const byte PGAIN_1X = 0;
        private const byte PGAIN_2X = 1;
        private const byte PGAIN_4X = 2;
        private const byte PGAIN_8X = 3;

        /* ALS Gain (AGAIN) values */
        private const byte AGAIN_1X = 0;
        private const byte AGAIN_4X = 1;
        private const byte AGAIN_16X = 2;
        private const byte AGAIN_64X = 3;

        /* Gesture Gain (GGAIN) values */
        private const byte GGAIN_1X = 0;
        private const byte GGAIN_2X = 1;
        private const byte GGAIN_4X = 2;
        private const byte GGAIN_8X = 3;

        /* LED Boost values */
        private const byte LED_BOOST_100 = 0;
        private const byte LED_BOOST_150 = 1;
        private const byte LED_BOOST_200 = 2;
        private const byte LED_BOOST_300 = 3;

        /* Gesture wait time values */
        private const byte GWTIME_0MS = 0;
        private const byte GWTIME_2_8MS = 1;
        private const byte GWTIME_5_6MS = 2;
        private const byte GWTIME_8_4MS = 3;
        private const byte GWTIME_14_0MS = 4;
        private const byte GWTIME_22_4MS = 5;
        private const byte GWTIME_30_8MS = 6;
        private const byte GWTIME_39_2MS = 7;

        /* Default values */
        private const byte DEFAULT_ATIME = 219; // 103ms
        private const byte DEFAULT_WTIME = 246; // 27ms
        private const byte DEFAULT_PROX_PPULSE = 0x87; // 16us, 8 pulses
        private const byte DEFAULT_GESTURE_PPULSE = 0x89; // 16us, 10 pulses
        private const byte DEFAULT_POFFSET_UR = 0; // 0 offset
        private const byte DEFAULT_POFFSET_DL = 0; // 0 offset      
        private const byte DEFAULT_CONFIG1 = 0x60; // No 12x wait (WTIME) factor
        private const byte DEFAULT_LDRIVE = LED_DRIVE_100MA;
        private const byte DEFAULT_PGAIN = PGAIN_4X;
        private const byte DEFAULT_AGAIN = AGAIN_4X;
        private const byte DEFAULT_PILT = 0; // Low proximity threshold
        private const byte DEFAULT_PIHT = 50; // High proximity threshold
        private const int DEFAULT_AILT = 0xFFFF; // Force interrupt for calibration
        private const byte DEFAULT_AIHT = 0;
        private const byte DEFAULT_PERS = 0x11; // 2 consecutive prox or ALS for int.
        private const byte DEFAULT_CONFIG2 = 0x01; // No saturation interrupts or LED boost  
        private const byte DEFAULT_CONFIG3 = 0; // Enable all photodiodes, no SAI
        private const byte DEFAULT_GPENTH = 40; // Threshold for entering gesture mode
        private const byte DEFAULT_GEXTH = 30; // Threshold for exiting gesture mode    
        private const byte DEFAULT_GCONF1 = 0x40; // 4 gesture events for int., 1 for exit
        private const byte DEFAULT_GGAIN = GGAIN_4X;
        private const byte DEFAULT_GLDRIVE = LED_DRIVE_100MA;
        private const byte DEFAULT_GWTIME = GWTIME_2_8MS;
        private const byte DEFAULT_GOFFSET = 0; // No offset scaling for gesture mode
        private const byte DEFAULT_GPULSE = 0xC9; // 32us, 10 pulses
        private const byte DEFAULT_GCONF3 = 0; // All photodiodes active during gesture
        private const byte DEFAULT_GIEN = 0; // Disable gesture interrupts

        #endregion

        #region Fields

        private GestureDataType _gestureData;
        private int _gestureUdDelta;
        private int _gestureLrDelta;
        private int _gestureUdCount;
        private int _gestureLrCount;
        private int _gestureNearCount;
        private int _gestureFarCount;
        private StateDefinitions _gestureState;
        private DirectionDefinitions _gestureMotion;

        #endregion

        #region Properties

        private I2cDevice I2C { get; }

        #endregion

        #region Constructors

        public SparkFun_APDS9960(ref I2cDevice i2CDevice)
        {
            I2C = i2CDevice;

            _gestureUdDelta = 0;
            _gestureLrDelta = 0;

            _gestureUdCount = 0;
            _gestureLrCount = 0;

            _gestureNearCount = 0;
            _gestureFarCount = 0;

            _gestureState = StateDefinitions.NA_STATE;
            _gestureMotion = DirectionDefinitions.DIR_NONE;
        }

        #endregion

        #region Methods

        internal bool Initialize()
        {
            /* Read ID register and check against known values for APDS-9960 */
            var id = ReadDataByte(APDS9960_ID);
            if (!(id == APDS9960_ID_1 || id == APDS9960_ID_2))
            {
                return false;
            }

            /* Set ENABLE register to 0 (disable all features) */
            if (!SetMode(ALL, OFF))
            {
                return false;
            }

            /* Set default values for ambient light and proximity registers */
            WriteDataByte(APDS9960_ATIME, DEFAULT_ATIME);
            WriteDataByte(APDS9960_WTIME, DEFAULT_WTIME);
            WriteDataByte(APDS9960_PPULSE, DEFAULT_PROX_PPULSE);
            WriteDataByte(APDS9960_POFFSET_UR, DEFAULT_POFFSET_UR);
            WriteDataByte(APDS9960_POFFSET_DL, DEFAULT_POFFSET_DL);
            WriteDataByte(APDS9960_CONFIG1, DEFAULT_CONFIG1);
            SetLedDrive(DEFAULT_LDRIVE);
            SetProximityGain(DEFAULT_PGAIN);
            SetAmbientLightGain(DEFAULT_AGAIN);
            SetProxIntLowThresh(DEFAULT_PILT);
            SetProxIntHighThresh(DEFAULT_PIHT);
            SetLightIntLowThreshold(DEFAULT_AILT);
            SetLightIntHighThreshold(DEFAULT_AIHT);
            WriteDataByte(APDS9960_PERS, DEFAULT_PERS);
            WriteDataByte(APDS9960_CONFIG2, DEFAULT_CONFIG2);
            WriteDataByte(APDS9960_CONFIG3, DEFAULT_CONFIG3);


            /* Set default values for gesture sense registers */
            SetGestureEnterThresh(DEFAULT_GPENTH);
            SetGestureExitThresh(DEFAULT_GEXTH);
            WriteDataByte(APDS9960_GCONF1, DEFAULT_GCONF1);
            SetGestureGain(DEFAULT_GGAIN);
            SetGestureLedDrive(DEFAULT_GLDRIVE);
            SetGestureWaitTime(DEFAULT_GWTIME);
            WriteDataByte(APDS9960_GOFFSET_U, DEFAULT_GOFFSET);
            WriteDataByte(APDS9960_GOFFSET_D, DEFAULT_GOFFSET);
            WriteDataByte(APDS9960_GOFFSET_L, DEFAULT_GOFFSET);
            WriteDataByte(APDS9960_GOFFSET_R, DEFAULT_GOFFSET);
            WriteDataByte(APDS9960_GPULSE, DEFAULT_GPULSE);
            WriteDataByte(APDS9960_GCONF3, DEFAULT_GCONF3);
            SetGestureIntEnable(DEFAULT_GIEN);

            return true;
        }

        internal byte GetMode()
        {
            /* Read current ENABLE register */
            return ReadDataByte(APDS9960_ENABLE);
        }

        internal bool SetMode(byte mode, byte enable)
        {
            /* Read current ENABLE register */
            var regVal = GetMode();
            if (regVal == ERROR)
            {
                return false;
            }

            /* Change bit(s) in ENABLE register */
            enable = (byte)(enable & 0x01);
            if (mode >= 0 && mode <= 6)
            {
                if (enable == 1)
                {
                    regVal |= (byte)(1 << mode);
                }
                else
                {
                    regVal &= (byte)~(1 << mode);
                }
            }
            else if (mode == ALL)
            {
                if (enable == 1)
                {
                    regVal = 0x7F;
                }
                else
                {
                    regVal = 0x00;
                }
            }

            /* Write value back to ENABLE register */
            WriteDataByte(APDS9960_ENABLE, regVal);
            return true;
        }

        internal void EnableLightSensor(bool interrupts)
        {
            /* Set default gain, interrupts, enable power, and enable sensor */
            SetAmbientLightGain(DEFAULT_AGAIN);
            if (interrupts)
            {
                SetAmbientLightIntEnable(1);
            }
            else
            {
                SetAmbientLightIntEnable(0);
            }
            EnablePower();
            SetMode(AMBIENT_LIGHT, 1);
        }

        internal void DisableLightSensor()
        {
            SetAmbientLightIntEnable(0);
            SetMode(AMBIENT_LIGHT, 0);
        }

        internal void EnableProximitySensor(bool interrupts)
        {
            /* Set default gain, LED, interrupts, enable power, and enable sensor */
            SetProximityGain(DEFAULT_PGAIN);
            SetLedDrive(DEFAULT_LDRIVE);
            if (interrupts)
            {
                SetProximityIntEnable(1);
            }
            else
            {
                SetProximityIntEnable(0);
            }
            EnablePower();
            SetMode(PROXIMITY, 1);
        }

        internal void DisableProximitySensor()
        {
            SetProximityIntEnable(0);
            SetMode(PROXIMITY, 0);
        }

        internal void EnableGestureSensor(bool interrupts)
        {
            /* Enable gesture mode
               Set ENABLE to 0 (power off)
               Set WTIME to 0xFF
               Set AUX to LED_BOOST_300
               Enable PON, WEN, PEN, GEN in ENABLE 
            */
            ResetGestureParameters();
            WriteDataByte(APDS9960_WTIME, 0xFF);
            WriteDataByte(APDS9960_PPULSE, DEFAULT_GESTURE_PPULSE);
            SetLedBoost(LED_BOOST_300);
            if (interrupts)
            {
                SetGestureIntEnable(1);
            }
            else
            {
                SetGestureIntEnable(0);
            }
            SetGestureMode(1);
            EnablePower();
            SetMode(WAIT, 1);
            SetMode(PROXIMITY, 1);
            SetMode(GESTURE, 1);
        }

        internal void DisableGestureSensor()
        {
            ResetGestureParameters();
            SetGestureIntEnable(0);
            SetGestureMode(0);
            SetMode(GESTURE, 0);
        }

        internal bool IsGestureAvailable()
        {
            /* Read value from GSTATUS register */
            var val = ReadDataByte(APDS9960_GSTATUS);

            /* Shift and mask out GVALID bit */
            val &= APDS9960_GVALID;

            /* Return true/false based on GVALID bit */
            if (val == 1)
            {
                return true;
            }
            return false;
        }

        internal DirectionDefinitions ReadGesture()
        {
            byte fifoLevel = 0;
            int bytesRead = 0;
            byte[] fifoData = new byte[128];
            byte gstatus;
            DirectionDefinitions motion;
            int i;

            _gestureData = new GestureDataType();
            _gestureData.UData = new byte[32];
            _gestureData.DData = new byte[32];
            _gestureData.LData = new byte[32];
            _gestureData.RData = new byte[32];

            /* Make sure that power and gesture is on and data is valid */
            var mode = GetMode() & 65;
            if (!IsGestureAvailable() || mode == 0)
            {
                return DirectionDefinitions.DIR_NONE;
            }

            /* Keep looping as long as gesture data is valid */
            while (true)
            {
                /* Wait some time to collect next batch of FIFO data */
                Task.Delay(FIFO_PAUSE_TIME).Wait();

                /* Get the contents of the STATUS register. Is data still valid? */
                gstatus = ReadDataByte(APDS9960_GSTATUS);

                /* If we have valid data, read in FIFO */
                if ((gstatus & APDS9960_GVALID) == APDS9960_GVALID)
                {
                    /* Read the current FIFO level */
                    fifoLevel = ReadDataByte(APDS9960_GFLVL);
#if DEBUG
                    Debug.Write("FIFO Level: ");
                    Debug.WriteLine(fifoLevel);
#endif
                    /* If there's stuff in the FIFO, read it into our data block */
                    if (fifoLevel > 0)
                    {
                        fifoData = ReadDataBlock(APDS9960_GFIFO_U, fifoLevel * 4);
                        bytesRead = fifoData.Length;

                        if (bytesRead == 0)
                        {
                            return DirectionDefinitions.DIR_NONE;
                        }
                        /* If at least 1 set of data, sort the data into U/D/L/R */
                        if (bytesRead >= 4)
                        {
                            for (i = 0; i < bytesRead; i += 4)
                            {
                                _gestureData.UData[_gestureData.Index] = fifoData[i + 0];
                                _gestureData.DData[_gestureData.Index] = fifoData[i + 1];
                                _gestureData.LData[_gestureData.Index] = fifoData[i + 2];
                                _gestureData.RData[_gestureData.Index] = fifoData[i + 3];
                                _gestureData.Index++;
                                _gestureData.TotalGestures++;
                            }

                            /* Filter and process gesture data. Decode near/far state */
                            if (ProcessGestureData())
                            {
                                if (DecodeGesture())
                                {
#if DEBUG
                                    Debug.WriteLine(_gestureMotion);
#endif
                                }
                            }
                            
                            /* Reset data */
                            _gestureData.Index = 0;
                            _gestureData.TotalGestures = 0;
                        }
                    }
                }
                else
                {
                    /* Determine best guessed gesture and clean up */
                    Task.Delay(FIFO_PAUSE_TIME).Wait();
                    DecodeGesture();
                    motion = _gestureMotion;
                    ResetGestureParameters();
                    return motion;
                }
            }
        }

        internal bool EnablePower()
        {
            if (!SetMode(POWER, 1))
            {
                return false;
            }

            return true;
        }

        internal bool DisablePower()
        {
            if (!SetMode(POWER, 0))
            {
                return false;
            }

            return true;
        }

        internal int ReadAmbientLight()
        {
            /* Read value from clear channel, low byte register */
            var valByte = ReadDataByte(APDS9960_CDATAL);
            var val = (int)valByte;

            /* Read value from clear channel, high byte register */
            valByte = ReadDataByte(APDS9960_CDATAH);
            val = val + (valByte << 8);

            return val;
        }

        internal int ReadRedLight()
        {
            /* Read value from clear channel, low byte register */
            var valByte = ReadDataByte(APDS9960_RDATAL);
            var val = (int)valByte;

            /* Read value from clear channel, high byte register */
            valByte = ReadDataByte(APDS9960_RDATAH);
            val = val + (valByte << 8);

            return val;
        }

        internal int ReadGreenLight()
        {
            /* Read value from clear channel, low byte register */
            var valByte = ReadDataByte(APDS9960_GDATAL);
            var val = (int)valByte;

            /* Read value from clear channel, high byte register */
            valByte = ReadDataByte(APDS9960_GDATAH);
            val = val + (valByte << 8);

            return val;
        }

        internal int ReadBlueLight()
        {
            /* Read value from clear channel, low byte register */
            var valByte = ReadDataByte(APDS9960_BDATAL);
            var val = (int)valByte;

            /* Read value from clear channel, high byte register */
            valByte = ReadDataByte(APDS9960_BDATAH);
            val = val + (valByte << 8);

            return val;
        }

        internal byte ReadProximity()
        {
            /* Read value from proximity data register */
            return ReadDataByte(APDS9960_PDATA);
        }

        internal byte GetProxIntLowThresh()
        {
            /* Read value from PLHT register */
            return ReadDataByte(APDS9960_PILT);
        }

        private void SetProxIntLowThresh(byte threshold)
        {
            WriteDataByte(APDS9960_PILT, threshold);
        }

        internal byte GetProxIntHighThresh()
        {
            /* Read value from PIHT register */
            return ReadDataByte(APDS9960_PIHT);
        }

        private void SetProxIntHighThresh(byte threshold)
        {
            WriteDataByte(APDS9960_PIHT, threshold);
        }

        internal byte GetLedDrive()
        {
            /* Read value from CONTROL register */
            var val = ReadDataByte(APDS9960_CONTROL);

            /* Shift and mask out LED drive bits */
            val = (byte)(val >> 6);
            val &= 3;

            return val;
        }

        private void SetLedDrive(byte drive)
        {
            /* Read value from CONTROL register */
            var val = ReadDataByte(APDS9960_CONTROL);

            /* Set bits in register to given value */
            drive &= 3;
            drive = (byte)(drive << 6);
            val &= 63;
            val |= drive;

            /* Write register value back into CONTROL register */
            WriteDataByte(APDS9960_CONTROL, val);
        }

        internal byte GetProximityGain()
        {
            /* Read value from CONTROL register */
            var val = ReadDataByte(APDS9960_CONTROL);

            /* Shift and mask out PDRIVE bits */
            val = (byte)(val >> 2);
            val &= 3;

            return val;
        }

        private void SetProximityGain(byte drive)
        {
            /* Read value from CONTROL register */
            var val = ReadDataByte(APDS9960_CONTROL);

            /* Set bits in register to given value */
            drive &= 3;
            drive = (byte)(drive << 2);
            val &= 243;
            val |= drive;

            /* Write register value back into CONTROL register */
            WriteDataByte(APDS9960_CONTROL, val);
        }

        internal byte GetAmbientLightGain()
        {
            /* Read value from CONTROL register */
            var val = ReadDataByte(APDS9960_CONTROL);

            /* Shift and mask out ADRIVE bits */
            val &= 3;

            return val;
        }

        private void SetAmbientLightGain(byte drive)
        {
            /* Read value from CONTROL register */
            var val = ReadDataByte(APDS9960_CONTROL);

            /* Set bits in register to given value */
            drive &= 3;
            val &= 252;
            val |= drive;

            /* Write register value back into CONTROL register */
            WriteDataByte(APDS9960_CONTROL, val);
        }

        internal byte GetLedBoost()
        {
            /* Read value from CONFIG2 register */
            var val = ReadDataByte(APDS9960_CONFIG2);

            /* Shift and mask out LED_BOOST bits */
            val = (byte)(val >> 4);
            val &= 3;

            return val;
        }

        private void SetLedBoost(byte boost)
        {
            /* Read value from CONFIG2 register */
            var val = ReadDataByte(APDS9960_CONFIG2);

            /* Set bits in register to given value */
            boost &= 3;
            boost = (byte)(boost << 4);
            val &= 207;
            val |= boost;

            /* Write register value back into CONFIG2 register */
            WriteDataByte(APDS9960_CONFIG2, val);
        }

        internal byte GetProxGainCompEnable()
        {
            /* Read value from CONFIG3 register */
            var val = ReadDataByte(APDS9960_CONFIG3);

            /* Shift and mask out PCMP bits */
            val = (byte)(val >> 5);
            val &= 1;

            return val;
        }

        internal void SetProxGainCompEnable(byte enable)
        {
            /* Read value from CONFIG3 register */
            var val = ReadDataByte(APDS9960_CONFIG3);

            /* Set bits in register to given value */
            enable &= 1;
            enable = (byte)(enable << 5);
            val &= 223;
            val |= enable;

            /* Write register value back into CONFIG3 register */
            WriteDataByte(APDS9960_CONFIG3, val);
        }

        internal byte GetProxPhotoMask()
        {
            /* Read value from CONFIG3 register */
            var val = ReadDataByte(APDS9960_CONFIG3);

            /* Mask out photodiode enable mask bits */
            val &= 15;

            return val;
        }

        internal void SetProxPhotoMask(byte mask)
        {
            /* Read value from CONFIG3 register */
            var val = ReadDataByte(APDS9960_CONFIG3);

            /* Set bits in register to given value */
            mask &= 15;
            val &= 240;
            val |= mask;

            /* Write register value back into CONFIG3 register */
            WriteDataByte(APDS9960_CONFIG3, val);
        }

        internal byte GetGestureEnterThresh()
        {
            /* Read value from GPENTH register */
            var val = ReadDataByte(APDS9960_GPENTH);

            return val;
        }

        private void SetGestureEnterThresh(byte threshold)
        {
            WriteDataByte(APDS9960_GPENTH, threshold);
        }

        internal byte GetGestureExitThresh()
        {
            /* Read value from GEXTH register */
            var val = ReadDataByte(APDS9960_GEXTH);

            return val;
        }

        private void SetGestureExitThresh(byte threshold)
        {
            WriteDataByte(APDS9960_GEXTH, threshold);
        }

        internal byte GetGestureGain()
        {
            /* Read value from GCONF2 register */
            var val = ReadDataByte(APDS9960_GCONF2);

            /* Shift and mask out GGAIN bits */
            val = (byte)(val >> 5);
            val &= 3;

            return val;
        }

        private void SetGestureGain(byte gain)
        {
            /* Read value from GCONF2 register */
            var val = ReadDataByte(APDS9960_GCONF2);

            /* Set bits in register to given value */
            gain &= 3;
            gain = (byte)(gain << 5);
            val &= 31;
            val |= gain;

            /* Write register value back into GCONF2 register */
            WriteDataByte(APDS9960_GCONF2, val);
        }

        internal byte GetGestureLedDrive()
        {
            /* Read value from GCONF2 register */
            var val = ReadDataByte(APDS9960_GCONF2);

            /* Shift and mask out GLDRIVE bits */
            val = (byte)(val >> 3);
            val &= 3;

            return val;
        }

        private void SetGestureLedDrive(byte drive)
        {
            /* Read value from GCONF2 register */
            var val = ReadDataByte(APDS9960_GCONF2);

            /* Set bits in register to given value */
            drive &= 3;
            drive = (byte)(drive << 3);
            val &= 231;
            val |= drive;

            /* Write register value back into GCONF2 register */
            WriteDataByte(APDS9960_GCONF2, val);
        }

        internal byte GetGestureWaitTime()
        {
            /* Read value from GCONF2 register */
            var val = ReadDataByte(APDS9960_GCONF2);

            /* Mask out GWTIME bits */
            val &= 7;

            return val;
        }

        private void SetGestureWaitTime(byte time)
        {
            /* Read value from GCONF2 register */
            var val = ReadDataByte(APDS9960_GCONF2);

            /* Set bits in register to given value */
            time &= 7;
            val &= 248;
            val |= time;

            /* Write register value back into GCONF2 register */
            WriteDataByte(APDS9960_GCONF2, val);
        }

        internal void GetLightIntLowThreshold(ref int threshold)
        {
            threshold = 0;

            /* Read value from ambient light low threshold, low byte register */
            var valByte = ReadDataByte(APDS9960_AILTL);
            threshold = valByte;

            /* Read value from ambient light low threshold, high byte register */
            valByte = ReadDataByte(APDS9960_AILTH);
            threshold = threshold + (valByte << 8);
        }

        private void SetLightIntLowThreshold(int threshold)
        {
            byte valLow;
            byte valHigh;

            /* Break 16-bit threshold into 2 8-bit values */
            valLow = (byte)(threshold & 0x00FF);
            valHigh = (byte)((threshold & 0xFF00) >> 8);

            /* Write low byte */
            WriteDataByte(APDS9960_AILTL, valLow);

            /* Write high byte */
            WriteDataByte(APDS9960_AILTH, valHigh);
        }

        internal void GetLightIntHighThreshold(ref int threshold)
        {
            threshold = 0;

            /* Read value from ambient light high threshold, low byte register */
            var valByte = ReadDataByte(APDS9960_AIHTL);
            threshold = valByte;

            /* Read value from ambient light high threshold, high byte register */
            valByte = ReadDataByte(APDS9960_AIHTH);
            threshold = (threshold + (valByte << 8));
        }

        private void SetLightIntHighThreshold(int threshold)
        {
            byte valLow;
            byte valHigh;

            /* Break 16-bit threshold into 2 8-bit values */
            valLow = (byte)(threshold & 0x00FF);
            valHigh = (byte)((threshold & 0xFF00) >> 8);

            /* Write low byte */
            WriteDataByte(APDS9960_AIHTL, valLow);

            /* Write high byte */
            WriteDataByte(APDS9960_AIHTH, valHigh);
        }

        internal bool GetProximityIntLowThreshold(ref byte threshold)
        {
            threshold = 0;

            /* Read value from proximity low threshold register */
            if (ReadDataByte(APDS9960_PILT) == 0)
            {
                return false;
            }
            return true;
        }

        internal void SetProximityIntLowThreshold(byte threshold)
        {
            /* Write threshold value to register */
            WriteDataByte(APDS9960_PILT, threshold);
        }

        internal bool GetProximityIntHighThreshold(ref byte threshold)
        {
            threshold = 0;

            /* Read value from proximity low threshold register */
            if (ReadDataByte(APDS9960_PIHT) == 0)
            {
                return false;
            }
            return true;
        }

        internal void SetProximityIntHighThreshold(byte threshold)
        {
            /* Write threshold value to register */
            WriteDataByte(APDS9960_PIHT, threshold);
        }

        internal byte GetAmbientLightIntEnable()
        {
            /* Read value from ENABLE register */
            var val = ReadDataByte(APDS9960_ENABLE);

            /* Shift and mask out AIEN bit */
            val = (byte)(val >> 4);
            val &= 1;

            return val;
        }

        private void SetAmbientLightIntEnable(byte enable)
        {
            /* Read value from ENABLE register */
            var val = ReadDataByte(APDS9960_ENABLE);

            /* Set bits in register to given value */
            enable &= 1;
            enable = (byte)(enable << 4);
            val &= 239;
            val |= enable;

            /* Write register value back into ENABLE register */
            WriteDataByte(APDS9960_ENABLE, val);
        }

        internal byte GetProximityIntEnable()
        {
            /* Read value from ENABLE register */
            var val = ReadDataByte(APDS9960_ENABLE);

            /* Shift and mask out PIEN bit */
            val = (byte)(val >> 5);
            val &= 1;

            return val;
        }

        private void SetProximityIntEnable(byte enable)
        {
            /* Read value from ENABLE register */
            var val = ReadDataByte(APDS9960_ENABLE);

            /* Set bits in register to given value */
            enable &= 1;
            enable = (byte)(enable << 5);
            val &= 223;
            val |= enable;

            /* Write register value back into ENABLE register */
            WriteDataByte(APDS9960_ENABLE, val);
        }

        internal byte GetGestureIntEnable()
        {
            /* Read value from GCONF4 register */
            var val = ReadDataByte(APDS9960_GCONF4);

            /* Shift and mask out GIEN bit */
            val = (byte)(val >> 1);
            val &= 1;

            return val;
        }

        private void SetGestureIntEnable(byte enable)
        {
            /* Read value from GCONF4 register */
            var val = ReadDataByte(APDS9960_GCONF4);

            /* Set bits in register to given value */
            enable &= 1;
            enable = (byte)(enable << 1);
            val &= 253;
            val |= enable;

            /* Write register value back into GCONF4 register */
            WriteDataByte(APDS9960_GCONF4, val);
        }

        internal void ClearAmbientLightInt()
        {
            ReadDataByte(APDS9960_AICLEAR);
        }

        internal void ClearProximityInt()
        {
            ReadDataByte(APDS9960_PICLEAR);
        }

        internal byte GetGestureMode()
        {
            /* Read value from GCONF4 register */
            var val = ReadDataByte(APDS9960_GCONF4);

            /* Mask out GMODE bit */
            val &= 1;

            return val;
        }

        private void SetGestureMode(byte mode)
        {
            /* Read value from GCONF4 register */
            var val = ReadDataByte(APDS9960_GCONF4);

            /* Set bits in register to given value */
            mode &= 1;
            val &= 254;
            val |= mode;

            /* Write register value back into GCONF4 register */
            WriteDataByte(APDS9960_GCONF4, val);
        }

        private void ResetGestureParameters()
        {
            _gestureData.Index = 0;
            _gestureData.TotalGestures = 0;

            _gestureUdDelta = 0;
            _gestureLrDelta = 0;

            _gestureUdCount = 0;
            _gestureLrCount = 0;

            _gestureNearCount = 0;
            _gestureFarCount = 0;

            _gestureState = StateDefinitions.NA_STATE;
            _gestureMotion = DirectionDefinitions.DIR_NONE;
        }

        private bool ProcessGestureData()
        {
            byte uFirst = 0;
            byte dFirst = 0;
            byte lFirst = 0;
            byte rFirst = 0;
            byte uLast = 0;
            byte dLast = 0;
            byte lLast = 0;
            byte rLast = 0;
            int udRatioFirst;
            int lrRatioFirst;
            int udRatioLast;
            int lrRatioLast;
            int udDelta;
            int lrDelta;
            int i;

            /* If we have less than 4 total gestures, that's not enough */
            if (_gestureData.TotalGestures <= 4)
            {
                return false;
            }

            /* Check to make sure our data isn't out of bounds */
            if ((_gestureData.TotalGestures <= 32) && (_gestureData.TotalGestures > 0))
            {
                /* Find the first value in U/D/L/R above the threshold */
                for (i = 0; i < _gestureData.TotalGestures; i++)
                {
                    if ((_gestureData.UData[i] > GESTURE_THRESHOLD_OUT) && (_gestureData.DData[i] > GESTURE_THRESHOLD_OUT) && (_gestureData.LData[i] > GESTURE_THRESHOLD_OUT) && (_gestureData.RData[i] > GESTURE_THRESHOLD_OUT))
                    {
                        uFirst = _gestureData.UData[i];
                        dFirst = _gestureData.DData[i];
                        lFirst = _gestureData.LData[i];
                        rFirst = _gestureData.RData[i];
                        break;
                    }
                }

                /* If one of the _first values is 0, then there is no good data */
                if ((uFirst == 0) || (dFirst == 0) || (lFirst == 0) || (rFirst == 0))
                {
                    return false;
                }
                /* Find the last value in U/D/L/R above the threshold */
                for (i = _gestureData.TotalGestures - 1; i >= 0; i--)
                {
                    if ((_gestureData.UData[i] > GESTURE_THRESHOLD_OUT) && (_gestureData.DData[i] > GESTURE_THRESHOLD_OUT) && (_gestureData.LData[i] > GESTURE_THRESHOLD_OUT) && (_gestureData.RData[i] > GESTURE_THRESHOLD_OUT))
                    {
                        uLast = _gestureData.UData[i];
                        dLast = _gestureData.DData[i];
                        lLast = _gestureData.LData[i];
                        rLast = _gestureData.RData[i];
                        break;
                    }
                }
            }

            /* Calculate the first vs. last ratio of up/down and left/right */
            udRatioFirst = ((uFirst - dFirst) * 100) / (uFirst + dFirst);
            lrRatioFirst = ((lFirst - rFirst) * 100) / (lFirst + rFirst);
            udRatioLast = ((uLast - dLast) * 100) / (uLast + dLast);
            lrRatioLast = ((lLast - rLast) * 100) / (lLast + rLast);
            /* Determine the difference between the first and last ratios */
            udDelta = udRatioLast - udRatioFirst;
            lrDelta = lrRatioLast - lrRatioFirst;
            /* Accumulate the UD and LR delta values */
            _gestureUdDelta += udDelta;
            _gestureLrDelta += lrDelta;
            /* Determine U/D gesture */
            if (_gestureUdDelta >= GESTURE_SENSITIVITY_1)
            {
                _gestureUdCount = 1;
            }
            else if (_gestureUdDelta <= -GESTURE_SENSITIVITY_1)
            {
                _gestureUdCount = -1;
            }
            else
            {
                _gestureUdCount = 0;
            }

            /* Determine L/R gesture */
            if (_gestureLrDelta >= GESTURE_SENSITIVITY_1)
            {
                _gestureLrCount = 1;
            }
            else if (_gestureLrDelta <= -GESTURE_SENSITIVITY_1)
            {
                _gestureLrCount = -1;
            }
            else
            {
                _gestureLrCount = 0;
            }

            /* Determine Near/Far gesture */
            if ((_gestureUdCount == 0) && (_gestureLrCount == 0))
            {
                if ((Math.Abs(udDelta) < GESTURE_SENSITIVITY_2) && (Math.Abs(lrDelta) < GESTURE_SENSITIVITY_2))
                {
                    if ((udDelta == 0) && (lrDelta == 0))
                    {
                        _gestureNearCount++;
                    }
                    else if ((udDelta != 0) || (lrDelta != 0))
                    {
                        _gestureFarCount++;
                    }

                    if ((_gestureNearCount >= 10) && (_gestureFarCount >= 2))
                    {
                        if ((udDelta == 0) && (lrDelta == 0))
                        {
                            _gestureState = StateDefinitions.NEAR_STATE;
                        }
                        else if ((udDelta != 0) && (lrDelta != 0))
                        {
                            _gestureState = StateDefinitions.FAR_STATE;
                        }
                        return true;
                    }
                }
            }
            else
            {
                if ((Math.Abs(udDelta) < GESTURE_SENSITIVITY_2) && (Math.Abs(lrDelta) < GESTURE_SENSITIVITY_2))
                {
                    if ((udDelta == 0) && (lrDelta == 0))
                    {
                        _gestureNearCount++;
                    }

                    if (_gestureNearCount >= 10)
                    {
                        _gestureUdCount = 0;
                        _gestureLrCount = 0;
                        _gestureUdDelta = 0;
                        _gestureLrDelta = 0;
                    }
                }
            }
            return false;
        }

        private bool DecodeGesture()
        {
            /* Return if near or far event is detected */
            if (_gestureState == StateDefinitions.NEAR_STATE)
            {
                _gestureMotion = DirectionDefinitions.DIR_NEAR;
                return true;
            }

            if (_gestureState == StateDefinitions.FAR_STATE)
            {
                _gestureMotion = DirectionDefinitions.DIR_FAR;
                return true;
            }

            /* Determine swipe direction */
            if ((_gestureUdCount == -1) && (_gestureLrCount == 0))
            {
                _gestureMotion = DirectionDefinitions.DIR_UP;
            }
            else if ((_gestureUdCount == 1) && (_gestureLrCount == 0))
            {
                _gestureMotion = DirectionDefinitions.DIR_DOWN;
            }
            else if ((_gestureUdCount == 0) && (_gestureLrCount == 1))
            {
                _gestureMotion = DirectionDefinitions.DIR_RIGHT;
            }
            else if ((_gestureUdCount == 0) && (_gestureLrCount == -1))
            {
                _gestureMotion = DirectionDefinitions.DIR_LEFT;
            }
            else if ((_gestureUdCount == -1) && (_gestureLrCount == 1))
            {
                if (Math.Abs(_gestureUdDelta) > Math.Abs(_gestureLrDelta))
                {
                    _gestureMotion = DirectionDefinitions.DIR_UP;
                }
                else
                {
                    _gestureMotion = DirectionDefinitions.DIR_RIGHT;
                }
            }
            else if ((_gestureUdCount == 1) && (_gestureLrCount == -1))
            {
                if (Math.Abs(_gestureUdDelta) > Math.Abs(_gestureLrDelta))
                {
                    _gestureMotion = DirectionDefinitions.DIR_DOWN;
                }
                else
                {
                    _gestureMotion = DirectionDefinitions.DIR_LEFT;
                }
            }
            else if ((_gestureUdCount == -1) && (_gestureLrCount == -1))
            {
                if (Math.Abs(_gestureUdDelta) > Math.Abs(_gestureLrDelta))
                {
                    _gestureMotion = DirectionDefinitions.DIR_UP;
                }
                else
                {
                    _gestureMotion = DirectionDefinitions.DIR_LEFT;
                }
            }
            else if ((_gestureUdCount == 1) && (_gestureLrCount == 1))
            {
                if (Math.Abs(_gestureUdDelta) > Math.Abs(_gestureLrDelta))
                {
                    _gestureMotion = DirectionDefinitions.DIR_DOWN;
                }
                else
                {
                    _gestureMotion = DirectionDefinitions.DIR_RIGHT;
                }
            }
            else
            {
                return false;
            }

            return true;
        }

        private void WriteDataByte(byte reg, byte val)
        {
            var command = new[] { reg, val };
            I2C.Write(command);
        }

        private byte ReadDataByte(byte reg)
        {
            var command = new[] { reg };
            var data = new byte[1];

            I2C.WriteRead(command, data);

            return data[0];
        }

        byte[] ReadDataBlock(byte reg, int len)
        {
            var command = new[] { reg };
            var data = new byte[len];

            I2C.WriteRead(command, data);

            return data;
        }

        #endregion
    }
}
