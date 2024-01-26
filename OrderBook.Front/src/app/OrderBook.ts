export class OrderBook{
    orderBookId: string;
    ticker:string;
    Timestamp:Date;
    Microtimestamp:Date;
    bids:BookLevel[];
    asks:BookLevel[];
}

export class BookLevel{
    side:string;
    price:number;
    amount:number;
    orderId:number;
    Timestamp:Date;
}

export class OrderBookData
{
    ticker:string;
    maxprice: number;
    minprice: number;
    averageprice: number;
    averagepriceLast5seconds: number;
    averageamountquantity: number;
}