using FirstFloor.ModernUI.Windows.Controls;
using IDE.Common.ViewModels;
using System.Windows;
using System.Windows.Interop;
using System;

namespace IDE.Others
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : ModernWindow
    {

        public MainWindow()
        {
            InitializeComponent();
            // Disable moving window
            SourceInitialized += MainWindow_SourceInitialized;
        }

        private void MainWindow_SourceInitialized(object sender, System.EventArgs e)
        {
            var helper = new WindowInteropHelper(this);
            var source = HwndSource.FromHwnd(helper.Handle);
            source?.AddHook(WndProc);
        }

        private IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                // WM_SYSCOMMAND
                case 0x0112:
                    var command = wParam.ToInt32() & 0xfff0;
                    if (command == 0xF010) // SC_MOVE
                    {
                        handled = true;
                    }
                    break;
                default:
                    break;
            }
            return IntPtr.Zero;
        }
    }
}
