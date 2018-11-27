using Common.ModelsDTO;
using Crm.Client.BL;
using Crm.Client.Helpers;
using Crm.Client.Utils;
using Crm.Client.Views;
using iTextSharp.text;
using iTextSharp.text.pdf;
using Microsoft.Win32;
using OfficeOpenXml;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
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
            Receipts = new ObservableCollection<LineReceiptDTO>() { new LineReceiptDTO() { LeftSms = 1 ,LeftMinutes = TimeSpan.FromSeconds(2000)} };
            //Receipts[0].LeftMinutes.TotalMinutes.ToString().Substring(0,2);
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

        private  void WriteToExcel()
        {
            using (ExcelPackage excelPackage = new ExcelPackage())
            {
                //create a new Worksheet
                ExcelWorksheet worksheet = excelPackage.Workbook.Worksheets.Add("Sheet 1");

                //add some text to cell A1
                worksheet.Cells["A1"].Value = "My second EPPlus spreadsheet!";
                //the path of the file
                string filePath = @"C:\\Cellular\ExcelDemo.xlsx";

                //or if you use asp.net, get the relative path
                //filePath = Server.MapPath("ExcelDemo.xlsx");

                //Write the file to the disk
                FileInfo fi = new FileInfo(filePath);
                excelPackage.SaveAs(fi);
            }
        }

        private static void WriteToPdf()
        {
            SaveFileDialog sfd = new SaveFileDialog() { Filter = "PDF file|*.pdf", ValidateNames = true };
            if (sfd.ShowDialog() == true)
            {
                iTextSharp.text.Document document = new iTextSharp.text.Document(PageSize.A4.Rotate());
                try
                {
                    PdfWriter.GetInstance(document, new FileStream(sfd.FileName, FileMode.Create));
                    document.Open();
                    document.Add(new iTextSharp.text.Paragraph("lalalalalalal"));
                }
                finally
                {
                    document.Close();
                }

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
            if (_receiptsManager.GetReceipt(CustomerId,SelectedYear,SelectedMonth))
            {
                Receipts = new ObservableCollection<LineReceiptDTO>(_receiptsManager._receipts);
                TotalPayment = Receipts.Sum(x => x.PackagePrice);
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