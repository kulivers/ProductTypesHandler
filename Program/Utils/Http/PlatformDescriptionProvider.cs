namespace ProductCodeAnalyser;

public class PlatformDescriptionProvider
{
    public HttpPlatformRequester Requester { get; set; }
    public JsonToDescriptionTransformer Transformer { get; set; }
    public PlatformDescriptionProvider()
    {
        Requester = new HttpPlatformRequester();
        Transformer = new JsonToDescriptionTransformer();
    }

    public async Task<IEnumerable<ControllerDescription>?> GetControllerModels()
    {
        var responseMessage =await Requester.GetControllers();
        var jsonString = await responseMessage.Content.ReadAsStringAsync();
        return Transformer.ParseControllers(jsonString);
    }
    
}