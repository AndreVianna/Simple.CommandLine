using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace Simple.CommandLine;

public abstract class Parameter : Token {
    private const string _validAliasPattern = "[a..zA..Z0..9]";
    private static readonly Regex _validAlias = new(_validAliasPattern, RegexOptions.Compiled);

    protected Parameter(string name, char alias, string? description = null, bool isAvailableToChildren = false)
        : base(name, description) {
        Alias = alias == '\0' ? null : alias.ToString();
        if (Alias is not null && !_validAlias.IsMatch(Alias))
            throw new ArgumentException($"Invalid value '{alias}'. Allowed values: '{_validAliasPattern}'.", nameof(alias));
        IsAvailableToChildren = isAvailableToChildren;
    }

    internal bool IsAvailableToChildren { get; }

    internal string? Alias { get; }

    internal override bool Is(string? alias) =>
        base.Is(alias) || (Alias is not null && $"-{Alias}" == alias?.Trim());

    internal abstract void Read(Command caller, ref Span<string> arguments, out bool terminate);
}