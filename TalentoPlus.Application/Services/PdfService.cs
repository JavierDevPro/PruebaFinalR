using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using TalentoPlus.Application.DTOs;
using TalentoPlus.Domain.Entities;

namespace TalentoPlus.Application.Services;

public class PdfService
{
    public byte[] GenerateHojaDeVida(EmpleadoDto empleado)
    {
        QuestPDF.Settings.License = LicenseType.Community;
        
        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(2, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(11));
                
                page.Header()
                    .AlignCenter()
                    .Text("HOJA DE VIDA - TALENTOPLUS S.A.S.")
                    .SemiBold().FontSize(18).FontColor(Colors.Blue.Darken3);
                
                page.Content()
                    .PaddingVertical(1, Unit.Centimetre)
                    .Column(column =>
                    {
                        column.Item().Text($"INFORMACIÓN PERSONAL").Bold().FontSize(14);
                        column.Item().PaddingBottom(5).LineHorizontal(1).LineColor(Colors.Grey.Lighten1);
                        
                        column.Item().Grid(grid =>
                        {
                            grid.Columns(2);
                            grid.Item().Text($"Documento: {empleado.Documento}");
                            grid.Item().Text($"Nombre: {empleado.Nombres} {empleado.Apellidos}");
                            grid.Item().Text($"Fecha Nacimiento: {empleado.FechaNacimiento:dd/MM/yyyy}");
                            grid.Item().Text($"Teléfono: {empleado.Telefono}");
                            grid.Item().Text($"Email: {empleado.Email}");
                            grid.Item().Text($"Dirección: {empleado.Direccion}");
                        });
                        
                        column.Item().PaddingTop(10).Text($"INFORMACIÓN LABORAL").Bold().FontSize(14);
                        column.Item().PaddingBottom(5).LineHorizontal(1).LineColor(Colors.Grey.Lighten1);
                        
                        column.Item().Grid(grid =>
                        {
                            grid.Columns(2);
                            grid.Item().Text($"Cargo: {empleado.Cargo}");
                            grid.Item().Text($"Departamento: {empleado.DepartamentoNombre}");
                            grid.Item().Text($"Salario: ${empleado.Salario:N0}");
                            grid.Item().Text($"Fecha Ingreso: {empleado.FechaIngreso:dd/MM/yyyy}");
                            grid.Item().Text($"Estado: {empleado.Estado}");
                            grid.Item().Text($"Nivel Educativo: {empleado.NivelEducativo}");
                        });
                        
                        if (!string.IsNullOrEmpty(empleado.PerfilProfesional))
                        {
                            column.Item().PaddingTop(10).Text($"PERFIL PROFESIONAL").Bold().FontSize(14);
                            column.Item().PaddingBottom(5).LineHorizontal(1).LineColor(Colors.Grey.Lighten1);
                            column.Item().Text(empleado.PerfilProfesional);
                        }
                    });
                
                page.Footer()
                    .AlignCenter()
                    .Text(text =>
                    {
                        text.Span("Generado el: ");
                        text.Span($"{DateTime.Now:dd/MM/yyyy HH:mm}");
                    });
            });
        });
        
        return document.GeneratePdf();
    }
}