using System;
using System.Runtime.InteropServices;
using static termios.Native;

namespace termios
{
    static class Native
    {
        public const int O_RDWR = 2;
        public const int TCSAFLUSH = 2;

        [DllImport("libc", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern int open(string pathname, int flags);

        [DllImport("libc", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern int tcgetattr(int fd, out termios termios);

        [DllImport("libc", CallingConvention = CallingConvention.Cdecl, CharSet = CharSet.Ansi)]
        public static extern int tcsetattr(int fd, int optActions, ref termios termios);
    }

    [StructLayout(LayoutKind.Sequential)]
    struct termios
    {
        private const int NCCS = 20;

        public InputFlags   c_iflag;
        public OutputFlags  c_oflag;
        public ControlFlags c_cflag;
        public LocalFlags   c_lflag;

        [MarshalAs(UnmanagedType.ByValArray, SizeConst = NCCS)]
        public byte[] c_cc;

        public ulong c_ispeed;
        public ulong c_ospeed;
    }

    [Flags]
    enum InputFlags : ulong { }

    [Flags]
    enum OutputFlags : ulong { }

    [Flags]
    enum ControlFlags : ulong { }

    [Flags]
    enum LocalFlags : ulong
    {
        ECHO = 0x00000008, // enable echoing
    }

    static class Program
    {
        static void Die(string msg)
        {
            Console.Error.WriteLine(msg);
            Environment.Exit(1);
        }

        static void Main(string[] args)
        {
            int res;
            int fd;
            bool enable = false;
            termios t;

            if (args.Length < 1)
                Die("missing argument");

            switch (args[0])
            {
                case "0":
                    enable = false;
                    break;
                case "1":
                    enable = true;
                    break;
                default:
                    Die("invalid argument (must be 0 or 1)");
                    break;
            }

            if ((fd = open("/dev/tty", O_RDWR)) < 0)
                Die($"failed to open /dev/tty file: {fd}");

            Console.WriteLine("\nfd: {0}", fd);

            if ((res = tcgetattr(fd, out t)) != 0)
                Die($"tcgetattr failed: {res}");

            Console.WriteLine("before: 0x{0:x}", (ulong)t.c_lflag);

            if (enable)
                t.c_lflag |= LocalFlags.ECHO;
            else
                t.c_lflag &= ~LocalFlags.ECHO;

            Console.WriteLine("after: 0x{0:x}", (ulong)t.c_lflag);

            if ((res = tcsetattr(fd, TCSAFLUSH, ref t)) != 0)
                Die($"tcsetattr failed: {res}");
        }
    }
}
