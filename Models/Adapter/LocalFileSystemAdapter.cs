using FileSystemManager.Models.Composite;
using System.Collections.Concurrent;

namespace FileSystemManager.Models.Adapter
{
    public class LocalFileSystemAdapter : IFileSystem
    {
        private FolderItem _root;
        private ConcurrentDictionary<string, byte[]> _storage = new ConcurrentDictionary<string, byte[]>();

        public LocalFileSystemAdapter(FolderItem root)
        {
            _root = root;
        }

        public List<string> ListItems(string path)
        {
            var items = new List<string>();
            var folder = FindFolder(path);
            if (folder != null)
            {
                for (int i = 0; i < folder.GetChildCount(); i++)
                {
                    items.Add(folder.GetChild(i).Name);
                }
            }
            return items;
        }

        public byte[] ReadFile(string path)
        {
            if (_storage.TryGetValue(path, out var data))
            {
                return data;
            }
            return Array.Empty<byte>();
        }

        public void WriteFile(string path, byte[] data)
        {
            _storage[path] = data;
        }

        public void DeleteItem(string path)
        {
            _storage.TryRemove(path, out _);
        }

        public long GetSize(string path)
        {
            var folder = FindFolder(path);
            if (folder != null)
            {
                return folder.GetSize();
            }
            return 0;
        }

        private FolderItem? FindFolder(string path)
        {
            if (string.IsNullOrEmpty(path) || path == _root.Name)
                return _root;

            var parts = path.Split('/');
            FileSystemItem? current = _root;

            foreach (var part in parts)
            {
                if (current is FolderItem folder)
                {
                    for (int i = 0; i < folder.GetChildCount(); i++)
                    {
                        if (folder.GetChild(i).Name == part)
                        {
                            current = folder.GetChild(i);
                            break;
                        }
                    }
                }
            }

            return current as FolderItem;
        }
    }
}