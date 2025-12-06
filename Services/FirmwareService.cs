using System.Collections.Generic;
using System.IO;
using System.IO.Compression;

namespace infex1rn.Services
{
    public class FirmwareService
    {
        public List<string> GetIpswEntries(string ipswPath)
        {
            var entries = new List<string>();
            using (ZipArchive archive = ZipFile.OpenRead(ipswPath))
            {
                foreach (ZipArchiveEntry entry in archive.Entries)
                {
                    entries.Add(entry.FullName);
                }
            }
            return entries;
        }

        public void ExtractIpswEntry(string ipswPath, string entryPath, string destinationPath)
        {
            using (ZipArchive archive = ZipFile.OpenRead(ipswPath))
            {
                if (string.IsNullOrEmpty(entryPath)) // Root node selected, extract all
                {
                    archive.ExtractToDirectory(destinationPath, true);
                }
                else
                {
                    var entry = archive.GetEntry(entryPath);
                    if (entry != null) // It's a file
                    {
                        string destinationFilePath = Path.Combine(destinationPath, entry.Name);
                        entry.ExtractToFile(destinationFilePath, true);
                    }
                    else // It's a directory
                    {
                        foreach (var zipEntry in archive.Entries)
                        {
                            if (zipEntry.FullName.StartsWith(entryPath + "/"))
                            {
                                string destinationFilePath = Path.Combine(destinationPath, zipEntry.FullName);
                                Directory.CreateDirectory(Path.GetDirectoryName(destinationFilePath));
                                if (!string.IsNullOrEmpty(zipEntry.Name))
                                {
                                    zipEntry.ExtractToFile(destinationFilePath, true);
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
