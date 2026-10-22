using System.Text.RegularExpressions;

namespace Lexicon.InventoryManagementSystem;

public sealed record Product(string Name, int Id)
{
	const int MinNumber = 200;
	const int MaxNumber = 500;

	public int Id { get; } = Id;
	public string Name { get; } = Name.ToUpperInvariant();

	public override string ToString() => $"{Name}-{Id}";

	public static Result<Product> Parse (string text, bool validateRange = true)
	{
		if (string.IsNullOrWhiteSpace(text))
			return Result<Product>.Error("ERROR: Input cannot be empty.");

		text = text.Trim();

		var dashIndex = text.IndexOf('-');

		if (dashIndex < 0)
			return Result<Product>.Error("ERROR: Product must contain a dash (-).");

		var left = text[..dashIndex];
		var right = text[(dashIndex + 1)..];

		if (!Regex.IsMatch(left, "^[A-Za-z]+$"))
			return Result<Product>.Error("ERROR: The left side must contain letters only.");

		if (!Regex.IsMatch(right, "^[0-9]+$"))
			return Result<Product>.Error("ERROR: The right side must contain numbers only.");

		if (!int.TryParse(right, out var id) || (validateRange && (id < MinNumber || id > MaxNumber)))
			return Result<Product>.Error($"ERROR: The numeric part must be between {MinNumber} and {MaxNumber}.");

		return Result<Product>.Success(new Product(left, id));
	}
}