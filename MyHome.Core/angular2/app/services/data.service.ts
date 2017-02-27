import { Injectable, Output } from '@angular/core';
import 'signalr/jquery.signalR.min';
import { EventEmitter } from '@angular/core';
import { Subject } from 'rxjs/Subject';

import { PresetElementModel, ElementItemValueModel, PresetElementEnumModel } from './../models';
import { ManageHubService } from './manageHub.service';
import { ElementService } from './element.service';
import { ElementItemEnumService } from './elementItemEnum.service';

@Injectable()
export class DataService {

	public elements = Array<PresetElementModel>();
	public elementEnums = Array<PresetElementEnumModel>();

	onRefresh = new EventEmitter<ElementItemValueModel>();

	constructor(
		private manageHubService: ManageHubService,
		private elementService: ElementService,
		private elementItemEnumService: ElementItemEnumService,
		
	) {
        this.manageHubService.onGetLastElementItemValues.subscribe((values) => {
            values.map((a) => {
                let e = this.findElement(a.elementItemId);
                if (e != null) {
                    e.value = a;
                }
            });
			this.onRefresh.emit(null);
        });

        this.manageHubService.onAfterChangeElementItemValue.subscribe((value) => {
			let e = this.findElement(value.elementItemId);
			if (e != null) {
				e.value = value;
			}
			this.onRefresh.emit(value);
        });

		this.manageHubService.onConnected.subscribe(() => {
			this.reloadElements();
		});
	}

	reloadElements() {
		this.elementService.query().subscribe(res => {
			this.elements = res;
			this.elementItemEnumService.query().subscribe(res => {
				this.elementEnums = res;
				let ids = [].concat.apply([], this.elements.map(function (a) { return a.items.map((a) => { return a.id }); }));
				this.manageHubService.getLastElementItemValuesModel(ids);
			});
		});
	}

	findElement(id: string): PresetElementModel {
		for (let i = 0; i < this.elements.length; i++) {
			let f = this.elements[i].items.find((v, i) => { return v.id == id; });
			if (f != null) {
				return f;
			}
		}
		return null;
	}

	changeValue(value: ElementItemValueModel) {
		this.manageHubService.tryChangeElementItemValue(value);
	}
}