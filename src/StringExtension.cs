namespace MarcoZechner.PrettyReflector;

public static class StringExtension{
    /// <summary>
    /// Replaces a substring at a given index with a new string, even if the new string is longer than the substring.
    /// </summary>
    /// <param name="str"> Original string </param>
    /// <param name="index"> Index to start replacing </param>
    /// <param name="newText"> New text to insert </param>
    /// <returns> String with replaced substring </returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the index is negative.</exception>
    public static string ReplaceAt(this string str, int index, string newText)
    {
        ArgumentOutOfRangeException.ThrowIfNegative(index, nameof(index));

        if (string.IsNullOrEmpty(str))
            str = string.Empty;
        if (index + newText.Length > str.Length)
            str = str.PadRight(index + newText.Length);

        return str.Remove(index, Math.Min(newText.Length, str.Length - index)).Insert(index, newText);
    }

    /// <summary>
    /// Shortens a string from the left side to a given length.
    /// </summary>
    /// <param name="str"> Original string </param>
    /// <param name="length"> Length of the shortened string </param>
    /// <returns> String with shortened left side if necessary </returns>
    public static string ShortenLeft(this string str, int length)
    {
        if (string.IsNullOrEmpty(str))
            return string.Empty;

        return str.Length > length ? str[^length..] : str;
    }

    /// <summary>
    /// Shortens a string from the right side to a given length.
    /// </summary>
    /// <param name="str"> Original string </param>
    /// <param name="length"> Length of the shortened string </param>
    /// <returns> String with shortened right side if necessary </returns>
    public static string ShortenRight(this string str, int length)
    {
        if (string.IsNullOrEmpty(str))
            return string.Empty;

        return str.Length > length ? str[..length] : str;
    }

    /// <summary>
    /// Sets the length of a string by either padding or truncating it.
    /// </summary>
    /// <param name="str"> Original string </param>
    /// <param name="length"> Length of the new string </param>
    /// <param name="padLeft"> True if the string should be padded on the left side, so right-aligned text. e.g.: "Text" -> "__Text" </param>
    /// <param name="truncateLeft"> True if the string should be truncated on the left side. e.g.: "Text" -> "xt" </param>
    /// <param name="paddingChar"> Character to use for padding </param>
    /// <returns> String with adjusted length </returns>
    public static string SetLength(this string str, int length, bool padLeft = false, bool truncateLeft = false, char paddingChar = ' ')
    {
        if (string.IsNullOrEmpty(str))
            str = string.Empty;

        if (str.Length == length)
            return str;

        if (str.Length < length)
            return padLeft ? str.PadLeft(length, paddingChar) : str.PadRight(length, paddingChar);

        return truncateLeft ? str.ShortenLeft(length) : str.ShortenRight(length);
    }

    /// <summary>
    /// Indents each line of a string by a given amount of characters.
    /// </summary>
    /// <param name="str">String to indent</param>
    /// <param name="count">Number of characters to indent</param>
    /// <param name="indentChar">Character that is used to indent each line</param>
    /// <returns></returns>
    public static string Indent(this string str, int count, char indentChar = ' ') {
        if (count == 0) {
            return str;
        }

        string indent = new(indentChar, count);
        return string.Join(Environment.NewLine, str.Split(Environment.NewLine).Select(line => indent + line));
    }

    /// <summary>
    /// Extends each line of a string on the right side with a specific character
    /// so all lines have the same specified length. Lines that are longer than
    /// the specified length are left unchanged.
    /// </summary>
    /// <param name="str">The string containing multiple lines.</param>
    /// <param name="length">The target length for each line.</param>
    /// <param name="paddingChar">The character used to pad lines.</param>
    /// <returns>A string with all lines extended to the specified length.</returns>
    public static string ExtendLinesRight(this string str, int length, char paddingChar = ' ')
    {
        if (string.IsNullOrEmpty(str))
            return string.Empty;

        return string.Join(Environment.NewLine, str.Split(Environment.NewLine)
            .Select(line => line.PadRight(length, paddingChar)));
    }
}