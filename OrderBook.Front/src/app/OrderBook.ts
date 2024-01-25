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
}