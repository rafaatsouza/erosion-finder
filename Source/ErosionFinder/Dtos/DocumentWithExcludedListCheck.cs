using Microsoft.CodeAnalysis;

namespace ErosionFinder.Dtos
{
    internal class DocumentWithExcludedListCheck
    {
        /// <summary>
        /// Document retrieved from the solution
        /// </summary>
        public Document Document { get; set; }

        /// <summary>
        /// Flag that indicates if the document is in excluded list
        /// </summary>
        public bool IsInExcludedList { get; set; }
    }
}