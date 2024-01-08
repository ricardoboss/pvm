namespace PhpVersionManager.ServiceInterfaces;

public interface ILinkManager
{
    Task LinkAsync(string targetDirectory, string linkName);

    Task UnlinkAsync(string linkName);
}