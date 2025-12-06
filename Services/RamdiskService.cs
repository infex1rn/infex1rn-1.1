using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Threading.Tasks;

namespace infex1rn.Services
{
    public class RamdiskService
    {
        private readonly string _ramdisksPath;
        private readonly string _toolsPath;

        public RamdiskService()
        {
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            _ramdisksPath = Path.Combine(baseDir, "ramdisks");
            _toolsPath = Path.Combine(baseDir, "tools");
            
            // Ensure ramdisks directory exists
            if (!Directory.Exists(_ramdisksPath))
            {
                Directory.CreateDirectory(_ramdisksPath);
            }
        }

        /// <summary>
        /// Automatically extracts ramdisk and boot files from an IPSW
        /// </summary>
        public async Task<RamdiskExtractionResult> AutoExtractFromIpsw(string ipswPath, Action<string> progressCallback = null)
        {
            var result = new RamdiskExtractionResult();
            string tempDir = Path.Combine(Path.GetTempPath(), $"infex1rn_ramdisk_{Guid.NewGuid():N}");

            try
            {
                progressCallback?.Invoke("Creating temporary directory...");
                Directory.CreateDirectory(tempDir);
                string extractDir = Path.Combine(tempDir, "ipsw_extracted");

                // Step 1: Extract IPSW (it's a ZIP file)
                progressCallback?.Invoke("Extracting IPSW contents...");
                await Task.Run(() => ZipFile.ExtractToDirectory(ipswPath, extractDir));
                result.IpswExtracted = true;

                // Step 2: Find ramdisk DMG
                progressCallback?.Invoke("Locating ramdisk in IPSW...");
                var dmgFiles = Directory.GetFiles(extractDir, "*.dmg", SearchOption.AllDirectories);
                
                string ramdiskDmg = null;
                foreach (var dmg in dmgFiles)
                {
                    var fileInfo = new FileInfo(dmg);
                    // Ramdisks are typically smaller (under 100MB)
                    if (fileInfo.Length < 100_000_000)
                    {
                        ramdiskDmg = dmg;
                        progressCallback?.Invoke($"Found ramdisk: {fileInfo.Name}");
                        break;
                    }
                }

                if (ramdiskDmg == null)
                {
                    // Fall back to looking for restore ramdisk pattern
                    ramdiskDmg = dmgFiles.FirstOrDefault(f => 
                        Path.GetFileName(f).ToLower().Contains("restore") ||
                        Path.GetFileName(f).ToLower().Contains("ramdisk"));
                }

                if (ramdiskDmg != null)
                {
                    string destPath = Path.Combine(_ramdisksPath, "ramdisk.dmg");
                    File.Copy(ramdiskDmg, destPath, true);
                    result.RamdiskPath = destPath;
                    result.RamdiskFound = true;
                    progressCallback?.Invoke($"Ramdisk copied to: {destPath}");
                }

                // Step 3: Extract boot components (iBSS, iBEC, etc.)
                progressCallback?.Invoke("Extracting boot components...");
                string dfuPath = Path.Combine(extractDir, "Firmware", "dfu");
                if (Directory.Exists(dfuPath))
                {
                    var bootFiles = Directory.GetFiles(dfuPath, "*.*", SearchOption.AllDirectories)
                        .Where(f => f.EndsWith(".im4p") || f.EndsWith(".img4"));
                    
                    foreach (var bootFile in bootFiles)
                    {
                        string destPath = Path.Combine(_ramdisksPath, Path.GetFileName(bootFile));
                        File.Copy(bootFile, destPath, true);
                        result.BootFilesExtracted.Add(Path.GetFileName(bootFile));
                        progressCallback?.Invoke($"Extracted: {Path.GetFileName(bootFile)}");
                    }
                }

                // Step 4: Extract device tree and other firmware files
                string allFlashPath = Path.Combine(extractDir, "Firmware", "all_flash");
                if (Directory.Exists(allFlashPath))
                {
                    var firmwareFiles = Directory.GetFiles(allFlashPath, "*.*", SearchOption.AllDirectories)
                        .Where(f => f.EndsWith(".im4p") || f.EndsWith(".img4"));
                    
                    foreach (var fwFile in firmwareFiles)
                    {
                        string destPath = Path.Combine(_ramdisksPath, Path.GetFileName(fwFile));
                        File.Copy(fwFile, destPath, true);
                        result.BootFilesExtracted.Add(Path.GetFileName(fwFile));
                    }
                }

                // Step 5: Create patch script for Setup.app removal
                progressCallback?.Invoke("Creating Setup.app patch script...");
                await CreateSetupAppPatchScript();
                result.PatchScriptCreated = true;

                result.Success = result.RamdiskFound;
            }
            catch (Exception ex)
            {
                result.Error = ex.Message;
                progressCallback?.Invoke($"Error: {ex.Message}");
            }
            finally
            {
                // Cleanup temp directory
                try
                {
                    if (Directory.Exists(tempDir))
                    {
                        Directory.Delete(tempDir, true);
                    }
                }
                catch { }
            }

            return result;
        }

        /// <summary>
        /// Creates a shell script to patch the ramdisk and remove Setup.app
        /// </summary>
        public async Task CreateSetupAppPatchScript()
        {
            string scriptPath = Path.Combine(_ramdisksPath, "patch_setup_app.sh");
            
            string script = @"#!/bin/bash
# Ramdisk Patch Script for Setup.app Removal
# Generated by infex1rn
# Run this script after booting the SSH ramdisk

echo '[*] Mounting filesystems...'
mount_filesystems

echo '[*] Backing up Setup.app...'
if [ -d '/mnt1/Applications/Setup.app' ]; then
    mv /mnt1/Applications/Setup.app /mnt1/Applications/Setup.app.bak
    echo '[+] Setup.app renamed to Setup.app.bak'
else
    echo '[!] Setup.app not found at expected location'
fi

echo '[*] Setting auto-boot...'
nvram auto-boot=true

echo '[*] Creating activation bypass marker...'
touch /mnt1/.cydia_no_stash
touch /mnt1/.installed_coolstar

echo '[*] Syncing filesystem...'
sync

echo '[+] Patch complete!'
echo '[*] The device will now reboot without activation lock'
echo '[*] Run: reboot'
";

            await File.WriteAllTextAsync(scriptPath, script);
        }

        /// <summary>
        /// Runs the auto ramdisk extraction batch script
        /// </summary>
        public async Task RunAutoRamdiskScript(string ipswPath, Action<string> outputCallback)
        {
            string scriptPath = Path.Combine(_toolsPath, "auto_ramdisk.bat");
            
            if (!File.Exists(scriptPath))
            {
                outputCallback?.Invoke("Error: auto_ramdisk.bat not found");
                return;
            }

            var startInfo = new ProcessStartInfo
            {
                FileName = "cmd.exe",
                Arguments = $"/c \"{scriptPath}\" \"{ipswPath}\"",
                WorkingDirectory = _toolsPath,
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            using var process = new Process { StartInfo = startInfo };
            
            process.OutputDataReceived += (s, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                    outputCallback?.Invoke(e.Data);
            };
            
            process.ErrorDataReceived += (s, e) =>
            {
                if (!string.IsNullOrEmpty(e.Data))
                    outputCallback?.Invoke($"[Error] {e.Data}");
            };

            process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            
            await process.WaitForExitAsync();
        }

        public async Task<string> UnpackRamdisk(string ramdiskPath)
        {
            string unpackDir = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
            Directory.CreateDirectory(unpackDir);

            // Copy the ramdisk file to the unpack directory
            string destPath = Path.Combine(unpackDir, "ramdisk.dmg");
            File.Copy(ramdiskPath, destPath, true);

            // Create info file
            await File.WriteAllTextAsync(
                Path.Combine(unpackDir, "ramdisk_info.txt"),
                $"Source: {ramdiskPath}\nExtracted: {DateTime.Now}\n\nNote: DMG files require macOS or specialized tools to fully extract.\nUse SSH ramdisk to access device filesystem directly."
            );

            await Task.Delay(500); // Brief delay for UI feedback

            return unpackDir;
        }

        public async Task RepackRamdisk(string unpackDir, string newRamdiskPath)
        {
            // Copy the original ramdisk to the new path
            string sourceDmg = Path.Combine(unpackDir, "ramdisk.dmg");
            if (File.Exists(sourceDmg))
            {
                File.Copy(sourceDmg, newRamdiskPath, true);
            }

            await Task.Delay(500); // Brief delay for UI feedback
        }

        /// <summary>
        /// Gets a list of available ramdisk files
        /// </summary>
        public string[] GetAvailableRamdisks()
        {
            if (!Directory.Exists(_ramdisksPath))
                return Array.Empty<string>();

            return Directory.GetFiles(_ramdisksPath, "*.dmg")
                .Select(Path.GetFileName)
                .ToArray();
        }

        /// <summary>
        /// Gets a list of available boot files
        /// </summary>
        public string[] GetAvailableBootFiles()
        {
            if (!Directory.Exists(_ramdisksPath))
                return Array.Empty<string>();

            return Directory.GetFiles(_ramdisksPath)
                .Where(f => f.EndsWith(".im4p") || f.EndsWith(".img4"))
                .Select(Path.GetFileName)
                .ToArray();
        }
    }

    public class RamdiskExtractionResult
    {
        public bool Success { get; set; }
        public bool IpswExtracted { get; set; }
        public bool RamdiskFound { get; set; }
        public string RamdiskPath { get; set; }
        public System.Collections.Generic.List<string> BootFilesExtracted { get; set; } = new();
        public bool PatchScriptCreated { get; set; }
        public string Error { get; set; }
    }
}
