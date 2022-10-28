using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
using Mono.Collections.Generic;

namespace DI.Codegen
{
    internal class InjectPropertyDirectGetterMethod
    {
        private readonly PropertyDefinition _propertyDefinition;
        private readonly MethodReference _getServiceMethodReference;
        private FieldDefinition _backField;

        public InjectPropertyDirectGetterMethod(TypeDefinition typeDefinition, PropertyDefinition propertyDefinition)
        {
            _propertyDefinition = propertyDefinition;
            var moduleDefinition = propertyDefinition.Module;

            _getServiceMethodReference = moduleDefinition.GetDependencyResolveMethod(propertyDefinition.PropertyType);
        }

        public void Process()
        {
            var getMethodDefinition = _propertyDefinition.GetMethod;
            Collection<Instruction> instructions = getMethodDefinition.Body.Instructions;
            instructions.Clear();

            instructions.Add(Instruction.Create(OpCodes.Call, _getServiceMethodReference));
            instructions.Add(Instruction.Create(OpCodes.Ret));
            getMethodDefinition.Body.OptimizeMacros();
        }
    }
}