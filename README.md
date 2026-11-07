# dwm-get-window-extended-frame-bounds

Reads a window's DWM extended frame bounds on Windows with .NET 10.

```csharp
using SevenKeyboard;
if (DwmGetWindowExtendedFrameBounds.TryGet(hWnd, out WindowFrameBounds bounds)) // Existing HWND.
    Console.WriteLine($"{bounds.Left}, {bounds.Top}, {bounds.Right}, {bounds.Bottom}");
```

Coordinates are in DWM screen space, without DPI conversion; they can differ from `GetWindowRect`.
Use `bounds` only on success. Failure, including an invalid handle, returns `false` and zeroed bounds.
