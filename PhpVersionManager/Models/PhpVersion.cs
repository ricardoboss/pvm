namespace PhpVersionManager.Models;

public record PhpVersion(int Major, int Minor, int Patch)
{
    /// <summary>
    /// This must always result in a valid filename.
    /// </summary>
    /// <returns>The version as a string.</returns>
    public override string ToString() => $"{Major}.{Minor}.{Patch}";
}
