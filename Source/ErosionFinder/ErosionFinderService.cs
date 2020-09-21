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

namespace ErosionFinder.Core
{
    internal static class ErosionFinder
    {
        public static async Task<IEnumerable<Violation>> GetViolationsBySolutionFilePathAndConstraintsAsync(
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

            var codeFiles = await GetCodeFilesBySolutionFilePathAsync(
                solutionFilePath, cancellationToken);

            return ConstraintsAndViolationsMethods.GetViolations(
                constraints, codeFiles, cancellationToken);
        }

        private static async Task<IEnumerable<CodeFile>> GetCodeFilesBySolutionFilePathAsync(
            string solutionFilePath, CancellationToken cancellationToken)
        {
            var documents = await GetDocumentsAsync(
                solutionFilePath, cancellationToken);

            var getCodeFiles = documents
                .Select(document => GetCodeFileBySyntaxAsync(document, cancellationToken));

            return await Task.WhenAll(getCodeFiles);
        }

        private static async Task<IEnumerable<Document>> GetDocumentsAsync(
            string solutionFilePath, CancellationToken cancellationToken)
        {
            using (var ws = MSBuildWorkspace.Create())
            {
                ws.LoadMetadataForReferencedProjects = true;

                var solution = await ws.OpenSolutionAsync(solutionFilePath,
                   msbuildLogger: new BuildLogger(),
                   cancellationToken: cancellationToken);

                RegisterWorkspaceDiagnostics(ws.Diagnostics);

                if (!solution.Projects.Any())
                {
                    throw new SolutionException(
                        SolutionError.SolutionWithoutProjects);
                }

                var checkDocuments = solution.Projects
                    .SelectMany(p => p.Documents)
                    .Select(document => GetDocumentsWithExcludedListCheckAsync(document, cancellationToken));

                var documentsWithExcludedListCheck = await Task.WhenAll(checkDocuments);
                
                var documents = documentsWithExcludedListCheck
                    .Where(d => !d.IsInExcludedList)
                    .Select(d => d.Document);

                if (!documents.Any())
                {
                    throw new SolutionException(
                        SolutionError.SolutionWithoutCodeFiles);
                }

                return documents;
            }
        }

        private static void RegisterWorkspaceDiagnostics(
            IImmutableList<WorkspaceDiagnostic> diagnostics)
        {
            if (diagnostics == null)
            {
                return;
            }

            Trace.WriteLine(string.Format("{0} diagnostic messages retrieved "
                + "at solution opening process", diagnostics.Count));

            foreach (var diagnostic in diagnostics)
            {
                if (diagnostic.Kind == WorkspaceDiagnosticKind.Failure)
                {
                    Trace.WriteLine(string.Format("Failure at solution opening: "
                        + "{0}", diagnostic.Message));
                }
                else if (diagnostic.Kind == WorkspaceDiagnosticKind.Warning)
                {
                    Trace.WriteLine(string.Format("Warning at solution opening: "
                        + "{0}", diagnostic.Message));
                }
            }
        }

        private static async Task<CodeFile> GetCodeFileBySyntaxAsync(
            Document document, CancellationToken cancellationToken)
        {
            var documentWalker = new DocumentWalker();

            if (cancellationToken.IsCancellationRequested)
            {
                return null;
            }

            await documentWalker.VisitDocumentAsync(
                document, cancellationToken);

            return new CodeFile()
            {
                FileName = document.Name,
                FilePath = document.FilePath,
                Structures = documentWalker.Structures
            };
        }

        private static async Task<DocumentWithExcludedListCheck> GetDocumentsWithExcludedListCheckAsync(
            Document document, CancellationToken cancellationToken)
        {
            var isInExcludedList = await document
                .IsInExcludedListAsync(cancellationToken);

            return new DocumentWithExcludedListCheck()
            {
                Document = document,
                IsInExcludedList = isInExcludedList
            };
        }
    }
}