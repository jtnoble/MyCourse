namespace MyCourseCore.Models
{
    // Optional contract for reportable entities
    public interface IReportable
    {
        string[] ToReportRow();
        string GetReportHeader();
    }
}