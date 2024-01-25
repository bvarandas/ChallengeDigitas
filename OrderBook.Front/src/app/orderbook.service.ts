import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';

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


  constructor(private http: HttpClient) { }
}
