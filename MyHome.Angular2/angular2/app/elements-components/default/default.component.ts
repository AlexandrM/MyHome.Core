import { Component, ViewContainerRef, OnInit, Input, Output, NgZone } from '@angular/core';
import { Subject } from 'rxjs/Subject';
import { Overlay } from 'angular2-modal';
import { Modal } from 'angular2-modal/plugins/bootstrap';

import { PresetElementModel, ElementItemValueModel, PresetElementEnumModel, ScheduleModel } from './../../models';

import { ManageHubService } from 'app/services/manageHub.service'
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

    element = new PresetElementModel();
    elementEnums = new Array<PresetElementEnumModel>();
    schedules: Array<ScheduleModel>;

    @Input()
    elementId: string;

    constructor(
        private dataService: DataService,
        private scheduleService: ScheduleService,
        private elementItemModeService: ElementItemModeService,
		private ngZone: NgZone,
        private overlay: Overlay,
        private vcRef: ViewContainerRef,
        private modal: Modal,
    ) {
        overlay.defaultViewContainer = vcRef;
    }

    doRefresh(element) {
        this.ngZone.run(() => {            
            this.element = element || this.element;
            if (this.elementId == 'MyHome.Core.Plugin.iSpyManager.Mode') {
                console.log(element, this.element);
            }
            if (this.element.value != null) {
                this.elementEnums = this.element.enumValues;
                this.element.value.value = this.elementEnums.find(v => v.id == this.element.value.valueId);
            }
        });

        this.scheduleService.get(this.elementId).subscribe(r => {
            this.schedules = r.list;
        });
    }

    ngOnInit() {
        this.dataService.getElement(this.elementId).subscribe(v => {
            this.doRefresh(v);
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
        this.elementItemModeService.post(this.element).subscribe(data => {
            this.dataService.reloadElements();
        });
    }
}
