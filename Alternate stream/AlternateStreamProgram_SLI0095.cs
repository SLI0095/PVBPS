using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AlternateStreamProgram
{
    class Program
    {
        private const uint GENERIC_WRITE = 0x40000000;
        private const uint GENERIC_READ = 0x80000000;

        private const uint FILE_SHARE_READ = 0x00000001;
        private const uint FILE_SHARE_WRITE = 0x00000002;

        private const uint CREATE_NEW = 1;
        private const uint CREATE_ALWAYS = 2;
        private const uint OPEN_EXISTING = 3;
        private const uint OPEN_ALWAYS = 4;

        [System.Runtime.InteropServices.DllImport("kernel32", SetLastError = true)]
        static extern uint GetFileSize(uint handle,
                                      IntPtr size);

        [System.Runtime.InteropServices.DllImport("kernel32", SetLastError = true)]
        static extern uint ReadFile(uint handle,
                                     byte[] buffer,
                                     uint byteToRead,
                                 ref uint bytesRead,
                                     IntPtr lpOverlapped);

        [System.Runtime.InteropServices.DllImport("kernel32", SetLastError = true)]
        static extern uint CreateFile(string filename,
                                       uint desiredAccess,
                                       uint shareMode,
                                       IntPtr attributes,
                                       uint creationDisposition,
                                       uint flagsAndAttributes,
                                       IntPtr templateFile);

        [System.Runtime.InteropServices.DllImport("kernel32.dll", SetLastError = true)]
        static extern bool WriteFile(uint hFile,
                                     byte[] lpBuffer,
                                     uint nNumberOfBytesToWrite,
                                 ref uint lpNumberOfBytesWritten,
                                     IntPtr lpOverlapped);

        [System.Runtime.InteropServices.DllImport("kernel32.dll", SetLastError = true)]
        static extern bool CloseHandle(uint hFile);
		
        static void Main(string[] args)
        {
            string file = "sli0095.txt";
            string stream = "alternative.txt";

            //write
            byte[] barData = System.Text.Encoding.ASCII.GetBytes("SLI0095, " + DateTime.Now.ToString());
            uint nReturn = 0;

            uint wfHandle = CreateFile(file + ":" + stream, GENERIC_WRITE, FILE_SHARE_WRITE, IntPtr.Zero, CREATE_ALWAYS, 0, IntPtr.Zero);
            bool writeOK = WriteFile(wfHandle,barData, (uint)barData.Length, ref nReturn, IntPtr.Zero);

            CloseHandle(wfHandle);
            if (!writeOK)
                Console.WriteLine("write not successful");
            else
                Console.WriteLine("Write ok in alternate datastream: " + file + ":" + stream);
            
            //read
            uint rfHandle = CreateFile(file + ":" + stream, GENERIC_READ, FILE_SHARE_READ, IntPtr.Zero, OPEN_EXISTING, 0, IntPtr.Zero);
            if (rfHandle != uint.MaxValue)
            {
                Console.WriteLine("Starting reading from: " + file + ":" + stream);
                uint size = GetFileSize(rfHandle, IntPtr.Zero);
                byte[] buffer = new byte[size];

                CloseHandle(rfHandle);
                string readingresult = System.Text.Encoding.ASCII.GetString(buffer);
                Console.WriteLine("Read successful: " + readingresult);
                Console.ReadKey();
            }
        }
    }
}
