using System.Text.Json.Serialization;

namespace EducationSystem.Domain.Enums;

//[JsonConverter(typeof(JsonStringEnumConverter))]
public enum SchoolStatus
{
    Active = 1,
    Inactive = 2,
    Closed = 3,
}