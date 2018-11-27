using Common.ModelsDTO;
using Crm.Client.BL;
using Crm.Client.Helpers;
using Crm.Client.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Crm.Client.ViewModels
{
    public class ChooseReceiptsViewModel : ViewModelPropertyChanged
    {
        public ICommand ReceiptsCommand { get; set; }
        public ICommand GoBackCommand { get; set; }

        private readonly ReceiptsManager _receiptsManager;
        private readonly IFrameNavigationService _navigationService;

        public List<int> Year { get; set; }
        public List<int> Month { get; set; }
        public int SelectedYear { get; set; }
        public int SelectedMonth { get; set; }
        public string CustomerId { get; set; }

        public ChooseReceiptsViewModel(IFrameNavigationService navigationService)
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
        }

        private void ShowReceipt()
        {
            string error = ValidateFields();
            if (error != null)
            {
                MessageBox.Show(error);
                return;
            }
            List<LineReceiptDTO> lineInvoices = _receiptsManager.GetReceipt();
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
