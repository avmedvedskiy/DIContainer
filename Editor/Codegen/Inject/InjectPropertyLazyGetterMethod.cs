using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
using Mono.Collections.Generic;
using FieldAttributes = Mono.Cecil.FieldAttributes;

namespace DI.Codegen
{
    internal class InjectPropertyLazyGetterMethod
    {
        private readonly TypeDefinition _typeDefinition;
        private readonly PropertyDefinition _propertyDefinition;
        private readonly MethodReference _getServiceMethodReference;
        private FieldDefinition _backField;

        public InjectPropertyLazyGetterMethod(TypeDefinition typeDefinition, PropertyDefinition propertyDefinition)
        {
            _typeDefinition = typeDefinition;
            _propertyDefinition = propertyDefinition;
            var moduleDefinition = propertyDefinition.Module;

            _getServiceMethodReference = moduleDefinition.GetDependencyResolveMethod(propertyDefinition.PropertyType);
        }

        public void Process()
        {
            //add backing field
            //and change get method to _backFieldValue ??= Dependency.Resolve<T>

            var getMethodDefinition = _propertyDefinition.GetMethod;
            Collection<Instruction> instructions = getMethodDefinition.Body.Instructions;
            CreateBackingField();

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