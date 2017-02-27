import { Routes, RouterModule } from '@angular/router';
import { HomeComponent } from './home';
import { IndicatorDetailsComponent } from './indicatorDetails';
import { ScheduleComponent } from './schedule/schedule.component';

export const ROUTES: Routes = [
  { path: '',      component: HomeComponent },
  { path: 'home',  component: HomeComponent },
  { path: 'indicatorDetails',  component: IndicatorDetailsComponent },
  { path: 'schedule/:id',  component: ScheduleComponent },  
];
