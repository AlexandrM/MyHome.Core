import { Component, OnInit, Input, NgZone, SimpleChanges } from '@angular/core';

import { PresetElementModel } from 'app/models/models';
import { DataService } from 'app/services/data.service'

@Component({
    selector: 'element-panel',
    templateUrl: './element-panel.component.html',
    styleUrls: ['./element-panel.component.css']
})
export class ElementPanelComponent implements OnInit {

    elements: Array<PresetElementModel>
    elementTabs: Array<PresetElementModel>
    activeTab: string;

    constructor(private ngZone: NgZone, private dataService: DataService) {
        dataService.elements.subscribe(v =>{
            this.elements = Array.from(v.values()).map(v => v.value);
            this.elements.sort((a, b) => {
                if (a.name < b.name) return -1;
                if (a.name > b.name) return 1;
                return 0;
            });
            this.activeTab = this.elements.length == 0 ? '' : this.elements[0].id;
            this.elementTabs = this.elements.filter(x => x.parent == null);
        });
    }

    ngOnInit() {
    }

    ngOnChanges(changes: SimpleChanges) {
        console.log('ngOnChanges', changes);
    }

    onDragstart(e, element) {
        e.dataTransfer.setData('moving', false);
        e.dataTransfer.setData('elementId', element.id);
    }
}
