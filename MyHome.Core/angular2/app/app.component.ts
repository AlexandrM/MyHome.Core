import { Component, ViewEncapsulation } from '@angular/core';
//import { RippleDirective } from 'ng2-ripple-directive';

/*
 * App Component
 * Top Level Component
 */
@Component({
  selector: 'app',
  encapsulation: ViewEncapsulation.None,
  styleUrls: [
    './app.component.css',
  ],
  templateUrl: './app.component.html'
})
export class AppComponent {

  constructor() {
  }

  ngOnInit() {
    console.log('AppComponent.ngOnInit()');
  }

}