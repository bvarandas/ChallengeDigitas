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
    maxPrice: number;
    minPrice: number;
    averagePrice: number;
    averagePriceLast5Seconds: number;
    averageAmountQuantity: number;
}