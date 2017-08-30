import { Injectable, Output, NgZone } from '@angular/core';
import 'signalr/jquery.signalR.min';
import { EventEmitter } from '@angular/core';
import { Subject } from 'rxjs/Subject';
import { Observable } from 'rxjs/Observable';
import { BehaviorSubject } from 'rxjs/BehaviorSubject';

import { Presets, PresetModel, PresetElementModel, ElementItemValueModel, PresetElementEnumModel } from './../models';
import { ManageHubService } from './manageHub.service';
import { ElementService } from './element.service';
import { ElementItemEnumService } from './elementItemEnum.service';
import { SettingService } from './setting.service';


@Injectable()
export class DataService {

	public elements = new BehaviorSubject<Map<string, BehaviorSubject<PresetElementModel>>>(new Map<string, BehaviorSubject<PresetElementModel>>());

	public elementEnums = new BehaviorSubject<Array<PresetElementEnumModel>>(new Array<PresetElementEnumModel>());
	public elementValues = new BehaviorSubject<Array<ElementItemValueModel>>(new Array<ElementItemValueModel>());

	public presets = new BehaviorSubject<Presets>(new Presets());

	constructor(
		private manageHubService: ManageHubService,
		private elementService: ElementService,
		private elementItemEnumService: ElementItemEnumService,
		private ngZone: NgZone,
		private settingService: SettingService,
		
	) {
        this.manageHubService.onGetLastElementItemValues.subscribe((values) => {
			values.map(value => {
				this.updateValue(value);
			});
        });

        this.manageHubService.onAfterChangeElementItemValue.subscribe((value) => {
			this.updateValue(value);
        });

		this.manageHubService.onConnected.subscribe(() => {
			this.reloadElements();
		});
		
		this.reloadElements();
		this.presetload();
	}

	updateValue(value: ElementItemValueModel) {
		let item = this.elements.value.get(value.elementItemId);
		if (item != null) {
			item.value.value = value;
			this.ngZone.run(() => {
				item.next(item.value);
			});
		}
	}

	reloadElements() {
		this.elementService.query().subscribe((res: Array<PresetElementModel>) => {
			let map = new Map<string, BehaviorSubject<PresetElementModel>>();
			map.clear();
			res.map(parent => {
				map.set(parent.id, new BehaviorSubject<PresetElementModel>(parent));
				parent.items.map(item => {
					item.parent = parent;
					item.value = (item as any).values != null ? (item as any).values[0] : null;
					map.set(item.id, new BehaviorSubject<PresetElementModel>(item));
				});
			});
			this.ngZone.run(() => {
				this.elements.next(map);
			});
		});
	}

	changeValue(value: ElementItemValueModel) {
		this.manageHubService.tryChangeElementItemValue(value);
	}

	presetload() {
		this.settingService.query('presets').subscribe((data: Response) => {
			let presets = new Presets();
			presets.list = new Array<PresetModel>();
            for(let idx in data) {
				presets.list.push({
					id: data[idx].id,
					name: data[idx].name,
					rows: JSON.parse(data[idx].value)
				});
			}

			this.settingService.get('CurrentPreset').subscribe((data: Response) => {
				presets.current = presets.list.find(x => x.id == data[0].value);
				this.presets.next(presets);
            });
        });		
	}

	presetSave(model: PresetModel, reload: boolean) {
		let copy = model.rows.map(c => {return { 
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
		
        return this.settingService.post({
            id: model.id,
            name: model.name,
            group: 'presets',
            value: JSON.stringify(copy)
        }).subscribe(x => {
			if (reload) {
				this.presetload();
			}
		});		
	}
}