using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Core.Infrastructure.Controls
{
    public class NavigationPanelRegionAdapter : RegionAdapterBase<NavigationPanel>
    {
        /// <summary>
        /// Initializes a new instance of <see cref="ContentControlRegionAdapter"/>.
        /// </summary>
        /// <param name="regionBehaviorFactory">The factory used to create the region behaviors to attach to the created regions.</param>
        public NavigationPanelRegionAdapter(IRegionBehaviorFactory regionBehaviorFactory)
            : base(regionBehaviorFactory)
        { 
        }

        /// <summary>
        /// Adapts a <see cref="ContentControl"/> to an <see cref="IRegion"/>.
        /// </summary>
        /// <param name="region">The new region being used.</param>
        /// <param name="regionTarget">The object to adapt.</param>
        protected override void Adapt(IRegion region, NavigationPanel regionTarget)
        {
            if (regionTarget == null) throw new ArgumentNullException("regionTarget");
            bool contentIsSet = regionTarget.Children.Count != 0;

            if (contentIsSet)
            {
                // throw new InvalidOperationException("");
            }

            region.ActiveViews.CollectionChanged += delegate
            {
                var view = region.ActiveViews.FirstOrDefault() as FrameworkElement;

                if (view != null)
                {
                    var navigateItem = view as INavigationPanelItem;
                    if (navigateItem == null) { regionTarget.Replace(view); }
                    else
                    {
                        switch (navigateItem.NavigationBehavior)
                        {
                            case NavigationBehavior.Next:
                                regionTarget.Next(view);
                                break;
                            case NavigationBehavior.Previous:
                                regionTarget.Previous(view);
                                break;
                            default:
                                regionTarget.Replace(view);
                                break;
                        }
                    }
                }

            };

            region.Views.CollectionChanged +=
                (sender, e) =>
                {
                    if (e.Action == NotifyCollectionChangedAction.Add && region.ActiveViews.Count() == 0)
                    {
                       // region.Activate(e.NewItems[0]);
                    }
                };
        }

        /// <summary>
        /// Creates a new instance of <see cref="SingleActiveRegion"/>.
        /// </summary>
        /// <returns>A new instance of <see cref="SingleActiveRegion"/>.</returns>
        protected override IRegion CreateRegion()
        {
            return new SingleActiveRegion();
        }
    }
}