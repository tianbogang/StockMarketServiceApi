namespace StockMarket.Data.Dapper
{
    internal struct StockDbQuery
    {
        public const string StockExists = "Select 1 From Stocks Where Code= @Code";
        public const string GetStocks = "Select * From Stocks Order By Code";
        public const string GetStocksWhere = "Select * From Stocks Where Code Like @Filter Order By Code";
        public const string GetStockByCode = "Select * From Stocks Where Code= @Code";
        public const string AddStock = "Insert Into Stocks (Code, Name, Price, PreviousPrice, Exchange, Favorite) Values (@Code, @Name, @Price, @PreviousPrice, @Exchange, @Favorite)";
        public const string UpdateStock = "Update Stocks Set Name = @Name , Price = @Price, PreviousPrice = @PreviousPrice, Exchange = @Exchange, Favorite = @Favorite Where Code = @Code";
        public const string DeleteStock = "Delete From Stocks Where Code = @Code";
        public const string GetStocksPrice = "Select Code, Price From Stocks Where Code= @Code";
        public const string UpdateStockPrice = "Update Stocks Set Price = @Price, PreviousPrice = @PreviousPrice Where Code = @Code";
    }
}
