using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.Compilation;

namespace DI.Codegen.Linker
{
    public static class LinkXmlTempStorage
    {
        private static string GetPathToAllUsedClasses() =>
            Path.Combine(Directory.GetCurrentDirectory(), "Library/AllDiClasses.txt");


        public static void WriteTempFile(string[] classNames) =>
            File.WriteAllLines(GetPathToAllUsedClasses(), classNames);

        public static string[] ReadAllUsedClasses() =>
            File.Exists(GetPathToAllUsedClasses())
                ? File.ReadAllLines(GetPathToAllUsedClasses())
                : Array.Empty<string>();
    }
    
    [InitializeOnLoad]
    public static class LinkXmlGenerator
    {
        static LinkXmlGenerator()
        {
            // Подписываемся на событие завершения компиляции
            CompilationPipeline.compilationFinished += OnCompilationFinished;
        }
        
        private static string GetPathToLinkXml() => Path.Combine(Directory.GetCurrentDirectory(), "Assets/link.xml");

        private static void OnCompilationFinished(object value)
        {
            var linker = UnityEditor.Build.Pipeline.Utilities.LinkXmlGenerator.CreateDefault();
            var allTypes = LinkXmlTempStorage.ReadAllUsedClasses().Select(Type.GetType);
            linker.AddTypes(allTypes);
            linker.Save(GetPathToLinkXml());
        }

        [MenuItem("Tools/Generate Link.xml")] // Опция для ручного запуска генерации
        public static void GenerateLinkXml()
        {
        }
    }
}