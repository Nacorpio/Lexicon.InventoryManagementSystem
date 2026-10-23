using System.Diagnostics.CodeAnalysis;

namespace Lexicon.InventoryManagementSystem;

public record Result(string Message, bool IsSuccess)
{
	public static Result Error(string text) => new(text, false);
	public static Result Success(string text = "") => new(text, true);
}

public sealed record Result<T>
{
	private Result(T? value, string message, bool isSuccess)
	{
		Value = value;
		Text = message;
		IsSuccess = isSuccess;
	}

	public T? Value { get; }
	public string Text { get; }

	[MemberNotNullWhen(true, nameof(Value))]
	public bool IsSuccess { get; }

	public static Result<T> Success(T value, string message = "") => new(value, message, true);
	public static Result<T> Error(string message) => new(default, message, false);

	public static implicit operator Result(Result<T> result) => new(result.Text, result.IsSuccess);
}