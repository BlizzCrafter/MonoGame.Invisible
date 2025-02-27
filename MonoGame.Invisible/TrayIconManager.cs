namespace MonoGame.Invisible
{
    /// <summary>
    /// Manages the tray icon for the application.
    /// </summary>
    public static class TrayIconManager
    {
        public static NotifyIcon TrayIcon { get; private set; }

        public static ContextMenuStrip ContextMenu => TrayIcon.ContextMenuStrip!;

        /// <summary>
        /// Initializes the tray icon with the specified icon.
        /// </summary>
        /// <param name="icon">The icon to use for the tray icon.</param>
        public static void Init(Icon? icon = default)
        {
            TrayIcon = new NotifyIcon
            {
                Icon = icon ?? SystemIcons.Application,
                Visible = true,
                Text = TransparentWindowManager.AppName,
                ContextMenuStrip = new ContextMenuStrip()
            };

            TrayIcon.MouseClick += TrayIcon_MouseClick;
        }

        private static void TrayIcon_MouseClick(object? sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                TrayIcon.ContextMenuStrip!.Show(Cursor.Position);
            }
        }

        /// <summary>
        /// Disposes the tray icon.
        /// </summary>
        public static void Dispose()
        {
            TrayIcon.Visible = false;
            TrayIcon.Dispose();
        }
    }
}
