import { Component, Input, NgZone } from '@angular/core'
import { D3Service } from 'd3-ng2-service'

import { Presets, PresetModel, PresetRowModel, PresetColumnModel, PresetElementModel } from 'app/models';
import { DataService } from 'app/services/data.service'
import { ManageHubService } from 'app/services/manageHub.service'
//import { SettingService } from 'app/services/setting.service'

@Component({
    selector: 'indicatorDetails',
    providers: [
        D3Service,
    ],
    styleUrls: [
        './indicatorDetails.component.css'
    ],
    templateUrl: './indicatorDetails.component.html'
})
export class IndicatorDetailsComponent {

    public elements = Array<PresetElementModel>();
    //public presets = new Presets();
    
    public dateFrom = new Date();
    public dateTo = new Date();
    public dateToToday = true;

    constructor(
        private d3Service: D3Service,
        private dataService: DataService,
        private manageHubService: ManageHubService,
        private ngZone: NgZone,
        //private settingService: SettingService,
    ) {
        dataService.onRefresh.subscribe(() => {
            this.elements = this.dataService.elements;
        });

        manageHubService.onConnected.subscribe(() =>{
            this.load();
        });
    }

    load() {
        this.ngZone.run(() => {            
            if (this.dataService.elements.length == 0) {
                this.dataService.reloadElements();
            } else {
                this.elements = this.dataService.elements;
            }
        });
    }

    ngOnInit() {
    }

    refresh() {
        console.log(this.dateFrom);
        console.log(this.dateTo);
    }
}
