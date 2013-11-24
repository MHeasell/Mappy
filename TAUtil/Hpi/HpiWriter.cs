namespace TAUtil.Hpi
{
    using System;
    using System.IO;

    public sealed class HpiWriter : IDisposable
    {
        private readonly CompressionMethod compression;

        private IntPtr handle;

        public HpiWriter(
            string filename,
            CompressionMethod compression = CompressionMethod.LZ77,
            HpiCallback callback = null)
        {
            this.handle = NativeMethods.HPICreate(filename, callback);
            if (this.handle == IntPtr.Zero)
            {
                throw new IOException("failed to create " + filename);
            }

            this.compression = compression;
        }

        ~HpiWriter()
        {
            this.Dispose(false);
        }

        public delegate int HpiCallback(
            string filename,
            string hpiName,
            int fileCount,
            int fileCountTotal,
            int fileBytes,
            int fileBytesTotal,
            int totalBytes,
            int totalBytesTotal);

        public enum CompressionMethod
        {
            None = 0,
            LZ77 = 1,
            ZLib = 2
        }

        public void CreateDirectory(string dirName)
        {
            int success = NativeMethods.HPICreateDirectory(this.handle, dirName);

            if (success == 0)
            {
                throw new IOException("failed to create directory " + dirName);
            }
        }

        public void AddFile(string hpiName, string fileName)
        {
            int success = NativeMethods.HPIAddFile(this.handle, hpiName, fileName);

            if (success == 0)
            {
                throw new IOException("failed to add file " + fileName);
            }
        }

        public void Close()
        {
            this.Dispose();
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (this.handle != IntPtr.Zero)
            {
                NativeMethods.HPIPackArchive(this.handle, this.compression);
                this.handle = IntPtr.Zero;
            }
        }
    }
}
