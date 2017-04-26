export interface IDashboardModel {
    rows: IDashboardRowModel[]
}

export interface IDashboardRowModel {
    order: number;
    columns: IDashboardColumnModel[];
}

export interface IDashboardColumnModel {
    order: number;
    size: number;
    elements: IDashboardElementModel[]
}

export interface IDashboardElementModel {
    id: string;
    name: string;
    type: string;
    items: Array<IDashboardElementModel>;
}