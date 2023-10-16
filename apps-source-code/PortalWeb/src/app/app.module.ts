import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';

import { AppComponent } from './app.component';
import { FileManagerModule } from './features/file-manager/file-manager.module';
import { EnvServiceProvider } from './features/shared/env.service';

@NgModule({
  declarations: [
    AppComponent
  ],
  imports: [
    BrowserModule,
    FileManagerModule,
  ],
  providers: [
    EnvServiceProvider,
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
