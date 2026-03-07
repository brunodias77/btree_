using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using Catalog.Domain.Services;

namespace Catalog.Infrastructure.Services;

public class SlugGenerator : ISlugGenerator
{
    public string GenerateSlug(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return string.Empty;

        var normalizedString = text.Normalize(NormalizationForm.FormD);
        var stringBuilder = new StringBuilder();

        foreach (var c in normalizedString)
        {
            var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
            if (unicodeCategory != UnicodeCategory.NonSpacingMark)
            {
                stringBuilder.Append(c);
            }
        }

        var str = stringBuilder.ToString().Normalize(NormalizationForm.FormC).ToLowerInvariant();

        // Remove invalid chars
        str = Regex.Replace(str, @"[^a-z0-9\s-]", "");

        // Convert multiple spaces into one space   
        str = Regex.Replace(str, @"\s+", " ").Trim();

        // Cut and trim 
        str = str.Substring(0, str.Length <= 45 ? str.Length : 45).Trim();
        
        // Replace spaces by hyphens
        str = Regex.Replace(str, @"\s", "-"); 

        return str;
    }
}
