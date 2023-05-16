using System;
using System.Collections.Generic;

namespace KeeperAPI.DataBase;

public partial class Employee
{
    public int Id { get; set; }

    public int EmployeeCode { get; set; }

    public string FullName { get; set; } = null!;

    public int? SubdivisionId { get; set; }

    public int? DepartmentId { get; set; }

    public virtual Department? Department { get; set; }

    public virtual Subdivision? Subdivision { get; set; }
}
