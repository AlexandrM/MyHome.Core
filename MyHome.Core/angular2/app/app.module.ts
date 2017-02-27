import { NgModule, ApplicationRef } from '@angular/core';
import { RequestOptions } from '@angular/http';
import { BrowserModule } from '@angular/platform-browser';
import { FormsModule } from '@angular/forms';
import { HttpModule } from '@angular/http';
import { RouterModule, PreloadAllModules } from '@angular/router';
import { removeNgStyles, createNewHosts, createInputTransfer } from '@angularclass/hmr';
import { RippleDirective } from 'ng2-ripple-directive';
import { ModalModule } from 'angular2-modal';
import { BootstrapModalModule } from 'angular2-modal/plugins/bootstrap';

import { ENV_PROVIDERS } from './environment';
import { ROUTES } from './app.routes';

import { IndicatorDetailsComponent } from './indicatorDetails';
import { DropdownModule } from 'ng2-bootstrap/dropdown';

import { CustomRequestOptions } from './shared/customRequestOptions';

import { AppComponent } from './app.component';
import { HomeComponent, DashboardComponent, ElementPanelComponent } from './home';
import { ScheduleComponent } from './schedule/schedule.component';

import { ElementDefaultComponent } from './elements-components/default/default.component';

import { ASE } from 'ase-ts-tools';

import { ManageHubService } from './services/manageHub.service';
import { DataService } from './services/data.service';
import { ElementService } from './services/element.service';
import { ElementItemEnumService } from './services/elementItemEnum.service';
import { ScheduleService } from './services/schedule.service';
import { ElementItemModeService } from 'app/services/elementItemMode.service'

const APP_PROVIDERS = [
    ASE,

    ManageHubService,
    ElementService,
    DataService,
    ElementItemEnumService,
    ScheduleService,
    ElementItemModeService
];

@NgModule({
  bootstrap: [
    AppComponent
  ],
  declarations: [
    AppComponent,
    RippleDirective,
    HomeComponent,
    IndicatorDetailsComponent,
    DashboardComponent,
    ElementPanelComponent,
    ScheduleComponent,

    ElementDefaultComponent,
  ],
  imports: [ // import Angular's modules
    BrowserModule,
    FormsModule,
    HttpModule,
    RouterModule.forRoot(ROUTES, { useHash: false, preloadingStrategy: PreloadAllModules }),
    DropdownModule.forRoot(),
    ModalModule.forRoot(),
    BootstrapModalModule    
  ],
  providers: [ // expose our Services and Providers into Angular's dependency injection
    ENV_PROVIDERS,
    APP_PROVIDERS
  ]
})
export class AppModule {

  constructor(public appRef: ApplicationRef) {    
  }

}

