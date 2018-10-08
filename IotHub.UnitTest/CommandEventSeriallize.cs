using IotHub.CommandsEvents.SampleDomain;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace IotHub.UnitTesting
{
    [TestClass]
  public  class CommandEventSeriallize
    {
        [TestMethod]
        public void SampleCommandData()
        {
            var x = JsonConvert.SerializeObject(new CreateSample() {
                PublishedCommandId = Guid.NewGuid(),
                SampleId= Guid.NewGuid(),
                TokenSession="token",
                Version="version"
            });

            Console.WriteLine(typeof(CreateSample).FullName);
            Console.WriteLine(x);
        }
    }
}
