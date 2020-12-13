using ErosionFinder.Data.Models;
using ErosionFinder.Data.Exceptions;
using ErosionFinder.Dtos;
using ErosionFinder.Extensions;
using ErosionFinder.Logger;
using ErosionFinder.SyntaxWalkers;
using ErosionFinder.Util;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;

namespace ErosionFinder
{
    public static class ErosionFinderMethods
    {
        /// <summary>
        /// Receives a C# solution file path and a set of architectural constraints, 
        /// and returns all it's architectural violations.
        /// </summary>
        /// <param name="solutionFilePath">Solution file path</param>
        /// <param name="constraints">Set of architectural constraints</param>
        /// <returns>List of architectural violations</returns>
        public static Task<IEnumerable<Violation>> GetViolationsBySolutionFilePathAndConstraintsAsync(
            string solutionFilePath, ArchitecturalConstraints constraints, 
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(solutionFilePath))
                throw new ArgumentNullException(nameof(solutionFilePath));

            var solutionFile = new FileInfo(solutionFilePath);

            if (!solutionFile.Exists)
            {
                throw new SolutionException(
                    SolutionError.SolutionFileNotFound);
            }

            ConstraintsAndViolationsMethods.CheckConstraints(constraints);

            return ExecuteGetViolationsBySolutionFilePathAndConstraintsAsync(
                solutionFilePath, constraints, cancellationToken);
        }

        private static async Task<IEnumerable<Violation>> ExecuteGetViolationsBySolutionFilePathAndConstraintsAsync(
            string solutionFilePath, ArchitecturalConstraints constraints, 
            CancellationToken cancellationToken)
        {
            var documents = await MSBuildWorkspaceMethods
                .GetDocumentsAsync(solutionFilePath, cancellationToken);

            var getCodeFiles = documents
                .Select(d => GetCodeFileBySyntaxAsync(d, cancellationToken));

            var codeFiles = await Task.WhenAll(getCodeFiles);

            return ConstraintsAndViolationsMethods.GetViolations(
                constraints, codeFiles, cancellationToken);
        }    

        private static async Task<CodeFile> GetCodeFileBySyntaxAsync(
            Document document, CancellationToken cancellationToken)
        {
            var documentWalker = new DocumentWalker();

            if (cancellationToken.IsCancellationRequested)
                return null;

            await documentWalker.VisitDocumentAsync(
                document, cancellationToken);

            return new CodeFile()
            {
                FileName = document.Name,
                FilePath = document.FilePath,
                Structures = documentWalker.Structures
            };
        }
    }
}