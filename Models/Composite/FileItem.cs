namespace FileSystemManager.Models.Composite
{
    public class FileItem : FileSystemItem
    {
        public long Size { get; set; }

        public FileItem(string name, long size, string path = "") : base(name, path)
        {
            Size = size;
        }

        public override long GetSize() => Size;

        public override void Display(int indent = 0)
        {
            Console.WriteLine(new string(' ', indent) + $"[Файл] {Name} ({Size} байт)");
        }
    }
}