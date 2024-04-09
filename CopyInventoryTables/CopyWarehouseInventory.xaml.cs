/* Title:           Copy Warehouse Inventory
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
using InventoryDLL;
using NewEventLogDLL;

namespace CopyInventoryTables
{
    /// <summary>
    /// Interaction logic for CopyWarehouseInventory.xaml
    /// </summary>
    public partial class CopyWarehouseInventory : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        InventoryClass TheInventoryClass = new InventoryClass();
        TWCInventoryClass TheTWCInventoryClass = new TWCInventoryClass();

        //setting up the data
        OldWarehouseInventoryDataSet TheOldWarehouseInventoryDataSet;
        FindWarehouseInventoryPartDataSet TheFindWarehouseInventoryPartDataSet = new FindWarehouseInventoryPartDataSet();

        public CopyWarehouseInventory()
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
            int intWarehouseID;
            int intQuantity;
            int intTransactionID;
            bool blnFatalError;

            PleaseWait PleaseWait = new PleaseWait();
            PleaseWait.Show();

            try
            {
                //getting the record count
                intNumberOfRecords = TheOldWarehouseInventoryDataSet.WarehouseInventory.Rows.Count - 1;

                //loop
                for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    if(TheOldWarehouseInventoryDataSet.WarehouseInventory[intCounter].IsTablePartIDNull() == false)
                    {
                        //loading variables
                        intPartID = TheOldWarehouseInventoryDataSet.WarehouseInventory[intCounter].TablePartID;
                        intWarehouseID = TheOldWarehouseInventoryDataSet.WarehouseInventory[intCounter].WarehouseID;
                        intQuantity = TheOldWarehouseInventoryDataSet.WarehouseInventory[intCounter].QTYOnHand;

                        //checking for the existance of the record
                        TheFindWarehouseInventoryPartDataSet = TheInventoryClass.FindWarehouseInventoryPart(intPartID, intWarehouseID);

                        //record count
                        intRecordsReturned = TheFindWarehouseInventoryPartDataSet.FindWarehouseInventoryPart.Rows.Count;

                        if (intRecordsReturned == 0)
                        {
                            blnFatalError = TheInventoryClass.InsertInventoryPart(intPartID, intQuantity, intWarehouseID);

                            if (blnFatalError == true)
                            {
                                TheMessagesClass.ErrorMessage("Contact IT");
                                return;
                            }
                        }
                        else if (intRecordsReturned > 0)
                        {
                            if (intQuantity != TheFindWarehouseInventoryPartDataSet.FindWarehouseInventoryPart[0].Quantity)
                            {
                                intTransactionID = TheFindWarehouseInventoryPartDataSet.FindWarehouseInventoryPart[0].TransactionID;

                                blnFatalError = TheInventoryClass.UpdateInventoryPart(intTransactionID, intQuantity);

                                if (blnFatalError == true)
                                {
                                    TheMessagesClass.ErrorMessage("Contact IT");
                                    return;
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Copy Inventory Tables // Copy Warehouse Inventory // Process Button " + Ex.Message);

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
            //this will load the existing data
            try
            {
                TheOldWarehouseInventoryDataSet = TheTWCInventoryClass.GetOldWarehouseInventoryInfo();
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Copy Inventory Tables // Copy Warehouse Inventory // Window Loaded " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
    }
}
