using CourseWork_OOP.Interfaces;
using CourseWork_OOP.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace CourseWork_OOP.Services
{
    // Делегат для синхронних кроків побудови
    public delegate void BuildStepDelegate(TitlePageData data);

    public abstract class BaseTitlePageGenerator : ITitlePageGenerator
    {
        public abstract string FormatName { get; }
        public abstract string FileExtension { get; }

        protected Dictionary<string, BuildStepDelegate> AvailableBuildSteps { get; }
        public List<string> StepExecutionOrder { get; set; }

        protected BaseTitlePageGenerator()
        {
            AvailableBuildSteps = new Dictionary<string, BuildStepDelegate>();
            PopulateBuildSteps();

            StepExecutionOrder = TitlePageSkeletonManager.LoadSkeletonOrderFromFile("C:\\Users\\Nazar\\Desktop\\MathAnalys\\StudyUni\\CourseWork_OOP\\CourseWork_OOP\\bin\\Debug\\net8.0-windows\\custom_skeleton.txt");
        }

        protected virtual void PopulateBuildSteps()
        {
            AvailableBuildSteps[TitlePageSkeletonManager.StepHeader] = BuildHeader;
            AvailableBuildSteps[TitlePageSkeletonManager.StepFacultyAndDepartment] = BuildFacultyAndDepartment;
            AvailableBuildSteps[TitlePageSkeletonManager.StepWorkTitle] = BuildWorkTitle;
            AvailableBuildSteps[TitlePageSkeletonManager.StepStudentAndSupervisor] = BuildStudentAndSupervisor;
            AvailableBuildSteps[TitlePageSkeletonManager.StepEvaluation] = BuildEvaluation;
            AvailableBuildSteps[TitlePageSkeletonManager.StepCommissionMembers] = BuildCommissionMembers;
            AvailableBuildSteps[TitlePageSkeletonManager.StepFooter] = BuildFooter;
        }

        protected virtual string GetText(string? text, string defaultText = "")
        {
            return string.IsNullOrWhiteSpace(text) ? defaultText : text;
        }

        public async Task GenerateAsync(TitlePageData data, string argument)
        {
            Debug.WriteLine($"[{FormatName}] Запуск процесу генерації (динамічний скелет). Аргумент: {argument}");

            if (!ValidateData(data))
            {
                Debug.WriteLine($"[{FormatName}] Некоректні вхідні дані для генерації. Генерацію перервано.");
                return;
            }

            InitializeInternalContext(data, argument);

            Debug.WriteLine($"[{FormatName}] Порядок виконання кроків: {string.Join(", ", StepExecutionOrder)}");
            foreach (string stepName in StepExecutionOrder)
            {
                if (AvailableBuildSteps.TryGetValue(stepName, out BuildStepDelegate? stepAction))
                {
                    if (stepAction != null)
                    {
                        Debug.WriteLine($"[{FormatName}] Виконання кроку: {stepName}");
                        stepAction(data); 
                    }
                    else
                    {
                        Debug.WriteLine($"[{FormatName}] УВАГА: Для кроку '{stepName}' делегат не визначено (null).");
                    }
                }
                else
                {
                    Debug.WriteLine($"[{FormatName}] УВАГА: Крок '{stepName}' не знайдено у AvailableBuildSteps. Пропускається.");
                }
            }

            await FinalizeAndPersistOutputAsync(data, argument);
            Debug.WriteLine($"[{FormatName}] Процес генерації завершено для аргументу: {argument}");
        }

        protected abstract void InitializeInternalContext(TitlePageData data, string argument);

        protected abstract void BuildHeader(TitlePageData data);
        protected abstract void BuildFacultyAndDepartment(TitlePageData data);
        protected abstract void BuildWorkTitle(TitlePageData data);
        protected abstract void BuildStudentAndSupervisor(TitlePageData data);
        protected abstract void BuildEvaluation(TitlePageData data);
        protected abstract void BuildCommissionMembers(TitlePageData data);
        protected abstract void BuildFooter(TitlePageData data);

        protected abstract Task FinalizeAndPersistOutputAsync(TitlePageData data, string argument);

        protected virtual bool ValidateData(TitlePageData data)
        {
            if (data == null)
            {
                Debug.WriteLine($"[{FormatName}] Помилка валідації: TitlePageData is null.");
                return false;
            }
            if (string.IsNullOrWhiteSpace(data.StudentsFullName) || string.IsNullOrWhiteSpace(data.Topic))
            {
                Debug.WriteLine($"[{FormatName}] Помилка валідації: ПІБ студента або Тема не вказані у ValidateData базового класу.");
                return false;
            }
            return true;
        }
    }
}
