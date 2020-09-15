<a href="icon.png"><img width="42px" height="42px" src="icon.png" /></a> ErosionFinder
==================================

[![NuGet version (ErosionFinder)](https://img.shields.io/nuget/v/ErosionFinder.svg?style=flat-square)](https://www.nuget.org/packages/ErosionFinder/)

ErosionFinder is a library to find architectural erosion across C# code; it uses the Roslyn API to navigate throught all components from some C# solution, and checks for architectural violations based on informed rules.

Installation
------------

[ErosionFinder is available on NuGet](https://www.nuget.org/packages/ErosionFinder). It can be installed with:
```
Install-Package ErosionFinder
```

Example
------------
```
using Microsoft.Build.Locator;
using System.Threading.Tasks;

namespace Sample
{
  static class Program
  {
    static Program()
    {
        MSBuildLocator.RegisterDefaults();
    }
  }
  
  static async Task Main(string[] args)
  {
    var serviceLayerRegex = new Regex(@"(TargetSolutionNamespace)(.+)(\\w*(Service([s]{1})?)\\b)");
    var modelLayerRegex = new Regex(@"(TargetSolutionNamespace)(.+)(\\w*(Model([s]{1})?)\\b)");
    
    var constraints = new ArchitecturalConstraints()
    {
        Layers = new Dictionary<string, NamespacesGroupingMethod>()
        {
            {
                "Services", new NamespacesRegularExpressionGrouped(serviceLayerRegex)
            },
            {
                "Models", new NamespacesRegularExpressionGrouped(modelLayerRegex)
            }
        },
        Rules = new List<ArchitecturalRule>()
        {
            new ArchitecturalRule()
            {
                OriginLayer = "Services",
                RuleOperator = RuleOperator.OnlyCanRelate,
                TargetLayer = "Models"
            }
        }
    };
    
    var solutionFilePath = @"C:\Users\MyUser\Documents\TargetSolution\TargetSolution.sln";
    
    //The IErosionFinderService implementation it's injected by IServiceCollection".AddErosionFinder()" 
    var violations = await erosionFinderService
      .GetViolationsBySolutionFilePathAndConstraintsAsync(
        solutionFilePath, constraints, default);
  }
}
```

<sub>*Icon made by [Freepik](https://www.flaticon.com/authors/freepik) from https://www.flaticon.com*</sub>
