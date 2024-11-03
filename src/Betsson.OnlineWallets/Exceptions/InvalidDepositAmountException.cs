namespace Betsson.OnlineWallets.Exceptions
{
    public class InvalidDepositAmountException : Exception
    {
        public InvalidDepositAmountException() : base("Invalid deposit amount. Deposit amount must be greater than 0.")
        {
        }

        public InvalidDepositAmountException(string message) : base(message)
        {
        }

        public InvalidDepositAmountException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
