using ClosedXML.Excel;
using System.IO;
using Microsoft.Maui.Storage;
using System.Collections.ObjectModel;

namespace Setta.Models
{
    public static class ExerciseData
    {
        public static ObservableCollection<Exercise> Exercises { get; }

        static ExerciseData()
        {
            const string fileName = "exercises.xlsx";
            // путь к копии в AppData
            var destPath = Path.Combine(FileSystem.Current.AppDataDirectory, fileName);

            // всегда переписываем ресурсный файл в AppData
            using (var sourceStream = FileSystem.OpenAppPackageFileAsync(fileName).Result)
            using (var destStream = File.Create(destPath))
            {
                sourceStream.CopyTo(destStream);
            }

            // загружаем из только-что записанного файла
            Exercises = LoadFromExcel(destPath);

            // отмечаем последний элемент
            for (int i = 0; i < Exercises.Count; i++)
                Exercises[i].IsLastItem = (i == Exercises.Count - 1);
        }

        static ObservableCollection<Exercise> LoadFromExcel(string path)
        {
            var list = new ObservableCollection<Exercise>();
            using var wb = new XLWorkbook(path);
            var ws = wb.Worksheet(1);

            // предполагаем, что первая строка — заголовки:
            foreach (var row in ws.RowsUsed().Skip(1))
            {
                var name = row.Cell(1).GetString();
                var primary = row.Cell(2).GetString();
                var secondary = row.Cell(3).GetString();
                var equip = row.Cell(4).GetString();

                list.Add(new Exercise
                {
                    ExerciseName = name,
                    MuscleGroup = primary,
                    SecondaryMuscleGroup = secondary,
                    Equipment = equip
                });
            }
            return list;
        }
    }
}
