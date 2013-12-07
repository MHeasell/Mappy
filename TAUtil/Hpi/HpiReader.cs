namespace TAUtil.Hpi
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;

    public sealed class HpiReader : IDisposable
    {
        private IntPtr handle;

        public HpiReader(string filename)
        {
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

        public void Close()
        {
            this.Dispose();
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <returns>An enumeration of all files in the HPI file,
        /// relative to the HPI root</returns>
        public IEnumerable<string> GetFiles()
        {
            return new FileEnumerator(this);
        }

        /// <param name="directory">The directory to enumerate</param>
        /// <returns>An enumeration of all files in the given dir, full path relative to HPI root</returns>
        public IEnumerable<string> GetFilesRecursive(string directory)
        {
            IEnumerable<string> en = new FileEnumerator(this, FileEnumerator.Mode.File, directory);
            foreach (string f in en)
            {
                yield return Path.Combine(directory, f);
            }

            IEnumerable<string> dirEn = new FileEnumerator(this, FileEnumerator.Mode.Directory, directory);
            foreach (string d in dirEn)
            {
                IEnumerable<string> recEn = this.GetFilesRecursive(Path.Combine(directory, d));
                foreach (string f in recEn)
                {
                    yield return f;
                }
            }
        }

        /// <param name="directory">The directory to enumerate, relative to the HPI root</param>
        /// <returns>An enumeration of all the files in the given directory inside the HPI file,
        /// relative to that directory</returns>
        public IEnumerable<string> GetFiles(string directory)
        {
            return new FileEnumerator(this, FileEnumerator.Mode.File, directory);
        }

        public IEnumerable<string> GetDirectories(string directory)
        {
            return new FileEnumerator(this, FileEnumerator.Mode.Directory, directory);
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

        private class FileEnumerator : IEnumerable<string>
        {
            private readonly HpiReader archive;
            private readonly string directory;
            private readonly Mode mode;

            public FileEnumerator(HpiReader archive,  Mode mode, string directory)
            {
                this.archive = archive;
                this.directory = directory;
                this.mode = mode;
            }

            public FileEnumerator(HpiReader archive, Mode mode) : this(archive, mode, null)
            {
            }

            public FileEnumerator(HpiReader archive)
                : this(archive, Mode.File)
            {
            }

            public enum Mode
            { 
                File,
                Directory
            }

            public IEnumerator<string> GetEnumerator()
            {
                int next = 0;
                for (;;)
                {
                    StringBuilder s = new StringBuilder();
                    int type;
                    int size;
                    if (this.directory == null)
                    {
                        next = NativeMethods.HPIGetFiles(this.archive.handle, next, s, out type, out size);
                    }
                    else
                    {
                        next = NativeMethods.HPIDir(this.archive.handle, next, this.directory, s, out type, out size);
                    }

                    if (next == 0)
                    {
                        break;
                    }

                    if ((this.mode == Mode.File && type == 0) || (this.mode == Mode.Directory && type == 1))
                    {
                        yield return s.ToString();
                    }
                }
            }

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }
        }
    }
}
