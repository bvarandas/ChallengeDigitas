import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { OrderBook } from './OrderBook';
import { Observable } from 'rxjs';

const httpOptions = {
  headers: new HttpHeaders({
    'Content-type': 'application/json'
  })
};
@Injectable({
  providedIn: 'root'
})
export class OrderbookService {
  url = 'http://localhost:5200/api/orderbook';

  constructor(private http: HttpClient) {   }

  GetAll(): Observable<OrderBook[]>{
    return this.http.get<OrderBook[]>(this.url);
  }
  
}
