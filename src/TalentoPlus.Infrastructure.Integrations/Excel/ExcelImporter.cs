using ClosedXML.Excel;
using Microsoft.AspNetCore.Http;
using TalentoPlus.Application.Common;
using TalentoPlus.Application.DTOs.Employees;

namespace TalentoPlus.Infrastructure.Integrations;

public class ExcelImporter
{
    // ✅ NUEVO MÉTODO: Acepta IFormFile (lo que necesita el controlador)
    public async Task<List<EmployeeExcelDTO>> ParseEmployeeExcel(IFormFile file)
    {
        using var stream = file.OpenReadStream();
        return ParseEmployeeExcel(stream);
    }

    // ✅ MÉTODO ORIGINAL: Acepta Stream
    public List<EmployeeExcelDTO> ParseEmployeeExcel(Stream stream)
    {
        var list = new List<EmployeeExcelDTO>();
        
        using var workbook = new XLWorkbook(stream);
        var sheet = workbook.Worksheet(1);
        
        int row = 2; // Empezar desde la fila 2 (asumiendo que la 1 son headers)
        
        while (!sheet.Cell(row, 1).IsEmpty())
        {
            try
            {
                var dto = new EmployeeExcelDTO
                {
                    FirstName = sheet.Cell(row, 1).GetString(),
                    LastName = sheet.Cell(row, 2).GetString(),
                    BirthDate = DateTime.Now.AddYears(-sheet.Cell(row, 3).GetValue<int>()),
                    Email = sheet.Cell(row, 4).GetString(),
                    Phone = sheet.Cell(row, 5).GetString(),
                    Address = sheet.Cell(row, 6).GetString(),
                    Salary = sheet.Cell(row, 7).GetValue<decimal>(),
                    HireDate = sheet.Cell(row, 8).GetDateTime(),
                    ProfessionalProfile = sheet.Cell(row, 9).GetString(),
                    
                    // ENUMS con mapeo automático
                    Department = EnumParser.ParseDepartment(sheet.Cell(row, 10).GetString()),
                    Position = EnumParser.ParsePosition(sheet.Cell(row, 11).GetString()),
                    EducationLevel = EnumParser.ParseEducation(sheet.Cell(row, 12).GetString()),
                    Status = EnumParser.ParseStatus(sheet.Cell(row, 13).GetString())
                };
                
                list.Add(dto);
            }
            catch (Exception ex)
            {
                // Log o ignorar filas con errores
                Console.WriteLine($"Error en fila {row}: {ex.Message}");
            }
            
            row++;
        }
        
        return list;
    }
}