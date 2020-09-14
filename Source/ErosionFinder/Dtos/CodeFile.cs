using System.Collections.Generic;

namespace ErosionFinder.Dtos
{
    /// <summary>
    /// C# code file - a file with ".cs" extension
    /// </summary>
    internal class CodeFile
    {
        /// <summary>
        /// Name of the file
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// Path of the file
        /// </summary>
        public string FilePath { get; set; }

        /// <summary>
        /// List of structures contained inside the code file
        /// </summary>
        public IEnumerable<Structure> Structures { get; set; }

        public override string ToString() => FileName;
    }
}