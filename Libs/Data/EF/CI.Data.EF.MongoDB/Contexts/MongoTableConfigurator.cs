using Hero;
using MongoDB.Bson.Serialization;
using CI.Data.EF.MongoDB.Interfaces;
using CI.Data.EF.MongoDB.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace CI.Data.EF.MongoDB.Contexts
{
    class MongoTableConfigurator<T> : IMongoTableConfigurable<T>
        where T : class, new()
    {
        private string tableName;

        public MongoTableConfigurator()
        {
            tableName = Helper.GetTableNameAttribute<T>();
        }

        public void Apply()
        {
            var newTableName = TableName();
            if (newTableName.IsNullOrEmpty())
                throw new NullReferenceException("Table name required");

            tableName = newTableName;

            RegisterClass(typeof(T));
        }

        protected string TableName()
        {
            return tableName;
        }

        protected virtual bool Map(MemberInfo member, BsonMemberMap map)
        {
            return false;
        }

        private IDictionary<Type, BsonClassMap> GetBaseClass(Type baseType)
        {
            if (baseType == null)
                return new Dictionary<Type, BsonClassMap>();

            var obj = typeof(object);
            BsonClassMap bcm = null;

            var col = new List<Type>();
            var colBcm = new List<BsonClassMap>();

            if (!obj.Equals(baseType))
            {
                col.Add(baseType);
            }

            while (!obj.Equals(baseType.BaseType) && (baseType.BaseType != null))
            {
                baseType = baseType.BaseType;
                col.Add(baseType);
            }

            var dic = new Dictionary<Type, BsonClassMap>();

            for (var n = col.Count - 1; n >= 0; n--)
            {
                baseType = col[n];
                bcm = new BsonClassMap(baseType, bcm);
                bcm.SetIgnoreExtraElements(true);
                bcm.SetIgnoreExtraElementsIsInherited(true);
                colBcm.Add(bcm);
            }

            for (var n = colBcm.Count - 1; n >= 0; n--)
            {
                bcm = colBcm[n];
                dic[bcm.ClassType] = bcm;
            }

            col.Clear();
            colBcm.Clear();

            return dic;
        }

        private void RegisterClass(Type typ)
        {
            if (typ == null)
            {
                return;
            }

            var obj = typeof(object);
            if (obj.Equals(typ))
            {
                return;
            }

            if (BsonClassMap.IsClassMapRegistered(typ))
                return;

            var dicBcm = GetBaseClass(typ);
            if ((dicBcm == null) || (dicBcm.Count < 1))
                return;

            var members = typ
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Cast<MemberInfo>()
                .Union(typ.GetFields(BindingFlags.Public | BindingFlags.Instance)
                            .Select(f => (MemberInfo)f)).ToCollection();

            BsonClassMap bcm;
            FieldMapName fmn;
            BsonMemberMap bmm;

            var colMapField = Helper.GetFieldMapNameAttribute(typ);
            var colIgnore = Helper.GetFieldIgnoreAttribute(typ);
            var idx = 0;
            var isIdType = false;

            members.Each(mi =>
            {
                var (isClass, @class) = IsClass(mi);
                if (isClass)
                {
                    @class = ExtractClass(@class);
                    if (@class != null)
                    {
                        RegisterClass(@class);
                    }
                }

                if (dicBcm.ContainsKey(mi.DeclaringType))
                {
                    bcm = dicBcm[mi.DeclaringType];

                    if (colIgnore.Contains(mi.Name))
                    {
                        if (mi.MemberType == MemberTypes.Field)
                            bcm.UnmapField(mi.Name);
                        else
                            bcm.UnmapProperty(mi.Name);
                        return;
                    }

                    if (Helper.FieldId.Contains(mi.Name, StringComparer.InvariantCultureIgnoreCase))
                    {
                        isIdType = true;
                        if (mi.MemberType == MemberTypes.Field)
                            bcm.UnmapField(mi.Name);
                        else
                            bcm.UnmapProperty(mi.Name);
                    }

                    if (mi.MemberType == MemberTypes.Field)
                        bmm = bcm.MapField(mi.Name);
                    else
                        bmm = bcm.MapProperty(mi.Name);

                    if (!Map(mi, bmm))
                    {
                        fmn = colMapField.FirstOrDefault(f => f.OriginalName == mi.Name);
                        if (fmn != null)
                        {
                            bmm.SetOrder(fmn.Order);
                            if (!fmn.NewName.IsNullOrEmpty() && !fmn.NewName.IsNullOrWhiteSpace())
                                bmm.SetElementName(fmn.NewName);
                        }

                        if (isIdType)
                        {
                            isIdType = false;
                            bmm.SetOrder(idx);
                        }
                    }

                    idx++;
                }
            });

            bcm = dicBcm.FirstOrDefault().Value;

            BsonClassMap.RegisterClassMap(bcm);

            members.Clear();
            dicBcm.Clear();
        }

        private (bool isClass, Type @class) IsClass(MemberInfo mi)
        {
            Type type = null;
            bool isClass = false;

            if (mi.MemberType == MemberTypes.Property)
            {
                if (!((PropertyInfo)mi).PropertyType.IsSimpleType())
                {
                    type = ((PropertyInfo)mi).PropertyType;
                    isClass = true;
                }
            }
            else if (mi.MemberType == MemberTypes.Field)
            {
                if (!((FieldInfo)mi).FieldType.IsSimpleType())
                {
                    type = ((FieldInfo)mi).FieldType;
                    isClass = true;
                }
            }

            //if (isClass && IsDictionary(type))
            //{
            //    isClass = false;
            //    type = null;
            //}

            return (isClass, type);
        }

        private Type ExtractClass(Type type)
        {
            Type typ;
            Type typEn = typeof(IEnumerable);

            var nullable = type.GetNullable();
            if (nullable != null)
            {
                typ = nullable;
            }
            else
            {
                typ = type;
            }

            while (true)
            {
                if (IsDictionary(typ))
                {
                    typ = null;
                    break;
                }
                else if (IsEnumerableSimple(typ))
                {
                    typ = null;
                    break;
                }
                else if (IsEnumerable(typ))
                {
                    typ = typ.GetGenericArguments().First();

                    nullable = type.GetNullable();
                    if (nullable != null)
                    {
                        typ = nullable;
                    }
                }
                else
                {
                    if (typ.IsSimpleType())
                    {
                        typ = null;
                    }
                    break;
                }
            }

            return typ;
        }

        private bool IsDictionary(Type type)
        {
            return type.GetInterfaces()
                .Where(i => i.IsGenericType)
                .Any(i => i.GetGenericTypeDefinition() == typeof(IDictionary<,>));
        }

        private bool IsEnumerable(Type type)
        {
            return type.IsGenericType && typeof(IEnumerable).IsAssignableFrom(type);
        }

        private bool IsEnumerableSimple(Type type)
        {
            var isEnumerable = IsEnumerable(type);
            if (isEnumerable)
            {
                return type.IsSimpleType();
            }
            return false;
        }
    }
}