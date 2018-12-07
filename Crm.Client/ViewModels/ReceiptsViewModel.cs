using Common.ModelsDTO;
using Crm.Client.BL;
using Crm.Client.Helpers;
using Crm.Client.Utils;
using Crm.Client.Views;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.Win32;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using TheArtOfDev.HtmlRenderer.PdfSharp;
using WPFCustomMessageBox;

namespace Crm.Client.ViewModels
{
    public class ReceiptsViewModel : ViewModelPropertyChanged
    {
        public ICommand ReceiptsCommand { get; set; }
        public ICommand GoBackCommand { get; set; }
        public ICommand ExportCommand { get; set; }

        private readonly ReceiptsManager _receiptsManager;
        private readonly IFrameNavigationService _navigationService;

        private ObservableCollection<LineReceiptDTO> _receipts { get; set; }
        public ObservableCollection<LineReceiptDTO> Receipts
        {
            get { return _receipts; }
            set
            {
                _receipts = value;
                OnPropertyChanged();
            }
        }
        public List<int> Year { get; set; }
        public List<int> Month { get; set; }
        public int SelectedYear { get; set; }
        public int SelectedMonth { get; set; }
        public string CustomerId { get; set; }


        public string CustomerName { get; set; }
        public double TotalPayment { get; set; }

        // ctor
        public ReceiptsViewModel(IFrameNavigationService navigationService)
        {
            _navigationService = navigationService;
            _receiptsManager = new ReceiptsManager();
            Year = Enumerable.Range(1990, DateTime.Now.Year - 1990 + 1).ToList();
            Month = Enumerable.Range(1, 12).ToList();
            ReceiptsCommand = new RelayCommand(ShowReceipt);
            GoBackCommand = new RelayCommand(() =>
            {
                _navigationService.NavigateTo("Login");
            });
            ExportCommand = new RelayCommand(ExportReceipt);
            Receipts = new ObservableCollection<LineReceiptDTO>() { new LineReceiptDTO() { LeftSms = 1, LeftMinutes = TimeSpan.FromSeconds(2000) } };
        }

        /// <summary>
        /// Export receipt to pdf or excel
        /// </summary>
        private void ExportReceipt()
        {
            var result = CustomMessageBox.ShowOKCancel(
                     "Which way do you want to export your receipt?",
                     "Export receipt",
                     "PDF",
                     "Excel");
            if (result == MessageBoxResult.OK)
            {
                WriteToPdf();
            }
            if (result == MessageBoxResult.Cancel)
            {
                WriteToExcel();
            }

        }

        /// <summary>
        /// Write receipt to excel file.
        /// </summary>
        private void WriteToExcel()
        {
            using (ExcelPackage excelPackage = new ExcelPackage())
            {

                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("Receipt");
                worksheet.Column(1).Width = 40;
                worksheet.Column(3).Width = 15;
                worksheet.Column(3).Style.HorizontalAlignment = ExcelHorizontalAlignment.Center;
                worksheet.Row(1).Style.Font.Bold = true;
                worksheet.Row(1).Style.Font.UnderLine = true;
                worksheet.Row(2).Style.Font.Bold = true;
                worksheet.Row(3).Style.Font.Bold = true;
                worksheet.Cells["A1"].Value = "Customer name";
                worksheet.Cells["C1"].Value = CustomerName;
                worksheet.Cells["A2"].Value = "Date";
                worksheet.Cells["C2"].Value = SelectedMonth + "/" + SelectedYear;
                worksheet.Cells["A3"].Value = "Total price";
                worksheet.Cells["C3"].Value = TotalPayment;
                for (int i = 0, j = 5; i < Receipts.Count; i++, j += 18)
                {
                    worksheet.Cells["A" + (1 + j)].Value = "Line info";
                    worksheet.Cells["A" + (1 + j)].Style.Font.Bold = true;
                    worksheet.Cells["A" + (2 + j)].Value = "Line Number";
                    worksheet.Cells["C" + (2 + j)].Value = Receipts[i].LineNumber;
                    worksheet.Cells["A" + (3 + j)].Value = "Amout of minute you used";
                    worksheet.Cells["C" + (3 + j)].Value = Receipts[i].UsageCall;
                    worksheet.Cells["A" + (4 + j)].Value = "Amout of sms you used";
                    worksheet.Cells["C" + (4 + j)].Value = Receipts[i].UsageSms;
                    worksheet.Cells["A" + (5 + j)].Value = "Total line price";
                    worksheet.Cells["C" + (5 + j)].Value = Receipts[i].LineTotalPrice;

                    worksheet.Cells["A" + (7 + j)].Value = "Package info";
                    worksheet.Cells["A" + (7 + j)].Style.Font.Bold = true;
                    worksheet.Cells["A" + (8 + j)].Value = "Minute";
                    worksheet.Cells["C" + (8 + j)].Value = Receipts[i].PackageMinute;
                    worksheet.Cells["A" + (9 + j)].Value = "Sms";
                    worksheet.Cells["C" + (9 + j)].Value = Receipts[i].PackageSms;
                    worksheet.Cells["A" + (10 + j)].Value = "Package price";
                    worksheet.Cells["C" + (10 + j)].Value = Receipts[i].PackagePrice;

                    worksheet.Cells["A" + (12 + j)].Value = "Out of package ";
                    worksheet.Cells["A" + (12 + j)].Style.Font.Bold = true;
                    worksheet.Cells["A" + (13 + j)].Value = "Minute beyond package limit";
                    worksheet.Cells["C" + (13 + j)].Value = Receipts[i].MinutesBeyondPackageLimit.TotalMinutes;
                    worksheet.Cells["A" + (14 + j)].Value = "Sms beyond package limit";
                    worksheet.Cells["C" + (14 + j)].Value = Receipts[i].SmsBeyondPackageLimit;
                }
                SaveFileDialog sfd = new SaveFileDialog() { Filter = "Excel file|*.xlsx", ValidateNames = true };
                if (sfd.ShowDialog() == true)
                {
                    FileInfo fi = new FileInfo(sfd.FileName);
                    excelPackage.SaveAs(fi);
                }
            }
        }

        /// <summary>
        /// Write receipt to excel file.
        /// </summary>
        private void WriteToPdf()
        {
            SaveFileDialog sfd = new SaveFileDialog() { Filter = "PDF file|*.pdf", ValidateNames = true };
            if (sfd.ShowDialog() == true)
            {
                Document doc = new Document(PageSize.A4);
                var output = new FileStream(sfd.FileName, FileMode.Append);
                var writer = PdfWriter.GetInstance(doc, output);
                doc.Open();
                PdfPTable table1 = CreateBillConstTable();
                doc.Add(table1);
                for (int i = 0; i < Receipts.Count; i++)
                {
                    PdfPTable table2 = CreateLineTable(Receipts[i]);
                    doc.Add(table2);
                }
                doc.Close();
            }
        }

        private PdfPTable CreateBillConstTable()
        {
            PdfPTable table = new PdfPTable(2)
            {
                WidthPercentage = 100
            };

            PdfPCell cell1 = new PdfPCell
            {
                Colspan = 1
            };
            cell1.AddElement(new Paragraph("Client Name: " + CustomerName));
            cell1.AddElement(new Paragraph("Date: " + SelectedMonth + "/" + SelectedYear, new Font()));
            cell1.HorizontalAlignment = Element.ALIGN_CENTER;

            PdfPCell cell2 = new PdfPCell();
            cell2.AddElement(new Paragraph("Total Price: " + TotalPayment));
            cell2.HorizontalAlignment = Element.ALIGN_CENTER;

            table.AddCell(cell1);
            table.AddCell(cell2);
            return table;
        }

        private PdfPTable CreateLineTable(LineReceiptDTO receipt)
        {
            PdfPTable table = new PdfPTable(3)
            {
                WidthPercentage = 100
            };

            PdfPCell cell = new PdfPCell(new Phrase("Line Number:" + receipt.LineNumber));
            cell.Colspan = 3;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            table.AddCell(cell);

            table.AddCell("Total line price: " + receipt.LineTotalPrice + "₪");
            cell = new PdfPCell(new Phrase("Package info:"));
            cell.Colspan = 2;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("Package"));
            cell.Colspan = 3;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            table.AddCell("Minute: " + receipt.UsageCall);  // "Row 4, Col 1"
            table.AddCell("Minute left: " + receipt.LeftMinutes);  // "Row 4, Col 2"
            table.AddCell("Package used: " + receipt.MinutesUsagePrecent + "%");  // "Row 4, Col 3"

            table.AddCell("SMS: " + receipt.UsageSms);  // "Row 5, Col 1"
            table.AddCell("SMS left: " + receipt.LeftSms);  // "Row 5, Col 2"
            table.AddCell("Package used: " + receipt.SmsUsagePrecent + "%");  // "Row 5, Col 3"

            cell = new PdfPCell(new Phrase("Package price: " + receipt.PackagePrice + "₪"));
            cell.Colspan = 3;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("Out of Package"));
            cell.Colspan = 3;
            cell.HorizontalAlignment = Element.ALIGN_LEFT;
            table.AddCell(cell);

            table.AddCell("Minute beyond package: " + receipt.MinutesBeyondPackageLimit);  // "Row 8, Col 1"
            table.AddCell("price per minunte: " + receipt.PricePerMinute + "₪");  // "Row 8, Col 2"
            table.AddCell("Total: " + receipt.ExceptionalMinutesPrice);  // "Row 8, Col 3"

            table.AddCell("SMS beyond package: " + receipt.SmsBeyondPackageLimit);  // "Row 9, Col 1"
            table.AddCell("price per SMS: " + receipt.PricePerSms + "₪");  // "Row 9, Col 2"
            table.AddCell("Total: " + receipt.ExceptionalSmsPrice);  // "Row 9, Col 3"

            //cell = new PdfPCell(new Phrase("Total:" + receipt.));
            cell.Colspan = 3;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            table.AddCell(cell);

            cell = new PdfPCell(new Phrase("  "));
            cell.Colspan = 3;
            cell.HorizontalAlignment = Element.ALIGN_CENTER;
            table.AddCell(cell);

            return table;
        }

        /// <summary>
        /// Get and show receipt
        /// </summary>
        private void ShowReceipt()
        {
            string error = ValidateFields();
            if (error != null)
            {
                MessageBox.Show(error);
                return;
            }
            if (_receiptsManager.GetReceipt(CustomerId, SelectedYear, SelectedMonth))
            {
                Receipts = new ObservableCollection<LineReceiptDTO>(_receiptsManager._receipts);
                TotalPayment = Receipts.Sum(x => x.LineTotalPrice);
                CustomerName = Receipts.Last().CustomerName;
                _navigationService.NavigateTo("Receipts");
            }
           
        }

        /// <summary>
        /// Validate fields to get receipt
        /// </summary>
        /// <returns>Error if invalid. otherwise null</returns>
        private string ValidateFields()
        {
            if (string.IsNullOrWhiteSpace(CustomerId))
                return "Customer id is required";
            if (SelectedYear == 0)
                return "Please select year";
            if (SelectedMonth == 0)
                return "Please select month";
            return null;
        }
    }
}