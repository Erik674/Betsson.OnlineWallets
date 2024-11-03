namespace Betsson.OnlineWallets.Exceptions
{
    public class InvalidWithdrawalAmountException : Exception
    {
        public InvalidWithdrawalAmountException() : base("Invalid withdrawal amount. Withdrawal amount must be greater than 0.")
        {
        }

        public InvalidWithdrawalAmountException(string message) : base(message)
        {
        }

        public InvalidWithdrawalAmountException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
