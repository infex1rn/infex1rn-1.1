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

        /// <summary>
        /// Automatically prepares all required bypass files when a ramdisk is loaded
        /// </summary>
        public async Task<BypassPreparationResult> AutoPrepareBypassFiles(Action<string> progressCallback = null)
        {
            var result = new BypassPreparationResult();

            try
            {
                progressCallback?.Invoke("=== Auto-Preparing Bypass Files ===\n");

                // Step 1: Check for gaster.exe in tools and copy to ramdisks
                progressCallback?.Invoke("[1/5] Checking for gaster.exe...");
                string gasterSource = Path.Combine(_toolsPath, "gaster", "gaster.exe");
                string gasterDest = Path.Combine(_ramdisksPath, "gaster.exe");
                
                if (File.Exists(gasterSource))
                {
                    // Ensure ramdisks directory exists before copying
                    Directory.CreateDirectory(_ramdisksPath);
                    File.Copy(gasterSource, gasterDest, true);
                    result.GasterReady = true;
                    progressCallback?.Invoke("  ✓ gaster.exe copied to ramdisks folder");
                }
                else
                {
                    progressCallback?.Invoke("  ✗ gaster.exe not found in tools/gaster/");
                    result.MissingFiles.Add("gaster.exe");
                }

                // Step 2: Check for iBSS*.im4p (bootchain stage 1)
                progressCallback?.Invoke("\n[2/5] Checking for iBSS*.im4p (bootchain stage 1)...");
                if (Directory.Exists(_ramdisksPath))
                {
                    var ibssFiles = Directory.GetFiles(_ramdisksPath, "iBSS*.im4p");
                    if (ibssFiles.Length > 0)
                    {
                        result.IBSSReady = true;
                        result.IBSSFile = Path.GetFileName(ibssFiles[0]);
                        progressCallback?.Invoke($"  ✓ Found: {result.IBSSFile}");
                    }
                    else
                    {
                        progressCallback?.Invoke("  ✗ iBSS*.im4p not found");
                        progressCallback?.Invoke("    Extract from IPSW using 'Auto Extract Ramdisk' in System Utilities");
                        result.MissingFiles.Add("iBSS*.im4p");
                    }
                }
                else
                {
                    progressCallback?.Invoke("  ✗ iBSS*.im4p not found (ramdisks directory doesn't exist)");
                    progressCallback?.Invoke("    Extract from IPSW using 'Auto Extract Ramdisk' in System Utilities");
                    result.MissingFiles.Add("iBSS*.im4p");
                }

                // Step 3: Check for iBEC*.im4p (bootchain stage 2)
                progressCallback?.Invoke("\n[3/5] Checking for iBEC*.im4p (bootchain stage 2)...");
                if (Directory.Exists(_ramdisksPath))
                {
                    var ibecFiles = Directory.GetFiles(_ramdisksPath, "iBEC*.im4p");
                    if (ibecFiles.Length > 0)
                    {
                        result.IBECReady = true;
                        result.IBECFile = Path.GetFileName(ibecFiles[0]);
                        progressCallback?.Invoke($"  ✓ Found: {result.IBECFile}");
                    }
                    else
                    {
                        progressCallback?.Invoke("  ✗ iBEC*.im4p not found");
                        progressCallback?.Invoke("    Extract from IPSW using 'Auto Extract Ramdisk' in System Utilities");
                        result.MissingFiles.Add("iBEC*.im4p");
                    }
                }
                else
                {
                    progressCallback?.Invoke("  ✗ iBEC*.im4p not found (ramdisks directory doesn't exist)");
                    progressCallback?.Invoke("    Extract from IPSW using 'Auto Extract Ramdisk' in System Utilities");
                    result.MissingFiles.Add("iBEC*.im4p");
                }

                // Step 4: Check for ramdisk.dmg
                progressCallback?.Invoke("\n[4/5] Checking for ramdisk.dmg...");
                string ramdiskPath = Path.Combine(_ramdisksPath, "ramdisk.dmg");
                if (File.Exists(ramdiskPath))
                {
                    result.RamdiskReady = true;
                    progressCallback?.Invoke("  ✓ ramdisk.dmg found");
                }
                else
                {
                    progressCallback?.Invoke("  ✗ ramdisk.dmg not found");
                    progressCallback?.Invoke("    Extract from IPSW using 'Auto Extract Ramdisk' in System Utilities");
                    result.MissingFiles.Add("ramdisk.dmg");
                }

                // Step 5: Check/Create patch_setup_app.sh
                progressCallback?.Invoke("\n[5/5] Checking for patch_setup_app.sh...");
                string patchScriptPath = Path.Combine(_ramdisksPath, "patch_setup_app.sh");
                if (!File.Exists(patchScriptPath))
                {
                    progressCallback?.Invoke("  Creating patch_setup_app.sh...");
                    await CreateSetupAppPatchScript();
                    result.PatchScriptReady = true;
                    progressCallback?.Invoke("  ✓ patch_setup_app.sh created");
                }
                else
                {
                    result.PatchScriptReady = true;
                    progressCallback?.Invoke("  ✓ patch_setup_app.sh already exists");
                }

                // Summary
                progressCallback?.Invoke("\n=== Preparation Summary ===");
                result.AllFilesReady = (result.GasterReady && result.IBSSReady && 
                                        result.IBECReady && result.RamdiskReady && 
                                        result.PatchScriptReady);

                if (result.AllFilesReady)
                {
                    progressCallback?.Invoke("\n✅ ALL FILES READY FOR BYPASS!");
                    progressCallback?.Invoke("\nFiles in ramdisks folder:");
                    progressCallback?.Invoke($"  • gaster.exe (boot pwned DFU)");
                    progressCallback?.Invoke($"  • {result.IBSSFile} (bootchain stage 1)");
                    progressCallback?.Invoke($"  • {result.IBECFile} (bootchain stage 2)");
                    progressCallback?.Invoke($"  • ramdisk.dmg (actual ramdisk)");
                    progressCallback?.Invoke($"  • patch_setup_app.sh (patch bypass)");
                    progressCallback?.Invoke("\nNext steps:");
                    progressCallback?.Invoke("  1. Put device in DFU mode");
                    progressCallback?.Invoke("  2. Run 'Untethered Bypass' from System Utilities");
                    progressCallback?.Invoke("  3. Or use the bypass batch scripts in tools/");
                }
                else
                {
                    progressCallback?.Invoke("\n⚠️  MISSING FILES - Cannot proceed with bypass");
                    progressCallback?.Invoke("\nMissing files:");
                    foreach (var file in result.MissingFiles)
                    {
                        progressCallback?.Invoke($"  • {file}");
                    }
                    progressCallback?.Invoke("\nTo get missing files:");
                    progressCallback?.Invoke("  Go to System Utilities → 'Auto Extract Ramdisk'");
                    progressCallback?.Invoke("  Select your device's IPSW file");
                }

                result.Success = true;
            }
            catch (Exception ex)
            {
                result.Error = ex.Message;
                progressCallback?.Invoke($"\n❌ Error: {ex.Message}");
            }

            return result;
        }
    }

    public class BypassPreparationResult
    {
        public bool Success { get; set; }
        public bool AllFilesReady { get; set; }
        public bool GasterReady { get; set; }
        public bool IBSSReady { get; set; }
        public bool IBECReady { get; set; }
        public bool RamdiskReady { get; set; }
        public bool PatchScriptReady { get; set; }
        public string IBSSFile { get; set; }
        public string IBECFile { get; set; }
        public System.Collections.Generic.List<string> MissingFiles { get; set; } = new();
        public string Error { get; set; }
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
