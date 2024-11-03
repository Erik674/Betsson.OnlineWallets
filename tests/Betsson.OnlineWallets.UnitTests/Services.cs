using Betsson.OnlineWallets.Data.Models;
using Betsson.OnlineWallets.Data.Repositories;
using Betsson.OnlineWallets.Exceptions;
using Betsson.OnlineWallets.Models;
using Betsson.OnlineWallets.Services;
using Moq;

namespace Betsson.OnlineWallets.OnlineWalletServiceTests
{
    [SetUpFixture]
    internal class OnlineWalletServiceTestSetup
    {
        internal Mock<IOnlineWalletRepository> _mockRepository;
        internal OnlineWalletService _service;

        [OneTimeSetUp]
        public void Setup()
        {
            _mockRepository = new Mock<IOnlineWalletRepository>();
            _service = new OnlineWalletService(_mockRepository.Object);
        }
    }

    [TestFixture]
    internal class WithdrawFundsAsyncTests : OnlineWalletServiceTestSetup
    {
        [TestCase(50, 30, ExpectedResult = 20)]
        [TestCase(50, 50, ExpectedResult = 0)]
        public async Task<decimal> WithdrawFundsAsync_ShouldReturnCorrectBalance_WhenWithdrawalIsSuccessful(decimal initialBalance, decimal withdrawalAmount)
        {
            // Set up the mock to return an initial balance
            _mockRepository.Setup(repo => repo.GetLastOnlineWalletEntryAsync())
                           .ReturnsAsync(new OnlineWalletEntry { BalanceBefore = initialBalance, Amount = 0 });

            // Act
            var result = await _service.WithdrawFundsAsync(new Withdrawal { Amount = withdrawalAmount });

            // Assert
            _mockRepository.Verify(repo => repo.InsertOnlineWalletEntryAsync(It.IsAny<OnlineWalletEntry>()), Times.AtLeastOnce);
            return result.Amount;
        }

        [Test]
        public void WithdrawFundsAsync_ShouldThrowInsufficientBalanceException_WhenWithdrawalExceedsBalance()
        {
            // Arrange
            decimal initialBalance = 50m;
            decimal withdrawalAmount = 100m;

            // Set up the mock to return a balance of 50
            _mockRepository.Setup(repo => repo.GetLastOnlineWalletEntryAsync())
                           .ReturnsAsync(new OnlineWalletEntry { BalanceBefore = initialBalance, Amount = 0 });

            // Act & Assert
            Assert.ThrowsAsync<InsufficientBalanceException>(
                async () => await _service.WithdrawFundsAsync(new Withdrawal { Amount = withdrawalAmount }));
        }

        [Test]
        public void WithdrawFundsAsync_ShouldThrowNullReferenceException_WhenWithdrawalIsNull()
        {
            // Act & Assert
            Assert.ThrowsAsync<NullReferenceException>(
                async () => await _service.WithdrawFundsAsync(null));
        }
    }

    [TestFixture]
    internal class DepositFundsAsyncTests : OnlineWalletServiceTestSetup
    {
        [TestCase(50, 25, ExpectedResult = 75)]
        [TestCase(0, 50, ExpectedResult = 50)]
        public async Task<decimal> DepositFundsAsync_ShouldReturnCorrectBalance_WhenDepositIsSuccessful(decimal initialBalance, decimal depositAmount)
        {
            // Set up the mock to return an initial balance
            _mockRepository.Setup(repo => repo.GetLastOnlineWalletEntryAsync())
                           .ReturnsAsync(new OnlineWalletEntry { BalanceBefore = initialBalance, Amount = 0 });

            // Act
            var result = await _service.DepositFundsAsync(new Deposit { Amount = depositAmount });

            // Assert
            _mockRepository.Verify(repo => repo.InsertOnlineWalletEntryAsync(It.IsAny<OnlineWalletEntry>()), Times.AtLeastOnce);
            return result.Amount;
        }

        [Test]
        public void DepositFundsAsync_ShouldThrowNullReferenceException_WhenDepositIsNull()
        {
            // Act & Assert
            Assert.ThrowsAsync<NullReferenceException>(
                async () => await _service.DepositFundsAsync(null));
        }
    }
}
