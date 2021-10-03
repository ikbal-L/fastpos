using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing.Printing;
using System.Globalization;
using System.Linq;
using System.Printing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Markup;
using Caliburn.Micro;
using FastPosFrontend.Helpers;
using FastPosFrontend.Navigation;
using FastPosFrontend.ViewModels.Settings;
using ServiceInterface.Model;
using ServiceLib.Service;
using ServiceLib.Service.StateManager;

namespace FastPosFrontend.ViewModels
{
    [NavigationItem(title: "Daily Expense Reports", target: typeof(DailyExpenseReportsViewModel),"",isQuickNavigationEnabled:true)]
    public class DailyExpenseReportsViewModel : LazyScreen
    {
        
        private bool _isReportGenerated;
        private DailyExpenseReport _report;

        public DailyExpenseReportsViewModel() : base()
        {
            SetupEmbeddedRightCommandBar();

        }

        protected override void Setup()
        {
            var reports = StateManager.GetAsync<DailyExpenseReport>();
            _data = new NotifyAllTasksCompletion(reports);
            
        }


        public override void Initialize()
        {
            var data = StateManager.Get<DailyExpenseReport>();
            _reports = new ObservableCollection<DailyExpenseReport>(data);
            
            Reports = (CollectionView)CollectionViewSource.GetDefaultView(_reports);
            Reports.Filter += FilterReportsByIssuedDate;

            

            //Reports.GroupDescriptions.Add(new PropertyGroupDescription(nameof(DailyExpenseReport.IssuedDateYear)));
            //Reports.GroupDescriptions.Add(new PropertyGroupDescription(nameof(DailyExpenseReport.IssuedDateMonth)));
            //Reports.GroupDescriptions.Add(new PropertyGroupDescription(nameof(DailyExpenseReport.CashPaymentsTotal)));
            //Reports.GroupDescriptions.Add(new PropertyGroupDescription(nameof(DailyExpenseReport.DeliveryPaymentsTotal)));
            Reports.SortDescriptions.Add(new System.ComponentModel.SortDescription(nameof(DailyExpenseReport.IssuedDate),System.ComponentModel.ListSortDirection.Descending));
            Report =  _reports?.FirstOrDefault(r => r.IssuedDate.ToString("d") == DateTime.Today.Date.ToString("d"));
            if (Report!= null)
            {
                IsReportGenerated = true;
            }
           
        }

        private bool FilterReportsByIssuedDate(object o)
        {
            if (o is DailyExpenseReport r)
            {
                if (string.IsNullOrEmpty(UserSearchQuery)) return true;

                var date = r.IssuedDate.ToString("d", new CultureInfo("fr-DZ"));
                return date.Contains(UserSearchQuery);
            }
            return false;
        }

        private void SetupEmbeddedRightCommandBar()
        {
            EmbeddedRightCommandBar = new EmbeddedCommandBarViewModel(this,"DailyExpenseReportRightCommandBar");
        }

        public bool IsReportGenerated
        {
            get => _isReportGenerated;
            set => Set(ref _isReportGenerated, value);
        }

        private bool _isOpennedReportTabOpen;

        public bool IsOpennedReportTabOpen
        {
            get { return _isOpennedReportTabOpen; }
            set { Set(ref _isOpennedReportTabOpen , value); }
        }


        public DailyExpenseReport Report
        {
            get => _report;
            set => Set(ref _report, value);
        }
        private DailyExpenseReport _opennedReport;

        public DailyExpenseReport OpennedReport
        {
            get { return _opennedReport; }
            set 
            { 
                Set(ref _opennedReport, value);
                NotifyOfPropertyChange(nameof(CanAddOrRefreshReportProperty));
            }
        }

        public bool CanAddOrRefreshReportProperty => CanAddOrRefreshReport();

        public bool CanAddOrRefreshReport()
        {
            if (IsReportGenerated)
            {
                return OpennedReport?.IssuedDate.Date == DateTime.Today.Date;
            }
            return true;
        }

        private ObservableCollection<DailyExpenseReport> _reports;


        public CollectionView Reports { get; private set; }

        private string _userSearchQuery;

        public string UserSearchQuery
        {
            get { return _userSearchQuery; }
            set { 
                Set(ref _userSearchQuery, value);
                Reports?.Refresh();
            }
        }

        public void Generate()
        {
            var parent = Parent as MainViewModel;

            var vm = new DailyExpenseReportInputDataViewModel(this,Report);
            vm.OnReportGenerated(report =>
            {
                Report = report;
                var oldReport = _reports.FirstOrDefault(r => r.IssuedDate.Date == DateTime.Today.Date);
                if (oldReport!= null)
                {
                    _reports.Remove(oldReport);
                }
                IsReportGenerated = true;
                _reports.Add(report);
                Reports.Refresh();

            });

            parent?.OpenDialog(vm).OnClose(() =>
            {
                vm.Dispose();
            });
        }

        private FixedDocument GeneratePrintReport(DailyExpenseReport report)
        {
            FixedDocument document = new FixedDocument();
            FixedPage fixedPage = new FixedPage();


            DataTemplate dt = Application.Current.FindResource("DailyExpenseReportPrint") as DataTemplate;
           

            var contentOfPage = new UserControl();
            contentOfPage.ContentTemplate = dt;

            contentOfPage.Content = report;
           
            var conv = new LengthConverter();

            double width = (double)conv?.ConvertFromString("7cm");

            double height = document.DocumentPaginator.PageSize.Height;
            contentOfPage.Width = width;
            document.DocumentPaginator.PageSize = new Size(width, height);


            fixedPage.Children.Add(contentOfPage);
            PageContent pageContent = new PageContent();
            ((IAddChild)pageContent).AddChild(fixedPage);

            document.Pages.Add(pageContent);
            return document;
        }

        public void PrintReport()
        {
            
            if (OpennedReport == null)
            {
                ToastNotification.Notify("Select a report to print");
                return;
            }

                FixedDocument fixedDocument = GeneratePrintReport(OpennedReport);
                var printers = PrinterSettings.InstalledPrinters.Cast<string>().ToList();

                IList<PrinterItem> printerItems = null;
                var PrinterItemSetting = AppConfigurationManager.Configuration<List<PrinterItem>>("PrintSettings");


            printerItems = PrinterItemSetting.Where(item => item.SelectedReceipt).ToList();

            foreach (var e in printerItems)
            {
                if (printers.Contains(e.Name))
                {
                    PrintDialog dialog = new PrintDialog { PrintQueue = new PrintQueue(new PrintServer(), e.Name) };
                    dialog.PrintDocument(fixedDocument.DocumentPaginator, "DailyExpenseReport");
                }
            }

            
        }

        public void OpenReport(DailyExpenseReport selected)
        {
            OpennedReport = selected;
            
            IsOpennedReportTabOpen = true;
            
        }

        public void CloseReport()
        {
            OpennedReport = null;
            IsOpennedReportTabOpen = false;
        }
    }

    public class Expense:PropertyChangedBase
    {
        public string Description { get; set; }

        public decimal Amount { get; set; }
    }

    
}