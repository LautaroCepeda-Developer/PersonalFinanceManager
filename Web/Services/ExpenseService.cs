using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Web.DTOs;
using Web.DTOs.Expense;
using Web.Models;
using Web.Repositories.Expenses;

namespace Web.Services
{
    public class ExpenseService(IExpenseRepository repo, IMapper mapper, IStringLocalizer<ValidationMessages> localizer) : IExpenseService
    {
        private readonly IMapper _mapper = mapper;
        private readonly IExpenseRepository _repo = repo;
        private readonly IStringLocalizer<ValidationMessages> _localizer = localizer;

        public async Task<OperationResult> AddExpenseAsync(ExpenseCreateDTO dto)
        {
            if (dto.Amount <= 0) return OperationResult.Fail(_localizer["AmountRangeError"]);
            if (dto.Date > DateTime.UtcNow.AddMinutes(1)) return OperationResult.Fail(_localizer["DateFutureError"]);

            Expense expense = _mapper.Map<ExpenseCreateDTO, Expense>(dto);

            await _repo.AddAsync(expense);
            await _repo.SaveChangesAsync();
            return OperationResult.Ok();
        }

        public async Task<ExpenseDTO> GetExpenseByIdAsync(int expenseId)
        {
            Expense? expense =  await _repo.GetByIdAsync(expenseId);
            if (expense is null) return null!;
            return _mapper.Map<Expense, ExpenseDTO>(expense);
        }

        public async Task<IEnumerable<ExpenseDTO>> GetUserExpensesAsync(string userId, DateTime? from, DateTime? to, int? categoryId)
        {
            IEnumerable<Expense> expenses = await _repo.GetByUserIdAsync(userId, from, to, categoryId);

            return _mapper.Map<IEnumerable<Expense>, IEnumerable<ExpenseDTO>>(expenses);
        }

        public async Task<IEnumerable<ExpenseDTO>> GetFilteredAsync(DateTime? from, DateTime? to, int? categoryId)
        {
            var query = _repo.Query();

            if (from.HasValue) query = query.Where(e => e.Date >= from.Value);

            if (to.HasValue) query = query.Where(e => e.Date <= to.Value);

            if (categoryId.HasValue) query = query.Where(e => e.CategoryId == categoryId.Value);

            return _mapper.Map<IEnumerable<Expense>, IEnumerable<ExpenseDTO>>(await query.ToListAsync());
        }

        public async Task<decimal>GetTotalAsync(string userId, DateTime? from, DateTime? to)
            => await _repo.GetTotalByUserAndPeriodAsync(userId, from, to);

        public async Task<IEnumerable<(string Label, decimal Amount)>> GetMonthlyTotalsAsync(string userId, int year)
            => await _repo.GetMonthlyTotalsAsync(userId, year);

        public async Task<OperationResult> UpdateExpenseAsync(ExpenseUpdateDTO dto)
        {
            Expense? existingExpense = await _repo.GetByIdAsync(dto.Id);
            if (existingExpense is null)
                return OperationResult.Fail(_localizer["ExpenseNotFoundError", dto.Id]);
            if (dto.Amount <= 0) return OperationResult.Fail(_localizer["AmountRangeError"]);
            if (dto.Date > DateTime.UtcNow.AddMinutes(1)) return OperationResult.Fail(_localizer["DateFutureError"]);

            _mapper.Map(dto, existingExpense);
            _repo.Update(existingExpense);
            await _repo.SaveChangesAsync();
            return OperationResult.Ok();
        }

        public async Task<OperationResult> DeleteExpenseAsync(int expenseId)
        {
            Expense? existingExpense = await _repo.GetByIdAsync(expenseId);
            if (existingExpense is null)
                return OperationResult.Fail(_localizer["ExpenseNotFoundError", expenseId]);
            _repo.Remove(existingExpense);
            await _repo.SaveChangesAsync();
            return OperationResult.Ok();
        }
    }
}
