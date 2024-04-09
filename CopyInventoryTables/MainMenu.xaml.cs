/* Title:           Main Menu
 * Date:            6-6-17
 * Author:          Terry Holmes*/

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

namespace CopyInventoryTables
{
    /// <summary>
    /// Interaction logic for MainMenu.xaml
    /// </summary>
    public partial class MainMenu : Window
    {
        //setting up the classes
        WPFMessagesClass TheMessagesClass = new WPFMessagesClass();

        public MainMenu()
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

        private void btnReceiveParts_Click(object sender, RoutedEventArgs e)
        {
            ReceiveParts ReceiveParts = new ReceiveParts();
            ReceiveParts.Show();
            Close();
        }

        private void btnIssuedParts_Click(object sender, RoutedEventArgs e)
        {
            IssuedParts IssuedParts = new IssuedParts();
            IssuedParts.Show();
            Close();
        }

        private void btnBOMParts_Click(object sender, RoutedEventArgs e)
        {
            BOMParts BOMParts = new BOMParts();
            BOMParts.Show();
            Close();
        }

        private void btnCopyCharterInventory_Click(object sender, RoutedEventArgs e)
        {
            CopyCharterInventory CopyCharterInventory = new CopyCharterInventory();
            CopyCharterInventory.Show();
            Close();
        }

        private void btnCopyWhseInventory_Click(object sender, RoutedEventArgs e)
        {
            CopyWarehouseInventory CopyWarehouseInventory = new CopyWarehouseInventory();
            CopyWarehouseInventory.Show();
            Close();
        }

        private void btnCopyAdjustInventory_Click(object sender, RoutedEventArgs e)
        {
            CopyAdjustInventory CopyAdjustInventory = new CopyAdjustInventory();
            CopyAdjustInventory.Show();
            Close();
        }
    }
}
