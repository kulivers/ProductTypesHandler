namespace ProductCodeAnalyser;

public class TypesContainer
{
    public HashSet<Type> Container { get; private set; }

    public TypesContainer()
    {
        Container = new HashSet<Type>();
    }

    private Type? FindInAssemblies(string typeName)
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies();
        Type? type = null;
        foreach (var assembly in assemblies)
        {
            type = assembly.GetType(typeName);
            if (type != null)
                return type;
        }

        return type;
    }

    public void Add(Type type)
    {
        Container.Add(type);
    }

    public Type? Get(string typeName)
    {
        Type? type = null;
        var inC = Container.FirstOrDefault(t => t.Name == typeName || t.FullName == typeName);
        return inC != null ? type : FindInAssemblies(typeName);
    }
}