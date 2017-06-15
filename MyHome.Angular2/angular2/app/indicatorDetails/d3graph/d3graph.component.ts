import { Component, OnInit, Input, ElementRef } from '@angular/core';
import { D3Service, D3, Selection } from 'd3-ng2-service';

import { PresetElementModel } from '../../models';

@Component({
    selector: 'd3graph',
    templateUrl: './d3graph.component.html',
    styleUrls: ['./d3graph.component.css']
})
export class D3Graph implements OnInit {

    private d3: D3;
    private parentNativeElement: any;

    constructor(element: ElementRef, d3Service: D3Service) {
        this.d3 = d3Service.getD3();
        this.parentNativeElement = element.nativeElement;
    }

    ngOnInit() {
        let d3 = this.d3;
        let d3ParentElement: Selection<any, any, any, any>; 
    }

    ngOnChanges() {
    }
}
