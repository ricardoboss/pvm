using System.IO.Compression;
using PhpVersionManager.ServiceInterfaces;

namespace PhpVersionManager.Services;

public class ZipExtractor : IZipExtractor
{
    public async Task ExtractAsync(string zipPath, string destination, Action<long> reportMaxSize, Action<long> incrementProgress, CancellationToken cancellationToken = default)
    {
        Directory.CreateDirectory(destination);

        using var archive = ZipFile.OpenRead(zipPath);

        reportMaxSize(archive.Entries.Sum(e => e.Length));

        foreach (var entry in archive.Entries)
        {
            var entryPath = Path.Combine(destination, entry.FullName);

            if (entry.Name == "")
            {
                Directory.CreateDirectory(entryPath);
            }
            else
            {
                await using var entryStream = entry.Open();
                await using var fileStream = File.Create(entryPath);

                await entryStream.CopyToAsync(fileStream, cancellationToken);

                incrementProgress(entry.Length);
            }
        }
    }
}
