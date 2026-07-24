using ClosedXML.Excel;
using FSM.Application.DTOs.Technicians;
using FSM.Application.DTOs.WorkOrders;
using FSM.Application.Interfaces;
using FSM.Domain.Entities;
using FSM.Domain.Enums;
using FSM.Domain.Interfaces;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using Microsoft.EntityFrameworkCore;
using iText.Kernel.Font;
using iText.IO.Font.Constants;

namespace FSM.Infrastructure.Services;

public class ReportService : IReportService
{
    private readonly IGenericRepository<WorkOrder> _workOrderRepository;
    private readonly IGenericRepository<Technician> _technicianRepository;

    public ReportService(
        IGenericRepository<WorkOrder> workOrderRepository,
        IGenericRepository<Technician> technicianRepository)
    {
        _workOrderRepository = workOrderRepository;
        _technicianRepository = technicianRepository;
    }

    // ============ EXCEL: İş Emirleri Raporu ============
    public async Task<byte[]> GenerateWorkOrdersExcelAsync()
    {
        var workOrders = await _workOrderRepository.GetAllAsQueryable()
            .Include(w => w.Technician)
            .Where(w => !w.IsDeleted)
            .ToListAsync();

        using var workbook = new XLWorkbook();
        var sheet = workbook.Worksheets.Add("İş Emirleri");

        // Başlık satırı
        sheet.Cell(1, 1).Value = "ID";
        sheet.Cell(1, 2).Value = "Başlık";
        sheet.Cell(1, 3).Value = "Durum";
        sheet.Cell(1, 4).Value = "Teknisyen";
        sheet.Cell(1, 5).Value = "Planlanan Başlangıç";
        sheet.Cell(1, 6).Value = "Planlanan Bitiş";
        sheet.Cell(1, 7).Value = "Oluşturulma Tarihi";
        sheet.Row(1).Style.Font.Bold = true;

        int row = 2;
        foreach (var wo in workOrders)
        {
            sheet.Cell(row, 1).Value = wo.Id;
            sheet.Cell(row, 2).Value = wo.Title;
            sheet.Cell(row, 3).Value = wo.State.ToString();
            sheet.Cell(row, 4).Value = wo.Technician?.FullName ?? "Atanmadı";
            sheet.Cell(row, 5).Value = wo.ScheduledStartDate?.ToString("dd.MM.yyyy HH:mm") ?? "-";
            sheet.Cell(row, 6).Value = wo.ScheduledEndDate?.ToString("dd.MM.yyyy HH:mm") ?? "-";
            sheet.Cell(row, 7).Value = wo.CreatedAt.ToString("dd.MM.yyyy HH:mm");
            row++;
        }

        sheet.Columns().AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    // ============ PDF: Teknisyen Performans Raporu ============
    public async Task<byte[]> GenerateTechnicianPerformancePdfAsync()
    {
        var technicians = await _technicianRepository.GetAllAsync();

        using var stream = new MemoryStream();
        using var pdfWriter = new PdfWriter(stream);
        using var pdfDoc = new PdfDocument(pdfWriter);
        var document = new Document(pdfDoc);

        // 1. Standart Helvetica Bold fontunu oluşturuyoruz
        var boldFont = PdfFontFactory.CreateFont(StandardFonts.HELVETICA_BOLD);

        // 2. Paragrafa doğrudan bu bold fontu uyguluyoruz
        document.Add(new Paragraph("Teknisyen Performans Raporu")
            .SetFont(boldFont)
            .SetFontSize(18));

        // Sonra bu biçimlendirilmiş metni paragrafa ekleyip dokümana basıyoruz
        document.Add(new Paragraph($"Oluşturulma Tarihi: {DateTime.Now:dd.MM.yyyy HH:mm}")
            .SetFontSize(10));
        document.Add(new Paragraph(" ")); // boşluk

        var table = new Table(4).UseAllAvailableWidth();
        table.AddHeaderCell("Ad Soyad");
        table.AddHeaderCell("E-posta");
        table.AddHeaderCell("Aktif İş Sayısı");
        table.AddHeaderCell("Durum");

        foreach (var tech in technicians.Where(t => !t.IsDeleted))
        {
            var activeCount = await _workOrderRepository.GetAllAsQueryable()
                .Where(w => w.TechnicianId == tech.Id && !w.IsDeleted && w.State != WorkOrderState.Completed)
                .CountAsync();

            table.AddCell(tech.FullName);
            table.AddCell(tech.Email);
            table.AddCell(activeCount.ToString());
            table.AddCell(tech.IsAvailable ? "Uygun" : "Meşgul");
        }

        document.Add(table);
        document.Close();

        return stream.ToArray();
    }
}