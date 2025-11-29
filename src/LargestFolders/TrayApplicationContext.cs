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
            Icon = SystemIcons.Application,
            Text = "Largest Folders - Click to view",
            Visible = true,
            ContextMenuStrip = CreateContextMenu()
        };

        _notifyIcon.Click += NotifyIcon_Click;
        _notifyIcon.DoubleClick += NotifyIcon_Click;
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

    private void NotifyIcon_Click(object? sender, EventArgs e)
    {
        if (e is MouseEventArgs mouseArgs && mouseArgs.Button != MouseButtons.Left)
        {
            return;
        }

        ShowMainForm();
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
