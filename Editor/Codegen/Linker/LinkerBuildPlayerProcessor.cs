using System;
using System.IO;
using System.Linq;
using UnityEditor.Build;

namespace DI.Codegen.Linker
{

    public class LinkerBuildPlayerProcessor : BuildPlayerProcessor
    {
        private static string GetPathToAllUsedClasses() =>
            Path.Combine(Directory.GetCurrentDirectory(), "Library/AllDiClasses.txt");

        private static string GetPathToLinkXml() => Path.Combine(Directory.GetCurrentDirectory(), "Assets/link.xml");

        public static void WriteTempFile(string[] classNames) =>
            File.WriteAllLines(GetPathToAllUsedClasses(), classNames);

        private static string[] ReadAllUsedClasses() =>
            File.Exists(GetPathToAllUsedClasses())
                ? File.ReadAllLines(GetPathToAllUsedClasses())
                : Array.Empty<string>();
        
        public override void PrepareForBuild(BuildPlayerContext buildPlayerContext)
        {
            var linker = UnityEditor.Build.Pipeline.Utilities.LinkXmlGenerator.CreateDefault();
            var allTypes = ReadAllUsedClasses().Select(Type.GetType);
            linker.AddTypes(allTypes);
            linker.Save(GetPathToLinkXml());
        }
    }
}