using MyScript.IInk;
using MyScript.InteractiveInk.Annotations;

namespace MyScript.InteractiveInk.Extensions
{
    public static class ContentBlockExtensions
    {
        public static bool IsContainer([NotNull] this ContentBlock source)
        {
            return source.Type == "Container";
        }
    }
}
