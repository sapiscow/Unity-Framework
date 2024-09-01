using System.Linq;

namespace Sapiscow
{
    public static class ReflectionHelper
    {
        public static System.Type GetImplementedClassType(System.Type interfaceType)
            => System.AppDomain.CurrentDomain.GetAssemblies().SelectMany(c => c.GetTypes())
                .FirstOrDefault(c => c != interfaceType && interfaceType.IsAssignableFrom(c));
    }
}