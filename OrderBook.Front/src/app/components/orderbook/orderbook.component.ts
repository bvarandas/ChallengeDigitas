import { Component, TemplateRef } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@aspnet/signalr';
import { OrderBook, OrderBookData } from 'src/app/OrderBook';
import { OrderbookService } from 'src/app/orderbook.service';

@Component({
  selector: 'app-orderbook',
  templateUrl: './orderbook.component.html',
  styleUrls: ['./orderbook.component.css'],
  
})
export class OrderbookComponent {
  orderBookBtc: OrderBook;
  orderBookEth: OrderBook;
  orderBookDataBtc: OrderBookData;
  orderBookDataEth: OrderBookData;

  private _hubConnection: HubConnection;
  title = 'ChallengeDigitas-Angular';
  
  constructor()
  {
    this.CreateConnection();
    this.startConnection();
  }
  
  connectToMessageBroker(){
    this._hubConnection.invoke('ConnectToMessageBroker');
    
  }

  private CreateConnection(){
    this._hubConnection = new HubConnectionBuilder()
                              .withUrl("http://localhost:9010/hubs/brokerhub")
                              .build();
    

    this._hubConnection.on('ReceiveMessageBtc', 
      (data: OrderBook)=> 
      { 
        this.orderBookBtc = data; 
      });

      this._hubConnection.on('ReceiveMessageEth', 
      (data: OrderBook)=> 
      { 
        this.orderBookEth = data; 
      });

      this._hubConnection.on('ReceiveMessageDataBtc', 
      (data: OrderBookData)=> 
      { 
        this.orderBookDataBtc = data; 
      });

      this._hubConnection.on('ReceiveMessageDataEth', 
      (data: OrderBookData)=> 
      { 
        this.orderBookDataEth = data; 
      });


  }

  private startConnection() : void {
    this._hubConnection
    .start()
    .then(()=> {
      console.log('Hub connection started');
      this.connectToMessageBroker();
      
    })
    .catch(()=> {
      setTimeout(() => { this.startConnection();}, 5000);
    });
  }

  

}
// - Dados de cotação
  //Maior e menor preço de cada ativo
  //Média de preço de cada ativo acumulada nos ultimos 5 segundos
  //Média de preço de cada ativo acumulada nos ultimos 5 segundos
  //Média de quantidade acumulada de cada ativo

  // - Dados da API
  // COMPRA
  // Deve ser informado a operação (compra ou venda), o instrumento e a quantidade 
  // Calcular o melhor preço para a quantidade total: 
  //       - Ordernar todos os itens da coleção de "asks" do JSON contido na ultima atualização recebida em ordem crescente de preço.
  //       - Calcaular o valor correspondente a 100 BTCs varrendo  essa coleção e  multiplicando quantidade x valor até chegar a 100 BTC. 
  //         Nesse caso serão necessários, provavelmente, vários itens da coleção para cumprir a quantidade de 100 BTC dados que os itens 
  //         isoladamente tendem a ser de pequena quantidade
  //       - Caso quantidade seja atendida já com o primeiro item da coleção, somente retornar o preço de quantidade desejada x valor
  
  // VENDA
  // Se for solicitado uma venda, fazer a mesma operação, mas na coleção de "bids"