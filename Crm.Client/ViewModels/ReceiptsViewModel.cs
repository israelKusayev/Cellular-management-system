using Common.ModelsDTO;
using Crm.Client.BL;
using Crm.Client.Helpers;
using Crm.Client.Utils;
using Crm.Client.Views;
using Microsoft.Win32;
using OfficeOpenXml;
using OfficeOpenXml.Style;
using PdfSharp;
using PdfSharp.Pdf;
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
                    string html = "<!DOCTYPE html><html lang='en'> <head> <meta charset='UTF-8' /> <meta name='viewport' content='width=device-width, initial-scale=1.0' /> <meta http-equiv='X-UA-Compatible' content='ie=edge' /> <title>Document</title> </head> <body> <header> <h1 style='text-align: center'>Customer Name: 0</h1> <h2 style='text-align: center'>Year: 2001 ,month: 12</h2> <h3 style='text-align: center'>Total price: 100</h3> </header> <table style='width:100%'>";
                    for (int i = 0; i < Receipts.Count; i++)
                    {
                        html += $@"<tr> <th colspan='3'><h2>Line Number: 10000</h2></th> </tr> <tr> <td><h2>line info</h2></td> </tr> <tr> <td> <div><b>Amount of minute you used: </b>0 |</div> </td> <td> <div><b>Amount of sms you used:</b>0 |</div> </td> <td> <div><b>Total line price:</b>0</div> </td> </tr> <tr> <td><h2>Package info</h2></td> </tr> <tr> <td> <div><b>Minute: </b>0</div> </td> <td> <div><b>Minute left in package:</b>0</div> </td> <td> <div><b>Package % usage:</b>0</div> </td> </tr> <tr> <td> <div><b>Sms: </b>0</div> </td> <td> <div><b>Sms left in package:</b>0</div> </td> <td> <div><b>Package % usage:</b>0</div> </td> </tr> <tr> <td> <div><b>Package price:</b>0</div> </td> </tr> <hr /> <tr> <td><h2>Out of package</h2></td> </tr> <tr> <td> <div><b>Minute beyond package limit: </b>0</div> </td> <td> <div><b>Price per minute:</b>0</div> </td> <td> <div><b>Total:</b>0</div> </td> </tr> <tr> <td> <div><b>Sms beyond package limit: </b>0</div> </td> <td> <div><b>Price per sms:</b>0</div> </td> <td> <div><b>Total:</b>0</div> </td> </tr> </table> <hr /> ";
                    }
                    html += "</body></html>";

                    PdfDocument pdf = PdfGenerator.GeneratePdf(html, PageSize.A4);
                    pdf.Save(sfd.FileName);
            }
        }

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