using System;
using System.Collections.Generic;

namespace KeeperAPI.DataBase;

public partial class Customer
{
    public int Id { get; set; }

    public string Surname { get; set; } = null!;

    public string Name { get; set; } = null!;

    public string Patronymic { get; set; } = null!;

    public string? Phone { get; set; }

    public string Email { get; set; } = null!;

    public string? Organization { get; set; }

    public string Note { get; set; } = null!;

    public string SeriaPasport { get; set; } = null!;

    public string NumberPasport { get; set; } = null!;

    public string? PhotoB64 { get; set; }

    public bool Blocked { get; set; }

    public DateOnly BitrthOfDate { get; set; }

    public virtual ICollection<ApplicaitonCustomer> ApplicaitonCustomers { get; set; } = new List<ApplicaitonCustomer>();
}
