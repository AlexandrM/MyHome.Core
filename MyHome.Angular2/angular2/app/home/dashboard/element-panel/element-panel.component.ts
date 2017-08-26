import { Component, OnInit, Input, NgZone, SimpleChanges } from '@angular/core';

import { IDashboardElementModel } from '../interfaces';

@Component({
  selector: 'element-panel',
  templateUrl: './element-panel.component.html',
  styleUrls: ['./element-panel.component.css']
})
export class ElementPanelComponent implements OnInit {

  @Input() elements: Array<IDashboardElementModel>
  activeTab: string;

  constructor(private ngZone: NgZone) { 
  }

  ngOnInit() {
    console.log(this.elements);

    this.elements.sort((a, b) => {
      if(a.name < b.name) return -1;
      if(a.name > b.name) return 1;
      return 0;
    });
    this.activeTab = this.elements.length == 0 ? '' : this.elements[0].id;
  }

  ngOnChanges(changes: SimpleChanges) {
    console.log('ngOnChanges', changes);
  }

  onDragstart(e, element) {
    e.dataTransfer.setData('moving', false);
    e.dataTransfer.setData('elementId', element.id);
  }
}
