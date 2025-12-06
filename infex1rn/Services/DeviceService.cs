using iMobileDevice;
using iMobileDevice.iDevice;
using iMobileDevice.Lockdown;
using iMobileDevice.Recovery;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace infex1rn.Services
{
    public class DeviceService
    {
        public ReadOnlyCollection<string> GetDeviceList()
        {
            var idevice = LibiMobileDevice.Instance.iDevice;
            ReadOnlyCollection<string> udids;
            int count = 0;
            var ret = idevice.idevice_get_device_list(out udids, ref count);
            if (ret == iDeviceError.NoDevice)
            {
                return new List<string>().AsReadOnly();
            }
            ret.ThrowOnError();
            return udids;
        }

        public Dictionary<string, string> GetDeviceInfo(string udid)
        {
            var info = new Dictionary<string, string>();
            var idevice = LibiMobileDevice.Instance.iDevice;
            var lockdown = LibiMobileDevice.Instance.Lockdown;
            var plist = LibiMobileDevice.Instance.Plist;

            iDeviceHandle deviceHandle;
            idevice.idevice_new(out deviceHandle, udid).ThrowOnError();

            using (deviceHandle)
            {
                LockdownClientHandle lockdownHandle;
                lockdown.lockdownd_client_new_with_handshake(deviceHandle, out lockdownHandle, "infex1rn").ThrowOnError();

                using (lockdownHandle)
                {
                    string deviceName;
                    lockdown.lockdownd_get_device_name(lockdownHandle, out deviceName).ThrowOnError();
                    info["DeviceName"] = deviceName;

                    PlistHandle modelNode;
                    lockdown.lockdownd_get_value(lockdownHandle, null, "ProductType", out modelNode).ThrowOnError();
                    using(modelNode)
                    {
                        string model;
                        plist.plist_get_string_val(modelNode, out model);
                        info["Model"] = model;
                    }

                    PlistHandle versionNode;
                    lockdown.lockdownd_get_value(lockdownHandle, null, "ProductVersion", out versionNode).ThrowOnError();
                    using(versionNode)
                    {
                        string version;
                        plist.plist_get_string_val(versionNode, out version);
                        info["iOSVersion"] = version;
                    }

                    PlistHandle serialNode;
                    lockdown.lockdownd_get_value(lockdownHandle, null, "SerialNumber", out serialNode).ThrowOnError();
                    using(serialNode)
                    {
                        string serial;
                        plist.plist_get_string_val(serialNode, out serial);
                        info["SerialNumber"] = serial;
                    }
                }
            }

            return info;
        }

        public void EnterRecovery(string udid)
        {
            var idevice = LibiMobileDevice.Instance.iDevice;
            var lockdown = LibiMobileDevice.Instance.Lockdown;

            iDeviceHandle deviceHandle;
            idevice.idevice_new(out deviceHandle, udid).ThrowOnError();
            using (deviceHandle)
            {
                LockdownClientHandle lockdownHandle;
                lockdown.lockdownd_client_new_with_handshake(deviceHandle, out lockdownHandle, "infex1rn").ThrowOnError();
                using(lockdownHandle)
                {
                    lockdown.lockdownd_enter_recovery(lockdownHandle).ThrowOnError();
                }
            }
        }

        public void ExitRecovery(string udid)
        {
            var idevice = LibiMobileDevice.Instance.iDevice;
            var recovery = LibiMobileDevice.Instance.Recovery;

            iDeviceHandle deviceHandle;
            idevice.idevice_new(out deviceHandle, udid).ThrowOnError();
            using(deviceHandle)
            {
                RecoveryClientHandle recoveryHandle;
                recovery.irecv_client_new(deviceHandle, out recoveryHandle, "infex1rn").ThrowOnError();
                using(recoveryHandle)
                {
                    recovery.irecv_setenv(recoveryHandle, "auto-boot", "true").ThrowOnError();
                    recovery.irecv_saveenv(recoveryHandle).ThrowOnError();
                    recovery.irecv_reboot(recoveryHandle).ThrowOnError();
                }
            }
        }

        public void LoadRamdisk(string udid, string ramdiskPath)
        {
            var idevice = LibiMobileDevice.Instance.iDevice;
            var recovery = LibiMobileDevice.Instance.Recovery;

            iDeviceHandle deviceHandle;
            idevice.idevice_new(out deviceHandle, udid).ThrowOnError();
            using(deviceHandle)
            {
                RecoveryClientHandle recoveryHandle;
                recovery.irecv_client_new(deviceHandle, out recoveryHandle, "infex1rn").ThrowOnError();
                using(recoveryHandle)
                {
                    byte[] ramdiskBytes = File.ReadAllBytes(ramdiskPath);
                    recovery.irecv_send_buffer(recoveryHandle, ramdiskBytes, (uint)ramdiskBytes.Length, 1).ThrowOnError();

                    recovery.irecv_setenv(recoveryHandle, "boot-args", "-v").ThrowOnError();
                    recovery.irecv_saveenv(recoveryHandle).ThrowOnError();

                    recovery.irecv_send_command(recoveryHandle, "ramdisk").ThrowOnError();
                }
            }
        }

        public async Task RunExternalTool(string executablePath, string arguments, Action<string> onOutput)
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = executablePath,
                    Arguments = arguments,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                }
            };

            process.OutputDataReceived += (sender, args) => 
            {
                if (args.Data != null)
                {
                    Console.WriteLine(args.Data);
                    onOutput(args.Data);
                }
            };
            process.ErrorDataReceived += (sender, args) => 
            {
                if (args.Data != null)
                {
                    Console.WriteLine(args.Data);
                    onOutput(args.Data);
                }
            };

            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();

            await process.WaitForExitAsync();
        }
    }
}
