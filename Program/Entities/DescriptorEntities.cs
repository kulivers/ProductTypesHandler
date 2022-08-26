namespace ProductCodeAnalyser.Entities.TypesDescription;

public enum TypeCategory
{
    Enum, // known
    ComplexClass, //known if all fields known
    Primitive, //knonw
    GenericDefinition, // known if from system assembly
    GenericConstructed, //as GenericDefinition + Complex class 
    ActionResult, //throw ex
}

public class PrimitiveDescription
{
    public string TypeName { get; set; }
    public TypeCategory TypeCategory { get; set; }
}

public class EnumDescription : PrimitiveDescription
{
    public Dictionary<string, int> EnumValues { get; }
}

public class GenericDefinitionDescription : PrimitiveDescription
{
    public struct Info
    {
        public string Type { get; set; }
        public bool IsGeneric { get; set; }
    }

    public IEnumerable<KeyValuePair<string, Info>> PublicsDesc { get; set; }
}

public class GenericConstructedDescription : PrimitiveDescription
{
    public string GenericTypeDefinitionDesc { get; set; }
};

public class ActionResultDescription : PrimitiveDescription
{
}

public class ComplexClassDescription : PrimitiveDescription
{
    public IEnumerable<KeyValuePair<string, string>> PublicsDesc { get; set; }
}