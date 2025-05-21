namespace BookStore.Api.Models;

public record Book(int Id, string Title, string Author, string Publisher, DateOnly PublishedDate);