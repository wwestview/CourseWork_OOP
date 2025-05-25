// File: CourseWork_OOP/Services/TitlePageSkeletonManager.cs
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace CourseWork_OOP.Services
{
    public static class TitlePageSkeletonManager
    {
        public const string StepHeader = "Header";
        public const string StepFacultyAndDepartment = "FacultyAndDepartment";
        public const string StepWorkTitle = "WorkTitle";
        public const string StepStudentAndSupervisor = "StudentAndSupervisor";
        public const string StepEvaluation = "Evaluation";
        public const string StepCommissionMembers = "CommissionMembers";
        public const string StepFooter = "Footer";

        private static readonly HashSet<string> AllKnownSteps = new HashSet<string>
        {
            StepHeader, StepFacultyAndDepartment, StepWorkTitle, StepStudentAndSupervisor,
            StepEvaluation, StepCommissionMembers, StepFooter
        };

        public static List<string> GetDefaultSkeletonOrder()
        {
            // Порядок з вашого файлу BaseTitlePageGenerator.cs
            return new List<string>
            {
                StepHeader,
                StepFacultyAndDepartment,
                StepWorkTitle,
                StepStudentAndSupervisor,
                StepEvaluation,
                StepCommissionMembers,
                StepFooter
            };
        }

        public static List<string> LoadSkeletonOrderFromFile(string filePath)
        {
           
            Debug.WriteLine($"[TitlePageSkeletonManager] Спроба завантаження скелету з файлу: {filePath}");
            if (!File.Exists(filePath))
            {
                Debug.WriteLine($"[TitlePageSkeletonManager] Файл скелету '{filePath}' не знайдено. Використовується скелет за замовчуванням.");
                return GetDefaultSkeletonOrder();
            }

            var stepsFromFile = new List<string>();
            try
            {
                string[] lines = File.ReadAllLines(filePath);
                foreach (string line in lines)
                {
                    string trimmedLine = line.Trim();
                    if (!string.IsNullOrEmpty(trimmedLine) && !trimmedLine.StartsWith("//"))
                    {
                        if (AllKnownSteps.Contains(trimmedLine))
                        {
                            stepsFromFile.Add(trimmedLine);
                        }
                        else
                        {
                            Debug.WriteLine($"[TitlePageSkeletonManager] УВАГА: Невідомий крок '{trimmedLine}' у файлі скелету '{filePath}'. Пропускається.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[TitlePageSkeletonManager] Помилка читання файлу скелету '{filePath}': {ex.Message}. Використовується скелет за замовчуванням.");
                return GetDefaultSkeletonOrder();
            }

            if (stepsFromFile.Any())
            {
                Debug.WriteLine($"[TitlePageSkeletonManager] Скелет успішно завантажено з '{filePath}'. Кроків: {stepsFromFile.Count}");
                return stepsFromFile;
            }
            else
            {
                Debug.WriteLine($"[TitlePageSkeletonManager] Файл скелету '{filePath}' порожній або не містить валідних кроків. Використовується скелет за замовчуванням.");
                return GetDefaultSkeletonOrder();
            }
        }

        public static void ApplySkeleton(BaseTitlePageGenerator generator, List<string> skeletonOrder)
        {
            if (generator != null && skeletonOrder != null && skeletonOrder.Any())
            {
                generator.StepExecutionOrder = new List<string>(skeletonOrder);
                Debug.WriteLine($"[{generator.FormatName}] Застосовано новий скелет з {skeletonOrder.Count} кроків: {string.Join(", ", skeletonOrder)}");
            }
            else
            {
                Debug.WriteLine($"[{generator.FormatName}] Не вдалося застосувати скелет. Буде використано скелет за замовчуванням.");
                if (generator != null && (generator.StepExecutionOrder == null || !generator.StepExecutionOrder.Any()))
                {
                    generator.StepExecutionOrder = GetDefaultSkeletonOrder();
                    Debug.WriteLine($"[{generator.FormatName}] Встановлено скелет за замовчуванням.");
                }
            }
        }
    }
}
