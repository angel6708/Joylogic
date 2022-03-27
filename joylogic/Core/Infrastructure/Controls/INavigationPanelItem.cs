using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Infrastructure.Controls
{
    public enum NavigationBehavior
    {
        Replace = 0,
        Animation = 1,
        Next = 3,
        Previous = 5,


    }

    public interface INavigationPanelItem
    {
        NavigationBehavior NavigationBehavior { get; set; }
    }
}
