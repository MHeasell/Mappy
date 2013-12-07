namespace TAUtil.Hpi
{
    public struct HpiEntry
    {
        public HpiEntry(string name, FileType type, int size)
            : this()
        {
            this.Name = name;
            this.Type = type;
            this.Size = size;
        }

        public enum FileType
        {
            File,
            Directory
        }

        public string Name { get; set; }

        public FileType Type { get; set; }

        public int Size { get; set; }
    }
}
