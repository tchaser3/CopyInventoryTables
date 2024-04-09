/* Title:           Issued Parts
 * Date:            6-7-17
 * Author:          Terry Holmes */

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
using IssuedPartsDLL;
using DateSearchDLL;

namespace CopyInventoryTables
{
    /// <summary>
    /// Interaction logic for IssuedParts.xaml
    /// </summary>
    public partial class IssuedParts : Window
    {
        //setting up th classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        IssuedPartsClass TheIssuedPartsClass = new IssuedPartsClass();
        TWCInventoryClass TheTWCInventoryClass = new TWCInventoryClass();
        DateSearchClass TheDataSearchClass = new DateSearchClass();

        //Data set up
        OldIssuedPartsDataSet TheOldIssuedPartsDataSet;
        IssuedPartsDataSet TheIssuedPartsDataSet;
        FindIssuedPartsByTransactionDateDataSet TheFindIssuedPartsByTransactionDateDataSet = new FindIssuedPartsByTransactionDateDataSet();

        public IssuedParts()
        {
            InitializeComponent();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void btnMainMenu_Click(object sender, RoutedEventArgs e)
        {
            MainMenu MainMenu = new MainMenu();
            MainMenu.Show();
            Close();
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            TheMessagesClass.CloseTheProgram();
        }

        private void btnProcess_Click(object sender, RoutedEventArgs e)
        {
            //setting local variables
            int intCounter;
            int intNumberOfRecords;
            int intRecordsReturned;
            int intTransactionID;
            int intReverseCounter = -1;
            int intEmployeeID;
            int intPartID;
            DateTime datTransactionDate;
            DateTime datLimitDate = DateTime.Now;

            PleaseWait PleaseWait = new PleaseWait();
            PleaseWait.Show();

            try
            {
                intNumberOfRecords = TheOldIssuedPartsDataSet.IssuedParts.Rows.Count - 1;
                datLimitDate = TheDataSearchClass.SubtractingDays(datLimitDate, 30);

                for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    intTransactionID = TheOldIssuedPartsDataSet.IssuedParts[intCounter].TransactionID;
                    datTransactionDate = TheOldIssuedPartsDataSet.IssuedParts[intCounter].Date;

                    if(datTransactionDate > datLimitDate)
                    {
                        
                        if (TheOldIssuedPartsDataSet.IssuedParts[intCounter].IsQTYNull() == false)
                        {
                            if (TheOldIssuedPartsDataSet.IssuedParts[intCounter].IsPartIDNull() == false)
                            {
                                intPartID = TheOldIssuedPartsDataSet.IssuedParts[intCounter].PartID;

                                TheFindIssuedPartsByTransactionDateDataSet = TheIssuedPartsClass.FindIssuedPartsByTransactionDate(datTransactionDate, intPartID);

                                intRecordsReturned = TheFindIssuedPartsByTransactionDateDataSet.FindIssuedPartsByTransactionDate.Rows.Count;

                                if (TheOldIssuedPartsDataSet.IssuedParts[intCounter].IsEmployeeIDNull() == true)
                                {
                                    intEmployeeID = 0;
                                }
                                else
                                {
                                    intEmployeeID = TheOldIssuedPartsDataSet.IssuedParts[intCounter].EmployeeID;
                                }

                                if (intRecordsReturned == 0)
                                {
                                    IssuedPartsDataSet.issuedpartsRow NewPartRow = TheIssuedPartsDataSet.issuedparts.NewissuedpartsRow();

                                    NewPartRow.EmployeeID = intEmployeeID;
                                    NewPartRow.EntryEmployeeID = 0;
                                    NewPartRow.PartID = TheOldIssuedPartsDataSet.IssuedParts[intCounter].PartID;
                                    NewPartRow.ProjectID = TheOldIssuedPartsDataSet.IssuedParts[intCounter].InternalProjectID;
                                    NewPartRow.Quantity = TheOldIssuedPartsDataSet.IssuedParts[intCounter].QTY;
                                    NewPartRow.TransactionDate = TheOldIssuedPartsDataSet.IssuedParts[intCounter].Date;
                                    NewPartRow.TransactionID = intReverseCounter;
                                    NewPartRow.WarehouseID = TheOldIssuedPartsDataSet.IssuedParts[intCounter].WarehouseID;

                                    TheIssuedPartsDataSet.issuedparts.Rows.Add(NewPartRow);
                                    TheIssuedPartsClass.UpdateIssuedPartsDB(TheIssuedPartsDataSet);
                                    intReverseCounter--;
                                }
                            }

                        }
                    }
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Copy Inventory Tables // Issued Parts // Process Button " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }

            PleaseWait.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                //loading the data
                TheOldIssuedPartsDataSet = TheTWCInventoryClass.GetOldIssuedPartsInfo();

                TheIssuedPartsDataSet = TheIssuedPartsClass.GetIssuedPartsInfo();
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Copy Inventory Tables // Issued Parts // Window Loaded " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
            
        }
    }
}
