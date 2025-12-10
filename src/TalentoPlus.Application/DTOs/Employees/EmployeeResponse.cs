namespace TalentoPlus.Application.DTOs.Employees;

public class EmployeeResponse
{
    public Guid Id { get; set; }
    public string FullName { get; set; }

    public string Email { get; set; }
    public string Phone { get; set; }
    public decimal Salary { get; set; }

    public string Status { get; set; }

    public string Department { get; set; }
    public string Position { get; set; }
    public string EducationLevel { get; set; }
}
