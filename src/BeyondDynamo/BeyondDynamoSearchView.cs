using Dynamo.Wpf.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BeyondDynamo.UI;
using Dynamo.ViewModels;

namespace BeyondDynamo
{
    class BeyondDynamoSearchView : IViewExtension
    {
        public SearchViewControl SearchControl {get; set;} 
        public string Name
        {
            get
            {
                return "Beyond Dynamo Search";
            }
        }

        public void Loaded(ViewLoadedParams p)
        {
        }

        public void Shutdown()
        {

        }

        public void Startup(ViewStartupParams p)
        {
        }

        public void Dispose() { }

        public string UniqueId
        {
            get
            {
                return Guid.NewGuid().ToString();
            }
        }
    }
}
