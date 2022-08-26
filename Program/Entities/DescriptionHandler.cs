using ProductCodeAnalyser.Entities;
using ProductCodeAnalyser.Entities.TypesDescription;

namespace ProductCodeAnalyser;

public class DescriptionHandler
{
    public Type? GetType(PrimitiveDescription description)
    {
        var type = description.TypeCategory switch
        {
            TypeCategory.Primitive => HandlePrimitive(description),
            TypeCategory.ComplexClass => HandleComplexClass((ComplexClassDescription)description),
            TypeCategory.Enum => HandleEnum((EnumDescription)description),
            TypeCategory.GenericConstructed => HandleGenericConstructed((GenericConstructedDescription)description),
            TypeCategory.GenericDefinition => HandleGenericDefinition((GenericDefinitionDescription)description),
            TypeCategory.ActionResult => throw new Exception("ActionResult types not supported"),
            _ => throw new InvalidOperationException("Cant handle type category")
        };
        return type;
    }

    private Type? HandlePrimitive(PrimitiveDescription description)
    {
        var ass = typeof(int).Assembly;
        return ass.GetType(description.TypeName);
    }

    private Type? HandleEnum(EnumDescription description)
    {
        throw new NotImplementedException();
    }

    private Type? HandleComplexClass(ComplexClassDescription description)
    {
        //if all props are known = create type
        //if not = create not known before 
        throw new NotImplementedException();

    }

    private Type? HandleGenericDefinition(GenericDefinitionDescription description)
    {
        throw new NotImplementedException();
    }

    private Type? HandleGenericConstructed(GenericConstructedDescription description)
    {
        throw new NotImplementedException();
    }
}