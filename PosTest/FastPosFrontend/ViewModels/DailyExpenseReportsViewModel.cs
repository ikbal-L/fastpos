using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Drawing.Printing;
using System.Linq;
using System.Printing;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Markup;
using Caliburn.Micro;
using FastPosFrontend.Helpers;
using FastPosFrontend.Navigation;
using FastPosFrontend.ViewModels.Settings;
using MaterialDesignThemes.Wpf;
using ServiceInterface.Model;
using ServiceLib.Service;
using ServiceLib.Service.StateManager;

namespace FastPosFrontend.ViewModels
{
    [NavigationItem(title: "Daily Expense Reports", target: typeof(DailyExpenseReportsViewModel),"")]
    public class DailyExpenseReportsViewModel : LazyScreen
    {
        
        private bool _isReportGenerated;
        private DailyExpenseReport _report;

        public DailyExpenseReportsViewModel()
        {
            SetupEmbeddedRightCommandBar();
            Setup();
            OnReady();
            
        }

        protected override void Setup()
        {
            var reports = StateManager.GetAsync<DailyExpenseReport>();
            _data = new NotifyAllTasksCompletion(reports);
            
        }


        public override void Initialize()
        {
            _reports = StateManager.Get<DailyExpenseReport>();

            Reports = (CollectionView)CollectionViewSource.GetDefaultView(_reports);
            //Reports.GroupDescriptions.Add(new PropertyGroupDescription(nameof(DailyExpenseReport.IssuedDateYear)));
            //Reports.GroupDescriptions.Add(new PropertyGroupDescription(nameof(DailyExpenseReport.IssuedDateMonth)));
            //Reports.GroupDescriptions.Add(new PropertyGroupDescription(nameof(DailyExpenseReport.CashPaymentsTotal)));
            //Reports.GroupDescriptions.Add(new PropertyGroupDescription(nameof(DailyExpenseReport.DeliveryPaymentsTotal)));

            Report =  _reports?.FirstOrDefault(r => r.IssuedDate == DateTime.Today.Date);
            if (Report!= null)
            {
                IsReportGenerated = true;
            }
           
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
            set { Set(ref _opennedReport, value); }
        }
        private int _selectedTabIndex;

        public int SelectedTabIndex
        {
            get { return _selectedTabIndex; }
            set { Set(ref _selectedTabIndex ,value); }
        }

        private ICollection<DailyExpenseReport> _reports;

        //public ObservableCollection<DailyExpenseReport> Reports { get; set; }

        public CollectionView Reports { get; private set; }


        public void Generate()
        {
            var parent = Parent as MainViewModel;
            var vm = new DailyExpenseReportInputDataViewModel(this);
            vm.OnReportGenerated(report =>
            {
                Report = report;
                IsReportGenerated = true;
            });

            parent?.OpenDialog(vm).OnClose(() =>
            {
                vm.Dispose();
            });
        }

        private FixedDocument GeneratePrintReport()
        {
            FixedDocument document = new FixedDocument();
            FixedPage fixedPage = new FixedPage();


            DataTemplate dt = Application.Current.FindResource("DailyExpenseReportPrint") as DataTemplate;
           

            var contentOfPage = new UserControl();
            contentOfPage.ContentTemplate = dt;

            contentOfPage.Content = Report;
           
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
            ToastNotification.Notify("Printing");

          
                FixedDocument fixedDocument = GeneratePrintReport();
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