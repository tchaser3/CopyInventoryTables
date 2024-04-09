/* Title:           Copy Adjust Inventory
 * Date:            6-9-17
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
using AdjustInventoryDLL;
using NewEventLogDLL;

namespace CopyInventoryTables
{
    /// <summary>
    /// Interaction logic for CopyAdjustInventory.xaml
    /// </summary>
    public partial class CopyAdjustInventory : Window
    {
        //Setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        TWCInventoryClass TheTWCInventoryClass = new TWCInventoryClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        AdjustInventoryClass TheAdjustInventoryClass = new AdjustInventoryClass();

        //setting up the data
        OldAdjustInventoryDataSet TheOldAdjustInventoryDataSet;
        AdjustInventoryDataSet TheAdjustInventoryDataSet;
        FindAdjustInventoryByPartIDWarehouseIDEmployeeIDDateDataSet TheFindAdjustInventoryByPartIDWarehouseIDEmployeeIDDateDataSet = new FindAdjustInventoryByPartIDWarehouseIDEmployeeIDDateDataSet();

        int gintRecordCount;

        public CopyAdjustInventory()
        {
            InitializeComponent();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void btnProcess_Click(object sender, RoutedEventArgs e)
        {
            //setting local variables
            int intCounter;
            int intNumberOfRecords;
            int intRecordsReturned;
            int intPartID;
            int intQuantity;
            string strExplanation;
            int intEmployeeID;
            DateTime datTransactionDate;
            int intWarehouseID;
            bool blnFatalError;

            PleaseWait PleaseWait = new PleaseWait();
            PleaseWait.Show();

            try
            {
                //getting ready for the loop
                intNumberOfRecords = TheOldAdjustInventoryDataSet.adjustinventory.Rows.Count - 1;

                for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    if(TheOldAdjustInventoryDataSet.adjustinventory[intCounter].IsPartIDNull() == false)
                    {
                        intPartID = TheOldAdjustInventoryDataSet.adjustinventory[intCounter].PartID;
                        intWarehouseID = TheOldAdjustInventoryDataSet.adjustinventory[intCounter].WarehouseID;
                        intQuantity = TheOldAdjustInventoryDataSet.adjustinventory[intCounter].Quantity;
                        strExplanation = TheOldAdjustInventoryDataSet.adjustinventory[intCounter].Reason;
                        intEmployeeID = TheOldAdjustInventoryDataSet.adjustinventory[intCounter].EmployeeID;
                        datTransactionDate = TheOldAdjustInventoryDataSet.adjustinventory[intCounter].Date;

                        TheFindAdjustInventoryByPartIDWarehouseIDEmployeeIDDateDataSet = TheAdjustInventoryClass.FindAdjustInventoryByPartIDWarehouseIDEmployeeIDDate(intPartID, intWarehouseID, intEmployeeID, datTransactionDate);

                        intRecordsReturned = TheFindAdjustInventoryByPartIDWarehouseIDEmployeeIDDateDataSet.FindAdjustedInventoryByPartIDWarehouseIDEmployeeIDDate.Rows.Count;

                        if(intRecordsReturned == 0)
                        {
                            AdjustInventoryDataSet.adjustinventoryRow NewAdjustRow = TheAdjustInventoryDataSet.adjustinventory.NewadjustinventoryRow();

                            NewAdjustRow.EmployeeID = intEmployeeID;
                            NewAdjustRow.Explanation = strExplanation;
                            NewAdjustRow.PartID = intPartID;
                            NewAdjustRow.Quantity = intQuantity;
                            NewAdjustRow.TransactionDate = datTransactionDate;
                            NewAdjustRow.TransactionID = gintRecordCount;
                            NewAdjustRow.WarehouseID = intWarehouseID;

                            TheAdjustInventoryDataSet.adjustinventory.Rows.Add(NewAdjustRow);
                            TheAdjustInventoryClass.UpdateAdjustInventoryDB(TheAdjustInventoryDataSet);
                            gintRecordCount--;
                        }
                    }
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Copy Inventory Tables // Copy Adjust Inventory // Process Button " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }

            PleaseWait.Close();
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

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                //loading up the data set
                TheOldAdjustInventoryDataSet = TheTWCInventoryClass.GetOldAdjustInventoryInfo();

                TheAdjustInventoryDataSet = TheAdjustInventoryClass.GetAdjustInventoryInfo();

                gintRecordCount = 0;
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Copy Inventory Tables // Copy Adjust Inventory " + Ex.Message);

                TheEventLogClass.Equals(Ex.ToString());
            }
        }
    }
}
