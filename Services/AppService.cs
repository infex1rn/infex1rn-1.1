using iMobileDevice;
using iMobileDevice.iDevice;
using iMobileDevice.InstallationProxy;
using iMobileDevice.Lockdown;
using iMobileDevice.Plist;
using System;
using System.Collections.Generic;

namespace infex1rn.Services
{
    public class AppService
    {
        public List<string> GetInstalledApps(string udid)
        {
            var apps = new List<string>();
            var idevice = LibiMobileDevice.Instance.iDevice;
            var lockdown = LibiMobileDevice.Instance.Lockdown;
            var installationProxy = LibiMobileDevice.Instance.InstallationProxy;
            var plist = LibiMobileDevice.Instance.Plist;

            iDeviceHandle deviceHandle;
            idevice.idevice_new(out deviceHandle, udid).ThrowOnError();
            using (deviceHandle)
            {
                LockdownClientHandle lockdownHandle;
                lockdown.lockdownd_client_new_with_handshake(deviceHandle, out lockdownHandle, "infex1rn").ThrowOnError();
                using (lockdownHandle)
                {
                    LockdownServiceDescriptorHandle service;
                    installationProxy.instproxy_client_start_service(lockdownHandle, out service, "infex1rn").ThrowOnError();
                    using (service)
                    {
                        InstallationProxyClientHandle client;
                        installationProxy.instproxy_client_new(deviceHandle, service, out client).ThrowOnError();
                        using (client)
                        {
                            PlistHandle options = plist.plist_new_dict();
                            using (options)
                            {
                                plist.plist_dict_set_item(options, "ReturnAttributes", plist.plist_new_array());
                                PlistHandle returnAttributes = plist.plist_dict_get_item(options, "ReturnAttributes");
                                plist.plist_array_append_item(returnAttributes, plist.plist_new_string("CFBundleDisplayName"));

                                PlistHandle appNodes;
                                installationProxy.instproxy_browse(client, options, out appNodes).ThrowOnError();
                                using (appNodes)
                                {
                                    for (uint i = 0; i < plist.plist_array_get_size(appNodes); i++)
                                    {
                                        PlistHandle app = plist.plist_array_get_item(appNodes, i);
                                        PlistHandle appNameNode = plist.plist_dict_get_item(app, "CFBundleDisplayName");
                                        if (appNameNode.IsNotNull)
                                        {
                                            string appName;
                                            plist.plist_get_string_val(appNameNode, out appName);
                                            apps.Add(appName);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            return apps;
        }

        public void InstallIpa(string udid, string ipaPath, Action<string, PlistHandle> statusCallback)
        {
            var idevice = LibiMobileDevice.Instance.iDevice;
            var lockdown = LibiMobileDevice.Instance.Lockdown;
            var installationProxy = LibiMobileDevice.Instance.InstallationProxy;

            iDeviceHandle deviceHandle;
            idevice.idevice_new(out deviceHandle, udid).ThrowOnError();
            using (deviceHandle)
            {
                LockdownClientHandle lockdownHandle;
                lockdown.lockdownd_client_new_with_handshake(deviceHandle, out lockdownHandle, "infex1rn").ThrowOnError();
                using (lockdownHandle)
                {
                    LockdownServiceDescriptorHandle service;
                    installationProxy.instproxy_client_start_service(lockdownHandle, out service, "infex1rn").ThrowOnError();
                    using (service)
                    {
                        InstallationProxyClientHandle client;
                        installationProxy.instproxy_client_new(deviceHandle, service, out client).ThrowOnError();
                        using (client)
                        {
                            installationProxy.instproxy_install(client, ipaPath, null, statusCallback, null).ThrowOnError();
                        }
                    }
                }
            }
        }
    }
}
