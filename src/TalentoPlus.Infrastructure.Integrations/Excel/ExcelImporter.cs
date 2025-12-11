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

            // ================================================================
            // 🔥 IMPRIMIR HOJAS DETECTADAS POR CLOSEDXML
            // ================================================================
            Console.WriteLine("=== HOJAS DETECTADAS EN EL EXCEL ===");

            using var workbook = new XLWorkbook(stream);

            foreach (var ws in workbook.Worksheets)
            {
                Console.WriteLine(ws.Name);
            }

            Console.WriteLine("====================================");


            // ================================================================
            // 📌 Usar la PRIMERA hoja (sin importar el nombre)
            // ================================================================
            var sheet = workbook.Worksheet(1); // Primera hoja

            if (sheet == null)
            {
                Console.WriteLine("❌ No se encontró ninguna hoja en el Excel.");
                return list;
            }

            Console.WriteLine($"✅ Procesando hoja: {sheet.Name}");


            // ================================================================
            // 📌 Procesar filas desde Fila 2
            // ================================================================
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
                        BirthDate = GetDateValue(sheet.Cell(row, 4)),
                        Address = sheet.Cell(row, 5).GetString(),
                        Phone = sheet.Cell(row, 6).GetString(),
                        Email = sheet.Cell(row, 7).GetString(),

                        // Cargo
                        Position = EnumParser.ParsePosition(sheet.Cell(row, 8).GetString()),

                        Salary = GetDecimalValue(sheet.Cell(row, 9)),

                        HireDate = GetDateValue(sheet.Cell(row, 10)),

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

        private DateTime GetDateValue(IXLCell cell)
        {
            DateTime dateValue;
            
            if (cell.TryGetValue(out DateTime cellDate))
            {
                dateValue = cellDate;
            }
            else
            {
                // Intentar parsear como string
                var cellValue = cell.GetString();
                if (!DateTime.TryParse(cellValue, out dateValue))
                {
                    throw new FormatException($"No se pudo convertir '{cellValue}' a fecha");
                }
            }
            
            // PostgreSQL requiere UTC - convertir si no lo es
            if (dateValue.Kind == DateTimeKind.Unspecified)
            {
                dateValue = DateTime.SpecifyKind(dateValue, DateTimeKind.Utc);
            }
            else if (dateValue.Kind == DateTimeKind.Local)
            {
                dateValue = dateValue.ToUniversalTime();
            }
            
            return dateValue;
        }

        private decimal GetDecimalValue(IXLCell cell)
        {
            if (cell.TryGetValue(out double doubleValue))
                return (decimal)doubleValue;
            
            if (cell.TryGetValue(out decimal decimalValue))
                return decimalValue;
            
            // Intentar parsear como string
            var cellValue = cell.GetString();
            if (decimal.TryParse(cellValue, out decimal parsedDecimal))
                return parsedDecimal;
            
            throw new FormatException($"No se pudo convertir '{cellValue}' a número");
        }
    }
}

