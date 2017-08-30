import { PresetElementModel } from 'app/models';

/*export interface IDashboardModel {
    rows: IDashboardRowModel[];
}*/

export interface IDashboardRowModel {
    order: number;
    columns: IDashboardColumnModel[];
}

export interface IDashboardColumnModel {
    order: number;
    size: number;
    elements: PresetElementModel[];
}

export interface IDashboardElementModel1 {
    id: string;
    parent: IDashboardElementModel1;
    name: string;
    viewType: string;
    items: Array<IDashboardElementModel1>;
}