using infex1rn.Services;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Compression;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using iMobileDevice;
using iMobileDevice.Plist;
using iMobileDevice.iDevice;
using iMobileDevice.HouseArrest;
using iMobileDevice.Afc;
using iMobileDevice.Lockdown;
using iMobileDevice.InstallationProxy;

namespace infex1rn.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private readonly DeviceService _deviceService;
        private readonly AppService _appService;
        private readonly FirmwareService _firmwareService;
        private readonly RamdiskService _ramdiskService;

        public ObservableCollection<string> Devices { get; }
        public ObservableCollection<string> Apps { get; }
        public ObservableCollection<KeyValuePair<string, string>> FileSharingApps { get; }
        public ObservableCollection<TreeViewItem> IpswTree { get; }
        public ObservableCollection<TreeViewItem> AppFileTree { get; }
        public ObservableCollection<TreeViewItem> RamdiskTree { get; }

        private string _selectedDevice;
        public string SelectedDevice
        {
            get => _selectedDevice;
            set
            {
                _selectedDevice = value;
                OnPropertyChanged();
                LoadDeviceDetails();
            }
        }

        private KeyValuePair<string, string> _selectedFileSharingApp;
        public KeyValuePair<string, string> SelectedFileSharingApp
        {
            get => _selectedFileSharingApp;
            set
            {
                _selectedFileSharingApp = value;
                OnPropertyChanged();
                LoadAppSandbox();
            }
        }

        private TreeViewItem _selectedIpswItem;
        public TreeViewItem SelectedIpswItem
        {
            get => _selectedIpswItem;
            set
            {
                _selectedIpswItem = value;
                OnPropertyChanged();
            }
        }

        private string _deviceName;
        public string DeviceName
        {
            get => _deviceName;
            set { _deviceName = value; OnPropertyChanged(); }
        }

        private string _model;
        public string Model
        {
            get => _model;
            set { _model = value; OnPropertyChanged(); }
        }

        private string _iosVersion;
        public string IOSVersion
        {
            get => _iosVersion;
            set { _iosVersion = value; OnPropertyChanged(); }
        }

        private string _serialNumber;
        public string SerialNumber
        {
            get => _serialNumber;
            set { _serialNumber = value; OnPropertyChanged(); }
        }

        private string _currentIpswPath;
        private string _ramdiskPath;
        private string _unpackedRamdiskPath;
        private bool _isRamdiskPresent;
        public bool IsRamdiskPresent
        {
            get => _isRamdiskPresent;
            set { _isRamdiskPresent = value; OnPropertyChanged(); }
        }

        private double _installationProgress;
        public double InstallationProgress
        {
            get => _installationProgress;
            set { _installationProgress = value; OnPropertyChanged(); }
        }

        private string _toolOutput;
        public string ToolOutput
        {
            get => _toolOutput;
            set { _toolOutput = value; OnPropertyChanged(); }
        }

        public ICommand ListDevicesCommand { get; }
        public ICommand EnterRecoveryCommand { get; }
        public ICommand ExitRecoveryCommand { get; }
        public ICommand LoadRamdiskCommand { get; }
        public ICommand LoadIpswCommand { get; }
        public ICommand ExtractIpswCommand { get; }
        public ICommand ExtractRamdiskCommand { get; }
        public ICommand InstallIpaCommand { get; }
        public ICommand EnterPwnDfuCommand { get; }
        public ICommand EnterPurpleModeCommand { get; }
        public ICommand BypassActivationLockCommand { get; }
        public ICommand LoadRamdiskFileCommand { get; }
        public ICommand AddFileToRamdiskCommand { get; }
        public ICommand RemoveFileFromRamdiskCommand { get; }
        public ICommand SaveRamdiskCommand { get; }
        public ICommand RamdiskExploitCommand { get; }
        public ICommand A10HelloBypassCommand { get; }
        public ICommand UnthetheredBypassCommand { get; }
        public ICommand AutoExtractRamdiskCommand { get; }

        public MainViewModel()
        {
            _deviceService = new DeviceService();
            _appService = new AppService();
            _firmwareService = new FirmwareService();
            _ramdiskService = new RamdiskService();

            Devices = new ObservableCollection<string>();
            Apps = new ObservableCollection<string>();
            FileSharingApps = new ObservableCollection<KeyValuePair<string, string>>();
            IpswTree = new ObservableCollection<TreeViewItem>();
            AppFileTree = new ObservableCollection<TreeViewItem>();
            RamdiskTree = new ObservableCollection<TreeViewItem>();

            ListDevicesCommand = new RelayCommand(ListDevices);
            EnterRecoveryCommand = new RelayCommand(EnterRecovery, CanExecuteDeviceAction);
            ExitRecoveryCommand = new RelayCommand(ExitRecovery, CanExecuteDeviceAction);
            LoadRamdiskCommand = new RelayCommand(LoadRamdisk, CanExecuteDeviceAction);
            LoadIpswCommand = new RelayCommand(LoadIpsw);
            ExtractIpswCommand = new RelayCommand(ExtractIpsw, CanExtractIpsw);
            ExtractRamdiskCommand = new RelayCommand(ExtractRamdisk, CanExtractRamdisk);
            InstallIpaCommand = new RelayCommand(InstallIpa, CanExecuteDeviceAction);
            EnterPwnDfuCommand = new RelayCommand(EnterPwnDfu);
            EnterPurpleModeCommand = new RelayCommand(EnterPurpleMode);
            LoadRamdiskFileCommand = new RelayCommand(LoadRamdiskFile);
            AddFileToRamdiskCommand = new RelayCommand(AddFileToRamdisk);
            RemoveFileFromRamdiskCommand = new RelayCommand(RemoveFileFromRamdisk);
            SaveRamdiskCommand = new RelayCommand(SaveRamdisk);
            BypassActivationLockCommand = new RelayCommand(BypassActivationLock);
            RamdiskExploitCommand = new RelayCommand(RamdiskExploit);
            A10HelloBypassCommand = new RelayCommand(A10HelloBypass);
            UnthetheredBypassCommand = new RelayCommand(UnthetheredBypass);
            AutoExtractRamdiskCommand = new RelayCommand(AutoExtractRamdisk);
        }

        private async void AutoExtractRamdisk()
        {
            ToolOutput = "";
            try
            {
                var openFileDialog = new OpenFileDialog 
                { 
                    Filter = "IPSW files (*.ipsw)|*.ipsw|All files (*.*)|*.*",
                    Title = "Select IPSW to Extract Ramdisk From"
                };
                
                if (openFileDialog.ShowDialog() == true)
                {
                    ToolOutput = "=== Auto Ramdisk Extraction ===\n\n";
                    ToolOutput += $"IPSW: {openFileDialog.FileName}\n\n";
                    ToolOutput += "Extracting ramdisk and boot files...\n";
                    ToolOutput += "This may take a few minutes...\n\n";

                    var result = await _ramdiskService.AutoExtractFromIpsw(
                        openFileDialog.FileName,
                        (message) =>
                        {
                            Application.Current.Dispatcher.Invoke(() =>
                            {
                                ToolOutput += message + "\n";
                            });
                        }
                    );

                    ToolOutput += "\n=== Extraction Complete ===\n\n";
                    
                    if (result.Success)
                    {
                        ToolOutput += "[+] Ramdisk extracted successfully!\n";
                        ToolOutput += $"[+] Ramdisk path: {result.RamdiskPath}\n";
                        ToolOutput += $"[+] Boot files extracted: {result.BootFilesExtracted.Count}\n";
                        
                        foreach (var file in result.BootFilesExtracted)
                        {
                            ToolOutput += $"    - {file}\n";
                        }
                        
                        if (result.PatchScriptCreated)
                        {
                            ToolOutput += "\n[+] Setup.app patch script created!\n";
                            ToolOutput += "[*] The patch script will remove Setup.app for iCloud bypass\n";
                        }
                        
                        ToolOutput += "\n[*] Next steps:\n";
                        ToolOutput += "    1. Put device in DFU mode\n";
                        ToolOutput += "    2. Click 'Untethered Bypass' button\n";
                        ToolOutput += "    3. Or use 'Ramdisk Exploit' with the extracted ramdisk\n";
                    }
                    else
                    {
                        ToolOutput += $"[!] Extraction failed: {result.Error}\n";
                    }
                }
            }
            catch (Exception ex)
            {
                ToolOutput += $"\n[!] Error: {ex.Message}\n";
            }
        }

        private async void BypassActivationLock()
        {
            ToolOutput = "";
            try
            {
                string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                string bypassPath = Path.Combine(baseDirectory, "tools", "bypass.bat");
                await _deviceService.RunExternalTool(bypassPath, "", (output) =>
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        ToolOutput += output + Environment.NewLine;
                    });
                });
            }
            catch (Exception ex)
            {
                ToolOutput = $"Error: {ex.Message}";
            }
        }

        private async void RamdiskExploit()
        {
            ToolOutput = "";
            try
            {
                var openFileDialog = new OpenFileDialog { Filter = "Ramdisk files (*.dmg)|*.dmg|All files (*.*)|*.*" };
                if (openFileDialog.ShowDialog() == true)
                {
                    string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                    string ramdiskBatPath = Path.Combine(baseDirectory, "tools", "ramdisk.bat");
                    ToolOutput = "Starting ramdisk exploit using checkm8...\n";
                    await _deviceService.RunExternalTool(ramdiskBatPath, $"\"{openFileDialog.FileName}\"", (output) =>
                    {
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            ToolOutput += output + Environment.NewLine;
                        });
                    });
                }
            }
            catch (Exception ex)
            {
                ToolOutput = $"Error: {ex.Message}";
            }
        }

        private async void A10HelloBypass()
        {
            ToolOutput = "";
            try
            {
                string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                string a10BypassPath = Path.Combine(baseDirectory, "tools", "a10_hello_bypass.bat");
                ToolOutput = "Starting A10 Hello Bypass with Signals...\n";
                ToolOutput += "Make sure your iPhone 7/7+ is in DFU mode.\n\n";
                await _deviceService.RunExternalTool(a10BypassPath, "", (output) =>
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        ToolOutput += output + Environment.NewLine;
                    });
                });
            }
            catch (Exception ex)
            {
                ToolOutput = $"Error: {ex.Message}";
            }
        }

        private async void UnthetheredBypass()
        {
            ToolOutput = "";
            try
            {
                string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                string untetheredPath = Path.Combine(baseDirectory, "tools", "untethered_bypass.bat");
                ToolOutput = "Starting Untethered iCloud Bypass...\n";
                ToolOutput += "Supported: A7-A11 devices (iPhone 5s - iPhone X)\n";
                ToolOutput += "Make sure your device is in DFU mode.\n\n";
                await _deviceService.RunExternalTool(untetheredPath, "hello", (output) =>
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        ToolOutput += output + Environment.NewLine;
                    });
                });
            }
            catch (Exception ex)
            {
                ToolOutput = $"Error: {ex.Message}";
            }
        }

        private async void LoadRamdiskFile()
        {
            var openFileDialog = new OpenFileDialog { Filter = "Ramdisk files (*.dmg)|*.dmg|All files (*.*)|*.*" };
            if (openFileDialog.ShowDialog() == true)
            {
                _unpackedRamdiskPath = await _ramdiskService.UnpackRamdisk(openFileDialog.FileName);
                PopulateRamdiskTree();
            }
        }

        private void PopulateRamdiskTree()
        {
            RamdiskTree.Clear();
            var root = new TreeViewItem { Header = Path.GetFileName(_unpackedRamdiskPath), Tag = _unpackedRamdiskPath };
            RamdiskTree.Add(root);
            PopulateDirectory(root, _unpackedRamdiskPath);
        }

        private void PopulateDirectory(TreeViewItem parent, string path)
        {
            foreach (var directory in Directory.GetDirectories(path))
            {
                var dirNode = new TreeViewItem { Header = Path.GetFileName(directory), Tag = directory };
                parent.Items.Add(dirNode);
                PopulateDirectory(dirNode, directory);
            }
            foreach (var file in Directory.GetFiles(path))
            {
                parent.Items.Add(new TreeViewItem { Header = Path.GetFileName(file), Tag = file });
            }
        }

        private void AddFileToRamdisk()
        {
            var openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                File.Copy(openFileDialog.FileName, Path.Combine(_unpackedRamdiskPath, Path.GetFileName(openFileDialog.FileName)));
                PopulateRamdiskTree();
            }
        }

        private void RemoveFileFromRamdisk()
        {
            // This is a placeholder. A real implementation would require selecting a file from the tree.
        }

        private async void SaveRamdisk()
        {
            var saveFileDialog = new SaveFileDialog { Filter = "Ramdisk files (*.dmg)|*.dmg|All files (*.*)|*.*" };
            if (saveFileDialog.ShowDialog() == true)
            {
                await _ramdiskService.RepackRamdisk(_unpackedRamdiskPath, saveFileDialog.FileName);
                MessageBox.Show("Ramdisk saved successfully.");
            }
        }

        private async void EnterPwnDfu()
        {
            ToolOutput = "";
            try
            {
                string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                string gasterPath = Path.Combine(baseDirectory, "tools", "gaster.bat");
                await _deviceService.RunExternalTool(gasterPath, "pwn", (output) =>
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        ToolOutput += output + Environment.NewLine;
                    });
                });
            }
            catch (Exception ex)
            {
                ToolOutput = $"Error: {ex.Message}";
            }
        }

        private async void EnterPurpleMode()
        {
            ToolOutput = "";
            try
            {
                string baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
                string purplePath = Path.Combine(baseDirectory, "tools", "purple.bat");
                await _deviceService.RunExternalTool(purplePath, "", (output) =>
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        ToolOutput += output + Environment.NewLine;
                    });
                });
            }
            catch (Exception ex)
            {
                ToolOutput = $"Error: {ex.Message}";
            }
        }

        private void ListDevices()
        {
            Devices.Clear();
            var devices = _deviceService.GetDeviceList();
            foreach (var device in devices)
            {
                Devices.Add(device);
            }
        }

        private void LoadDeviceDetails()
        {
            if (SelectedDevice == null) return;

            var info = _deviceService.GetDeviceInfo(SelectedDevice);
            DeviceName = info["DeviceName"];
            Model = info["Model"];
            IOSVersion = info["iOSVersion"];
            SerialNumber = info["SerialNumber"];

            Apps.Clear();
            var apps = _appService.GetInstalledApps(SelectedDevice);
            foreach (var app in apps)
            {
                Apps.Add(app);
            }

            FileSharingApps.Clear();
            var idevice = LibiMobileDevice.Instance.iDevice;
            var lockdown = LibiMobileDevice.Instance.Lockdown;
            var installationProxy = LibiMobileDevice.Instance.InstallationProxy;
            var plist = LibiMobileDevice.Instance.Plist;

            iDeviceHandle deviceHandle;
            idevice.idevice_new(out deviceHandle, SelectedDevice).ThrowOnError();
            using (deviceHandle)
            {
                LockdownClientHandle lockdownHandle;
                lockdown.lockdownd_client_new_with_handshake(deviceHandle, out lockdownHandle, "infex1rn").ThrowOnError();
                using (lockdownHandle)
                {
                    InstallationProxyClientHandle client;
                    installationProxy.instproxy_client_start_service(deviceHandle, out client, "infex1rn").ThrowOnError();
                    using (client)
                    {
                        PlistHandle options = plist.plist_new_dict();
                        using(options)
                        {
                            plist.plist_dict_set_item(options, "ReturnAttributes", plist.plist_new_array());
                            PlistHandle returnAttributes = plist.plist_dict_get_item(options, "ReturnAttributes");
                            plist.plist_array_append_item(returnAttributes, plist.plist_new_string("CFBundleDisplayName"));
                            plist.plist_array_append_item(returnAttributes, plist.plist_new_string("CFBundleIdentifier"));
                            plist.plist_array_append_item(returnAttributes, plist.plist_new_string("UIFileSharingEnabled"));

                            PlistHandle appNodes;
                            installationProxy.instproxy_browse(client, options, out appNodes).ThrowOnError();
                            using (appNodes)
                            {
                                for (uint i = 0; i < plist.plist_array_get_size(appNodes); i++)
                                {
                                    PlistHandle appNode = plist.plist_array_get_item(appNodes, i);
                                    PlistHandle fileSharingNode = plist.plist_dict_get_item(appNode, "UIFileSharingEnabled");
                                    if (!fileSharingNode.IsInvalid)
                                    {
                                        bool fileSharingEnabled = false;
                                        char boolVal = '\0';
                                        plist.plist_get_bool_val(fileSharingNode, ref boolVal);
                                        fileSharingEnabled = boolVal != '\0';
                                        if (fileSharingEnabled)
                                        {
                                            PlistHandle appNameNode = plist.plist_dict_get_item(appNode, "CFBundleDisplayName");
                                            PlistHandle bundleIdNode = plist.plist_dict_get_item(appNode, "CFBundleIdentifier");
                                            if (!appNameNode.IsInvalid && !bundleIdNode.IsInvalid)
                                            {
                                                string appName, bundleId;
                                                plist.plist_get_string_val(appNameNode, out appName);
                                                plist.plist_get_string_val(bundleIdNode, out bundleId);
                                                FileSharingApps.Add(new KeyValuePair<string, string>(appName, bundleId));
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private void LoadAppSandbox()
        {
            if (SelectedFileSharingApp.Key == null) return;

            AppFileTree.Clear();
            var root = new TreeViewItem { Header = "/" };
            AppFileTree.Add(root);

            var idevice = LibiMobileDevice.Instance.iDevice;
            var houseArrest = LibiMobileDevice.Instance.HouseArrest;
            var afc = LibiMobileDevice.Instance.Afc;

            iDeviceHandle deviceHandle;
            idevice.idevice_new(out deviceHandle, SelectedDevice).ThrowOnError();
            using (deviceHandle)
            {
                HouseArrestClientHandle houseArrestHandle;
                houseArrest.house_arrest_client_start_service(deviceHandle, out houseArrestHandle, "infex1rn").ThrowOnError();
                using (houseArrestHandle)
                {
                    AfcClientHandle afcHandle;
                    houseArrest.afc_client_new_from_house_arrest_client(houseArrestHandle, out afcHandle).ThrowOnError();
                    using (afcHandle)
                    {
                        houseArrest.house_arrest_send_command(houseArrestHandle, "VendContainer", SelectedFileSharingApp.Value).ThrowOnError();
                        PopulateAppFileTree(root, "/", afcHandle);
                    }
                }
            }
        }

        private void PopulateAppFileTree(TreeViewItem parent, string path, AfcClientHandle afcHandle)
        {
            var afc = LibiMobileDevice.Instance.Afc;
            ReadOnlyCollection<string> entries;
            afc.afc_read_directory(afcHandle, path, out entries).ThrowOnError();

            foreach (string entry in entries)
            {
                if (entry == "." || entry == "..")
                {
                    continue;
                }

                string fullPath = Path.Combine(path, entry).Replace('\\', '/');
                var node = new TreeViewItem { Header = entry };
                parent.Items.Add(node);

                ReadOnlyCollection<string> info;
                afc.afc_get_file_info(afcHandle, fullPath, out info).ThrowOnError();

                // Parse the string array into a dictionary
                var infoDict = new Dictionary<string, string>();
                if (info.Count % 2 != 0)
                {
                    // Log or handle the case where info.Count is not even
                    // For now, we ignore the last element if odd
                }
                for (int i = 0; i < info.Count - 1; i += 2)
                {
                    infoDict[info[i]] = info[i + 1];
                }

                if (infoDict.ContainsKey("st_ifmt") && infoDict["st_ifmt"] == "S_IFDIR")
                {
                    PopulateAppFileTree(node, fullPath, afcHandle);
                }
            }
        }

        private void EnterRecovery()
        {
            _deviceService.EnterRecovery(SelectedDevice);
            MessageBox.Show("Device is entering recovery mode.");
        }

        private void ExitRecovery()
        {
            _deviceService.ExitRecovery(SelectedDevice);
            MessageBox.Show("Device is exiting recovery mode.");
        }

        private void LoadRamdisk()
        {
            var openFileDialog = new OpenFileDialog { Filter = "Ramdisk files (*.dmg)|*.dmg|All files (*.*)|*.*" };
            if (openFileDialog.ShowDialog() == true)
            {
                _deviceService.LoadRamdisk(SelectedDevice, openFileDialog.FileName);
                MessageBox.Show("Ramdisk loaded successfully.");
            }
        }

        private void LoadIpsw()
        {
            var openFileDialog = new OpenFileDialog { Filter = "IPSW files (*.ipsw)|*.ipsw|All files (*.*)|*.*" };
            if (openFileDialog.ShowDialog() == true)
            {
                LoadIpswFile(openFileDialog.FileName);
            }
        }

        /// <summary>
        /// Loads an IPSW file from a given path. Used by both file dialog and drag-drop.
        /// </summary>
        public void LoadIpswFile(string ipswPath)
        {
            try
            {
                if (string.IsNullOrEmpty(ipswPath) || !File.Exists(ipswPath))
                {
                    MessageBox.Show("Invalid IPSW file path.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (!ipswPath.EndsWith(".ipsw", StringComparison.OrdinalIgnoreCase))
                {
                    MessageBox.Show("Please select a valid IPSW file.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                IsRamdiskPresent = false;
                _ramdiskPath = null;
                _currentIpswPath = ipswPath;

                IpswTree.Clear();
                var root = new TreeViewItem { Header = Path.GetFileName(_currentIpswPath), Tag = "" };
                IpswTree.Add(root);

                var entries = _firmwareService.GetIpswEntries(_currentIpswPath);
                var directories = new Dictionary<string, TreeViewItem> { { "", root } };

                foreach (var entry in entries)
                {
                    string[] pathParts = entry.Split('/');
                    string currentPath = "";
                    for (int i = 0; i < pathParts.Length - 1; i++)
                    {
                        string parentPath = currentPath;
                        currentPath = Path.Combine(currentPath, pathParts[i]).Replace('\\', '/');
                        if (!directories.ContainsKey(currentPath))
                        {
                            var newNode = new TreeViewItem { Header = pathParts[i], Tag = currentPath };
                            directories[parentPath].Items.Add(newNode);
                            directories[currentPath] = newNode;
                        }
                    }

                    if (!string.IsNullOrEmpty(Path.GetFileName(entry)))
                    {
                        var fileNode = new TreeViewItem { Header = Path.GetFileName(entry), Tag = entry };
                        directories[currentPath].Items.Add(fileNode);
                    }
                }

                // Try to find ramdisk in BuildManifest.plist
                try
                {
                    using (ZipArchive archive = ZipFile.OpenRead(_currentIpswPath))
                    {
                        var manifestEntry = archive.GetEntry("BuildManifest.plist");
                        if (manifestEntry != null)
                        {
                            using (Stream stream = manifestEntry.Open())
                            {
                                using (var reader = new StreamReader(stream))
                                {
                                    string plistXml = reader.ReadToEnd();
                                    var plist = LibiMobileDevice.Instance.Plist;
                                    PlistHandle parsedPlist;
                                    plist.plist_from_xml(plistXml, (uint)plistXml.Length, out parsedPlist);
                                    using (parsedPlist)
                                    {
                                        var buildIdentities = plist.plist_dict_get_item(parsedPlist, "BuildIdentities");
                                        if (!buildIdentities.IsInvalid)
                                        {
                                            var firstIdentity = plist.plist_array_get_item(buildIdentities, 0);
                                            if (!firstIdentity.IsInvalid)
                                            {
                                                var manifest = plist.plist_dict_get_item(firstIdentity, "Manifest");
                                                if (!manifest.IsInvalid)
                                                {
                                                    var restoreRamdisk = plist.plist_dict_get_item(manifest, "RestoreRamdisk");
                                                    if (!restoreRamdisk.IsInvalid)
                                                    {
                                                        var info = plist.plist_dict_get_item(restoreRamdisk, "Info");
                                                        if (!info.IsInvalid)
                                                        {
                                                            var pathNode = plist.plist_dict_get_item(info, "Path");
                                                            if (!pathNode.IsInvalid)
                                                            {
                                                                string path;
                                                                plist.plist_get_string_val(pathNode, out path);
                                                                _ramdiskPath = path;
                                                                IsRamdiskPresent = true;
                                                            }
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    // Ramdisk extraction from manifest failed, but IPSW is still loaded
                    MessageBox.Show($"Note: Could not extract ramdisk path from BuildManifest: {ex.Message}\nIPSW loaded successfully.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                }

                MessageBox.Show($"IPSW loaded successfully!\n{Path.GetFileName(_currentIpswPath)}", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading IPSW: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                IpswTree.Clear();
                _currentIpswPath = null;
                IsRamdiskPresent = false;
                _ramdiskPath = null;
            }
        }

        private void ExtractIpsw()
        {
            if (SelectedIpswItem == null) return;
            string entryPath = (string)SelectedIpswItem.Tag;
            _firmwareService.ExtractIpswEntry(_currentIpswPath, entryPath, "extracted_firmware");
            MessageBox.Show("Extraction complete.");
        }

        private void ExtractRamdisk()
        {
            try
            {
                if (string.IsNullOrEmpty(_currentIpswPath))
                {
                    MessageBox.Show("No IPSW file loaded.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                if (string.IsNullOrEmpty(_ramdiskPath))
                {
                    MessageBox.Show("No ramdisk found in this IPSW.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                // Create extracted_firmware directory if it doesn't exist
                string extractPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "extracted_firmware");
                if (!Directory.Exists(extractPath))
                {
                    Directory.CreateDirectory(extractPath);
                }

                _firmwareService.ExtractIpswEntry(_currentIpswPath, _ramdiskPath, extractPath);
                
                string extractedFile = Path.Combine(extractPath, Path.GetFileName(_ramdiskPath));
                MessageBox.Show($"Ramdisk extracted successfully!\n\nLocation: {extractedFile}", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                
                // Open the folder
                System.Diagnostics.Process.Start("explorer.exe", extractPath);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error extracting ramdisk: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void InstallIpa()
        {
            var openFileDialog = new OpenFileDialog { Filter = "IPA files (*.ipa)|*.ipa|All files (*.*)|*.*" };
            if (openFileDialog.ShowDialog() == true)
            {
                InstallationProgress = 0;
                _appService.InstallIpa(SelectedDevice, openFileDialog.FileName, InstallationStatusCallback);
            }
        }

        private void InstallationStatusCallback(string operation, PlistHandle status)
        {
            if (!status.IsInvalid)
            {
                var plist = LibiMobileDevice.Instance.Plist;
                PlistHandle percentNode = plist.plist_dict_get_item(status, "PercentComplete");
                if (!percentNode.IsInvalid)
                {
                    ulong percent = 0;
                    plist.plist_get_uint_val(percentNode, ref percent);
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        InstallationProgress = percent;
                    });
                }

                PlistHandle statusNode = plist.plist_dict_get_item(status, "Status");
                string statusString;
                plist.plist_get_string_val(statusNode, out statusString);

                if (statusString == "Complete")
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        MessageBox.Show("Installation complete.");
                        InstallationProgress = 0;
                        LoadDeviceDetails(); // Refresh app list
                    });
                }

                PlistHandle errorNode = plist.plist_dict_get_item(status, "Error");
                if (!errorNode.IsInvalid)
                {
                    string error;
                    plist.plist_get_string_val(errorNode, out error);
                    MessageBox.Show($"Installation failed: {error}");
                }
            }
        }

        private bool CanExecuteDeviceAction()
        {
            return SelectedDevice != null;
        }

        private bool CanExtractIpsw()
        {
            return SelectedIpswItem != null && !string.IsNullOrEmpty(_currentIpswPath);
        }

        private bool CanExtractRamdisk()
        {
            return IsRamdiskPresent;
        }
    }

    public class RelayCommand : ICommand
    {
        private readonly Action _execute;
        private readonly Func<bool> _canExecute;

        public event EventHandler CanExecuteChanged
        {
            add => CommandManager.RequerySuggested += value;
            remove => CommandManager.RequerySuggested -= value;
        }

        public RelayCommand(Action execute, Func<bool> canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        public bool CanExecute(object parameter)
        {
            return _canExecute == null || _canExecute();
        }

        public void Execute(object parameter)
        {
            _execute();
        }
    }
}
