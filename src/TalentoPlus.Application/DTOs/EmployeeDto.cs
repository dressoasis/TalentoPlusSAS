namespace TalentoPlus.Application.DTOs.Employees
{
    public class EmployeeExcelDTO
    {
        public string Document { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public DateTime BirthDate { get; set; }
        public string Address { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Position { get; set; } = string.Empty;
        public decimal Salary { get; set; }
        public DateTime HireDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public string EducationLevel { get; set; } = string.Empty;
        public string ProfessionalProfile { get; set; } = string.Empty;
        public string Department { get; set; } = string.Empty;
    }
}
