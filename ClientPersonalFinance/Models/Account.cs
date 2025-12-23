namespace ClientPersonalFinance.Models
{
    public class Account
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Balance { get; set; }
        public string Currency { get; set; } = "RUB";
        public AccountType Type { get; set; }
        public int UserId { get; set; }
    }

    public enum AccountType
    {
        Cash,
        BankCard,
        CreditCard,
        Savings,
        Investment
    }
}