import { Component } from '@angular/core';

@Component({
  selector: 'indicatorDetails', 
  providers: [
  ],
  styleUrls: [ 
  ],
  templateUrl: './indicatorDetails.component.html'
})
export class IndicatorDetailsComponent {
    constructor() {

  }

  ngOnInit() {
    console.log('hello `IndicatorDetails` component');
  }

  submitState(value: string) {
  }
}
