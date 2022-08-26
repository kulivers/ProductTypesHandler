public class ControllerDescription
{
    public string ReturnType { get; set; }
    public string ClassName { get; set; }
    public string MethodName { get; set; }
    public IEnumerable<JsApiActionParameterInfo> Parameters { get; set; }
    public string Url { get; set; }
    public string HttpMethod { get; set; }
}
public class JsApiActionParameterInfo
{
    public JsApiActionParameterInfo(string name, string typeName)
    {
        this.Name = name;
        this.TypeName = typeName;
    }

    public string Name { get; private set; }
    public string TypeName { get; private set; }
}
