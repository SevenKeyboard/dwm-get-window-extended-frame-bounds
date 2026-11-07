// Source: https://github.com/SevenKeyboard/dwm-get-window-extended-frame-bounds
// Author: SevenKeyboard Ltd. (2026)
// License: The Unlicense (see LICENSE)

namespace SevenKeyboard;

/// <summary>Immutable extended frame edges in the screen-space coordinates returned by DWM.</summary>
public readonly record struct WindowFrameBounds
{
    public WindowFrameBounds(int left, int top, int right, int bottom)
    {
        Left = left;
        Top = top;
        Right = right;
        Bottom = bottom;
    }

    public int Left { get; }
    public int Top { get; }
    public int Right { get; }
    public int Bottom { get; }
}