public class TypeNameInfo
{
    public string TypeName { get; private set; }
    public bool IsSystem { get; private set; }
    public bool IsArray { get; private set; }
    public bool IsGeneric { get; private set; }
    public bool IsGenericConstructed { get; private set; }
    public string GenericDefinitionType { get; private set; }
    public IEnumerable<string> GenericParameters { get; private set; }
    public bool IsNestedGeneric { get; set; }


    public TypeNameInfo(bool isNestedGeneric)
    {
        IsNestedGeneric = isNestedGeneric;
    }

    public TypeNameInfo(string typeName, bool isSystem, bool isArray, bool isGeneric, bool isGenericConstructed,
        string genericDefinitionType, IEnumerable<string> genericParameters, bool isNestedGeneric)
    {
        TypeName = typeName;
        IsSystem = isSystem;
        IsArray = isArray;
        IsGeneric = isGeneric;
        IsGenericConstructed = isGenericConstructed;
        GenericDefinitionType = genericDefinitionType;
        GenericParameters = genericParameters;
        IsNestedGeneric = isNestedGeneric;
    }
}

public class TypeNameParser
{
    private string TypeName { get; set; }
    private bool IsSystem { get; set; }

    private bool IsArray { get; set; }
    private bool IsGeneric { get; set; }
    private bool IsGenericConstructed { get; set; }
    private string GenericDefinitionType { get; set; }
    private IEnumerable<string> GenericParameters { get; set; }

    private bool IsNestedGeneric { get; set; }

    public TypeNameInfo Parse(string typeName)
    {
        TypeName = typeName;
        CheckIsSystem();
        CheckIsArray();
        CheckIsGeneric();
        CheckIsGenericConstructed();
        ParseGenericParts();
        return new TypeNameInfo(TypeName, IsSystem, IsArray, IsGeneric, IsGenericConstructed, GenericDefinitionType,
            GenericParameters, IsNestedGeneric);
    }

    private void CheckIsSystem() => IsSystem = typeof(int).Assembly.GetType(TypeName) != null;
    private void CheckIsArray() => IsArray = TypeName.EndsWith("[]");
    private void CheckIsGeneric() => IsGeneric = TypeName.Contains('`');
    private void CheckIsGenericConstructed() => IsGenericConstructed = IsGeneric && TypeName.Contains('[');

    private void ParseGenericParts()
    {
        if (!IsGeneric)
            return;
        if (!IsGenericConstructed)
        {
            GenericDefinitionType = TypeName;
            return;
        }

        if (TypeName.Count(c => c == '`') > 1)
        {
            IsNestedGeneric = true;
            return;
        }

        var parser = GenericTypeNameParser.Parse(TypeName);
        GenericDefinitionType = parser.Definition;
        GenericParameters = parser.Parameters;
    }
}


