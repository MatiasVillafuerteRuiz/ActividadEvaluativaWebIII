using GestionClinicaVeterinaria.Models;
using GestionClinicaVeterinaria.Models.ViewModels;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace GestionClinicaVeterinaria.Services
{
    public class ReportePdfService
    {
        // =========================================================
        // REPORTE GENERAL DE CITAS
        // =========================================================

        public byte[] GenerarReporteGeneral(
            List<Cita> citas)
        {
            var documento = Document.Create(container =>
            {
                container.Page(page =>
                {
                    // ---------------------------------------------
                    // CONFIGURACIÓN
                    // ---------------------------------------------

                    page.Size(PageSizes.A4);

                    page.Margin(30);

                    page.DefaultTextStyle(x =>
                        x.FontSize(10));


                    // ---------------------------------------------
                    // ENCABEZADO
                    // ---------------------------------------------

                    page.Header()
                        .Column(column =>
                        {
                            column.Item()
                                .Text("CLÍNICA VETERINARIA")
                                .FontSize(20)
                                .Bold();

                            column.Item()
                                .Text("Reporte general de citas")
                                .FontSize(14);

                            column.Item()
                                .Text(
                                    $"Generado: {DateTime.Now:dd/MM/yyyy HH:mm}")
                                .FontSize(9)
                                .FontColor(
                                    Colors.Grey.Darken1);
                        });


                    // ---------------------------------------------
                    // CONTENIDO
                    // ---------------------------------------------

                    page.Content()
                        .PaddingVertical(20)
                        .Column(column =>
                        {
                            column.Item()
                                .Text(
                                    $"Total de citas: {citas.Count}")
                                .Bold();


                            if (citas.Count == 0)
                            {
                                column.Item()
                                    .PaddingTop(20)
                                    .Text(
                                        "No existen citas registradas.");
                            }
                            else
                            {
                                column.Item()
                                    .PaddingTop(15)
                                    .Table(table =>
                                    {
                                        // -------------------------
                                        // COLUMNAS
                                        // -------------------------

                                        table.ColumnsDefinition(
                                            columns =>
                                            {
                                                columns.RelativeColumn(2);

                                                columns.RelativeColumn(2);

                                                columns.RelativeColumn(2);

                                                columns.RelativeColumn(2);

                                                columns.RelativeColumn(1);
                                            });


                                        // -------------------------
                                        // ENCABEZADO DE TABLA
                                        // -------------------------

                                        table.Header(header =>
                                        {
                                            header.Cell()
                                                .Element(HeaderCell)
                                                .Text("Mascota")
                                                .Bold();

                                            header.Cell()
                                                .Element(HeaderCell)
                                                .Text("Cliente")
                                                .Bold();

                                            header.Cell()
                                                .Element(HeaderCell)
                                                .Text("Servicio")
                                                .Bold();

                                            header.Cell()
                                                .Element(HeaderCell)
                                                .Text("Fecha")
                                                .Bold();

                                            header.Cell()
                                                .Element(HeaderCell)
                                                .Text("Estado")
                                                .Bold();
                                        });


                                        // -------------------------
                                        // DATOS
                                        // -------------------------

                                        foreach (var cita in citas)
                                        {
                                            // Mascota

                                            table.Cell()
                                                .Element(DataCell)
                                                .Text(
                                                    cita.Mascota?.Nombre
                                                    ?? "-");


                                            // Cliente

                                            var nombreCliente = "-";

                                            if (cita.Mascota?.Usuario != null)
                                            {
                                                nombreCliente =
                                                    $"{cita.Mascota.Usuario.Nombre} " +
                                                    $"{cita.Mascota.Usuario.Apellido}";
                                            }

                                            table.Cell()
                                                .Element(DataCell)
                                                .Text(nombreCliente);


                                            // Servicio

                                            table.Cell()
                                                .Element(DataCell)
                                                .Text(
                                                    cita.ServicioVeterinario
                                                        ?.NombreServicio
                                                    ?? "-");


                                            // Fecha

                                            table.Cell()
                                                .Element(DataCell)
                                                .Text(
                                                    cita.FechaHora.ToString(
                                                        "dd/MM/yyyy HH:mm"));


                                            // Estado

                                            table.Cell()
                                                .Element(DataCell)
                                                .Text(
                                                    cita.Estado ?? "-");
                                        }
                                    });
                            }
                        });


                    // ---------------------------------------------
                    // PIE DE PÁGINA
                    // ---------------------------------------------

                    page.Footer()
                        .AlignCenter()
                        .Text(text =>
                        {
                            text.Span("Página ");

                            text.CurrentPageNumber();

                            text.Span(" de ");

                            text.TotalPages();
                        });
                });
            });


            return documento.GeneratePdf();
        }


        // =========================================================
        // REPORTE DE CITAS POR CLIENTE
        // =========================================================

        public byte[] GenerarReportePorCliente(
            ApplicationUser cliente,
            List<Cita> citas)
        {
            var documento = Document.Create(container =>
            {
                container.Page(page =>
                {
                    // ---------------------------------------------
                    // CONFIGURACIÓN
                    // ---------------------------------------------

                    page.Size(PageSizes.A4);

                    page.Margin(30);

                    page.DefaultTextStyle(x =>
                        x.FontSize(10));


                    // ---------------------------------------------
                    // ENCABEZADO
                    // ---------------------------------------------

                    page.Header()
                        .Column(column =>
                        {
                            column.Item()
                                .Text("CLÍNICA VETERINARIA")
                                .FontSize(20)
                                .Bold();

                            column.Item()
                                .Text(
                                    "Reporte de citas por cliente")
                                .FontSize(14);

                            column.Item()
                                .Text(
                                    $"Generado: {DateTime.Now:dd/MM/yyyy HH:mm}")
                                .FontSize(9)
                                .FontColor(
                                    Colors.Grey.Darken1);
                        });


                    // ---------------------------------------------
                    // CONTENIDO
                    // ---------------------------------------------

                    page.Content()
                        .PaddingVertical(20)
                        .Column(column =>
                        {
                            // INFORMACIÓN DEL CLIENTE

                            column.Item()
                                .Text(
                                    $"Cliente: {cliente.Nombre} {cliente.Apellido}")
                                .FontSize(12)
                                .Bold();

                            column.Item()
                                .Text(
                                    $"Correo: {cliente.Email ?? "-"}");

                            column.Item()
                                .PaddingTop(5)
                                .Text(
                                    $"Total de citas: {citas.Count}");


                            // -------------------------------------
                            // SIN CITAS
                            // -------------------------------------

                            if (citas.Count == 0)
                            {
                                column.Item()
                                    .PaddingTop(20)
                                    .Text(
                                        "El cliente no tiene citas registradas.");
                            }

                            // -------------------------------------
                            // CON CITAS
                            // -------------------------------------

                            else
                            {
                                column.Item()
                                    .PaddingTop(15)
                                    .Table(table =>
                                    {
                                        table.ColumnsDefinition(
                                            columns =>
                                            {
                                                columns.RelativeColumn(2);

                                                columns.RelativeColumn(2);

                                                columns.RelativeColumn(2);

                                                columns.RelativeColumn(1);
                                            });


                                        // ENCABEZADOS

                                        table.Header(header =>
                                        {
                                            header.Cell()
                                                .Element(HeaderCell)
                                                .Text("Mascota")
                                                .Bold();

                                            header.Cell()
                                                .Element(HeaderCell)
                                                .Text("Servicio")
                                                .Bold();

                                            header.Cell()
                                                .Element(HeaderCell)
                                                .Text("Fecha")
                                                .Bold();

                                            header.Cell()
                                                .Element(HeaderCell)
                                                .Text("Estado")
                                                .Bold();
                                        });


                                        // DATOS

                                        foreach (var cita in citas)
                                        {
                                            table.Cell()
                                                .Element(DataCell)
                                                .Text(
                                                    cita.Mascota?.Nombre
                                                    ?? "-");


                                            table.Cell()
                                                .Element(DataCell)
                                                .Text(
                                                    cita.ServicioVeterinario
                                                        ?.NombreServicio
                                                    ?? "-");


                                            table.Cell()
                                                .Element(DataCell)
                                                .Text(
                                                    cita.FechaHora.ToString(
                                                        "dd/MM/yyyy HH:mm"));


                                            table.Cell()
                                                .Element(DataCell)
                                                .Text(
                                                    cita.Estado ?? "-");
                                        }
                                    });
                            }
                        });


                    // ---------------------------------------------
                    // PIE
                    // ---------------------------------------------

                    page.Footer()
                        .AlignCenter()
                        .Text(text =>
                        {
                            text.Span("Página ");

                            text.CurrentPageNumber();

                            text.Span(" de ");

                            text.TotalPages();
                        });
                });
            });


            return documento.GeneratePdf();
        }


        // =========================================================
        // REPORTE DE SERVICIOS MÁS SOLICITADOS
        // =========================================================

        public byte[] GenerarReporteServiciosMasSolicitados(
            List<ServicioSolicitadoViewModel> servicios)
        {
            var documento = Document.Create(container =>
            {
                container.Page(page =>
                {
                    // ---------------------------------------------
                    // CONFIGURACIÓN
                    // ---------------------------------------------

                    page.Size(PageSizes.A4);

                    page.Margin(30);

                    page.DefaultTextStyle(x =>
                        x.FontSize(10));


                    // ---------------------------------------------
                    // ENCABEZADO
                    // ---------------------------------------------

                    page.Header()
                        .Column(column =>
                        {
                            column.Item()
                                .Text("CLÍNICA VETERINARIA")
                                .FontSize(20)
                                .Bold();

                            column.Item()
                                .Text(
                                    "Servicios más solicitados")
                                .FontSize(14);

                            column.Item()
                                .Text(
                                    $"Generado: {DateTime.Now:dd/MM/yyyy HH:mm}")
                                .FontSize(9)
                                .FontColor(
                                    Colors.Grey.Darken1);
                        });


                    // ---------------------------------------------
                    // CONTENIDO
                    // ---------------------------------------------

                    page.Content()
                        .PaddingVertical(20)
                        .Column(column =>
                        {
                            column.Item()
                                .Text(
                                    "Servicios ordenados de mayor a menor según la cantidad de citas.");


                            // SIN SERVICIOS

                            if (servicios.Count == 0)
                            {
                                column.Item()
                                    .PaddingTop(20)
                                    .Text(
                                        "No existen servicios con citas registradas.");
                            }

                            // CON SERVICIOS

                            else
                            {
                                column.Item()
                                    .PaddingTop(15)
                                    .Table(table =>
                                    {
                                        // COLUMNAS

                                        table.ColumnsDefinition(
                                            columns =>
                                            {
                                                columns.ConstantColumn(40);

                                                columns.RelativeColumn(3);

                                                columns.RelativeColumn(2);

                                                columns.RelativeColumn(2);
                                            });


                                        // ENCABEZADOS

                                        table.Header(header =>
                                        {
                                            header.Cell()
                                                .Element(HeaderCell)
                                                .Text("#")
                                                .Bold();

                                            header.Cell()
                                                .Element(HeaderCell)
                                                .Text("Servicio")
                                                .Bold();

                                            header.Cell()
                                                .Element(HeaderCell)
                                                .Text("Precio")
                                                .Bold();

                                            header.Cell()
                                                .Element(HeaderCell)
                                                .Text("Cantidad de citas")
                                                .Bold();
                                        });


                                        // DATOS

                                        for (int i = 0;
                                             i < servicios.Count;
                                             i++)
                                        {
                                            var servicio =
                                                servicios[i];


                                            // POSICIÓN

                                            table.Cell()
                                                .Element(DataCell)
                                                .Text(
                                                    (i + 1)
                                                        .ToString());


                                            // SERVICIO

                                            table.Cell()
                                                .Element(DataCell)
                                                .Text(
                                                    servicio
                                                        .NombreServicio);


                                            // PRECIO

                                            table.Cell()
                                                .Element(DataCell)
                                                .Text(
                                                    $"Bs {servicio.Precio:N2}");


                                            // CANTIDAD

                                            table.Cell()
                                                .Element(DataCell)
                                                .Text(
                                                    servicio
                                                        .CantidadCitas
                                                        .ToString());
                                        }
                                    });
                            }
                        });


                    // ---------------------------------------------
                    // PIE
                    // ---------------------------------------------

                    page.Footer()
                        .AlignCenter()
                        .Text(text =>
                        {
                            text.Span("Página ");

                            text.CurrentPageNumber();

                            text.Span(" de ");

                            text.TotalPages();
                        });
                });
            });


            return documento.GeneratePdf();
        }


        // =========================================================
        // ESTILO DEL ENCABEZADO DE LAS TABLAS
        // =========================================================

        private static IContainer HeaderCell(
            IContainer container)
        {
            return container
                .Background(Colors.Grey.Lighten2)
                .Border(1)
                .BorderColor(
                    Colors.Grey.Lighten1)
                .Padding(5);
        }


        // =========================================================
        // ESTILO DE LAS CELDAS
        // =========================================================

        private static IContainer DataCell(
            IContainer container)
        {
            return container
                .BorderBottom(1)
                .BorderColor(
                    Colors.Grey.Lighten2)
                .Padding(5);
        }
    }
}