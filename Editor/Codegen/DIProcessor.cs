using System;
using System.Collections.Generic;
using Mono.Cecil;
using Mono.Cecil.Rocks;
using Unity.CompilationPipeline.Common.Diagnostics;

namespace DI.Codegen
{
    public class DIProcessor
    {
        private ModuleDefinition _module;
        private bool _hasInjectAttributes;

        public List<DiagnosticMessage> Process(AssemblyDefinition assembly, out bool isChanged)
        {
            isChanged = false;
            List<DiagnosticMessage> diagnosticMessages = new List<DiagnosticMessage>();

            _module = assembly.MainModule;

            var allTypes = _module
                .GetAllTypes();

            foreach (var type in allTypes)
            {
                foreach (PropertyDefinition property in type.Properties)
                {
                    isChanged |= Process(type, property);
                }

                foreach (FieldDefinition field in type.Fields)
                {
                    isChanged |= Process(type, field);
                }

                foreach (var method in type.Methods)
                {
                    isChanged |= Process(type, method);
                }
            }

            return diagnosticMessages;
        }

        private bool Process(TypeDefinition typeDefinition, MethodDefinition methodDefinition)
        {
            var injectAttribute = methodDefinition.GetCustomAttribute<InjectAttribute>();
            if (injectAttribute == null)
            {
                return false;
            }

            Console.WriteLine($"Custom Method Attribute {methodDefinition.Name}");
            new InjectInMethod(typeDefinition, methodDefinition).Process();
            return true;
        }

        private bool Process(TypeDefinition typeDefinition, PropertyDefinition propertyDefinition)
        {
            var injectAttribute = propertyDefinition.GetCustomAttribute<InjectAttribute>();
            if (injectAttribute == null)
            {
                return false;
            }

            Console.WriteLine($"Custom Property Attribute {propertyDefinition.Name}");
            new InjectPropertyLazyGetterMethod(typeDefinition, propertyDefinition).Process();
            return true;
        }

        private bool Process(TypeDefinition typeDefinition, FieldDefinition fieldDefinition)
        {
            var injectAttribute = fieldDefinition.GetCustomAttribute<InjectAttribute>();
            if (injectAttribute == null)
            {
                return false;
            }

            Console.WriteLine($"Custom Field Attribute {fieldDefinition.Name}");
            new InjectFieldGetterMethod(typeDefinition, fieldDefinition).Process();
            return true;
        }
    }
}