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
using FastPosFrontend.Configurations;
using FastPosFrontend.Helpers;
using FastPosFrontend.Navigation;
using Caliburn.Micro;
using FastPosFrontend.ViewModels.Settings;
using ServiceInterface.Interface;
using ServiceInterface.Model;
using ServiceLib.Service;
using ServiceLib.Service.StateManager;
using Utilities.Extensions.Collections;

namespace FastPosFrontend.ViewModels
{
    [NavigationItem(title: "Daily Earnings Reports", target: typeof(DailyExpenseReportsViewModel),"",isQuickNavigationEnabled:true)]
    [PreAuthorize("" +
        "Create_DailyEarningsReport," +
        "Read_DailyEarningsReport," +
        "Update_DailyEarningsReport," +
        "Delete_DailyEarningsReport")]
    public class DailyExpenseReportsViewModel : LazyScreen
    {
        
       
 

        public DailyExpenseReportsViewModel() : base()
        {
            SetupEmbeddedRightCommandBar();

        }

        protected override void Setup()
        {
            var reports = StateManager.GetAsync<DailyEarningsReport>();
            _data = new NotifyAllTasksCompletion(reports);
            
        }


        public override void Initialize()
        {
            var data = StateManager.GetAll<DailyEarningsReport>().ToList();

            var api = new RestApi();
            var (status, result) = GenericRest.GetThing<DailyEarningsReport>(api.Resource("daily-earnings-report", "get/date/today"));
            if (status is 200 || status is 201)
            {
                ReportOfTheDay = result;
                OpennedReport = ReportOfTheDay;
                _ = (data?.AddOrReplaceIf(ReportOfTheDay, r => r.IssuedDate.ToString("d") == DateTime.Today.Date.ToString("d")));
            }

            _reports = data.OrderByDescending(r => r.IssuedDate).ToObservableCollection();


            Reports = (CollectionView)CollectionViewSource.GetDefaultView(_reports);
            Reports.Filter += FilterReportsByIssuedDate;

            //Reports.GroupDescriptions.Add(new PropertyGroupDescription(nameof(DailyExpenseReport.IssuedDateYear)));
            //Reports.GroupDescriptions.Add(new PropertyGroupDescription(nameof(DailyExpenseReport.IssuedDateMonth)));
            //Reports.GroupDescriptions.Add(new PropertyGroupDescription(nameof(DailyExpenseReport.CashPaymentsTotal)));
            //Reports.GroupDescriptions.Add(new PropertyGroupDescription(nameof(DailyExpenseReport.DeliveryPaymentsTotal)));

            Reports.SortDescriptions.Add(new System.ComponentModel.SortDescription(nameof(DailyEarningsReport.IssuedDate),System.ComponentModel.ListSortDirection.Descending));
 
        }

        private bool FilterReportsByIssuedDate(object o)
        {
            if (o is DailyEarningsReport r)
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


        private bool _isOpennedReportTabOpen;

        public bool IsOpennedReportTabOpen
        {
            get { return _isOpennedReportTabOpen; }
            set { Set(ref _isOpennedReportTabOpen , value); }
        }


       
        private DailyEarningsReport _opennedReport;

        public DailyEarningsReport OpennedReport
        {
            get { return _opennedReport; }
            set 
            { 
                Set(ref _opennedReport, value);
             
            }
        }

        private DailyEarningsReport _reportOfTheDay;

        public DailyEarningsReport ReportOfTheDay
        {
            get { return _reportOfTheDay; }
            set
            {
                Set(ref _reportOfTheDay, value);
               
            }
        }

      

        private ObservableCollection<DailyEarningsReport> _reports;


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

        public void GenerateReport()
        {
            var parent = Parent as MainViewModel;

            var vm = new DailyExpenseReportInputDataViewModel(this);

            vm.OnReportGenerated(report =>
            {
                OpennedReport = report;
                var oldReport = _reports.FirstOrDefault(r => r.IssuedDate.Date == DateTime.Today.Date);
                if (oldReport!= null)
                {
                    _reports.Remove(oldReport);
                }
      
                _reports.Add(report);
                OpennedReport = report;
                Reports.Refresh();

            });

            parent?.OpenDialog(vm).OnClose(() =>
            {
                vm.Dispose();
            });
        }


        //public void ReloadReport()
        //{
        //    var parent = Parent as MainViewModel;

        //    var vm = new DailyExpenseReportInputDataViewModel(this, OpennedReport);

        //    vm.OnReportGenerated(report =>
        //    {
        //        OpennedReport = report;
        //        var oldReport = _reports.FirstOrDefault(r => r.IssuedDate.Date == DateTime.Today.Date);
        //        if (oldReport != null)
        //        {
        //            _reports.Remove(oldReport);
        //        }

        //        _reports.Add(report);
        //        OpennedReport = report;
        //        Reports.Refresh();

        //    });

        //    parent?.OpenDialog(vm).OnClose(() =>
        //    {
        //        vm.Dispose();
        //    });
        //}


        public void ReloadReport()
        {
            var (status,result) = DailyExpenseReportInputDataViewModel.SaveReport(OpennedReport?.Id);
            if (status is 200)
            {
                _ = _reports.ReplaceIf(result, i => i?.Id == result?.Id);
                OpennedReport = result;
            }
        }

        private FixedDocument GeneratePrintReport(DailyEarningsReport report)
        {
            FixedDocument document = new FixedDocument();
            FixedPage fixedPage = new FixedPage();


            DataTemplate dt = Application.Current.FindResource("DailyExpenseReportPrint") as DataTemplate;
           

            var contentOfPage = new UserControl();
            contentOfPage.ContentTemplate = dt;

            contentOfPage.Content = report;
           
            var conv = new LengthConverter();

            double width = (double)conv?.ConvertFromString("8cm");

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
                var PrinterItemSetting = ConfigurationManager.Get<PosConfig>().Printing.Printers;


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

        public void OpenReport(DailyEarningsReport selected)
        {
            OpennedReport = selected;
            IsOpennedReportTabOpen = true;
        }

        public void CloseReport()
        {
            OpennedReport = null;
            IsOpennedReportTabOpen = false;
        }

        public override void BeforeNavigateAway()
        {
            StateManager.Flush<DailyEarningsReport>();
        }
    }

    public class Expense:PropertyChangedBase
    {
        public string Description { get; set; }

        public decimal Amount { get; set; }
    }

    
}