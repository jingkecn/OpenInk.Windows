using System.ComponentModel;
using MyScript.InteractiveInk.Attributes;
using MyScript.InteractiveInk.Converters;

namespace MyScript.InteractiveInk.Enumerations
{
    /// <summary>
    ///     Supported content types of iink SDK.
    /// </summary>
    /// <remarks>
    ///     For each type of content, and provided that it makes sense, you can consistently:
    ///     <list type="bullet">
    ///         <item>Input digital ink and have it interpreted, or import data from supported import formats,</item>
    ///         <item>Display the content,</item>
    ///         <item>Interact with the content via editing/decoration gestures,</item>
    ///         <item>Convert it to typeset form,</item>
    ///         <item>Save/load content to/from the file system,</item>
    ///         <item>
    ///             Export/import ink and interpretation to/from MyScript’s readable and parseable exchange format, and
    ///             integrate with your application or ecosystem,
    ///         </item>
    ///         <item>Export to a set of built-in supported export formats.</item>
    ///     </list>
    ///     To localize the display names for each value, please use a <see cref="TypeDescriptor" /> to get the
    ///     <see cref="TypeConverter" /> and convert the value to a localized string:
    ///     <code>TypeDescriptor.GetConverter(value).ConvertToString(value)</code>
    /// </remarks>
    [TypeConverter(typeof(EnumLocalizationConverter))]
    public enum ContentType
    {
        /// <summary>
        ///     Block supporting a diagram, with automatic text/non-text distinction and dynamic reorganization possibilities.
        /// </summary>
        /// <remarks>
        ///     To localize the display name of this value, please add to your localizations a string value keyed by
        ///     "ContentTypeDiagram".
        /// </remarks>
        [Description("Diagram")] [LocalizedDisplayName("ContentTypeDiagram")]
        Diagram,

        /// <summary>
        ///     Block hosting a collection of strokes without any interpretation.
        /// </summary>
        /// <remarks>
        ///     Useful for scribbles and drawing activities.
        /// </remarks>
        [Description("Drawing")] [LocalizedDisplayName("ContentTypeDrawing")]
        Drawing,

        /// <summary>
        ///     Block supporting one equation or more.
        /// </summary>
        /// <remarks>
        ///     To localize the display name of this value, please add to your localizations a string value keyed by
        ///     "ContentTypeMath".
        /// </remarks>
        [Description("Math")] [LocalizedDisplayName("ContentTypeMath")]
        Math,

        /// <summary>
        ///     Block hosting raw digital ink with no explicit segmentation into text, math, diagram or other items which semantics
        ///     is known by iink SDK.
        /// </summary>
        /// <remarks>
        ///     Content is analyzed by iink SDK to retrieve ink corresponding to text blocks from the rest of the content. It
        ///     is key to implement ink search functionality on raw digital ink. You can obtain this information via a JIIX
        ///     export. In non-text blocks, according to its configuration, iink SDK can further distinguish shape from other
        ///     blocks.
        ///     To localize the display name of this value, please add to your localizations a string value keyed by
        ///     "ContentTypeRawContent".
        /// </remarks>
        [Description("Raw Content")] [LocalizedDisplayName("ContentTypeRawContent")]
        RawContent,

        /// <summary>
        ///     Simple block of multi-line text (including possible line breaks), responsive reflow.
        /// </summary>
        /// <remarks>
        ///     It shows guides by default but these can be deactivated (for instance when processing ink from an outside source
        ///     that is not aligned on the guides).
        ///     To localize the display name of this value, please add to your localizations a string value keyed by
        ///     "ContentTypeText".
        /// </remarks>
        [Description("Text")] [LocalizedDisplayName("ContentTypeText")]
        Text,

        /// <summary>
        ///     Container hosting an ordered collection of text, math, diagram, raw content and drawing blocks.
        /// </summary>
        /// <remarks>
        ///     It is a vertical, dynamic and responsive stream of content.
        ///     To localize the display name of this value, please add to your localizations a string value keyed by
        ///     "ContentTypeTextDocument".
        /// </remarks>
        [Description("Text Document")] [LocalizedDisplayName("ContentTypeTextDocument")]
        TextDocument
    }
}
