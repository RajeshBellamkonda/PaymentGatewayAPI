using NUnit.Framework;

namespace PaymentGateway.IntegrationTests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test1()
        {
            //_server = new TestServer(new WebHostBuilder().UseStartup<Startup>());
            //_client = _server.CreateClient();
            //// Pass a not valid model 
            //var response = await _client.PostAsJsonAsync("Track", new DataItem());
            //Assert.IsFalse(response.IsSuccessStatusCode);
            Assert.Pass();
        }
    }
}