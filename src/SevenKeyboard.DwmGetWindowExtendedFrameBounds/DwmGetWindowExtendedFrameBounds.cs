// Source: https://github.com/SevenKeyboard/dwm-get-window-extended-frame-bounds
// Author: SevenKeyboard Ltd. (2026)
// License: The Unlicense (see LICENSE)
// Reference: https://learn.microsoft.com/en-us/windows/win32/api/dwmapi/nf-dwmapi-dwmgetwindowattribute

using System.Runtime.InteropServices;

namespace SevenKeyboard;

/// <summary>Queries a window's extended frame bounds without changing DPI policy.</summary>
public static partial class DwmGetWindowExtendedFrameBounds
{
    private const uint ExtendedFrameBoundsAttribute = 9;

    /// <summary>Returns true only when DWM returns S_OK (HRESULT zero).</summary>
    /// <param name="hWnd">The window handle to query.</param>
    /// <param name="bounds">DWM screen-space edges, or the default value on failure.</param>
    public static unsafe bool TryGet(nint hWnd, out WindowFrameBounds bounds)
    {
        bounds = default;

        if (GetWindowAttribute(hWnd, ExtendedFrameBoundsAttribute, out NativeRect rectangle,
            (uint)sizeof(NativeRect)) != 0)
        {
            return false;
        }

        bounds = new WindowFrameBounds(rectangle.Left, rectangle.Top, rectangle.Right, rectangle.Bottom);
        return true;
    }

    // RECT is four signed 32-bit LONGs (16 bytes), including in a 64-bit process.
    [StructLayout(LayoutKind.Sequential)]
    private struct NativeRect
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }

    [LibraryImport("dwmapi.dll", EntryPoint = "DwmGetWindowAttribute")]
    private static partial int GetWindowAttribute(nint hWnd, uint attribute,
        out NativeRect rectangle, uint size);
}