using System.Net.Http.Json;
using Betsson.OnlineWallets.Web.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Betsson.OnlineWallets.Web;

namespace Betsson.OnlineWallets.OnlineWalletAPITests
{
    [TestFixture]
    public class OnlineWalletControllerTests
    {
        private HttpClient _client;

        [SetUp]
        public void SetUp()
        {
            // Create a test server and client
            var server = new TestServer(new WebHostBuilder()
                .UseStartup<Startup>());

            _client = server.CreateClient();
        }

        [TearDown]
        public void TearDown()
        {
            _client.Dispose();
        }

        [Test]
        public async Task Balance_ShouldReturnCurrentBalance_WhenCalled()
        {
            // Act
            var response = await _client.GetAsync("/OnlineWallet/Balance");

            // Assert
            response.EnsureSuccessStatusCode();
            var balanceResponse = await response.Content.ReadFromJsonAsync<BalanceResponse>();
            Assert.IsNotNull(balanceResponse);
            Assert.That(balanceResponse.Amount, Is.GreaterThanOrEqualTo(0)); // Assuming balance can't be negative
        }

        [Test]
        public async Task Deposit_ShouldIncreaseBalance_WhenValidDeposit()
        {
            // Arrange
            var depositRequest = new DepositRequest { Amount = 50 };

            // Act
            var response = await _client.PostAsJsonAsync("/OnlineWallet/Deposit", depositRequest);

            // Assert
            response.EnsureSuccessStatusCode();
            var balanceResponse = await response.Content.ReadFromJsonAsync<BalanceResponse>();
            Assert.IsNotNull(balanceResponse);
            Assert.That(balanceResponse.Amount, Is.EqualTo(50)); // Assuming initial balance was 0
        }

        [Test]
        public async Task Withdraw_ShouldDecreaseBalance_WhenValidWithdrawal()
        {
            // Arrange
            await _client.PostAsJsonAsync("/OnlineWallet/Deposit", new DepositRequest { Amount = 50 }); // Ensure balance is set
            var initialBalance = await _client.GetAsync("/OnlineWallet/Balance");
            var initialBalanceResponse = await initialBalance.Content.ReadFromJsonAsync<BalanceResponse>();
            var withdrawRequest = new WithdrawalRequest { Amount = 20 };

            // Act
            var response = await _client.PostAsJsonAsync("/OnlineWallet/Withdraw", withdrawRequest);

            // Assert
            response.EnsureSuccessStatusCode();
            var finalBalanceResponse = await response.Content.ReadFromJsonAsync<BalanceResponse>();
            Assert.IsNotNull(finalBalanceResponse);
            Assert.That(finalBalanceResponse.Amount, Is.EqualTo(initialBalanceResponse.Amount - 20));
        }

        [Test]
        public async Task Deposit_ShouldReturnBadRequest_WhenInvalidAmount()
        {
            // Arrange
            var depositRequest = new DepositRequest { Amount = -10 }; // Invalid deposit amount

            // Act
            var response = await _client.PostAsJsonAsync("/OnlineWallet/Deposit", depositRequest);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.BadRequest));
        }

        [Test]
        public async Task Withdraw_ShouldReturnBadRequest_WhenInsufficientBalance()
        {
            // Arrange
            var currentBalance = await _client.GetAsync("/OnlineWallet/Balance");
            var currentBalanceResponse = await currentBalance.Content.ReadFromJsonAsync<BalanceResponse>();
            var withdrawRequest = new WithdrawalRequest { Amount = currentBalanceResponse.Amount + 1 }; // More than current balance

            // Act
            var response = await _client.PostAsJsonAsync("/OnlineWallet/Withdraw", withdrawRequest);

            // Assert
            Assert.That(response.StatusCode, Is.EqualTo(System.Net.HttpStatusCode.BadRequest));
        }
    }
}
