using Iteracode.Api.Abstractions;
using Iteracode.Api.Data;

namespace Iteracode.Api.Services;

public class LanguageService : ILanguageService
{
    private readonly ApplicationDbContext _context;
    private readonly Dictionary<string, int> _languageIds;

    public LanguageService(ApplicationDbContext context)
    {
        _context = context;
        _languageIds = _context.LanguageJudge0Ids
            .Where(l => l.Enabled)
            .ToDictionary(l => l.Language, l => l.Judge0Id);
    }

    public int GetLanguageId(string language)
        => _languageIds.TryGetValue(language, out var id) ? id : -1;
}