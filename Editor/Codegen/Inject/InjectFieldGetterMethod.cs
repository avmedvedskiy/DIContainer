using System;
using System.Linq;
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

            _getServiceMethodReference = moduleDefinition.GetDependencyResolveMethod(fieldDefinition.FieldType);
        }

        public void Process()
        {
            if (_typeDefinition.IsMonoBehaviourClass())
            {
                InjectInAwake();
            }
            else
            {
                InjectInConstructor();
            }
        }

        private void InjectInAwake()
        {
            Console.WriteLine($"Inject in mono class {_typeDefinition.FullName}");
            var methodDefinition = GetOrCreateMethodDefinition("Awake");
            InjectResolveInstructions(methodDefinition);
        }

        private MethodDefinition GetOrCreateMethodDefinition(string methodName)
        {
            var methodDefinition = _typeDefinition.Methods.FirstOrDefault(x => x.Name == methodName);
            if (methodDefinition == null)
            {
                //lets add own Awake
                methodDefinition = new MethodDefinition(methodName, MethodAttributes.Private,
                    _typeDefinition.Module.ImportReference(typeof(void)));
                var instructions = methodDefinition.Body.Instructions;
                var parentMethod = GetPublicParentMethod(methodName);
                if (parentMethod != null)
                {
                    Console.WriteLine($"Parent method find {parentMethod}");
                    instructions.Add(Instruction.Create(OpCodes.Ldarg_0));
                    instructions.Add(Instruction.Create(OpCodes.Call, parentMethod));
                }

                instructions.Add(Instruction.Create(OpCodes.Nop));
                instructions.Add(Instruction.Create(OpCodes.Ret));
                methodDefinition.Body.OptimizeMacros();
                //instructions.Write();
                _typeDefinition.Methods.Add(methodDefinition);
            }

            return methodDefinition;
        }

        private MethodDefinition GetPublicParentMethod(string methodName)
        {
            var parentMethod = _typeDefinition.BaseType.Resolve().FindParentMethod(methodName);
            if (parentMethod is { IsPrivate: true })
                //lets create new public method
            {
                var newPublicParentMethod = new MethodDefinition($"{methodName}_Invoke", MethodAttributes.Public,
                    _typeDefinition.Module.ImportReference(typeof(void)));
                var instructions = newPublicParentMethod.Body.Instructions;
                instructions.Add(Instruction.Create(OpCodes.Ldarg_0));
                instructions.Add(Instruction.Create(OpCodes.Call, parentMethod));
                instructions.Add(Instruction.Create(OpCodes.Nop));
                instructions.Add(Instruction.Create(OpCodes.Ret));
                
                parentMethod.DeclaringType.Methods.Add(newPublicParentMethod);
                parentMethod = newPublicParentMethod;
            }

            return parentMethod;
        }

        private void InjectInConstructor()
        {
            foreach (var cctor in _typeDefinition.Methods.Where(m => m.IsConstructor))
            {
                InjectResolveInstructions(cctor);
            }
        }

        private void InjectResolveInstructions(MethodDefinition methodDefinition)
        {
            if(methodDefinition.IsStatic != _fieldDefinition.IsStatic)
                return;
            
            var instructions = methodDefinition.Body.Instructions;
            instructions.Insert(0, Instruction.Create(OpCodes.Stfld, _fieldDefinition));
            instructions.Insert(0, Instruction.Create(OpCodes.Call, _getServiceMethodReference));
            instructions.Insert(0, Instruction.Create(OpCodes.Ldarg_0));

            methodDefinition.Body.OptimizeMacros();
        }
    }
}