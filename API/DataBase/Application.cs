using System;
using System.Collections.Generic;

namespace KeeperAPI.DataBase;

public partial class Application
{
    public int Id { get; set; }

    public string Purpose { get; set; } = null!;

    public DateOnly ValidityApplicationBegin { get; set; }

    public DateOnly ValidityApplicationEnd { get; set; }

    public DateTime? BeginVisit { get; set; }

    public DateTime? EndVisit { get; set; }

    public bool Access { get; set; }

    public DateTime? BeginVisitOnSubdivision { get; set; }

    public DateTime? EndVisitOnSubdivision { get; set; }

    public int SubdivisionId { get; set; }

    public int UserId { get; set; }

    public string? PasportB64 { get; set; }

    public string Status { get; set; } = null!;

    public string? StatusDescription { get; set; }

    public virtual ICollection<ApplicaitonCustomer> ApplicaitonCustomers { get; set; } = new List<ApplicaitonCustomer>();

    public virtual Subdivision Subdivision { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
