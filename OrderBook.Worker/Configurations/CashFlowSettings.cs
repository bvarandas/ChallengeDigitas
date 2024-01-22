namespace OrderBook.Queue.Worker.Configurations;

public class CashFlowSettings
{
    public string ConnectionString { get; set; } = null!;

    public string DatabaseName { get; set; } = null!;

    public string CashFlowCollectionName { get; set; } = null!;
}
