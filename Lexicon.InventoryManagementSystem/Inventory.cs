using System.Collections;

namespace Lexicon.InventoryManagementSystem;

public class Inventory : IEnumerable<Product>
{
	private readonly List<Product> _products = [];

	public int ProductCount => _products.Count;

	public IReadOnlyList<Product> Products => _products;

	public bool TryAdd(Product product)
	{
		if (Contains(product))
			return false;

		_products.Add(product);
		return true;
	}

	public bool Remove(Product product)
	{
		return _products.Remove(product);
	}

	public bool Contains(string name) =>
		_products.Exists(p => string.Equals(name, p.Name, StringComparison.InvariantCultureIgnoreCase));

	public bool Contains(int number) =>
		_products.Exists(p => p.Id == number);

	public bool Contains(Product product) =>
		_products.Exists(p => string.Equals(product.Name, p.Name, StringComparison.InvariantCultureIgnoreCase) && product.Id == p.Id);


	public IEnumerable<Product> GetProductsByName(string name) => _products
		.Where(p => string.Equals(name, p.Name, StringComparison.InvariantCultureIgnoreCase));

	public IEnumerable<Product> GetProductsById(int number) => _products
		.Where(p => p.Id == number);


	public IEnumerator<Product> GetEnumerator() => _products.GetEnumerator();

	IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}