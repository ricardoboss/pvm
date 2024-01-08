namespace PhpVersionManager.Models;

public record VersionDownloadData(PhpVersion Version, string ZipUrl, string Sha256);
