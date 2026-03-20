namespace FileSystemManager.Models.Composite
{
    public abstract class FileSystemItem
    {
        public string Name { get; set; }
        public string Path { get; set; }

        public FileSystemItem(string name, string path = "")
        {
            Name = name;
            Path = string.IsNullOrEmpty(path) ? name : path + "/" + name;
        }

        public abstract long GetSize();
        public virtual void Add(FileSystemItem item) => throw new InvalidOperationException("Этот объект не может содержать потомков");
        public virtual void Remove(FileSystemItem item) => throw new InvalidOperationException("Этот объект не может содержать потомков");
        public virtual FileSystemItem? GetChild(int index) => throw new InvalidOperationException("Этот объект не может содержать потомков");
        public virtual int GetChildCount() => 0;
        public abstract void Display(int indent = 0);
    }
}