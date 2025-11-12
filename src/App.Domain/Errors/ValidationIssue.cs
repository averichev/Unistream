namespace App.Domain.Errors;

public sealed record ValidationIssue(string Field, string Message);
