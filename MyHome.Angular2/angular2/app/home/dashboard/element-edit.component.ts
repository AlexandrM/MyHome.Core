import { Component, OnInit, Input, NgZone, SimpleChanges } from '@angular/core';

import { PresetElementModel } from 'app/models';
import { DataService } from 'app/services/data.service'

@Component({
    selector: 'element-edit',
    templateUrl: './element-edit.component.html',
    //styleUrls: ['./element-edit.component.css']
})
export class ElementEditComponent implements OnInit {

    @Input() elementId: string;
    element = new PresetElementModel();

    constructor(private dataService: DataService, private ngZone: NgZone) {
    }

    ngOnInit() {
        this.dataService.elements.subscribe(l => {
            let item = Array.from(l.values()).find(x => x.value.id == this.elementId);
            if (item != null) {
                item.subscribe(e => {
                    this.element = e;
                });
            }
        });
    }

    ngOnChanges(changes: SimpleChanges) {
    }
}
