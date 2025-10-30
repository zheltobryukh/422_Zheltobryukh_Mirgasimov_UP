using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms.DataVisualization.Charting;
using Word = Microsoft.Office.Interop.Word;
using Excel = Microsoft.Office.Interop.Excel;
using System.Windows.Navigation;

namespace _422_Zheltobryukh.Pages
{
    public partial class DiagrammPage : Page
    {
        private Zheltobryukh_DB_PaymentsEntities1 _context = new Zheltobryukh_DB_PaymentsEntities1();

        public DiagrammPage()
        {
            InitializeComponent();

            ChartPayments.ChartAreas.Add(new ChartArea("Main"));

            var currentSeries = new Series("Платежи")
            {
                IsValueShownAsLabel = true
            };
            ChartPayments.Series.Add(currentSeries);

            CmbUser.ItemsSource = _context.Users.ToList();
            CmbDiagram.ItemsSource = Enum.GetValues(typeof(SeriesChartType));
        }

        private void UpdateChart(object sender, SelectionChangedEventArgs e)
        {
            if (CmbUser.SelectedItem is User currentUser &&
                CmbDiagram.SelectedItem is SeriesChartType currentType)
            {
                Series currentSeries = ChartPayments.Series.FirstOrDefault();
                currentSeries.ChartType = currentType;
                currentSeries.Points.Clear();

                var categoriesList = _context.Categories.ToList();
                foreach (var category in categoriesList)
                {
                    currentSeries.Points.AddXY(category.Name,
                        _context.Payments.ToList().Where(u =>
                            u.UserID == currentUser.ID && u.CategoryID == category.ID).Sum(u => u.Price * u.Num));
                }
            }
        }

        private void ButtonWord_Click(object sender, RoutedEventArgs e)
        {
            var allUsers = _context.Users.ToList();
            var allCategories = _context.Categories.ToList();

            var application = new Word.Application();
            Word.Document document = application.Documents.Add();

            try
            {
                foreach (var user in allUsers)
                {
                    Word.Paragraph userParagraph = document.Paragraphs.Add();
                    Word.Range userRange = userParagraph.Range;
                    userRange.Text = user.FIO;
                    userParagraph.set_Style("Заголовок 1");
                    userRange.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                    userRange.InsertParagraphAfter();

                    document.Paragraphs.Add();

                    Word.Paragraph tableParagraph = document.Paragraphs.Add();
                    Word.Range tableRange = tableParagraph.Range;
                    Word.Table paymentsTable = document.Tables.Add(tableRange, allCategories.Count() + 1, 2);
                    paymentsTable.Borders.InsideLineStyle = paymentsTable.Borders.OutsideLineStyle = Word.WdLineStyle.wdLineStyleSingle;
                    paymentsTable.Range.Cells.VerticalAlignment = Word.WdCellVerticalAlignment.wdCellAlignVerticalCenter;

                    Word.Range cellRange;
                    cellRange = paymentsTable.Cell(1, 1).Range;
                    cellRange.Text = "Категория";
                    cellRange = paymentsTable.Cell(1, 2).Range;
                    cellRange.Text = "Сумма расходов";
                    paymentsTable.Rows[1].Range.Font.Name = "Times New Roman";
                    paymentsTable.Rows[1].Range.Font.Size = 14;
                    paymentsTable.Rows[1].Range.Bold = 1;
                    paymentsTable.Rows[1].Range.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;

                    for (int i = 0; i < allCategories.Count(); i++)
                    {
                        var currentCategory = allCategories[i];
                        cellRange = paymentsTable.Cell(i + 2, 1).Range;
                        cellRange.Text = currentCategory.Name;
                        cellRange.Font.Name = "Times New Roman";
                        cellRange.Font.Size = 12;

                        cellRange = paymentsTable.Cell(i + 2, 2).Range;
                        decimal sum = _context.Payments.ToList()
                            .Where(p => p.UserID == user.ID && p.CategoryID == currentCategory.ID)
                            .Sum(p => p.Price * p.Num);
                        cellRange.Text = sum.ToString("N2") + " руб.";

                        cellRange.Font.Name = "Times New Roman";
                        cellRange.Font.Size = 12;
                    }

                    document.Paragraphs.Add();

                    Payment maxPayment = user.Payments.OrderByDescending(u => u.Price * u.Num).FirstOrDefault();
                    if (maxPayment != null)
                    {
                        Word.Paragraph maxPaymentParagraph = document.Paragraphs.Add();
                        Word.Range maxPaymentRange = maxPaymentParagraph.Range;
                        maxPaymentRange.Text = $"Самый дорогостоящий платеж {maxPayment.Name} за {(maxPayment.Price * maxPayment.Num).ToString("N2")} руб. от {maxPayment.Date.ToString("dd.MM.yyyy")}";
                        maxPaymentParagraph.set_Style("Подзаголовок");
                        maxPaymentRange.Font.Color = Word.WdColor.wdColorDarkRed;
                        maxPaymentRange.InsertParagraphAfter();
                    }

                    document.Paragraphs.Add();

                    Payment minPayment = user.Payments.OrderBy(u => u.Price * u.Num).FirstOrDefault();
                    if (minPayment != null)
                    {
                        Word.Paragraph minPaymentParagraph = document.Paragraphs.Add();
                        Word.Range minPaymentRange = minPaymentParagraph.Range;
                        minPaymentRange.Text = $"Самый дешевый платеж {minPayment.Name} за {(minPayment.Price * minPayment.Num).ToString("N2")} руб. от {minPayment.Date.ToString("dd.MM.yyyy")}";
                        minPaymentParagraph.set_Style("Подзаголовок");
                        minPaymentRange.Font.Color = Word.WdColor.wdColorDarkGreen;
                        minPaymentRange.InsertParagraphAfter();
                    }

                    if (user != allUsers.LastOrDefault())
                        document.Words.Last.InsertBreak(Word.WdBreakType.wdPageBreak);
                }

                foreach (Word.Section section in document.Sections)
                {
                    Word.HeaderFooter footer = section.Footers[Word.WdHeaderFooterIndex.wdHeaderFooterPrimary];
                    footer.PageNumbers.Add(Word.WdPageNumberAlignment.wdAlignPageNumberCenter);
                }

                foreach (Word.Section section in document.Sections)
                {
                    Word.Range headerRange = section.Headers[Word.WdHeaderFooterIndex.wdHeaderFooterPrimary].Range;
                    headerRange.Text = DateTime.Now.ToString("dd/MM/yyyy");
                    headerRange.ParagraphFormat.Alignment = Word.WdParagraphAlignment.wdAlignParagraphCenter;
                    headerRange.Font.ColorIndex = Word.WdColorIndex.wdBlack;
                    headerRange.Font.Size = 10;
                }

                application.Visible = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при экспорте в Word: " + ex.Message);
                if (document != null)
                {
                    (document as Word._Document).Close(Word.WdSaveOptions.wdDoNotSaveChanges);
                }
                if (application != null)
                {
                    (application as Word._Application).Quit();
                }
            }
        }

        private void ButtonExcel_Click(object sender, RoutedEventArgs e)
        {
            var allUsers = _context.Users.ToList().OrderBy(u => u.FIO).ToList();
            decimal grandTotal = 0;

            var application = new Excel.Application();
            application.SheetsInNewWorkbook = allUsers.Count();
            Excel.Workbook workbook = application.Workbooks.Add(Type.Missing);

            try
            {
                for (int i = 0; i < allUsers.Count(); i++)
                {
                    int startRowIndex = 1;
                    Excel.Worksheet worksheet = application.Worksheets.Item[i + 1];
                    worksheet.Name = allUsers[i].FIO;

                    worksheet.Cells[1][startRowIndex] = "Дата платежа";
                    worksheet.Cells[2][startRowIndex] = "Название";
                    worksheet.Cells[3][startRowIndex] = "Стоимость";
                    worksheet.Cells[4][startRowIndex] = "Количество";
                    worksheet.Cells[5][startRowIndex] = "Сумма";

                    Excel.Range columnHeaderRange = worksheet.Range[worksheet.Cells[1][1], worksheet.Cells[5][1]];
                    columnHeaderRange.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                    columnHeaderRange.Font.Bold = true;
                    startRowIndex++;

                    var userCategories = allUsers[i].Payments.OrderBy(u => u.Date)
                        .GroupBy(u => u.Category).OrderBy(u => u.Key.Name);

                    foreach (var groupCategory in userCategories)
                    {
                        Excel.Range headerRange = worksheet.Range[worksheet.Cells[1][startRowIndex], worksheet.Cells[5][startRowIndex]];
                        headerRange.Merge();
                        headerRange.Value = groupCategory.Key.Name;
                        headerRange.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
                        headerRange.Font.Italic = true;
                        startRowIndex++;

                        foreach (var payment in groupCategory)
                        {
                            worksheet.Cells[1][startRowIndex] = payment.Date.ToString("dd.MM.yyyy");
                            worksheet.Cells[2][startRowIndex] = payment.Name;
                            worksheet.Cells[3][startRowIndex] = payment.Price;
                            (worksheet.Cells[3][startRowIndex] as Excel.Range).NumberFormat = "0.00";
                            worksheet.Cells[4][startRowIndex] = payment.Num;
                            worksheet.Cells[5][startRowIndex].Formula = $"=C{startRowIndex}*D{startRowIndex}";
                            (worksheet.Cells[5][startRowIndex] as Excel.Range).NumberFormat = "0.00";

                            grandTotal += (payment.Price * payment.Num);
                            startRowIndex++;
                        }

                        Excel.Range sumRange = worksheet.Range[worksheet.Cells[1][startRowIndex], worksheet.Cells[4][startRowIndex]];
                        sumRange.Merge();
                        sumRange.Value = "ИТОГО:";
                        sumRange.HorizontalAlignment = Excel.XlHAlign.xlHAlignRight;

                        worksheet.Cells[5][startRowIndex].Formula = $"=SUM(E{startRowIndex - groupCategory.Count()}:E{startRowIndex - 1})";
                        sumRange.Font.Bold = worksheet.Cells[5][startRowIndex].Font.Bold = true;
                        startRowIndex++;
                    }

                    Excel.Range rangeBorders = worksheet.Range[worksheet.Cells[1][1], worksheet.Cells[5][startRowIndex - 1]];
                    rangeBorders.Borders[Excel.XlBordersIndex.xlEdgeBottom].LineStyle = Excel.XlLineStyle.xlContinuous;
                    rangeBorders.Borders[Excel.XlBordersIndex.xlEdgeLeft].LineStyle = Excel.XlLineStyle.xlContinuous;
                    rangeBorders.Borders[Excel.XlBordersIndex.xlEdgeRight].LineStyle = Excel.XlLineStyle.xlContinuous;
                    rangeBorders.Borders[Excel.XlBordersIndex.xlEdgeTop].LineStyle = Excel.XlLineStyle.xlContinuous;
                    rangeBorders.Borders[Excel.XlBordersIndex.xlInsideHorizontal].LineStyle = Excel.XlLineStyle.xlContinuous;
                    rangeBorders.Borders[Excel.XlBordersIndex.xlInsideVertical].LineStyle = Excel.XlLineStyle.xlContinuous;

                    worksheet.Columns.AutoFit();
                }

                Excel.Worksheet summarySheet = workbook.Worksheets.Add(After: workbook.Worksheets[workbook.Worksheets.Count]);
                summarySheet.Name = "Общий итог";
                summarySheet.Cells[1, 1] = "Общий итог:";
                summarySheet.Cells[1, 2] = grandTotal;
                (summarySheet.Cells[1, 2] as Excel.Range).NumberFormat = "0.00";

                Excel.Range summaryRange = summarySheet.Range[summarySheet.Cells[1, 1], summarySheet.Cells[1, 2]];
                summaryRange.Font.Color = Excel.XlRgbColor.rgbRed;
                summaryRange.Font.Bold = true;
                summarySheet.Columns.AutoFit();

                application.Visible = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Ошибка при экспорте в Excel: " + ex.Message);
                if (workbook != null)
                {
                    (workbook as Excel._Workbook).Close(false);
                }
                if (application != null)
                {
                    (application as Excel._Application).Quit();
                }
            }
        }
    }
}