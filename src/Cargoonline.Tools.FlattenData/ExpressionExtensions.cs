using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Cargoonline.Tools.FlattenData
{
    static class ExpressionExtensions
    {
        public static TProperty GetPropertyValue<T, TProperty>(this T target, Expression<Func<T, TProperty>> propertyLamda)
        {
            var memberSelectorExpression = propertyLamda.Body as MemberExpression;

            var property = memberSelectorExpression?.Member as PropertyInfo;

            if (property != null)
            {
                return (TProperty)property.GetValue(target);
            }
            else 
            {
                throw new ArgumentException("Incorect property lambda", nameof(propertyLamda));
            }
        }

        public static void SetPropertyValue<T, TProperty>(this T target, Expression<Func<T, TProperty>> propertyLamda, TProperty value)
        {
            var memberSelectorExpression = propertyLamda.Body as MemberExpression;

            if (memberSelectorExpression != null)
            {
                var property = memberSelectorExpression.Member as PropertyInfo;

                property?.SetValue(target, value, null);
            }
            else
            {
                throw new ArgumentException("Incorect property lambda", nameof(propertyLamda));
            }
        }
    }
}