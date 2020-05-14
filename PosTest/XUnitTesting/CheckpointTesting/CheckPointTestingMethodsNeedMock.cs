using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace XUnitTesting.CheckpointTesting
{
    public class CheckPointTestingMethodsNeedMock 
    {
       
    }

    [DataContract]
    class GuidTest
    {
        [DataMember]
        public Guid Id { get; set; }

        [DataMember]
        public string Name { get; set; }
    }
}
