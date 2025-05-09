using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CourseWork_OOP.Services;

namespace CourseWork_OOP.Interfaces
{
    public interface ITitlePageGenerator
    {
        Task GenerateAsync(TitlePageData data, string arg);
    }
}
