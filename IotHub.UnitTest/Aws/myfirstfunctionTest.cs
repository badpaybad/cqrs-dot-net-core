using Amazon;
using Amazon.Lambda;
using Amazon.Lambda.Model;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace IotHub.UnitTesting.Aws
{
    [TestClass]
   public class myfirstfunctionTest
    {
        [TestMethod]
        public void invoke()
        {
            AmazonLambdaClient client = new AmazonLambdaClient("AKIAJHTR2OYL3LKMA7TA", "FugS3+bephKimPa9orSL7Ox2+mv1RreS4IFPK0oJ"
                , RegionEndpoint.USWest2);


            InvokeRequest ir = new InvokeRequest
            {
                FunctionName = "myfirstfunction",
                InvocationType = InvocationType.RequestResponse,
                Payload = "\"Hello Du\""
            };

            InvokeResponse response = client.InvokeAsync(ir).Result;

            var sr = new StreamReader(response.Payload);
            JsonReader reader = new JsonTextReader(sr);

            var serilizer = new JsonSerializer();
            var op = serilizer.Deserialize(reader);

            Console.WriteLine(op);
        }
    }
}
