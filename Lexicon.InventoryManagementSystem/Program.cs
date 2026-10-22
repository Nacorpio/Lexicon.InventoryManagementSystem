using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;
using Lexicon.InventoryManagementSystem;

var inventory = new Inventory();
Result? status = null;

while (true)
{
	Console.Clear();

	Console.WriteLine("Inventory Management System");
	Console.WriteLine();

	if (status is { } result && !string.IsNullOrWhiteSpace(result.Message) )
	{
		PrintResult(result);
		Console.WriteLine();
	}

	status = null;

	Console.WriteLine("1. Add Product");
	Console.WriteLine("2. View Products");
	Console.WriteLine("3. Search Products"); 
	Console.WriteLine("4. Delete Products");
	Console.WriteLine("5. Exit");

	Console.WriteLine();

	Console.Write("Specify the number of a menu to open, or `exit` to quit: ");

	var input = Console.ReadLine();

	if (string.IsNullOrWhiteSpace(input))
		continue;

	if (string.Equals(input.Trim().ToLower(), "exit"))
		break;

	if (!int.TryParse(input, out var option))
		continue;

	Console.Clear();

	switch (option)
	{
		case 1: 
			{
				status = HandleAddProduct();
				break;
			}
		case 2:
			{
				status = HandleViewProducts();
				break;
			}
		case 3:
			{
				status = HandleSearchProducts();
				break;
			}
		case 5:
			{
				break;
			}

		default: 
			{
				break;
			}
	}
}

static void PrintResult ( Result result )
{
	Console.ForegroundColor = result.IsSuccess ? ConsoleColor.Green : ConsoleColor.Red;
	Console.WriteLine(result.Message);

	Console.ResetColor();
}

static bool HandleInput (string message, [NotNullWhen(true)] out string? result) 
{
	Console.Write($"{message}: ");

	var input = Console.ReadLine();

	if (string.IsNullOrWhiteSpace(input))
	{
		result = null;
		return false;
	}

	result = input;
	return true;
}

Result HandleAddProduct ()
{
	if (!HandleInput("Enter a product name", out var input))
		return Result.Error("ERROR: The product name cannot be empty.");

	var parsed = Product.Parse(input);

	if (!parsed.IsSuccess)
		return parsed;

	if (!inventory.TryAdd(parsed.Value))
		return Result.Error("WARNING: Product already exists.");

	return Result.Success($"Added product `{parsed.Value}` successfully.");
}

Result HandleViewProducts()
{
	var products = inventory.Products
		.OrderBy(p => p.Name)
		.ThenBy(p => p.Id)
		.ToArray();

	if (products.Length == 0)
		return Result.Error("ERROR: There are no products.");

	Console.WriteLine("Products");
	Console.WriteLine();

	for (var i = 0; i < products.Length; ++i)
	{
		Console.WriteLine($"{i + 1}. {products[i]}");
	}

	Console.WriteLine();
	Console.WriteLine("Press ENTER to continue");

	Console.ReadLine();

	return Result.Success();
}

Result HandleSearchProducts()
{
	if (!HandleInput("Enter a search query", out var input))
		return Result.Error("ERROR: The search query cannot be empty.");

	IReadOnlyList<Product> results = [.. (int.TryParse(input, out var id) 
		? inventory.GetProductsById(id) 
		: inventory.GetProductsByName(input))];

	Console.WriteLine();

	if (results.Count is 0)
		return Result.Error("ERROR: No products matched the given search query.");

	foreach (var result in results.OrderBy(p => p.Name).ThenBy(p => p.Id))
	{
		Console.WriteLine($"- {result}");
	}

	Console.WriteLine();
	Console.WriteLine("Press ENTER to continue");

	Console.ReadLine();

	return Result.Success();
}