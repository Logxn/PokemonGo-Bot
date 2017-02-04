/*
 * Created by SharpDevelop.
 * User: Xelwon
 * Date: 03/02/2017
 * Time: 22:59
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Windows.Forms;

namespace PokemonGo.RocketAPI.Console.Components
{
    
   public class ListViewItemComparer :  System.Collections.IComparer
    {
        int col;
        SortOrder order;
        public ListViewItemComparer()
        {
            col = 0;
            order = SortOrder.None;
        }
        public ListViewItemComparer(int column, SortOrder order)
        {
            col = column;
            this.order = order;
        }
        public int Compare(object x, object y)
        {
            var text1 = "";
            if (((ListViewItem)x).SubItems.Count > col)
                text1 =((ListViewItem)x).SubItems[col].Text;
            var text2 = "";
            if (((ListViewItem)y).SubItems.Count > col)
                text2 =((ListViewItem)y).SubItems[col].Text;

            int int1;
            var IsNumeric1 = int.TryParse(text1, out int1);
            if (IsNumeric1 ){
                int int2;
                var IsNumeric2 = int.TryParse(text2, out int2);
                if (IsNumeric2 )
                    return order == SortOrder.Ascending ? (int1 - int2) : (int2 - int1);
            }

            return order == SortOrder.Ascending ? String.Compare(text1, text2) : String.Compare(text2, text1);
        }
    }
}
