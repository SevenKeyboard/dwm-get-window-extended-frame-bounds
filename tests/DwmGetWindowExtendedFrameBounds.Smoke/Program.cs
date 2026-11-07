using System;
using System.Runtime.InteropServices;
using SevenKeyboard;

internal static partial class Program
{
    private const int Guard = 0x53A7B9C1;

    private static int Main()
    {
        if (!OperatingSystem.IsWindows())
        {
            Console.WriteLine("SKIP: Windows is required.");
            return 2;
        }

        try
        {
            CheckInvalidHandle(0);
            CheckInvalidHandle((nint)(-1));
            Console.WriteLine("PASS: Invalid HWNDs fail and reset all seeded bounds.");

            // Read an existing visible foreground window; never create, activate or move one.
            for (int attempt = 0; attempt < 5; attempt++)
            {
                nint hWnd = GetForegroundWindow();
                if (!IsVisibleCandidate(hWnd))
                {
                    continue;
                }

                var before = ReadNative(hWnd);
                WindowFrameBounds bounds = new(-11, 22, 333, 444);
                bool success = DwmGetWindowExtendedFrameBounds.TryGet(hWnd, out bounds);
                var after = ReadNative(hWnd);

                if (!success)
                {
                    Check(bounds == default, "A failed live query must reset all four bounds.");
                }

                // Window lifetime, placement and foreground selection can change between calls.
                if (before != after || hWnd != GetForegroundWindow() || !IsVisibleCandidate(hWnd))
                {
                    continue;
                }

                Check(success == (before.HResult == 0), "Success must match native S_OK.");
                if (!success)
                {
                    Console.WriteLine($"SKIP: Foreground DWM query returned 0x{before.HResult:X8}.");
                    return 2;
                }

                Check(bounds == before.Bounds, "All four signed edges must match the native RECT.");
                if (bounds.Right <= bounds.Left || bounds.Bottom <= bounds.Top)
                {
                    continue;
                }

                Console.WriteLine($"PASS: 16-byte RECT ABI; HWND=0x{hWnd:X}; {bounds}.");
                return 0;
            }

            Console.WriteLine("SKIP: No stable visible foreground window with usable frame bounds.");
            return 2;
        }
        catch (Exception exception)
        {
            Console.Error.WriteLine($"FAIL: {exception.Message}");
            return 1;
        }
    }

    private static void CheckInvalidHandle(nint hWnd)
    {
        Check(ReadNative(hWnd).HResult != 0, "The invalid HWND must fail in DWM.");
        WindowFrameBounds bounds = new(-11, 22, 333, 444);
        Check(!DwmGetWindowExtendedFrameBounds.TryGet(hWnd, out bounds), "Invalid HWND must return false.");
        Check(bounds == default, "Invalid HWND must reset all four seeded bounds.");
    }

    private static bool IsVisibleCandidate(nint hWnd) =>
        hWnd != 0 && IsWindowVisible(hWnd) != 0 && IsIconic(hWnd) == 0;

    private static unsafe (int HResult, WindowFrameBounds Bounds) ReadNative(nint hWnd)
    {
        // Probe the documented 16-byte RECT directly, independently of the library's struct.
        // Signed words at byte offsets 0, 4, 8 and 12 are guarded on both sides.
        int* buffer = stackalloc int[6] { Guard, int.MinValue, int.MinValue, int.MinValue, int.MinValue, Guard };
        int hr = QueryNative(hWnd, 9, buffer + 1, 16);
        Check(buffer[0] == Guard && buffer[5] == Guard, "Native RECT write exceeded 16 bytes.");

        return hr == 0
            ? (hr, new WindowFrameBounds(buffer[1], buffer[2], buffer[3], buffer[4]))
            : (hr, default);
    }

    private static void Check(bool condition, string message)
    {
        if (!condition)
        {
            throw new InvalidOperationException(message);
        }
    }

    [LibraryImport("dwmapi.dll", EntryPoint = "DwmGetWindowAttribute")]
    private static unsafe partial int QueryNative(nint hWnd, uint attribute, int* rectangle, uint size);

    [LibraryImport("user32.dll")]
    private static partial nint GetForegroundWindow();

    [LibraryImport("user32.dll")]
    private static partial int IsWindowVisible(nint hWnd);

    [LibraryImport("user32.dll")]
    private static partial int IsIconic(nint hWnd);
}