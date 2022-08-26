using Newtonsoft.Json;

namespace ProductCodeAnalyser;

public class MocksProvider
{
    public HashSet<string> GetTypeNames(IEnumerable<ControllerDescription> controllerModels)
    {
        var types = controllerModels.Select(m => m.ReturnType).ToHashSet();
        var parameters = controllerModels.Select(model => model.Parameters);
        foreach (var parameterInfos in parameters)
        {
            foreach (var info in parameterInfos) 
                types.Add(info.TypeName);
        }

        return types;
    }

    public List<ControllerDescription>? GetMockControllerModels(string? jsonPath = null)
    {
        jsonPath ??= @"D:\Playground\ProductCodeAnalyser\Program\Mocks\ControllersResponseSnapshot.json";
        using var r = new StreamReader(jsonPath);
        var json = r.ReadToEnd();
        return JsonConvert.DeserializeObject<List<ControllerDescription>>(json);
    }
}