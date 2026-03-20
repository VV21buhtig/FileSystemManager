using System.Text;

namespace FileSystemManager.Models.Adapter
{
    public class FtpFileSystemAdapter : IFileSystem
    {
        private Dictionary<string, byte[]> _storage = new Dictionary<string, byte[]>();
        private string _serverAddress;

        public FtpFileSystemAdapter(string serverAddress)
        {
            _serverAddress = serverAddress;
        }

        public List<string> ListItems(string path)
        {
            Console.WriteLine($"[FTP] Подключение к {_serverAddress}, список файлов в {path}");
            return new List<string> { "ftp_file1.txt", "ftp_file2.txt" };
        }

        public byte[] ReadFile(string path)
        {
            Console.WriteLine($"[FTP] Чтение файла {path}");
            if (_storage.TryGetValue(path, out var data))
            {
                return data;
            }
            return Encoding.UTF8.GetBytes($"FTP файл: {path}");
        }

        public void WriteFile(string path, byte[] data)
        {
            Console.WriteLine($"[FTP] Запись файла {path}");
            _storage[path] = data;
        }

        public void DeleteItem(string path)
        {
            Console.WriteLine($"[FTP] Удаление {path}");
            _storage.Remove(path);
        }

        public long GetSize(string path)
        {
            Console.WriteLine($"[FTP] Получение размера {path}");
            return 1024000;
        }
    }
}