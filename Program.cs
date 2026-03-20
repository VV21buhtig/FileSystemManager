using FileSystemManager.Models.Composite;
using FileSystemManager.Models.Adapter;
using FileSystemManager.Models.Facade;

namespace FileSystemManager
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Лабораторная работа №5.\n");

            var root = new FolderItem("Root", "");
            var documents = new FolderItem("Documents", "Root");
            var images = new FolderItem("Images", "Root");

            documents.Add(new FileItem("report.docx", 25000, "Root/Documents"));
            documents.Add(new FileItem("notes.txt", 1500, "Root/Documents"));
            images.Add(new FileItem("photo1.jpg", 2500000, "Root/Images"));
            images.Add(new FileItem("photo2.jpg", 3200000, "Root/Images"));

            root.Add(documents);
            root.Add(images);

            Console.WriteLine("Этап 1: Паттерн Компоновщик\n");
            root.Display();
            Console.WriteLine($"\nОбщий размер: {root.GetSize()} байт\n");

            Console.WriteLine("Этап 2: Паттерн Адаптер\n");

            IFileSystem localFS = new LocalFileSystemAdapter(root);
            IFileSystem ftpFS = new FtpFileSystemAdapter("ftp.example.com");
            IFileSystem cloudFS = new CloudFileSystemAdapter("GoogleDrive");

            Console.WriteLine("Локальная ФС:");
            Console.WriteLine($"Файлы: {string.Join(", ", localFS.ListItems("Root/Documents"))}\n");

            Console.WriteLine("FTP ФС:");
            Console.WriteLine($"Файлы: {string.Join(", ", ftpFS.ListItems("/remote"))}\n");

            Console.WriteLine("Облачная ФС:");
            Console.WriteLine($"Файлы: {string.Join(", ", cloudFS.ListItems("/cloud"))}\n");

            Console.WriteLine("Этап 3: Паттерн Фасад\n");

            SyncFacade syncFacade = new SyncFacade(localFS, cloudFS);
            syncFacade.SyncFolder("Root/Documents", "Cloud/Documents");
            syncFacade.Backup("Root/Images", "Backup/Images");

            Console.WriteLine("Нажмите любую клавишу для выхода...");
            Console.ReadKey();
        }
    }
}