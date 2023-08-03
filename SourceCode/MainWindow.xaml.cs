using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Forms;


namespace Language_switcher_2._0
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    //A class that connects buttons with fitting language handles
    public class Layout
    {
        //LanguageHandles
        public IntPtr inputHandle { get; private set; }
        //Toggle Buttons for each language
        public ToggleButton inputButton { get; private set; }
        public Layout(IntPtr newInputHandle, ToggleButton newInputButton)
        {
            inputHandle = newInputHandle;
            inputButton = newInputButton;
        }
        //Converts IsChecked bool? to bool value for if operator compatibility (inputButton.IsChecked.Value may be null and does not work with if operator)
        public bool IsChecked()
        {
            return inputButton.IsChecked.HasValue ? inputButton.IsChecked.Value : false;
        }
    }
    public partial class MainWindow : Window
    {
        //List of language buttons
        public List<Layout> LangButtons { get; private set; }
        public MainWindow()
        {
            InitializeComponent();
            LangButtons = new List<Layout>();
        }
        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            foreach (InputLanguage lang in InputLanguage.InstalledInputLanguages)
            {
                //Initializing buttons 
                LangButtons.Add(new Layout(lang.Handle, new ToggleButton()));
                LangButtons.Last().inputButton.Content = lang.LayoutName;
                //Activate current input language
                if (lang.Handle == InputLanguage.CurrentInputLanguage.Handle)
                {
                    LangButtons.Last().inputButton.IsChecked = true;
                }
                langChoice.Children.Add(LangButtons.Last().inputButton);
            }
            //Draws the window to set all buttons on it (otherwise it starts below the panel)
            //This procedure cannot be done in the app OnStartup due to the call of EnsureHandle
            this.Show();
            this.Visibility = Visibility.Hidden;
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
        }
        //Hides the window when user clicked elsewhere
        private void Window_Deactivated(object sender, EventArgs e)
        {
            this.Visibility = Visibility.Hidden;
        }
    }
}