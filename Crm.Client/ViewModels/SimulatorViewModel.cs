using Common.Enums;
using Common.Models;
using Common.ModelsDTO;
using Crm.Client.BL;
using Crm.Client.Helpers;
using Crm.Client.Utils;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Crm.Client.ViewModels
{
    class SimulatorViewModel : ViewModelPropertyChanged
    {
        private readonly IFrameNavigationService _navigationService;
        private readonly SimulatorManager _simulatorManager;
        private string _lastIdCardSearch;
        public ICommand SearchCommand { get; set; }
        public ICommand SimulateCommand { get; set; }
        public ICommand LineSelectedCommand { get; set; }
        public ICommand GoBackCommand { get; set; }
        public ICommand ClearCommand { get; set; }

        private ObservableCollection<Line> _lines;
        public ObservableCollection<Line> Lines
        {
            get { return _lines; }
            set
            {
                _lines = value;
                OnPropertyChanged();
            }
        }

        private ObservableCollection<string> _enumSendTo;

        public ObservableCollection<string> EnumSendTo
        {
            get { return _enumSendTo; }
            set
            {
                _enumSendTo = value;
                OnPropertyChanged();
            }
        }

        private string _searchText;
        public string SearchText
        {
            get { return _searchText; }
            set
            {
                _searchText = value;
                OnPropertyChanged();
            }
        }

        private int _selectedLine;
        public int SelectedLine
        {
            get { return _selectedLine; }
            set
            {
                _selectedLine = value;
                OnPropertyChanged();
            }
        }

        private string _minDuration;
        public string MinDuration
        {
            get { return _minDuration; }
            set
            {
                _minDuration = value;
                OnPropertyChanged();
            }
        }

        private string _maxDuration;
        public string MaxDuration
        {
            get { return _maxDuration; }
            set
            {
                _maxDuration = value;
                OnPropertyChanged();
            }
        }

        private bool _isSms;
        public bool IsSms
        {
            get { return _isSms; }
            set
            {
                _isSms = value;
                OnPropertyChanged();
            }
        }

        private string _count;
        public string Count
        {
            get { return _count; }
            set
            {
                _count = value;
                OnPropertyChanged();
            }
        }

        private string _callToCenter;

        public string CallToCenter
        {
            get { return _callToCenter; }
            set
            {
                _callToCenter = value;
                OnPropertyChanged();
            }
        }


        public string SelectedSendTo { get; set; }

        public SimulatorViewModel(IFrameNavigationService navigationService)
        {
            _navigationService = navigationService;
            _simulatorManager = new SimulatorManager();
            SearchCommand = new RelayCommand(Search);
            SimulateCommand = new RelayCommand(Simulate);
            LineSelectedCommand = new RelayCommand(ChackIfPackageHasFriends);
            ClearCommand = new RelayCommand(Clear);
            GoBackCommand = new RelayCommand(() =>
            {
                _navigationService.NavigateTo("Login");
                ViewModelLocator.UnRegisterSimulatorViewModel();
            });
        }

        private void Clear()
        {
            SelectedLine = 0;
            MinDuration = null;
            MaxDuration = null;
            IsSms = false;
            Count = null;
            CallToCenter = null;
            SelectedSendTo = null;
            EnumSendTo = null;
            Lines = null;

        }

        private void ChackIfPackageHasFriends()
        {
            Package package = _simulatorManager.GetPackage(SelectedLine);
            EnumSendTo.Remove(Enum.GetName(typeof(SimulateSendTo), SimulateSendTo.Friends));
            if (package != null && package.Friends != null)
            {
                EnumSendTo.Add(Enum.GetName(typeof(SimulateSendTo), SimulateSendTo.Friends));

            }
        }

        private bool CanSearch()
        {
            if (string.IsNullOrWhiteSpace(SearchText))
            {
                MessageBox.Show("You must write an Id card.", "", MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
            return true;
        }

        private void Search()
        {
            if (CanSearch())
            {
                Clear();
                EnumSendTo = new ObservableCollection<string>(Enum.GetNames(typeof(SimulateSendTo)).ToList());
                _lastIdCardSearch = SearchText;
                var lines = _simulatorManager.GetLines(SearchText);
                if (lines != null)
                {
                    Lines = new ObservableCollection<Line>(lines);
                    if (lines.Count == 1)
                    {
                        EnumSendTo.Remove(Enum.GetName(typeof(SimulateSendTo), SimulateSendTo.Family));
                    }
                    MessageBox.Show("Customer is found, please choose a line", "", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
        }

        private string CanSimulate()
        {
            if (SelectedLine == 0)
                return "You must choose a line";
            if (!IsSms)
            {
                if (string.IsNullOrWhiteSpace(MinDuration))
                    return "Min duration is required";
                if (string.IsNullOrWhiteSpace(MaxDuration))
                    return "Max duration is required";

                if (!int.TryParse(MinDuration, out int min))
                    return "Min duration must be a number";
                if (!int.TryParse(MaxDuration, out int max))
                    return "Max duration must be a number";
            }
            if (string.IsNullOrWhiteSpace(Count))
                return "Number of Calls/SMS is required";
            if (SelectedSendTo == null)
                return "You must select a destination number";
            if (!int.TryParse(Count, out int count))
                return "Number of Calls/SMS must be a number";
            if (!string.IsNullOrWhiteSpace(CallToCenter) && !int.TryParse(CallToCenter, out int callTocenter))
                return "Call to center must be a number";
            return null;
        }

        private void Simulate()
        {
            string error = CanSimulate();
            if (error != null)
            {
                MessageBox.Show(error, "", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            int.TryParse(Count, out int count);
            int.TryParse(CallToCenter, out int callToCenter);
            Enum.TryParse(SelectedSendTo, out SimulateSendTo selectedEnumValue);
            SimulateDTO simulateDTO = new SimulateDTO()
            {
                IdentityCard = _lastIdCardSearch,
                LineId = SelectedLine,
                IsSms = IsSms,
                NumberOfCallsOrSms = count,
                SendTo = selectedEnumValue,
                CallToCenter = callToCenter

            };
            if (!IsSms)
            {
                int.TryParse(MinDuration, out int minDuration);
                int.TryParse(MaxDuration, out int maxDuration);
                simulateDTO.MinDuration = minDuration;
                simulateDTO.MaxDuration = maxDuration;
            }
            _simulatorManager.Simulate(simulateDTO);

        }
    }
}
