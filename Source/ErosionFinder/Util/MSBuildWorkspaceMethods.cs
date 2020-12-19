using ErosionFinder.Data.Exceptions;
using ErosionFinder.Dtos;
using ErosionFinder.Extensions;
using ErosionFinder.Logger;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;

namespace ErosionFinder.Util
{
    internal static class MSBuildWorkspaceMethods
    {
        public static async Task<IEnumerable<Document>> GetDocumentsAsync(
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

                var documents = await GetDocumentsWithExcludedListCheckAsync(
                    solution.Projects, cancellationToken);

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
                return;

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

        private static async Task<IEnumerable<Document>> GetDocumentsWithExcludedListCheckAsync(
            IEnumerable<Project> projects, CancellationToken cancellationToken)
        {
            var checkDocuments = projects
                .SelectMany(p => p.Documents)
                .Select(document => GetDocumentWithExcludedListCheckAsync(document, cancellationToken));

            var documentsWithExcludedListCheck = await Task.WhenAll(checkDocuments);
            
            return documentsWithExcludedListCheck
                .Where(d => !d.IsInExcludedList)
                .Select(d => d.Document);
        }

        private static async Task<DocumentWithExcludedListCheck> GetDocumentWithExcludedListCheckAsync(
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