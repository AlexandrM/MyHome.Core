import { Component, OnInit, Input } from '@angular/core';

import { IDashboardElementModel } from '../interfaces';

@Component({
  selector: 'element-panel',
  templateUrl: './element-panel.component.html',
  styleUrls: ['./element-panel.component.css']
})
export class ElementPanelComponent implements OnInit {

  @Input() elements: Array<IDashboardElementModel>
  activeTab: string;

  constructor() { 
  }

  ngOnInit() {
  }

  ngOnChanges() {
    this.elements.sort((a, b) => {
      if(a.name < b.name) return -1;
      if(a.name > b.name) return 1;
      return 0;
    });

    this.activeTab = this.elements.length == 0 ? '' : this.elements[0].id;
  }

  onDragstart(e, element) {
    e.dataTransfer.setData('moving', false);
    e.dataTransfer.setData('elementId', element.id);
  }
}
