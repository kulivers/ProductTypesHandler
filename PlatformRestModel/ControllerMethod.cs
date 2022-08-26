using System;
using System.Collections.Generic;


public class Method
{
    public Type ReturnType { get; set; }
    public string ClassName { get; set; }
    public string MethodName { get; set; }
    public string ControllerRoute { get; set; }
    public IEnumerable<JsApiActionParameterInfo> Parameters { get; set; }
    public string Url { get; set; }
    public string HttpMethod { get; set; }
    public ControllerProtocol Protocol { get; set; }
    
}
public class MethodDescriptionStr
{
    public string ReturnType { get; set; }
    public string ClassName { get; set; }
    public string MethodName { get; set; }
    public IEnumerable<JsApiActionParameterInfo> Parameters { get; set; }
    public string Url { get; set; }
    public string HttpMethod { get; set; }
    public ControllerProtocol Protocol { get; set; }
}


public enum ControllerProtocol
{
    Unknown = 0,
    Mvc,
    WebApi
}