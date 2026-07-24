namespace FSM.Application.Interfaces;

public interface IReportService
{
    Task<byte[]> GenerateWorkOrdersExcelAsync();
    Task<byte[]> GenerateTechnicianPerformancePdfAsync();
}