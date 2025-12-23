namespace ClientPersonalFinance.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; } = string.Empty;
        public DateTime Date { get; set; } = DateTime.UtcNow;
        public TransactionType Type { get; set; }
        public int CategoryId { get; set; }
        public int AccountId { get; set; }
        public int UserId { get; set; }
    }

    public enum TransactionType
    {
        Income,
        Expense
    }
}