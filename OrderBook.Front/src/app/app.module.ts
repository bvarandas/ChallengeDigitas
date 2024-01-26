import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppComponent } from './app.component';
import { OrderbookComponent } from './components/orderbook/orderbook.component';
import { OrderbookService } from './orderbook.service';
import { CurrencyMaskModule } from 'ng2-currency-mask';
import { MatTabsModule } from '@angular/material/tabs'
import { HttpClientModule } from '@angular/common/http';
import { AppRoutingModule } from './app-routing.module';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { FormsModule } from '@angular/forms';

@NgModule({
  declarations: [
    AppComponent,
    OrderbookComponent
  ],
  imports: [
    BrowserModule,
    FormsModule,
    AppRoutingModule,
    CurrencyMaskModule,
    MatTabsModule,
    HttpClientModule,
    BrowserAnimationsModule
  ],
  providers: [HttpClientModule,OrderbookComponent,OrderbookService],
  bootstrap: [AppComponent]
})
export class AppModule { }
