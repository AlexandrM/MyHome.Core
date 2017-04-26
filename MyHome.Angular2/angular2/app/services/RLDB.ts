interface IRLItem {
	id: string;
}

interface IRLDb {
	items: Map<string, IRLItem>;
}


export class RLDb implements IRLDb {

	public static instance = new RLDb();

	public items = new Map<string, IRLItem>();
	public types = new Map<string, Map<string, IRLItem>>();

	private getType(item: IRLItem) {
		return item.constructor.name;
	}

	public set(item: IRLItem): number {
		this.items.set(item.id, item);

		let type = this.getType(item);
		let list = this.types.get(type);
		if (list == null) {
			list = new Map<string, IRLItem>();
			this.types.set(type, list);
		}
		list.set(item.id, item);

		return 0;
	}

	public deleteById(id: string): number {
		let item = this.items.get(id);
		let type = this.getType(item);
		this.items.delete(id);
		let list = this.types.get(type);
		list.delete(id);

		return 0;
	}

	public delete(item: IRLItem): number {
		return this.deleteById(item.id);
	}

	private propertyOf = <TObj>(name: keyof TObj) => name;

	private propertyNamesOf = <TObj>() => (name: keyof TObj) => name;

	private isRLItemProperty(property: string): boolean {
		return false;
	}

	private instanceOfA(object: any): object is IRLItem {
		if (object instanceof Object) {
    		return 'id' in object;
		}
		return false;
	}

	public getItem(id: string): IRLItem {
		let item = this.items.get(id);
		let array = Object.getOwnPropertyNames(item);
		//console.log(array);
		array.forEach((k, v) => {
			let val = (<any>item)[k];
			if (this.instanceOfA(val)) {
				//(<any>item)[k] = this.getItem(val.id);
			}
			//console.log(k, (<any>item)[k], this.instanceOfA(val));
		});
		//console.log(propertyOf<array>);
		return ;
	}

	public getItems(type: string): Array<IRLItem> {
		return Array.from(this.types.get(type).values());
	}

	public debug(mode) {
		if (mode == 0) {
			for(let i = 0; i < 5; i++) {
				if (i % 2 == 0) {
					let item1 = new RLItemTest1();
					item1.id = '00' + i; 
					item1.name = 'name: ' + i;
					item1.sub1 = new RLItemTest3();
					item1.sub1.id = '001';
					item1.sub1.sub = new RLItemTest3();
					item1.sub1.sub.id = '002';
					this.set(item1);
				} else {
					let item2 = new RLItemTest2();
					item2.id = '00' + i; 
					item2.value = 'value: ' + i;
					this.set(item2);
				}
			}
			this.set({ id: 'CUSTOM_1'});
			this.set({ id: 'CUSTOM_2'});
			this.set({ id: 'CUSTOM_2'});
		}
		if (mode == 1) {
			console.log('items', this.items);
		}
		if (mode == 2) {
			console.log('types', this.types);
		}
		if (mode == 3) {
			this.items.forEach((v, k, m) => {
				console.log(k, this.getItem(k));
			})
		}
		if (mode == 4) {
			this.types.forEach((v, k, m) => {
				console.log('---' + k, v);
				v.forEach((v2, k2, m2) => {
					console.log('   ' + k2, v2);
				});
			});
		}
		if (mode == 5) {
		}
	}
}

export class RLItem implements IRLItem  {
	id: string;
}

export class RLItemTest1 implements IRLItem  {
	id: string;
	name: string;
	sub1: RLItemTest3;
	sub2: RLItemTest3;
}


export class RLItemTest2 implements IRLItem  {
	id: string;
	value: string;
	sub1: RLItemTest3;
}

export class RLItemTest3  {
	id: string;
	value: string;
	sub: RLItemTest3;
}

