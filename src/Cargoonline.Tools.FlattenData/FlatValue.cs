namespace Cargoonline.Tools.FlattenData
{
    /// <summary>
    /// DTO for GetAllExtensions
    /// </summary>
    /// <typeparam name="TEnum"></typeparam>
    public class FlatValue<TEnum> where TEnum : struct
    {
        public FlatValue(TEnum valueType, object value)
        {
            Value = value;
            ValueType = valueType;
        }

        public object Value { get; }

        public TEnum ValueType { get; }

        public T Get<T>()
        {
            ValueType.CheckValueType<TEnum, T>();
            return (T)Value;
        }
    }
}