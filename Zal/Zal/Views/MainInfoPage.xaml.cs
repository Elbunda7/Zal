using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Zal.Elements;

namespace Zal.Views
{
	[XamlCompilation(XamlCompilationOptions.Compile)]
	public partial class MainInfoPage : ContentPage
	{
        private int numOfColumns = 3;
        private double itemSize = 0;
        private double pageWidthSize = 0;

        public MainInfoPage()
        {
            InitializeComponent();
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
            if (pageWidthSize != width)
            {
                pageWidthSize = width;
                OnOrientationChanged(width, height);
            }
        }

        private void OnOrientationChanged(double width, double height)
        {
            if (width > height)
            {
                numOfColumns = 5;
            }
            else
            {
                numOfColumns = 3;
            }
            double spaces = ContentGrid.ColumnSpacing;
            double newItemSize = (width - spaces * (numOfColumns + 1)) / numOfColumns;
            if (itemSize != newItemSize)
            {
                itemSize = newItemSize;
                InitGrid();
            }
        }

        private void InitGrid()
        {
            ContentGrid.RowDefinitions = new RowDefinitionCollection();
            ContentGrid.ColumnDefinitions = new ColumnDefinitionCollection();
            ContentGrid.Children.Clear();
            InfoGridItem.Randomize(8);
            for (int i = 0; i < numOfColumns; i++)
            {
                ContentGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = GridLength.Star });
            }
            for (int j = 0; j < 4; j++)
            {
                ContentGrid.RowDefinitions.Add(new RowDefinition() { Height = itemSize });
                for (int i = 0; i < numOfColumns; i++)
                {
                    InfoGridItem item = new InfoGridItem()
                    {
                        Text = "Title",
                        Icon = "ic_person_black_24dp.png"
                    };
                    ContentGrid.Children.Add(item, i, j);
                }
            }
        }
    }
}