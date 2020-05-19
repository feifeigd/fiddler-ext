
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Fiddler;
using Inspectors;

namespace fiddler_ext.Inspectors.response
{
    public class ResponseInspector : InspectorsBase, IResponseInspector2
    {
        HTTPResponseHeaders _responseHeaders;
        public HTTPResponseHeaders headers { get => _responseHeaders; set => _responseHeaders = value; }
    }
}
