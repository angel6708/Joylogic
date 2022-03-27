using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using Core.Infrastructure.Workflow;

namespace FlowViewModule.Flowchart
{
	class FlowchartModel
	{
		private ObservableCollection<Activity> _nodes = new ObservableCollection<Activity>();
		internal ObservableCollection<Activity> Nodes
		{
			get { return _nodes; }
		}

		private ObservableCollection<Link> _links = new ObservableCollection<Link>();
		internal ObservableCollection<Link> Links
		{
			get { return _links; }
		}
	}
}
