using Microsoft.CodeAnalysis;

namespace ErosionFinder.Dtos
{
    internal class DocumentWithExcludedListCheck
    {
        public Document Document { get; set; }

        public bool IsInExcludedList { get; set; }
    }
}