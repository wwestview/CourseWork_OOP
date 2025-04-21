using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CourseWork_OOP.Services
{
    public abstract class CoverPageGenerator
    {
        protected CoverPageData Data { get; }

        protected CoverPageGenerator(CoverPageData data)
        {
            Data = data ?? throw new ArgumentNullException(nameof(data));
        }
        public abstract Task GenerateAsync(string baseFileName);
    }
}
