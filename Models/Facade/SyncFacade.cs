using FileSystemManager.Models.Adapter;

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