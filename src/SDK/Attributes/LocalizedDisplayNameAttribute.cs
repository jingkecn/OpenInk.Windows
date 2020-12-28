using System.ComponentModel;
using Windows.ApplicationModel.Resources.Core;
using MyScript.InteractiveInk.Annotations;

namespace MyScript.InteractiveInk.Attributes
{
    /// <summary>Specifies the display name for a property, event, or public void method.</summary>
    internal class LocalizedDisplayNameAttribute : DisplayNameAttribute
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="LocalizedDisplayNameAttribute"></see> class using the display
        ///     name.
        /// </summary>
        /// <param name="resourceKey">The resource key to retrieve resource string.</param>
        public LocalizedDisplayNameAttribute([NotNull] string resourceKey)
        {
            ResourceKey = resourceKey;
        }

        private string ResourceKey { get; }

        public override string DisplayName =>
            ResourceManager.Current.MainResourceMap.GetValue($"Resources/{ResourceKey}").ValueAsString;
    }
}
