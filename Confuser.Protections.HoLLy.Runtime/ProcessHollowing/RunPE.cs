using System;
using System.Runtime.InteropServices;
using dnlib;
using dnlib.DotNet.Writer;

namespace Confuser.Protections.HoLLy.Runtime.ProcessHollowing
{
    internal class RunPE
    {
        /*
         * See:
         * https://github.com/hasherezade/libpeconv/blob/master/run_pe/run_pe.cpp
         * https://github.com/Zer0Mem0ry/RunPE/blob/master/RunPE.cpp
         * https://www.adlice.com/runpe-hide-code-behind-legit-process/
         * https://forum.tuts4you.com/topic/40236-crack-me-32x64/
         */
        public void Install(byte[] newModule, string pathToStart)
        {
            int offsetPE = BitConverter.ToInt32(newModule, 0x3C);

            //read various things from the PE structure
            //perhaps cast into struct later
            int noSections = BitConverter.ToInt16(newModule, offsetPE + 6);
            uint addrEntryPoint = BitConverter.ToUInt32(newModule, offsetPE + 0x28);
            int imageBase = BitConverter.ToInt32(newModule, offsetPE + 0x34);
            uint sizeOfHeaders = BitConverter.ToUInt32(newModule, offsetPE + 0x50);
            int sizeOptHeader = BitConverter.ToInt32(newModule, offsetPE + 0x54);

            //start the remote/target process
            if (!PInvokes.CreateProcess(pathToStart, null, IntPtr.Zero, IntPtr.Zero, false, 4 /*start paused*/, IntPtr.Zero, null, new byte[0x44], out PInvokes.PROCESS_INFORMATION pInfo))
                throw new Exception("Couldn't create process");

            //get the remote image base from the PEB
            //we're also storing the thread context, because we'll update the entry point and then re-set it
            IntPtr imageBasePtr;
            PInvokes.CONTEXT ctx = new PInvokes.CONTEXT { ContextFlags = PInvokes.CONTEXT_FLAGS.CONTEXT_INTEGER }; ;
            PInvokes.CONTEXT64 ctx64 = new PInvokes.CONTEXT64 { ContextFlags = PInvokes.CONTEXT_FLAGS.CONTEXT_INTEGER }; ;
            if (IntPtr.Size == 4) {
                //32-bit process
                if (!PInvokes.GetThreadContext(pInfo.hThread, ref ctx))
                    throw new Exception("Couldn't get thread context");

                imageBasePtr = new IntPtr(ctx.Ebx + 8);
            }
            else {
                //TODO: check for WoW
                //64-bit process
                if (!PInvokes.GetThreadContext(pInfo.hThread, ref ctx64))
                    throw new Exception("Couldn't get thread context");
                imageBasePtr = new IntPtr((long)ctx64.Rdx + 16);
            }
            
            //make sure we can read from the process. TODO: most likely not needed
            byte[] buffer = new byte[4];
            if (!PInvokes.ReadProcessMemory(pInfo.hProcess, imageBasePtr, buffer, 4, out var read1))
                throw new Exception("Couldn't read from remote process");

            //allocate space in the remote process
            var remotePtr = PInvokes.VirtualAllocEx(pInfo.hProcess, new IntPtr(imageBase), sizeOfHeaders,
                PInvokes.AllocationType.Commit | PInvokes.AllocationType.Reserve, PInvokes.MemoryProtection.ExecuteReadWrite);

            //write the image
            if (!PInvokes.WriteProcessMemory(pInfo.hProcess, remotePtr, newModule, sizeOptHeader, out var read2))
                throw new Exception("Couldn't write to remote process");

            //copy the sections over
            for (int i = 0; i < noSections; i++) {
                int[] header = new int[10]; //40 bytes in size
                Buffer.BlockCopy(newModule, offsetPE + 0xF8 + i * 40, header, 0, 40);

                byte[] data = new byte[header[4]];
                Buffer.BlockCopy(newModule, header[5] /*PointerToRawData*/, data, 0, data.Length);

                if (!PInvokes.WriteProcessMemory(pInfo.hProcess, new IntPtr(checked(remotePtr.ToInt32() + header[3] /*RVA*/)), data, data.Length, out var read3))
                    throw new Exception("Couldn't read from remote process");
            }

            //write new image base
            //TODO: can I do IntPtr.Size here? see above
            if (!PInvokes.WriteProcessMemory(pInfo.hProcess, imageBasePtr, BitConverter.GetBytes(remotePtr.ToInt64()), IntPtr.Size, out var read4))
                throw new Exception("Couldn't write to remote process");

            //edit entrypoint and rewrite context
            if (IntPtr.Size == 4) {
                ctx.Eax = (uint)remotePtr.ToInt32() + addrEntryPoint;
                PInvokes.SetThreadContext(pInfo.hThread, ref ctx);
            } else {
                ctx64.Rcx = (ulong)remotePtr.ToInt64() + addrEntryPoint;
                PInvokes.SetThreadContext(pInfo.hThread, ref ctx64);
            }

            //resume the thread, if everything went well it should all work!
            PInvokes.ResumeThread(pInfo.hThread);
        }

        private class PInvokes
        {
            #region CreateProcess
            [DllImport("kernel32.dll", SetLastError = true, CharSet = CharSet.Auto)]
            public static extern bool CreateProcess(
                string lpApplicationName,
                string lpCommandLine,
                //ref SECURITY_ATTRIBUTES lpProcessAttributes,
                //ref SECURITY_ATTRIBUTES lpThreadAttributes,
                IntPtr lpProcessAttributes,
                IntPtr lpThreadAttributes,
                bool bInheritHandles,
                uint dwCreationFlags,
                IntPtr lpEnvironment,
                string lpCurrentDirectory,
                //[In] ref STARTUPINFO lpStartupInfo,
                //IntPtr lpStartupInfo,
                byte[] lpStartupInfo,
                out PROCESS_INFORMATION lpProcessInformation);

            /*
            [StructLayout(LayoutKind.Sequential)]
            public struct SECURITY_ATTRIBUTES
            {
                public int nLength;
                public IntPtr lpSecurityDescriptor;
                public int bInheritHandle;
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct STARTUPINFO
            {
                public uint cb;
                public string lpReserved;
                public string lpDesktop;
                public string lpTitle;
                public uint dwX;
                public uint dwY;
                public uint dwXSize;
                public uint dwYSize;
                public uint dwXCountChars;
                public uint dwYCountChars;
                public uint dwFillAttribute;
                public uint dwFlags;
                public short wShowWindow;
                public short cbReserved2;
                public IntPtr lpReserved2;
                public IntPtr hStdInput;
                public IntPtr hStdOutput;
                public IntPtr hStdError;
            }
            */

            [StructLayout(LayoutKind.Sequential)]
            public struct PROCESS_INFORMATION
            {
                public IntPtr hProcess;
                public IntPtr hThread;
                public uint dwProcessId;
                public uint dwThreadId;
            }
            #endregion

            #region GetThreadContext
            [DllImport("kernel32.dll", SetLastError = true)]
            public static extern bool GetThreadContext(IntPtr hThread, ref CONTEXT lpContext);

            // Get context of thread x64, in x64 application
            [DllImport("kernel32.dll", SetLastError = true)]
            public static extern bool GetThreadContext(IntPtr hThread, ref CONTEXT64 lpContext);

            public enum CONTEXT_FLAGS : uint
            {
                CONTEXT_i386 = 0x10000,
                CONTEXT_i486 = 0x10000,   //  same as i386
                CONTEXT_CONTROL = CONTEXT_i386 | 0x01, // SS:SP, CS:IP, FLAGS, BP
                CONTEXT_INTEGER = CONTEXT_i386 | 0x02, // AX, BX, CX, DX, SI, DI
                CONTEXT_SEGMENTS = CONTEXT_i386 | 0x04, // DS, ES, FS, GS
                CONTEXT_FLOATING_POINT = CONTEXT_i386 | 0x08, // 387 state
                CONTEXT_DEBUG_REGISTERS = CONTEXT_i386 | 0x10, // DB 0-3,6,7
                CONTEXT_EXTENDED_REGISTERS = CONTEXT_i386 | 0x20, // cpu specific extensions
                CONTEXT_FULL = CONTEXT_CONTROL | CONTEXT_INTEGER | CONTEXT_SEGMENTS,
                CONTEXT_ALL = CONTEXT_CONTROL | CONTEXT_INTEGER | CONTEXT_SEGMENTS | CONTEXT_FLOATING_POINT | CONTEXT_DEBUG_REGISTERS | CONTEXT_EXTENDED_REGISTERS
            }
            
            [StructLayout(LayoutKind.Sequential)]
            public struct FLOATING_SAVE_AREA
            {
                public uint ControlWord;
                public uint StatusWord;
                public uint TagWord;
                public uint ErrorOffset;
                public uint ErrorSelector;
                public uint DataOffset;
                public uint DataSelector;
                [MarshalAs(UnmanagedType.ByValArray, SizeConst = 80)]
                public byte[] RegisterArea;
                public uint Cr0NpxState;
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct CONTEXT
            {
                public CONTEXT_FLAGS ContextFlags; //set this to an appropriate value 
                                          // Retrieved by CONTEXT_DEBUG_REGISTERS 
                public uint Dr0;
                public uint Dr1;
                public uint Dr2;
                public uint Dr3;
                public uint Dr6;
                public uint Dr7;
                // Retrieved by CONTEXT_FLOATING_POINT 
                public FLOATING_SAVE_AREA FloatSave;
                // Retrieved by CONTEXT_SEGMENTS 
                public uint SegGs;
                public uint SegFs;
                public uint SegEs;
                public uint SegDs;
                // Retrieved by CONTEXT_INTEGER 
                public uint Edi;
                public uint Esi;
                public uint Ebx;
                public uint Edx;
                public uint Ecx;
                public uint Eax;
                // Retrieved by CONTEXT_CONTROL 
                public uint Ebp;
                public uint Eip;
                public uint SegCs;
                public uint EFlags;
                public uint Esp;
                public uint SegSs;
                // Retrieved by CONTEXT_EXTENDED_REGISTERS 
                [MarshalAs(UnmanagedType.ByValArray, SizeConst = 512)]
                public byte[] ExtendedRegisters;
            }
            
            [StructLayout(LayoutKind.Sequential)]
            public struct M128A
            {
                public ulong High;
                public long Low;
            }
            
            [StructLayout(LayoutKind.Sequential, Pack = 16)]
            public struct XSAVE_FORMAT64
            {
                public ushort ControlWord;
                public ushort StatusWord;
                public byte TagWord;
                public byte Reserved1;
                public ushort ErrorOpcode;
                public uint ErrorOffset;
                public ushort ErrorSelector;
                public ushort Reserved2;
                public uint DataOffset;
                public ushort DataSelector;
                public ushort Reserved3;
                public uint MxCsr;
                public uint MxCsr_Mask;

                [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
                public M128A[] FloatRegisters;

                [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
                public M128A[] XmmRegisters;

                [MarshalAs(UnmanagedType.ByValArray, SizeConst = 96)]
                public byte[] Reserved4;
            }
            
            [StructLayout(LayoutKind.Sequential, Pack = 16)]
            public struct CONTEXT64
            {
                public ulong P1Home;
                public ulong P2Home;
                public ulong P3Home;
                public ulong P4Home;
                public ulong P5Home;
                public ulong P6Home;

                public CONTEXT_FLAGS ContextFlags;
                public uint MxCsr;

                public ushort SegCs;
                public ushort SegDs;
                public ushort SegEs;
                public ushort SegFs;
                public ushort SegGs;
                public ushort SegSs;
                public uint EFlags;

                public ulong Dr0;
                public ulong Dr1;
                public ulong Dr2;
                public ulong Dr3;
                public ulong Dr6;
                public ulong Dr7;

                public ulong Rax;
                public ulong Rcx;
                public ulong Rdx;
                public ulong Rbx;
                public ulong Rsp;
                public ulong Rbp;
                public ulong Rsi;
                public ulong Rdi;
                public ulong R8;
                public ulong R9;
                public ulong R10;
                public ulong R11;
                public ulong R12;
                public ulong R13;
                public ulong R14;
                public ulong R15;
                public ulong Rip;

                public XSAVE_FORMAT64 DUMMYUNIONNAME;

                [MarshalAs(UnmanagedType.ByValArray, SizeConst = 26)]
                public M128A[] VectorRegister;
                public ulong VectorControl;

                public ulong DebugControl;
                public ulong LastBranchToRip;
                public ulong LastBranchFromRip;
                public ulong LastExceptionToRip;
                public ulong LastExceptionFromRip;
            }
            #endregion

            [DllImport("kernel32.dll", SetLastError = true)]
            public static extern bool ReadProcessMemory(
                IntPtr hProcess,
                IntPtr lpBaseAddress,
                [Out] byte[] lpBuffer,
                int dwSize,
                out IntPtr lpNumberOfBytesRead);

            [DllImport("kernel32.dll", SetLastError = true)]
            public static extern bool WriteProcessMemory(
                IntPtr hProcess,
                IntPtr lpBaseAddress,
                byte[] lpBuffer,
                int nSize,
                out IntPtr lpNumberOfBytesWritten);

            [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
            public static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress,
                uint dwSize, AllocationType flAllocationType, MemoryProtection flProtect);

            [Flags]
            public enum AllocationType
            {
                Commit      =     0x1000,
                Reserve     =     0x2000,
                Decommit    =     0x4000,
                Release     =     0x8000,
                Reset       =    0x80000,
                Physical    =   0x400000,
                TopDown     =   0x100000,
                WriteWatch  =   0x200000,
                LargePages  = 0x20000000
            }

            [Flags]
            public enum MemoryProtection
            {
                NoAccess                =   0x1,
                ReadOnly                =   0x2,
                ReadWrite               =   0x4,
                WriteCopy               =   0x8,
                Execute                 =  0x10,
                ExecuteRead             =  0x20,
                ExecuteReadWrite        =  0x40,
                ExecuteWriteCopy        =  0x80,
                GuardModifierflag       = 0x100,
                NoCacheModifierflag     = 0x200,
                WriteCombineModifierflag= 0x400
            }


            [DllImport("kernel32.dll", SetLastError = true)]
            public static extern bool SetThreadContext(IntPtr hThread, ref CONTEXT lpContext);

            // Get context of thread x64, in x64 application
            [DllImport("kernel32.dll", SetLastError = true)]
            public static extern bool SetThreadContext(IntPtr hThread, ref CONTEXT64 lpContext);

            [DllImport("kernel32.dll", SetLastError = true)]
            public static extern uint ResumeThread(IntPtr hThread);
        }
    }
}
