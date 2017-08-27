import { Component, OnInit, Input, OnChanges, SimpleChanges } from '@angular/core';
import { ASE } from 'ase-ts-tools';

import { ElementPanelComponent } from './element-panel/element-panel.component';
import { IDashboardModel, IDashboardRowModel, IDashboardColumnModel } from './interfaces';
import { DataService } from 'app/services/data.service'
import { Presets, PresetModel, PresetRowModel, PresetColumnModel, PresetElementModel } from 'app/models';

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
    elements: Array<PresetElementModel>
    colsizes: Array<number>;
    activeTab: string;
    availableElements: Array<PresetElementModel>

    constructor(private dataService: DataService) {
        this.colsizes = Array(12).fill(12).map((x, i) => i);
        dataService.elements.subscribe(v => {
            this.elements = Array.from(v.values()).map(v => v.value);
            if (this.preset != null) {
                this.preset.rows = this.preset.rows || [];
            }
        });
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

    addColumn(row: IDashboardRowModel, size: number) {
        if (row.columns == null) {
            row.columns = new Array<IDashboardColumnModel>();
        }
        row.columns.push({
            size: size,
            order: row.columns.length,
            elements: new Array<PresetElementModel>()
        });
    }

    findElement(elementId) {
        let ret = this.elements.find((v, i, a) => {
            return v.id == elementId;
        });

        console.log(elementId, ret, this.elements);
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

    delete(column: IDashboardColumnModel, element: PresetElementModel) {
        column.elements.find((v, i) => {
            if (v.id == element.id) {
                column.elements.splice(i, 1);
                return true;
            }
        });
    }
}
