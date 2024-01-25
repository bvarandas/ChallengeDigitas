import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppComponent } from './app.component';
import { OrderbookComponent } from './components/orderbook/orderbook.component';
import { OrderbookService } from './orderbook.service';
import { CurrencyMaskModule } from 'ng2-currency-mask';
import { MatTabsModule } from '@angular/material/tabs'
import { HttpClientModule } from '@angular/common/http';

@NgModule({
  declarations: [
    AppComponent,
    OrderbookComponent
  ],
  imports: [
    BrowserModule,
    CurrencyMaskModule,
    MatTabsModule,
    HttpClientModule
  ],
  providers: [OrderbookService,OrderbookComponent],
  bootstrap: [AppComponent]
})
export class AppModule { }
