//import { IPresetModel, IPresetRowModel, IPresetColumnModel } from './dashboard/interfaces';
import { Response } from '@angular/http';
import { SettingService } from 'app/services/setting.service'
import { ElementService } from 'app/services/element.service'

export class PresetModel {

    id: string;
    name: string;
    rows = new Array<PresetRowModel>();

    /*save(settingService: SettingService) {
        let copy = this.rows.map(c => {return { 
            columns: c.columns == null ? null : c.columns.map(e => {
                return {
                    order: e.order,
                    size: e.size,
                    elements: e.elements.map(i => {
                        return {
                            id: i.id,
                            type: i.type,
                            viewType: i.viewType,
                        }
                    })
                }
            })
        }});
        console.log(this.rows);
        console.log(copy);
        
        return settingService.post({
            id: this.id,
            name: this.name,
            group: 'presets',
            value: JSON.stringify(copy)
        });
    };
    delete(settingService: SettingService) {
        return settingService.delete(this.id);
    };*/
}

export class PresetRowModel {
    order: number;
    columns: PresetColumnModel[];
}

export class PresetColumnModel {
    order: number;
    size: number;
    elements: Array<PresetElementModel>;
}

export class Presets {
    list = new Array<PresetModel>();
    current: PresetModel;
    name: string;

    public load(settingService: SettingService): void {
        /*settingService.query('presets').subscribe((data: Response) => {
            this.list = new Array<PresetModel>();
            for(let idx in data) {
                let item = data[idx];
                let pm = new PresetModel();
                pm.id = item.id;
                pm.name = item.name;
                if (item.value != null) {
                    pm.rows = JSON.parse(item.value);
                }
                this.list.push(pm);
            }   
            this.getCurrent(settingService).subscribe(res => {
                for(let idx in this.list) {
                    if (this.list[idx].id == res[0].value) {
                        this.current = this.list[idx];
                        break;
                    }
               } 
            });
        });*/
    }

    public setCurrent(settingService: SettingService) {
        return settingService.post({
            id: 'CurrentPreset',
            group: 'General',
            name: 'CurrentPreset',
            value: this.current.id
        });
    }

    public getCurrent(settingService: SettingService) {
        return settingService.get('CurrentPreset', null);
    }
}

export class PresetElementModel {
    id: string;
    parent: PresetElementModel;
    name: string;
    description: string;
    type: string;
    allowSchedule: boolean;
    modeId: string;
    items: Array<PresetElementModel>;
    value: ElementItemValueModel;
    enumValues: PresetElementEnumModel[];

    viewType: string;
}

export class ElementItemValueModel {
    dateTime: Date;
    updated: Date;
    elementItem?: PresetElementModel;
    elementItemId: string;
    rawValue: string;
    valueId: string;
    value?: PresetElementEnumModel;
}

export class SettingModel {
    id: string;
    group: string;
    name: string;
    value: string;
}

export class PresetElementEnumModel {
    id: string;
    elementItemId: string;
    value: string;
    name: string;
}

export class ScheduleModel {
    id: string;
    elementItemId: string;
    name: string;
    scheduleHours: ScheduleHourModel[]
}

export class ScheduleHourModel {
    scheduleId: string;
    dayOfWeek: number;
    hour: number;
    valueId: string;
    rawValue: string;
}