namespace FileSystemManager.Models.Composite
{
    public class FolderItem : FileSystemItem
    {
        private List<FileSystemItem> _children = new List<FileSystemItem>();

        public FolderItem(string name, string path = "") : base(name, path) { }

        public override long GetSize()
        {
            long totalSize = 0;
            foreach (var child in _children)
            {
                totalSize += child.GetSize();
            }
            return totalSize;
        }

        public override void Add(FileSystemItem item)
        {
            _children.Add(item);
        }

        public override void Remove(FileSystemItem item)
        {
            _children.Remove(item);
        }

        public override FileSystemItem? GetChild(int index)
        {
            if (index >= 0 && index < _children.Count)
                return _children[index];
            return null;
        }

        public override int GetChildCount() => _children.Count;

        public override void Display(int indent = 0)
        {
            Console.WriteLine(new string(' ', indent) + $"[Папка] {Name}");
            foreach (var child in _children)
            {
                child.Display(indent + 2);
            }
        }
    }
}