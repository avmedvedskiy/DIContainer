﻿using System.Linq;
using Editor;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;

namespace DI.Codegen
{
    internal class InjectFieldGetterMethod
    {
        private readonly TypeDefinition _typeDefinition;
        private readonly FieldDefinition _fieldDefinition;
        private readonly MethodReference _getServiceMethodReference;

        public InjectFieldGetterMethod(TypeDefinition typeDefinition, FieldDefinition fieldDefinition)
        {
            _typeDefinition = typeDefinition;
            _fieldDefinition = fieldDefinition;
            var moduleDefinition = fieldDefinition.Module;

            _getServiceMethodReference = Helpers.MakeGenericMethod(
                moduleDefinition.ImportReference(typeof(Dependency).GetMethod("Resolve")),
                new[] { fieldDefinition.FieldType });
        }

        public void Process()
        {
            foreach (var cctor in _typeDefinition.Methods.Where(m => m.Name == ".ctor"))
            {
                var instructions = cctor.Body.Instructions;

                instructions.Insert(0, Instruction.Create(OpCodes.Stfld, _fieldDefinition));
                instructions.Insert(0, Instruction.Create(OpCodes.Call, _getServiceMethodReference));
                instructions.Insert(0, Instruction.Create(OpCodes.Ldarg_0));

                //foreach (var instruction in instructions)
                //{
                //    Console.WriteLine(instruction);
                //}

                cctor.Body.OptimizeMacros();
            }
        }
    }
}