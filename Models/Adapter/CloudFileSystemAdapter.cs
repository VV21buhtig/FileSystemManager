using System.Text;

namespace FileSystemManager.Models.Adapter
{
    public class CloudFileSystemAdapter : IFileSystem
    {
        private Dictionary<string, byte[]> _storage = new Dictionary<string, byte[]>();
        private string _cloudProvider;

        public CloudFileSystemAdapter(string cloudProvider)
        {
            _cloudProvider = cloudProvider;
        }

        public List<string> ListItems(string path)
        {
            Console.WriteLine($"[{_cloudProvider}] Список файлов в {path}");
            return new List<string> { "cloud_doc1.docx", "cloud_doc2.xlsx" };
        }

        public byte[] ReadFile(string path)
        {
            Console.WriteLine($"[{_cloudProvider}] Чтение файла {path}");
            if (_storage.TryGetValue(path, out var data))
            {
                return data;
            }
            return Encoding.UTF8.GetBytes($"Cloud файл: {path}");
        }

        public void WriteFile(string path, byte[] data)
        {
            Console.WriteLine($"[{_cloudProvider}] Запись файла {path}");
            _storage[path] = data;
        }

        public void DeleteItem(string path)
        {
            Console.WriteLine($"[{_cloudProvider}] Удаление {path}");
            _storage.Remove(path);
        }

        public long GetSize(string path)
        {
            Console.WriteLine($"[{_cloudProvider}] Получение размера {path}");
            return 2048000;
        }
    }
}