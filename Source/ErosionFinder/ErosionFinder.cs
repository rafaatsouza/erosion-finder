using ErosionFinder.Data.Models;
using ErosionFinder.Data.Exceptions;
using ErosionFinder.Dtos;
using ErosionFinder.Extensions;
using ErosionFinder.SyntaxWalkers;
using ErosionFinder.Util;
using Microsoft.CodeAnalysis;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Threading;
using ErosionFinder.Helpers;

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
        public static Task<ArchitecturalConformanceCheck> CheckArchitecturalConformanceAsync(
            string solutionFilePath, ArchitecturalConstraints constraints, 
            CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(solutionFilePath))
                throw new SolutionException(
                    SolutionError.SolutionFileNotFound);

            var solutionFile = new FileInfo(solutionFilePath);

            if (!solutionFile.Exists)
            {
                throw new SolutionException(
                    SolutionError.SolutionFileNotFound);
            }

            constraints.CheckIfItsValid();

            return ExecuteCheckArchitecturalConformanceAsync(
                solutionFile.FullName, solutionFile.Name, 
                constraints, cancellationToken);
        }

        private static async Task<ArchitecturalConformanceCheck> ExecuteCheckArchitecturalConformanceAsync(
            string solutionPath, string solutionName, ArchitecturalConstraints constraints, 
            CancellationToken cancellationToken)
        {
            var documents = await MSBuildWorkspaceMethods
                .GetDocumentsAsync(solutionPath, cancellationToken);

            var getCodeFilesTask = documents
                .Select(d => GetCodeFileByDocumentAsync(d, cancellationToken));

            var codeFiles = await Task.WhenAll(getCodeFilesTask);

            if (cancellationToken.IsCancellationRequested)
                return null;

            var transgressedRules = GetTransgressedRulesByConstraintsAndCodeFiles(
                constraints, codeFiles);

            return new ArchitecturalConformanceCheck()
            {
                SolutionName = solutionName.Replace(".sln", ""),
                TransgressedRules = transgressedRules,
                FollowedRules = constraints.Rules
                    .Where(r => !transgressedRules.Any(tr => tr.Rule.IsSameRule(r)))
            };
        }

        private static async Task<CodeFile> GetCodeFileByDocumentAsync(
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

        private static IEnumerable<TransgressedRule> GetTransgressedRulesByConstraintsAndCodeFiles(
            ArchitecturalConstraints constraints, ICollection<CodeFile> codeFiles)
        {
            var structures = codeFiles.SelectMany(c => c.Structures);
            var namespaces = structures.Select(s => s.Namespace).Distinct();

            var layersNamespaces = NamespacesGroupingMethodHelper
                .GetLayersNamespaces(constraints.Layers, namespaces);

            foreach(var rule in constraints.Rules)
            {
                var violatingOccurrences = ArchitecturalRuleHelper
                    .GetViolatingOccurrences(rule, layersNamespaces, structures);

                if (violatingOccurrences.Any())
                {
                    yield return new TransgressedRule()
                    {
                        Rule = rule,
                        Violations = violatingOccurrences
                    };
                }
            }
        }
    }
}