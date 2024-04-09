/* Title:           TWC Inventory Class
 * Date:            6-6-17
 * Author:          Terry Holmes */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NewEventLogDLL;
using ProjectsDLL;
using DateSearchDLL;

namespace CopyInventoryTables
{
    class TWCInventoryClass
    {
        //setting up the class
        WPFMessagesClass TheMessageClass = new WPFMessagesClass();
        EventLogClass TheEventLogClass = new EventLogClass();
        DateSearchClass TheDateSearchClass = new DateSearchClass();

        //setting up the data
        OldReceivedPartsDataSet aOldReceivedPartsDataSet;
        OldReceivedPartsDataSetTableAdapters.ReceivedPartsTableAdapter aOldReceivedPartTableAdapter;

        OldProjectsDataSet aOldProjectsDataSet;
        OldProjectsDataSetTableAdapters.internalprojectsTableAdapter aOldProjectTableAdapter;

        ProjectClass TheProjectClass = new ProjectClass();

        FindProjectByProjectIDDataSet TheFindProjectByProjectID = new FindProjectByProjectIDDataSet();
        ProjectsDataSet TheProjectsDataSet = new ProjectsDataSet();

        OldIssuedPartsDataSet aOldIssuedPartsDataSet;
        OldIssuedPartsDataSetTableAdapters.IssuedPartsTableAdapter aOldIssuedPartsTableAdapter;

        OldBOMPartsDataSet aOldBOMPartsDataSet;
        OldBOMPartsDataSetTableAdapters.BOMPartsTableAdapter aOLDBOMPartsTableAdapter;

        OldTWCInventoryDataSet aOldTWCInventoryDataSet;
        OldTWCInventoryDataSetTableAdapters.InventoryTableAdapter aOldTWCInventoryTableAdapter;

        OldWarehouseInventoryDataSet aOldWarehouseInventoryDataSet;
        OldWarehouseInventoryDataSetTableAdapters.WarehouseInventoryTableAdapter aOldWarehouseInventoryTableAdapter;

        OldAdjustInventoryDataSet aOldAdjustInventoryDataSet;
        OldAdjustInventoryDataSetTableAdapters.adjustinventoryTableAdapter aOldAdjustInventoryTableAdapter;

        public OldAdjustInventoryDataSet GetOldAdjustInventoryInfo()
        {
            try
            {
                aOldAdjustInventoryDataSet = new OldAdjustInventoryDataSet();
                aOldAdjustInventoryTableAdapter = new OldAdjustInventoryDataSetTableAdapters.adjustinventoryTableAdapter();
                aOldAdjustInventoryTableAdapter.Fill(aOldAdjustInventoryDataSet.adjustinventory);
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Copy Inventory Table // TWC Inventory Class // Get Old Adjust Inventory " + Ex.Message);

                TheMessageClass.ErrorMessage(Ex.ToString());
            }

            return aOldAdjustInventoryDataSet;
        }
        public OldWarehouseInventoryDataSet GetOldWarehouseInventoryInfo()
        {
            try
            {
                aOldWarehouseInventoryDataSet = new OldWarehouseInventoryDataSet();
                aOldWarehouseInventoryTableAdapter = new OldWarehouseInventoryDataSetTableAdapters.WarehouseInventoryTableAdapter();
                aOldWarehouseInventoryTableAdapter.Fill(aOldWarehouseInventoryDataSet.WarehouseInventory);
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Copy Inventory Tables // TWC Inventory Class // Get Old Warehouse Inventory Info " + Ex.Message);
            }

            return aOldWarehouseInventoryDataSet;
        }
        public OldTWCInventoryDataSet GetOldTWCInventory()
        {
            try
            {
                aOldTWCInventoryDataSet = new OldTWCInventoryDataSet();
                aOldTWCInventoryTableAdapter = new OldTWCInventoryDataSetTableAdapters.InventoryTableAdapter();
                aOldTWCInventoryTableAdapter.Fill(aOldTWCInventoryDataSet.Inventory);
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Copy Inventory Tables // TWC Inventory Class // Get Old TWC Inventory " + Ex.Message);
            }

            return aOldTWCInventoryDataSet;
        }
        public OldBOMPartsDataSet GetOldBOMPartsInfo()
        {
            try
            {
                aOldBOMPartsDataSet = new OldBOMPartsDataSet();
                aOLDBOMPartsTableAdapter = new OldBOMPartsDataSetTableAdapters.BOMPartsTableAdapter();
                aOLDBOMPartsTableAdapter.Fill(aOldBOMPartsDataSet.BOMParts);
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Copy Inventory Table // TWC Inventory Class // Get Old BOM Parts Info " + Ex.Message);

                TheMessageClass.ErrorMessage(Ex.ToString());
            }

            return aOldBOMPartsDataSet;
        }
        public OldIssuedPartsDataSet GetOldIssuedPartsInfo()
        {
            try
            {
                aOldIssuedPartsDataSet = new OldIssuedPartsDataSet();
                aOldIssuedPartsTableAdapter = new OldIssuedPartsDataSetTableAdapters.IssuedPartsTableAdapter();
                aOldIssuedPartsTableAdapter.Fill(aOldIssuedPartsDataSet.IssuedParts);
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "TWC Inventory Class // Get Old Issued Parts Info " + Ex.Message);
            }

            return aOldIssuedPartsDataSet;
        }
        public void UpdateProjects()
        {
            int intCounter;
            int intNumberOfRecords;
            int intRecordsReturn;
            int intProjectID;
            string strAssignedProjectID;
            string strProjectName;
            DateTime datTransactionDate = DateTime.Now;
            
            try
            {
                TheProjectsDataSet = TheProjectClass.GetProjectsInfo();

                aOldProjectsDataSet = new OldProjectsDataSet();
                aOldProjectTableAdapter = new OldProjectsDataSetTableAdapters.internalprojectsTableAdapter();
                aOldProjectTableAdapter.Fill(aOldProjectsDataSet.internalprojects);

                intNumberOfRecords = aOldProjectsDataSet.internalprojects.Rows.Count - 1;
                datTransactionDate = TheDateSearchClass.SubtractingDays(datTransactionDate, 1000);

                for(intCounter = 0; intCounter <= intNumberOfRecords; intCounter++)
                {
                    //setting up the variables
                    intProjectID = aOldProjectsDataSet.internalprojects[intCounter].internalProjectID;
                    
                    if((aOldProjectsDataSet.internalprojects[intCounter].IsProjectNameNull() == false) || (aOldProjectsDataSet.internalprojects[intCounter].IsTWCControlNumberNull() == false))
                    {
                        if(aOldProjectsDataSet.internalprojects[intCounter].IsProjectNameNull() == true)
                        {
                            strProjectName = "NOT ENTERED";
                        }
                        else
                        {
                            strProjectName = aOldProjectsDataSet.internalprojects[intCounter].ProjectName;
                        }
                        if(aOldProjectsDataSet.internalprojects[intCounter].IsTWCControlNumberNull() == true)
                        {
                            strAssignedProjectID = "NOT ENTERED";
                        }
                        else
                        {
                            strAssignedProjectID = aOldProjectsDataSet.internalprojects[intCounter].TWCControlNumber;
                        }

                        TheFindProjectByProjectID = TheProjectClass.FindProjectByProjectID(intProjectID);

                        intRecordsReturn = TheFindProjectByProjectID.FindProjectByProjectID.Rows.Count;

                        if(intRecordsReturn == 0)
                        {
                            ProjectsDataSet.projectsRow NewProjectRow = TheProjectsDataSet.projects.NewprojectsRow();

                            NewProjectRow.ProjectID = intProjectID;
                            NewProjectRow.AssignedProjectID = strAssignedProjectID;
                            NewProjectRow.ProjectName = strProjectName;
                            NewProjectRow.TransactionDate = datTransactionDate;

                            TheProjectsDataSet.projects.Rows.Add(NewProjectRow);
                            TheProjectClass.UpdateProjectsDB(TheProjectsDataSet);
                        }
                    }
                }
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Create Inventory Tables // TWC Inventory Class // Update Projects " + Ex.Message);

                TheMessageClass.ErrorMessage(Ex.ToString());
            }
        }
        public OldReceivedPartsDataSet GetOldReceivedPartsInfo()
        {
            try
            {
                aOldReceivedPartsDataSet = new OldReceivedPartsDataSet();
                aOldReceivedPartTableAdapter = new OldReceivedPartsDataSetTableAdapters.ReceivedPartsTableAdapter();
                aOldReceivedPartTableAdapter.Fill(aOldReceivedPartsDataSet.ReceivedParts);
            }
            catch (Exception Ex)
            {
                TheEventLogClass.InsertEventLogEntry(DateTime.Now, "Copy Inventory Tables // TWC Inventory Class // Get Old Received Parts Info " + Ex.Message);

                TheMessageClass.ErrorMessage(Ex.ToString());
            }

            return aOldReceivedPartsDataSet;
        }
    }
}
