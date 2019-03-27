using Dynamo.Core;
using Dynamo.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BeyondDynamo.UI
{
    class ChangeNodeColorsViewModel : NotificationObject, IDisposable
    {
            private ReadyParams readyParams;
            public ReadyParams ReadyParamType
            {
                get
                {
                    readyParams = getReadyParams();
                    return readyParams;
                }
            }
            public ReadyParams getReadyParams()
            {
                return readyParams;
            }
            public ChangeNodeColorsViewModel(ReadyParams p)
            {
                readyParams = p;
            }
            public void Dispose()
            {
            }
    }
   
}
