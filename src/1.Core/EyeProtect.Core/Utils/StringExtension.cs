using System;
using System.ComponentModel;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using Newtonsoft.Json;

namespace EyeProtect.Core.Utils
{
    public static class StringExtension
    {
        /// <summary>
        /// 首字母大写
        /// </summary>
        public static string ToTitleCase(this string str)
        {
            if (string.IsNullOrWhiteSpace(str)) return str;

            return str.Substring(0, 1).ToUpper() + str.Substring(1);
        }

        /// <summary>
        /// 比较字符串是否相同，忽略大小写（InvariantCultureIgnoreCase）
        /// </summary>
        /// <param name="str"></param>
        /// <param name="str2"></param>
        /// <returns></returns>
        public static bool EqualsIgnoreCase(this string str, string str2)
        {
            return str.Equals(str2, StringComparison.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// 字符串是转换为 指定类型
        /// </summary>
        public static bool TryParse(this string value, Type type, out object val)
        {
            try
            {
                var valType = type.GetNonNullableType();

                TypeConverter converter = TypeDescriptor.GetConverter(valType);
                if (converter.CanConvertFrom(typeof(string)))
                {
                    val = converter.ConvertFrom(value);
                    return true;
                }
                converter = TypeDescriptor.GetConverter(type);
                if (converter.CanConvertTo(valType))
                {
                    val = converter.ConvertTo(value, valType);
                    return true;
                }
            }
            catch (Exception)
            {
            }
            val = null;
            return false;
        }

        /// <summary>
        /// 字符串是转换为 指定类型
        /// </summary>
        /// <typeparam name="TValue">转换的目标类型</typeparam>
        public static bool TryParse<TValue>(this string value, out TValue val)
        {
            try
            {
                var valType = typeof(TValue).GetNonNullableType();

                TypeConverter converter = TypeDescriptor.GetConverter(valType);
                if (converter.CanConvertFrom(typeof(string)))
                {
                    val = (TValue)converter.ConvertFrom(value);
                    return true;
                }
                converter = TypeDescriptor.GetConverter(typeof(TValue));
                if (converter.CanConvertTo(valType))
                {
                    val = (TValue)converter.ConvertTo(value, valType);
                    return true;
                }
            }
            catch
            {
            }

            val = default;
            return false;
        }

        /// <summary>
        /// 字符串是转换为 指定类型
        /// </summary>
        /// <typeparam name="TValue">转换的目标类型</typeparam>
        public static TValue As<TValue>(this string value, TValue defaultValue = default)
        {
            try
            {
                var valType = typeof(TValue).GetNonNullableType();

                TypeConverter converter = TypeDescriptor.GetConverter(valType);
                if (converter.CanConvertFrom(typeof(string)))
                {
                    return (TValue)converter.ConvertFrom(value);
                }
                converter = TypeDescriptor.GetConverter(typeof(TValue));
                if (converter.CanConvertTo(valType))
                {
                    return (TValue)converter.ConvertTo(value, valType);
                }
            }
            catch (Exception)
            {
            }
            return defaultValue;
        }


        /// <summary>
        /// 字符串是转换为 指定类型
        /// </summary>
        /// <typeparam name="TValue">转换的目标类型</typeparam>
        public static TValue? As<TValue>(this string value, TValue? defaultValue)
            where TValue : struct
        {
            try
            {
                var valType = typeof(TValue).GetNonNullableType();

                TypeConverter converter = TypeDescriptor.GetConverter(valType);
                if (converter.CanConvertFrom(typeof(string)))
                {
                    return (TValue)converter.ConvertFrom(value);
                }
                converter = TypeDescriptor.GetConverter(typeof(string));
                if (converter.CanConvertTo(valType))
                {
                    return (TValue)converter.ConvertTo(value, valType);
                }
            }
            catch (Exception)
            {
            }
            return defaultValue;
        }


        /// <summary>
        /// 确保字符串以指定字符结尾
        /// </summary>
        public static string EnsureEndsWith(this string str, string c, StringComparison comparison = StringComparison.OrdinalIgnoreCase)
        {
            if (str?.EndsWith(c, comparison) == true)
                return str;

            return str + c;
        }

        /// <summary>
        /// 从JSON中解析对像, 如果字符串为 null, 则返回 <typeparamref name="T"/> 的默认值.
        /// </summary>
        public static T FromJson<T>(this string json, JsonSerializerSettings settings = null)
        {
            if (json == null) return default;

            return JsonConvert.DeserializeObject<T>(json, settings);
        }

        /// <summary>
        /// 将对像转换为Json字符串, 如果对像为 null, 则返回 null
        /// </summary>
        public static string ToJson(this object json, Formatting formatting = Formatting.None, JsonSerializerSettings settings = null)
        {
            if (json == null) return null;

            return JsonConvert.SerializeObject(json, formatting, settings);
        }


        ///// <summary>
        ///// Adds a char to end of given string if it does not ends with the char.
        ///// </summary>
        //public static string EnsureEndsWith(this string str, char c)
        //{
        //    return EnsureEndsWith(str, c, StringComparison.Ordinal);
        //}

        ///// <summary>
        ///// Adds a char to end of given string if it does not ends with the char.
        ///// </summary>
        //public static string EnsureEndsWith(this string str, char c, StringComparison comparisonType)
        //{
        //    if (str == null)
        //    {
        //        throw new ArgumentNullException(nameof(str));
        //    }

        //    if (str.EndsWith(c.ToString(), comparisonType))
        //    {
        //        return str;
        //    }

        //    return str + c;
        //}

        ///// <summary>
        ///// Adds a char to end of given string if it does not ends with the char.
        ///// </summary>
        //public static string EnsureEndsWith(this string str, char c, bool ignoreCase, CultureInfo culture)
        //{
        //    if (str == null)
        //    {
        //        throw new ArgumentNullException(nameof(str));
        //    }

        //    if (str.EndsWith(c.ToString(culture), ignoreCase, culture))
        //    {
        //        return str;
        //    }

        //    return str + c;
        //}

        ///// <summary>
        ///// Adds a char to beginning of given string if it does not starts with the char.
        ///// </summary>
        //public static string EnsureStartsWith(this string str, char c)
        //{
        //    return EnsureStartsWith(str, c, StringComparison.Ordinal);
        //}

        ///// <summary>
        ///// Adds a char to beginning of given string if it does not starts with the char.
        ///// </summary>
        //public static string EnsureStartsWith(this string str, char c, StringComparison comparisonType)
        //{
        //    if (str == null)
        //    {
        //        throw new ArgumentNullException(nameof(str));
        //    }

        //    if (str.StartsWith(c.ToString(), comparisonType))
        //    {
        //        return str;
        //    }

        //    return c + str;
        //}

        ///// <summary>
        ///// Adds a char to beginning of given string if it does not starts with the char.
        ///// </summary>
        //public static string EnsureStartsWith(this string str, char c, bool ignoreCase, CultureInfo culture)
        //{
        //    if (str == null)
        //    {
        //        throw new ArgumentNullException("str");
        //    }

        //    if (str.StartsWith(c.ToString(culture), ignoreCase, culture))
        //    {
        //        return str;
        //    }

        //    return c + str;
        //}

        ///// <summary>
        ///// Indicates whether this string is null or an System.String.Empty string.
        ///// </summary>
        //public static bool IsNullOrEmpty(this string str)
        //{
        //    return string.IsNullOrEmpty(str);
        //}

        ///// <summary>
        ///// indicates whether this string is null, empty, or consists only of white-space characters.
        ///// </summary>
        //public static bool IsNullOrWhiteSpace(this string str)
        //{
        //    return string.IsNullOrWhiteSpace(str);
        //}

        ///// <summary>
        ///// Gets a substring of a string from beginning of the string.
        ///// </summary>
        ///// <exception cref="ArgumentNullException">Thrown if <paramref name="str"/> is null</exception>
        ///// <exception cref="ArgumentException">Thrown if <paramref name="len"/> is bigger that string's length</exception>
        //public static string Left(this string str, int len)
        //{
        //    if (str == null)
        //    {
        //        throw new ArgumentNullException("str");
        //    }

        //    if (str.Length < len)
        //    {
        //        throw new ArgumentException("len argument can not be bigger than given string's length!");
        //    }

        //    return str.Substring(0, len);
        //}

        ///// <summary>
        ///// Converts line endings in the string to <see cref="Environment.NewLine"/>.
        ///// </summary>
        //public static string NormalizeLineEndings(this string str)
        //{
        //    return str.Replace("\r\n", "\n").Replace("\r", "\n").Replace("\n", Environment.NewLine);
        //}

        ///// <summary>
        ///// Gets index of nth occurence of a char in a string.
        ///// </summary>
        ///// <param name="str">source string to be searched</param>
        ///// <param name="c">Char to search in <paramref name="str"/></param>
        ///// <param name="n">Count of the occurence</param>
        //public static int NthIndexOf(this string str, char c, int n)
        //{
        //    if (str == null)
        //    {
        //        throw new ArgumentNullException(nameof(str));
        //    }

        //    var count = 0;
        //    for (var i = 0; i < str.Length; i++)
        //    {
        //        if (str[i] != c)
        //        {
        //            continue;
        //        }

        //        if ((++count) == n)
        //        {
        //            return i;
        //        }
        //    }

        //    return -1;
        //}

        ///// <summary>
        ///// Removes first occurrence of the given postfixes from end of the given string.
        ///// Ordering is important. If one of the postFixes is matched, others will not be tested.
        ///// </summary>
        ///// <param name="str">The string.</param>
        ///// <param name="postFixes">one or more postfix.</param>
        ///// <returns>Modified string or the same string if it has not any of given postfixes</returns>
        //public static string RemovePostFix(this string str, params string[] postFixes)
        //{
        //    if (str == null)
        //    {
        //        return null;
        //    }

        //    if (str == string.Empty)
        //    {
        //        return string.Empty;
        //    }

        //    if (postFixes.IsNullOrEmpty())
        //    {
        //        return str;
        //    }

        //    foreach (var postFix in postFixes)
        //    {
        //        if (str.EndsWith(postFix))
        //        {
        //            return str.Left(str.Length - postFix.Length);
        //        }
        //    }

        //    return str;
        //}

        ///// <summary>
        ///// Removes first occurrence of the given prefixes from beginning of the given string.
        ///// Ordering is important. If one of the preFixes is matched, others will not be tested.
        ///// </summary>
        ///// <param name="str">The string.</param>
        ///// <param name="preFixes">one or more prefix.</param>
        ///// <returns>Modified string or the same string if it has not any of given prefixes</returns>
        //public static string RemovePreFix(this string str, params string[] preFixes)
        //{
        //    if (str == null)
        //    {
        //        return null;
        //    }

        //    if (str == string.Empty)
        //    {
        //        return string.Empty;
        //    }

        //    if (preFixes.IsNullOrEmpty())
        //    {
        //        return str;
        //    }

        //    foreach (var preFix in preFixes)
        //    {
        //        if (str.StartsWith(preFix))
        //        {
        //            return str.Right(str.Length - preFix.Length);
        //        }
        //    }

        //    return str;
        //}

        ///// <summary>
        ///// Gets a substring of a string from end of the string.
        ///// </summary>
        ///// <exception cref="ArgumentNullException">Thrown if <paramref name="str"/> is null</exception>
        ///// <exception cref="ArgumentException">Thrown if <paramref name="len"/> is bigger that string's length</exception>
        //public static string Right(this string str, int len)
        //{
        //    if (str == null)
        //    {
        //        throw new ArgumentNullException("str");
        //    }

        //    if (str.Length < len)
        //    {
        //        throw new ArgumentException("len argument can not be bigger than given string's length!");
        //    }

        //    return str.Substring(str.Length - len, len);
        //}

        ///// <summary>
        ///// Uses string.Split method to split given string by given separator.
        ///// </summary>
        //public static string[] Split(this string str, string separator)
        //{
        //    return str.Split(new[] { separator }, StringSplitOptions.None);
        //}

        ///// <summary>
        ///// Uses string.Split method to split given string by given separator.
        ///// </summary>
        //public static string[] Split(this string str, string separator, StringSplitOptions options)
        //{
        //    return str.Split(new[] { separator }, options);
        //}

        ///// <summary>
        ///// Uses string.Split method to split given string by <see cref="Environment.NewLine"/>.
        ///// </summary>
        //public static string[] SplitToLines(this string str)
        //{
        //    return str.Split(Environment.NewLine);
        //}

        ///// <summary>
        ///// Uses string.Split method to split given string by <see cref="Environment.NewLine"/>.
        ///// </summary>
        //public static string[] SplitToLines(this string str, StringSplitOptions options)
        //{
        //    return str.Split(Environment.NewLine, options);
        //}

        ///// <summary>
        ///// Converts PascalCase string to camelCase string.
        ///// </summary>
        ///// <param name="str">String to convert</param>
        ///// <param name="invariantCulture">Invariant culture</param>
        ///// <returns>camelCase of the string</returns>
        //public static string ToCamelCase(this string str, bool invariantCulture = true)
        //{
        //    if (string.IsNullOrWhiteSpace(str))
        //    {
        //        return str;
        //    }

        //    if (str.Length == 1)
        //    {
        //        return invariantCulture ? str.ToLowerInvariant() : str.ToLower();
        //    }

        //    return (invariantCulture ? char.ToLowerInvariant(str[0]) : char.ToLower(str[0])) + str.Substring(1);
        //}

        ///// <summary>
        ///// Converts PascalCase string to camelCase string in specified culture.
        ///// </summary>
        ///// <param name="str">String to convert</param>
        ///// <param name="culture">An object that supplies culture-specific casing rules</param>
        ///// <returns>camelCase of the string</returns>
        //public static string ToCamelCase(this string str, CultureInfo culture)
        //{
        //    if (string.IsNullOrWhiteSpace(str))
        //    {
        //        return str;
        //    }

        //    if (str.Length == 1)
        //    {
        //        return str.ToLower(culture);
        //    }

        //    return char.ToLower(str[0], culture) + str.Substring(1);
        //}

        ///// <summary>
        ///// Converts given PascalCase/camelCase string to sentence (by splitting words by space).
        ///// Example: "ThisIsSampleSentence" is converted to "This is a sample sentence".
        ///// </summary>
        ///// <param name="str">String to convert.</param>
        ///// <param name="invariantCulture">Invariant culture</param>
        //public static string ToSentenceCase(this string str, bool invariantCulture = false)
        //{
        //    if (string.IsNullOrWhiteSpace(str))
        //    {
        //        return str;
        //    }

        //    return Regex.Replace(
        //        str,
        //        "[a-z][A-Z]",
        //        m => m.Value[0] + " " + (invariantCulture ? char.ToLowerInvariant(m.Value[1]) : char.ToLower(m.Value[1]))
        //    );
        //}

        ///// <summary>
        ///// Converts given PascalCase/camelCase string to sentence (by splitting words by space).
        ///// Example: "ThisIsSampleSentence" is converted to "This is a sample sentence".
        ///// </summary>
        ///// <param name="str">String to convert.</param>
        ///// <param name="culture">An object that supplies culture-specific casing rules.</param>
        //public static string ToSentenceCase(this string str, CultureInfo culture)
        //{
        //    if (string.IsNullOrWhiteSpace(str))
        //    {
        //        return str;
        //    }

        //    return Regex.Replace(str, "[a-z][A-Z]", m => m.Value[0] + " " + char.ToLower(m.Value[1], culture));
        //}

        ///// <summary>
        ///// Converts string to enum value.
        ///// </summary>
        ///// <typeparam name="T">Type of enum</typeparam>
        ///// <param name="value">String value to convert</param>
        ///// <returns>Returns enum object</returns>
        //public static T ToEnum<T>(this string value)
        //    where T : struct
        //{
        //    if (value == null)
        //    {
        //        throw new ArgumentNullException(nameof(value));
        //    }

        //    return (T)Enum.Parse(typeof(T), value);
        //}

        ///// <summary>
        ///// Converts string to enum value.
        ///// </summary>
        ///// <typeparam name="T">Type of enum</typeparam>
        ///// <param name="value">String value to convert</param>
        ///// <param name="ignoreCase">Ignore case</param>
        ///// <returns>Returns enum object</returns>
        //public static T ToEnum<T>(this string value, bool ignoreCase)
        //    where T : struct
        //{
        //    if (value == null)
        //    {
        //        throw new ArgumentNullException(nameof(value));
        //    }

        //    return (T)Enum.Parse(typeof(T), value, ignoreCase);
        //}

        //public static string ToMd5(this string str)
        //{
        //    using (var md5 = MD5.Create())
        //    {
        //        var inputBytes = Encoding.UTF8.GetBytes(str);
        //        var hashBytes = md5.ComputeHash(inputBytes);

        //        var sb = new StringBuilder();
        //        foreach (var hashByte in hashBytes)
        //        {
        //            sb.Append(hashByte.ToString("X2"));
        //        }

        //        return sb.ToString();
        //    }
        //}

        ///// <summary>
        ///// Converts camelCase string to PascalCase string.
        ///// </summary>
        ///// <param name="str">String to convert</param>
        ///// <param name="invariantCulture">Invariant culture</param>
        ///// <returns>PascalCase of the string</returns>
        //public static string ToPascalCase(this string str, bool invariantCulture = true)
        //{
        //    if (string.IsNullOrWhiteSpace(str))
        //    {
        //        return str;
        //    }

        //    if (str.Length == 1)
        //    {
        //        return invariantCulture ? str.ToUpperInvariant() : str.ToUpper();
        //    }

        //    return (invariantCulture ? char.ToUpperInvariant(str[0]) : char.ToUpper(str[0])) + str.Substring(1);
        //}

        ///// <summary>
        ///// Converts camelCase string to PascalCase string in specified culture.
        ///// </summary>
        ///// <param name="str">String to convert</param>
        ///// <param name="culture">An object that supplies culture-specific casing rules</param>
        ///// <returns>PascalCase of the string</returns>
        //public static string ToPascalCase(this string str, CultureInfo culture)
        //{
        //    if (string.IsNullOrWhiteSpace(str))
        //    {
        //        return str;
        //    }

        //    if (str.Length == 1)
        //    {
        //        return str.ToUpper(culture);
        //    }

        //    return char.ToUpper(str[0], culture) + str.Substring(1);
        //}

        ///// <summary>
        ///// Gets a substring of a string from beginning of the string if it exceeds maximum length.
        ///// </summary>
        ///// <exception cref="ArgumentNullException">Thrown if <paramref name="str"/> is null</exception>
        //public static string Truncate(this string str, int maxLength)
        //{
        //    if (str == null)
        //    {
        //        return null;
        //    }

        //    if (str.Length <= maxLength)
        //    {
        //        return str;
        //    }

        //    return str.Left(maxLength);
        //}

        ///// <summary>
        ///// Gets a substring of a string from beginning of the string if it exceeds maximum length.
        ///// It adds a "..." postfix to end of the string if it's truncated.
        ///// Returning string can not be longer than maxLength.
        ///// </summary>
        ///// <exception cref="ArgumentNullException">Thrown if <paramref name="str"/> is null</exception>
        //public static string TruncateWithPostfix(this string str, int maxLength)
        //{
        //    return TruncateWithPostfix(str, maxLength, "...");
        //}

        ///// <summary>
        ///// Gets a substring of a string from beginning of the string if it exceeds maximum length.
        ///// It adds given <paramref name="postfix"/> to end of the string if it's truncated.
        ///// Returning string can not be longer than maxLength.
        ///// </summary>
        ///// <exception cref="ArgumentNullException">Thrown if <paramref name="str"/> is null</exception>
        //public static string TruncateWithPostfix(this string str, int maxLength, string postfix)
        //{
        //    if (str == null)
        //    {
        //        return null;
        //    }

        //    if (str == string.Empty || maxLength == 0)
        //    {
        //        return string.Empty;
        //    }

        //    if (str.Length <= maxLength)
        //    {
        //        return str;
        //    }

        //    if (maxLength <= postfix.Length)
        //    {
        //        return postfix.Left(maxLength);
        //    }

        //    return str.Left(maxLength - postfix.Length) + postfix;
        //}
    }

    public static class TimeSpanExtension
    {
        /// <summary>
        /// 获取分钟数，向上进1取整
        /// </summary>
        public static int TotalMinutesCeiling(this TimeSpan timeSpan)
        {
            return (int)Math.Ceiling(timeSpan.TotalMinutes);
        }

        /// <summary>
        /// 返回分钟数或秒数字符串
        /// </summary>
        public static string GetMinutesOrSecondsString(this TimeSpan timeSpan)
        {
            if (Math.Abs(timeSpan.TotalSeconds) < 60)
            {
                return $"{Math.Abs(timeSpan.TotalSeconds)}秒";
            }
            else
            {
                return $"{Math.Abs(timeSpan.TotalMinutesCeiling())}分";
            }
        }
    }
}
