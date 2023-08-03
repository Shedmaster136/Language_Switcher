using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;
using WinForms = System.Windows.Forms;
namespace Language_switcher_2._0
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {

        //NotifyIcon - Windows.Forms element that allows to create
        //an application icon in instruments panel
        private WinForms.NotifyIcon langSwitchIcon;
        //Pointer to an underlying window Win32 to use non-WPF functions from C libraries
        //Needed to register a hotkey combination
        private IntPtr hWndWIN32;
        //Wrapper for a Win32 window to access hook registration functions
        //Needed to register a function that will react to a hotkey
        private HwndSource win32Wrap;
        //Register and unregister Hotkey functions import from DLL
        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint gsModifiers, uint vk);
        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);


        //flags for hotkey functions
        private const uint MOD_CONTROL = 0x0002; //Ctrl has to be held
        private const uint MOD_NOREPEAT = 0x4000;//Flag that disables multiple reactions to a comand
        private const uint VK_SPACE = 0x20;//hot key to be pressed along with geld Ctrl
        private const int HOTKEY_ID = 0x001F;// Randomly chosen ID to register and unregister hotkeys
        private const int WM_HOTKEY = 0x0312;//A constant, that is defined within windows documentation to determine that the hotkey signal has come
        private const uint WM_INPUTLANGCHANGEREQUEST = 0x0050;

        //Function to switch keyboard layout. It sends a message to all windows to change the keyboard layout
        [DllImport("user32.dll")]
        static extern bool PostMessage(Int64 hWnd, uint msg, IntPtr wParam, IntPtr lParam);
        public App()
        {
            //Initialize Notification Icon
            langSwitchIcon = new WinForms.NotifyIcon();
            this.langSwitchIcon.Icon = new System.Drawing.Icon("Icon\\Language-Switch-icon.ico");
        }
        //This func is called when the application is started
        protected override void OnStartup(StartupEventArgs e)
        {
            //Basic startup of a parent class, that would have started if this one did not exist
            base.OnStartup(e);
            //Notification Icon parameters initialization 
            this.langSwitchIcon.Text = "Choose languages to switch";
            this.langSwitchIcon.Visible = true;
            //Register left click action
            this.langSwitchIcon.MouseClick += new WinForms.MouseEventHandler(this.LangSwitchIcon_Click);
            //Context menu buttons include:
            //Close button that terminates the process
            this.langSwitchIcon.ContextMenuStrip = new WinForms.ContextMenuStrip();
            this.langSwitchIcon.ContextMenuStrip.Items.Add("Close");
            this.langSwitchIcon.ContextMenuStrip.Items[0].Click += new EventHandler(this.ContextClose_Click);
            var interopHelper = new WindowInteropHelper(this.MainWindow);
            //EnsureHandle launches MainWin inizialization and OnSourceInitialized
            //This function is necessary to ensure that the window is created as soon as app starts
            hWndWIN32 = interopHelper.EnsureHandle();
            win32Wrap = HwndSource.FromHwnd(hWndWIN32);
            //Register hook for HotKey occurence
            win32Wrap.AddHook(HotKeyHook);
            //Register HotKey that will be caught by the hook
            if (!RegisterHotKey(hWndWIN32, HOTKEY_ID, MOD_CONTROL | MOD_NOREPEAT, VK_SPACE))
            {
                System.Windows.MessageBox.Show("Unable to initialize a HotKey. Application is closing");
                this.Shutdown();
                return;
            }
        }
        /*Goes through the list of dynamically created buttons and after finding the currently chosen language,
         chooses the next "checked". For cases the chosen language is last on the list, goes over the list twice.
        If chosen language is not among the checked, goes over the list once again and chooses the first checked*/
        private IntPtr NextLanguage()
        {
            MainWindow testWindow = (MainWindow)this.MainWindow;
            int LangCount = testWindow.LangButtons.Count;
            bool ReachedCurrent = false;
            for (int i = 0; i < (LangCount * 2); i++)
            {
                int m = i % LangCount;
                bool Checked = testWindow.LangButtons[m].IsChecked();
                if (Checked)
                {
                    if (ReachedCurrent == false)
                    {
                        if (testWindow.LangButtons[m].inputHandle == WinForms.InputLanguage.CurrentInputLanguage.Handle)
                        {
                            ReachedCurrent = true;
                        }
                    }
                    else
                    {
                        return testWindow.LangButtons[m].inputHandle;
                    }
                }
                //Currently chosen language is not checked, choose the first checked language from the top
                if (m == LangCount - 1)
                {
                    ReachedCurrent = true;
                }
            }
            return WinForms.InputLanguage.CurrentInputLanguage.Handle;
        }
        //Hook for hotkey combination

            private IntPtr HotKeyHook(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == WM_HOTKEY)
            {
                if (wParam.ToInt32() == HOTKEY_ID)
                {
                    int ctrlKey = ((int)lParam >> 16) & 0xFFFF;
                    if (ctrlKey == VK_SPACE)
                    {
                        //Turn the next availible and chosen layout
                        IntPtr nextLang = NextLanguage();
                        PostMessage(0xffff, WM_INPUTLANGCHANGEREQUEST, IntPtr.Zero, nextLang);
                    }
                    handled = true;
                }
            }
            return IntPtr.Zero;
        } 

        //Finish everything on shutdown
        protected override void OnExit(ExitEventArgs e)
        {
            win32Wrap.RemoveHook(HotKeyHook);
            UnregisterHotKey(hWndWIN32, HOTKEY_ID);
            this.langSwitchIcon.Dispose();
            base.OnExit(e);
        }
        //Shutdown application
        private void ContextClose_Click(object sender, EventArgs e)
        {
            this.Shutdown();
        }
        //Open the window
        private void LangSwitchIcon_Click(object sender, WinForms.MouseEventArgs e)
        {
            if (e.Button == WinForms.MouseButtons.Left)
            {
                var DesktopSize = SystemParameters.WorkArea;
                MainWindow.Left = DesktopSize.Right - this.MainWindow.Width;
                MainWindow.Top = DesktopSize.Bottom - this.MainWindow.Height;
                if (this.MainWindow.Visibility == Visibility.Visible)
                {
                    this.MainWindow.Visibility = Visibility.Hidden;
                }
                else
                {
                    this.MainWindow.Visibility = Visibility.Visible;
                    this.MainWindow.Activate();
                }

            }
        }
    }
}
