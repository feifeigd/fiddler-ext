using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Fiddler;
using Inspectors;
namespace fiddler_ext.Inspectors.request
{
    public class RequestInspector : InspectorsBase, IRequestInspector2
    {
        HTTPRequestHeaders _requestHeaders;
        public HTTPRequestHeaders headers { get => _requestHeaders; set => _requestHeaders = value; }
    }
}
