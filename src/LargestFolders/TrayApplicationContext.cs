namespace LargestFolders;

/// <summary>
/// Application context for the system tray application.
/// Manages the notify icon and main form lifecycle.
/// </summary>
public class TrayApplicationContext : ApplicationContext
{
    private readonly NotifyIcon _notifyIcon;
    private MainForm? _mainForm;

    public TrayApplicationContext()
    {
        _notifyIcon = new NotifyIcon
        {
            Icon = LoadFolderIcon(),
            Text = "Largest Folders - Click to view",
            Visible = true,
            ContextMenuStrip = CreateContextMenu()
        };

        // Only handle left-click to show the form (not double-click to avoid rapid show/hide)
        _notifyIcon.MouseClick += NotifyIcon_MouseClick;
    }

    private static Icon LoadFolderIcon()
    {
        // Try to load the embedded folder icon from resources
        var assembly = typeof(TrayApplicationContext).Assembly;
        using var stream = assembly.GetManifestResourceStream("LargestFolders.folder.ico");
        if (stream != null)
        {
            return new Icon(stream);
        }
        
        // Fallback to default application icon if embedded resource not found
        return SystemIcons.Application;
    }

    private ContextMenuStrip CreateContextMenu()
    {
        var contextMenu = new ContextMenuStrip();
        
        var showItem = new ToolStripMenuItem("Show Largest Folders", null, ShowFolders_Click);
        var refreshItem = new ToolStripMenuItem("Refresh", null, Refresh_Click);
        var exitItem = new ToolStripMenuItem("Exit", null, Exit_Click);

        contextMenu.Items.Add(showItem);
        contextMenu.Items.Add(refreshItem);
        contextMenu.Items.Add(new ToolStripSeparator());
        contextMenu.Items.Add(exitItem);

        return contextMenu;
    }

    private void NotifyIcon_MouseClick(object? sender, MouseEventArgs e)
    {
        if (e.Button == MouseButtons.Left)
        {
            ShowMainForm();
        }
    }

    private void ShowFolders_Click(object? sender, EventArgs e)
    {
        ShowMainForm();
    }

    private void Refresh_Click(object? sender, EventArgs e)
    {
        if (_mainForm != null && !_mainForm.IsDisposed)
        {
            _mainForm.RefreshFolderList();
        }
        else
        {
            ShowMainForm();
        }
    }

    private void Exit_Click(object? sender, EventArgs e)
    {
        _notifyIcon.Visible = false;
        _mainForm?.Close();
        Application.Exit();
    }

    private void ShowMainForm()
    {
        if (_mainForm == null || _mainForm.IsDisposed)
        {
            _mainForm = new MainForm();
            _mainForm.FormClosed += MainForm_FormClosed;
        }

        _mainForm.Show();
        _mainForm.BringToFront();
        _mainForm.Activate();
        
        if (_mainForm.WindowState == FormWindowState.Minimized)
        {
            _mainForm.WindowState = FormWindowState.Normal;
        }
    }

    private void MainForm_FormClosed(object? sender, FormClosedEventArgs e)
    {
        // Don't exit when form is closed, just hide to tray
        _mainForm = null;
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _notifyIcon.Visible = false;
            _notifyIcon.Dispose();
            _mainForm?.Dispose();
        }
        base.Dispose(disposing);
    }
}
