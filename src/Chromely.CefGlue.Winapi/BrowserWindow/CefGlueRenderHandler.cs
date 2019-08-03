using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using WinApi.Gdi32;
using WinApi.User32;
using Xilium.CefGlue;

namespace Chromely.CefGlue.Winapi.BrowserWindow
{
    internal class CefGlueRenderHandler : CefRenderHandler
    {
        [DllImport("user32.dll")]
        static extern bool RedrawWindow(IntPtr hWnd, IntPtr lprcUpdate, IntPtr hrgnUpdate, RedrawWindowFlags flags);

        private readonly Window _window;

        public CefGlueRenderHandler(Window window)
        {
            _window = window;
        }

        protected override CefAccessibilityHandler GetAccessibilityHandler()
        {
            throw new NotImplementedException();
        }

        protected override bool GetRootScreenRect(CefBrowser browser, ref CefRectangle rect)
        {
            return false;
        }

        protected override bool GetViewRect(CefBrowser browser, ref CefRectangle rect)
        {
            var size = _window.GetWindowSize();
            rect.Height = size.Height;
            rect.Width = size.Width;
            return true;
        }

        protected override bool GetScreenInfo(CefBrowser browser, CefScreenInfo screenInfo)
        {
            return false;
        }

        protected override void OnCursorChange(CefBrowser browser, IntPtr cursorHandle, CefCursorType type, CefCursorInfo customCursorInfo)
        {
            _window.Cursor = cursorHandle;
        }

        protected override void OnImeCompositionRangeChanged(CefBrowser browser, CefRange selectedRange, CefRectangle[] characterBounds)
        {
            throw new NotImplementedException();
        }

        protected override void OnPaint(CefBrowser browser, CefPaintElementType type, CefRectangle[] dirtyRects, IntPtr buffer, int width, int height)
        {
            if (type == CefPaintElementType.View)
            {
                Gdi32Methods.DeleteObject(_window.Bitmap);
                _window.Bitmap = Gdi32Methods.CreateBitmap(width, height, 1, 32, buffer);
                _window.BitmapWidth = width;
                _window.BitmapHeight = height;
                RedrawWindow(_window.Handle, IntPtr.Zero, IntPtr.Zero, RedrawWindowFlags.RDW_INTERNALPAINT | RedrawWindowFlags.RDW_INVALIDATE);
            }
        }

        protected override void OnPopupSize(CefBrowser browser, CefRectangle rect)
        {
            throw new NotImplementedException();
        }

        protected override void OnScrollOffsetChanged(CefBrowser browser, double x, double y)
        {
            //throw new NotImplementedException();
        }
    }
}
