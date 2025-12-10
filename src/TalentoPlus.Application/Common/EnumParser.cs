using TalentoPlus.Domain.Enums;
using System.Globalization;

namespace TalentoPlus.Application.Common;

public static class EnumParser
{
    private static string Normalize(string value) =>
        value.Trim().ToLowerInvariant()
             .Replace("á", "a")
             .Replace("é", "e")
             .Replace("í", "i")
             .Replace("ó", "o")
             .Replace("ú", "u");

    public static DepartmentEnum ParseDepartment(string value)
    {
        value = Normalize(value);
        return value switch
        {
            "logistica" => DepartmentEnum.Logistica,
            "marketing" => DepartmentEnum.Marketing,
            "recursos humanos" => DepartmentEnum.RecursosHumanos,
            "operaciones" => DepartmentEnum.Operaciones,
            "ventas" => DepartmentEnum.Ventas,
            "tecnologia" => DepartmentEnum.Tecnologia,
            "contabilidad" => DepartmentEnum.Contabilidad,
            _ => DepartmentEnum.Tecnologia
        };
    }

    public static Position ParsePosition(string value)
    {
        value = Normalize(value);
        return value switch
        {
            "ingeniero" => Position.Ingeniero,
            "soporte tecnico" => Position.SoporteTecnico,
            "analista" => Position.Analista,
            "coordinador" => Position.Coordinador,
            "desarrollador" => Position.Desarrollador,
            "auxiliar" => Position.Auxiliar,
            _ => Position.Analista
        };
    }

    public static EducationLevelEnum ParseEducation(string value)
    {
        value = Normalize(value);
        return value switch
        {
            "tecnico" => EducationLevelEnum.Tecnico,
            "tecnologo" => EducationLevelEnum.Tecnologo,
            "profesional" => EducationLevelEnum.Profesional,
            "especializacion" => EducationLevelEnum.Especializacion,
            "maestria" => EducationLevelEnum.Maestria,
            _ => EducationLevelEnum.Profesional
        };
    }

    public static EmploymentStatus ParseStatus(string value)
    {
        value = Normalize(value);
        return value switch
        {
            "activo" => EmploymentStatus.Activo,
            "inactivo" => EmploymentStatus.Inactivo,
            "vacaciones" => EmploymentStatus.Vacaciones,
            _ => EmploymentStatus.Activo
        };
    }
}
