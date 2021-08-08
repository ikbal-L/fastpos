using System;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.Linq;
using System.Printing;
using System.Runtime.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;
using Caliburn.Micro;
using FastPosFrontend.Helpers;
using FastPosFrontend.ViewModels.Settings;
using ServiceInterface.Model;
using ServiceLib.Service;
using ServiceLib.Service.StateManager;

namespace FastPosFrontend.ViewModels
{
    [NavigationItemConfiguration(title: "Daily Expense Reports", target: typeof(DailyExpenseReportsViewModel))]
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
            var reports = StateManager.Get<DailyExpenseReport>();
            Reports = (reports != null && reports.Any())
                ? new BindableCollection<DailyExpenseReport>(reports)
                : new BindableCollection<DailyExpenseReport>();
            Report =  reports?.FirstOrDefault(r => r.IssuedDate == DateTime.Today.Date);
            if (Report!= null)
            {
                IsReportGenerated = true;
            }
           
        }

        private void SetupEmbeddedRightCommandBar()
        {
            this.EmbeddedRightCommandBar = new EmbeddedCommandBarViewModel()
            {
                Commands = new BindableCollection<EmbeddedCommandBarCommand>()
                {
                    new EmbeddedCommandBarCommand(Icon.Get("PrintSalesReport"), o => { PrintReport(); })
                }
            };
        }

        public bool IsReportGenerated
        {
            get => _isReportGenerated;
            set => Set(ref _isReportGenerated, value);
        }

        public DailyExpenseReport Report
        {
            get => _report;
            set => Set(ref _report, value);
        }

        public BindableCollection<DailyExpenseReport> Reports { get; set; }


        public void Generate()
        {
            var parent = this.Parent as MainViewModel;
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

            //contentOfPage.Content = CurrentOrder;
            //contentOfPage.Content = GenerateContent(CurrentOrder);
           
            contentOfPage.Content = Report;
           
            var conv = new LengthConverter();

            double width = (double)conv?.ConvertFromString("7cm");

            double height = document.DocumentPaginator.PageSize.Height;
            contentOfPage.Width = width;
            document.DocumentPaginator.PageSize = new Size(width, height);

            // fixedPage.Width = contentOfPage.Width;
            //fixedPage.Height = contentOfPage.Height;
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
                //PrintDialog dialog = new PrintDialog();
                //dialog.PrintQueue = LocalPrintServer.GetDefaultPrintQueue();
                //dialog.PrintDocument(fixedDocument.DocumentPaginator, "Print");

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
    }

    [DataContract]
    public class DailyExpenseReportInputData : PropertyChangedBase
    {
        private decimal _cashRegisterInitialAmount;
        private decimal _cashRegisterActualAmount;
        private DateTime _workTimeStart;
        private DateTime _workTimeEnd;

        public DailyExpenseReportInputData()
        {
          
        }

        [DataMember]
        public decimal CashRegisterInitialAmount
        {
            get => _cashRegisterInitialAmount;
            set => Set(ref _cashRegisterInitialAmount, value);
        }

        [DataMember]
        public decimal CashRegisterActualAmount
        {
            get => _cashRegisterActualAmount;
            set => Set(ref _cashRegisterActualAmount, value);
        }

        public DateTime WorkTimeStart
        {
            get => _workTimeStart;
            set => Set(ref _workTimeStart, value);
        }

        public DateTime WorkTimeEnd
        {
            get => _workTimeEnd;
            set => Set(ref _workTimeEnd, value);
        }
        [DataMember]
        public Dictionary<string,decimal> Expenses { get; set; }

        
    }

    public class Expense:PropertyChangedBase
    {
        public string Description { get; set; }

        public decimal Amount { get; set; }
    }
}