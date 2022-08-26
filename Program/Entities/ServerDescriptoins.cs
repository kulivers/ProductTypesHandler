using System.Collections;
using System.Reflection;
using System.Web.Mvc;

namespace ProductCodeAnalyser.Entities;

class ServerDescriptors //todo delete if not used
{
    public class TypeDescription
    {
        public string TypeName { get; set; }
        public TypeCategory TypeCategory { get; set; }

        public TypeDescription(string typeName, TypeCategory typeCategory)
        {
            TypeName = typeName;
            TypeCategory = typeCategory;
        }
    }

    public class EnumDescription : TypeDescription
    {
        public Dictionary<string, int> EnumValues { get; }

        public EnumDescription(Type enumType) : base(enumType.Name, TypeCategory.Enum)
        {
            EnumValues = new Dictionary<string, int>();
            foreach (var enumValue in enumType.GetEnumValues())
            {
                var name = enumValue.ToString();
                var value = (int)enumValue;
                EnumValues.Add(name, value);
            }
        }
    }

    public class GenericDefinitionDescription : TypeDescription
    {
        public struct Info
        {
            public string Type { get; set; }
            public bool IsGeneric { get; set; }

            public Info(string type, bool isGeneric)
            {
                Type = type;
                IsGeneric = isGeneric;
            }
        }

        private IEnumerable<Type> GenericTypes { get; }

        // separated cuz we need to know that some prop/field is GenericType   (PropertyType.IsGenericType or for field)
        private HashSet<KeyValuePair<string, PropertyInfo>> Props { get; }
        private HashSet<KeyValuePair<string, FieldInfo>> Fields { get; }

        public IEnumerable<KeyValuePair<string, Info>> PublicsDesc
        {
            get
            {
                var propsDesc = Props.Select(p =>
                    new KeyValuePair<string, Info>(p.Key,
                        new Info(p.Value.PropertyType.FullName, p.Value.PropertyType.IsGenericParameter)));
                var fieldsDesc = Fields.Select(pair =>
                    new KeyValuePair<string, Info>(pair.Key,
                        new Info(pair.Value.FieldType.FullName, pair.Value.FieldType.IsGenericParameter)));
                return propsDesc.Union(fieldsDesc);
            }
        }


        public GenericDefinitionDescription(Type type) : base(type.FullName, TypeCategory.GenericDefinition)
        {
            Props = new HashSet<KeyValuePair<string, PropertyInfo>>();
            Fields = new HashSet<KeyValuePair<string, FieldInfo>>();

            GenericTypes = type.GetGenericArguments();
            foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                Props.Add(new KeyValuePair<string, PropertyInfo>(prop.Name, prop));

            foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.Instance))
                Fields.Add(new KeyValuePair<string, FieldInfo>(field.Name, field));
        }
    }

    public class GenericConstructedDescription : TypeDescription
    {
        private Type GenericTypeDefinition { get; set; }
        public string GenericTypeDefinitionDesc => GenericTypeDefinition.FullName;
        private IEnumerable<Type> GenericTypes { get; set; }
        public IEnumerable<string> GenericTypesDesc => GenericTypes.Select(type => type.FullName);

        public GenericConstructedDescription(Type type) : base(type.FullName, TypeCategory.GenericConstructed)
        {
            GenericTypeDefinition = type.GetGenericTypeDefinition();
            GenericTypes = type.GetGenericArguments();
        }
    }

    public class ActionResultDescription : TypeDescription
    {
        public ActionResultDescription(Type type) : base(type.FullName, TypeCategory.ActionResult)
        {
        }
    }

    public class ComplexClassDescription : TypeDescription
    {
        private HashSet<KeyValuePair<string, Type>> Publics { get; set; }

        public IEnumerable<KeyValuePair<string, string>> PublicsDesc =>
            Publics.Select(pair => new KeyValuePair<string, string>(pair.Key, pair.Value.FullName));

        private FieldInfo[] Fields { get; set; }

        private PropertyInfo[] Props { get; set; }

        public ComplexClassDescription(Type type) : base(type.FullName, TypeCategory.ComplexClass)
        {
            Fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
            Props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            Publics = new HashSet<KeyValuePair<string, Type>>();
            foreach (var prop in type.GetProperties(BindingFlags.Public | BindingFlags.Instance))
                Publics.Add(new KeyValuePair<string, Type>(prop.Name, prop.PropertyType));
            foreach (var field in type.GetFields(BindingFlags.Public | BindingFlags.Instance))
                Publics.Add(new KeyValuePair<string, Type>(field.Name, field.FieldType));
        }
    }

    public class TypesDescriptionProvider
    {
        private Type GetTypeByNameOrThrow(string fullTypeName)
        {
            Assembly GetAssemblyByTypeFullName(string fullTypeName)
            {
                Assembly GetAssemblyByParts(string[] parts)
                {
                    if (parts.Length == 0)
                        return null;
                    try
                    {
                        var fullname = string.Join(".", parts);
                        return Assembly.Load(fullname);
                    }
                    catch
                    {
                        return GetAssemblyByParts(parts.Take(parts.Length - 1).ToArray());
                    }
                }

                var parts = fullTypeName.Split('.');
                return GetAssemblyByParts(parts);
            }

            var assembly = GetAssemblyByTypeFullName(fullTypeName);
            if (assembly == null)
            {
                try
                {
                    assembly = typeof(int).Assembly;
                    var t = assembly.GetType(fullTypeName);
                    if (t != null)
                        return t;
                }
                catch
                {
                    throw new Exception($"Can't find assembly of this type: {fullTypeName}");
                }

                throw new Exception($"Can't find assembly of this type: {fullTypeName}");
            }

            var type = assembly.GetType(fullTypeName);
            if (type == null)
                throw new Exception($"Can't find type {type} in assembly {assembly}");
            return type;
        }

        public TypeDescription Create(string typeFullName)
        {
            var type = GetTypeByNameOrThrow(typeFullName);
            return Create(type);
        }

        public TypeDescription Create(Type type)
        {
            var category = TypeCategoryQualifier.GetTypeCategory(type);
            return category switch
            {
                TypeCategory.Enum => new EnumDescription(type),
                TypeCategory.Primitive => new TypeDescription(typeCategory: category, typeName: type.FullName),
                TypeCategory.GenericDefinition => new GenericDefinitionDescription(type),
                TypeCategory.GenericConstructed => new GenericConstructedDescription(type),
                TypeCategory.ActionResult => new ActionResultDescription(type),
                TypeCategory.ComplexClass => new ComplexClassDescription(type),
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }

    public static class TypeCategoryQualifier
    {
        private static bool IsEnumerableType(Type type) =>
            type.Name != nameof(String)
            && type.GetInterface(nameof(IEnumerable)) != null;

        private static bool IsPrimitive(Type type) => type.IsPrimitive || type == typeof(string) ||
                                                      type == typeof(DateTime) || type == typeof(TimeSpan);

        public static TypeCategory GetTypeCategory(Type type)
        {
            if (IsPrimitive(type))
                return TypeCategory.Primitive;

            if (type.IsEnum)
                return TypeCategory.Enum;

            if (type.IsGenericTypeDefinition)
                return TypeCategory.GenericDefinition;

            if (type.IsConstructedGenericType)
                return TypeCategory.GenericConstructed;

            if (type.IsSubclassOf(typeof(ActionResult)) || type == typeof(ActionResult))
                return TypeCategory.ActionResult;


            return TypeCategory.ComplexClass;
        }
    }

    public enum TypeCategory
    {
        Enum,
        ComplexClass,
        Primitive,
        GenericDefinition,
        GenericConstructed,
        ActionResult,
    }
}