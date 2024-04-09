using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using NewEventLogDLL;
using ReceivePartsDLL;
using DateSearchDLL;

namespace CopyInventoryTables
{
    /// <summary>
    /// Interaction logic for ReceiveParts.xaml
    /// </summary>
    public partial class ReceiveParts : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        ReceivePartsClass TheReceivePartsClass = new ReceivePartsClass();
        TWCInventoryClass TheTWCInventoryClass = new TWCInventoryClass();
        DateSearchClass TheDateSearchClass = new DateSearchClass();

        //setting up the data
        OldReceivedPartsDataSet TheOldReceivedPartsDataSet;
        ReceivePartsDataSet TheReceivePartsDataSet;
        FindReceivePartsByTransactionDateDataSet TheFindReceivePartsByTransactionDateDataSet= new FindReceivePartsByTransactionDateDataSet();

        //setting up global variables
        int gintReceivedCounter;
        int gintReceivedUpperLimit;

        public ReceiveParts()
        {
            InitializeComponent();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            TheMessagesClass.CloseTheProgram();
        }

        private void btnMainMenu_Click(object sender, RoutedEventArgs e)
        {
            MainMenu MainMenu = new MainMenu();
            MainMenu.Show();
            Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                TheOldReceivedPartsDataSet = TheTWCInventoryClass.GetOldReceivedPartsInfo();

                TheReceivePartsDataSet = TheReceivePartsClass.GetReceivePartsInfo();

                gintReceivedCounter = TheReceivePartsDataSet.receiveparts.Rows.Count;

                gintReceivedUpperLimit = gintReceivedCounter - 1;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Copy Inventory Tables // Receive Parts // Window Loaded " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }

        private void btnProcess_Click(object sender, RoutedEventArgs e)
        {
            //setting local variables
            int intCounter;
            int intNumberOfRecords;
            int intTransactionID;
            int intRecordsReturned;
            int intEmployeeID;
            string strMSRNumber;
            int intReverseCounter = -1;
            DateTime datLimitDate = DateTime.Now;
            DateTime datTransactionDate;

            PleaseWait PleaseWait = new PleaseWait();
            PleaseWait.Show();

            try
            {
                //getting ready for the loop
                intNumberOfRecords = TheOldReceivedPartsDataSet.ReceivedParts.Rows.Count - 1;

                datLimitDate = TheDateSearchClass.SubtractingDays(datLimitDate, 30);

                for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    intTransactionID = TheOldReceivedPartsDataSet.ReceivedParts[intCounter].TransactionID;
                    datTransactionDate = TheOldReceivedPartsDataSet.ReceivedParts[intCounter].Date;

                    if(datTransactionDate > datLimitDate)
                    {
                        TheFindReceivePartsByTransactionDateDataSet = TheReceivePartsClass.FindReceivePartsByTransactionDate(datTransactionDate);

                        intRecordsReturned = TheFindReceivePartsByTransactionDateDataSet.FindReceivePartsByTransactionDate.Rows.Count;

                        if (TheOldReceivedPartsDataSet.ReceivedParts[intCounter].IsPartIDNull() == true)
                        {
                            intRecordsReturned = 1;
                        }

                        if (intRecordsReturned == 0)
                        {
                            if (TheOldReceivedPartsDataSet.ReceivedParts[intCounter].IsEmployeeIDNull() == true)
                            {
                                intEmployeeID = 0;
                            }
                            else
                            {
                                intEmployeeID = TheOldReceivedPartsDataSet.ReceivedParts[intCounter].EmployeeID;
                            }

                            if (TheOldReceivedPartsDataSet.ReceivedParts[intCounter].IsMSRNumberNull() == true)
                            {
                                strMSRNumber = "NOT ENTERED";
                            }
                            else
                            {
                                strMSRNumber = TheOldReceivedPartsDataSet.ReceivedParts[intCounter].MSRNumber;
                            }

                            ReceivePartsDataSet.receivepartsRow NewReceiveRow = TheReceivePartsDataSet.receiveparts.NewreceivepartsRow();

                            NewReceiveRow.EmployeeID = intEmployeeID;
                            NewReceiveRow.PartID = TheOldReceivedPartsDataSet.ReceivedParts[intCounter].PartID;
                            NewReceiveRow.PONumber = strMSRNumber;
                            NewReceiveRow.ProjectID = TheOldReceivedPartsDataSet.ReceivedParts[intCounter].InternalProjectID;
                            NewReceiveRow.Quantity = TheOldReceivedPartsDataSet.ReceivedParts[intCounter].QTY;
                            NewReceiveRow.TransactionDate = TheOldReceivedPartsDataSet.ReceivedParts[intCounter].Date;
                            NewReceiveRow.TransactionID = intReverseCounter;
                            NewReceiveRow.WarehouseID = TheOldReceivedPartsDataSet.ReceivedParts[intCounter].WarehouseID;

                            TheReceivePartsDataSet.receiveparts.Rows.Add(NewReceiveRow);
                            TheReceivePartsClass.UpdateReceivePartsDB(TheReceivePartsDataSet);
                            intReverseCounter--;
                        }
                    }
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Copy Inventory Tables // Receive Parts // Process Button " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }

            PleaseWait.Close();
        }
    }
}
