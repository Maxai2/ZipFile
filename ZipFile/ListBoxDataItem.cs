using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace ZipFile
{
    public class ListBoxDataItem
    {
        //public SolidColorBrush BackColor { get; set; } // фон элемента
        //public string Title { get; set; } // текст элемента
        public int Progress { get; set; } // прогресс

        public ListBoxDataItem(int progress = 0)
        {
          //  this.Title = title;
          //  this.BackColor = new SolidColorBrush(backColor);
            this.Progress = progress;
        }

        //public ListBoxDataItem(ListBoxDataItem item, int progress, Color backColor) : this(item.Title, backColor, progress)
        //{

        //}

        //public ListBoxDataItem(ListBoxDataItem item, Color backColor) : this(item.Title, backColor, item.Progress)
        //{

        //}
    }
}
