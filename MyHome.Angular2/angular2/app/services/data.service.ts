import { Injectable, Output, NgZone } from '@angular/core';
import 'signalr/jquery.signalR.min';
import { EventEmitter } from '@angular/core';
import { Subject } from 'rxjs/Subject';
import { Observable } from 'rxjs/Observable';
import { BehaviorSubject } from 'rxjs/BehaviorSubject';

import { PresetElementModel, ElementItemValueModel, PresetElementEnumModel } from './../models';
import { ManageHubService } from './manageHub.service';
import { ElementService } from './element.service';
import { ElementItemEnumService } from './elementItemEnum.service';

@Injectable()
export class DataService {

	//public elements = new BehaviorSubject<Map<string, BehaviorSubject<PresetElementModel>>>(new Map<string, BehaviorSubject<PresetElementModel>>());
	public elementItems = new Map<string, BehaviorSubject<PresetElementModel>>();
	public elements = new BehaviorSubject<Map<string, BehaviorSubject<PresetElementModel>>>(this.elementItems);

	public elementEnums = new BehaviorSubject<Array<PresetElementEnumModel>>(new Array<PresetElementEnumModel>());
	public elementValues = new BehaviorSubject<Array<ElementItemValueModel>>(new Array<ElementItemValueModel>());


	constructor(
		private manageHubService: ManageHubService,
		private elementService: ElementService,
		private elementItemEnumService: ElementItemEnumService,
		private ngZone: NgZone
		
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
	}

	updateValue(value: ElementItemValueModel) {
		let item = this.elements.value.get(value.elementItemId);
		if (item != null) {
			item.value.value = value;
			item.next(item.value);
		}
	}

	reloadElements() {
		this.elementService.query().subscribe((res: Array<PresetElementModel>) => {
			//let map = new Map<string, BehaviorSubject<PresetElementModel>>();
			let map = this.elements.value;
			map.clear();
			res.map(parent => {
				map.set(parent.id, new BehaviorSubject<PresetElementModel>(parent));
				parent.items.map(item => {
					item.parent = parent;
					map.set(item.id, new BehaviorSubject<PresetElementModel>(item));
				});
			});
			//this.ngZone.run(() => {
				this.elements.next(map);
			//});
		});
	}

	changeValue(value: ElementItemValueModel) {
		this.manageHubService.tryChangeElementItemValue(value);
	}
}