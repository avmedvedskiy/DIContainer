using System;
using System.Linq;
using System.Reflection;
using Editor;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
using FieldAttributes = Mono.Cecil.FieldAttributes;

namespace DI.Codegen
{
    internal class InjectPropertyGetterMethod
    {
        private readonly TypeDefinition _typeDefinition;
        private readonly PropertyDefinition _propertyDefinition;
        private readonly MethodReference _getServiceMethodReference;
        private FieldDefinition _backField;

        public InjectPropertyGetterMethod(TypeDefinition typeDefinition, PropertyDefinition propertyDefinition)
        {
            _typeDefinition = typeDefinition;
            _propertyDefinition = propertyDefinition;
            var moduleDefinition = propertyDefinition.Module;

            _getServiceMethodReference = Helpers.MakeGenericMethod(
                moduleDefinition.ImportReference(typeof(Dependency).GetMethod("Resolve",
                    BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)),
                propertyDefinition.PropertyType);
        }

        public void Process()
        {
            //add backing field
            //and change get method to _backFieldValue ??= Dependency.Resolve<T>

            var getMethodDefinition = _propertyDefinition.GetMethod;
            var instructions = getMethodDefinition.Body.Instructions;

            CreateBackingField();

            //foreach (var instruction in instructions)
            //{
            //    Console.WriteLine(instruction);
            //}
            //Console.WriteLine("NEW LINE " + _propertyDefinition.PropertyType);

            instructions.Clear();
            var returnInstruction = Instruction.Create(OpCodes.Ret);
            instructions.Add(Instruction.Create(OpCodes.Ldarg_0));
            instructions.Add(Instruction.Create(OpCodes.Ldfld, _backField));
            instructions.Add(Instruction.Create(OpCodes.Dup));
            instructions.Add(Instruction.Create(OpCodes.Brtrue_S, returnInstruction));

            instructions.Add(Instruction.Create(OpCodes.Pop));
            instructions.Add(Instruction.Create(OpCodes.Ldarg_0));
            instructions.Add(Instruction.Create(OpCodes.Call, _getServiceMethodReference));
            //instructions.Add(Instruction.Create(OpCodes.Dup));
            //instructions.Add(Instruction.Create(OpCodes.Stloc_0));
            instructions.Add(Instruction.Create(OpCodes.Stfld, _backField));
            //instructions.Add(Instruction.Create(OpCodes.Ldloc_0));
            instructions.Add(Instruction.Create(OpCodes.Ldarg_0));
            instructions.Add(Instruction.Create(OpCodes.Ldfld, _backField));
            instructions.Add(returnInstruction);


            getMethodDefinition.Body.OptimizeMacros();

            //foreach (var instruction in instructions)
            //{
            //    Console.WriteLine(instruction);
            //}

            //simple return
            //instructions.Add(Instruction.Create(OpCodes.Call, _getServiceMethodReference));
            //instructions.Add(Instruction.Create(OpCodes.Ret));
        }

        private void CreateBackingField()
        {
            _backField =
                _typeDefinition.Fields.FirstOrDefault(x => x.Name == $"<{_propertyDefinition.Name}>k__BackingField");
            if (_backField != null)
                return;

            _backField = new FieldDefinition($"<{_propertyDefinition.Name}>k__BackingField", FieldAttributes.Private,
                _propertyDefinition.PropertyType);
            _typeDefinition.Fields.Add(_backField);
        }
    }
}