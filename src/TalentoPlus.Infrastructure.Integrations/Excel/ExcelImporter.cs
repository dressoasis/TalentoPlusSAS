using ClosedXML.Excel;
using Microsoft.AspNetCore.Http;
using TalentoPlus.Application.Common;
using TalentoPlus.Application.DTOs.Employees;

namespace TalentoPlus.Infrastructure.Integrations
{
    public class ExcelImporter
    {
        public async Task<List<EmployeeExcelDTO>> ParseEmployeeExcel(IFormFile file)
        {
            using var stream = file.OpenReadStream();
            return ParseEmployeeExcel(stream);
        }

        public List<EmployeeExcelDTO> ParseEmployeeExcel(Stream stream)
        {
            var list = new List<EmployeeExcelDTO>();

            using var workbook = new XLWorkbook(stream);

            // 👇 Buscar hoja por nombre
            var sheet = workbook.Worksheet("Empleados");

            int row = 2; // Encabezados en fila 1

            // Mientras haya documento (columna A)
            while (!sheet.Cell(row, 1).IsEmpty())
            {
                try
                {
                    var dto = new EmployeeExcelDTO
                    {
                        Document = sheet.Cell(row, 1).GetString(),
                        FirstName = sheet.Cell(row, 2).GetString(),
                        LastName = sheet.Cell(row, 3).GetString(),
                        BirthDate = sheet.Cell(row, 4).GetDateTime(),
                        Address = sheet.Cell(row, 5).GetString(),
                        Phone = sheet.Cell(row, 6).GetString(),
                        Email = sheet.Cell(row, 7).GetString(),

                        // Cargo
                        Position = EnumParser.ParsePosition(sheet.Cell(row, 8).GetString()),

                        Salary = sheet.Cell(row, 9).GetValue<decimal>(),

                        HireDate = sheet.Cell(row, 10).GetDateTime(),

                        // Estado (Activo, Vacaciones, etc.)
                        Status = EnumParser.ParseStatus(sheet.Cell(row, 11).GetString()),

                        EducationLevel = EnumParser.ParseEducation(sheet.Cell(row, 12).GetString()),

                        ProfessionalProfile = sheet.Cell(row, 13).GetString(),

                        Department = EnumParser.ParseDepartment(sheet.Cell(row, 14).GetString())
                    };

                    list.Add(dto);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"⚠ Error en fila {row}: {ex.Message}");
                }

                row++;
            }

            return list;
        }
    }
}
