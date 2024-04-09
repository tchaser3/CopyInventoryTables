/* Title:           Copy Charter Inventory
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
using CharterInventoryDLL;
using NewEventLogDLL;

namespace CopyInventoryTables
{
    /// <summary>
    /// Interaction logic for CopyCharterInventory.xaml
    /// </summary>
    public partial class CopyCharterInventory : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        CharterInventoryClass TheCharterInventoryClass = new CharterInventoryClass();
        TWCInventoryClass TheTWCInventoryClass = new TWCInventoryClass();

        //setting up the data
        OldTWCInventoryDataSet TheOldTWCInventoryDataSet;
        CharterInventoryDataSet TheCharterInventoryDataSet;
        FindCharterWarehouseInventoryForPartDataSet TheFindCharterWarehouseInventoryForPartDataSet = new FindCharterWarehouseInventoryForPartDataSet();

        public CopyCharterInventory()
        {
            InitializeComponent();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DragMove();
        }

        private void btnProcess_Click(object sender, RoutedEventArgs e)
        {
            int intCounter;
            int intNumberOfRecords;
            int intRecordsReturned;
            int intQuantity;
            int intPartID;
            int intWarehouseID;
            int intTransactionID;
            bool blnFatalError = false;

            PleaseWait PleaseWait = new PleaseWait();
            PleaseWait.Show();

            try
            {
                //getting the record count
                intNumberOfRecords = TheOldTWCInventoryDataSet.Inventory.Rows.Count - 1;

                for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    if(TheOldTWCInventoryDataSet.Inventory[intCounter].IsTablePartIDNull() == false)
                    {
                        intPartID = TheOldTWCInventoryDataSet.Inventory[intCounter].TablePartID;
                        intWarehouseID = TheOldTWCInventoryDataSet.Inventory[intCounter].WarehouseID;
                        intQuantity = TheOldTWCInventoryDataSet.Inventory[intCounter].QTYResponible;

                        //running query statement
                        TheFindCharterWarehouseInventoryForPartDataSet = TheCharterInventoryClass.FindCharterWarehouseInventoryForPart(intPartID, intWarehouseID);

                        intRecordsReturned = TheFindCharterWarehouseInventoryForPartDataSet.FindCharterWarehouseInventoryForPart.Rows.Count;

                        if (intRecordsReturned == 0)
                        {
                            blnFatalError = TheCharterInventoryClass.InsertCharterInventory(intPartID, intWarehouseID, intQuantity);

                            if (blnFatalError == true)
                            {
                                TheMessagesClass.ErrorMessage("Contact IT");
                                return;
                            }
                        }
                        if (intRecordsReturned > 0)
                        {
                            intQuantity = TheFindCharterWarehouseInventoryForPartDataSet.FindCharterWarehouseInventoryForPart[0].Quantity;
                            intTransactionID = TheFindCharterWarehouseInventoryForPartDataSet.FindCharterWarehouseInventoryForPart[0].TransactionID;

                            if (intQuantity != TheOldTWCInventoryDataSet.Inventory[intCounter].QTYResponible)
                            {
                                blnFatalError = TheCharterInventoryClass.UpdateCharterInventory(intTransactionID, TheOldTWCInventoryDataSet.Inventory[intCounter].QTYResponible);

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
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Copy Inventory Tables // Copy Charter Inventory // Process Button " + Ex.Message);

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
                TheOldTWCInventoryDataSet = TheTWCInventoryClass.GetOldTWCInventory();

                TheCharterInventoryDataSet = TheCharterInventoryClass.GetCharterInventoryInfo();
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Copy Inventory Tables // Copy Charter Inventory // Window Loaded " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
    }
}
