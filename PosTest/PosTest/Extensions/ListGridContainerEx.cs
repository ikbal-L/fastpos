using System.Linq;
using System.Windows;
using System.Windows.Controls;
using MahApps.Metro.Controls;

namespace PosTest.Extensions
{
    public class ListGridContainerEx
    {
        public static readonly DependencyProperty ColumnsProperty = DependencyProperty.RegisterAttached("Columns", typeof(int),typeof(Grid));

        public static void SetColumns(UIElement element, int value)
        {
            element.SetValue(ColumnsProperty,value);
            var grid = element as Grid;
            int col = 0;
            int row = 0;
            if (grid != null)
            {
                var gridChildren = grid.Children.Cast<UIElement>().ToList();
                var gridSize = gridChildren.Count;
                var rows = gridSize / value;
                grid.ColumnDefinitions.Clear();
                grid.RowDefinitions.Clear();
                for (int i = 0; i < value; i++)
                {
                    var  columnDefinition = new ColumnDefinition();
                    grid.ColumnDefinitions.Add(columnDefinition);
                }
                for (int i = 0; i < rows; i++)
                {
                    var rowDefinition = new RowDefinition();
                    grid.RowDefinitions.Add(rowDefinition);
                }
                foreach (var uiElement in gridChildren)
                {
                
                    uiElement.SetValue(Grid.ColumnProperty,col);
                    uiElement.SetValue(Grid.RowProperty,row);
                    if (col%value ==0)
                    {
                        row++;
                    }
                    col++;
                }
            }
        }

        public static int GetColumns(UIElement element)
        {
            return (int) element.GetValue(ColumnsProperty);
        }
    }
}