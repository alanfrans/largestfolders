namespace LargestFolders;

/// <summary>
/// Main form that displays the largest folders on the C: drive.
/// </summary>
public partial class MainForm : Form
{
    private readonly ListView _folderListView;
    private readonly Label _statusLabel;
    private readonly Button _refreshButton;
    private readonly ProgressBar _progressBar;
    private CancellationTokenSource? _cancellationTokenSource;

    public MainForm()
    {
        InitializeComponent();

        Text = "Largest Folders on C: Drive";
        Size = new Size(800, 600);
        MinimumSize = new Size(600, 400);
        StartPosition = FormStartPosition.CenterScreen;
        Icon = SystemIcons.Application;

        // Create status label
        _statusLabel = new Label
        {
            Text = "Ready",
            Dock = DockStyle.Bottom,
            Height = 25,
            TextAlign = ContentAlignment.MiddleLeft,
            Padding = new Padding(5, 0, 0, 0)
        };
        Controls.Add(_statusLabel);

        // Create progress bar
        _progressBar = new ProgressBar
        {
            Dock = DockStyle.Bottom,
            Height = 20,
            Style = ProgressBarStyle.Marquee,
            Visible = false
        };
        Controls.Add(_progressBar);

        // Create toolbar panel
        var toolbarPanel = new Panel
        {
            Dock = DockStyle.Top,
            Height = 40,
            Padding = new Padding(5)
        };
        Controls.Add(toolbarPanel);

        // Create refresh button
        _refreshButton = new Button
        {
            Text = "Refresh",
            Size = new Size(80, 30),
            Location = new Point(5, 5)
        };
        _refreshButton.Click += RefreshButton_Click;
        toolbarPanel.Controls.Add(_refreshButton);

        // Create ListView
        _folderListView = new ListView
        {
            Dock = DockStyle.Fill,
            View = View.Details,
            FullRowSelect = true,
            GridLines = true
        };

        _folderListView.Columns.Add("Rank", 50, HorizontalAlignment.Right);
        _folderListView.Columns.Add("Path", 500, HorizontalAlignment.Left);
        _folderListView.Columns.Add("Size", 150, HorizontalAlignment.Right);

        Controls.Add(_folderListView);
        _folderListView.BringToFront();

        // Start scanning on load
        Load += MainForm_Load;
    }

    private async void MainForm_Load(object? sender, EventArgs e)
    {
        await ScanFoldersAsync();
    }

    private async void RefreshButton_Click(object? sender, EventArgs e)
    {
        await ScanFoldersAsync();
    }

    /// <summary>
    /// Triggers a refresh of the folder list.
    /// This is intentionally async void as it's called like an event handler
    /// from the tray context menu. Errors are handled within ScanFoldersAsync.
    /// </summary>
    public async void RefreshFolderList()
    {
        await ScanFoldersAsync();
    }

    private async Task ScanFoldersAsync()
    {
        // Cancel any existing scan
        _cancellationTokenSource?.Cancel();
        _cancellationTokenSource = new CancellationTokenSource();
        var token = _cancellationTokenSource.Token;

        _refreshButton.Enabled = false;
        _progressBar.Visible = true;
        _statusLabel.Text = "Scanning folders on C: drive...";
        _folderListView.Items.Clear();

        try
        {
            var folders = await Task.Run(() => FolderScanner.GetLargestFolders("C:\\", 100, token), token);

            if (token.IsCancellationRequested)
                return;

            _folderListView.BeginUpdate();
            int rank = 1;
            foreach (var folder in folders)
            {
                var item = new ListViewItem(rank.ToString());
                item.SubItems.Add(folder.Path);
                item.SubItems.Add(FormatSize(folder.SizeBytes));
                _folderListView.Items.Add(item);
                rank++;
            }
            _folderListView.EndUpdate();

            _statusLabel.Text = $"Found {folders.Count} folders. Last updated: {DateTime.Now:HH:mm:ss}";
        }
        catch (OperationCanceledException)
        {
            _statusLabel.Text = "Scan cancelled.";
        }
        catch (Exception ex)
        {
            _statusLabel.Text = $"Error: {ex.Message}";
        }
        finally
        {
            _progressBar.Visible = false;
            _refreshButton.Enabled = true;
        }
    }

    private static string FormatSize(long bytes)
    {
        string[] sizes = { "B", "KB", "MB", "GB", "TB" };
        double size = bytes;
        int order = 0;
        
        while (size >= 1024 && order < sizes.Length - 1)
        {
            order++;
            size /= 1024;
        }

        return $"{size:0.##} {sizes[order]}";
    }

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        // Cancel any running scan
        _cancellationTokenSource?.Cancel();
        
        // Hide instead of close to keep tray icon active
        if (e.CloseReason == CloseReason.UserClosing)
        {
            e.Cancel = true;
            Hide();
            return;
        }
        
        base.OnFormClosing(e);
    }
}
