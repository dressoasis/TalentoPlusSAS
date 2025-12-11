using Xunit;
using FluentAssertions;
using TalentoPlus.Infrastructure.Integrations.Pdf;
using TalentoPlus.Domain.Entities;
using TalentoPlus.Domain.Enums;

namespace TalentoPlus.Tests.Unit;

public class PdfServiceTests
{
    private readonly IPdfService _pdfService;

    public PdfServiceTests()
    {
        _pdfService = new PdfService();
    }

    [Fact]
    public void GenerateEmployeeCV_ShouldReturnPdfBytes_WhenEmployeeIsValid()
    {
        // Arrange
        var employee = new Employee
        {
            Id = Guid.NewGuid(),
            FirstName = "María",
            LastName = "García",
            Email = "maria.garcia@test.com",
            Phone = "3001234567",
            Address = "Carrera 45 #12-34",
            BirthDate = new DateTime(1992, 5, 15),
            Salary = 4500000,
            HireDate = new DateTime(2020, 3, 1),
            ProfessionalProfile = "Ingeniera de Software con 5 años de experiencia en desarrollo web.",
            Department = DepartmentEnum.Tecnologia,
            Position = Position.Desarrollador,
            EducationLevel = EducationLevelEnum.Profesional,
            Status = EmploymentStatus.Activo,
            CreatedAt = DateTime.UtcNow
        };

        // Act
        var pdfBytes = _pdfService.GenerateEmployeeCV(employee);

        // Assert
        pdfBytes.Should().NotBeNull();
        pdfBytes.Should().NotBeEmpty();
        pdfBytes.Length.Should().BeGreaterThan(0);

        // Verificar que es un PDF válido (comienza con %PDF)
        var pdfHeader = System.Text.Encoding.ASCII.GetString(pdfBytes, 0, 4);
        pdfHeader.Should().Be("%PDF");
    }

    [Fact]
    public void GenerateEmployeeCV_ShouldIncludeEmployeeData_InPdf()
    {
        // Arrange
        var employee = new Employee
        {
            Id = Guid.NewGuid(),
            FirstName = "Carlos",
            LastName = "Rodríguez",
            Email = "carlos.rodriguez@test.com",
            Phone = "3159876543",
            Address = "Calle 80 #50-25",
            BirthDate = new DateTime(1988, 8, 20),
            Salary = 5000000,
            HireDate = new DateTime(2019, 1, 15),
            ProfessionalProfile = "Arquitecto de software especializado en microservicios.",
            Department = DepartmentEnum.Tecnologia,
            Position = Position.Ingeniero,
            EducationLevel = EducationLevelEnum.Maestria,
            Status = EmploymentStatus.Activo,
            CreatedAt = DateTime.UtcNow
        };

        // Act
        var pdfBytes = _pdfService.GenerateEmployeeCV(employee);

        // Assert
        pdfBytes.Should().NotBeNull();
        pdfBytes.Length.Should().BeGreaterThan(1000); // Un CV debe tener un tamaño razonable
        
        // Convertir a string para buscar datos (esto es simplificado)
        var pdfContent = System.Text.Encoding.UTF8.GetString(pdfBytes);
        
        // Verificar que contiene información del empleado
        pdfContent.Should().Contain("Carlos");
        pdfContent.Should().Contain("Rodríguez");
    }
}