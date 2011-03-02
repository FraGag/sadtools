//------------------------------------------------------------------------------
// <copyright file="NativeMethods.cs" company="Sonic Retro &amp; Contributors">
//     Copyright (c) Sonic Retro &amp; Contributors. All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

namespace SonicRetro.SonicAdventure.SADXPCDecompiler
{
    using System;
    using System.Runtime.InteropServices;
    using BOOL = System.Boolean;
    using DWORD = System.UInt32;
    using HICON = System.IntPtr;
    using HIMAGELIST = System.IntPtr;
    using HWND = System.IntPtr;
    using LONG = System.Int32;
    using UINT = System.UInt32;
    using ULONGLONG = System.UInt64;

    internal static class NativeMethods
    {
        [Flags]
        public enum TBPFLAG
        {
            TBPF_NOPROGRESS = 0,
            TBPF_INDETERMINATE = 0x1,
            TBPF_NORMAL = 0x2,
            TBPF_ERROR = 0x4,
            TBPF_PAUSED = 0x8
        };

        [Flags]
        public enum THUMBBUTTONMASK
        {
            THB_BITMAP = 0x1,
            THB_ICON = 0x2,
            THB_TOOLTIP = 0x4,
            THB_FLAGS = 0x8
        }

        [Flags]
        public enum THUMBBUTTONFLAGS
        {
            THBF_ENABLED = 0,
            THBF_DISABLED = 0x1,
            THBF_DISMISSONCLICK = 0x2,
            THBF_NOBACKGROUND = 0x4,
            THBF_HIDDEN = 0x8,
            THBF_NONINTERACTIVE = 0x10
        }

        [Flags]
        public enum STPFLAG
        {
            STPF_NONE = 0,
            STPF_USEAPPTHUMBNAILALWAYS = 0x1,
            STPF_USEAPPTHUMBNAILWHENACTIVE = 0x2,
            STPF_USEAPPPEEKALWAYS = 0x4,
            STPF_USEAPPPEEKWHENACTIVE = 0x8
        }

        [StructLayout(LayoutKind.Sequential)]
        public class RefRECT
        {
            public LONG left;
            public LONG top;
            public LONG right;
            public LONG bottom;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct THUMBBUTTON
        {
            public THUMBBUTTONMASK dwMask;
            public UINT iId;
            public UINT iBitmap;
            public HICON hIcon;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szTip;
            public THUMBBUTTONFLAGS dwFlags;
        }

        [ComImport, InterfaceType(ComInterfaceType.InterfaceIsIUnknown), Guid("ea1afb91-9e28-4b86-90e9-9e9f8a5eefaf")]
        public interface ITaskbarList3
        {
            void HrInit();
            void AddTab([In] HWND hwnd);
            void DeleteTab([In] HWND hwnd);
            void ActivateTab([In] HWND hwnd);
            void SetActiveAlt([In] HWND hwnd);
            void MarkFullscreenWindow([In] HWND hwnd, [In, MarshalAs(UnmanagedType.Bool)] BOOL fFullscreen);
            void SetProgressValue([In] HWND hwnd, [In] ULONGLONG ullCompleted, [In] ULONGLONG ullTotal);
            void SetProgressState([In] HWND hwnd, [In] TBPFLAG tbpFlags);
            void RegisterTab([In] HWND hwndTab, [In] HWND hwndMDI);
            void UnregisterTab([In] HWND hwndTab);
            void SetTabOrder([In] HWND hwndTab, [In] HWND hwndInsertBefore);
            void SetTabActive([In] HWND hwndTab, [In] HWND hwndMDI, DWORD dwReserved);
            void ThumbBarAddButtons([In] HWND hwnd, [In] UINT cButtons, [In, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] THUMBBUTTON[] pButton);
            void ThumbBarUpdateButtons([In] HWND hwnd, [In] UINT cButtons, [In, MarshalAs(UnmanagedType.LPArray, SizeParamIndex = 1)] THUMBBUTTON[] pButton);
            void ThumbBarSetImageList([In] HWND hwnd, [In] HIMAGELIST himl = default(HIMAGELIST));
            void SetOverlayIcon([In] HWND hwnd, [In] HICON hIcon, [In, MarshalAs(UnmanagedType.LPWStr)] string pszDescription = null);
            void SetThumbnailTooltip([In] HWND hwnd, [In, MarshalAs(UnmanagedType.LPWStr)] string pszTip = null);
            void SetThumbnailClip([In] HWND hwnd, RefRECT prcClip);
        }

        public static int WM_TASKBARBUTTONCREATED;

        [DllImport("user32.dll", CharSet = CharSet.Auto, BestFitMapping = false, ThrowOnUnmappableChar = true)]
        public static extern int RegisterWindowMessage(string msg);
    }
}
