#Requires AutoHotkey v1.1.0+
;==============================================================
; dwmGetWindowExtendedFrameBounds — Gets a window's extended frame bounds via DwmGetWindowAttribute
;
; GitHub: https://github.com/SevenKeyboard/dwm-get-window-extended-frame-bounds
; Author: SevenKeyboard Ltd. (2026)
; License: The Unlicense
;
; Documentation / References:
;   DwmGetWindowAttribute function (dwmapi.h)
;     https://learn.microsoft.com/en-us/windows/win32/api/dwmapi/nf-dwmapi-dwmgetwindowattribute
;==============================================================
class VersionManager_dwmGetWindowExtendedFrameBounds
{
    static _ := VersionManager_dwmGetWindowExtendedFrameBounds._init()
    _init()    {
        global
        DWMGETWINDOWEXTENDEDFRAMEBOUNDS_VERSION := "1.0.0"
    }
}
dwmGetWindowExtendedFrameBounds(hWnd)    {
    static DWMWA_EXTENDED_FRAME_BOUNDS:=9
        ,S_OK:=0x00000000
    obj:={left:"", top:"", right:"", bottom:""}
    varSetCapacity(RECT,cbAttribute:=16,0)
    if (dllCall("Dwmapi.dll\DwmGetWindowAttribute", "Ptr",hWnd, "UInt",DWMWA_EXTENDED_FRAME_BOUNDS, "Ptr",&RECT, "UInt",cbAttribute, "Int")==S_OK)
        obj.left:=numGet(&RECT,0,"Int"), obj.top:=numGet(&RECT,4,"Int"), obj.right:=numGet(&RECT,8,"Int"), obj.bottom:=numGet(&RECT,12,"Int")
    return obj
}