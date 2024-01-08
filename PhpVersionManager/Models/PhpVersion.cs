namespace PhpVersionManager.Models;

public record PhpVersion(int Major, int Minor, int Patch)
{
    public override string ToString() => $"{Major}.{Minor}.{Patch}";
}
