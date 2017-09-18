import { NgModule, ApplicationRef } from '@angular/core';
import { RequestOptions } from '@angular/http';
import { BrowserModule } from '@angular/platform-browser';
import { FormsModule } from '@angular/forms';
import { HttpModule } from '@angular/http';
import { RouterModule, PreloadAllModules } from '@angular/router';
import { removeNgStyles, createNewHosts, createInputTransfer } from '@angularclass/hmr';
import { RippleDirective } from 'ng2-ripple-directive';
import { PopoverModule } from 'ng2-bootstrap/popover';
import { TabsModule } from 'ng2-bootstrap/tabs';
import { DatePickerDirective } from 'bootstrap-material-datetimepicker';

import { ModalModule } from 'ngx-modialog';
import { BootstrapModalModule } from 'ngx-modialog/plugins/bootstrap';

import { ENV_PROVIDERS } from './environment';
import { ROUTES } from './app.routes';

import { IndicatorDetailsComponent } from './components/indicatorDetails';
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';

import { CustomRequestOptions } from './shared/customRequestOptions';

import { AppComponent } from './app.component';
import { HomeComponent } from './components/home/home.component';

import { DashboardComponent } from './components/home/dashboard/dashboard.component';
import { ElementPanelComponent } from './components/home/dashboard/element-panel.component';
import { ElementEditComponent } from './components/home/dashboard/element-edit.component';

import { ScheduleComponent } from './components/schedule/schedule.component';
import { ElementSelectPanelComponent } from './components/indicatorDetails/element-panel/element-select-panel.component';
import { D3Graph } from './components/indicatorDetails/d3graph/d3graph.component';

import { ElementDefaultComponent } from './components/elements-components/default/default.component';
import { ElementNameComponent } from './components/elements-components/element-name/element-name.component';

import { ASE } from 'ase-ts-tools';

import { ManageHubService } from './services/manageHub.service';
import { DataService } from './services/data.service';
import { ElementService } from './services/element.service';
import { ElementItemEnumService } from './services/elementItemEnum.service';
import { ScheduleService } from './services/schedule.service';
import { ElementItemModeService } from './services/elementItemMode.service'
import { ElementItemService } from './services/elementItem.service';
import { SettingService } from './services/setting.service';

import * as moment from 'moment/moment';

const APP_PROVIDERS = [
    ASE,

    ManageHubService,
    ElementService,
    DataService,
    SettingService,
    ElementItemEnumService,
    ScheduleService,
    ElementItemModeService,
    ElementItemService,
];

@NgModule({
  bootstrap: [
    AppComponent
  ],
  declarations: [
    AppComponent,
    RippleDirective,
    DatePickerDirective,
    HomeComponent,
    IndicatorDetailsComponent,
    DashboardComponent,
    ElementPanelComponent,
    ScheduleComponent,
    ElementSelectPanelComponent,
    D3Graph,
    ElementEditComponent,

    ElementDefaultComponent,
    ElementNameComponent,
  ],
  imports: [ // import Angular's modules
    BrowserModule,
    FormsModule,
    HttpModule,
    RouterModule.forRoot(ROUTES, { useHash: false, preloadingStrategy: PreloadAllModules }),
    BsDropdownModule.forRoot(),
    ModalModule.forRoot(),
    BootstrapModalModule,
    PopoverModule.forRoot()  
  ],
  providers: [ // expose our Services and Providers into Angular's dependency injection
    ENV_PROVIDERS,
    APP_PROVIDERS
  ]
})
export class AppModule {

  constructor(public appRef: ApplicationRef) {    
    moment.locale("ru");
  }
}

