using ClientPersonalFinance.DTOs;

namespace ClientPersonalFinance.Services
{
    public interface IAccountService
    {
        Task<ApiResponse<IEnumerable<AccountDto>>> GetAccountsAsync();
        Task<ApiResponse<AccountDetailDto>> GetAccountByIdAsync(int id);
        Task<ApiResponse<AccountDto>> CreateAccountAsync(CreateAccountDto account);
        Task<ApiResponse<AccountDto>> UpdateAccountAsync(int id, UpdateAccountDto account);
        Task<ApiResponse<bool>> DeleteAccountAsync(int id);
        Task<ApiResponse<object>> GetAccountsSummaryAsync();
    }
}