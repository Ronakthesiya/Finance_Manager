using Finance_Manager_Core.Interface;

namespace Finance_Manager_Core.Services
{
    public class DTOPOCOMapper : IDTOPOCOMapper
    {
        public TTarget Map<TSource, TTarget>(TSource source) where TTarget : new()
        {
            var target = new TTarget();

            var sourceProps = typeof(TSource).GetProperties();
            var targetProps = typeof(TTarget).GetProperties();

            foreach (var sp in sourceProps)
            {
                var tp = targetProps.FirstOrDefault(p => p.Name == sp.Name);

                if (tp != null && tp.CanWrite)
                {
                    var value = sp.GetValue(source);

                    if (value == null)
                    {
                        tp.SetValue(target, null);
                        continue;
                    }

                    var sourceType = sp.PropertyType;
                    var targetType = tp.PropertyType;

                    // ✅ Handle enum → string
                    if (targetType == typeof(string) && sourceType.IsEnum)
                    {
                        tp.SetValue(target, value.ToString());
                    }
                    // ✅ Handle string → enum
                    else if (targetType.IsEnum && sourceType == typeof(string))
                    {
                        var enumValue = Enum.Parse(targetType, value.ToString());
                        tp.SetValue(target, enumValue);
                    }
                    // ✅ Handle same type
                    else if (targetType.IsAssignableFrom(sourceType))
                    {
                        tp.SetValue(target, value);
                    }
                    // ✅ Try safe conversion (int, double, etc.)
                    else
                    {
                        try
                        {
                            var convertedValue = Convert.ChangeType(value, targetType);
                            tp.SetValue(target, convertedValue);
                        }
                        catch
                        {
                            // ignore incompatible types (or log if needed)
                        }
                    }
                }
            }

            return target;
        }

    }
}
