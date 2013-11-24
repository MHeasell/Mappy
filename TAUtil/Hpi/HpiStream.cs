namespace TAUtil.Hpi
{
    using System;
    using System.IO;

    /// <summary>
    /// This class represents an unamanged memory stream with unbounded length(!).
    /// This stream doesn't know how long it is, so it's trivial to read
    /// over the end of the buffer and land in unknown memory.
    /// 
    /// Very dangerous! Use with caution!
    /// </summary>
    internal unsafe sealed class HpiStream : Stream
    {
        private readonly IntPtr ptr;
        private readonly byte* hpiFileHandle;

        public HpiStream(IntPtr ptr)
        {
            this.ptr = ptr;
            this.hpiFileHandle = (byte*)ptr.ToPointer();
        }

        public bool StopAtNull
        {
            get;
            set;
        }

        public override bool CanRead
        {
            get { return true; }
        }

        public override bool CanSeek
        {
            get { return true; }
        }

        public override bool CanWrite
        {
            get { return false; }
        }

        public override long Length
        {
            get { throw new NotSupportedException(); }
        }

        public override long Position
        {
            get;
            set;
        }

        public override void Flush()
        {
            throw new NotSupportedException();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            for (int i = 0; i < count; i++)
            {
                byte b = this.hpiFileHandle[this.Position];
                if (this.StopAtNull && b == 0)
                {
                    return i;
                }

                buffer[offset + i] = b;
                this.Position++;
            }

            return count;
        }

        public override int ReadByte()
        {
            byte b = this.hpiFileHandle[this.Position];

            if (this.StopAtNull && b == 0)
            {
                return -1;
            }

            this.Position++;
            return b;
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            switch (origin)
            {
                case SeekOrigin.Begin:
                    this.Position = offset;
                    break;
                case SeekOrigin.Current:
                    this.Position += offset;
                    break;
                case SeekOrigin.End:
                    throw new NotSupportedException("cannot seek from end of stream");
                default:
                    throw new ArgumentException("invalid seek origin");
            }

            return this.Position;
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            NativeMethods.HPICloseFile(this.ptr);
        }
    }
}
