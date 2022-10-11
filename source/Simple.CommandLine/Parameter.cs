using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Simple.CommandLine;

public abstract class Parameter : Token {
    private static readonly Regex _validAlias = new("^[a-z0-9]$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

    protected Parameter(string name, char alias, string? description = null, bool isAvailableToChildren = false)
        : base(name, description) {
        Alias = alias == '\0' ? null : alias.ToString();
        if (Alias is not null && !_validAlias.IsMatch(Alias))
            throw new ArgumentException($"Invalid value '{alias}'. Alias must be a letter or number.", nameof(alias));
        IsAvailableToChildren = isAvailableToChildren;
    }

    internal bool IsAvailableToChildren { get; }

    internal string? Alias { get; }

    internal override bool Is(string candidate) => Name == candidate.ToLower();
    internal bool IsAlias(string candidate) => Alias is not null && Alias == candidate;

    internal abstract void Read(Command caller, ref Span<string> arguments, out bool terminate);
}