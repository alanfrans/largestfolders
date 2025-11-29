namespace LargestFolders;

static class Program
{
    // Unique mutex name using a GUID to prevent conflicts with other applications
    private const string MutexName = "Global\\LargestFolders_8A7F3E2D-9B1C-4D5E-A6F7-8C9D0E1F2A3B";

    /// <summary>
    ///  The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        // Ensure only one instance runs at a time
        using var mutex = new Mutex(true, MutexName, out bool createdNew);
        if (!createdNew)
        {
            // Another instance is already running
            return;
        }

        ApplicationConfiguration.Initialize();
        Application.Run(new TrayApplicationContext());
    }
}