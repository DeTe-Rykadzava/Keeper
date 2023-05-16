using System;
using System.Collections.Generic;

namespace KeeperAPI.DataBase;

public partial class Subdivision
{
    public int Id { get; set; }

    public string SubdivisionName { get; set; } = null!;

    public virtual ICollection<Application> Applications { get; set; } = new List<Application>();

    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();
}
