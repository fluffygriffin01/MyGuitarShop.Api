namespace MyGuitarShop.Api.Mappers
{
    public static class AutoReflectionMapper
    {
        public static TTarget? Map<TSource, TTarget>(TSource? source)
            where TTarget : new()
        {
            if (source == null)
                return default;

            TTarget target = new TTarget();
            var sourceProperties = typeof(TSource).GetProperties();
            var targetProperties = typeof(TTarget).GetProperties();

            foreach (var sourceProp in sourceProperties)
            {
                var targetProp = targetProperties
                    .FirstOrDefault(tp => tp.Name == sourceProp.Name &&
                                          tp.PropertyType == sourceProp.PropertyType &&
                                          tp.CanWrite);
                if (targetProp == null) continue;

                var value = sourceProp.GetValue(source);
                targetProp.SetValue(target, value);
            }
            return target;
        }
    }
}
