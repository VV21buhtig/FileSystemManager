using FileSystemManager.Models.Composite;

namespace FileSystemManager
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("=== Лабораторная работа №5. Паттерн Компоновщик ===\n");

            var root = new FolderItem("Root", "");

            var documents = new FolderItem("Documents", "Root");
            var images = new FolderItem("Images", "Root");
            var projects = new FolderItem("Projects", "Root/Documents");

            documents.Add(new FileItem("report.docx", 25000, "Root/Documents"));
            documents.Add(new FileItem("notes.txt", 1500, "Root/Documents"));
            documents.Add(projects);

            projects.Add(new FileItem("project1.cs", 5000, "Root/Documents/Projects"));
            projects.Add(new FileItem("project2.cs", 7500, "Root/Documents/Projects"));

            images.Add(new FileItem("photo1.jpg", 2500000, "Root/Images"));
            images.Add(new FileItem("photo2.jpg", 3200000, "Root/Images"));

            root.Add(documents);
            root.Add(images);

            Console.WriteLine("Структура файловой системы:\n");
            root.Display();

            Console.WriteLine($"\nОбщий размер корневой папки: {root.GetSize()} байт");
            Console.WriteLine($"Количество элементов в корне: {root.GetChildCount()}");

            Console.WriteLine("\nНажмите любую клавишу для выхода...");
            Console.ReadKey();
        }
    }
}