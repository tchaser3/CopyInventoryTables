/* Title:           BOM Parts
 * Date:            6-8-17
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
using BOMPartsDLL;
using DateSearchDLL;

namespace CopyInventoryTables
{
    /// <summary>
    /// Interaction logic for BOMParts.xaml
    /// </summary>
    public partial class BOMParts : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        BOMPartsClass TheBOMPartsClass = new BOMPartsClass();
        TWCInventoryClass TheTWCInventoryClass = new TWCInventoryClass();
        DateSearchClass TheDateSearchClass = new DateSearchClass();

        //setting up the data
        OldBOMPartsDataSet TheOldBOMPartsDataSet;
        BOMPartsDataSet TheBOMPartsDataSet;
        FindBOMPartsByTransactionDateDataSet TheFindBOMPartsByTransactionDateDataSet = new FindBOMPartsByTransactionDateDataSet();

        public BOMParts()
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

        private void btnProcess_Click(object sender, RoutedEventArgs e)
        {
            //setting local variables
            int intCounter;
            int intNumberOfRecords;
            int intRecordsReturned;
            int intTransactionID = 0;
            DateTime datTransactionDate;
            int intReverseCounter = -11;
            DateTime datLimitDate = DateTime.Now;
            
            PleaseWait PleaseWait = new PleaseWait();
            PleaseWait.Show();

            try
            {
                //getting ready for the loop
                intNumberOfRecords = TheOldBOMPartsDataSet.BOMParts.Rows.Count - 1;

                datLimitDate = TheDateSearchClass.SubtractingDays(datLimitDate, 30);

                //loop to enter the records
                for (intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    intTransactionID = TheOldBOMPartsDataSet.BOMParts[intCounter].TransactionID;
                    datTransactionDate = TheOldBOMPartsDataSet.BOMParts[intCounter].Date;

                    if(datTransactionDate > datLimitDate)
                    {
                        TheFindBOMPartsByTransactionDateDataSet = TheBOMPartsClass.FindBOMPartsByTransactionDate(datTransactionDate);

                        intRecordsReturned = TheFindBOMPartsByTransactionDateDataSet.FindBOMPartsByTransactionDate.Rows.Count;

                        if (TheOldBOMPartsDataSet.BOMParts[intCounter].IsDateNull() == false)
                        {
                            if (intRecordsReturned == 0)
                            {
                                BOMPartsDataSet.bompartsRow NewPartRow = TheBOMPartsDataSet.bomparts.NewbompartsRow();

                                NewPartRow.EmployeeID = 0;
                                NewPartRow.PartID = TheOldBOMPartsDataSet.BOMParts[intCounter].PartID;
                                NewPartRow.ProjectID = TheOldBOMPartsDataSet.BOMParts[intCounter].InternalProjectID;
                                NewPartRow.Quantity = Convert.ToInt32(TheOldBOMPartsDataSet.BOMParts[intCounter].QTY);
                                NewPartRow.TransactionDate = TheOldBOMPartsDataSet.BOMParts[intCounter].Date;
                                NewPartRow.TransactionID = intReverseCounter;
                                NewPartRow.WarehouseID = TheOldBOMPartsDataSet.BOMParts[intCounter].WarehouseID;

                                TheBOMPartsDataSet.bomparts.Rows.Add(NewPartRow);
                                TheBOMPartsClass.UpdateBOMPartsDB(TheBOMPartsDataSet);
                                intReverseCounter--;
                            }
                        }
                    }

                    
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now,Convert.ToString(intTransactionID) + " Copy Inventory Tables // BOM Parts // Process Button " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }

            PleaseWait.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //this will load the data sets
            try
            {
                TheOldBOMPartsDataSet = TheTWCInventoryClass.GetOldBOMPartsInfo();

                TheBOMPartsDataSet = TheBOMPartsClass.GetBOMPartsInfo();
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Copy Inventory Tables // BOM Parts // Window Loaded " + Ex.Message);

                TheMessagesClass.ErrorMessage(Ex.ToString());
            }
        }
    }
}
