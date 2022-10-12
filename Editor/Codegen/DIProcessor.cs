using System;
using System.Collections.Generic;
using Editor;
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
            }

            return diagnosticMessages;
        }

        bool Process(TypeDefinition typeDefinition, PropertyDefinition propertyDefinition)
        {
            var injectAttribute = Helpers.GetCustomAttribute<InjectAttribute>(propertyDefinition);
            if (injectAttribute == null)
            {
                return false;
            }


            Console.WriteLine($"Custom Attribute {propertyDefinition.Name}");
            new InjectPropertyGetterMethod(typeDefinition, propertyDefinition).Process();
            return true;
        }

        bool Process(TypeDefinition typeDefinition, FieldDefinition fieldDefinition)
        {
            var injectAttribute = Helpers.GetCustomAttribute<InjectAttribute>(fieldDefinition);
            if (injectAttribute == null)
            {
                return false;
            }

            Console.WriteLine($"Custom Field {fieldDefinition.Name}");
            new InjectFieldGetterMethod(typeDefinition, fieldDefinition).Process();
            return true;
        }
    }
}