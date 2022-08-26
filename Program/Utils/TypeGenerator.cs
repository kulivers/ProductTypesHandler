using System.Reflection;
using System.Reflection.Emit;

namespace ProductCodeAnalyser;

public class TypeCreator
{
    public TypeCreator()
    {
        var aName = new AssemblyName("GeneratedAssembly");
        var assBuilder = AssemblyBuilder.DefineDynamicAssembly(aName, AssemblyBuilderAccess.Run);
        ModuleBuilder = assBuilder.DefineDynamicModule(aName.Name);
    }
    private ModuleBuilder ModuleBuilder { get; }
    
    public Type? CreateEnumType(string name, Dictionary<string, int> values, string? @namespace = "")
    {
        @namespace = string.IsNullOrEmpty(@namespace) ? @namespace : $"{@namespace}.";
        var eb = ModuleBuilder.DefineEnum($"{@namespace}{name}", TypeAttributes.Public, typeof(int));
        foreach (var (s, value) in values) eb.DefineLiteral(s, value);
        return eb.CreateType();
    }
    public Type? CreateGenericConstructed(Type genericDefinition, IEnumerable<Type> genericParameters) =>
        genericDefinition.MakeGenericType(genericParameters.ToArray());

    public Type? CreateComplexType(string name, Dictionary<string, Type> fields, string? @namespace = "",
        Type? parent = null)
    {
        @namespace = string.IsNullOrEmpty(@namespace) ? @namespace : $"{@namespace}.";
        var eb = ModuleBuilder.DefineType($"{@namespace}{name}", TypeAttributes.Public, parent);
        foreach (var (fieldName, type) in fields) eb.DefineField(fieldName, type, FieldAttributes.Public);
        return eb.CreateType();
    }

    public Type? CreateGenericDefinition(string name, Dictionary<string, Type> fields,
        IEnumerable<GenericField> genericFields,
        string? @namespace = "", Type? parent = null)
    {
        @namespace = string.IsNullOrEmpty(@namespace) ? @namespace : $"{@namespace}.";
        var tb = ModuleBuilder.DefineType($"{@namespace}{name}", TypeAttributes.Public, parent);
        var genericArguments = genericFields.Select(field => field.GenericArgument).Distinct().ToArray();
        tb.DefineGenericParameters(genericArguments);
        foreach (var gField in genericFields)
        {
            var genericParameterType = tb.GenericTypeParameters.First(p => p.Name == gField.GenericArgument);
            tb.DefineField(gField.Name, genericParameterType, FieldAttributes.Public);
        }

        foreach (var (fieldName, type) in fields)
            tb.DefineField(fieldName, type, FieldAttributes.Public);

        return tb.CreateType();
    }

    public struct GenericField
    {
        public readonly string Name;
        public readonly string GenericArgument; // T/U for example

        public GenericField(string name, string genericArgument)
        {
            Name = name;
            GenericArgument = genericArgument;
        }
    }
}