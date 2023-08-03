# Language_Switcher
Notification Icon app that allows to manage keyboard layouts if you have more than 2
****Intended Functionality:****
- An application with an interface that resembles Windows desktop language panel as closely as possible.

  
![image](https://github.com/Shedmaster136/Language_Switcher/assets/96218277/bc454c28-6a7f-4290-b45a-42123daecaad)



- Application allows to manage keyboard layout switching when user knows and actively uses more than one language.
- When user presses HotKey combination **(Ctrl+Spacebar) **, the layout switches between two or more user defined layouts.
- User chooses the layouts using window interface with radiobuttons, representing one layout.
- The windows appears when the NotifyIcon is clicked.
- To close the application a user has to click on NotificationIcon with right mouse button, and choose 'Close' in the context menu that appears.

This is how the interface looks on practice:


![image](https://github.com/Shedmaster136/Language_Switcher/assets/96218277/98fe7220-6db6-4b39-bef3-4550d0b9a7d0)



****Realization Details:****
Logical parts of an application:
1. Language Window
2. NotifyIcon
3. Layouts management
4. HotKey Registration
5. Hook procedure

**1.**
  Language Window is written using WPF framework. All the static elements are made in XAML, all the dynamic ones are added in C# code.

**2.**
  NotifyIcon is not in WPF framework. To access this functionality the use of WindowsForms framework was necessary.

**3.**
  Layouts management is represented by a separate class, that stores Radio Button handles and layout ids. The list of installed layouts is accessed via Windows. Forms classes. The layouts list compiled on application launch and after the installation of additional layouts it is necessary to reload the application.

**4.**
  HotKey registration functionality is accessed via imported unmanaged functions from user32.dll:
  
  -RegisterHotKey
  
  -UnregisterHotKey

**5.**
  Hook procedure is necessary to catch and manage the registered HotKey message, when the window is inactive. The hook procedure is registered using a function from and Interop library, that allows to acces the low-level functionality of Win32 API. When the hook is registered it monitors for HotKey message. When it gets the message, it calls the function from user32.dll "PostMessage" with WM_INPUTLANGCHANGEREQUEST and layour id, that we want to activate.
  
  
