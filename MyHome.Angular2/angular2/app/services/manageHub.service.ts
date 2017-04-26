import { Injectable } from '@angular/core';
import 'signalr/jquery.signalR.min';
import { EventEmitter } from '@angular/core';
import { Observable, Subscription } from 'rxjs/Rx';

import { ElementItemValueModel } from './../models';

import { environment } from 'environments/environment';

declare var $:any;

@Injectable()
export class ManageHubService {
	private hub: any;
    private server: any;
    private client: any;
    private iRecconect: Subscription;

	public connectionExists: Boolean;

	public onConnected: EventEmitter<void> = new EventEmitter<void>();
    public onChangeElementItemValue: EventEmitter<void> = new EventEmitter<void>();
    public onAfterChangeElementItemValue: EventEmitter<ElementItemValueModel> = new EventEmitter<ElementItemValueModel>();
    public onGetLastElementItemValues: EventEmitter<ElementItemValueModel[]> = new EventEmitter<ElementItemValueModel[]>();

 	constructor() {
        let conn = $.hubConnection(`${SIGNALR_URL}`, { useDefaultPath: false });

        let self = this;
        conn.stateChanged(state => {
            switch (state.newState) {
                case $.signalR.connectionState.connecting:
                    break;
                case $.signalR.connectionState.connected:
                    if (this.iRecconect != null) {
                        this.iRecconect.unsubscribe();
                    }
                    self.connectionExists = true;
                    self.onConnected.emit();
                    break;
                case $.signalR.connectionState.reconnecting:
                    break;
                case $.signalR.connectionState.disconnected:
                    if (this.iRecconect != null) {
                        this.iRecconect.unsubscribe();
                    }
                    this.iRecconect = Observable
                        .interval(5 * 1000)
                        .subscribe(() => {
                            this.startConnection();
                        });
                    break;
            }
        });
        this.hub = conn.createHubProxy('manageHub');

        this.registerOnServerEvents();
	    this.startConnection();
 	}

    private registerOnServerEvents(): void {
        this.hub.on('onChangeElementItemValue', (data) => {
        });
        this.hub.on('onAfterChangeElementItemValue', (data) => {
            this.onAfterChangeElementItemValue.emit(data);
        });
    }

	private startConnection(): void {
        let self = this;
        this.hub.connection.start().done((data: any) => {
        }).fail((error) => {
            console.log('Could not connect ' + error);
        });
    }

    public getLastElementItemValueModel(id: string) {
        this.hub.invoke('getLastElementItemValue', id).done(function (data){
        });
    }

    public getLastElementItemValuesModel(ids: string[]) {
        let self = this;
        this.hub.invoke('getLastElementItemValues', ids).done(function (data){
            self.onGetLastElementItemValues.emit(data);
        });
    }

    public tryChangeElementItemValue(value: ElementItemValueModel) {
        this.hub.invoke('tryChangeElementItemValue', value).done(function (data){
        });
    }
}