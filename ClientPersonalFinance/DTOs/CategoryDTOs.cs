using System.ComponentModel.DataAnnotations;

namespace ClientPersonalFinance.DTOs
{
    public class CategoryDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty; // "Income" или "Expense"
        public decimal MonthlyBudget { get; set; }
        public int TransactionCount { get; set; }
        public decimal TotalSpent { get; set; }
        public decimal BudgetProgress => MonthlyBudget > 0 ? (TotalSpent / MonthlyBudget) * 100 : 0;
    }

    public class CreateCategoryDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;

        public string Color { get; set; } = "#2196F3";
        public string Icon { get; set; } = "category";

        [Required]
        public int Type { get; set; } // 0 - Income, 1 - Expense

        public decimal MonthlyBudget { get; set; }
    }

    public class UpdateCategoryDto
    {
        [Required]
        public string Name { get; set; } = string.Empty;
        public string Color { get; set; } = string.Empty;
        public string Icon { get; set; } = string.Empty;
        public decimal MonthlyBudget { get; set; }
    }
}