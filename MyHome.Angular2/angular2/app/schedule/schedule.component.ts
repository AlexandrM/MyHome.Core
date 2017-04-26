import { Component, OnInit, ViewContainerRef } from '@angular/core';
import { Router, ActivatedRoute, Params } from '@angular/router';
import { Overlay } from 'angular2-modal';
import { Modal } from 'angular2-modal/plugins/bootstrap';

import { PresetElementModel, ScheduleModel, PresetElementEnumModel } from './../models';

import { ScheduleService } from './../services/schedule.service';

import { ASE } from 'ase-ts-tools';

@Component({
    selector: 'app-schedule',
    templateUrl: './schedule.component.html',
    styleUrls: ['./schedule.component.css']
})
export class ScheduleComponent implements OnInit {

    constructor(
        private route: ActivatedRoute,
        private router: Router,
        private scheduleService: ScheduleService,
        public A: ASE,
        overlay: Overlay,
        vcRef: ViewContainerRef,
        private modal: Modal
    ) {
        overlay.defaultViewContainer = vcRef;
    }
    
    schedules: ScheduleModel[];
    element: PresetElementModel = new PresetElementModel();
    schedule: ScheduleModel = new ScheduleModel();
    newName = '';
    scheduleRawList: string[];

    dayOfWeeks: string[] = ['ПН', 'ВТ', 'СР', 'ЧТ', 'ПТ', 'СБ', 'ВС'];
    hours: number[] = [];

    ngOnInit() {
        for(let i = 0; i < 24; i++) {
            this.hours.push(i);
        }
        this.load();
    }

    load() {
        this.route.params.subscribe(params => {
            let id = params['id'];
            this.scheduleService.get(params['id']).subscribe(data => {
                this.element =  data.element;
                this.element.enumValues = this.element.enumValues.sort((a, b) => { 
                    if(a.name < b.name) return -1;
                    if(a.name > b.name) return 1;
                    return 0; 
                });
                this.schedules =  data.list;
                this.schedules = this.schedules.sort((a, b) => { 
                    if(a.name < b.name) return -1;
                    if(a.name > b.name) return 1;
                    return 0; 
                });
            });
        });
    }

    add() {
        let model = { id: '', name: this.newName, elementItemId: this.element.id, scheduleHours: null };
        this.scheduleService.post(model).subscribe(data => {
            this.newName = '';
            this.load();
        });        
    }

    loadSchedule(schedule) {
        this.scheduleRawList = new Array<string>();
        this.schedule = schedule;
        this.schedule.scheduleHours = this.schedule.scheduleHours || [];
    }

    delete(schedule: ScheduleModel) {
        this.modal.confirm()
            .size('sm')
            .isBlocking(true)
            .showClose(true)
            .keyboard(27)
            .title('Подтверждение')
            .body('Удалить ' + schedule.name + '?')
            .okBtn('Ок').okBtnClass('btn btn-raised btn-danger')
            .cancelBtn('Отмена').cancelBtnClass('btn btn-raised btn-primary')
            .open()
            .then((dr) => {
                dr.result.then((data) => {
                    this.scheduleService.delete(schedule.id).subscribe(res => {
                        this.load();
                    });
                }).catch((data) => {
                });
            });
    }

    save() {
        this.scheduleService.post(this.schedule).subscribe(data => {
            this.schedule = new ScheduleModel();
            this.newName = '';
            this.load();
        });        
    }


    dhValue(day: number, hour: number): any { 
        if ((this.schedule.scheduleHours == null) || (this.element == null)) {
            return null;
        }
        let val = this.schedule.scheduleHours.find(v => { return v.dayOfWeek == day && v.hour == hour });
        if (val == null) {
            return null;
        }
        if (this.isEnum()) {
            let ret = this.element.enumValues.find(x => x.id == val.valueId);
            if (ret == null) {
                return null;
            }
            return ret;
        } else {
            return val.rawValue;
        }
    }

    dhValueText(day: number, hour: number) { 
        let ret = this.dhValue(day, hour);
        if (ret == null) {
            return 'Не выбрано';
        }
        if (this.isEnum()) {
            return ret.name;
        } else {
            return ret;
        }
    }

    dhClass(day: number, hour: number){
        let ret = this.dhValue(day, hour);
        if (ret == null) {
            return '';
        }
        if (!this.isEnum()) {
            if (this.scheduleRawList != null) {
                let idx = this.scheduleRawList.indexOf(ret);
                if (idx == -1) {
                    idx = this.scheduleRawList.push(ret) - 1;
                }                
                return 'color' + idx;
            }
            return '';
        }
        return 'color' + this.element.enumValues.indexOf(ret);
    }

    setValue(day, hour, valueId, rawValue) {
        let val = this.schedule.scheduleHours.find(v => { return v.dayOfWeek == day && v.hour == hour });
        if (this.isEnum()) {
            if (val == null) {
                this.schedule.scheduleHours.push({
                    scheduleId: this.schedule.id,
                    dayOfWeek: day,
                    hour: hour,
                    valueId: valueId,
                    rawValue: null,
                });
            } else {
                val.valueId = valueId;
            }
        } else {
            if (val == null) {
                this.schedule.scheduleHours.push({
                    scheduleId: this.schedule.id,
                    dayOfWeek: day,
                    hour: hour,
                    valueId: null,
                    rawValue: rawValue,
                });
            } else {
                val.rawValue = rawValue;
            }
        }
    }
    changeValue(day, hour, valueId, rawValue) {
        let val = this.schedule.scheduleHours.find(v => { return v.dayOfWeek == day && v.hour == hour });
        if (this.isEnum()) {
            this.setValue(day, hour, valueId, rawValue);
        } else { 
            if (rawValue == null) {
                this.modal.prompt()
                    .size('sm')
                    .isBlocking(true)
                    .showClose(true)
                    .keyboard(27)
                    .title('Значение')
                    .body('Введите Значение')
                    .okBtn('Ок').okBtnClass('btn btn-raised btn-primary')
                    .cancelBtn('Отмена').cancelBtnClass('btn btn-raised btn-warning')
                    .open()
                    .then((dr) => {
                        dr.result.then((data) => {
                            if (data == '') {
                                return;
                            }
                            this.setValue(day, hour, valueId, data);
                        }).catch((data) => {
                        });
                    });
            }
        }
    }

    changeValues(day, hour, mode) {
        let val = this.dhValue(day, hour);
        if (val == null) {
            return;
        }
        if (this.isEnum()) {
            val = val.id;
        }
        if (mode == 1) {
            for(let i = hour; i < 24; i++) {
                this.setValue(day, i + 1, val, val);
            }
        } else if (mode == 2) {
            for(let i = day; i < 7; i++) {
                this.setValue(i + 1, hour, val, val);
            }
        } else if (mode == 3) {
            this.setValue(day, hour + 1, val, val);
        } else if (mode == 4) {
            this.setValue(day + 1, hour, val, val);
        }
    }

    isEnum() {
        if (this.element == null) {
            return false;
        }
        if (this.element.enumValues == null) {
            return false;
        }
        if (this.element.enumValues.length == 0) {
            return false;
        }
        return true;
    }

    cancel() {
        this.schedule = new  ScheduleModel();
        this.load();
    }
}
