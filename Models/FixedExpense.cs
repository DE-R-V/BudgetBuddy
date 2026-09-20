public class FixedExpense
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public decimal Estimated { get; set; }
    public decimal Allocated { get; set; }
}