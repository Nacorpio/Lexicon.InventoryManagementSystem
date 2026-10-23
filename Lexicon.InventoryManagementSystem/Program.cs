using System.Diagnostics.CodeAnalysis;
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

	if (string.Equals(input.Trim(), "exit", StringComparison.OrdinalIgnoreCase))
		break;

	if (!int.TryParse(input, out var option))
		continue;

	if (option == 5)
		break;

	Console.Clear();

	status = option switch
	{
		1 => HandleAddProduct(),
		2 => HandleViewProducts(),
		3 => HandleSearchProducts(),
		4 => HandleDeleteProduct(),

		_ => Result.Error("ERROR: Invalid menu choice.")
	};
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

static bool? HandleYesNo (string message)
{
	if (!HandleInput($"{message} (y/n)", out var input))
		return false;

	input = input.Trim().ToLowerInvariant();

	switch (input)
	{
		case "y":
		case "yes":
			{
				return true;
			}

		case "n":
		case "no":
			{
				return false;
			}
	}

	return null;
}

Result HandleAddProduct ()
{
	if (!HandleInput("Enter a product name", out var input))
		return Result.Error("ERROR: Input cannot be empty.");

	var parsed = Product.Parse(input);

	if (!parsed.IsSuccess)
		return parsed;

	if (!inventory.TryAdd(parsed.Value))
		return Result.Error("WARNING: Product already exists.");

	return Result.Success($"Added product `{parsed.Value}` successfully.");
}

Result HandleViewProducts()
{
	if (inventory.ProductCount == 0)
		return Result.Error("ERROR: There are no products.");

	PrintStatistics(inventory.Products);

	Console.WriteLine();

	Console.WriteLine("Products");
	Console.WriteLine();

	PrintProducts(inventory.Products, true);

	Console.WriteLine();

	WaitForEnter();

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

	Console.WriteLine("Search query results");
	Console.WriteLine();

	PrintProducts(results);

	Console.WriteLine();
	WaitForEnter();

	return Result.Success();
}

static void WaitForEnter()
{
	Console.WriteLine("Press ENTER to continue");
	Console.ReadLine();
}

static void PrintProducts(IEnumerable<Product> products, bool numbered = false)
{
	var number = 1;

	foreach (var product in products.OrderBy(p => p.Name).ThenBy(p => p.Id))
		Console.WriteLine(numbered ? $"{number++}. {product}" : $"- {product}");
}

static void PrintStatistics(IReadOnlyCollection<Product> products)
{
	var numbers = products.Select(p => p.Id).ToArray();

	Console.WriteLine("Statistics");
	Console.WriteLine($"  Total products : {numbers.Length}");
	Console.WriteLine($"  Highest number : {numbers.Max()}");
	Console.WriteLine($"  Lowest number  : {numbers.Min()}");
	Console.WriteLine($"  Average number : {numbers.Average():F1}");
}

Result HandleDeleteProduct()
{
	if (inventory.ProductCount == 0)
		return Result.Error("ERROR: There are no products.");

	PrintProducts(inventory.Products);

	Console.WriteLine();

	if (!HandleInput("Enter the product to delete (for example AB-300), or leave empty to go back", out var input))
		return Result.Success();

	var parsed = Product.Parse(input, validateRange: false);

	if (!parsed.IsSuccess)
		return parsed;

	if (!inventory.Contains(parsed.Value))
		return Result.Error("ERROR: Couldn't find a matching product.");

	var answer = HandleYesNo($"Are you sure you want to delete product `{parsed.Value}`?");

	if (answer is null)
		return Result.Error("ERROR: Please answer `y` or `n`.");

	if (answer is false)
		return Result.Success();

	return inventory.Remove(parsed.Value)
		? Result.Success("Product removed successfully.")
		: Result.Error($"ERROR: Failed to remove product `{parsed.Value}`.");
}