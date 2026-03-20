# FileSystemManager - Документация по проекту

## Содержание
1. [FileSystemManager.csproj](#filesystemmanagercsproj)
2. [Program.cs](#programcs)
3. [Models/Adapter](#models-adapter)
   1. [CloudFileSystemAdapter.cs](#cloudfilesystemadaptercs)
   2. [FtpFileSystemAdapter.cs](#ftpfilesystemadaptercs)
   3. [IFileSystem.cs](#ifilesystemcs)
   4. [LocalFileSystemAdapter.cs](#localfilesystemadaptercs)
4. [Models/Composite](#models-composite)
   1. [FileItem.cs](#fileitemcs)
   2. [FileSystemItem.cs](#filesystemitemcs)
   3. [FolderItem.cs](#folderitemcs)
5. [Models/Facade](#models-facade)
   1. [SyncFacade.cs](#syncfacadecs)

## FILE 1: FileSystemManager.csproj

<a id='filesystemmanagercsproj'></a>

```xml
﻿<Project Sdk="Microsoft.NET.Sdk">

  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <TargetFramework>net8.0</TargetFramework>
    <ImplicitUsings>enable</ImplicitUsings>
    <Nullable>enable</Nullable>
  </PropertyGroup>

</Project>
```

---

## FILE 2: CloudFileSystemAdapter.cs

<a id='cloudfilesystemadaptercs'></a>

```csharp
﻿using System.Text;

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
```

---

## FILE 3: FtpFileSystemAdapter.cs

<a id='ftpfilesystemadaptercs'></a>

```csharp
﻿using System.Text;

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
```

---

## FILE 4: IFileSystem.cs

<a id='ifilesystemcs'></a>

```csharp
﻿namespace FileSystemManager.Models.Adapter
{
    public interface IFileSystem
    {
        List<string> ListItems(string path);
        byte[] ReadFile(string path);
        void WriteFile(string path, byte[] data);
        void DeleteItem(string path);
        long GetSize(string path);
    }
}
```

---

## FILE 5: LocalFileSystemAdapter.cs

<a id='localfilesystemadaptercs'></a>

```csharp
﻿using FileSystemManager.Models.Composite;
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
```

---

## FILE 6: FileItem.cs

<a id='fileitemcs'></a>

```csharp
﻿namespace FileSystemManager.Models.Composite
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
```

---

## FILE 7: FileSystemItem.cs

<a id='filesystemitemcs'></a>

```csharp
﻿namespace FileSystemManager.Models.Composite
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
```

---

## FILE 8: FolderItem.cs

<a id='folderitemcs'></a>

```csharp
﻿namespace FileSystemManager.Models.Composite
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
```

---

## FILE 9: SyncFacade.cs

<a id='syncfacadecs'></a>

```csharp
﻿using FileSystemManager.Models.Adapter;

namespace FileSystemManager.Models.Facade
{
    public class SyncFacade
    {
        private IFileSystem _sourceFS;
        private IFileSystem _targetFS;

        public SyncFacade(IFileSystem source, IFileSystem target)
        {
            _sourceFS = source;
            _targetFS = target;
        }

        public void SyncFolder(string sourcePath, string targetPath)
        {
            Console.WriteLine($"\nСинхронизация: {sourcePath} -> {targetPath} ");

            var items = _sourceFS.ListItems(sourcePath);

            foreach (var item in items)
            {
                string sourceFilePath = $"{sourcePath}/{item}";
                string targetFilePath = $"{targetPath}/{item}";

                Console.WriteLine($"Синхронизация файла: {item}");

                try
                {
                    var data = _sourceFS.ReadFile(sourceFilePath);
                    _targetFS.WriteFile(targetFilePath, data);
                    Console.WriteLine($"  Успешно");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"  Ошибка: {ex.Message}");
                }
            }

            Console.WriteLine("Синхронизация завершена.\n");
        }

        public void Backup(string sourcePath, string backupPath)
        {
            Console.WriteLine($"\nРезервное копирование: {sourcePath} -> {backupPath} ===");

            var items = _sourceFS.ListItems(sourcePath);
            int successCount = 0;
            int failCount = 0;

            foreach (var item in items)
            {
                string sourceFilePath = $"{sourcePath}/{item}";
                string backupFilePath = $"{backupPath}/{item}";

                Console.WriteLine($"Копирование: {item}");

                try
                {
                    var data = _sourceFS.ReadFile(sourceFilePath);
                    _targetFS.WriteFile(backupFilePath, data);
                    successCount++;
                    Console.WriteLine($"  Успешно");
                }
                catch (Exception ex)
                {
                    failCount++;
                    Console.WriteLine($"  Ошибка: {ex.Message}");
                }
            }

            Console.WriteLine($"\nРезультат: {successCount} успешно, {failCount} ошибок");
            Console.WriteLine("Резервное копирование завершено.\n");
        }

        public void FullSync()
        {
            Console.WriteLine("\nПолная синхронизация всех систем");
            SyncFolder("Root/Documents", "Backup/Documents");
            SyncFolder("Root/Images", "Backup/Images");
            Console.WriteLine("Полная синхронизация завершена.\n");
        }
    }
}
```

---

## FILE 10: Program.cs

<a id='programcs'></a>

```csharp
﻿using FileSystemManager.Models.Composite;
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
```

---

