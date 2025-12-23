using ClientPersonalFinance.DTOs;

namespace ClientPersonalFinance.Services
{
    public interface ITransactionService
    {
        Task<ApiResponse<IEnumerable<TransactionDto>>> GetTransactionsAsync(int page = 1, int pageSize = 20);
        Task<ApiResponse<TransactionDto>> GetTransactionByIdAsync(int id);
        Task<ApiResponse<TransactionDto>> CreateTransactionAsync(CreateTransactionDto transaction);
        Task<ApiResponse<TransactionDto>> UpdateTransactionAsync(int id, UpdateTransactionDto transaction);
        Task<ApiResponse<bool>> DeleteTransactionAsync(int id);
        Task<ApiResponse<FinancialSummaryDto>> GetFinancialSummaryAsync(DateTime? startDate = null, DateTime? endDate = null);
    }
}