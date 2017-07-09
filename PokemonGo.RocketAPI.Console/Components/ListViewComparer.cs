/*
 * Created by SharpDevelop.
 * User: Xelwon
 * Date: 24/09/2016
 * Time: 1:15
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Windows.Forms;

namespace PokeMaster.Components
{
// Compares two ListView items based on a selected column.
    public class ListViewComparer : System.Collections.IComparer
    {
        private int ColumnNumber;
        private SortOrder SortOrder;

        public ListViewComparer(int column_number, SortOrder sort_order)
        {
            ColumnNumber = column_number;
            SortOrder = sort_order;
        }

        // Compare two ListViewItems.
        public int Compare(object object_x, object object_y)
        {
            // Get the objects as ListViewItems.
            var item_x = object_x as ListViewItem;
            var item_y = object_y as ListViewItem;

            // Get the corresponding sub-item values.
            var string_x = item_x.SubItems.Count <= ColumnNumber ? "" : item_x.SubItems[ColumnNumber].Text;

            var string_y = item_y.SubItems.Count <= ColumnNumber ? "" : item_y.SubItems[ColumnNumber].Text;

            if (ColumnNumber == 3) //IV
            {
                string_x = string_x.Substring(0, string_x.IndexOf("%"));
                string_y = string_y.Substring(0, string_y.IndexOf("%"));

            }
            else if (ColumnNumber == 8) //HP
            {
                string_x = string_x.Substring(0, string_x.IndexOf("/"));
                string_y = string_y.Substring(0, string_y.IndexOf("/"));
            }

            // Compare them.
            int result;
            double double_x, double_y;
            if (double.TryParse(string_x, out double_x) &&
                double.TryParse(string_y, out double_y))
            {
                // Treat as a number.
                result = double_x.CompareTo(double_y);
            }
            else
            {
                DateTime date_x, date_y;
                if (DateTime.TryParse(string_x, out date_x) &&
                    DateTime.TryParse(string_y, out date_y))
                {
                    // Treat as a date.
                    result = date_x.CompareTo(date_y);
                }
                else
                {
                    // Treat as a string.
                    result = string_x.CompareTo(string_y);
                }
            }

            // Return the correct result depending on whether
            // we're sorting ascending or descending.
            switch (SortOrder) {
                case SortOrder.Ascending:
                    return result;
                default:
                    return -result;
            }
        }
    }
}
