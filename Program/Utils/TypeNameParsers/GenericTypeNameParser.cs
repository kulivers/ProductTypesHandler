public class GenericTypeNameInfo
{
    public string Definition { get;  set; }
    public IEnumerable<string> Parameters { get;  set; }
}
public static class GenericTypeNameParser
{
    public static GenericTypeNameInfo Parse(string genericFullName)
    {
        var genTnInfo = new GenericTypeNameInfo();
        var firstBracketIndex = genericFullName.IndexOf('[');
        genTnInfo.Definition = string.Concat(genericFullName.Take(firstBracketIndex));

        var genericsPart = string.Concat(genericFullName.Skip(firstBracketIndex)); //delete typename
        genericsPart = genericsPart.Substring(1, genericsPart.Length - 2);
        // [System.Collections.Generic.IDictionary`2[[System.String, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089],[System.Object, mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]][], mscorlib, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089]
        var parts = new List<string>();
        var part = new List<char>();
        var genericStartsFlag = false;
        var genericEndsFlag = false;
        for (var index = 0; index < genericsPart.Length; index++)
        {
            var c = genericsPart[index];
            switch (c)
            {
                case '[':
                    try
                    {
                        if (genericsPart[index + 1] == ']')
                            break;
                        genericEndsFlag = false;
                        genericStartsFlag = true;
                        continue;
                    }
                    catch
                    {
                        genericEndsFlag = false;
                        genericStartsFlag = true;
                        continue;
                    }

                //generic description starts
                case ']':
                    if (genericsPart[index - 1] == '[')
                        break;
                    genericStartsFlag = false;
                    genericEndsFlag = true;
                    break;

                case ',':
                    continue;
            }

            if (genericStartsFlag) part.Add(c);
            if (genericEndsFlag)
            {
                parts.Add(string.Concat(new List<char>(part)));
                part.Clear();
            }
        }

        //parsed parts, then get types
        genTnInfo.Parameters = parts.Select(desc => desc.Split(' ')[0]);
        return genTnInfo;
    }
}