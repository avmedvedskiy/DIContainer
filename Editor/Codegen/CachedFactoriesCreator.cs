using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Cecil;
using Mono.Cecil.Cil;
using Mono.Cecil.Rocks;
using UnityEngine;
using MethodAttributes = Mono.Cecil.MethodAttributes;

namespace DI.Codegen
{
    /*
    public static class CachedFactoriesCreator
    {
        public static void Create(List<TypeDefinition> usedClasses, AssemblyDefinition assemblyDefinition)
        {
            var cachedFactories = assemblyDefinition.AddStaticClass("GeneratedCachedFactoriesForContainer");
            var method = AddRuntimeInitializeOnLoadMethod(cachedFactories, assemblyDefinition);
            AddInstructions(method, usedClasses,cachedFactories, assemblyDefinition);
        }

        private static void AddInstructions(MethodDefinition method, List<TypeDefinition> usedClasses,
            TypeDefinition typeDefinition, AssemblyDefinition assembly)
        {
            //var debugType = assembly.MainModule.ImportReference(typeof(Debug)).Resolve();
            var module = assembly.MainModule;
            var cachedDictionaryField =
                module.ImportReference(typeof(Factory).GetField(nameof(Factory.CachedFactories)));
            
            var funcType = typeof(Func<>).MakeGenericType(typeof(object));
            var funcCtor = funcType.GetConstructor(new[] { typeof(object), typeof(IntPtr) });
            
            //var funcType = module.ImportReference(typeof(Func<object>));
            //var funcCtor = funcType.Resolve().GetConstructors().First(c => c.Parameters.Count == 2 && c.Parameters[0].ParameterType.FullName == "System.Object" && c.Parameters[1].ParameterType.FullName == "System.IntPtr");

            var dictionaryType = typeof(Dictionary<Type, Func<object>>);

            Console.WriteLine($"funcCtor = {funcCtor}");
            
            //Create new method for initialize Factories Dictionary
            // Clear static dictionary for Fast Play mode in unity
            var instructions = method.Body.Instructions;
            instructions.Clear();
            instructions.Add(Instruction.Create(OpCodes.Nop));
            instructions.Add(Instruction.Create(OpCodes.Ldsfld, cachedDictionaryField));
            instructions.Add(Instruction.Create(OpCodes.Callvirt, module.ImportReference(dictionaryType.GetMethod("Clear"))));
            instructions.Add(Instruction.Create(OpCodes.Nop));
            
            
            //create Create method for every class
            //private static object Get() => new CurrencyService();
            for (int i = 0; i < usedClasses.Count; i++)
            {
                var type = module.ImportReference(usedClasses[i]);
                var typeCtor = usedClasses[i].GetConstructors().First();
            
                var newGeneratedMethod = new MethodDefinition($"Create_{type.FullName.Replace('.','_')}", MethodAttributes.Private | MethodAttributes.Static, module.ImportReference(typeof(object)));
                var gInstructions = newGeneratedMethod.Body.Instructions;
                gInstructions.Clear();
                //need to add Instruction.Create(OpCodes.Ldnull) for every parameter in constructor
                for (int j = 0; j < typeCtor.Parameters.Count; j++)
                {
                    gInstructions.Add(Instruction.Create(OpCodes.Ldnull));
                }
                gInstructions.Add(Instruction.Create(OpCodes.Newobj, module.ImportReference(typeCtor)));
                gInstructions.Add(Instruction.Create(OpCodes.Ret));
                typeDefinition.Methods.Add(newGeneratedMethod);


                instructions.Add(Instruction.Create(OpCodes.Ldsfld, cachedDictionaryField));
                //and add Create Call to the Factories dictionary
                //Factory.CachedFactories.Add(typeof(T), Create);
                instructions.Add(Instruction.Create(OpCodes.Ldtoken, type));
                instructions.Add(Instruction.Create(OpCodes.Call, module.ImportReference(typeof(Type).GetMethod("GetTypeFromHandle"))));
                instructions.Add(Instruction.Create(OpCodes.Ldnull));
            
                instructions.Add(Instruction.Create(OpCodes.Ldftn, newGeneratedMethod));
                instructions.Add(Instruction.Create(OpCodes.Newobj, module.ImportReference(funcCtor)));
                instructions.Add(Instruction.Create(OpCodes.Callvirt, module.ImportReference(dictionaryType.GetMethod("Add"))));

            }
            
            //end method
            instructions.Add(Instruction.Create(OpCodes.Nop));
            instructions.Add(Instruction.Create(OpCodes.Ret));
            method.Body.OptimizeMacros();
            //instructions.Write();
        }

        private static MethodDefinition AddRuntimeInitializeOnLoadMethod(TypeDefinition typeDefinition, AssemblyDefinition assembly)
        {
            var runtimeInitializeOnLoadMethod = new MethodDefinition(
                "RuntimeInitializeOnLoadMethod",
                MethodAttributes.Static | MethodAttributes.Public | MethodAttributes.HideBySig,
                typeDefinition.Module.ImportReference(typeof(void)));
            AddRuntimeInitializeOnLoadAttribute(runtimeInitializeOnLoadMethod, assembly);
            typeDefinition.Methods.Add(runtimeInitializeOnLoadMethod);
            return runtimeInitializeOnLoadMethod;
        }

        private static void AddRuntimeInitializeOnLoadAttribute(MethodDefinition methodDefinition, AssemblyDefinition assembly)
        {
            var attribute = assembly.CreateCustomAttribute<RuntimeInitializeOnLoadMethodAttribute>((x)=> x.Parameters.Count == 1);
            
            attribute.ConstructorArguments.Add(new CustomAttributeArgument(
                assembly.MainModule.ImportReference(typeof(RuntimeInitializeLoadType)), RuntimeInitializeLoadType.SubsystemRegistration));

            methodDefinition.CustomAttributes.Add(attribute);
        }
    }
    */
}