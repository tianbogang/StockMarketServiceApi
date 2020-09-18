using System;

namespace StockMarket.Core.Exceptions
{
    public class NotFoundInDbException : Exception
    {
        public override string Message => ("Not Found in Database: " + base.Message);
    }
}
