namespace ClientPersonalFinance.DTOs
{
    public class AccountDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Balance { get; set; }
        public string Currency { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty; // "Cash", "BankCard", etc.
        public int TransactionCount { get; set; }
        public DateTime CreatedDate { get; set; }
    }

    public class CreateAccountDto
    {
        public string Name { get; set; } = string.Empty;
        public decimal Balance { get; set; }
        public string Currency { get; set; } = string.Empty;
        public int Type { get; set; } // 0 - Cash, 1 - BankCard, etc.
    }

    public class UpdateAccountDto
    {
        public string Name { get; set; } = string.Empty;
        public string Currency { get; set; } = string.Empty;
        public int Type { get; set; }
    }

    public class AccountDetailDto : AccountDto
    {
        public List<TransactionDto> RecentTransactions { get; set; } = new();
        public decimal MonthlyIncome { get; set; }
        public decimal MonthlyExpense { get; set; }
    }
}