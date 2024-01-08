using System.Runtime.CompilerServices;
using PhpVersionManager.Models;

namespace PhpVersionManager.Services;

public class StaticVersionDownloadsProvider : BaseVersionDownloadsProvider
{
    public override async IAsyncEnumerable<VersionDownloadData> GetVersionsAsync([EnumeratorCancellation] CancellationToken cancellationToken = default)
    {
        await Task.Delay(TimeSpan.FromSeconds(2), cancellationToken);

        yield return new(new(8, 3, 1), "https://windows.php.net/downloads/releases/php-8.3.1-nts-Win32-vs16-x64.zip", "76f22325fd13657027f8013c4b51e2a3f5e0eed23d8c43713cfd7b682a799054");

        await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);

        yield return new(new(8, 2, 14), "https://windows.php.net/downloads/releases/php-8.2.14-nts-Win32-vs16-x64.zip", "bbd429d7bdcc34badeb2137adc9eef693c482fa4dfc25dcef6826bef052d4ee2");

        await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);

        yield return new(new(8, 1, 27), "https://windows.php.net/downloads/releases/php-8.1.27-nts-Win32-vs16-x64.zip", "beda45964e30568f4f114c394c0bff7b7b16354b7be75ddfec506e0866c736fc");

        await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
    }
}