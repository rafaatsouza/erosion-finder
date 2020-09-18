using ErosionFinder.Data.Models;
using ErosionFinder.Data.Exceptions;
using ErosionFinder.Data.Interfaces;
using ErosionFinder.Dtos;
using ErosionFinder.Extensions;
using ErosionFinder.Logger;
using ErosionFinder.SyntaxWalkers;
using ErosionFinder.Util;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;

namespace ErosionFinder
{
    internal class ErosionFinderService : IErosionFinderService
    {
        private readonly ILogger logger;
        private readonly ILoggerFactory loggerFactory;
        private readonly Microsoft.Build.Framework.ILogger buildLogger;

        public ErosionFinderService(ILoggerFactory loggerFactory, BuildLogger buildLogger)
        {
            this.loggerFactory = loggerFactory
                ?? throw new ArgumentNullException(nameof(loggerFactory));

            this.logger = loggerFactory.CreateLogger<ErosionFinderService>()
                ?? throw new InvalidOperationException(
                    $"Could not create logger for {nameof(ErosionFinderService)}");

            this.buildLogger = buildLogger
                ?? throw new ArgumentNullException(nameof(buildLogger));
        }

        public async Task<IEnumerable<Violation>> GetViolationsBySolutionFilePathAndConstraintsAsync(
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

        private async Task<IEnumerable<CodeFile>> GetCodeFilesBySolutionFilePathAsync(
            string solutionFilePath, CancellationToken cancellationToken)
        {
            var documents = await GetDocumentsAsync(
                solutionFilePath, cancellationToken);

            var getCodeFiles = documents
                .Select(document => GetCodeFileBySyntaxAsync(document, cancellationToken));

            return await Task.WhenAll(getCodeFiles);
        }

        private async Task<IEnumerable<Document>> GetDocumentsAsync(
            string solutionFilePath, CancellationToken cancellationToken)
        {
            using (var ws = MSBuildWorkspace.Create())
            {
                ws.LoadMetadataForReferencedProjects = true;

                var solution = await ws.OpenSolutionAsync(solutionFilePath,
                   msbuildLogger: buildLogger,
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

        private void RegisterWorkspaceDiagnostics(
            IImmutableList<WorkspaceDiagnostic> diagnostics)
        {
            if (diagnostics == null)
            {
                return;
            }

            logger.LogDebug("{DiagnosticsCount} diagnostic messages retrieved "
                + "at solution opening process", diagnostics.Count);

            foreach (var diagnostic in diagnostics)
            {
                if (diagnostic.Kind == WorkspaceDiagnosticKind.Failure)
                {
                    logger.LogDebug("Failure at solution opening: "
                        + "{DiagnosticMessage}", diagnostic.Message);
                }
                else if (diagnostic.Kind == WorkspaceDiagnosticKind.Warning)
                {
                    logger.LogDebug("Warning at solution opening: "
                        + "{DiagnosticMessage}", diagnostic.Message);
                }
            }
        }

        private async Task<CodeFile> GetCodeFileBySyntaxAsync(
            Document document, CancellationToken cancellationToken)
        {
            var documentWalker = new DocumentWalker(loggerFactory);

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

        private async Task<DocumentWithExcludedListCheck> GetDocumentsWithExcludedListCheckAsync(
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