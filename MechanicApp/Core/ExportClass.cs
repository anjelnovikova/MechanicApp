using iText.IO.Font;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using System.Dynamic;
using System.IO;
using System.Text;

namespace MechanicApp.Core
{
    internal class ExportClass
    {
        public static void ExportDataGridToCsv(System.Windows.Controls.DataGrid grid, string fileName, string[] Headers, string[] Bindings)
        {
            StringBuilder csv = new StringBuilder();

            // Заголовки столбцов
            for (int i = 0; i < Headers.Length; i++)
            {
                csv.Append(Headers[i]);
                if (i < Headers.Length - 1)
                    csv.Append(",");
            }
            csv.AppendLine();

            // Данные
            foreach (ExpandoObject row in grid.ItemsSource)
            {
                var ExpandoDict = row as IDictionary<string, object>;
                for (int i = 0; i < Headers.Length; i++)
                {
                    csv.Append(ExpandoDict[Bindings[i]]);
                    if (i < Headers.Length - 1)
                        csv.Append(",");
                }
                csv.AppendLine();
            }

            File.WriteAllText(fileName, csv.ToString(), Encoding.UTF8);
        }

        public static void ExportDataGridToPdf(System.Windows.Controls.DataGrid grid, string fileName, string[] Headers, string[] Bindings, string DocumentHeader)
        {
            PdfFont font = PdfFontFactory.CreateFont(@"C:\Windows\Fonts\arial.ttf", PdfEncodings.IDENTITY_H, PdfFontFactory.EmbeddingStrategy.FORCE_EMBEDDED);

            using (PdfWriter writer = new PdfWriter(fileName))
            {
                using (PdfDocument pdf = new PdfDocument(writer))
                {
                    Document document = new Document(pdf);
                    document.SetFont(font);
                    Table table = new Table(Headers.Count());

                    var headerStyle = new Style()
                       .SetBackgroundColor(iText.Kernel.Colors.ColorConstants.LIGHT_GRAY)
                       .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER)
                       .SetFontSize(12)
                       .SetBold();
                    // Заголовки столбцов
                    foreach (string header in Headers)
                    {
                        Cell headerCell = new Cell().Add(new Paragraph(header));
                        headerCell.AddStyle(headerStyle);
                        table.AddHeaderCell(headerCell);
                    }

                    // Данные
                    foreach (ExpandoObject row in grid.ItemsSource)
                    {
                        var ExpandoDict = row as IDictionary<string, object>;
                        foreach (var binding in Bindings)
                        {
                            var obj = ExpandoDict[binding];
                            table.AddCell(new Cell().Add(new Paragraph(obj is bool ? BoolToString(obj.ToString()) : obj.ToString())));
                        }
                    }

                    var docHeaderStyle = new iText.Layout.Style()
                   .SetTextAlignment(iText.Layout.Properties.TextAlignment.CENTER)
                   .SetFontSize(18)
                   .SetBold();

                    document.Add(new Paragraph(DocumentHeader).AddStyle(docHeaderStyle));
                    document.Add(table);
                }
            }
        }

        static string BoolToString(string boolText)
        {
            switch (boolText)
            {
                case "True": return "Утверждено";
                case "False": return "Не утверждено";
            }
            return "";
        }

    }
}
