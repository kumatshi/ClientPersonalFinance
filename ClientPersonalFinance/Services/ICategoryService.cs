using ClientPersonalFinance.DTOs;

namespace ClientPersonalFinance.Services
{
    public interface ICategoryService
    {
        Task<ApiResponse<IEnumerable<CategoryDto>>> GetCategoriesAsync();
        Task<ApiResponse<CategoryDto>> GetCategoryByIdAsync(int id);
        Task<ApiResponse<IEnumerable<CategoryDto>>> GetIncomeCategoriesAsync();
        Task<ApiResponse<IEnumerable<CategoryDto>>> GetExpenseCategoriesAsync();
        Task<ApiResponse<CategoryDto>> CreateCategoryAsync(CreateCategoryDto category);
        Task<ApiResponse<CategoryDto>> UpdateCategoryAsync(int id, UpdateCategoryDto category);
        Task<ApiResponse<bool>> DeleteCategoryAsync(int id);
    }
}