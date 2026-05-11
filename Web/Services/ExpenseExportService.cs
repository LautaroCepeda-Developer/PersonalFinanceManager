using ClosedXML.Excel;
using Microsoft.Extensions.Localization;
using System.Globalization;
using System.Text;
using Web.DTOs.Expense;
using Web.Models;
using Web.Repositories.Expenses;
namespace Web.Services
{
    public class ExpenseExportService(IExpenseRepository repo, IStringLocalizer<ExportHeaders> localizer) : IExpenseExportService
    {
        private readonly IExpenseRepository _repo = repo;
        private readonly IStringLocalizer<ExportHeaders> _localizer = localizer;

        public async Task<byte[]> ExportExpensesAsync(ExpenseExportDTO dto)
        {
            var expenses = await _repo.GetFilteredExpensesAsync(dto.UserId, dto.From, dto.To, dto.MinAmount, dto.MaxAmount, dto.CategoryIds);

            return dto.Format.ToLower() switch
            {
                "csv" => GenerateCSV(expenses),
                _ => GenerateExcel(expenses)
            };
        }

        private byte[] GenerateCSV(IEnumerable<Expense> expenses)
        {
            StringBuilder sb = new();
            string separator = CultureInfo.CurrentCulture.TextInfo.ListSeparator;
            sb.AppendLine(string.Join(separator, [
                _localizer["DateHeader"],_localizer["CategoryHeader"],_localizer["AmountHeader"],_localizer["DescriptionHeader"]
            ]));
            
            foreach (var expense in expenses)
            {
                sb.AppendLine(
                    string.Join(separator,
                        expense.Date.ToString("d", CultureInfo.CurrentCulture),
                        expense.Category!.Name,
                        expense.Amount.ToString("N2", CultureInfo.CurrentCulture),
                        expense.Description
                    )
                );
            }

            return Encoding.UTF8.GetBytes(sb.ToString());
        }

        private byte[] GenerateExcel(IEnumerable<Expense> expenses)
        {
            using XLWorkbook workbook = new();
            var worksheet = workbook.Worksheets.Add(_localizer["WorksheetName"]);

            // Headers
            worksheet.Cell(1,1).Value = _localizer["DateHeader"].ToString();
            worksheet.Cell(1,1).Style.Font.Bold = true;
            worksheet.Cell(1,1).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

            worksheet.Cell(1,2).Value = _localizer["CategoryHeader"].ToString();
            worksheet.Cell(1,2).Style.Font.Bold = true;
            worksheet.Cell(1,2).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

            worksheet.Cell(1,3).Value = _localizer["AmountHeader"].ToString();
            worksheet.Cell(1,3).Style.Font.Bold = true;
            worksheet.Cell(1,3).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

            worksheet.Cell(1,4).Value = _localizer["DescriptionHeader"].ToString();
            worksheet.Cell(1,4).Style.Font.Bold = true;
            worksheet.Cell(1,4).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

            int row = 2;

            foreach (var expense in expenses)
            { 
                worksheet.Cell(row, 1).Value = expense.Date;
                worksheet.Cell(row, 1).Style.DateFormat.Format = "dd/mm/yyyy"; // Short date pattern
                worksheet.Cell(row, 1).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                worksheet.Cell(row, 2).Value = expense.Category!.Name;
                worksheet.Cell(row, 2).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                worksheet.Cell(row, 3).Value = expense.Amount;
                worksheet.Cell(row, 3).Style.NumberFormat.Format = "0.00"; // Number with 2 decimal places
                worksheet.Cell(row, 3).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                worksheet.Cell(row, 4).Value = expense.Description;
                worksheet.Cell(row, 4).Style.Border.OutsideBorder = XLBorderStyleValues.Thin;

                row++;
            }

            worksheet.Columns().AdjustToContents();
            worksheet.Rows().AdjustToContents();

            using MemoryStream stream = new();

            workbook.SaveAs(stream);

            return stream.ToArray();
        }
    }
}
