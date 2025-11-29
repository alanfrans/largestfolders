namespace LargestFolders;

/// <summary>
/// Represents information about a folder and its size.
/// </summary>
public record FolderInfo(string Path, long SizeBytes);

/// <summary>
/// Scans the file system to find the largest folders.
/// </summary>
public static class FolderScanner
{
    // System directories to skip during scanning
    private static readonly string[] SystemDirectories =
    {
        @"C:\$Recycle.Bin",
        @"C:\System Volume Information",
        @"C:\Config.Msi",
        @"C:\Recovery",
        @"C:\ProgramData\Microsoft\Windows\Containers"
    };

    /// <summary>
    /// Gets the largest folders on the specified drive.
    /// </summary>
    /// <param name="rootPath">The root path to scan (e.g., "C:\").</param>
    /// <param name="count">The number of folders to return.</param>
    /// <param name="cancellationToken">Cancellation token to stop the scan.</param>
    /// <returns>A list of the largest folders, sorted by size descending.</returns>
    public static List<FolderInfo> GetLargestFolders(string rootPath, int count, CancellationToken cancellationToken = default)
    {
        var folderSizes = new Dictionary<string, long>();

        try
        {
            ScanDirectory(new DirectoryInfo(rootPath), folderSizes, cancellationToken);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch
        {
            // Ignore errors at root level
        }

        return folderSizes
            .Select(kvp => new FolderInfo(kvp.Key, kvp.Value))
            .OrderByDescending(f => f.SizeBytes)
            .Take(count)
            .ToList();
    }

    private static long ScanDirectory(DirectoryInfo directory, Dictionary<string, long> folderSizes, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        long totalSize = 0;

        try
        {
            // Calculate size of files in this directory
            foreach (var file in directory.EnumerateFiles())
            {
                try
                {
                    totalSize += file.Length;
                }
                catch
                {
                    // Skip files we can't access
                }
            }

            // Recursively scan subdirectories
            foreach (var subDirectory in directory.EnumerateDirectories())
            {
                cancellationToken.ThrowIfCancellationRequested();
                
                try
                {
                    // Skip system directories that typically cause access issues
                    if (IsSystemDirectory(subDirectory.FullName))
                    {
                        continue;
                    }

                    totalSize += ScanDirectory(subDirectory, folderSizes, cancellationToken);
                }
                catch (UnauthorizedAccessException)
                {
                    // Skip directories we don't have access to
                }
                catch (DirectoryNotFoundException)
                {
                    // Skip directories that no longer exist
                }
                catch (OperationCanceledException)
                {
                    throw;
                }
                catch
                {
                    // Skip any other errors
                }
            }
        }
        catch (UnauthorizedAccessException)
        {
            // Skip directories we don't have access to
        }
        catch (DirectoryNotFoundException)
        {
            // Skip directories that no longer exist
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch
        {
            // Skip any other errors
        }

        // Store the total size for this directory
        if (totalSize > 0)
        {
            folderSizes[directory.FullName] = totalSize;
        }

        return totalSize;
    }

    private static bool IsSystemDirectory(string path)
    {
        return SystemDirectories.Any(sysDir => 
            path.StartsWith(sysDir, StringComparison.OrdinalIgnoreCase));
    }
}
