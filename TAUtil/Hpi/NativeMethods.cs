namespace TAUtil.Hpi
{
    using System;
    using System.Runtime.InteropServices;
    using System.Text;

    internal static class NativeMethods
    {
        [DllImport("HPIUtil.dll", BestFitMapping = false)]
        public static extern IntPtr HPIOpen([MarshalAs(UnmanagedType.LPStr)] string fileName);

        [DllImport("HPIUtil.dll")]
        public static extern void HPIClose(IntPtr hpi);

        [DllImport("HPIUtil.dll", BestFitMapping = false)]
        public static extern IntPtr HPIOpenFile(
            IntPtr hpi,
            [MarshalAs(UnmanagedType.LPStr)] string fileName);

        [DllImport("HPIUtil.dll")]
        public static extern int HPICloseFile(IntPtr fileHandle);

        [DllImport("HPIUtil.dll", BestFitMapping = false)]
        public static extern int HPIExtractFile(
            IntPtr hpi,
            [MarshalAs(UnmanagedType.LPStr)] string fileName,
            [MarshalAs(UnmanagedType.LPStr)] string extractName);

        [DllImport("HPIUtil.dll", BestFitMapping = false)]
        public static extern int HPIGetFiles(
            IntPtr hpi,
            int next,
            [Out, MarshalAs(UnmanagedType.LPStr)] StringBuilder name,
            out int type,
            out int size);

        [DllImport("HPIUtil.dll", BestFitMapping = false)]
        public static extern int HPIDir(
            IntPtr hpi,
            int next,
            [MarshalAs(UnmanagedType.LPStr)] string dirName,
            [Out, MarshalAs(UnmanagedType.LPStr)] StringBuilder name,
            out int type,
            out int size);

        [DllImport("HPIUtil.dll", BestFitMapping = false)]
        public static extern IntPtr HPICreate(
            [MarshalAs(UnmanagedType.LPStr)] string fileName,
            HpiWriter.HpiCallback callback);

        [DllImport("HPIUtil.dll", BestFitMapping = false)]
        public static extern int HPICreateDirectory(
            IntPtr pack,
            [MarshalAs(UnmanagedType.LPStr)] string dirName);

        [DllImport("HPIUtil.dll", BestFitMapping = false)]
        public static extern int HPIAddFile(
            IntPtr pack,
            [MarshalAs(UnmanagedType.LPStr)] string hpiName,
            [MarshalAs(UnmanagedType.LPStr)] string fileName);

        [DllImport("HPIUtil.dll", BestFitMapping = false)]
        public static extern int HPIAddFileFromMemory(
            IntPtr pack,
            [MarshalAs(UnmanagedType.LPStr)] string hpiName,
            IntPtr fileBlock,
            int fSize);

        [DllImport("HpiUtil.dll")]
        public static extern int HPIPackArchive(
            IntPtr pack,
            HpiWriter.CompressionMethod cMethod);
    }
}
