import { Component, OnInit, Input } from '@angular/core';

import { PresetElementModel } from 'app/models/models';

@Component({
  selector: 'element-select-panel',
  templateUrl: './element-select-panel.component.html',
  styleUrls: ['./element-select-panel.component.css']
})
export class ElementSelectPanelComponent implements OnInit {

  @Input() elements: Array<PresetElementModel>
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
}
