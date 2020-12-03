using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.Reflection;
using System.Linq;
using System.Security;
using System.Threading.Tasks;
//using System.Runtime.InteropServices;
namespace OosterhofDesign.CncApi_Netstandard
{

    public static class G_GetVersionsIons
    {
        public static readonly string CncApiHeaderVersion = "CNC V4.04.08";
        public static readonly string WrapperVersion = "3.00";

        [DllImport(G_GetCncServer.CncApiDll)]
        private unsafe static extern void CncGetAPIVersion(sbyte* Version);
        public static void GetApiVersion(out string VERSION)//
        {
            unsafe
            {
                sbyte* temp_version = stackalloc sbyte[(int)CncConstants.CNC_MAX_NAME_LENGTH];
                CncGetAPIVersion(temp_version);
                VERSION = StringConversie.CharArrayToString((IntPtr)temp_version, 0, (int)CncConstants.CNC_MAX_NAME_LENGTH);
            }
        }
        


        [DllImport(G_GetCncServer.CncApiDll)]
        private unsafe static extern CncRc CncGetServerVersion(sbyte * Version);
        public static CncRc GetServerVersion(out string version)//
        {
            unsafe
            {
                sbyte* temp_version = stackalloc sbyte[(int)CncConstants.CNC_MAX_NAME_LENGTH];
                CncRc rc = CncGetServerVersion(temp_version);
                version = StringConversie.CharArrayToString((IntPtr)temp_version, 0, (int)CncConstants.CNC_MAX_NAME_LENGTH);
                G_GetCncServer.LastKnowRcState = rc;
                return rc;
            }
        }

        public static void GetHeaderVersion(out string version)
        {
            version = CncApiHeaderVersion;
        }
        public static CncRc CheckVersionMatch()
        {

            string serverVersion = "";
            string apiVersion = "";
            string headerVersion = "";
            GetHeaderVersion(out headerVersion);
            GetApiVersion(out apiVersion);
            CncRc rc = CncRc.CNC_RC_OK;
            rc = GetServerVersion(out serverVersion);

            if (rc == CncRc.CNC_RC_OK)
            {
                if (serverVersion == apiVersion && serverVersion == headerVersion)
                {
                    rc = CncRc.CNC_RC_ERR_VERSION_MISMATCH;
                }
            }
            G_GetCncServer.LastKnowRcState = rc;
            return rc;
        }
    }
    public static class G_GetCncServer
    {
        public const string CncApiDll = "Cncapi";


        public static CncRc LastKnowRcState { get; internal set; } = CncRc.CNC_RC_ERR_NOT_CONNECTED;

        [DllImport(G_GetCncServer.CncApiDll)]
        private unsafe static extern CncRc CncConnectServer(sbyte * iniFileName);

        public static CncRc ConnectServer(string INI_FILENAME)//
        {
            unsafe
            {
                sbyte* temp_iniFile = stackalloc sbyte[(int)CncConstants.CNC_MAX_PATH] ;
                StringConversie.StringToMaxCharArray(INI_FILENAME, (IntPtr)temp_iniFile, 0, (int)CncConstants.CNC_MAX_PATH);
                CncRc return_result = CncConnectServer(temp_iniFile);
                LastKnowRcState = return_result;
                return return_result;
            }
        }

        [DllImport(G_GetCncServer.CncApiDll)]
        private static extern CncRc CncDisConnectServer();

        public static CncRc DisConnectServer()
        {
            CncRc returnResult = CncDisConnectServer();
            LastKnowRcState = returnResult;
            return returnResult;
        }
    }
    public static class G_GetConfigItems
    {
        private static CncSystemConfig _CncSystemConfig = null;
        private static CncInterpreterConfig _CncInterpreterConfig = null;
        private static CncSafetyConfig _CncSafetyConfig = null;
        private static CncTrafficLightCfg _CncTrafficLightCfg = null;
        private static CncProbingCfg _CncProbingCfg = null;
        private static CncIoConfig _CncIoConfig = null;
        private static CncI2cgpioCardConfig _CncI2cgpioCardConfig = null;
        private static CncJointCfg[] _CncJointCfg = new CncJointCfg[(int)CncConstants.CNC_MAX_JOINTS];
        private static CncSpindleConfig[] _CncSpindleConfig = new CncSpindleConfig[(int)CncConstants.CNC_MAX_SPINDLES];
        private static CncHandwheelCfg _CncHandwheelCfg = null;
        private static CncFeedspeedCfg _CncFeedspeedCfg = null;
        private static CncTrajectoryCfg _CncTrajectoryCfg = null;
        private static CncKinCfg _CncKinCfg = null;
        private static CncVacuumbedConfig _CncVacuumbedConfig = null;

        private static CncCameraConfig _CncCameraConfig = null;
        private static CncThcCfg _CncThcCfg = null;
        private static CncServiceCfg _CncServiceCfg = null;
        private static Cnc3dprintingConfig _Cnc3dprintingConfig = null;

        private static CncIoPortSts[] _CncIoPortSts = null;
        private static CncGpioPortSts[,] _CncGpioPortSts = null;
        private static FieldInfo[] _CncGpioId_CncGpioPortSts = null;
        private static FieldInfo[] _CncIoId_CncIoPortSts = null;
        

        static G_GetConfigItems()
        {
            Type cncIoPortSts = typeof(CncIoPortSts);
            _CncIoId_CncIoPortSts = cncIoPortSts.GetFields();
            _CncIoPortSts = new CncIoPortSts[_CncIoId_CncIoPortSts.Length];

            Type cncGpioId = typeof(CncGpioId);
            _CncGpioId_CncGpioPortSts = cncGpioId.GetFields();
            _CncGpioPortSts = new CncGpioPortSts[(int)CncConstants.CNC_MAX_GPIOCARD_CARDS, _CncGpioId_CncGpioPortSts.Length];

        }
        [DllImport(G_GetCncServer.CncApiDll)]
        private unsafe static extern sbyte * CncGetSetupPassword();
        public unsafe static string GetSetupPassword()//
        {
            return StringConversie.CharArrayToString((IntPtr)CncGetSetupPassword(), 0, (int)Offst_CncSystemConfig.setupPasswordRankL_1);
        }
        [DllImport(G_GetCncServer.CncApiDll)]
        private unsafe static extern CncRc CncSetSetupPassword(sbyte * newPassword);
        public static CncRc SetSetupPassword(string newPassword)//
        {
            unsafe
            {
                sbyte* temp_newPassword = stackalloc sbyte[(int)Offst_CncSystemConfig.setupPasswordRankL_1];
                CncRc return_result = CncSetSetupPassword(temp_newPassword);
                G_GetCncServer.LastKnowRcState = return_result;
                return return_result;
            }
        }
        [DllImport(G_GetCncServer.CncApiDll)]
        private unsafe static extern CNC_SYSTEM_CONFIG * CncGetSystemConfig();
        public unsafe static CncSystemConfig GetSystemConfig()
        {
            if (_CncSystemConfig == null || _CncSystemConfig.IsDisposed == true)
            {
                _CncSystemConfig = new CncSystemConfig((IntPtr)CncGetSystemConfig());
            }
            else
            {
                _CncSystemConfig.Pointer = (IntPtr)CncGetSystemConfig();
            }
            return _CncSystemConfig;
        }
        [DllImport(G_GetCncServer.CncApiDll)]
        private unsafe static extern CNC_INTERPRETER_CONFIG * CncGetInterpreterConfig();
        public unsafe static CncInterpreterConfig GetInterpreterConfig()
        {
            if (_CncInterpreterConfig == null || _CncInterpreterConfig.IsDisposed == true)
            {
                _CncInterpreterConfig = new CncInterpreterConfig((IntPtr)CncGetInterpreterConfig());
            }
            else
            {
                _CncInterpreterConfig.Pointer = (IntPtr)CncGetInterpreterConfig();
            }
            return _CncInterpreterConfig;
        }
        [DllImport(G_GetCncServer.CncApiDll)]
        private unsafe static extern CNC_SAFETY_CONFIG * CncGetSafetyConfig();
        public unsafe static CncSafetyConfig GetSafetyConfig()
        {
            if (_CncSafetyConfig == null || _CncSafetyConfig.IsDisposed == true)
            {
                _CncSafetyConfig = new CncSafetyConfig((IntPtr)CncGetSafetyConfig());
            }
            else
            {
                _CncSafetyConfig.Pointer = (IntPtr)CncGetSafetyConfig();
            }
            return _CncSafetyConfig;
        }
        [DllImport(G_GetCncServer.CncApiDll)]
        private unsafe static extern CNC_TRAFFIC_LIGHT_CFG * CncGetTrafficLightConfig();
        public unsafe static CncTrafficLightCfg GetTrafficLightConfig()
        {
            if (_CncTrafficLightCfg == null || _CncTrafficLightCfg.IsDisposed == true)
            {
                _CncTrafficLightCfg = new CncTrafficLightCfg((IntPtr)CncGetTrafficLightConfig());
            }
            else
            {
                _CncTrafficLightCfg.Pointer = (IntPtr)CncGetTrafficLightConfig();
            }
            return _CncTrafficLightCfg;
        }
        [DllImport(G_GetCncServer.CncApiDll)]
        private unsafe static extern CNC_PROBING_CFG * CncGetProbingConfig();
        public unsafe static CncProbingCfg GetProbingConfig()
        {
            if (_CncProbingCfg == null || _CncProbingCfg.IsDisposed == true)
            {
                _CncProbingCfg = new CncProbingCfg((IntPtr)CncGetProbingConfig());
            }
            else
            {
                _CncProbingCfg.Pointer = (IntPtr)CncGetProbingConfig();
            }

            return _CncProbingCfg;
        }
        [DllImport(G_GetCncServer.CncApiDll)]
        private unsafe static extern CNC_IO_CONFIG * CncGetIOConfig();
        public unsafe static CncIoConfig GetIOConfig()
        {
            if (_CncIoConfig == null || _CncIoConfig.IsDisposed == true)
            {
                _CncIoConfig = new CncIoConfig((IntPtr)CncGetIOConfig());
            }
            else
            {
                _CncIoConfig.Pointer = (IntPtr)CncGetIOConfig();
            }

            return _CncIoConfig;
        }
        [DllImport(G_GetCncServer.CncApiDll)]
        private unsafe static extern CNC_I2CGPIO_CARD_CONFIG * CncGetGPIOConfig();
        public unsafe static CncI2cgpioCardConfig GetGPIOConfig()
        {
            if (_CncI2cgpioCardConfig == null || _CncI2cgpioCardConfig.IsDisposed == true)
            {
                _CncI2cgpioCardConfig = new CncI2cgpioCardConfig((IntPtr)CncGetGPIOConfig());
            }
            else
            {
                _CncI2cgpioCardConfig.Pointer = (IntPtr)CncGetGPIOConfig();
            }

            return _CncI2cgpioCardConfig;
        }
        [DllImport(G_GetCncServer.CncApiDll)]
        private unsafe static extern CNC_JOINT_CFG * CncGetJointConfig(int joint);
        public unsafe static CncJointCfg GetJointConfig(int joint)
        {
            if (_CncJointCfg[joint] == null || _CncJointCfg[joint].IsDisposed == true)
            {
                _CncJointCfg[joint] = new CncJointCfg((IntPtr)CncGetJointConfig(joint));
            }
            else
            {
                _CncJointCfg[joint].Pointer = (IntPtr)CncGetJointConfig(joint);
            }

            return _CncJointCfg[joint];
        }
        [DllImport(G_GetCncServer.CncApiDll)]
        private unsafe static extern CNC_SPINDLE_CONFIG * CncGetSpindleConfig(int spindle);
        public unsafe static CncSpindleConfig GetSpindleConfig(int spindle)
        {
            if (_CncSpindleConfig[spindle] == null || _CncSpindleConfig[spindle].IsDisposed == true)
            {
                _CncSpindleConfig[spindle] = new CncSpindleConfig((IntPtr)CncGetSpindleConfig(spindle));
            }
            else
            {
                _CncSpindleConfig[spindle].Pointer = (IntPtr)CncGetSpindleConfig(spindle);
            }

            return _CncSpindleConfig[spindle];
        }
        [DllImport(G_GetCncServer.CncApiDll)]
        private unsafe static extern CNC_FEEDSPEED_CFG * CncGetFeedSpeedConfig();
        public unsafe static CncFeedspeedCfg GetFeedSpeedConfig()
        {
            if(_CncFeedspeedCfg == null || _CncFeedspeedCfg.IsDisposed == true)
            {
                _CncFeedspeedCfg = new CncFeedspeedCfg((IntPtr)CncGetFeedSpeedConfig());
            }
            else
            {
                _CncFeedspeedCfg.Pointer = (IntPtr)CncGetFeedSpeedConfig();
            }
            return _CncFeedspeedCfg;
        }

        [DllImport(G_GetCncServer.CncApiDll)]
        private unsafe static extern CNC_HANDWHEEL_CFG * CncGetHandwheelConfig();
        public unsafe static CncHandwheelCfg GetHandwheelConfig()
        {
            if (_CncHandwheelCfg == null || _CncHandwheelCfg.IsDisposed == true)
            {
                _CncHandwheelCfg = new CncHandwheelCfg((IntPtr)CncGetHandwheelConfig());
            }
            else
            {
                _CncHandwheelCfg.Pointer = (IntPtr)CncGetHandwheelConfig();
            }
            return _CncHandwheelCfg;
        }
        [DllImport(G_GetCncServer.CncApiDll)]
        private unsafe static extern CNC_TRAJECTORY_CFG * CncGetTrajectoryConfig();
        public unsafe static CncTrajectoryCfg GetTrajectoryConfig()
        {
            if (_CncTrajectoryCfg == null || _CncTrajectoryCfg.IsDisposed == true)
            {
                _CncTrajectoryCfg = new CncTrajectoryCfg((IntPtr)CncGetTrajectoryConfig());
            }
            else
            {
                _CncTrajectoryCfg.Pointer = (IntPtr)CncGetTrajectoryConfig();
            }

            return _CncTrajectoryCfg;
        }
        [DllImport(G_GetCncServer.CncApiDll)]
        private unsafe static extern CNC_KIN_CFG * CncGetKinConfig();
        public unsafe static CncKinCfg GetKinConfig()
        {
            if (_CncKinCfg == null || _CncKinCfg.IsDisposed == true)
            {
                _CncKinCfg = new CncKinCfg((IntPtr)CncGetKinConfig());
            }
            else
            {
                _CncKinCfg.Pointer = (IntPtr)CncGetKinConfig();
            }

            return _CncKinCfg;
        }
        [DllImport(G_GetCncServer.CncApiDll)]//,EntryPoint = "CncGetVacuumConfig"
        private unsafe static extern CNC_VACUUMBED_CONFIG* CncGetVacuumConfig();
        public unsafe static CncVacuumbedConfig GetVacuumConfig()
        {
            if(_CncVacuumbedConfig == null || _CncVacuumbedConfig.IsDisposed == true)
            {
                _CncVacuumbedConfig = new CncVacuumbedConfig((IntPtr)CncGetVacuumConfig());
            }
            else
            {
                _CncVacuumbedConfig.Pointer = (IntPtr)CncGetVacuumConfig();
            }

            return _CncVacuumbedConfig;
        }



        


        [DllImport(G_GetCncServer.CncApiDll)]
        private unsafe static extern CNC_CAMERA_CONFIG * CncGetCameraConfig();
        public unsafe static CncCameraConfig GetCameraConfig()
        {
            if (_CncCameraConfig == null || _CncCameraConfig.IsDisposed == true)
            {
                _CncCameraConfig = new CncCameraConfig((IntPtr)CncGetCameraConfig());
            }
            else
            {
                _CncCameraConfig.Pointer = (IntPtr)CncGetCameraConfig();
            }

            return _CncCameraConfig;
        }
        [DllImport(G_GetCncServer.CncApiDll)]
        private unsafe static extern CNC_THC_CFG * CncGetTHCConfig();
        public unsafe static CncThcCfg GetTHCConfig()
        {
            if (_CncThcCfg == null || _CncThcCfg.IsDisposed == true)
            {
                _CncThcCfg = new CncThcCfg((IntPtr)CncGetTHCConfig());
            }
            else
            {
                _CncThcCfg.Pointer = (IntPtr)CncGetTHCConfig();
            }

            return _CncThcCfg;
        }
        [DllImport(G_GetCncServer.CncApiDll)]
        private unsafe static extern CNC_SERVICE_CFG * CncGetServiceConfig();
        public unsafe static CncServiceCfg GetServiceConfig()
        {
            if (_CncServiceCfg == null || _CncServiceCfg.IsDisposed == true)
            {
                _CncServiceCfg = new CncServiceCfg((IntPtr)CncGetServiceConfig());
            }
            else
            {
                _CncServiceCfg.Pointer = (IntPtr)CncGetServiceConfig();
            }

            return _CncServiceCfg;
        }
        [DllImport(G_GetCncServer.CncApiDll)]
        private unsafe static extern CNC_3DPRINTING_CONFIG * CncGet3DPrintConfig();
        public unsafe static Cnc3dprintingConfig Get3DPrintConfig()
        {
            if (_Cnc3dprintingConfig == null || _Cnc3dprintingConfig.IsDisposed == true)
            {
                _Cnc3dprintingConfig = new Cnc3dprintingConfig((IntPtr)CncGet3DPrintConfig());
            }
            else
            {
                _Cnc3dprintingConfig.Pointer = (IntPtr)CncGet3DPrintConfig();
            }

            return _Cnc3dprintingConfig;
        }

        [DllImport(G_GetCncServer.CncApiDll)]
        private unsafe static extern CNC_IO_PORT_STS * CncGetIOStatus(CncIoId ioID);
        public unsafe static CncIoPortSts GetIOStatus(CncIoId ioID)
        {
            int index = -1;
            for (int i = 0; i < _CncIoId_CncIoPortSts.Length; i++)
            {
                CncIoId value = (CncIoId)_CncIoId_CncIoPortSts[i].GetValue(null);
                if (value == ioID)
                {
                    index = i;
                    break;
                }
            }

            if (_CncIoPortSts[index] == null || _CncIoPortSts[index].IsDisposed == true)
            {
                _CncIoPortSts[index] = new CncIoPortSts((IntPtr)CncGetIOStatus(ioID));
            }
            else
            {
                _CncIoPortSts[index].Pointer = (IntPtr)CncGetIOStatus(ioID);
            }
            return _CncIoPortSts[index];
        }
        [DllImport(G_GetCncServer.CncApiDll)]
        private unsafe static extern CNC_GPIO_PORT_STS* CncGetGPIOStatus(int cardNr, CncGpioId ioID);
        public unsafe static CncGpioPortSts GetGPIOStatus(int cardNr, CncGpioId ioID)
        {
            int indexCard = cardNr;
            int indexioID = -1;

            for(int i =0;i< _CncGpioId_CncGpioPortSts.Length;i++)
            {
                CncGpioId value = (CncGpioId)_CncGpioId_CncGpioPortSts[i].GetValue(null);
                if (value == ioID)
                {
                    indexioID = i;
                    break;
                }
            }


            if(_CncGpioPortSts[indexCard, indexioID] == null || _CncGpioPortSts[indexCard, indexioID].IsDisposed == true)
            {
                _CncGpioPortSts[indexCard, indexioID] = new CncGpioPortSts((IntPtr)CncGetGPIOStatus(cardNr, ioID));
            }
            else
            {
                _CncGpioPortSts[indexCard, indexioID].Pointer = (IntPtr)CncGetGPIOStatus(cardNr, ioID);
            }
            return _CncGpioPortSts[indexCard, indexioID];
        }
        [DllImport(G_GetCncServer.CncApiDll)]
        private  static extern CncRc CncStoreIniFile(int saveFixtures);
        public static CncRc StoreIniFile(int saveFixtures)
        {
            CncRc return_result = CncStoreIniFile(saveFixtures);
            G_GetCncServer.LastKnowRcState = return_result;

            return return_result;
        }
        [DllImport(G_GetCncServer.CncApiDll)]
        private static extern CncRc CncReInitialize();
        public static CncRc ReInitialize()
        {
            CncRc return_result = CncReInitialize();
            G_GetCncServer.LastKnowRcState = return_result;

            return return_result;
        }
        [DllImport(G_GetCncServer.CncApiDll)]
        private unsafe static extern CncRc CncGetMacroFileName(sbyte * name);
        public static CncRc GetMacroFileName(out string name)//
        {
            unsafe
            {
                sbyte* temp_name = stackalloc sbyte[(int)Offst_CncInterpreterConfig.macroFileNameRankL_1];
                CncRc return_result = CncGetMacroFileName(temp_name);
                name = StringConversie.CharArrayToString((IntPtr)temp_name, 0, (int)Offst_CncInterpreterConfig.macroFileNameRankL_1);
                G_GetCncServer.LastKnowRcState = return_result;
                return return_result;
            }
        }
        [DllImport(G_GetCncServer.CncApiDll)]
        private unsafe static extern CncRc CncGetUserMacroFileName(sbyte * name);
        public static CncRc GetUserMacroFileName(out string name)//
        {
            unsafe
            {
                sbyte* temp_name = stackalloc sbyte[(int)Offst_CncInterpreterConfig.userMacroFileNameRankL_1];
                CncRc return_result = CncGetUserMacroFileName(temp_name);
                name = StringConversie.CharArrayToString((IntPtr)temp_name, 0, (int)Offst_CncInterpreterConfig.userMacroFileNameRankL_1);
                G_GetCncServer.LastKnowRcState = return_result;
                return return_result;
            }
        }
        [DllImport(G_GetCncServer.CncApiDll)]
        private unsafe static extern CncRc CncSetMacroFileName(sbyte * name);
        public static CncRc SetMacroFileName(string name)//
        {
            unsafe
            {
                sbyte* temp_name = stackalloc sbyte[(int)Offst_CncInterpreterConfig.macroFileNameRankL_1];
                StringConversie.StringToMaxCharArray(name, (IntPtr)temp_name, 0, (int)Offst_CncInterpreterConfig.macroFileNameRankL_1);
                CncRc return_result = CncSetMacroFileName(temp_name);
                G_GetCncServer.LastKnowRcState = return_result;
                return return_result;
            }

        }
        [DllImport(G_GetCncServer.CncApiDll)]
        private unsafe static extern CncRc CncSetUserMacroFileName(sbyte * name);//
        public static CncRc SetUserMacroFileName(string name)
        {
            unsafe
            {
                sbyte* temp_name = stackalloc sbyte[(int)Offst_CncInterpreterConfig.userMacroFileNameRankL_1];
                StringConversie.StringToMaxCharArray(name, (IntPtr)temp_name, 0, (int)Offst_CncInterpreterConfig.userMacroFileNameRankL_1);
                CncRc return_result = CncSetUserMacroFileName(temp_name);
                G_GetCncServer.LastKnowRcState = return_result;
                return return_result;
            }
        }
    }
    public static class G_GetConfigControllerCpu
    {
        private static CncJointSts _CncJointSts = null;

        [DllImport(G_GetCncServer.CncApiDll)]
        private unsafe static extern sbyte * CncGetControllerFirmwareVersion();
        public unsafe static string GetControllerFirmwareVersion()//
        {
            return StringConversie.CharArrayToString((IntPtr)CncGetControllerFirmwareVersion(), 0, (int)CncConstants.CNC_MAX_NAME_LENGTH);
        }
        [DllImport(G_GetCncServer.CncApiDll)]
        private unsafe static extern CncRc CncGetControllerSerialNumber(byte * serial);
        public static CncRc GetControllerSerialNumber(out string serial)
        {
            unsafe
            {
                int serialMaxLength = 6;
                byte* temp_serial = stackalloc byte[serialMaxLength];
                CncRc return_result = CncGetControllerSerialNumber(temp_serial);
                serial = StringConversie.UCharArrayToString((IntPtr)temp_serial, 0, serialMaxLength);

                if(serial == "" || serial == null)
                {
                    for(int i =0;i< serialMaxLength;i++)
                    {
                        serial = serial + 0;
                    }
                }
                return return_result;
            }
        }
        [DllImport(G_GetCncServer.CncApiDll)]
        private static extern int CncGetControllerNumberOfFrequencyItems();
        public static int GetControllerNumberOfFrequencyItems()
        {
            return CncGetControllerNumberOfFrequencyItems();
        }

        [DllImport(G_GetCncServer.CncApiDll)]
        private static extern double CncGetControllerFrequencyItem(uint index);
        public static double GetControllerFrequencyItem(uint index)
        {
            return CncGetControllerFrequencyItem(index);
        }
        [DllImport(G_GetCncServer.CncApiDll)]
        private static extern int CncGetControllerConnectionNumberOfItems();
        public static int GetControllerConnectionNumberOfItems()
        {
            return CncGetControllerConnectionNumberOfItems();
        }
        [DllImport(G_GetCncServer.CncApiDll)]
        private unsafe static extern sbyte * CncGetControllerConnectionItem(int itemNumber);
        public unsafe static string GetControllerConnectionItem(int itemNumber)//
        {
            return StringConversie.CharArrayToString((IntPtr)CncGetControllerConnectionItem(itemNumber), 0, (int)CncConstants.CNC_COMMPORT_NAME_LEN);
        }
        [DllImport(G_GetCncServer.CncApiDll)]
        private unsafe static extern void CncGetNrOfAxesOnController(int* maxNrOfAxes, int* availableNrOfAxes);
        public static  void GetNrOfAxesOnController(out int maxNrOfAxes, out int availableNrOfAxes)
        {
            unsafe
            {
                int* temp_maxNrOfAxes = stackalloc int[1];
                int* temp_availableNrOfAxes = stackalloc int[1];
                CncGetNrOfAxesOnController(temp_maxNrOfAxes, temp_availableNrOfAxes);
                maxNrOfAxes = temp_maxNrOfAxes[0];
                availableNrOfAxes = temp_availableNrOfAxes[0];
            }


        }
        [DllImport(G_GetCncServer.CncApiDll)]
        private static extern int CncGetAxisIsConfigured(int axis, bool includingSlaves);
        public static int GetAxisIsConfigured(int axis, bool includingSlaves)
        {
            return CncGetAxisIsConfigured(axis, includingSlaves);
        }
        [DllImport(G_GetCncServer.CncApiDll)]
        private static extern int CncGetFirmwareHasOptions();
        public static int GetFirmwareHasOptions()
        {
            return CncGetFirmwareHasOptions();
        }
        [DllImport(G_GetCncServer.CncApiDll)]//
        private unsafe static extern CncRc CncGetActiveOptions(sbyte * actCustomerName,
        int* actNumberOfAxes,
        uint* actCPUEnabled,
        uint* actGPIOAVXEnabled,
        uint* actGPIOEDIEnabled,
        uint* actWolfcutCameraEnabled,
        uint* actTURNMACRO,
        uint* actXHCPendant,
        uint* actLimitedSoftwareEnabled);
        public static CncRc GetActiveOptions(out string actCustomerName,
       out int actNumberOfAxes,
       out uint actCPUEnabled,
       out uint actGPIOAVXEnabled,
       out uint actGPIOEDIEnabled,
       out uint actWolfcutCameraEnabled,
       out uint actTURNMACRO,
       out uint actXHCPendant,
       out uint actLimitedSoftwareEnabled)
        {
            unsafe
            {
                sbyte* temp_actCustomerName = stackalloc sbyte[(int)CncConstants.CNC_MAX_CUSTOMER_NAME ];
                int temp_actNumberOfAxes = 0;
                uint temp_actCPUEnabled = 0;
                uint temp_actGPIOAVXEnabled = 0;
                uint temp_actGPIOEDIEnabled = 0;
                uint temp_actWolfcutCameraEnabled = 0;
                uint temp_actTURNMACRO = 0;
                uint temp_actXHCPendant = 0;
                uint temp_actLimitedSoftwareEnabled = 0;


                CncRc return_result = CncGetActiveOptions(temp_actCustomerName,
                    &temp_actNumberOfAxes,
                    &temp_actCPUEnabled,
                    &temp_actGPIOAVXEnabled,
                    &temp_actGPIOEDIEnabled,
                    &temp_actWolfcutCameraEnabled,
                    &temp_actTURNMACRO,
                    &temp_actXHCPendant,
                    &temp_actLimitedSoftwareEnabled);
                actCustomerName = StringConversie.CharArrayToString((IntPtr)temp_actCustomerName, 0, (int)CncConstants.CNC_MAX_CUSTOMER_NAME);
                actNumberOfAxes = temp_actNumberOfAxes;
                actCPUEnabled = temp_actCPUEnabled;
                actGPIOAVXEnabled = temp_actGPIOAVXEnabled;
                actGPIOEDIEnabled = temp_actGPIOEDIEnabled;
                actWolfcutCameraEnabled = temp_actWolfcutCameraEnabled;
                actTURNMACRO = temp_actTURNMACRO;
                actXHCPendant = temp_actXHCPendant;
                actLimitedSoftwareEnabled = temp_actLimitedSoftwareEnabled;


                G_GetCncServer.LastKnowRcState = return_result;
                return return_result;
            }

        }

        [DllImport(G_GetCncServer.CncApiDll)]//
        private unsafe static extern CncRc CncGetOptionRequestCode(sbyte * newCustomerName,
        int newNumberOfAxes,
        uint newGPIOAVXEnabled,
        uint newGPIOEDIEnabled,
        uint newPLASMAEnabled,
        uint newTURMACROEnabled,
        uint newXHCPendant,
        uint newLimitedSoftwareEnabled,
        sbyte * requestCode);
        public static CncRc GetOptionRequestCode(string newCustomerName,
        int newNumberOfAxes,
        uint newGPIOAVXEnabled,
        uint newGPIOEDIEnabled,
        uint newPLASMAEnabled,
        uint newTURMACROEnabled,
        uint newXHCPendant,
        uint newLimitedSoftwareEnabled,
        out string requestCode)
        {
            unsafe
            {
                sbyte* temp_newCustomerName = stackalloc sbyte[(int)CncConstants.CNC_MAX_CUSTOMER_NAME];
                sbyte* temp_requestCode = stackalloc sbyte[256 * (int)Offst_Char.TotalSize];
                StringConversie.StringToMaxCharArray(newCustomerName, (IntPtr)temp_newCustomerName, 0, (int)CncConstants.CNC_MAX_CUSTOMER_NAME);
                CncRc return_result = CncGetOptionRequestCode(temp_newCustomerName,
                newNumberOfAxes,
                newGPIOAVXEnabled,
                newGPIOEDIEnabled,
                newPLASMAEnabled,
                newTURMACROEnabled,
                newXHCPendant,
                newLimitedSoftwareEnabled,
                temp_requestCode);
                requestCode = StringConversie.CharArrayToString((IntPtr)temp_requestCode, 0, 256);
                G_GetCncServer.LastKnowRcState = return_result;
                return return_result;
            }
        }

        [DllImport(G_GetCncServer.CncApiDll)]
        private unsafe static extern CncRc CncGetOptionRequestCodeCurrent(sbyte * requestCode);
        public static CncRc GetOptionRequestCodeCurrent(out string requestCode)
        {
            unsafe
            {
                sbyte* temp_requestCode = stackalloc sbyte[256];
                CncRc return_result = CncGetOptionRequestCodeCurrent(temp_requestCode);
                requestCode=StringConversie.CharArrayToString((IntPtr)temp_requestCode,0,256);
                G_GetCncServer.LastKnowRcState = return_result;
                return return_result;
            }
        }
        [DllImport(G_GetCncServer.CncApiDll)]
        private unsafe static extern CncRc CncActivateOption(sbyte * activationKey);
        public static CncRc ActivateOption(string activationKey)
        {
            unsafe
            {
                sbyte* temp_activationKey = stackalloc sbyte[activationKey.Length];
                CncRc return_result = CncActivateOption(temp_activationKey);
                G_GetCncServer.LastKnowRcState = return_result;
                return return_result;
            }
        }
        [DllImport(G_GetCncServer.CncApiDll)]
        private unsafe static extern CNC_JOINT_STS* CncGetJointStatus(int joint);
        public unsafe static CncJointSts GetJointStatus(int joint)
        {
            if(_CncJointSts == null || _CncJointSts.IsDisposed == true)
            {
                _CncJointSts = new CncJointSts((IntPtr)CncGetJointStatus(joint));
            }
            else
            {
                _CncJointSts.Pointer = (IntPtr)CncGetJointStatus(joint);
            }


            return _CncJointSts;
        }

    }
    public static class G_GetSetToolTableData
    {
        private static CncToolData[] _CncToolData = new CncToolData[(int)CncConstants.CNC_MAX_TOOLS];

        [DllImport(G_GetCncServer.CncApiDll)]
        private static extern CNC_TOOL_DATA CncGetToolData(int index);
        public static CncToolData GetToolData(int index)
        {
            if(_CncToolData[index] == null || _CncToolData[index].IsDisposed == true)
            {
                _CncToolData[index] = new CncToolData();

            }
            CNC_TOOL_DATA temp_tooldata = CncGetToolData(index);
            _CncToolData[index].SetStructValue(ref temp_tooldata);
            return _CncToolData[index];
        }
        [DllImport(G_GetCncServer.CncApiDll)]
        private unsafe static extern CncRc CncUpdateToolData(CNC_TOOL_DATA * pTool, int index);
        public unsafe static CncRc UpdateToolData(CncToolData pTool, int index)
        {
            CncRc return_result = CncUpdateToolData((CNC_TOOL_DATA*)pTool.Pointer, index);

            G_GetCncServer.LastKnowRcState = return_result;
            return return_result;
        }
        [DllImport(G_GetCncServer.CncApiDll)]
        private static extern CncRc CncLoadToolTable();
        public static CncRc LoadToolTable()
        {
            CncRc return_result = CncLoadToolTable();
            G_GetCncServer.LastKnowRcState = return_result;
            return return_result;
        }
    }
    public static class G_VariableAccess
    {
        [DllImport(G_GetCncServer.CncApiDll)]
        private static extern double CncGetVariable(int varIndex);
        public static double GetVariable(int varIndex)
        {
            return CncGetVariable(varIndex);
        }
        [DllImport(G_GetCncServer.CncApiDll)]
        private static extern void CncSetVariable(int varIndex, double value);
        public static void SetVariable(int varIndex, double value)
        {
            CncSetVariable( varIndex,  value);
        }
    }
    public static class G_StatusItems
    {
        private static CncRunningStatus _CncRunningStatus = null;
        private static CncMotionStatus _CncMotionStatus = null;
        private static CncControllerStatus _CncControllerStatus = null;
        private static CncControllerConfigStatus _CncControllerConfigStatus = null;
        private static CncTrafficLightStatus _CncTrafficLightStatus = null;
        private static CncJobStatus _CncJobStatus = null;
        private static CncTrackingStatus _CncTrackingStatus = null;
        private static CncThcStatus _CncThcStatus = null;
        private static CncNestingStatus _CncNestingStatus = null;
        private static CncKinStatus _CncKinStatus = null;
        private static CncSpindleSts _CncSpindleSts = null;
        private static CncPauseSts _CncPauseSts = null;
        private static CncSearchStatus _CncSearchStatus = null;
        private static Cnc3dprintingSts _Cnc3dprintingSts = null;
        private static CncCompensationStatus _CncCompensationStatus = null;
        private static CncVacuumStatus _CncVacuumStatus = null;
        
        [DllImport(G_GetCncServer.CncApiDll)]
        private unsafe static extern CNC_RUNNING_STATUS * CncGetRunningStatus();
        public unsafe static CncRunningStatus GetRunningStatus()
        {
            if(_CncRunningStatus == null || _CncRunningStatus.IsDisposed == true)
            {
                _CncRunningStatus = new CncRunningStatus((IntPtr)CncGetRunningStatus());
            }
            else
            {
                _CncRunningStatus.Pointer = (IntPtr)CncGetRunningStatus();
            }

            return _CncRunningStatus;
        }
        [DllImport(G_GetCncServer.CncApiDll)]
        private unsafe static extern CNC_MOTION_STATUS * CncGetMotionStatus();
        public unsafe static CncMotionStatus GetMotionStatus()
        {
            if (_CncMotionStatus == null || _CncMotionStatus.IsDisposed == true)
            {
                _CncMotionStatus = new CncMotionStatus((IntPtr)CncGetMotionStatus());
            }
            else
            {
                _CncMotionStatus.Pointer = (IntPtr)CncGetMotionStatus();
            }

            return _CncMotionStatus;
        }
        [DllImport(G_GetCncServer.CncApiDll)]
        private unsafe static extern CNC_CONTROLLER_STATUS* CncGetControllerStatus();
        public unsafe static CncControllerStatus GetControllerStatus()
        {
            if (_CncControllerStatus == null || _CncControllerStatus.IsDisposed == true)
            {
                _CncControllerStatus = new CncControllerStatus((IntPtr)CncGetControllerStatus());
            }
            else
            {
                _CncControllerStatus.Pointer = (IntPtr)CncGetControllerStatus();
            }

            return _CncControllerStatus;
        }
        [DllImport(G_GetCncServer.CncApiDll)]
        private unsafe static extern CNC_CONTROLLER_CONFIG_STATUS* CncGetControllerConfigStatus();
        public unsafe static CncControllerConfigStatus GetControllerConfigStatus()
        {
            if (_CncControllerConfigStatus == null || _CncControllerConfigStatus.IsDisposed == true)
            {
                _CncControllerConfigStatus = new CncControllerConfigStatus((IntPtr)CncGetControllerConfigStatus());
            }
            else
            {
                _CncControllerConfigStatus.Pointer = (IntPtr)CncGetControllerConfigStatus();
            }

            return _CncControllerConfigStatus;
        }
        [DllImport(G_GetCncServer.CncApiDll)]
        private unsafe static extern CNC_TRAFFIC_LIGHT_STATUS * CncGetTrafficLightStatus();
        public unsafe static CncTrafficLightStatus GetTrafficLightStatus()
        {
            if (_CncTrafficLightStatus == null || _CncTrafficLightStatus.IsDisposed == true)
            {
                _CncTrafficLightStatus = new CncTrafficLightStatus((IntPtr)CncGetTrafficLightStatus());
            }
            else
            {
                _CncTrafficLightStatus.Pointer = (IntPtr)CncGetTrafficLightStatus();
            }

            return _CncTrafficLightStatus;
        }
        [DllImport(G_GetCncServer.CncApiDll)]
        private unsafe static extern CNC_JOB_STATUS* CncGetJobStatus();
        public unsafe static CncJobStatus GetJobStatus()
        {
            if (_CncJobStatus == null || _CncJobStatus.IsDisposed == true)
            {
                _CncJobStatus = new CncJobStatus((IntPtr)CncGetJobStatus());
            }
            else
            {
                _CncJobStatus.Pointer = (IntPtr)CncGetJobStatus();
            }

            return _CncJobStatus;
        }
        [DllImport(G_GetCncServer.CncApiDll)]
        private unsafe static extern CNC_TRACKING_STATUS* CncGetTrackingStatus();
        public unsafe static CncTrackingStatus GetTrackingStatus()
        {
            if (_CncTrackingStatus == null || _CncTrackingStatus.IsDisposed == true)
            {
                _CncTrackingStatus = new CncTrackingStatus((IntPtr)CncGetTrackingStatus());
            }
            else
            {
                _CncTrackingStatus.Pointer = (IntPtr)CncGetTrackingStatus();
            }

            return _CncTrackingStatus;
        }
        [DllImport(G_GetCncServer.CncApiDll)]
        private unsafe static extern CNC_THC_STATUS * CncGetTHCStatus();
        public unsafe static CncThcStatus GetTHCStatus()
        {
            if (_CncThcStatus == null || _CncThcStatus.IsDisposed == true)
            {
                _CncThcStatus = new CncThcStatus((IntPtr)CncGetTHCStatus());
            }
            else
            {
                _CncThcStatus.Pointer = (IntPtr)CncGetTHCStatus();
            }

            return _CncThcStatus;
        }
        [DllImport(G_GetCncServer.CncApiDll)]
        private unsafe static extern CNC_NESTING_STATUS* CncGetNestingStatus();
        public unsafe static CncNestingStatus GetNestingStatus()
        {
            if (_CncNestingStatus == null || _CncNestingStatus.IsDisposed == true)
            {
                _CncNestingStatus = new CncNestingStatus((IntPtr)CncGetNestingStatus());
            }
            else
            {
                _CncNestingStatus.Pointer = (IntPtr)CncGetNestingStatus();
            }

            return _CncNestingStatus;
        }
        [DllImport(G_GetCncServer.CncApiDll)]
        private unsafe static extern CNC_KIN_STATUS* CncGetKinStatus();
        public unsafe static CncKinStatus GetKinStatus()
        {
            if (_CncKinStatus == null || _CncKinStatus.IsDisposed == true)
            {
                _CncKinStatus = new CncKinStatus((IntPtr)CncGetKinStatus());
            }
            else
            {
                _CncKinStatus.Pointer = (IntPtr)CncGetKinStatus();
            }

            return _CncKinStatus;
        }
        [DllImport(G_GetCncServer.CncApiDll)]
        private unsafe static extern CNC_SPINDLE_STS* CncGetSpindleStatus();
        public unsafe static CncSpindleSts GetSpindleStatus()
        {
            if (_CncSpindleSts == null || _CncSpindleSts.IsDisposed == true)
            {
                _CncSpindleSts = new CncSpindleSts((IntPtr)CncGetSpindleStatus());
            }
            else
            {
                _CncSpindleSts.Pointer = (IntPtr)CncGetSpindleStatus();
            }

            return _CncSpindleSts;
        }
        [DllImport(G_GetCncServer.CncApiDll)]
        private unsafe static extern CNC_PAUSE_STS* CncGetPauseStatus();
        public unsafe static CncPauseSts GetPauseStatus()
        {
            if (_CncPauseSts == null || _CncPauseSts.IsDisposed == true)
            {
                _CncPauseSts = new CncPauseSts((IntPtr)CncGetPauseStatus());
            }
            else
            {
                _CncPauseSts.Pointer = (IntPtr)CncGetPauseStatus();
            }

            return _CncPauseSts;
        }
        [DllImport(G_GetCncServer.CncApiDll)]
        private unsafe static extern CNC_SEARCH_STATUS* CncGetSearchStatus();
        public unsafe static CncSearchStatus GetSearchStatus()
        {
            if (_CncSearchStatus == null || _CncSearchStatus.IsDisposed == true)
            {
                _CncSearchStatus = new CncSearchStatus((IntPtr)CncGetSearchStatus());
            }
            else
            {
                _CncSearchStatus.Pointer = (IntPtr)CncGetSearchStatus();
            }

            return _CncSearchStatus;
        }
        [DllImport(G_GetCncServer.CncApiDll)]
        private unsafe static extern CNC_3DPRINTING_STS* CncGet3DPrintStatus();
        public unsafe static Cnc3dprintingSts Get3DPrintStatus()
        {
            if (_Cnc3dprintingSts == null || _Cnc3dprintingSts.IsDisposed == true)
            {
                _Cnc3dprintingSts = new Cnc3dprintingSts((IntPtr)CncGet3DPrintStatus());
            }
            else
            {
                _Cnc3dprintingSts.Pointer = (IntPtr)CncGet3DPrintStatus();
            }

            return _Cnc3dprintingSts;
        }
        [DllImport(G_GetCncServer.CncApiDll)]
        private unsafe static extern CNC_COMPENSATION_STATUS* CncGetCompensationStatus();
        public unsafe static CncCompensationStatus GetCompensationStatus()
        {
            if (_CncCompensationStatus == null || _CncCompensationStatus.IsDisposed == true)
            {
                _CncCompensationStatus = new CncCompensationStatus((IntPtr)CncGetCompensationStatus());
            }
            else
            {
                _CncCompensationStatus.Pointer = (IntPtr)CncGetCompensationStatus();
            }

            return _CncCompensationStatus;
        }
        [DllImport(G_GetCncServer.CncApiDll)]
        private unsafe static extern CNC_VACUUM_STATUS* CncGetVacuumStatus();
        public unsafe static CncVacuumStatus GetVacuumStatus()
        {
            if (_CncVacuumStatus == null || _CncVacuumStatus.IsDisposed == true)
            {
                _CncVacuumStatus = new CncVacuumStatus((IntPtr)CncGetVacuumStatus());
            }
            else
            {
                _CncVacuumStatus.Pointer = (IntPtr)CncGetVacuumStatus();
            }

            return _CncVacuumStatus;
        }
        [DllImport(G_GetCncServer.CncApiDll)]
        private static extern int CncGet10msHeartBeat();
        public static int Get10msHeartBeat()
        {
            return CncGet10msHeartBeat();
        }
        [DllImport(G_GetCncServer.CncApiDll)]
        private static extern int CncIsServerConnected();
        public static int IsServerConnected()
        {
            return CncIsServerConnected();
        }

    }
    public static class G_StatusItemsposition
    {
        private static CncCartDouble _CncCartDouble = null;
        private static CncJointDouble _CncJointDouble_GetWorkPosition = null;
        private static CncCartDouble _CncJointDouble_GetMachinePosition = null;
        private static CncCartDouble _CncCartDouble_GetActualOriginOffset = null;

        [DllImport(G_GetCncServer.CncApiDll)]
        private static extern CncIeState CncGetState();
        public static CncIeState GetState()
        {
            return CncGetState();
        }
        [DllImport(G_GetCncServer.CncApiDll)]
        private unsafe static extern sbyte * CncGetStateText(CncIeState state);//unkown length cnciestate
        public unsafe static string GetStateText(CncIeState state)//
        {
            return StringConversie.CharArrayToString((IntPtr)CncGetStateText(state), 0, Convert.ToString(state).Length);
        }
        [DllImport(G_GetCncServer.CncApiDll)]
        private static extern int CncInMillimeterMode();
        public static int InMillimeterMode()
        {
            return CncInMillimeterMode();
        }
        [DllImport(G_GetCncServer.CncApiDll)]
        private static extern CncPlane CncGetActualPlane();
        public static CncPlane GetActualPlane()
        {
            return CncGetActualPlane();

        }
        [DllImport(G_GetCncServer.CncApiDll)]
        private static extern CNC_CART_DOUBLE CncGetWorkPosition();
        public static CncCartDouble GetWorkPosition()
        {
            if (_CncCartDouble == null || _CncCartDouble.IsDisposed == true)
            {
                _CncCartDouble = new CncCartDouble();
            }
            CNC_CART_DOUBLE temp_CncCartDouble = CncGetWorkPosition();
            _CncCartDouble.SetStructValue(ref temp_CncCartDouble);

            return _CncCartDouble;
        }
        [DllImport(G_GetCncServer.CncApiDll)]
        private static extern CNC_JOINT_DOUBLE CncGetMotorPosition();
        public static CncJointDouble GetMotorPosition()
        {
            if (_CncJointDouble_GetWorkPosition == null || _CncJointDouble_GetWorkPosition.IsDisposed == true)
            {
                _CncJointDouble_GetWorkPosition = new CncJointDouble();
            }
            CNC_JOINT_DOUBLE temp_CncJointDouble = CncGetMotorPosition();
            _CncJointDouble_GetWorkPosition.SetStructValue(ref temp_CncJointDouble);
            return _CncJointDouble_GetWorkPosition;
        }
        [DllImport(G_GetCncServer.CncApiDll)]
        private static extern CNC_CART_DOUBLE CncGetMachinePosition();
        public static CncCartDouble GetMachinePosition()
        {
            if (_CncJointDouble_GetMachinePosition == null || _CncJointDouble_GetMachinePosition.IsDisposed == true)
            {
                _CncJointDouble_GetMachinePosition = new CncCartDouble();
            }
            CNC_CART_DOUBLE temp__CncJointDouble = CncGetMachinePosition();
            _CncJointDouble_GetMachinePosition.SetStructValue(ref temp__CncJointDouble);
            return _CncJointDouble_GetMachinePosition;
        }
        [DllImport(G_GetCncServer.CncApiDll)]
        private unsafe static extern void CncGetMachineZeroWorkPoint(CNC_CART_DOUBLE* pos, int* rotationActive);
        public static void GetMachineZeroWorkPoint(CncCartDouble pos, out int rotationActive)
        {
            unsafe
            {
                int temp_rotationActive = 0;
                CncGetMachineZeroWorkPoint((CNC_CART_DOUBLE*)pos.Pointer, &temp_rotationActive);
                rotationActive = temp_rotationActive;
            }
        }
        [DllImport(G_GetCncServer.CncApiDll)]
        private static extern CNC_CART_DOUBLE CncGetActualOriginOffset();
        public static CncCartDouble GetActualOriginOffset()
        {
            if (_CncCartDouble_GetActualOriginOffset == null || _CncCartDouble_GetActualOriginOffset.IsDisposed == true)
            {
                _CncCartDouble_GetActualOriginOffset = new CncCartDouble();
            }
            CNC_CART_DOUBLE temp_CncCartDouble = CncGetActualOriginOffset();
            _CncCartDouble_GetActualOriginOffset.SetStructValue(ref temp_CncCartDouble);

            return _CncCartDouble_GetActualOriginOffset;
        }
        [DllImport(G_GetCncServer.CncApiDll)]
        private static extern double CncGetActualToolZOffset();
        public static double GetActualToolZOffset()
        {
            return CncGetActualToolZOffset();
        }
        [DllImport(G_GetCncServer.CncApiDll)]
        private static extern double CncGetActualToolXOffset();
        public static double GetActualToolXOffset()
        {
            return CncGetActualToolXOffset();
        }
        [DllImport(G_GetCncServer.CncApiDll)]
        private static extern double CncGetActualG68Rotation();
        public static double GetActualG68Rotation()
        {
            return CncGetActualG68Rotation();
        }
        [DllImport(G_GetCncServer.CncApiDll)]
        private static extern CncPlane CncGetActualG68RotationPlane();
        public static CncPlane GetActualG68RotationPlane()
        {
            return CncGetActualG68RotationPlane();
        }
    }
    public static class G_StatusItemsInterpreter
    {
        [DllImport(G_GetCncServer.CncApiDll)]
        private unsafe static extern void CncGetCurrentGcodesText(sbyte* activeGCodes);
        public static void GetCurrentGcodesText(out string activeGCodes)//
        {
            unsafe
            {
                sbyte* temp_activeGCodes = stackalloc sbyte[80];
                CncGetCurrentGcodesText(temp_activeGCodes);
                activeGCodes= StringConversie.CharArrayToString((IntPtr)temp_activeGCodes,0,80);
            }
        }
        [DllImport(G_GetCncServer.CncApiDll)]
        private unsafe static extern void CncGetCurrentMcodesText(sbyte* activeGCodes);
        public static void GetCurrentMcodesText(out string activeMCodes)//
        {
            unsafe
            {
                sbyte* temp_activeMCodes = stackalloc sbyte[80];
                CncGetCurrentMcodesText(temp_activeMCodes);
                activeMCodes = StringConversie.CharArrayToString((IntPtr)temp_activeMCodes, 0, 80);
            }
        }
        [DllImport(G_GetCncServer.CncApiDll)]
        private unsafe static extern void CncGetCurrentGcodeSettingsText(sbyte * activeGCodes);
        public static void GetCurrentGcodeSettingsText(out string actualGCodeSettings)//
        {
            unsafe
            {
                sbyte* temp_actualGCodeSettings = stackalloc sbyte[80];
                CncGetCurrentGcodeSettingsText(temp_actualGCodeSettings);
                actualGCodeSettings = StringConversie.CharArrayToString((IntPtr)temp_actualGCodeSettings, 0, 80);
            }
        }
        [DllImport(G_GetCncServer.CncApiDll)]
        private static extern double CncGetProgrammedSpeed();
        public static double GetProgrammedSpeed()
        {
            return CncGetProgrammedSpeed();
        }
        [DllImport(G_GetCncServer.CncApiDll)]
        private static extern double CncGetProgrammedFeed();
        public static double GetProgrammedFeed()
        {
            return CncGetProgrammedFeed();
        }
        [DllImport(G_GetCncServer.CncApiDll)]
        private static extern int CncGetCurrentToolNumber();
        public static int GetCurrentToolNumber()
        {
            return CncGetCurrentToolNumber();
        }
        [DllImport(G_GetCncServer.CncApiDll)]
        private static extern int CncG43Active();
        public static int G43Active()
        {
            return CncG43Active();
        }
        [DllImport(G_GetCncServer.CncApiDll)]
        private static extern int CncG95Active();
        public static int G95Active()
        {
            return CncG95Active();
        }
        [DllImport(G_GetCncServer.CncApiDll)]
        private static extern int CncGetCurInterpreterLineNumber();
        public static int GetCurInterpreterLineNumber()
        {
            return CncGetCurInterpreterLineNumber();
        }
        [DllImport(G_GetCncServer.CncApiDll)]
        private unsafe static extern int CncGetCurInterpreterLineText(sbyte * text);
        public static int GetCurInterpreterLineNumber(out string text)//
        {
            unsafe
            {
                sbyte* temp_text = stackalloc sbyte[(int)CncConstants.CNC_MAX_INTERPRETER_LINE];
                int return_result = CncGetCurInterpreterLineText(temp_text);
                text = StringConversie.CharArrayToString((IntPtr)temp_text,0, (int)CncConstants.CNC_MAX_INTERPRETER_LINE);
                return return_result;
            }
        }
        [DllImport(G_GetCncServer.CncApiDll)]
        private static extern int CncCurrentInterpreterLineContainsToolChange();
        public static int CurrentInterpreterLineContainsToolChange()
        {
            return CncCurrentInterpreterLineContainsToolChange();
        }
    }
    public static class G_StatusErrorSafety
    {
        [DllImport(G_GetCncServer.CncApiDll)]
        private static extern int CncGetSwLimitError();
        public static int GetSwLimitError()
        {
            return CncGetSwLimitError();
        }
        [DllImport(G_GetCncServer.CncApiDll)]
        private static extern int CncGetFifoError();
        public static int GetFifoError()
        {
            return CncGetFifoError();
        }
        [DllImport(G_GetCncServer.CncApiDll)]
        private static extern int CncGetEMStopActive();
        public static int GetEMStopActive()
        {
            return CncGetEMStopActive();
        }
        [DllImport(G_GetCncServer.CncApiDll)]
        private static extern int CncGetAllAxesHomed();
        public static int GetAllAxesHomed()
        {
            return CncGetAllAxesHomed();
        }
        [DllImport(G_GetCncServer.CncApiDll)]
        private static extern int CncGetSafetyMode();
        public static int GetSafetyMode()
        {
            return CncGetSafetyMode();
        }
    }
    public static class G_Kinematics
    {
        private static CncVector _CncVector_KinGetARotationPoint = null;

        [DllImport(G_GetCncServer.CncApiDll)]
        private static extern CncKinematicsType CncKinGetActiveType();
        public static CncKinematicsType KinGetActiveType()
        {
            return CncKinGetActiveType();
        }
        [DllImport(G_GetCncServer.CncApiDll)]
        private static extern int CncKinActivate(int active);
        public static int KinActivate(int active)
        {
            return CncKinActivate(active);
        }
        [DllImport(G_GetCncServer.CncApiDll)]
        private static extern int CncKinInit();
        public static int KinInit()
        {
            return CncKinInit();
        }
        [DllImport(G_GetCncServer.CncApiDll)]
        private unsafe static extern int CncKinControl(KinControlId controlID, KIN_CONTROLDATA * pControlData);
        public unsafe static int KinControl(KinControlId controlID, KinControldata pControlData)
        {
            return CncKinControl(controlID, (KIN_CONTROLDATA*)pControlData.Pointer);
        }
        [DllImport(G_GetCncServer.CncApiDll)]
        private static extern CNC_VECTOR CncKinGetARotationPoint();
        public static CncVector KinGetARotationPoint()
        {
            if(_CncVector_KinGetARotationPoint == null || _CncVector_KinGetARotationPoint.IsDisposed == true)
            {
                _CncVector_KinGetARotationPoint = new CncVector();
            }
            CNC_VECTOR temp_CncVector = CncKinGetARotationPoint();
            _CncVector_KinGetARotationPoint.SetStructValue(ref temp_CncVector);

            return _CncVector_KinGetARotationPoint;
        }

    }
    public static class G_StatusItemsIO
    {
        [DllImport(G_GetCncServer.CncApiDll)]
        private unsafe static extern sbyte * CncGetIOName(CncIoId id);
        public unsafe static string GetIOName(CncIoId id)//
        {
            return StringConversie.CharArrayToString((IntPtr)CncGetIOName(id),0,(int)CncConstants.CNC_MAX_IO_NAME_LENGTH);
        }
        [DllImport(G_GetCncServer.CncApiDll)]
        private static extern int CncGetOutput(CncIoId id);
        public static int GetOutput(CncIoId id)
        {
            return CncGetOutput( id);
        }
        [DllImport(G_GetCncServer.CncApiDll)]
        private static extern int CncGetOutputRaw(CncIoId id);
        public static int GetOutputRaw(CncIoId id)
        {
            return CncGetOutputRaw(id);
        }
        [DllImport(G_GetCncServer.CncApiDll)]
        private static extern int CncGetGPIOOutput(int gpioCardIndex, CncGpioId ioId);
        public static int GetGPIOOutput(int gpioCardIndex, CncGpioId ioId)
        {
            return CncGetGPIOOutput( gpioCardIndex,  ioId);
        }
        [DllImport(G_GetCncServer.CncApiDll)]
        private static extern int CncGetInput(CncIoId id);
        public static int GetInput(CncIoId id)
        {
            return CncGetInput(id);
        }


        [DllImport(G_GetCncServer.CncApiDll)]
        private static extern int CncGetInputRaw(CncIoId id);
        public static int GetInputRaw(CncIoId id)
        {
            return CncGetInputRaw( id);
        }
        [DllImport(G_GetCncServer.CncApiDll)]
        private static extern int CncGetGPIOInput(int gpioCardIndex, CncGpioId ioId);
        public static int GetGPIOInput(int gpioCardIndex, CncGpioId ioId)
        {
            return CncGetGPIOInput( gpioCardIndex,  ioId);
        }
        [DllImport(G_GetCncServer.CncApiDll)]
        private static extern CncRc CncSetOutput(CncIoId id, int value);
        public static CncRc SetOutput(CncIoId id, int value)
        {
            CncRc return_result = CncSetOutput( id,  value);
            G_GetCncServer.LastKnowRcState = return_result;
            return return_result;
        }
        [DllImport(G_GetCncServer.CncApiDll)]
        private static extern CncRc CncSetOutputRaw(CncIoId id, int value);
        public static CncRc SetOutputRaw(CncIoId id, int value)
        {
            CncRc return_result = CncSetOutputRaw( id,  value);
            G_GetCncServer.LastKnowRcState = return_result;
            return return_result;
        }
        [DllImport(G_GetCncServer.CncApiDll)]
        private static extern CncRc CncSetGPIOOutput(int gpioCardIndex, CncGpioId ioId, int value);
        public static CncRc SetGPIOOutput(int gpioCardIndex, CncGpioId ioId, int value)
        {
            CncRc return_result = CncSetGPIOOutput(gpioCardIndex, ioId, value);
            G_GetCncServer.LastKnowRcState = return_result;
            return return_result;
        }
        [DllImport(G_GetCncServer.CncApiDll)]
        private unsafe static extern CncRc CncCheckStartConditionOK(int generateMessage, int ignoreHoming, int* result);
        public static CncRc CheckStartConditionOK(int generateMessage, int ignoreHoming,out int result)
        {
            unsafe
            {
                int temp_result = 0;
                CncRc return_result = CncCheckStartConditionOK(generateMessage, ignoreHoming, &temp_result);
                G_GetCncServer.LastKnowRcState = return_result;
                result = temp_result;
                return return_result;
            }
        }
        [DllImport(G_GetCncServer.CncApiDll)]
        private static extern CncRc CncSetSpindleOutput(int onOff, int direction, double absSpeed);
        public static CncRc SetSpindleOutput(int onOff, int direction, double absSpeed)
        {
            CncRc return_result = CncSetSpindleOutput( onOff,  direction,  absSpeed);
            G_GetCncServer.LastKnowRcState = return_result;
            return return_result;
        }
    }
    
    public static class G_LogMessagesRealtime
    {
        private static CncLogMessage _CncLogMessage_LogFifoGet = null;
        private static CncPosFifoData _CncPosFifoData_PosFifoGet = null;
        private static CncPosFifoData _CncPosFifoData_PosFifoGet2 = null;
        private static CncGraphFifoData _CncGraphFifoData_GraphFifoGet = null;

        [DllImport(G_GetCncServer.CncApiDll)]
        private unsafe static extern CncRc CncLogFifoGet(CNC_LOG_MESSAGE* data);
        public unsafe static CncRc LogFifoGet(CncLogMessage data)
        {
            CncRc return_result = CncLogFifoGet((CNC_LOG_MESSAGE*)data.Pointer);
            G_GetCncServer.LastKnowRcState = return_result;
            return return_result;
        }
        public static CncRc LogFifoGet(out CncLogMessage data)
        {
            if (_CncLogMessage_LogFifoGet == null || _CncLogMessage_LogFifoGet.IsDisposed == true)
            {
                _CncLogMessage_LogFifoGet = new CncLogMessage();
            }
            data = _CncLogMessage_LogFifoGet;
            return LogFifoGet(_CncLogMessage_LogFifoGet);
        }
        [DllImport(G_GetCncServer.CncApiDll)]
        private unsafe static extern CncRc CncPosFifoGet(CNC_POS_FIFO_DATA* data);
        public unsafe static CncRc PosFifoGet(CncPosFifoData data)
        {
            CncRc return_result = CncPosFifoGet((CNC_POS_FIFO_DATA*)data.Pointer);
            G_GetCncServer.LastKnowRcState = return_result;
            return return_result;
        }
        public  static CncRc PosFifoGet(out CncPosFifoData data)
        {
            if (_CncPosFifoData_PosFifoGet == null || _CncPosFifoData_PosFifoGet.IsDisposed == true)
            {
                _CncPosFifoData_PosFifoGet = new CncPosFifoData();
            }
            data = _CncPosFifoData_PosFifoGet;
            return PosFifoGet(data);
        }
        [DllImport(G_GetCncServer.CncApiDll)]
        private unsafe static extern CncRc CncPosFifoGet2(CNC_POS_FIFO_DATA* data, int* isLast);
        public static CncRc PosFifoGet2(CncPosFifoData data, out int isLast)
        {
            unsafe
            {
                int temp_isLast = 0;
                CncRc return_result = CncPosFifoGet2((CNC_POS_FIFO_DATA*)data.Pointer, &temp_isLast);
                isLast = temp_isLast;
                G_GetCncServer.LastKnowRcState = return_result;
                return return_result;
            }
        }
        public static CncRc PosFifoGet2(out CncPosFifoData data, out int isLast)
        {
            if (_CncPosFifoData_PosFifoGet2 == null || _CncPosFifoData_PosFifoGet2.IsDisposed == true)
            {
                _CncPosFifoData_PosFifoGet2 = new CncPosFifoData();
            }
            data = _CncPosFifoData_PosFifoGet2;
            return PosFifoGet2(data, out isLast);
        }
        [DllImport(G_GetCncServer.CncApiDll)]
        private unsafe static extern CncRc CncGraphFifoGet(CNC_GRAPH_FIFO_DATA* data);
        public unsafe static CncRc GraphFifoGet(CncGraphFifoData data)
        {
            CncRc return_result = CncGraphFifoGet( (CNC_GRAPH_FIFO_DATA*)data.Pointer);
            
            G_GetCncServer.LastKnowRcState = return_result;
            return return_result;
        }
        public static CncRc GraphFifoGet(out CncGraphFifoData data)
        {
            if(_CncGraphFifoData_GraphFifoGet == null || _CncGraphFifoData_GraphFifoGet.IsDisposed == true)
            {
                _CncGraphFifoData_GraphFifoGet = new CncGraphFifoData();
            }
            data = _CncGraphFifoData_GraphFifoGet;
            return GraphFifoGet(_CncGraphFifoData_GraphFifoGet);
        }


    }

    
    public static class G_CommandsJobInterpreter
    {
        private static CncCmdArrayData GetJobArrayParameters_CncCmdArrayData = null;
        private static CncCmdArrayData SetJobArrayParameters_CncCmdArrayData = null;
        private static CncVector GetJobMaterialSize_CncVector = null;


        [DllImport(G_GetCncServer.CncApiDll)]
        private static extern CncRc CncReset();
        public static CncRc Reset()
        {
            CncRc return_result = CncReset();
            G_GetCncServer.LastKnowRcState = return_result;
            return return_result;
        }
        [DllImport(G_GetCncServer.CncApiDll)]
        private static extern CncRc CncReset2(uint resetFlags);
        public static CncRc Reset2(uint resetFlags)
        {
            CncRc return_result = CncReset2(resetFlags);
            G_GetCncServer.LastKnowRcState = return_result;
            return return_result;
        }
        [DllImport(G_GetCncServer.CncApiDll)]
        private unsafe static extern CncRc CncRunSingleLine(sbyte * text);
        public static CncRc RunSingleLine(string text)
        {
            unsafe
            {
                sbyte* temp_text = stackalloc sbyte[text.Length+1];
                StringConversie.StringToMaxCharArray(text,(IntPtr)temp_text,0, text.Length+1);
                CncRc return_result = CncRunSingleLine(temp_text);
                G_GetCncServer.LastKnowRcState = return_result;
                return return_result;
            }
        }
        //[SuppressUnmanagedCodeSecurity]
        [DllImport(G_GetCncServer.CncApiDll)]
        private static extern CncRc CncWaitSingleLine(IntPtr pKeepAlive, IntPtr  pKeepAliveParameter);
        public static CncRc WaitSingleLine(CncKeepUiAliveFunction pKeepAlive)
        {
            return CncWaitSingleLine(pKeepAlive.Functionpfunc, pKeepAlive._Functionparameter);
        }
        public static async Task<CncRc> WaitSingleLineAsync(CncKeepUiAliveFunction pKeepAlive)
        {
            return await Task.Run(()=>
            {
                return WaitSingleLine(pKeepAlive);
            });
        }


        [DllImport(G_GetCncServer.CncApiDll)]
        private unsafe static extern CncRc CncLoadJobA(sbyte * fileName);
        public unsafe static CncRc LoadJobA(string fileName)
        {
            CncRc return_result = CncRc.CNC_RC_OK;
            sbyte * pointer =  stackalloc sbyte[(int)CncConstants.CNC_MAX_PATH]; 
            StringConversie.StringToMaxCharArray(fileName, (IntPtr)pointer, 0, (int)CncConstants.CNC_MAX_PATH);
            return_result = CncLoadJobA(pointer);
            G_GetCncServer.LastKnowRcState = return_result;
            return return_result;
        }

        [DllImport(G_GetCncServer.CncApiDll)]
        private unsafe static extern CncRc CncLoadJobW(char * fileName);
        public unsafe static CncRc LoadJobW(string fileName)
        {
            CncRc return_result = CncRc.CNC_RC_OK;
            char* pointer = stackalloc char[(int)CncConstants.CNC_MAX_PATH];
            StringConversie.StringToMaxWCharArray(fileName, (IntPtr)pointer, 0, (int)CncConstants.CNC_MAX_PATH);
            return_result = CncLoadJobW(pointer);
            G_GetCncServer.LastKnowRcState = return_result;
            return return_result;
        }

        public static CncRc LoadJob(string fileName)
        {
            CncRc return_result = CncRc.CNC_RC_OK;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) == true)
            {
                return_result = LoadJobW(fileName);
            }
            else
            {
                return_result = LoadJobA(fileName);
            }
            return return_result;
        }
        [DllImport(G_GetCncServer.CncApiDll)]
        private static extern CncRc CncRunOrResumeJob();
        public static CncRc RunOrResumeJob()
        {
            CncRc return_result = CncRunOrResumeJob();
            G_GetCncServer.LastKnowRcState = return_result;
            return return_result;
        }
        [DllImport(G_GetCncServer.CncApiDll)]
        private static extern CncRc CncStartRenderGraph(int outLines, int contour);
        public static CncRc StartRenderGraph(int outLines, int contour)
        {
            CncRc return_result = CncStartRenderGraph( outLines,  contour);
            G_GetCncServer.LastKnowRcState = return_result;
            return return_result;
        }
        [DllImport(G_GetCncServer.CncApiDll)]
        private static extern CncRc CncStartRenderSearch(int outLines, int contour, int lineNr, int toolNr, int arrayX, int arrayY);
        public static CncRc StartRenderSearch(int outLines, int contour, int lineNr, int toolNr, int arrayX, int arrayY)
        {
            CncRc return_result = CncStartRenderSearch( outLines,  contour,  lineNr,  toolNr,  arrayX,  arrayY);
            G_GetCncServer.LastKnowRcState = return_result;
            return return_result;
        }
        [DllImport(G_GetCncServer.CncApiDll)]
        private static extern CncRc CncRewindJob();
        public static CncRc RewindJob()
        {
            CncRc return_result = CncRewindJob();
            G_GetCncServer.LastKnowRcState = return_result;
            return return_result;
        }
        [DllImport(G_GetCncServer.CncApiDll)]
        private static extern CncRc CncAbortJob();
        public static CncRc AbortJob()
        {
            CncRc return_result = CncAbortJob();
            G_GetCncServer.LastKnowRcState = return_result;
            return return_result;
        }
        [DllImport(G_GetCncServer.CncApiDll)]
        private unsafe static extern CncRc CncSetJobArrayParameters(CNC_CMD_ARRAY_DATA* runJobData);
        public unsafe static CncRc SetJobArrayParameters(CncCmdArrayData runJobData)
        {
            CncRc return_result = CncSetJobArrayParameters((CNC_CMD_ARRAY_DATA*)runJobData.Pointer);
            G_GetCncServer.LastKnowRcState = return_result;
            return return_result;
        }
        public static CncRc SetJobArrayParameters(out CncCmdArrayData runJobData)
        {
            if(SetJobArrayParameters_CncCmdArrayData == null || SetJobArrayParameters_CncCmdArrayData.IsDisposed == true)
            {
                SetJobArrayParameters_CncCmdArrayData = new CncCmdArrayData();
            }
            runJobData = SetJobArrayParameters_CncCmdArrayData;
            return SetJobArrayParameters(SetJobArrayParameters_CncCmdArrayData);
        }
        [DllImport(G_GetCncServer.CncApiDll)]
        private unsafe static extern CncRc CncGetJobArrayParameters(CNC_CMD_ARRAY_DATA* runJobData);
        public unsafe static CncRc GetJobArrayParameters(CncCmdArrayData runJobData)
        {
            CncRc return_result = CncGetJobArrayParameters((CNC_CMD_ARRAY_DATA*)runJobData.Pointer);
            G_GetCncServer.LastKnowRcState = return_result;
            return return_result;
        }
        public static CncRc GetJobArrayParameters(out CncCmdArrayData runJobData)
        {
            if(GetJobArrayParameters_CncCmdArrayData == null || GetJobArrayParameters_CncCmdArrayData.IsDisposed == true)
            {
                GetJobArrayParameters_CncCmdArrayData = new CncCmdArrayData();
            }
            runJobData = GetJobArrayParameters_CncCmdArrayData;
            return GetJobArrayParameters(GetJobArrayParameters_CncCmdArrayData);
        }
        [DllImport(G_GetCncServer.CncApiDll)]
        private static extern CNC_VECTOR CncGetJobMaterialSize();
        public static CncVector GetJobMaterialSize()
        {
            if(GetJobMaterialSize_CncVector == null || GetJobMaterialSize_CncVector.IsDisposed == true)
            {
                GetJobMaterialSize_CncVector = new CncVector();
            }
            CNC_VECTOR temp = CncGetJobMaterialSize();
            GetJobMaterialSize_CncVector.SetStructValue(ref temp);
            return GetJobMaterialSize_CncVector;
        }
        [DllImport(G_GetCncServer.CncApiDll)]
        private unsafe static extern CncRc CncGetJobFiducual(int n, CNC_FIDUCIAL_DATA* fiducial);
        public unsafe static CncRc GetJobFiducual(int n, CncFiducialData fiducial)
        {
            CncRc return_result = CncGetJobFiducual(n,(CNC_FIDUCIAL_DATA*)fiducial.Pointer);
            G_GetCncServer.LastKnowRcState = return_result;
            return return_result;
        }
        [DllImport(G_GetCncServer.CncApiDll)]
        private static extern CncRc CncEnableBlockDelete(int enable);
        public static CncRc EnableBlockDelete(int enable)
        {
            CncRc return_result = CncEnableBlockDelete(enable);
            G_GetCncServer.LastKnowRcState = return_result;
            return return_result;
        }
        [DllImport(G_GetCncServer.CncApiDll)]
        private static extern int CncGetBlocDelete();
        public static int GetBlocDelete()
        {
            return CncGetBlocDelete();
        }
        [DllImport(G_GetCncServer.CncApiDll)]
        private static extern CncRc CncEnableOptionalStop(int enable);
        public static CncRc EnableOptionalStop(int enable)
        {
            return CncEnableOptionalStop( enable);
        }
        [DllImport(G_GetCncServer.CncApiDll)]
        private static extern int CncGetOptionalStop();
        public static int GetOptionalStop()
        {
            return CncGetOptionalStop();
        }


        [DllImport(G_GetCncServer.CncApiDll)]
        private static extern CncRc CncSingleStepMode(int enable);
        public static CncRc SingleStepMode(int enable)
        {
            CncRc return_result = CncSingleStepMode( enable);
            G_GetCncServer.LastKnowRcState = return_result;
            return return_result;
        }
        [DllImport(G_GetCncServer.CncApiDll)]
        private static extern int CncGetSingleStepMode();
        public static int GetSingleStepMode()
        {
            return CncGetSingleStepMode();
        }
        [DllImport(G_GetCncServer.CncApiDll)]
        private unsafe static extern CncRc CncSetExtraJobOptions(sbyte* extraLine, int doRepeat, uint numberOfRepeats);
        public unsafe static CncRc SetExtraJobOptions(string extraLine, int doRepeat, uint numberOfRepeats)
        {
            CncRc return_result = CncRc.CNC_RC_OK;
            sbyte* temp = stackalloc sbyte[(int)CncConstants.CNC_MAX_INTERPRETER_LINE];
            return_result = CncSetExtraJobOptions(temp, doRepeat, numberOfRepeats);
            G_GetCncServer.LastKnowRcState = return_result;
            return return_result;
        }
        [DllImport(G_GetCncServer.CncApiDll)]
        private unsafe static extern void CncGetExtraJobOptions(sbyte * extraLine, int* doRepeat, uint* numberOfRepeats);
        public unsafe static void GetExtraJobOptions(out string extraLine,out int doRepeat,out uint numberOfRepeats)
        {
            sbyte* temp = stackalloc sbyte[(int)CncConstants.CNC_MAX_INTERPRETER_LINE];
            
            int temp_doRepeat = 0;
            uint temp_numberOfRepeats = 0;
            CncGetExtraJobOptions(temp, &temp_doRepeat, &temp_numberOfRepeats);
            doRepeat = temp_doRepeat;
            numberOfRepeats = temp_numberOfRepeats;
            extraLine = StringConversie.CharArrayToString((IntPtr)temp, 0, (int)CncConstants.CNC_MAX_INTERPRETER_LINE);
        }
        [DllImport(G_GetCncServer.CncApiDll)]
        private static extern CncRc CncSetSimulationMode(int enable);
        public static CncRc SetSimulationMode(int enable)
        {
            CncRc return_result = CncSetSimulationMode(enable);
            G_GetCncServer.LastKnowRcState = return_result;
            return return_result;
        }
        [DllImport(G_GetCncServer.CncApiDll)]
        private static extern int CncGetSimulationMode();
        public static int GetSimulationMode()
        {
            return CncGetSimulationMode();
        }
        [DllImport(G_GetCncServer.CncApiDll)]
        private static extern CncRc CncSetFeedOverride(double factor);
        public static CncRc SetFeedOverride(double factor)
        {
            CncRc return_result = CncSetFeedOverride(factor);
            G_GetCncServer.LastKnowRcState = return_result;
            return return_result;
        }
        [DllImport(G_GetCncServer.CncApiDll)]
        private static extern CncRc CncSetArcFeedOverride(double factor);
        public static CncRc SetArcFeedOverride(double factor)
        {
            CncRc return_result = CncSetArcFeedOverride(factor);
            G_GetCncServer.LastKnowRcState = return_result;
            return return_result;
        }
        [DllImport(G_GetCncServer.CncApiDll)]
        private static extern double CncGetActualFeedOverride();
        public static double GetActualFeedOverride()
        {
            return CncGetActualFeedOverride();
        }
        [DllImport(G_GetCncServer.CncApiDll)]
        private static extern double CncGetActualArcFeedOverride();
        public static double GetActualArcFeedOverride()
        {
            return CncGetActualArcFeedOverride();
        }
        [DllImport(G_GetCncServer.CncApiDll)]
        private static extern double CncGetActualFeed();
        public static double GetActualFeed()
        {
            return CncGetActualFeed();
        }
        [DllImport(G_GetCncServer.CncApiDll)]
        private static extern CncRc CncSetSpeedOverride(double factor);
        public static CncRc SetSpeedOverride(double factor)
        {
            CncRc return_result = CncSetSpeedOverride(factor);
            G_GetCncServer.LastKnowRcState = return_result;
            return return_result;
        }
        [DllImport(G_GetCncServer.CncApiDll)]
        private static extern double CncGetActualSpeedOverride();
        public static double GetActualSpeedOverride()
        {
            return CncGetActualSpeedOverride();
        }
        [DllImport(G_GetCncServer.CncApiDll)]
        private static extern double CncGetActualSpeed();
        public static double GetActualSpeed()
        {
            return GetActualSpeed();
        }
        [DllImport(G_GetCncServer.CncApiDll)]
        private unsafe static extern CncRc CncFindFirstJobLine(sbyte * text, int * endOfJob, int * totNumOfLines);
        public unsafe static CncRc FindFirstJobLine(out string text, out int endOfJob, out int totNumOfLines)
        {
            CncRc return_result = CncRc.CNC_RC_OK;
            sbyte* temp = stackalloc sbyte[(int)CncConstants.CNC_MAX_INTERPRETER_LINE];
            int temp_endOfJob = 0;
            int temp_totNumOfLines = 0;
            return_result = CncFindFirstJobLine(temp, &temp_endOfJob, &temp_totNumOfLines);
            text = StringConversie.CharArrayToString((IntPtr)temp,0, (int)CncConstants.CNC_MAX_INTERPRETER_LINE);
            endOfJob = temp_endOfJob;
            totNumOfLines = temp_totNumOfLines;
            G_GetCncServer.LastKnowRcState = return_result;
            return return_result;
        }
        [DllImport(G_GetCncServer.CncApiDll)]
        private unsafe static extern CncRc CncFindFirstJobLineF(sbyte * text, int * endOfJob);
        public unsafe static CncRc FindFirstJobLineF(out string text,out int endOfJob)
        {
            CncRc return_result = CncRc.CNC_RC_OK;
            sbyte* temp = stackalloc sbyte[(int)CncConstants.CNC_MAX_INTERPRETER_LINE];
            int temp_endOfJob = 0;
            return_result = CncFindFirstJobLineF(temp, &temp_endOfJob);
            text = StringConversie.CharArrayToString((IntPtr)temp,0, (int)CncConstants.CNC_MAX_INTERPRETER_LINE);
            endOfJob = temp_endOfJob;
            G_GetCncServer.LastKnowRcState = return_result;
            return return_result;
        }
        [DllImport(G_GetCncServer.CncApiDll)]
        private unsafe static extern CncRc CncFindNextJobLine(sbyte * text, int* endOfJob);
        public unsafe static CncRc FindNextJobLine(out string text,out int endOfJob)
        {
            CncRc return_result = CncRc.CNC_RC_OK;
            sbyte* temp = stackalloc sbyte[(int)CncConstants.CNC_MAX_INTERPRETER_LINE];
            int temp_endOfJob = 0;
            return_result = CncFindNextJobLine(temp, &temp_endOfJob);
            text = StringConversie.CharArrayToString((IntPtr)temp, 0, (int)CncConstants.CNC_MAX_INTERPRETER_LINE);
            endOfJob = temp_endOfJob;
            G_GetCncServer.LastKnowRcState = return_result;
            return return_result;
        }
        [DllImport(G_GetCncServer.CncApiDll)]
        private unsafe static extern CncRc CncFindNextJobLineF(sbyte * text, int* endOfJob);
        public unsafe static CncRc FindNextJobLineF(out string text,out int endOfJob)
        {
            CncRc return_result = CncRc.CNC_RC_OK;
            sbyte * temp = stackalloc sbyte[(int)CncConstants.CNC_MAX_INTERPRETER_LINE];
            int temp_endOfJob = 0;
            return_result = CncFindNextJobLineF(temp, &temp_endOfJob);
            text = StringConversie.CharArrayToString((IntPtr)temp,0, (int)CncConstants.CNC_MAX_INTERPRETER_LINE);
            endOfJob = temp_endOfJob;
            G_GetCncServer.LastKnowRcState = return_result;
            return return_result;
        }
    }
    public static class G_PauseFunctions
    {
        [DllImport(G_GetCncServer.CncApiDll)]
        private static extern CncRc CncSwitchOffSpindleAndWaitUntilOff(IntPtr pFunc, IntPtr pFuncParameter);
        public static CncRc SwitchOffSpindleAndWaitUntilOff(CncKeepUiAliveFunction pFunc)
        {
            return CncSwitchOffSpindleAndWaitUntilOff(pFunc.Functionpfunc, pFunc._Functionparameter);
        }
        public static async Task<CncRc> SwitchOffSpindleAndWaitUntilOffAsync(CncKeepUiAliveFunction pFunc)
        {
            return await Task.Run(() =>
            {
                return SwitchOffSpindleAndWaitUntilOff(pFunc);
            });
        }


        [DllImport(G_GetCncServer.CncApiDll)]
        private static extern CncRc CncSwitchOnSpindleAndWaitUntilOn(IntPtr pFunc, IntPtr pFuncParameter, bool CCW);
        public static CncRc SwitchOnSpindleAndWaitUntilOn(CncKeepUiAliveFunction pFunc, bool CCW)
        {
            return CncSwitchOnSpindleAndWaitUntilOn(pFunc.Functionpfunc, pFunc._Functionparameter, CCW);
        }
        public  static async Task<CncRc> SwitchOnSpindleAndWaitUntilOnAsync(CncKeepUiAliveFunction pFunc, bool CCW)
        {
            return await Task.Run(()=>
            {
                return SwitchOnSpindleAndWaitUntilOn(pFunc, CCW);
            });
        }
        public static int SwitchOnSpindleAndWaitUntilOn(CncKeepUiAliveFunction pFunc)
        {
            return Convert.ToInt32(CncSwitchOnSpindleAndWaitUntilOn(pFunc.Functionpfunc, pFunc._Functionparameter, false));
        }
        public static async Task<int> SwitchOnSpindleAndWaitUntilOnAsync(CncKeepUiAliveFunction pFunc)
        {
            return await Task.Run(() =>
            {
                return SwitchOnSpindleAndWaitUntilOn(pFunc);
            });
        }


        [DllImport(G_GetCncServer.CncApiDll)]
        private static extern CncRc CncPauseJob();
        public static CncRc PauseJob()
        {
            CncRc return_result = CncPauseJob();
            G_GetCncServer.LastKnowRcState = return_result;
            return return_result;
        }
        [DllImport(G_GetCncServer.CncApiDll)]
        private static extern CncRc CncPauseJob2(IntPtr pFunc, IntPtr pFuncParameter);
        public static CncRc PauseJob2(CncKeepUiAliveFunction pFunc)
        {
            CncRc return_result = CncPauseJob2(pFunc.Functionpfunc, pFunc._Functionparameter);
            G_GetCncServer.LastKnowRcState = return_result;
            return return_result;
        }
        public static async Task<CncRc> PauseJob2Async(CncKeepUiAliveFunction pFunc)
        {
            return await Task.Run(()=>
            {
                return PauseJob2(pFunc);
            });
        }
        [DllImport(G_GetCncServer.CncApiDll)]
        private static extern int CncSyncPauseZSafe();
        public static int SyncPauseZSafe()
        {
            return SyncPauseZSafe();
        }
        [DllImport(G_GetCncServer.CncApiDll)]
        private static extern int CncSyncPauseXSafe();
        public static int SyncPauseXSafe()
        {
            return CncSyncPauseXSafe();
        }
        
        [DllImport(G_GetCncServer.CncApiDll)]
        private static extern int CncSyncPauseAxis(int axis, double feed, IntPtr pFunc, IntPtr pFuncParameter);
        public static int SyncPauseAxis(int axis, double feed, CncKeepUiAliveFunction pFunc)
        {
            return CncSyncPauseAxis(axis,feed, pFunc.Functionpfunc, pFunc._Functionparameter);
        }
        public static async Task<int> SyncPauseAxisAsync(int axis, double feed, CncKeepUiAliveFunction pFunc)
        {
            return await Task.Run(() =>
            {
                return SyncPauseAxis(axis,feed,pFunc);
            });
        }

        [DllImport(G_GetCncServer.CncApiDll)]
        private static extern int CncSyncFromPauseAndStartAutomatic(double approachFeed, IntPtr pFunc, IntPtr pFuncParameter);
        public static int SyncFromPauseAndStartAutomatic(double approachFeed, CncKeepUiAliveFunction pFunc)
        {
            return CncSyncFromPauseAndStartAutomatic(approachFeed,pFunc.Functionpfunc, pFunc._Functionparameter);
        }
        public static async Task<int> SyncFromPauseAndStartAutomaticAsync(double approachFeed, CncKeepUiAliveFunction pFunc)
        {
            return await Task.Run(()=>
            {
                return SyncFromPauseAndStartAutomatic(approachFeed,pFunc);
            });
        }
    }
    public static class G_SearchFunctions
    {
        [DllImport(G_GetCncServer.CncApiDll)]
        private static extern int CncSyncSearchZSafe(IntPtr pFunc, IntPtr pFuncParameter);
        public static int SyncSearchZSafe(CncKeepUiAliveFunction pFunc)
        {
            return CncSyncSearchZSafe(pFunc.Functionpfunc,pFunc._Functionparameter);
        }
        public static async Task<int> SyncSearchZSafeAsync(CncKeepUiAliveFunction pFunc)
        {
            return await Task.Run(()=>
            {
                return SyncSearchZSafe(pFunc);
            });
        }
        [DllImport(G_GetCncServer.CncApiDll)]
        private static extern int CncSyncSearchXSafe(IntPtr pFunc, IntPtr pFuncParameter);
        public static int SyncSearchXSafe(CncKeepUiAliveFunction pFunc)
        {
            return CncSyncSearchXSafe(pFunc.Functionpfunc, pFunc._Functionparameter);
        }
        public static async Task<int> SyncSearchXSafeAsync(CncKeepUiAliveFunction pFunc)
        {
            return await Task.Run(()=>
            {
                return SyncSearchXSafe(pFunc);
            });
        }
        [DllImport(G_GetCncServer.CncApiDll)]
        private static extern int CncSyncSearchTool(IntPtr pFunc, IntPtr pFuncParameter);
        public static int SyncSearchTool(CncKeepUiAliveFunction pFunc)
        {
            return CncSyncSearchTool(pFunc.Functionpfunc, pFunc._Functionparameter);
        }
        public static async Task<int> SyncSearchToolAsync(CncKeepUiAliveFunction pFunc)
        {
            return await Task.Run(()=>
            {
                return SyncSearchTool(pFunc);
            });
        }
        [DllImport(G_GetCncServer.CncApiDll)]
        private static extern int CncSyncInchModeAndParametersAndOffset(IntPtr pFunc, IntPtr pFuncParameter);
        public static int SyncInchModeAndParametersAndOffset(CncKeepUiAliveFunction pFunc)
        {
            return CncSyncInchModeAndParametersAndOffset(pFunc.Functionpfunc, pFunc._Functionparameter);
        }
        public static async Task<int> SyncInchModeAndParametersAndOffsetAsync(CncKeepUiAliveFunction pFunc)
        {
            return await Task.Run(()=>
            {
                return SyncInchModeAndParametersAndOffset(pFunc);
            });
        }
        [DllImport(G_GetCncServer.CncApiDll)]
        private static extern int CncSyncSearchAxis(int axis, double feed, IntPtr pFunc, IntPtr pFuncParameter);
        public static int SyncSearchAxis(int axis, double feed, CncKeepUiAliveFunction pFunc)
        {
            return CncSyncSearchAxis(axis,feed, pFunc.Functionpfunc, pFunc._Functionparameter);
        }
        public static async Task<int> SyncSearchAxisAsync(int axis, double feed, CncKeepUiAliveFunction pFunc)
        {
            return await Task.Run(()=>
            {
                return SyncSearchAxis(axis, feed, pFunc);
            });
        }
        [DllImport(G_GetCncServer.CncApiDll)]
        private static extern int CncSyncFromSearchAndStartAutomatic(double approachFeed, IntPtr pFunc, IntPtr pFuncParameter);
        public static int SyncFromSearchAndStartAutomatic(double approachFeed, CncKeepUiAliveFunction pFunc)
        {
            return CncSyncFromSearchAndStartAutomatic(approachFeed,pFunc.Functionpfunc, pFunc._Functionparameter);
        }
        public static async Task<int> SyncFromSearchAndStartAutomaticAsync(double approachFeed, CncKeepUiAliveFunction pFunc)
        {
            return await Task.Run(()=>
            {
                return SyncFromSearchAndStartAutomatic(approachFeed,pFunc);
            });
        }

    }
    public static class G_JoggingFunctions
    {
        [DllImport(G_GetCncServer.CncApiDll)]
        private unsafe static extern CncRc CncStartJog(double* axes,
                                    double velocityFactor,
                                    int continuous);
        public static unsafe CncRc StartJog(double[] axes,double velocityFactor,int continuous)
        {
            double* temp = stackalloc double[(int)CncConstants.CNC_MAX_AXES];
            for(int i =0;i< (int)CncConstants.CNC_MAX_AXES;i++)
            {
                if(i< axes.Length)
                {
                    temp[i] = axes[i];
                }
                else
                {
                    temp[i] = 0;
                }
            }
            CncRc return_result = CncStartJog(temp,velocityFactor, continuous);
            G_GetCncServer.LastKnowRcState = return_result;
            return return_result;
        }
        public static CncRc StartJog(CncCartDouble axes, double velocityFactor, int continuous)
        {
            return StartJog(new double[] 
            {
                axes.x, 
                axes.y, 
                axes.z, 
                axes.a, 
                axes.b, 
                axes.c 
            }, velocityFactor, continuous);
        }
        [DllImport(G_GetCncServer.CncApiDll)]
        private static extern CncRc CncStartJog2(int axis,double step,double velocityFactor,int continuous);
        public static CncRc StartJog2(int axis,double step,double velocityFactor,int continuous)
        {
            CncRc return_result = CncStartJog2(axis, step, velocityFactor,continuous);
            G_GetCncServer.LastKnowRcState = return_result;
            return return_result;
        }
        [DllImport(G_GetCncServer.CncApiDll)]
        private static extern CncRc CncStopJog(int axis);
        public static CncRc StopJog(int axis)
        {
            CncRc return_Result = CncStopJog(axis);
            G_GetCncServer.LastKnowRcState = return_Result;
            return return_Result;
        }
        [DllImport(G_GetCncServer.CncApiDll)]
        private static extern CncRc CncMoveTo(CNC_CART_DOUBLE pos, CNC_CART_BOOL move, double velocityFactor);
        public static CncRc MoveTo(CncCartDouble pos, CncCartBool move, double velocityFactor)
        {
            CncRc return_result = CncMoveTo(pos.GetStructValue(), move.GetStructValue(), velocityFactor);
            G_GetCncServer.LastKnowRcState = return_result;
            return return_result;
        }

    }
    public static class G_TrackingFunctions
    {
        private static CncThcProcessParameters CncThcProcessParameters_GetPlasmaParameters = null;

        [DllImport(G_GetCncServer.CncApiDll)]
        private static extern CncRc CncStartPositionTracking();
        public static CncRc StartPositionTracking()
        {
            CncRc return_result = CncStartPositionTracking();
            G_GetCncServer.LastKnowRcState = return_result;
            return return_result;
        }
        [DllImport(G_GetCncServer.CncApiDll)]
        private static extern CncRc CncStartVelocityTracking();
        public static CncRc StartVelocityTracking()
        {
            CncRc return_result = CncStartVelocityTracking();
            G_GetCncServer.LastKnowRcState = return_result;
            return return_result;
        }
        [DllImport(G_GetCncServer.CncApiDll)]
        private static extern CncRc CncStartHandweelTracking(int axis, double vLimit, int handwheelID,int velMode,double multiplicationFactor,int handwheelCountsPerRevolution);
        public static CncRc StartHandweelTracking(int axis, double vLimit, int handwheelID,int velMode,double multiplicationFactor,int handwheelCountsPerRevolution)
        {
            CncRc return_result = CncStartHandweelTracking( axis,  vLimit,  handwheelID,velMode,multiplicationFactor,handwheelCountsPerRevolution);
            G_GetCncServer.LastKnowRcState = return_result;
            return return_result;
        }
        [DllImport(G_GetCncServer.CncApiDll)]
        private static extern CncRc CncSetTrackingPosition(CNC_CART_DOUBLE pos,CNC_CART_DOUBLE vel,CNC_CART_BOOL move);
        public static CncRc SetTrackingPosition(CncCartDouble pos, CncCartDouble vel, CncCartBool move)
        {
            CncRc return_result = CncSetTrackingPosition(pos.GetStructValue(), vel.GetStructValue(), move.GetStructValue());
            G_GetCncServer.LastKnowRcState = return_result;
            return return_result;
        }
        [DllImport(G_GetCncServer.CncApiDll)]
        private static extern CncRc CncSetTrackingPosition2(CNC_CART_DOUBLE pos,CNC_CART_DOUBLE vel,CNC_CART_DOUBLE acc,CNC_CART_BOOL move);
        public static CncRc SetTrackingPosition2(CncCartDouble pos, CncCartDouble vel, CncCartDouble acc, CncCartBool move)
        {
            CncRc return_result = CncSetTrackingPosition2(pos.GetStructValue(),vel.GetStructValue(), acc.GetStructValue(), move.GetStructValue());
            G_GetCncServer.LastKnowRcState = return_result;
            return return_result;
        }
        [DllImport(G_GetCncServer.CncApiDll)]
        private static extern CncRc CncSetTrackingVelocity(CNC_CART_DOUBLE vel,CNC_CART_BOOL move);
        public static CncRc SetTrackingVelocity(CncCartDouble vel, CncCartBool move)
        {
            CncRc return_result = CncSetTrackingVelocity(vel.GetStructValue(),move.GetStructValue());
            G_GetCncServer.LastKnowRcState = return_result;
            return return_result;
        }
        [DllImport(G_GetCncServer.CncApiDll)]
        private static extern CncRc CncSetTrackingVelocity2(CNC_CART_DOUBLE vel,CNC_CART_DOUBLE acc,CNC_CART_BOOL axes);
        public static CncRc SetTrackingVelocity2(CncCartDouble vel, CncCartDouble acc, CncCartBool axes)
        {
            CncRc return_result = CncSetTrackingVelocity2(vel.GetStructValue(),acc.GetStructValue(), axes.GetStructValue());
            G_GetCncServer.LastKnowRcState = return_result;
            return return_result;
        }
        [DllImport(G_GetCncServer.CncApiDll)]
        private static extern CncRc CncSetTrackingHandwheelCounter(int hw1Count, int hw2Count, int hw3Count);
        public static CncRc SetTrackingHandwheelCounter(int hw1Count, int hw2Count, int hw3Count)
        {
            CncRc return_result = CncSetTrackingHandwheelCounter(hw1Count, hw2Count, hw3Count);
            G_GetCncServer.LastKnowRcState = return_result;
            return return_result;
        }
        [DllImport(G_GetCncServer.CncApiDll)] 
        private static extern CncRc CncStartPlasmaTHCTracking(double pLimit, double nLimit);
        public static CncRc StartPlasmaTHCTracking(double pLimit, double nLimit)
        {
            CncRc return_result = CncStartPlasmaTHCTracking( pLimit,  nLimit);
            G_GetCncServer.LastKnowRcState = return_result;
            return return_result;
        }
        [DllImport(G_GetCncServer.CncApiDll)]
        private static extern CncRc CncSetPlasmaParameters(CNC_THC_PROCESS_PARAMETERS thcCfg);
        public static CncRc SetPlasmaParameters(CncThcProcessParameters thcCfg)
        {
            CncRc return_result = CncSetPlasmaParameters(thcCfg.GetStructValue());
            G_GetCncServer.LastKnowRcState = return_result;
            return return_result;
        }
        [DllImport(G_GetCncServer.CncApiDll)]
        private unsafe static extern CNC_THC_PROCESS_PARAMETERS* CncGetPlasmaParameters();
        public static unsafe CncThcProcessParameters GetPlasmaParameters()
        {
            if(CncThcProcessParameters_GetPlasmaParameters == null || CncThcProcessParameters_GetPlasmaParameters.IsDisposed == true)
            {
                CncThcProcessParameters_GetPlasmaParameters = new CncThcProcessParameters((IntPtr)CncGetPlasmaParameters());
            }
            else
            {
                CncThcProcessParameters_GetPlasmaParameters.Pointer = (IntPtr)CncGetPlasmaParameters();
            }
            return CncThcProcessParameters_GetPlasmaParameters;
        }
        [DllImport(G_GetCncServer.CncApiDll)]
        private static extern CncRc CncStopTracking();
        public static CncRc StopTracking()
        {
            CncRc return_result = CncStopTracking();
            G_GetCncServer.LastKnowRcState = return_result;
            return return_result;
        }
    }
    public static class G_3DPrinter
    {
        private static Cnc3dprintingCommand Cnc3dprintingCommand_3DPrintCommand = null;

        [DllImport(G_GetCncServer.CncApiDll)]
        private unsafe static extern CncRc Cnc3DPrintCommand(CNC_3DPRINTING_COMMAND* pCmd);
        public unsafe static CncRc _3DPrintCommand(Cnc3dprintingCommand pCmd)
        {
            CncRc return_result = Cnc3DPrintCommand((CNC_3DPRINTING_COMMAND*)pCmd.Pointer);
            G_GetCncServer.LastKnowRcState = return_result;
            return return_result;
        }
        public static CncRc _3DPrintCommand(out Cnc3dprintingCommand pCmd)
        {
            if(Cnc3dprintingCommand_3DPrintCommand == null || Cnc3dprintingCommand_3DPrintCommand.IsDisposed == true)
            {
                Cnc3dprintingCommand_3DPrintCommand = new Cnc3dprintingCommand();
            }
            pCmd = Cnc3dprintingCommand_3DPrintCommand;
            return _3DPrintCommand(Cnc3dprintingCommand_3DPrintCommand);
        }
    }
    public static class G_UtilityItems
    {
        [DllImport(G_GetCncServer.CncApiDll)]
        private unsafe static extern sbyte * CncGetRCText(CncRc rc);
        public unsafe static string GetRCText(CncRc rc)
        {
            return StringConversie.CharArrayToString((IntPtr)CncGetRCText(rc), 0, (int)CncConstants.CNC_MAX_NAME_LENGTH);
        }
        [DllImport(G_GetCncServer.CncApiDll)]
        private unsafe static extern void CncSendUserMessage(sbyte * functionName, sbyte * fileName, int lineNumber, CncErrorClass ec, CncRc rc, sbyte * msg);
        public unsafe static void SendUserMessage(string functionName, string fileName, int lineNumber, CncErrorClass ec, CncRc rc, string msg)
        {
            sbyte* temp_functionName = stackalloc sbyte[(int)CncConstants.CNC_MAX_FUNCTION_NAME_TEXT];
            sbyte* temp_fileName = stackalloc sbyte[(int)CncConstants.CNC_MAX_PATH];
            sbyte* temp_msg = stackalloc sbyte[(int)CncConstants.CNC_MAX_MESSAGE_TEXT];

            StringConversie.StringToMaxCharArray(functionName,(IntPtr)temp_functionName,0, (int)CncConstants.CNC_MAX_FUNCTION_NAME_TEXT);
            StringConversie.StringToMaxCharArray(fileName, (IntPtr)temp_fileName,0, (int)CncConstants.CNC_MAX_PATH);
            StringConversie.StringToMaxCharArray(msg, (IntPtr)temp_msg,0, (int)CncConstants.CNC_MAX_MESSAGE_TEXT);
            CncSendUserMessage(temp_functionName, temp_fileName,lineNumber,ec,rc, temp_msg);
        }
    }

}
