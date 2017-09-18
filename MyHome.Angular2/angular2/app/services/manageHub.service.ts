import { Injectable } from '@angular/core';
import { HttpConnection, HubConnection, ConsoleLogger, LogLevel } from '@aspnet/signalr-client/';
import { EventEmitter } from '@angular/core';
import { Observable, Subscription } from 'rxjs/Rx';

import { ElementItemValueModel } from 'app/models/models';

import { environment } from 'environments/environment';

declare var $:any;

@Injectable()
export class ManageHubService {
	private hub: HubConnection;
    private iRecconect: Subscription;

	public connectionExists: Boolean;

	public onConnected: EventEmitter<void> = new EventEmitter<void>();
    public onChangeElementItemValue: EventEmitter<void> = new EventEmitter<void>();
    public onAfterChangeElementItemValue: EventEmitter<ElementItemValueModel> = new EventEmitter<ElementItemValueModel>();
    public onGetLastElementItemValues: EventEmitter<ElementItemValueModel[]> = new EventEmitter<ElementItemValueModel[]>();

 	constructor() {
        let conn = new HttpConnection(`${SIGNALR_URL}` + '/manageHub');

        let self = this;
        /*conn.stateChanged(state => {
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
        });*/
        this.hub = new HubConnection(conn);
                
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
        this.hub.start().catch(err => console.log('SignalR.startConnection() error!'));
    }

    public getLastElementItemValueModel(id: string) {
        this.hub.invoke('getLastElementItemValue', id).then(function (data){
        });
    }

    public getLastElementItemValuesModel(ids: string[]) {
        let self = this;
        this.hub.invoke('getLastElementItemValues', ids).then(function (data){
            self.onGetLastElementItemValues.emit(data);
        });
    }

    public tryChangeElementItemValue(value: ElementItemValueModel) {
        this.hub.invoke('tryChangeElementItemValue', value).then(function (data){
        });
    }
}