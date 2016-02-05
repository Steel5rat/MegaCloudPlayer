using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Interop;
using Player.ViewModels;

namespace Player.Utils
{
    public class HotkeyHook : IDisposable
    {
        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);


        //Codes: https://msdn.microsoft.com/en-us/library/windows/desktop/dd375731(v=vs.85).aspx
        private const uint MOD_ALT = 0x0001;
        private const uint KEY_X = 0x58;
        private const uint KEY_Z = 0x5A;
        private const uint KEY_C = 0x43;
        private const uint KEY_S = 0x53;


        private IntPtr _windowHandle;
        private HwndSource _source;
        private readonly MainWindowViewModel _viewModel;
        public HotkeyHook(Window window, MainWindowViewModel viewModel)
        {
            _viewModel = viewModel;
            _windowHandle = new WindowInteropHelper(window).Handle;
            _source = HwndSource.FromHwnd(_windowHandle);
            _source.AddHook(HwndHook);

            RegisterHotKey(_windowHandle, KEY_X.GetHashCode(), MOD_ALT, KEY_X);
            RegisterHotKey(_windowHandle, KEY_Z.GetHashCode(), MOD_ALT, KEY_Z);
            RegisterHotKey(_windowHandle, KEY_C.GetHashCode(), MOD_ALT, KEY_C);
            RegisterHotKey(_windowHandle, KEY_S.GetHashCode(), MOD_ALT, KEY_S);
        }

        private IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            const int WM_HOTKEY = 0x0312;
            if (msg == WM_HOTKEY)
            {
                handled = true;
                int vkey = (((int)lParam >> 16) & 0xFFFF);
                if (wParam.ToInt32() == KEY_X.GetHashCode())
                {
                    if (vkey == KEY_X)
                    {
                        _viewModel.PauseHotkeyPressed();
                    }
                    else
                    {
                        throw new Exception("This if is necessary");
                    }
                }
                else if (wParam.ToInt32() == KEY_Z.GetHashCode())
                {
                    if (vkey == KEY_Z)
                    {
                        _viewModel.PreviousHotkeyPressed();

                    }
                }
                else if (wParam.ToInt32() == KEY_C.GetHashCode())
                {
                    if (vkey == KEY_C)
                    {
                        _viewModel.NextHotkeyPressed();
                    }
                }
                else if (wParam.ToInt32() == KEY_S.GetHashCode())
                {
                    if (vkey == KEY_S)
                    {
                        _viewModel.ShowHotkeyPressed();
                    }
                }
                else
                {
                    handled = false;
                }
            }
            return IntPtr.Zero;
        }

        public void Dispose()
        {
            _source.RemoveHook(HwndHook);
            UnregisterHotKey(_windowHandle, KEY_X.GetHashCode());
            UnregisterHotKey(_windowHandle, KEY_Z.GetHashCode());
            UnregisterHotKey(_windowHandle, KEY_C.GetHashCode());
            UnregisterHotKey(_windowHandle, KEY_S.GetHashCode());
        }
    }
}