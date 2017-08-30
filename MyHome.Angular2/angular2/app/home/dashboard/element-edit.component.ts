import { Component, OnInit, Input, NgZone, SimpleChanges } from '@angular/core';

import { PresetElementModel, PresetRowModel, PresetColumnModel } from 'app/models';
import { DataService } from 'app/services/data.service'

@Component({
    selector: 'element-edit',
    templateUrl: './element-edit.component.html',
    //styleUrls: ['./element-edit.component.css']
})
export class ElementEditComponent implements OnInit {

    @Input() elementId: string;
    @Input() row: PresetRowModel;
    @Input() column: PresetColumnModel;
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

    changeType(element, type)
    {
        this.column.elements.map((v, i) => {
            if (v.id == element.id) {
                v.viewType = type;
            };
        });        
    }

    delete(element) {
        this.column.elements.map((v, i) => {
            if (v.id == element.id) {
                this.column.elements.splice(i, 1);
            };
        });
    }
}
