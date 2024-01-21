using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderBook.Infrastructure.Data;

public interface IOrderBookContext
{
    IMongoCollection<Core.Entities.OrderBook> OrderBooks { get; }
}
