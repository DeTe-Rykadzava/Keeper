namespace KeeperAPI.Models;
public class ApplicationPutModel
{
    public DateTime? BeginVisit { get; set; }
    public DateTime? EndVisit { get; set; }
    public DateTime? BeginVisitOnSubdivision { get; set; }
    public DateTime? EndVisitOnSubdivision { get; set; }
    public string? Status { get; set; }
    public string? StatusDescription { get; set; }
}
