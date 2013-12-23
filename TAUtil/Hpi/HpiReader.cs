namespace TAUtil.Hpi
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;

    public sealed class HpiReader : IDisposable
    {
        private IntPtr handle;

        public HpiReader(string filename)
        {
            this.FileName = filename;
            this.handle = NativeMethods.HPIOpen(filename);
            if (this.handle == IntPtr.Zero)
            {
                throw new IOException("failed to read " + filename);
            }
        }

        ~HpiReader()
        {
            this.Dispose(false);
        }

        public string FileName { get; private set; }

        public void Close()
        {
            this.Dispose();
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <param name="directory">The directory to enumerate</param>
        /// <returns>An enumeration of all files in the given dir, full path relative to HPI root</returns>
        public IEnumerable<HpiEntry> GetFilesRecursive(string directory)
        {
            IEnumerable<HpiEntry> en = this.GetFilesAndDirectories(directory);

            foreach (HpiEntry e in en)
            {
                if (e.Type == HpiEntry.FileType.File)
                {
                    yield return new HpiEntry(Path.Combine(directory, e.Name), e.Type, e.Size);
                }
                else
                {
                    var recEn = this.GetFilesRecursive(Path.Combine(directory, e.Name));
                    foreach (HpiEntry f in recEn)
                    {
                        yield return f;
                    }
                }
            }
        }

        /// <returns>An enumeration of all files in the HPI file,
        /// relative to the HPI root</returns>
        public IEnumerable<HpiEntry> GetFiles()
        {
            return this.GetFiles(string.Empty);
        }

        /// <param name="directory">The directory to enumerate, relative to the HPI root</param>
        /// <returns>An enumeration of all the files in the given directory inside the HPI file,
        /// relative to that directory</returns>
        public IEnumerable<HpiEntry> GetFiles(string directory)
        {
            return this.GetFilesAndDirectories(directory).Where(x => x.Type == HpiEntry.FileType.File);
        }

        public IEnumerable<HpiEntry> GetDirectories(string directory)
        {
            return this.GetFilesAndDirectories(directory).Where(x => x.Type == HpiEntry.FileType.Directory);
        }

        public IEnumerable<HpiEntry> GetFilesAndDirectories(string directory)
        {
            int next = 0;
            for (;;)
            {
                StringBuilder s = new StringBuilder();
                int type;
                int size;
                next = NativeMethods.HPIDir(this.handle, next, directory, s, out type, out size);

                if (next == 0)
                {
                    break;
                }

                yield return new HpiEntry(
                    s.ToString(),
                    type == 0 ? HpiEntry.FileType.File : HpiEntry.FileType.Directory,
                    size);
            }
        }

        public Stream ReadFile(string filename)
        {
            IntPtr ptr = NativeMethods.HPIOpenFile(this.handle, filename);

            if (ptr == IntPtr.Zero)
            {
                throw new IOException("failed to read " + filename);
            }

            return new HpiStream(ptr);
        }

        public Stream ReadTextFile(string filename)
        {
            IntPtr ptr = NativeMethods.HPIOpenFile(this.handle, filename);

            if (ptr == IntPtr.Zero)
            {
                throw new IOException("failed to read " + filename);
            }

            HpiStream s = new HpiStream(ptr);
            s.StopAtNull = true;
            return s;
        }

        public void ExtractFile(string filename, string destname)
        {
            if (NativeMethods.HPIExtractFile(this.handle, filename, destname) == 0)
            {
                throw new IOException(string.Format("failed to extract {0} to {1}", filename, destname));
            }
        }

        private void Dispose(bool disposing)
        {
            if (this.handle != IntPtr.Zero)
            {
                NativeMethods.HPIClose(this.handle);
                this.handle = IntPtr.Zero;
            }
        }
    }
}
