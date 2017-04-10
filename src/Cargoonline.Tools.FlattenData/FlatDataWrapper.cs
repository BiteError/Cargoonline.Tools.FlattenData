using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using Newtonsoft.Json.Linq;

namespace Cargoonline.Tools.FlattenData
{
    public class FlatDataWrapper<TEntity>
        where TEntity : class
    {
        private JObject jObject;
        private FlatDataWrapper<TEntity> parentWrapper;
        public readonly TEntity Entity;
        public readonly Expression<Func<TEntity, string>> PropertyLambda;
        public readonly Expression<Func<TEntity, TEntity>> ParentLambda;

        public FlatDataWrapper(TEntity entity, Expression<Func<TEntity, string>> propertyLambda, Expression<Func<TEntity, TEntity>> parentLambda)
        {
            Entity = entity;
            PropertyLambda = propertyLambda;
            ParentLambda = parentLambda;
        }

        public string Data
        {
            get { return Entity.GetPropertyValue(PropertyLambda); }
            set
            {
                Entity.SetPropertyValue(PropertyLambda, value);
                jObject = null;
            }
        }


        public JObject JObject => jObject ?? (jObject = JObject.Parse(Data));

        public TEntity Parent => ParentLambda != null ? Entity.GetPropertyValue(ParentLambda) : null;
        public FlatDataWrapper<TEntity> ParentWrapper => parentWrapper ?? (parentWrapper = this.GetWrapperForParent());

        internal object Get(Enum valueName, Type valueType, FlattenDataProvider<TEntity> provider = null)
        {
            valueName.CheckValueType(valueType);

            var key = valueName.ToString();

            if (!string.IsNullOrEmpty(Data))
            {
                var settings = this.GetByKey(key, valueType);

                if (settings != null)
                {
                    return settings;
                }
            }

            return Parent != null
                ? this.GetFromParent(valueName, valueType, provider)
                : null;
        }

        public void Set<TEnum>(TEnum valueName, object value, Type valueType)
        {
            string key = valueName.ToString();
            this.SetValue(key, value);
        }
    }

    public class FlattenDataProvider<TEntity> 
        where TEntity : class
    {
        Dictionary<TEntity, FlatDataWrapper<TEntity>> cache = new Dictionary<TEntity, FlatDataWrapper<TEntity>>();

        public FlatDataWrapper<TEntity> GetParentWrapper(FlatDataWrapper<TEntity> wrapper)
        {
            if (!cache.ContainsKey(wrapper.Parent))
            {
                cache[wrapper.Parent] = wrapper.ParentWrapper;
            }

            return cache[wrapper.Parent];
        }
    }

    public static class GetExtensions
    {
        public static TResult Get<TEntity, TResult>(this FlatDataWrapper<TEntity> wrapper, Enum valueName, FlattenDataProvider<TEntity> provider)
            where TEntity : class
        {
            var value = wrapper.Get(valueName, typeof(TResult), provider);
            return value != null ? (TResult)value : default(TResult);
        }
    }

    public static class SetExtensions
    {
        public static void Set<TEntity, TEnum, TValue>(this FlatDataWrapper<TEntity> wrapper, TEnum valueName, TValue valueType)
            where TEntity : class
        {
            valueName.CheckValueType<TEnum, TValue>();
            string key = valueName.ToString();
            wrapper.SetValue(key, valueType);
        }
    }

    public static class JsonExtensions
    {
        public static void Set<TEntity, TEnum>(this FlatDataWrapper<TEntity> wrapper, string name, JToken value)
            where TEntity : class
            where TEnum : struct, IConvertible
        {
            TEnum valueName = GetValueName<TEnum>(name);
            Type valueType = valueName.GetAttributeOfType<TEnum, TypeConstraintAttribute>().ValueType;
            wrapper.Set(valueName, value, valueType);
        }

        private static TEnum GetValueName<TEnum>(string name) where TEnum : struct, IConvertible
        {
            if (!typeof(TEnum).IsEnum)
            {
                throw new ArgumentException($"{typeof(TEnum)} must be an enumerated type");
            }

            TEnum valueName;

            if (Enum.TryParse(name, out valueName))
            {
                return valueName;
            }

            throw new ArgumentException($"{typeof(TEnum)} Incorrect type for SettingType");
        }
    }

    public static class GroupExtensions
    {
        public static TResult Get<TEntity, TAttribute, TResult>(this FlatDataWrapper<TEntity> wrapper)
            where TEntity : class
            where TAttribute : Attribute, IEnumType
            where TResult : class, new()
        {
            var result = new TResult();
            foreach (var propertyInfo in result.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                var valueTypeAttribute = propertyInfo.GetCustomAttribute<TAttribute>();
                var valueType = valueTypeAttribute.Type;
                var value = wrapper.Get(valueType, propertyInfo.PropertyType);

                propertyInfo.SetValue(result, value);
            }

            return result;
        }

        public static void Set<TEntity, TAttribute, TResult>(this FlatDataWrapper<TEntity> wrapper, TResult model)
            where TEntity : class
            where TAttribute : Attribute, IEnumType
            where TResult : class, new()
        {
            foreach (var propertyInfo in model.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                var valueTypeAttribute = propertyInfo.GetCustomAttribute<TAttribute>();
                var valueType = valueTypeAttribute.Type;

                valueType.CheckValueType(propertyInfo.PropertyType);
                string key = valueType.ToString();

                wrapper.SetValue(key, propertyInfo.GetValue(model));
            }
        }
    }
}