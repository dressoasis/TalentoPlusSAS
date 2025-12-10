using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using TalentoPlus.Domain.Entities;

namespace TalentoPlus.Infrastructure.Integrations.Pdf;

public interface IPdfService
{
    byte[] GenerateEmployeeCV(Employee employee);
}

public class PdfService : IPdfService
{
    public PdfService()
    {
        // Configurar licencia (Community para uso no comercial)
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public byte[] GenerateEmployeeCV(Employee employee)
    {
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(11).FontFamily("Arial"));

                // HEADER
                page.Header().Element(c => ComposeHeader(c, employee));

                // CONTENT
                page.Content().Element(c => ComposeContent(c, employee));

                // FOOTER
                page.Footer().AlignCenter().Text(txt =>
                {
                    txt.Span("Generado el ");
                    txt.Span(DateTime.Now.ToString("dd/MM/yyyy HH:mm")).SemiBold();
                    txt.Span(" | TalentoPlus S.A.S.");
                });
            });
        });

        return document.GeneratePdf();
    }

    private void ComposeHeader(IContainer container, Employee employee)
    {
        container.Column(column =>
        {
            // Título principal
            column.Item().Background(Colors.Blue.Darken3).Padding(10).Row(row =>
            {
                row.RelativeItem().Column(col =>
                {
                    col.Item().Text("HOJA DE VIDA")
                        .FontSize(20)
                        .Bold()
                        .FontColor(Colors.White);
                    
                    col.Item().Text($"{employee.FirstName} {employee.LastName}")
                        .FontSize(16)
                        .SemiBold()
                        .FontColor(Colors.White);
                });
            });

            column.Item().PaddingVertical(5);
        });
    }

    private void ComposeContent(IContainer container, Employee employee)
    {
        container.PaddingVertical(10).Column(column =>
        {
            // DATOS PERSONALES
            column.Item().Element(c => ComposeSection(c, "DATOS PERSONALES", () =>
            {
                return new Dictionary<string, string>
                {
                    { "Nombre Completo", $"{employee.FirstName} {employee.LastName}" },
                    { "Fecha de Nacimiento", employee.BirthDate.ToString("dd/MM/yyyy") },
                    { "Edad", CalculateAge(employee.BirthDate).ToString() + " años" },
                    { "Email", employee.Email },
                    { "Teléfono", employee.Phone },
                    { "Dirección", employee.Address }
                };
            }));

            column.Item().PaddingVertical(5);

            // INFORMACIÓN LABORAL
            column.Item().Element(c => ComposeSection(c, "INFORMACIÓN LABORAL", () =>
            {
                return new Dictionary<string, string>
                {
                    { "Cargo", FormatEnum(employee.Position) },
                    { "Departamento", FormatEnum(employee.Department) },
                    { "Fecha de Ingreso", employee.HireDate.ToString("dd/MM/yyyy") },
                    { "Antigüedad", CalculateAntiquity(employee.HireDate) },
                    { "Salario", $"${employee.Salary:N0}" },
                    { "Estado", FormatEnum(employee.Status) }
                };
            }));

            column.Item().PaddingVertical(5);

            // FORMACIÓN ACADÉMICA
            column.Item().Element(c => ComposeSection(c, "FORMACIÓN ACADÉMICA", () =>
            {
                return new Dictionary<string, string>
                {
                    { "Nivel Educativo", FormatEnum(employee.EducationLevel) }
                };
            }));

            column.Item().PaddingVertical(5);

            // PERFIL PROFESIONAL
            column.Item().Element(c => ComposePerfil(c, employee.ProfessionalProfile));
        });
    }

    private void ComposeSection(IContainer container, string title, Func<Dictionary<string, string>> getData)
    {
        container.Column(column =>
        {
            // Título de sección
            column.Item()
                .Background(Colors.Grey.Lighten3)
                .Padding(8)
                .Text(title)
                .FontSize(12)
                .Bold();

            // Contenido
            column.Item().Padding(10).Column(innerColumn =>
            {
                var data = getData();
                foreach (var item in data)
                {
                    innerColumn.Item().PaddingBottom(5).Row(row =>
                    {
                        row.ConstantItem(150).Text(item.Key + ":").SemiBold();
                        row.RelativeItem().Text(item.Value);
                    });
                }
            });
        });
    }

    private void ComposePerfil(IContainer container, string perfil)
    {
        container.Column(column =>
        {
            column.Item()
                .Background(Colors.Grey.Lighten3)
                .Padding(8)
                .Text("PERFIL PROFESIONAL")
                .FontSize(12)
                .Bold();

            column.Item()
                .Padding(10)
                .Text(perfil ?? "No especificado")
                .FontSize(11)
                .Justify();
        });
    }

    private int CalculateAge(DateTime birthDate)
    {
        var today = DateTime.Today;
        var age = today.Year - birthDate.Year;
        if (birthDate.Date > today.AddYears(-age)) age--;
        return age;
    }

    private string CalculateAntiquity(DateTime hireDate)
    {
        var diff = DateTime.Now - hireDate;
        var years = diff.Days / 365;
        var months = (diff.Days % 365) / 30;
        
        if (years > 0 && months > 0)
            return $"{years} año(s) y {months} mes(es)";
        else if (years > 0)
            return $"{years} año(s)";
        else if (months > 0)
            return $"{months} mes(es)";
        else
            return $"{diff.Days} día(s)";
    }

    private string FormatEnum(Enum value)
    {
        if (value == null) return "";
        string text = value.ToString();
        // Insert space before capital letters (except the first one)
        return System.Text.RegularExpressions.Regex.Replace(text, "(\\B[A-Z])", " $1");
    }
}