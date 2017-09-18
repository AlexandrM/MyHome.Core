//import { IPresetModel, IPresetRowModel, IPresetColumnModel } from './dashboard/interfaces';
import { Response } from '@angular/http';
import { SettingService } from 'app/services/setting.service'
import { ElementService } from 'app/services/element.service'

export class PresetModel {

    id: string;
    name: string;
    rows = new Array<PresetRowModel>();
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
    valueId: string;
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