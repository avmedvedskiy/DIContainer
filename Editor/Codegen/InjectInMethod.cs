using System.Reflection;
using Editor;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;

namespace DI.Codegen
{
    internal class InjectInMethod
    {
        private readonly TypeDefinition _typeDefinition;
        private readonly MethodDefinition _methodDefinition;
        private readonly ModuleDefinition _moduleDefinition;

        public InjectInMethod(TypeDefinition typeDefinition, MethodDefinition methodDefinition)
        {
            _typeDefinition = typeDefinition;
            _methodDefinition = methodDefinition;
            _moduleDefinition = methodDefinition.Module;
        }

        public void Process()
        {
            var instructions = _methodDefinition.Body.Instructions;
            var oldInstructions = instructions.ToArray();
            instructions.Clear();

            instructions.Add(Instruction.Create(OpCodes.Nop));
            foreach (var parameterDefinition in _methodDefinition.Parameters)
            {
                var resolveMethod = Helpers.MakeGenericMethod(
                    _moduleDefinition.ImportReference(typeof(Dependency).GetMethod("Resolve",
                        BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)),
                    parameterDefinition.ParameterType);

                instructions.Add(Instruction.Create(OpCodes.Call, resolveMethod));
                instructions.Add(Instruction.Create(OpCodes.Starg_S, parameterDefinition));
            }

            foreach (var instruction in oldInstructions)
            {
                instructions.Add(instruction);
            }

            _methodDefinition.Body.OptimizeMacros();
            _typeDefinition.Methods.Remove(_methodDefinition);
        }
    }
}