import { Component, ViewContainerRef, NgZone } from '@angular/core';
import { Response } from '@angular/http';
import { DropdownModule } from 'ng2-bootstrap/dropdown';
import { Overlay } from 'angular2-modal';
import { Modal } from 'angular2-modal/plugins/bootstrap';

import { DashboardComponent } from './dashboard/dashboard.component';

import { Presets, PresetModel, PresetRowModel, PresetColumnModel, PresetElementModel } from 'app/models';
import { SettingService } from 'app/services/setting.service'
import { ElementService } from 'app/services/element.service'
import { ManageHubService } from 'app/services/manageHub.service'
import { DataService } from 'app/services/data.service'
import { RLDb } from 'app/services/RLDB'


@Component({
    selector: 'home',  // <home></home>
    providers: [
        SettingService,
        ElementService,
    ],
    styleUrls: [
        //'./home.component.css' 
    ],
    templateUrl: './home.component.html',
})
export class HomeComponent {

    presets = new Presets();
    elements = Array<PresetElementModel>();

    constructor(
        private settingService: SettingService,
        private elementService: ElementService,
        private manageHubService: ManageHubService,
        private dataService: DataService,
        private ngZone: NgZone,
        overlay: Overlay,
        vcRef: ViewContainerRef,
        private modal: Modal
    ) {
        overlay.defaultViewContainer = vcRef;

        this.dataService.getElements().subscribe(v => {
            this.elements  = v;
        });
    }

    load() {
        //this.ngZone.run(() => {            
            this.presets.load(this.settingService);
            this.presets.current = new PresetModel();
        //});
    }

    ngOnInit() {
        this.load();
    }

    vm = {
        editMode: false,
    }

    rootViewModel = {
        Name: '',
    }

    addRow() {
        if (this.presets.current.rows == null) {
            this.presets.current.rows = new Array<PresetRowModel>();
        }
        this.presets.current.rows.push(new PresetRowModel());
    }

    editMode(mode?: number) {
        if (!this.vm.editMode) {
            this.vm.editMode = true;
            return;
        }
        if (mode == null) {
            this.vm.editMode = false;
            this.presets.load(this.settingService);
            return;
        }
        if (mode == 1) {
            this.presets.current.save(this.settingService).subscribe(res => {
                this.vm.editMode = false;
            });
        } else if (mode == 2) {
            this.modal.prompt()
                .size('sm')
                .isBlocking(true)
                .showClose(true)
                .keyboard(27)
                .title('Имя представления')
                .body('Введите имя')
                .okBtn('Ок').okBtnClass('btn btn-raised btn-primary')
                .cancelBtn('Отмена').cancelBtnClass('btn btn-raised btn-warning')
                .open()
                .then((dr) => {
                    dr.result.then((data) => {
                        if (data == '') {
                            return;
                        }
                        this.vm.editMode = false;
                        this.presets.current.id = '';
                        this.presets.current.name = data;
                        this.presets.current.save(this.settingService).subscribe(res => {
                            this.presets.current.id = res.id;
                            this.presets.setCurrent(this.settingService).subscribe(res2 => {
                                this.presets.load(this.settingService);
                            });
                        });
                    }).catch((data) => {
                    });
                });
        } else if (mode == 3) {
            this.presets.current.delete(this.settingService);
            this.vm.editMode = false;
        }
    }

    setPreset(preset: PresetModel) {
        this.presets.current = preset;
        this.presets.setCurrent(this.settingService);
    }

    deletePreset(preset: PresetModel) {
        this.modal.confirm()
            .size('sm')
            .isBlocking(true)
            .showClose(true)
            .keyboard(27)
            .title('Подтверждение')
            .body('Удалить ' + preset.name + '?')
            .okBtn('Ок').okBtnClass('btn btn-raised btn-danger')
            .cancelBtn('Отмена').cancelBtnClass('btn btn-raised btn-primary')
            .open()
            .then((dr) => {
                dr.result.then((data) => {
                    this.presets.current.delete(this.settingService).subscribe(res => {
                        this.presets.load(this.settingService);
                        this.presets.current = new PresetModel();
                    });
                }).catch((data) => {
                });
            });
    }
}
