using ClosedXML.Excel;
using System.IO;
using Microsoft.Maui.Storage;
using System.Collections.ObjectModel;

/// <summary>
/// Класс для загрузки и хранения коллекции упражнений из Excel-файла.
/// При первом обращении к классу копирует файл exercises.xlsx из ресурсов приложения в папку AppData,
/// затем считывает все упражнения из этого файла в коллекцию Exercises.
/// Каждый Exercise получает корректные данные, а последний элемент отмечается флагом IsLastItem.
/// </summary>

namespace Setta.Models
{
    public static class ExerciseData
    {
        // Коллекция упражнений, доступная для использования в приложении
        public static ObservableCollection<Exercise> Exercises { get; }

        // Статический конструктор — выполняется один раз при первом обращении к классу
        static ExerciseData()
        {
            const string fileName = "exercises.xlsx";
            // Формируем путь для копии файла в AppData приложения
            var destPath = Path.Combine(FileSystem.Current.AppDataDirectory, fileName);

            // Всегда копируем ресурсный Excel-файл в рабочую директорию приложения
            using (var sourceStream = FileSystem.OpenAppPackageFileAsync(fileName).Result)
            using (var destStream = File.Create(destPath))
            {
                sourceStream.CopyTo(destStream);
            }

            // Загружаем упражнения из только что скопированного файла Excel
            Exercises = LoadFromExcel(destPath);

            // Помечаем последний элемент коллекции для специфических нужд интерфейса (например, оформление разделителей)
            for (int i = 0; i < Exercises.Count; i++)
                Exercises[i].IsLastItem = (i == Exercises.Count - 1);
        }

        /// <summary>
        /// Загружает упражнения из указанного Excel-файла.
        /// </summary>
        /// <param name="path">Путь к Excel-файлу</param>
        /// <returns>Коллекция упражнений</returns>
        static ObservableCollection<Exercise> LoadFromExcel(string path)
        {
            var list = new ObservableCollection<Exercise>();
            using var wb = new XLWorkbook(path);
            var ws = wb.Worksheet(1);

            // Предполагаем, что первая строка — заголовки столбцов, поэтому начинаем с второй строки
            foreach (var row in ws.RowsUsed().Skip(1))
            {
                // Считываем данные из столбцов строки
                var name = row.Cell(1).GetString();
                var primary = row.Cell(2).GetString();
                var secondary = row.Cell(3).GetString();
                var equip = row.Cell(4).GetString();

                // Добавляем новое упражнение в коллекцию
                list.Add(new Exercise
                {
                    ExerciseName = name,
                    MuscleGroup = primary,
                    SecondaryMuscleGroup = secondary,
                    Equipment = equip,
                    IsFromDatabase = false
                });
            }
            return list;
        }
    }
}
