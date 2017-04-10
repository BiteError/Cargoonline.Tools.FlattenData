using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;

namespace Cargoonline.Tools.FlattenData
{
    public static class FlatDataWrapperExtensions
    {
        internal static object GetFromParent<T>(
            this FlatDataWrapper<T> wrapper, 
            Enum settingType, 
            Type type,
            FlattenDataProvider<T> provider) 
            where T : class
        {
            var parentWrapper = provider != null 
                ? provider.GetParentWrapper(wrapper) 
                : wrapper.ParentWrapper;

            return parentWrapper.Get(settingType, type, provider);
        }

        internal static FlatDataWrapper<T> GetWrapperForParent<T>(this FlatDataWrapper<T> wrapper) where T : class
        {
            return new FlatDataWrapper<T>(wrapper.Parent, wrapper.PropertyLambda, wrapper.ParentLambda);
        }

        internal static object GetByKey<T>(this FlatDataWrapper<T> wrapper, string key, Type type) where T : class
        {
            var jobj = wrapper.JObject;
            var value = jobj[key];
            return value?.ToObject(type);
        }

        internal static void SetValue<T>(this FlatDataWrapper<T> wrapper, string key, object value) where T : class
        {
            JObject jobj;

            if (!string.IsNullOrEmpty(wrapper.Data))
            {
                jobj = JObject.Parse(wrapper.Data);
                jobj.Remove(key);
            }
            else
            {
                jobj = new JObject();
            }

            if (value != null)
            {
                jobj[key] = JToken.FromObject(value);
                wrapper.Entity.SetPropertyValue(wrapper.PropertyLambda, jobj.ToString());
            }
        }

        public static object GetValueFromType<T>(this FlatDataWrapper<T> wrapper, Enum valueName, FlattenDataProvider<T> provider) where T : class
        {
            return wrapper.Get(valueName, valueName.GetValueType(), provider);
        }

        public static IEnumerable<TEnum> FilterRelatedTypes<TEntity, TEnum>(this FlatDataWrapper<TEntity> wrapper) where TEntity : class
        {
            var allValues = Enum.GetValues(typeof(TEnum)).Cast<Enum>();
            return allValues.Where(value => value.RelatedToTypeOfEntity(wrapper.Entity)).Cast<TEnum>();
        }
    }
}