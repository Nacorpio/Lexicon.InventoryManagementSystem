using System.Numerics;
using System.Text.RegularExpressions;

namespace Lexicon.InventoryManagementSystem;

public sealed record Product(string Name, int Id)
{
	const int MinNumber = 200;
	const int MaxNumber = 500;

	public int Id { get; } = Id;
	public string Name { get; } = Name.ToUpper();

	public override string ToString() => $"{Name}-{Id}";

	public static Result<Product> Parse (string message)
	{
		var match = Regex.Match(message, "^(?<name>[A-Za-z]+)?-(?<id>\\d+)?$");

		if (!match.Success)
			return Result<Product>.Error("The product must follow the `XXX-999` format.");

		var nameGroup = match.Groups["name"];
		var idGroup = match.Groups["id"];

		if (!nameGroup.Success)
			return Result<Product>.Error("The name part is missing.");

		if (!idGroup.Success)
			return Result<Product>.Error("The number part is missing.");

		if (!int.TryParse(idGroup.Value, out var id) || id < MinNumber || id > MaxNumber)
			return Result<Product>.Error($"The number must be between {MinNumber} and {MaxNumber}.");

		return Result<Product>.Success(new Product(nameGroup.Value.ToUpperInvariant(), id));
	}
}