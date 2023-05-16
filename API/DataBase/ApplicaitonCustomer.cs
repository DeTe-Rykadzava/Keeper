using System;
using System.Collections.Generic;

namespace KeeperAPI.DataBase;

public partial class ApplicaitonCustomer
{
    public int Id { get; set; }

    public int ApplicationId { get; set; }

    public int CustomerId { get; set; }

    public int? GroupNumber { get; set; }

    public virtual Application Application { get; set; } = null!;

    public virtual Customer Customer { get; set; } = null!;
}
