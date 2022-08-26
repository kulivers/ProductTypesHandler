using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Newtonsoft.Json;

class Example
{
    static void LocalMain()
    {
        var tn = typeof(WebApiResponse<IDictionary<string, object>>).FullName;

        tn =
            "[" +
            "[" +
            "System.Collections.Generic.IDictionary`2" +
            "[" +
            "[System.String, System.Private.CoreLib, Version=6.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e]," +
            "[System.Object, System.Private.CoreLib, Version=6.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e]" +
            "], System.Private.CoreLib, Version=6.0.0.0, Culture=neutral, PublicKeyToken=7cec85d7bea7798e" +
            "]" +
            "]";

        var blocks = ParseBlocks(tn);
        var stringBlocks = new List<string>();
        foreach (var block in blocks)
        {
            var sub = tn.Substring(block.IStart, block.IEnd - block.IStart + 1);
            stringBlocks.Add(sub);
        }
    }

    static HashSet<Block> ParseBlocks(string tn)
    {
        var blocks = new HashSet<Block>();
        var bracketsStack = new Stack<KeyValuePair<int, bool>>();
        KeyValuePair<int, bool> lastFront;
        KeyValuePair<int, bool> lastBack;
        for (var i = 0; i < tn.Length; i++)
        {
            var c = tn[i];
            if (c == ']')
            {
                lastFront = bracketsStack.Pop();
                lastBack = new KeyValuePair<int, bool>(i, false);
                blocks.Add(new Block(lastFront.Key, lastBack.Key, bracketsStack.Count));
            }

            if (c == '[') bracketsStack.Push(new KeyValuePair<int, bool>(i, true));
        }

        return blocks;
    }
}

public class Block
{
    public Block(int start, int end, int level)
    {
        IStart = start;
        IEnd = end;
        Level = level;
    }

    public int IStart { get; }
    public int IEnd { get; }
    public int Level { get; }

    public override int GetHashCode() =>
        IStart + IEnd
               + Level;
}


[DataContract]
[Serializable]
public class WebApiResponse<T,TU>
{
    /// <summary>
    /// Request result.
    /// </summary>
    [DataMember]
    [JsonProperty(NullValueHandling = NullValueHandling.Include)]
    public virtual T Response { get; set; }
    public virtual TU Response2 { get; set; }

    /// <summary>
    /// Errors list.
    /// </summary>
    [DataMember]
    public WebApiError Error { get; set; }

    /// <summary>
    /// Success.
    /// </summary>
    [DataMember]
    public bool Success { get; set; }
}
public class WebApiResponse<T>
{
    /// <summary>
    /// Request result.
    /// </summary>
    [DataMember]
    [JsonProperty(NullValueHandling = NullValueHandling.Include)]
    public virtual T Response { get; set; }

    /// <summary>
    /// Errors list.
    /// </summary>
    [DataMember]
    public WebApiError Error { get; set; }

    /// <summary>
    /// Success.
    /// </summary>
    [DataMember]
    public bool Success { get; set; }
}

[DataContract]
public class WebApiError
{
    /// <summary>
    /// Exception message.
    /// </summary>
    [DataMember]
    public string Message { get; set; }

    /// <summary>
    /// Exception type.
    /// </summary>
    [DataMember]
    public string Type { get; set; }

    /// <summary>
    /// Inner errors.
    /// </summary>
    [DataMember]
    public WebApiError Inner { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="WebApiError"/> class.
    /// </summary>
    /// <param name="exception">Exception.</param>
    public WebApiError(Exception exception)
    {
        if (exception != null)
        {
            Type = exception.GetType().Name;
            Message = exception.Message;
        }
    }
}