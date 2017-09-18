import { Component, ViewContainerRef, OnInit, Input, Output, NgZone } from '@angular/core';

import { Overlay } from 'ngx-modialog';
import { Modal } from 'ngx-modialog/plugins/bootstrap';

import { PresetElementModel, ElementItemValueModel, PresetElementEnumModel, ScheduleModel } from 'app/models/models';

import { DataService } from 'app/services/data.service'
import { ScheduleService } from 'app/services/schedule.service'
import { ElementItemModeService } from 'app/services/elementItemMode.service'

@Component({
    selector: 'element-default',
    providers: [
    ],
    templateUrl: './default.component.html',
    styleUrls: ['./default.component.css']
})
export class ElementDefaultComponent {

    private element = new PresetElementModel();
    private elementEnums = new Array<PresetElementEnumModel>();
    private schedules: Array<ScheduleModel>;

    @Input()
    elementId: string;

    constructor(
        private dataService: DataService,
        private scheduleService: ScheduleService,
        private elementItemModeService: ElementItemModeService,
        private modal: Modal
    ) {
    }

    ngOnInit() {
        this.dataService.elements.subscribe(data => {
            let item = Array.from(data.values()).find(x => x.value.id == this.elementId);
            if (item != null) {
                item.subscribe(e => {
                    this.element = e;
                    if (e.enumValues.length != 0) {
                        this.elementEnums = e.enumValues;
                        this.element.value.value = this.elementEnums.find(x => x.id == e.value.valueId);
                    }
                });
            }
        });
        this.scheduleService.get(this.elementId).subscribe(data => {
            this.schedules = data.list;
        });
    }

    changeValue(item: PresetElementEnumModel) {
        let value: ElementItemValueModel = {
            dateTime: new Date(),
            elementItemId: this.element.id,
            updated: new Date(),
            rawValue: null,
            valueId: item.id,
        };
        this.dataService.changeValue(value);
    }

    changeRawValue() {
        let defaultValue = '';
        if ((this.element != null) && (this.element.value != null)) {
            defaultValue = this.element.value.rawValue;
        }
        this.modal.prompt()
            .defaultValue(defaultValue)
            .size('sm')
            .isBlocking(true)
            .showClose(true)
            .keyboard(27)
            .title('Новое значение')
            .body('Значение')
            .okBtn('Ок').okBtnClass('btn btn-raised btn-primary')
            .cancelBtn('Отмена').cancelBtnClass('btn btn-raised btn-warning')
            .open()
            .then((dr) => {
                dr.result.then((data) => {
                    if (data == '') {
                        return;
                    }
                    let value: ElementItemValueModel = {
                        dateTime: new Date(),
                        elementItemId: this.element.id,
                        updated: new Date(),
                        rawValue: data,
                        valueId: null,
                    };
                    this.dataService.changeValue(value);
                }).catch((data) => {
                });
            });
    }

    modeSet(schedule: ScheduleModel) {
        if (schedule == null) {
            this.element.modeId = null;
        } else {
            this.element.modeId = schedule.id;
        }
        this.elementItemModeService.post({ id: this.element.id, modeId: schedule.id}).subscribe(data => {
            this.dataService.reloadElements();
        });
    }
}
