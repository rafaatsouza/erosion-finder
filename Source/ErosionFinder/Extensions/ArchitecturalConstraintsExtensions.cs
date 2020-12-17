using ErosionFinder.Data.Models;
using ErosionFinder.Data.Exceptions;
using System.Collections.Generic;
using System.Linq;

namespace ErosionFinder.Extensions
{
    public static class ArchitecturalConstraintsExtensions
    {
        public static void CheckIfItsValid(
            this ArchitecturalConstraints constraints)
        {
            if (constraints == null)
                throw new ConstraintsException(
                    ConstraintsError.ConstraintsNullOrEmpty);

            constraints.Rules = constraints.Rules
                .Where(r => r != null)
                .Select(r => 
                {
                    r.CheckIfItsValid();

                    return r;
                });
            
            if (!constraints.Layers.Any()
                || !constraints.Rules.Any())
            {
                throw new ConstraintsException(
                    ConstraintsError.ConstraintsNullOrEmpty);
            }
                        
            ChecksExplicitlyEmptyLayers(constraints);
            ChecksUndefinedLayers(constraints);
        }

        private static void ChecksUndefinedLayers(
            ArchitecturalConstraints constraints)
        {
            var originLayers = constraints.Rules
                .Select(r => r.OriginLayer);

            var targetLayers = constraints.Rules
                .Select(r => r.TargetLayer);

            var layersNotDefined = originLayers
                .Concat(targetLayers)
                .Distinct()
                .Any(rl => !constraints.Layers.Any(l => l.Key.Equals(rl)));

            if (layersNotDefined)
            {
                throw new ConstraintsException(
                    ConstraintsError.LayerOfRuleNotDefined);
            }
        }
    
        private static void ChecksExplicitlyEmptyLayers(
            ArchitecturalConstraints constraints)
        {
            var explicitlyDefinedLayers = constraints.Layers
                .Where(l => l.Value is NamespacesExplicitlyGrouped)
                .Select(l => new KeyValuePair<string, NamespacesExplicitlyGrouped>(
                    l.Key, (NamespacesExplicitlyGrouped)l.Value));

            if (explicitlyDefinedLayers.Any())
            {
                foreach (var layer in explicitlyDefinedLayers)
                {
                    if (layer.Value.Namespaces == null || !layer.Value.Namespaces.Any())
                    {
                        throw new ConstraintsException(
                            ConstraintsError.NamespaceNotFoundForLayer(layer.Key));
                    }
                } 
            }
        }
    }
}