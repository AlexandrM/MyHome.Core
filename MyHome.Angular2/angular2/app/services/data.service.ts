import { Injectable, Output } from '@angular/core';
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

	//get all elements
	public elements = new BehaviorSubject<Array<PresetElementModel>>(new Array<PresetElementModel>());
	//get single element
	private elementItems = new Map<string, BehaviorSubject<PresetElementModel>>();

	public elementEnums = new BehaviorSubject<Array<PresetElementEnumModel>>(new Array<PresetElementEnumModel>());
	public elementValues = new BehaviorSubject<Array<ElementItemValueModel>>(new Array<ElementItemValueModel>());


	constructor(
		private manageHubService: ManageHubService,
		private elementService: ElementService,
		private elementItemEnumService: ElementItemEnumService,
		
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
		let c = this.elements.value.find(v => v.id == value.elementItemId);
		if (c != null) {
			c.value = value;
			this.getElement(value.elementItemId).next(c);
		}
	}

	getElements(): BehaviorSubject<Array<PresetElementModel>> {
		return this.elements;
	}

	getElement(id: string): BehaviorSubject<PresetElementModel> {
		let item = this.elementItems.get(id);
		if (item == null) {
			let element = this.elements.value.find((v, i) => v.id == id);
			if (element == null) {
				element = new PresetElementModel();
			}
			item = new BehaviorSubject<PresetElementModel>(element);
			this.elementItems.set(id, item);
		} else {
		}
		return item;
	}

	reloadElements() {
		this.elementService.query().subscribe((res: Array<PresetElementModel>) => {
			let a = new Array<PresetElementModel>();
			res.map(v => { 
				a.push(v); 
				v.items.map(v2 => {
					v2.parent = v;
					a.push(v2);
				})
			});

			a.map(v => {
				this.getElement(v.id).next(v);
			});
			this.elements.next(a);
			this.manageHubService.getLastElementItemValuesModel(a.map(v => v.id));
		});
	}

	changeValue(value: ElementItemValueModel) {
		this.manageHubService.tryChangeElementItemValue(value);
	}
}