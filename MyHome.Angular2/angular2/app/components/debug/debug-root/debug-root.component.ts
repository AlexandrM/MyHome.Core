import { Component, OnInit } from '@angular/core';
import { NgZone } from '@angular/core';
import { DebugService } from 'app/services/debug.service'

@Component({
  selector: 'app-debug-root',
  templateUrl: './debug-root.component.html',
  styleUrls: ['./debug-root.component.css']
})
export class DebugRootComponent implements OnInit {

  private items = new Array<string>();

  constructor(
    private debugService: DebugService,
		private ngZone: NgZone,
  ) {     
  }

  ngOnInit() {
    this.debugService.query().subscribe(data => {
      this.items = data.list;
    });
  }
}
