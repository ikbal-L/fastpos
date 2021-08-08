using System;
using System.Runtime.Serialization;

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
