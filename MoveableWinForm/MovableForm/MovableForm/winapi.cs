using System;

using System.Collections.Generic;
using System.Text;

using System.Runtime.InteropServices;
using System.Reflection;

//no namespace is global

    class winapi
    {
        public static void moveWindow(System.Windows.Forms.Form form){
            SetWindowPos(form.Handle, (IntPtr) HWNDPOS.HWND_TOPMOST, form.Left + 10, form.Top + 10, form.Width - 20, form.Height - 20, (uint)SWP.SWP_SHOWWINDOW);
        }

        public static void setStyle(System.Windows.Forms.Form frm)
        {
            uint oldStyle = getStyle(frm);
            uint newStyle = (uint)(
                            WINSTYLES.WS_OVERLAPPED | WINSTYLES.WS_POPUP | WINSTYLES.WS_VISIBLE | 
                            //WINSTYLES.WS_SYSMENU |  // add if you need a X to close
                            WINSTYLES.WS_CAPTION | WINSTYLES.WS_BORDER | //WINSTYLES.WS_DLGFRAME | WINSTYLES.WS_MAXIMIZEBOX |
                            WINSTYLES.WS_POPUPWINDOW);
            int iRet = SetWindowLong(frm.Handle, (int)GWL.GWL_STYLE, (int)newStyle);
            //if returned iRet is zero we got an error

            int newExStyle = GetWindowLong(frm.Handle, (int)GWL.GWL_EXSTYLE);
            newExStyle = (int)((uint)newExStyle - (uint) WINEXSTYLES.WS_EX_CAPTIONOKBTN); //remove OK button
            iRet = SetWindowLong(frm.Handle, (int)GWL.GWL_EXSTYLE, (int)newExStyle);
            moveWindow(frm);
            frm.Refresh();
        }

        public static uint getStyle(System.Windows.Forms.Form frm)
        {
            IntPtr hwnd = frm.Handle;// FindWindow(IntPtr.Zero, "[ zero Dump v0.1 ]");
            uint uiStyle = 0xFFffFFff;
            if (hwnd != IntPtr.Zero)
            {
                string sStyles = "";
                uint wStyle = (uint)GetWindowLong(hwnd, (int)GWL.GWL_STYLE);
                uiStyle = wStyle;
                foreach (WINSTYLES ws in GetValues(new WINSTYLES())) 
                { 
                    if((uint)((uint)ws & (uint)wStyle)==(uint)ws){
                        sStyles += ws.ToString() + ", ";
                    }
                }
                System.Diagnostics.Debug.WriteLine(frm.Text + " is " + sStyles);
                //zDump is WS_OVERLAPPED, WS_POPUP, WS_VISIBLE, WS_CAPTION, WS_BORDER, WS_DLGFRAME, WS_SYSMENU, WS_MAXIMIZEBOX, WS_POPUPWINDOW, 
            }
            return uiStyle;
        }
        public static bool setStyle(System.Windows.Forms.Form frm, uint uStyle)
        {
            int styleOld = 0;
            uint currentStyle = getStyle(frm);
            uint newStyle = currentStyle | uStyle;
            styleOld = SetWindowLong(frm.Handle, (int)(GWL.GWL_STYLE), (int)newStyle);
            if (styleOld != 0)
                return true;
            else
                return false;
        }
        public static bool unsetStyle(System.Windows.Forms.Form frm, uint uStyle)
        {
            int styleOld = 0;
            uint currentStyle = getStyle(frm);
            uint newStyle = currentStyle - uStyle;
            styleOld = SetWindowLong(frm.Handle, (int)(GWL.GWL_STYLE), (int)newStyle);
            if (styleOld != 0)
                return true;
            else
                return false;
        }
        public static string[] getStyles()
        {
            List<string> list = new List<string>();
            foreach (WINSTYLES ws in GetValues(new WINSTYLES()))
            {
                list.Add(ws.ToString());
            }
            return list.ToArray();
        }
        //############## ex styles ###################
        public static uint getStyleEx(System.Windows.Forms.Form frm)
        {
            IntPtr hwnd = frm.Handle;// FindWindow(IntPtr.Zero, "[ zero Dump v0.1 ]");
            uint uiStyle = 0xFFffFFff;
            if (hwnd != IntPtr.Zero)
            {
                string sStyles = "";
                uint wStyle = (uint)GetWindowLong(hwnd, (int)GWL.GWL_EXSTYLE);
                uiStyle = wStyle;
                foreach (WINEXSTYLES ws in GetValues(new WINEXSTYLES()))
                {
                    if ((uint)((uint)ws & (uint)wStyle) == (uint)ws)
                    {
                        sStyles += ws.ToString() + ", ";
                    }
                }
                System.Diagnostics.Debug.WriteLine(frm.Text + " EX is " + sStyles);
                //zDump is WS_OVERLAPPED, WS_POPUP, WS_VISIBLE, WS_CAPTION, WS_BORDER, WS_DLGFRAME, WS_SYSMENU, WS_MAXIMIZEBOX, WS_POPUPWINDOW, 
            }
            return uiStyle;
        }
        public static bool setStyleEx(System.Windows.Forms.Form frm, uint uStyle)
        {
            int styleOld = 0;
            uint currentStyle = getStyleEx(frm);
            uint newStyle = currentStyle | uStyle;
            styleOld = SetWindowLong(frm.Handle, (int)(GWL.GWL_EXSTYLE), (int)newStyle);
            if (styleOld != 0)
                return true;
            else
                return false;
        }
        public static bool unsetStyleEx(System.Windows.Forms.Form frm, uint uStyle)
        {
            int styleOld = 0;
            uint currentStyle = getStyleEx(frm);
            uint newStyle = currentStyle - uStyle;
            styleOld = SetWindowLong(frm.Handle, (int)(GWL.GWL_EXSTYLE), (int)newStyle);
            if (styleOld != 0)
                return true;
            else
                return false;
        }
        public static string[] getStylesEx()
        {
            List<string> list = new List<string>();
            foreach (WINEXSTYLES ws in GetValues(new WINEXSTYLES()))
            {
                list.Add(ws.ToString());
            }
            return list.ToArray();
        }

        //great stuff by http://ideas.dalezak.ca/2008/11/enumgetvalues-in-compact-framework.html
        public static IEnumerable<Enum> GetValues(Enum enumeration)
        {
            List<Enum> enumerations = new List<Enum>();
            foreach (FieldInfo fieldInfo in enumeration.GetType().GetFields(
                  BindingFlags.Static | BindingFlags.Public))
            {
                enumerations.Add((Enum)fieldInfo.GetValue(enumeration));
            }
            return enumerations;
        }

        [DllImport("coredll.dll", SetLastError = true)]
        static extern IntPtr FindWindow(string lpClassName, string lpWindowName);
        // For Windows Mobile, replace user32.dll with coredll.dll
        [DllImport("coredll.dll", SetLastError = true)]
        static extern IntPtr FindWindow(IntPtr ZeroOnly , string lpWindowName);

        [DllImport("coredll.dll", SetLastError = true)]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);
        [DllImport("coredll.dll", SetLastError = true)]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        //non-fullscreen window (ex zDump) = WS_STYLE=0x90c9004, WS_EX_STYLE=0x00000008
        //WS_POPUP | WS_VISIBLE
        enum GWL:int{
            GWL_STYLE = -16,
            GWL_EXSTYLE = -20,
        }

        [Flags]
        public enum WINSTYLES:uint{
            WS_OVERLAPPED =     0x00000000,    //#define WS_OVERLAPPED       WS_BORDER | WS_CAPTION
            WS_POPUP=           0x80000000,
            WS_VISIBLE=         0x10000000,
            WS_MINIMIZE=        0x20000000,
            WS_CLIPSIBLINGS=    0x04000000,
            WS_CLIPCHILDREN=    0x02000000,
            WS_DISABLED=        0x08000000,
            WS_MAXIMIZE=        0x01000000,
            WS_CAPTION =        0x00C00000,    //#define WS_CAPTION          0x00C00000L     /* WS_BORDER | WS_DLGFRAME  */
            WS_BORDER=          0x00800000,
            WS_DLGFRAME=        0x00400000,
            WS_VSCROLL=         0x00200000,
            WS_HSCROLL=         0x00100000,
            WS_SYSMENU=         0x00080000,
            WS_THICKFRAME=      0x00040000,
            WS_MINIMIZEBOX=     0x00020000,
            WS_MAXIMIZEBOX=     0x00010000,
            WS_POPUPWINDOW=     0x80880000,     // 	Creates a pop-up window with WS_BORDER, WS_POPUP, and WS_SYSMENU styles. The WS_CAPTION and WS_POPUPWINDOW styles must be combined to make the window menu visible.
        }
        public enum WINEXSTYLES:uint{
            WS_EX_DLGMODALFRAME    = 0x00000001,
            WS_EX_TOPMOST          = 0x00000008,
            WS_EX_TOOLWINDOW       = 0x00000080,
            WS_EX_WINDOWEDGE       = 0x00000100,
            WS_EX_CLIENTEDGE       = 0x00000200,
            WS_EX_CONTEXTHELP      = 0x00000400,
            WS_EX_RIGHT            = 0x00001000,
            WS_EX_RTLREADING       = 0x00002000,
            WS_EX_LEFTSCROLLBAR    = 0x00004000,
            WS_EX_STATICEDGE       = 0x00020000,
            WS_EX_NOINHERITLAYOUT  = 0x00100000, // Disable inheritence of mirroring by children
            WS_EX_LAYOUTRTL        = 0x00400000, // Right to left mirroring
            WS_EX_OVERLAPPEDWINDOW = 0x00000300, // (WS_EX_WINDOWEDGE | WS_EX_CLIENTEDGE)
            WS_EX_CAPTIONOKBTN     = 0x80000000,
            WS_EX_NODRAG           = 0x40000000,
            WS_EX_ABOVESTARTUP     = 0x20000000,
            WS_EX_INK              = 0x10000000,
            WS_EX_NOANIMATION      = 0x04000000,
    }
        enum BTNSTYLES{
            BS_CENTER = 0x00000300,
            BS_VCENTER = 0x00000C00,
            BS_MULTILINE = 0x00002000,
        }
        [DllImport("coredll.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, uint uFlags);

        enum SWP{
            SWP_ASYNCWINDOWPOS = 0x4000,
            SWP_DEFERERASE = 0x2000,
            SWP_DRAWFRAME = 0x0020,
            SWP_FRAMECHANGED = 0x0020,
            SWP_HIDEWINDOW = 0x0080,
            SWP_NOACTIVATE = 0x0010,
            SWP_NOCOPYBITS = 0x0100,
            SWP_NOMOVE = 0x0002,
            SWP_NOOWNERZORDER = 0x0200,
            SWP_NOREDRAW = 0x0008,
            SWP_NOREPOSITION = 0x0200,
            SWP_NOSENDCHANGING = 0x0400,
            SWP_NOSIZE = 0x0001,
            SWP_NOZORDER = 0x0004,
            SWP_SHOWWINDOW = 0x0040,
        }
        enum HWNDPOS{
            HWND_TOP = 0,
            HWND_BOTTOM = 1,
            HWND_TOPMOST = -1,
            HWND_NOTOPMOST = -2,
        }
    }
