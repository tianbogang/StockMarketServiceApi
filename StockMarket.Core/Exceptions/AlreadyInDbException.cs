using System;

namespace StockMarket.Core.Exceptions
{
    public class AlreadyInDbException : Exception
    {
        public override string Message => ("Already in Database: " + base.Message);
    }
}
