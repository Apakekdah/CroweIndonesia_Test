using CI.Data.EF.MongoDB.Models;
using Hero;
using Ride.Attributes.DB;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;

namespace CI.Data.EF.MongoDB
{
    static class Helper
    {
        static readonly IDictionary<string, string> DicTableName = new ConcurrentDictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        static readonly IDictionary<string, IDictionary<string, FieldIndexHeader>> DicFieldIndex = new ConcurrentDictionary<string, IDictionary<string, FieldIndexHeader>>(StringComparer.OrdinalIgnoreCase);
        static readonly IDictionary<string, ICollection<string>> DicFieldIgnore = new ConcurrentDictionary<string, ICollection<string>>(StringComparer.OrdinalIgnoreCase);
        static readonly IDictionary<string, ICollection<FieldMapName>> DicFieldName = new ConcurrentDictionary<string, ICollection<FieldMapName>>(StringComparer.OrdinalIgnoreCase);
        static readonly IDictionary<string, ICollection<string>> DicUniqueKey = new ConcurrentDictionary<string, ICollection<string>>(StringComparer.OrdinalIgnoreCase);
        static readonly IDictionary<string, ICollection<FieldKeyDetail>> DicFieldKey = new ConcurrentDictionary<string, ICollection<FieldKeyDetail>>(StringComparer.OrdinalIgnoreCase);
        static readonly IDictionary<string, ICollection<string>> DicMemberInfo = new ConcurrentDictionary<string, ICollection<string>>(StringComparer.OrdinalIgnoreCase);

        public static readonly string[] FieldId = new string[] { "id", "_id" };

        public static string GetTableNameAttribute<T>()
        {
            return GetTableNameAttribute(typeof(T));
        }

        public static string GetTableNameAttribute(Type typ)
        {
            var defaultName = typ.Name;

            if (DicTableName.ContainsKey(defaultName))
                return DicTableName[defaultName];

            if (typ.GetCustomAttributes(false).Where(f => typ.IsDefined(f.GetType())).SingleOrDefault() is TableNameAttribute cls)
            {
                if (!string.IsNullOrEmpty(cls.Name))
                    defaultName = cls.Name;
            }

            DicTableName[typ.Name] = defaultName;

            return defaultName;
        }

        public static IDictionary<string, FieldIndexHeader> GetFieldIndexAttribute<T>()
        {
            return GetFieldIndexAttribute(typeof(T));
        }

        public static IDictionary<string, FieldIndexHeader> GetFieldIndexAttribute(Type typ)
        {
            if (DicFieldIndex.ContainsKey(typ.Name))
                return DicFieldIndex[typ.Name];

            FieldIndexHeader hdr;
            FieldNameAttribute fn;
            IDictionary<string, FieldIndexHeader> dic = new Dictionary<string, FieldIndexHeader>(StringComparer.OrdinalIgnoreCase);

            var col = new HashSet<string>();

            typ.GetFields(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.IsDefined(typeof(FieldIndexAttribute), false))
                .Each(fi =>
                {
                    if (fi.GetCustomAttribute<FieldIgnoreAttribute>() != null) return;

                    fi.GetCustomAttributes<FieldIndexAttribute>().Each(c =>
                    {
                        fn = fi.GetCustomAttribute<FieldNameAttribute>();

                        if (dic.ContainsKey(c.Name))
                        {
                            hdr = dic[c.Name];
                        }
                        else
                        {
                            hdr = new FieldIndexHeader() { IndexName = c.Name };
                            dic[c.Name] = hdr;
                        }

                        hdr.Details.Add(new FieldIndexDetail()
                        {
                            PropertyName = fi.Name,
                            Type = fi.FieldType,
                            Ascending = c.Ascending,
                            Unique = c.Unique,
                            MappingPropertyName = fn?.Name
                        });

                        if (c.Unique && !col.Contains(fi.Name))
                            col.Add(fi.Name);
                    });
                });

            typ.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.IsDefined(typeof(FieldIndexAttribute)))
                .Each(pi =>
                {
                    if (pi.GetCustomAttribute<FieldIgnoreAttribute>() != null) return;

                    pi.GetCustomAttributes<FieldIndexAttribute>().Each(c =>
                    {
                        fn = pi.GetCustomAttribute<FieldNameAttribute>();

                        if (dic.ContainsKey(c.Name))
                        {
                            hdr = dic[c.Name];
                        }
                        else
                        {
                            hdr = new FieldIndexHeader() { IndexName = c.Name };
                            dic[c.Name] = hdr;
                        }

                        hdr.Details.Add(new FieldIndexDetail()
                        {
                            PropertyName = pi.Name,
                            Type = pi.PropertyType,
                            Ascending = c.Ascending,
                            Unique = c.Unique,
                            MappingPropertyName = fn?.Name
                        });

                        if (c.Unique && !col.Contains(pi.Name))
                            col.Add(pi.Name);
                    });
                });


            DicUniqueKey[typ.Name] = col;
            DicFieldIndex[typ.Name] = dic;

            return dic;
        }

        public static ICollection<string> GetFieldIgnoreAttribute<T>()
        {
            return GetFieldIgnoreAttribute(typeof(T));
        }

        public static ICollection<string> GetFieldIgnoreAttribute(Type typ)
        {
            if (DicFieldIgnore.ContainsKey(typ.Name))
                return DicFieldIgnore[typ.Name];

            ICollection<string> col;

            col = new HashSet<string>(
                typ.GetFields(BindingFlags.Public | BindingFlags.Instance)
                    .Where(p => p.IsDefined(typeof(FieldIgnoreAttribute), false))
                    .GroupBy(fi => fi.Name)
                    .Select(f => f.Key)
                    .Union(typ.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                        .Where(p => p.IsDefined(typeof(FieldIgnoreAttribute)))
                        .GroupBy(pi => pi.Name)
                        .Select(f => f.Key)).ToArray()
            );

            DicFieldIgnore[typ.Name] = col;

            return col;
        }

        public static ICollection<FieldMapName> GetFieldMapNameAttribute<T>()
        {
            return GetFieldMapNameAttribute(typeof(T));
        }

        public static ICollection<FieldMapName> GetFieldMapNameAttribute(Type typ)
        {
            if (DicFieldName.ContainsKey(typ.Name))
                return DicFieldName[typ.Name];

            ICollection<FieldMapName> col = new HashSet<FieldMapName>();

            typ.GetFields()
                .Where(p => p.IsDefined(typeof(FieldNameAttribute), false) && !p.IsDefined(typeof(FieldIgnoreAttribute), false))
                .Select(p => (MemberInfo)p)
                .Union(typ.GetProperties()
                        .Where(p => p.IsDefined(typeof(FieldNameAttribute)) && !p.IsDefined(typeof(FieldIgnoreAttribute)))
                        .Select(p => (MemberInfo)p)).Each(mi =>
                        {
                            //if(col.Any(f=> f.OriginalName=
                            var fn = mi.GetCustomAttributes<FieldNameAttribute>().SingleOrDefault();
                            if (fn == null) return;
                            //fn.Name
                            if (col.Where(f => !f.NewName.IsNullOrEmpty()).Any(f => f.NewName == fn.Name))
                            {
                                throw new Exception($"Field name '{fn.Name}' already defined");
                            }

                            col.Add(new FieldMapName
                            {
                                OriginalName = mi.Name,
                                NewName = fn.Name,
                                Order = fn.Order
                            });
                        });

            DicFieldName[typ.Name] = col;

            return col;
        }

        public static ICollection<string> GetUniqueKey<T>()
        {
            return GetUniqueKey(typeof(T));
        }

        public static ICollection<string> GetUniqueKey(Type typ)
        {
            GetFieldIndexAttribute(typ);
            return DicUniqueKey[typ.Name] ?? new HashSet<string>();
        }

        public static object GetPropertyOrFieldValue(this object target, string name)
        {
            object val = null;

            try
            {
                var typ = target.GetType();

                var prop = typ.GetProperty(name, BindingFlags.Public | BindingFlags.Instance);
                if (prop != null)
                {
                    val = prop.GetValue(target);
                }

                var fi = typ.GetField(name, BindingFlags.Public | BindingFlags.Instance);
                if (fi != null)
                {
                    val = fi.GetValue(target);
                }
            }
            catch { }

            return val;
        }

        public static ICollection<FieldKeyDetail> GetFieldKeyAttributeNew<T>()
        {
            var typ = typeof(T);

            if (DicFieldKey.ContainsKey(typ.Name))
                return DicFieldKey[typ.Name];

            ICollection<FieldKeyDetail> col = new HashSet<FieldKeyDetail>();

            FieldNameAttribute fn;
            PropertyInfo pi = null;
            FieldInfo fi = null;
            Type type;

            typ.GetFields(BindingFlags.Public | BindingFlags.Instance)
                    .Where(p => p.IsDefined(typeof(FieldKeyAttribute), false) && !p.IsDefined(typeof(FieldIgnoreAttribute), false))
                    .Select(p => (MemberInfo)p)
                    .Union(typ.GetProperties()
                        .Where(p => p.IsDefined(typeof(FieldKeyAttribute)) && !p.IsDefined(typeof(FieldIgnoreAttribute)))
                        .Select(p => (MemberInfo)p)).Each(mi =>
                        {
                            if (mi.MemberType == MemberTypes.Property)
                                pi = mi as PropertyInfo;
                            else if (mi.MemberType == MemberTypes.Field)
                                fi = mi as FieldInfo;

                            if ((pi == null) && (fi == null))
                                return;

                            type = (pi?.PropertyType ?? fi.FieldType);
                            fn = mi.GetCustomAttributes<FieldNameAttribute>().SingleOrDefault();
                            col.Add(new FieldKeyDetail
                            {
                                PropertyName = mi.Name,
                                MappingPropertyName = fn?.Name,
                                Type = type
                            });
                        });

            DicFieldKey[typ.Name] = col;

            return col;
        }

        public static bool ContainPropertyOrFieldId(this Type target)
        {
            var typ = target;

            //var lookup = new string[] { "id", "_id", "Id","_Id", "ID", "_ID", "iD", "_iD"};

            if (typ.GetProperties(BindingFlags.Public | BindingFlags.Instance).Any(f => FieldId.Contains(f.Name, StringComparer.OrdinalIgnoreCase)))
                return true;
            else if (typ.GetFields(BindingFlags.Public | BindingFlags.Instance).Any(f => FieldId.Contains(f.Name, StringComparer.OrdinalIgnoreCase)))
                return true;
            return false;
        }

        public static ICollection<MemberInfo> GetIds(this Type target)
        {
            ICollection<MemberInfo> col = new HashSet<MemberInfo>(
                target.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Where(f => FieldId.Contains(f.Name, StringComparer.OrdinalIgnoreCase))
                    .Select(p => (MemberInfo)p)
                    .Union(
                        target.GetFields(BindingFlags.Public | BindingFlags.Instance)
                        .Where(f => FieldId.Contains(f.Name, StringComparer.OrdinalIgnoreCase))
                        .Select(p => (MemberInfo)p)).ToArray()
            );
            return col;
        }

        public static ICollection<string> GetMemberList<T>()
        {
            var typ = typeof(T);

            if (DicMemberInfo.ContainsKey(typ.Name))
                return DicMemberInfo[typ.Name];

            ICollection<string> col;

            col = new HashSet<string>(
                typ.GetFields(BindingFlags.Public | BindingFlags.Instance)
                    .Where(p => !p.IsDefined(typeof(FieldIgnoreAttribute), false))
                    .GroupBy(fi => fi.Name)
                    .Select(f => f.Key)
                    .Union(typ.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                        .Where(p => !p.IsDefined(typeof(FieldIgnoreAttribute)))
                        .GroupBy(pi => pi.Name)
                        .Select(f => f.Key)).ToArray()
            );

            DicMemberInfo[typ.Name] = col;

            return col;
        }
    }
}