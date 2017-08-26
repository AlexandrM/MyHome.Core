import { Component, OnInit, Input, OnChanges, SimpleChanges } from '@angular/core';
import { ASE } from 'ase-ts-tools';

import { ElementPanelComponent } from './element-panel/element-panel.component';
import { IDashboardModel, IDashboardRowModel, IDashboardColumnModel, IDashboardElementModel } from './interfaces';

declare var $: any;

@Component({
    selector: 'dashboard',
    templateUrl: './dashboard.component.html',
    styleUrls: ['./dashboard.component.css']
})
export class DashboardComponent implements OnInit, OnChanges  {

    @Input() preset: IDashboardModel;
    @Input() isEditMode: boolean;
    @Input() showElementPanels: boolean;
    @Input() elements: Array<IDashboardElementModel>
    colsizes: Array<number>;
    activeTab: string;
    availableElements: Array<IDashboardElementModel>

    constructor() {
        this.colsizes = Array(12).fill(12).map((x, i) => i);
    }

    ngOnInit() {
        this.preset.rows = this.preset.rows || [];
        this.availableElements = this.elements.filter(v => v.parent == null);
    }

    ngOnChanges(changes: SimpleChanges) {
    }

    onDrop(e, row: IDashboardRowModel, column: IDashboardColumnModel) {
        if (e.dataTransfer.getData('moving') == 'true') {
            ASE.a.recursive(this.preset.rows, (row) => {
                return ASE.a.recursive(row.columns, (rcolumn) => {
                    return ASE.a.recursive(rcolumn.elements, (element, idx) => {
                        if (element.id == e.dataTransfer.getData('elementId')) {
                            column.elements.push(rcolumn.elements.splice(idx, 1)[0]);
                            return true;
                        }
                        return false;
                    });
                });
            });
        } else {
            this.elements.map(v => {
                if (v.id == e.dataTransfer.getData('elementId')) {
                    v.viewType = 'default';
                    column.elements.push(v);
                }
            });
             /*.find((val, idx, arr) => {
                console.log(val.id, e.dataTransfer.getData('elementId'));
                return val.id == e.dataTransfer.getData('elementId');
                /*return val.items.find((val2, idx2, arr2) => {
                    if (val2.id == e.dataTransfer.getData('elementId')) {
                        val2.viewType = 'default';
                        column.elements.push(val2);
                        return true;
                    }
                    return false;
                }) != null;
            });*/
        }
        e.preventDefault();
    }

    ondragover(e) {
        if (e.toElement.classList.contains('panel')) {
            e.preventDefault();
        }
    }

    onDragstart(e, element) {
        e.dataTransfer.setData('moving', true);
        e.dataTransfer.setData('elementId', element.id);
    }

    /*getElements() {
        console.log(111);
        return this.elements.filter(v => v.parent == null);
    }*/

    addColumn(row: IDashboardRowModel, size: number) {
        if (row.columns == null) {
            row.columns = new Array<IDashboardColumnModel>();
        }
        row.columns.push({
            size: size,
            order: row.columns.length,
            elements: new Array<IDashboardElementModel>()
        });
    }

    findElement(elementId) {
        let ret = this.elements.find((v, i, a) => {
            return v.id == elementId;
        });

        return ret == null ? "" : ret.name;
    }

    changeType(element, type) {
        var e1 = this.elements.find((v, i) => {
            return v.id == element.id;
        });
        if (e1 != null) {
            element.viewType = type;
        }
        ASE.a.recursive(this.preset.rows, (row) => {
            return ASE.a.recursive(row.columns, (rcolumn) => {
                return ASE.a.recursive(rcolumn.elements, (elementR, idx) => {
                    if (elementR.id == element.id) {
                        elementR.viewType = type;
                    }
                    return false;
                });
            });
        });        
    }

    delete(column: IDashboardColumnModel, element: IDashboardElementModel) {
        column.elements.find((v, i) => {
            if (v.id == element.id) {
                column.elements.splice(i, 1);
                return true;
            }
        });
    }
}
