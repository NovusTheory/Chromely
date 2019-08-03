// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Window.cs" company="Chromely Projects">
//   Copyright (c) 2017-2019 Chromely Projects
// </copyright>
// <license>
//      See the LICENSE.md file in the project root for more information.
// </license>
// ----------------------------------------------------------------------------------------------------------------------

using System;
using Chromely.CefGlue.Browser;
using Chromely.CefGlue.Browser.Handlers;
using Chromely.CefGlue.BrowserWindow;
using Chromely.Core;
using WinApi.User32;
using Xilium.CefGlue;

namespace Chromely.CefGlue.Winapi.BrowserWindow
{
    /// <summary>
    /// The window.
    /// </summary>
    internal class Window : NativeWindow, IWindow
    {
        /// <summary>
        /// The host/app/window application.
        /// </summary>
        private readonly HostBase _application;

        /// <summary>
        /// The host config.
        /// </summary>
        private readonly ChromelyConfiguration _hostConfig;

        /// <summary>
        /// The browser window handle.
        /// </summary>
        private IntPtr _browserWindowHandle;

        private readonly CefGlueRenderHandler _renderHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref="Window"/> class.
        /// </summary>
        /// <param name="application">
        /// The application.
        /// </param>
        /// <param name="hostConfig">
        /// The host config.
        /// </param>
        public Window(HostBase application, ChromelyConfiguration hostConfig)
            : base(hostConfig)
        {
            _hostConfig = hostConfig;
            Browser = new CefGlueBrowser(this, hostConfig, new CefBrowserSettings());
            Browser.Created += OnBrowserCreated;
            _application = application;
            _renderHandler = new CefGlueRenderHandler(this);

            // Set event handler
            Browser.SetEventHandlers();

            ShowWindow();
        }

        /// <summary>
        /// The browser.
        /// </summary>
        public CefGlueBrowser Browser { get; private set; }

        public void CenterToScreen()
        {
            base.CenterToScreen();
        }

        /// <inheritdoc />
        public new void Exit()
        {
            base.CloseWindowExternally();
        }

        #region Dispose

        /// <summary>
        /// The dispose.
        /// </summary>
        public void Dispose()
        {
            if (Browser != null)
            {
                Browser.Dispose();
                Browser = null;
                _browserWindowHandle = IntPtr.Zero;
            }
        }

        #endregion Dispose

        /// <summary>
        /// The on realized.
        /// </summary>
        /// <param name="hwnd">
        /// The hwnd.
        /// </param>
        /// <param name="width">
        /// The width.
        /// </param>
        /// <param name="height">
        /// The height.
        /// </param>
        /// <exception cref="NotSupportedException">
        /// Exception returned for MacOS not supported.
        /// </exception>
        protected override void OnCreate(IntPtr hwnd, int width, int height)
        {
            var windowInfo = CefWindowInfo.Create();
            windowInfo.SetAsChild(hwnd, new CefRectangle(0, 0, _hostConfig.HostWidth, _hostConfig.HostHeight));
            windowInfo.SetAsWindowless(hwnd, false);
            Browser.Create(windowInfo, _renderHandler);
        }

        /// <summary>
        /// The on size.
        /// </summary>
        /// <param name="width">
        /// The width.
        /// </param>
        /// <param name="height">
        /// The height.
        /// </param>
        protected override void OnSize(int width, int height)
        {
            if (Browser.CefBrowser != null)
                Browser.CefBrowser.GetHost().WasResized();
        }

        /// <summary>
        /// The on exit.
        /// </summary>
        protected override void OnExit()
        {
            _application.Quit();
        }

        protected override void OnFocus()
        {
            if (Browser.CefBrowser != null)
                Browser.CefBrowser.GetHost().SendFocusEvent(true);
        }

        protected override void OnFocusLost()
        {
            if (Browser.CefBrowser != null)
                Browser.CefBrowser.GetHost().SendFocusEvent(false);
        }

        protected override void OnMouseMove(int x, int y)
        {
            if (Browser.CefBrowser != null)
                Browser.CefBrowser.GetHost().SendMouseMoveEvent(new CefMouseEvent(x, y, CefEventFlags.None), false);
        }

        protected override void OnMouseEvent(int x, int y, CefMouseButtonType buttonType, bool mouseUp)
        {
            if (Browser.CefBrowser != null)
                Browser.CefBrowser.GetHost().SendMouseClickEvent(new CefMouseEvent(x, y, CefEventFlags.None), buttonType, mouseUp, 1);
        }

        protected override void OnMouseScrollEvent(int x, int y, int deltaX, int deltaY)
        {
            if (Browser.CefBrowser != null)
                Browser.CefBrowser.GetHost().SendMouseWheelEvent(new CefMouseEvent(x, y, CefEventFlags.None), deltaX, deltaY);
        }

        protected override void OnKeyEvent(CefKeyEvent keyEvent)
        {
            if (Browser.CefBrowser != null)
                Browser.CefBrowser.GetHost().SendKeyEvent(keyEvent);
        }

        /// <summary>
        /// The browser created.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void OnBrowserCreated(object sender, EventArgs e)
        {
            /*_browserWindowHandle = Browser.CefBrowser.GetHost().GetWindowHandle();
            if (_browserWindowHandle != IntPtr.Zero)
            {
                var size = GetClientSize();
                NativeMethods.SetWindowPos(_browserWindowHandle, IntPtr.Zero, 0, 0, size.Width, size.Height, WindowPositionFlags.SWP_NOZORDER);
            }*/
        }
    }
}
