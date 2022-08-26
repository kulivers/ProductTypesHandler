using System.Diagnostics;
using System.Net.Http.Json;
using ProductCodeAnalyser;
using ProductCodeAnalyser.Entities.TypesDescription;

class Program
{
    public static async Task<IEnumerable<ControllerDescription>?> GetActions()
    {
        var responseMessage = await new HttpPlatformRequester().GetControllers();
        return await responseMessage.Content.ReadFromJsonAsync<IEnumerable<ControllerDescription>>();
    }


    public async Task<PrimitiveDescription?> GetFirstDesc()
    {
        var typeName = "Comindware.Platform.Contracts.AccountModel";
        // typeName = "System.Collections.Generic.List`1";
        var responseMessage = await new HttpPlatformRequester().GetTypeName(typeName);
        var contentString = await responseMessage.Content.ReadAsStringAsync();
        var transformer = new JsonToDescriptionTransformer();
        return await transformer.ParseTypeDescription(contentString);
    }


    public static void SearchStrangeDesc(IEnumerable<ControllerDescription>? controllerDescriptions)
    {
        var sub = "System.Collections.Generic.IDictionary`2[[System.String";
        foreach (var desc in controllerDescriptions)
        {
            if (desc.ReturnType.Contains(sub))
            {
                Debugger.Break();
            }

            var pars = desc.Parameters;
            foreach (var par in pars)
            {
                if (par.TypeName.Contains(sub))
                {
                    Debugger.Break();
                }
            }
        }
    }

    public static async Task Main(string[] args)
    {
        var requester = new HttpPlatformRequester();
        var transformer = new JsonToDescriptionTransformer();
        var typeNameParser = new TypeNameParser();
        var resp = await requester.GetControllers();
        var jsonResp = await resp.Content.ReadAsStringAsync();
        var controllerDescriptions = transformer.ParseControllers(jsonResp);

        var typeNames = new MocksProvider().GetTypeNames(controllerDescriptions);
        foreach (var type in typeNames)
        {
            var info = typeNameParser.Parse(type);
            if (info.IsGenericConstructed)
            {
                var typeDescResponseMessage1 = await requester.GetTypeName(info.GenericDefinitionType);
                var typeDescResponseMessage2 = await requester.GetTypeName(info.GenericParameters.ElementAt(0));
                var jsonResp1 = await typeDescResponseMessage1.Content.ReadAsStringAsync();
                var jsonResp2 = await typeDescResponseMessage2.Content.ReadAsStringAsync();
                var typeDescription1 = await transformer.ParseTypeDescription(jsonResp1);
                var typeDescription2 = await transformer.ParseTypeDescription(jsonResp2);
            }
            else
            {
                var typeDescResponseMessage = await requester.GetTypeName(type);
                jsonResp = await typeDescResponseMessage.Content.ReadAsStringAsync();
                var typeDescription = await transformer.ParseTypeDescription(jsonResp);
            }
        }
    }
}