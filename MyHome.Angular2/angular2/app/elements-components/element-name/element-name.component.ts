import { Component, OnInit, Input } from '@angular/core';
import { Modal } from 'angular2-modal/plugins/bootstrap';

import { PresetElementModel } from './../../models';
import { ElementItemService } from './../../services/elementItem.service';

@Component({
    selector: 'element-name',
    templateUrl: './element-name.component.html',
    styleUrls: ['./element-name.component.css']
})
export class ElementNameComponent implements OnInit {

    @Input()
    element: PresetElementModel;

    constructor(
        private modal: Modal,
        private elementItemService: ElementItemService,
    ) {
    }

    ngOnInit() {
    }

    edit() {
        this.modal.prompt()
            .defaultValue(this.element.name)
            .size('sm')
            .isBlocking(true)
            .showClose(true)
            .keyboard(27)
            .title('Новое наименование')
            .body('Наименование')
            .okBtn('Ок').okBtnClass('btn btn-raised btn-primary')
            .cancelBtn('Отмена').cancelBtnClass('btn btn-raised btn-warning')
            .open()
            .then((dr) => {
                dr.result.then((data) => {
                    if (data == '') {
                        return;
                    }
                    this.element.name = data;
                    this.elementItemService.post(this.element).subscribe(r => {
                        console.log(r);                        
                        this.element.name = r.name;
                    });
                }).catch((data) => {
                });
            });
    }
}
