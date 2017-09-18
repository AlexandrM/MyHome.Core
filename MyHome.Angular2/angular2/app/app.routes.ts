import { Routes, RouterModule } from '@angular/router';
import { HomeComponent } from './components/home/home.component';
import { IndicatorDetailsComponent } from './components/indicatorDetails/indicatorDetails.component';
import { ScheduleComponent } from './components/schedule/schedule.component';
import { DebugRootComponent } from './components/debug/debug-root/debug-root.component';

export const ROUTES: Routes = [
  { path: '',      component: HomeComponent },
  { path: 'home',  component: HomeComponent },
  { path: 'indicatorDetails',  component: IndicatorDetailsComponent },
  { path: 'schedule/:id',  component: ScheduleComponent },  
  { path: 'debug',  component: DebugRootComponent },
];
