using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using ProductCodeAnalyser.Entities;
using ProductCodeAnalyser.Entities.TypesDescription;

public class JsonToDescriptionTransformer
{
    private TypeCategory ParseTypeCategory(string json)
    {
        var typeDescription = JsonConvert.DeserializeObject<PrimitiveDescription>(json,
            new StringEnumConverter());
        return typeDescription.TypeCategory;
    }

    public IEnumerable<ControllerDescription>? ParseControllers(string json)
    {
        var typeDescriptions = JsonConvert.DeserializeObject<IEnumerable<ControllerDescription>>(json);
        return typeDescriptions;
    }

    public async Task<PrimitiveDescription?> ParseTypeDescription(string jsonString)
    {
        var category = ParseTypeCategory(jsonString);
        return category switch
        {
            TypeCategory.ComplexClass => JsonConvert
                .DeserializeObject<ComplexClassDescription>(jsonString,
                    new StringEnumConverter()),
            TypeCategory.Enum => JsonConvert.DeserializeObject<EnumDescription>(
                jsonString, new StringEnumConverter()),
            TypeCategory.Primitive => JsonConvert
                .DeserializeObject<PrimitiveDescription>(jsonString, new StringEnumConverter()),
            TypeCategory.GenericConstructed => JsonConvert
                .DeserializeObject<GenericConstructedDescription>(jsonString,
                    new StringEnumConverter()),
            TypeCategory.GenericDefinition => JsonConvert
                .DeserializeObject<GenericDefinitionDescription>(jsonString,
                    new StringEnumConverter()),
            TypeCategory.ActionResult => throw new Exception("Action result return is not supported"),
            _ => throw new Exception("Type category not identified")
        };
    }
}