using ErosionFinder.Domain.Models;
using ErosionFinder.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ErosionFinder.SyntaxWalkers;
using System.Threading;
using ErosionFinder.Logger;
using Dasync.Collections;
using Microsoft.Extensions.Logging;
using System.Collections.Immutable;
using ErosionFinder.Util;
using System.IO;
using ErosionFinder.Domain.Exceptions;
using ErosionFinder.Domain.Interfaces;

namespace ErosionFinder
{
    internal class ErosionFinderService : IErosionFinderService
    {
        private readonly ILogger logger;
        private readonly ILoggerFactory loggerFactory;
        private readonly Microsoft.Build.Framework.ILogger buildLogger;

        private const int MaxDegreeOfParallelism = 10;

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
            string solutionFilePath, ArchitecturalConstraints constraints, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(solutionFilePath))
                throw new ArgumentNullException(nameof(solutionFilePath));

            var solutionFile = new FileInfo(solutionFilePath);

            if (!solutionFile.Exists)
            {
                throw new SolutionCustomException(
                    SolutionCustomError.SolutionFileNotFound);
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
            var documents = await GetDocumentsAsync(solutionFilePath, cancellationToken);

            var codeFiles = new List<CodeFile>();
            var lockObject = new object();

            await documents.ParallelForEachAsync(async document =>
            {
                var codeFile = await GetCodeFileBySyntaxAsync(
                    document, cancellationToken);

                if (codeFile != null)
                {
                    lock (lockObject)
                    {
                        codeFiles.Add(codeFile);
                    }
                }
            }, maxDegreeOfParallelism: MaxDegreeOfParallelism, cancellationToken);

            return codeFiles;
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
                    throw new SolutionCustomException(
                        SolutionCustomError.SolutionWithoutProjects);
                }

                var lockObject = new object();
                var documents = new List<Document>();

                await solution.Projects
                    .SelectMany(p => p.Documents)
                    .ParallelForEachAsync(async document =>
                    {
                        var isInExcludedList = await document
                            .IsInExcludedListAsync(cancellationToken);

                        if (!isInExcludedList)
                        {
                            lock (lockObject)
                            {
                                documents.Add(document);
                            }
                        }
                    }, maxDegreeOfParallelism: MaxDegreeOfParallelism, cancellationToken);

                if (!documents.Any())
                {
                    throw new SolutionCustomException(
                        SolutionCustomError.SolutionWithoutCodeFiles);
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
    }
}