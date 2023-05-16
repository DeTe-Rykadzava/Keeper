using System;
using System.Collections.Generic;

namespace Keeper.Models;
public class ApplicationRequestModel
{
    public string Purpose { get; set; } = null!;
    public DateOnly ValidityApplicationBegin { get; set; }
    public DateOnly ValidityApplicationEnd { get; set; }
    public Nullable<int> UserId { get; set; }
    public string PasportB64 { get; set; } = null!;
    public string SubdivisionName { get; set; } = null!;
    public List<RequestCustomers> Customers { get; set; } = null!;
    public class RequestCustomers
    {
        public string Surname { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Patronymic { get; set; } = null!;
        public string? Phone { get; set; }
        public string Email { get; set; } = null!;
        public string? Organization { get; set; }
        public string Note { get; set; } = null!;
        public DateOnly BirthOfDate { get; set; }
        public string SeriaPasport { get; set; } = null!;
        public string NumberPasport { get; set; } = null!;
        public string? PhotoB64 { get; set; }
    }
}
