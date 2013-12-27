namespace TAUtil.Gaf
{
    using System.IO;

    public class CompressedFrameReader
    {
        private readonly BinaryReader reader;

        private readonly byte transparencyIndex;

        public CompressedFrameReader(BinaryReader reader, byte transparencyIndex)
        {
            this.reader = reader;
            this.transparencyIndex = transparencyIndex;
        }

        public byte[] ReadCompressedImage(int width, int height)
        {
            byte[] data = new byte[width * height];

            var s = new MemoryStream(data, true);

            this.ReadCompressedImage(s, height);

            return data;
        }

        private void ReadCompressedImage(Stream output, int rows)
        {
            for (int i = 0; i < rows; i++)
            {
                this.ReadCompressedRow(output);
            }
        }

        private void ReadCompressedRow(Stream output)
        {
            int bytes = this.reader.ReadUInt16();
            long nextLinePos = this.reader.BaseStream.Position + bytes;

            // while there are still bytes left in the line to read
            while (this.reader.BaseStream.Position < nextLinePos)
            {
                this.ReadMaskedBlock(output);
            }
        }

        private void ReadMaskedBlock(Stream output)
        {
            // read the mask
            byte mask = this.reader.ReadByte();

            if ((mask & 0x01) == 0x01)
            {
                // skip n pixels (transparency)
                this.SkipBytes(output, mask >> 1);
            }
            else if ((mask & 0x02) == 0x02)
            {
                // repeat this byte n times
                int count = (mask >> 2) + 1;
                this.RepeatNextByte(output, count);
            }
            else
            {
                // copy next n bytes
                int count = (mask >> 2) + 1;
                this.CopyBytes(output, count);
            }
        }

        private void SkipBytes(Stream output, int count)
        {
            for (int i = 0; i < count; i++)
            {
                output.WriteByte(this.transparencyIndex);
            }
        }

        private void CopyBytes(Stream output, int count)
        {
            for (int i = 0; i < count; i++)
            {
                output.WriteByte(this.reader.ReadByte());
            }
        }

        private void RepeatNextByte(Stream output, int count)
        {
            byte value = this.reader.ReadByte();
            for (int i = 0; i < count; i++)
            {
                output.WriteByte(value);
            }
        }
    }
}