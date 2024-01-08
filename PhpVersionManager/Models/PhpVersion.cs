namespace PhpVersionManager.Models;

public record PhpVersion(int Major, int Minor, int Patch)
{
    /// <summary>
    /// This must always result in a valid filename.
    /// </summary>
    /// <returns>The version as a string.</returns>
    public override string ToString() => $"{Major}.{Minor}.{Patch}";

    public static PhpVersion FromDirectoryName(string directoryName)
    {
        var parts = directoryName.Split('.');
        if (parts.Length != 3)
            throw new ArgumentException("Directory name must be in the format 'major.minor.patch'.", nameof(directoryName));

        if (!int.TryParse(parts[0], out var major))
            throw new ArgumentException("Major version must be a number.", nameof(directoryName));

        if (!int.TryParse(parts[1], out var minor))
            throw new ArgumentException("Minor version must be a number.", nameof(directoryName));

        if (!int.TryParse(parts[2], out var patch))
            throw new ArgumentException("Patch version must be a number.", nameof(directoryName));

        return new(major, minor, patch);
    }
}
