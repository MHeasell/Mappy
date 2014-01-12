namespace TAUtil.Gaf
{
    using System;
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

            this.ReadCompressedImage(s, width, height);

            return data;
        }

        private static void RepeatByte(Stream output, byte value, int count)
        {
            for (int i = 0; i < count; i++)
            {
                output.WriteByte(value);
            }
        }

        private void ReadCompressedImage(Stream output, int width, int height)
        {
            for (int i = 0; i < height; i++)
            {
                this.ReadCompressedRow(output, width);
            }
        }

        private void ReadCompressedRow(Stream output, int rowLength)
        {
            int bytes = this.reader.ReadUInt16();

            byte[] compressedRow = this.reader.ReadBytes(bytes);

            this.DecompressRow(compressedRow, output, rowLength);
        }

        private void DecompressRow(byte[] compressedRow, Stream output, int rowLength)
        {
            var stream = new MemoryStream(compressedRow);
            var reader = new BinaryReader(stream);

            int bytesLeft = rowLength;

            while (bytesLeft > 0 && stream.Position < compressedRow.Length)
            {
                bytesLeft -= this.DecompressBlock(reader, output, bytesLeft);
            }

            // Make up for any missing bytes if the row wasn't long enough
            // after being decompressed.
            RepeatByte(output, this.transparencyIndex, bytesLeft);
        }

        private int DecompressBlock(BinaryReader input, Stream output, int limit)
        {
            // read the mask
            byte mask = input.ReadByte();

            if ((mask & 0x01) == 0x01)
            {
                // skip n pixels (transparency)
                int count = Math.Min(mask >> 1, limit);
                RepeatByte(output, this.transparencyIndex, count);
                return count;
            }

            if ((mask & 0x02) == 0x02)
            {
                // repeat this byte n times
                int count = Math.Min((mask >> 2) + 1, limit);
                byte val = input.ReadByte();
                RepeatByte(output, val, count);
                return count;
            }

            // by default, copy next n bytes
            int bytesToRead = (mask >> 2) + 1;
            byte[] buffer = input.ReadBytes(bytesToRead);

            int bytesToWrite = Math.Min(bytesToRead, limit);
            output.Write(buffer, 0, bytesToWrite);

            return bytesToWrite;
        }
    }
}